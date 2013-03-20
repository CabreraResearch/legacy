using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Security;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassRole : CswNbtObjClass
    {
        public const string ChemSWAdminRoleName = CswAuthenticator.ChemSWAdminRoleName;

        public sealed class PropertyName
        {
            public const string Administrator = "Administrator";
            public const string Description = "Description";
            public const string NodeTypePermissions = "NodeType Permissions";
            public const string ActionPermissions = "Action Permissions";
            public const string Timeout = "Timeout";
            public const string Name = "Name";
        }


        public const string ActionPermissionsXValueName = CswNbtAction.PermissionXValue;


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassRole( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RoleClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassRole
        /// </summary>
        public static implicit operator CswNbtObjClassRole( CswNbtNode Node )
        {
            CswNbtObjClassRole ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.RoleClass ) )
            {
                ret = (CswNbtObjClassRole) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            // The user cannot change his or her own Administrator privileges.
            if( Administrator.WasModified && 
                Administrator.Checked != CswConvert.ToTristate(Administrator.GetOriginalPropRowValue()) &&
                _CswNbtResources.CurrentUser.RoleId == _CswNbtNode.NodeId )
            {
                _CswNbtNode.Properties.clearModifiedFlag();  // prevents multiple error messages from appearing if we attempt to write() again
                throw new CswDniException( ErrorType.Warning, "You may not change your own administrator status", "User (" + _CswNbtResources.CurrentUser.Username + ") attempted to edit the Administrator property of their own Role" );
            }

            // case 22512
            // also case 22557 - use the original name, not the new one
            CswNbtNodePropWrapper NamePropWrapper = Node.Properties[PropertyName.Name];
            if( NamePropWrapper.GetOriginalPropRowValue( _CswNbtResources.MetaData.getFieldTypeRule( NamePropWrapper.getFieldTypeValue() ).SubFields.Default.Column ) == ChemSWAdminRoleName &&
                _CswNbtResources.CurrentNbtUser.Username != CswNbtObjClassUser.ChemSWAdminUsername &&
                false == ( _CswNbtResources.CurrentNbtUser is CswNbtSystemUser ) )
            {
                throw new CswDniException( ErrorType.Warning, "The " + ChemSWAdminRoleName + " role cannot be edited", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to edit the '" + ChemSWAdminRoleName + "' role." );
            }

            // case 22437
            if( ActionPermissions.WasModified )
            {
                // case 25444 - was it *really* modified?
                CswNbtNodePropWrapper ActionPermissionsPropWrapper = Node.Properties[PropertyName.ActionPermissions];
                string ActionPermissionsOriginalValueStr = ActionPermissionsPropWrapper.GetOriginalPropRowValue( ( (CswNbtFieldTypeRuleMultiList) _CswNbtResources.MetaData.getFieldTypeRule( ActionPermissionsPropWrapper.getFieldTypeValue() ) ).ValueSubField.Column );
                CswCommaDelimitedString ActionPermissionsOriginalValue = new CswCommaDelimitedString();
                ActionPermissionsOriginalValue.FromString( ActionPermissionsOriginalValueStr );

                if( ActionPermissions.Value != ActionPermissionsOriginalValue )
                {
                    // You can never grant your own action permissions
                    if( _CswNbtResources.CurrentUser.RoleId == _CswNbtNode.NodeId && this.Name.Text != ChemSWAdminRoleName )
                    {
                        // case 26346 - we might be trimming invalid actions out automatically, 
                        // so just throw if an action permissions is being ADDED, not removed
                        bool ActionAdded = false;
                        foreach( string ActionStr in ActionPermissions.Value )
                        {
                            if( false == ActionPermissionsOriginalValue.Contains( ActionStr ) )
                            {
                                ActionAdded = true;
                            }
                        }

                        if( ActionAdded )
                        {
                            throw new CswDniException( ErrorType.Warning, "You may not grant access to actions for which you have no permissions",
                                "User (" + _CswNbtResources.CurrentUser.Username + ") attempted to edit their own action permissions on role: " + _CswNbtNode.NodeName );
                        }
                    }
                    // You can only grant action permissions on other roles to which you have access
                    foreach( CswNbtAction Action in _CswNbtResources.Actions )
                    {
                        if( true == _CswNbtResources.Permit.can( Action, this ) ) // permission is being granted
                        {
                            if( ( Action.Name == CswNbtActionName.Design ||
                                    Action.Name == CswNbtActionName.Create_Inspection || //Case 24288
                                    Action.Name == CswNbtActionName.View_Scheduled_Rules ) && //Case 28564
                                    _CswNbtResources.CurrentNbtUser.Rolename != ChemSWAdminRoleName &&  //Case 28433: chemsw_admin can grant Design to anyone.
                                    false == _CswNbtResources.IsSystemUser
                                )
                            {
                                // case 23677
                                throw new CswDniException( ErrorType.Warning, "You may not grant access to " + Action.DisplayName + " to this role",
                                    "User (" + _CswNbtResources.CurrentUser.Username + ") attempted to grant access to action " + Action.DisplayName + " to role " + _CswNbtNode.NodeName );
                            }
                            /* Case 24447 */
                            if( Action.Name == CswNbtActionName.Create_Material )
                            {
                                CswNbtMetaDataObjectClass MaterialOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialClass );

                                bool HasOneMaterialCreate = false;
                                foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getNodeTypes() )
                                {
                                    string NodeTypePermission = MakeNodeTypePermissionValue(
                                        MaterialNt.FirstVersionNodeTypeId,
                                        CswNbtPermit.NodeTypePermission.Create );

                                    HasOneMaterialCreate = HasOneMaterialCreate ||
                                                           NodeTypePermissions.CheckValue( NodeTypePermission );
                                }
                                if( false == HasOneMaterialCreate )
                                {
                                    throw new CswDniException( ErrorType.Warning, "You may not grant access to " + Action.DisplayName + " to this role without first granting Create permission to at least one Material.",
                                        "User (" + _CswNbtResources.CurrentUser.Username + ") attempted to grant access to action " + Action.DisplayName + " to role " + _CswNbtNode.NodeName );
                                }

                            }
                            if( false == _CswNbtResources.Permit.can( Action, _CswNbtResources.CurrentNbtUser ) )
                            {
                                throw new CswDniException( ErrorType.Warning, "You may not grant access to actions for which you have no permissions",
                                    "User (" + _CswNbtResources.CurrentUser.Username + ") attempted to grant access to action " + Action.DisplayName + " to role " + _CswNbtNode.NodeName );
                            }
                        } // if( true == _CswNbtResources.Permit.can( Action, this ) )
                    } // foreach( string ActionNameString in ActionPermissions.YValues )
                } // if( ActionPermissions.Value != ActionPermissionsOriginalValue )
            } // if( ActionPermissions.WasModified )

            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();

            // BZ 9170
            _CswNbtResources.ConfigVbls.setConfigVariableValue( "cache_lastupdated", DateTime.Now.ToString() );

        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

            // Prevent deleting your own role
            if( _CswNbtNode.NodeId == _CswNbtResources.CurrentUser.RoleId )
            {
                throw ( new CswDniException( ErrorType.Warning, "You can not delete your own role account.", "Current user (" + _CswNbtResources.CurrentUser.Username + ") can not delete own RoleClass node." ) );
            }

            // case 22635 - prevent deleting the chemsw admin role
            CswNbtNodePropWrapper NamePropWrapper = Node.Properties[PropertyName.Name];
            if( NamePropWrapper.GetOriginalPropRowValue( _CswNbtResources.MetaData.getFieldTypeRule( NamePropWrapper.getFieldTypeValue() ).SubFields.Default.Column ) == ChemSWAdminRoleName &&
                false == ( _CswNbtResources.CurrentNbtUser is CswNbtSystemUser ) )
            {
                throw new CswDniException( ErrorType.Warning, "The '" + ChemSWAdminRoleName + "' role cannot be deleted", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to delete the '" + ChemSWAdminRoleName + "' role." );
            }

            //case 28010 - delete all view assigned to this role
            _CswNbtResources.ViewSelect.deleteViewsByRoleId( NodeId );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public static string MakeNodeTypePermissionValue( Int32 FirstVersionNodeTypeId, CswNbtPermit.NodeTypePermission Permission )
        {
            return "nt_" + FirstVersionNodeTypeId.ToString() + "_" + Permission.ToString();
        }
        public static string MakeNodeTypePermissionText( string LatestVersionNodeTypeName, CswNbtPermit.NodeTypePermission Permission )
        {
            return LatestVersionNodeTypeName + ": " + Permission.ToString();
        }
        public static string MakeNodeTypeTabPermissionValue( Int32 FirstVersionNodeTypeId, Int32 FirstTabVersionID, CswNbtPermit.NodeTypeTabPermission Permission )
        {
            return "nt_" +
                    FirstVersionNodeTypeId.ToString() +
                    "_tab_" +
                    FirstTabVersionID +
                    "_" +
                    Permission.ToString();
        }
        public static string MakeNodeTypeTabPermissionText( string LatestVersionNodeTypeName, string LatestVersionTabName, CswNbtPermit.NodeTypeTabPermission Permission )
        {
            return LatestVersionNodeTypeName +
                   ", " +
                //NodeTypeTab.NodeType.LatestVersionNodeType.getNodeTypeTabByFirstVersionId( NodeTypeTab.FirstTabVersionId ).TabName +
                   LatestVersionTabName +
                   ": " +
                   Permission.ToString();
        }
        public static string MakeActionPermissionValue( CswNbtAction Action )
        {
            return "act_" + Action.ActionId.ToString();
        }
        public static string MakeActionPermissionText( CswNbtAction Action )
        {
            return Action.DisplayName;
        }


        private Dictionary<string, string> InitNodeTypePermissionOptions()
        {
            // set NodeType Permissions options
            // Could be a performance problem!!!
            Dictionary<string, string> NodeTypeOptions = new Dictionary<string, string>();
            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypesLatestVersion() )
            {
                foreach( CswNbtPermit.NodeTypePermission Permission in Enum.GetValues( typeof( CswNbtPermit.NodeTypePermission ) ) )
                {
                    string Key = MakeNodeTypePermissionValue( NodeType.FirstVersionNodeTypeId, Permission );
                    string Value = MakeNodeTypePermissionText( NodeType.NodeTypeName, Permission );
                    NodeTypeOptions.Add( Key, Value );

                }
                foreach( CswNbtMetaDataNodeTypeTab Tab in NodeType.getNodeTypeTabs() )
                {
                    foreach( CswNbtPermit.NodeTypeTabPermission Permission in Enum.GetValues( typeof( CswNbtPermit.NodeTypeTabPermission ) ) )
                    {
                        string Key = MakeNodeTypeTabPermissionValue( NodeType.FirstVersionNodeTypeId, Tab.FirstTabVersionId, Permission );
                        string Value = MakeNodeTypeTabPermissionText( NodeType.NodeTypeName, Tab.TabName, Permission );
                        NodeTypeOptions.Add( Key, Value );

                    }
                } // foreach( CswNbtMetaDataNodeTypeTab Tab in NodeType.NodeTypeTabs )
            } // foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.NodeTypes )
            return NodeTypeOptions;
        } // InitNodeTypePermissionOptions()

        private Dictionary<string, string> InitActionPermissionOptions()
        {
            // set Action Permissions options
            Dictionary<string, string> ActionOptions = new Dictionary<string, string>();
            foreach( CswNbtAction Action in _CswNbtResources.Actions )
            {
                string Key = MakeActionPermissionValue( Action );
                string Value = MakeActionPermissionText( Action );
                ActionOptions.Add( Key, Value );
            }
            return ActionOptions;
        } // InitActionPermissionOptions()

        public override void afterPopulateProps()
        {
            NodeTypePermissions.InitOptions = InitNodeTypePermissionOptions;
            ActionPermissions.InitOptions = InitActionPermissionOptions;

            //case 27793: only an administrator can edit nodes
            if( ( null == _CswNbtResources.CurrentNbtUser ) || ( false == _CswNbtResources.CurrentNbtUser.IsAdministrator() ) )
            {
                this.Node.setReadOnly( true, false );
            }



            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {



            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropLogical Administrator { get { return ( _CswNbtNode.Properties[PropertyName.Administrator] ); } }
        public CswNbtNodePropMemo Description { get { return ( _CswNbtNode.Properties[PropertyName.Description] ); } }
        public CswNbtNodePropMultiList NodeTypePermissions { get { return ( _CswNbtNode.Properties[PropertyName.NodeTypePermissions] ); } }
        public CswNbtNodePropMultiList ActionPermissions { get { return ( _CswNbtNode.Properties[PropertyName.ActionPermissions] ); } }
        public CswNbtNodePropNumber Timeout { get { return ( _CswNbtNode.Properties[PropertyName.Timeout] ); } }
        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }

        #endregion


    }//CswNbtObjClassRole

}//namespace ChemSW.Nbt.ObjClasses
