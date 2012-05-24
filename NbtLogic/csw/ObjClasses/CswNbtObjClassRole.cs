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
        public static string ChemSWAdminRoleName { get { return CswAuthenticator.ChemSWAdminRoleName; } }

        public static string AdministratorPropertyName { get { return "Administrator"; } }
        public static string DescriptionPropertyName { get { return "Description"; } }
        public static string NodeTypePermissionsPropertyName { get { return "NodeType Permissions"; } }
        public static string ActionPermissionsPropertyName { get { return "Action Permissions"; } }
        public static string TimeoutPropertyName { get { return "Timeout"; } }
        public static string NamePropertyName { get { return "Name"; } }

        public static string ActionPermissionsXValueName { get { return CswNbtAction.PermissionXValue; } }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassRole( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassRole
        /// </summary>
        public static explicit operator CswNbtObjClassRole( CswNbtNode Node )
        {
            CswNbtObjClassRole ret = null;
            if( _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass ) )
            {
                ret = (CswNbtObjClassRole) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            // BZ 9170
            _CswNbtResources.ConfigVbls.setConfigVariableValue( "cache_lastupdated", DateTime.Now.ToString() );

            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            // The user cannot change his or her own Administrator privileges.
            if( Administrator.WasModified && _CswNbtResources.CurrentUser.RoleId == _CswNbtNode.NodeId )
            {
                _CswNbtNode.Properties.clearModifiedFlag();  // prevents multiple error messages from appearing if we attempt to write() again
                throw new CswDniException( ErrorType.Warning, "You may not change your own administrator status", "User (" + _CswNbtResources.CurrentUser.Username + ") attempted to edit the Administrator property of their own Role" );
            }

            // case 22512
            // also case 22557 - use the original name, not the new one
            CswNbtNodePropWrapper NamePropWrapper = Node.Properties[NamePropertyName];
            if( NamePropWrapper.GetOriginalPropRowValue( _CswNbtResources.MetaData.getFieldTypeRule( NamePropWrapper.getFieldType().FieldType ).SubFields.Default.Column ) == ChemSWAdminRoleName &&
                _CswNbtResources.CurrentNbtUser.Username != CswNbtObjClassUser.ChemSWAdminUsername &&
                false == ( _CswNbtResources.CurrentNbtUser is CswNbtSystemUser ) )
            {
                throw new CswDniException( ErrorType.Warning, "The " + ChemSWAdminRoleName + " role cannot be edited", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to edit the '" + ChemSWAdminRoleName + "' role." );
            }

            // case 22437
            if( ActionPermissions.WasModified )
            {
                // case 25444 - was it *really* modified?
                CswNbtNodePropWrapper ActionPermissionsPropWrapper = Node.Properties[ActionPermissionsPropertyName];
                string ActionPermissionsOriginalValueStr = ActionPermissionsPropWrapper.GetOriginalPropRowValue( ( (CswNbtFieldTypeRuleMultiList) _CswNbtResources.MetaData.getFieldTypeRule( ActionPermissionsPropWrapper.getFieldType().FieldType ) ).ValueSubField.Column );
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
                                    Action.Name == CswNbtActionName.Create_Inspection ) && //Case 24288
                                  this.Name.Text != ChemSWAdminRoleName &&
                                    false == ( _CswNbtResources.CurrentNbtUser is CswNbtSystemUser ) )
                            {
                                // case 23677
                                throw new CswDniException( ErrorType.Warning, "You may not grant access to " + Action.DisplayName + " to this role",
                                    "User (" + _CswNbtResources.CurrentUser.Username + ") attempted to grant access to action " + Action.DisplayName + " to role " + _CswNbtNode.NodeName );
                            }
                            /* Case 24447 */
                            if( Action.Name == CswNbtActionName.Create_Material )
                            {
                                CswNbtMetaDataObjectClass MaterialOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );

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

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

            // Prevent deleting your own role
            if( _CswNbtNode.NodeId == _CswNbtResources.CurrentUser.RoleId )
            {
                throw ( new CswDniException( ErrorType.Warning, "You can not delete your own role account.", "Current user (" + _CswNbtResources.CurrentUser.Username + ") can not delete own RoleClass node." ) );
            }

            // case 22635 - prevent deleting the chemsw admin role
            CswNbtNodePropWrapper NamePropWrapper = Node.Properties[NamePropertyName];
            if( NamePropWrapper.GetOriginalPropRowValue( _CswNbtResources.MetaData.getFieldTypeRule( NamePropWrapper.getFieldType().FieldType ).SubFields.Default.Column ) == ChemSWAdminRoleName &&
                false == ( _CswNbtResources.CurrentNbtUser is CswNbtSystemUser ) )
            {
                throw new CswDniException( ErrorType.Warning, "The '" + ChemSWAdminRoleName + "' role cannot be deleted", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to delete the '" + ChemSWAdminRoleName + "' role." );
            }

            //// case 22424
            //// Prevent deleting roles in use
            //CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            //foreach( CswNbtNode UserNode in UserOC.getNodes( false, true ) )
            //{
            //    CswNbtObjClassUser UserNodeAsUser = (CswNbtObjClassUser) UserNode;
            //    if( UserNodeAsUser.Role.RelatedNodeId == _CswNbtNode.NodeId )
            //    {
            //        throw ( new CswDniException( ErrorType.Warning, "This role cannot be deleted because it is in use by user: " + UserNodeAsUser.Username,
            //                                     "Current user (" + _CswNbtResources.CurrentUser.Username + ") tried to delete a role that is in use (" + _CswNbtNode.NodeName + ") by user: " + UserNodeAsUser.Username ) );
            //    }
            //}

            ////prevent user from deleting ScheduleRunner
            //if (Name.Text.ToLower() == "schedulerunner")
            //{
            //    throw new CswDniException("You cannot delete the ScheduleRunner role", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to delete the ScheduleRunner role");
            //}

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
            // case 8411 - for backwards compatibility
            //if( _CswNbtNode.Properties[NodeTypePermissionsPropertyName].getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.MultiList )
            //{
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
            //} // if( _CswNbtNode.Properties[NodeTypePermissionsPropertyName].FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.MultiList )
        } // InitNodeTypePermissionOptions()

        private Dictionary<string, string> InitActionPermissionOptions()
        {
            // case 8411 - for backwards compatibility
            //if( _CswNbtNode.Properties[ActionPermissionsPropertyName].getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.MultiList )
            //{
                // set Action Permissions options
                Dictionary<string, string> ActionOptions = new Dictionary<string, string>();
                foreach( CswNbtAction Action in _CswNbtResources.Actions )
                {
                    string Key = MakeActionPermissionValue( Action );
                    string Value = MakeActionPermissionText( Action );
                    ActionOptions.Add( Key, Value );
                }
                return ActionOptions;
            //}
        } // InitActionPermissionOptions()

        public override void afterPopulateProps()
        {
            NodeTypePermissions.InitOptions = InitNodeTypePermissionOptions;
            ActionPermissions.InitOptions = InitActionPermissionOptions;

            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropLogical Administrator
        {
            get
            {
                return ( _CswNbtNode.Properties[AdministratorPropertyName].AsLogical );
            }
        }

        public CswNbtNodePropMemo Description
        {
            get
            {
                return ( _CswNbtNode.Properties[DescriptionPropertyName].AsMemo );
            }
        }

        public CswNbtNodePropMultiList NodeTypePermissions
        {
            get
            {
                return ( _CswNbtNode.Properties[NodeTypePermissionsPropertyName].AsMultiList );
            }
        }

        public CswNbtNodePropMultiList ActionPermissions
        {
            get
            {
                return ( _CswNbtNode.Properties[ActionPermissionsPropertyName].AsMultiList );
            }
        }

        public CswNbtNodePropNumber Timeout
        {
            get
            {
                return ( _CswNbtNode.Properties[TimeoutPropertyName].AsNumber );
            }
        }

        public CswNbtNodePropText Name
        {
            get
            {
                return ( _CswNbtNode.Properties[NamePropertyName].AsText );
            }
        }

        #endregion


    }//CswNbtObjClassRole

}//namespace ChemSW.Nbt.ObjClasses
