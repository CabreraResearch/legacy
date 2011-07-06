using System;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassRole : CswNbtObjClass
    {
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

        public CswNbtObjClassRole( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode()
        {
            _CswNbtObjClassDefault.beforeCreateNode();
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            // BZ 9170
            _CswNbtResources.setConfigVariableValue( "cache_lastupdated", DateTime.Now.ToString() );

            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode()
        {
            // The user cannot change his or her own Administrator privileges.
            if( Administrator.WasModified && _CswNbtResources.CurrentUser.RoleId == _CswNbtNode.NodeId )
            {
                _CswNbtNode.Properties.clearModifiedFlag();  // prevents multiple error messages from appearing if we attempt to write() again
                throw new CswDniException( "You may not change your own administrator status", "User (" + _CswNbtResources.CurrentUser.Username + ") attempted to edit the Administrator property of their own Role" );
            }

			// case 22512
			// also case 22557 - use the original name, not the new one
			CswNbtNodePropWrapper NamePropWrapper = Node.Properties[NamePropertyName];
			if( NamePropWrapper.GetOriginalPropRowValue( NamePropWrapper.NodeTypeProp.FieldTypeRule.SubFields.Default.Column ) == "chemsw_admin_role" &&
				_CswNbtResources.CurrentNbtUser.Username != "chemsw_admin" &&
				false == ( _CswNbtResources.CurrentNbtUser is CswNbtSystemUser ) )
			{
				throw new CswDniException( "The 'chemsw_admin_role' role cannot be edited", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to edit the 'chemsw_admin_role' role." );
			}

			// case 22437
			if( ActionPermissions.WasModified )
			{
				// You can never grant your own action permissions
				if( _CswNbtResources.CurrentUser.RoleId == _CswNbtNode.NodeId )
				{
					throw new CswDniException( "You may not grant access to actions for which you have no permissions",
						"User (" + _CswNbtResources.CurrentUser.Username + ") attempted to edit their own action permissions on role: " + _CswNbtNode.NodeName );
				}
				// You can only grant action permissions on other roles to which you have access
				foreach( string ActionNameString in ActionPermissions.YValues )
				{
					CswNbtActionName ActionName = CswNbtAction.ActionNameStringToEnum( ActionNameString );
					if( true == _CswNbtResources.Permit.can( ActionName, this ) &&
						false == _CswNbtResources.Permit.can( ActionName, _CswNbtResources.CurrentNbtUser ) )
					{
						throw new CswDniException( "You may not grant access to actions for which you have no permissions",
							"User (" + _CswNbtResources.CurrentUser.Username + ") attempted to grant access to action " + CswNbtAction.ActionNameEnumToString( ActionName ) + " to role " + _CswNbtNode.NodeName );
					}
				} // foreach( string ActionNameString in ActionPermissions.YValues )
			} // if( ActionPermissions.WasModified )

            _CswNbtObjClassDefault.beforeWriteNode();
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();

            // BZ 9170
            _CswNbtResources.setConfigVariableValue( "cache_lastupdated", DateTime.Now.ToString() );

        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

            // Prevent deleting your own role
            if( _CswNbtNode.NodeId == _CswNbtResources.CurrentUser.RoleId )
            {
                throw ( new CswDniException( "You can not delete your own role account.", "Current user (" + _CswNbtResources.CurrentUser.Username + ") can not delete own RoleClass node." ) );
            }
			
			// case 22635 - prevent deleting chemsw_admin_role
			CswNbtNodePropWrapper NamePropWrapper = Node.Properties[NamePropertyName];
			if( NamePropWrapper.GetOriginalPropRowValue( NamePropWrapper.NodeTypeProp.FieldTypeRule.SubFields.Default.Column ) == "chemsw_admin_role" &&
				false == ( _CswNbtResources.CurrentNbtUser is CswNbtSystemUser ) )
			{
				throw new CswDniException( "The 'chemsw_admin_role' role cannot be deleted", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to delete the 'chemsw_admin_role' role." );
			}

			// case 22424
			// Prevent deleting roles in use
			CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
			foreach( CswNbtNode UserNode in UserOC.getNodes( false, true ) )
			{
				CswNbtObjClassUser UserNodeAsUser = CswNbtNodeCaster.AsUser( UserNode );
				if( UserNodeAsUser.Role.RelatedNodeId == _CswNbtNode.NodeId )
				{
					throw ( new CswDniException( "This role cannot be deleted because it is in use by user: " + UserNodeAsUser.Username, 
												 "Current user (" + _CswNbtResources.CurrentUser.Username + ") tried to delete a role that is in use (" + _CswNbtNode.NodeName + ") by user: " + UserNodeAsUser.Username ) );
				}
			}

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

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
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

        public CswNbtNodePropLogicalSet NodeTypePermissions
        {
            get
            {
                return ( _CswNbtNode.Properties[NodeTypePermissionsPropertyName].AsLogicalSet );
            }
        }

        public CswNbtNodePropLogicalSet ActionPermissions
        {
            get
            {
                return ( _CswNbtNode.Properties[ActionPermissionsPropertyName].AsLogicalSet );
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
