using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassUser : CswNbtObjClass, ICswNbtUser
    {
        public static string RolePropertyName { get { return "Role"; } }
        public static string AccountLockedPropertyName { get { return "AccountLocked"; } }
        public static string FailedLoginCountPropertyName { get { return "FailedLoginCount"; } }
        public static string PasswordPropertyName { get { return "Password"; } }
        public static string UsernamePropertyName { get { return "Username"; } }
        public static string FirstNamePropertyName { get { return "First Name"; } }
        public static string LastNamePropertyName { get { return "Last Name"; } }
        public static string LastLoginPropertyName { get { return "Last Login"; } }
        public static string QuickLaunchViewsPropertyName { get { return "Quick Launch Views"; } }
        public static string QuickLaunchActionsPropertyName { get { return "Quick Launch Actions"; } }
        public static string EmailPropertyName { get { return "Email"; } }
        public static string PageSizePropertyName { get { return "Page Size"; } }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;
        private CswNbtObjClassRole _RoleNodeObjClass = null;
        private CswNbtNode _RoleNode = null;
        private CswNbtNode _UserNode = null;

        public CswNbtObjClassUser( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }

        public CswNbtObjClassUser( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

            if( Node.NodeId != null )
            {
                CswNbtMetaDataObjectClass User_ObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
                CswNbtMetaDataObjectClass Role_ObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
                CswNbtMetaDataObjectClassProp UserName_ObjectClassProp = User_ObjectClass.getObjectClassProp( CswNbtObjClassUser.UsernamePropertyName );
                CswNbtMetaDataObjectClassProp Role_ObjectClassProp = User_ObjectClass.getObjectClassProp( CswNbtObjClassUser.RolePropertyName );

                // generate the view
                CswNbtView View = new CswNbtView( _CswNbtResources );
                View.ViewName = "CswNbtObjClassUser(" + Node.NodeId.ToString() + ")";
                CswNbtViewRelationship UserRelationship = View.AddViewRelationship( User_ObjectClass, false );
                UserRelationship.NodeIdsToFilterIn.Add( Node.NodeId );
                CswNbtViewRelationship RoleRelationship = View.AddViewRelationship( UserRelationship, CswNbtViewRelationship.PropOwnerType.First, Role_ObjectClassProp, false );

                // generate the tree
                ICswNbtTree UserTree = _CswNbtResources.Trees.getTreeFromView( View, false, true, false, true );


                // get user node
                UserTree.goToRoot();
                if( UserTree.getChildNodeCount() > 0 )
                {
                    UserTree.goToNthChild( 0 );

                    //get role node
                    if( UserTree.getChildNodeCount() > 0 )
                    {
                        UserTree.goToNthChild( 0 );
                        _RoleNode = UserTree.getNodeForCurrentPosition();
                        _RoleNodeObjClass = CswNbtNodeCaster.AsRole( _RoleNode );
                    }
                }

            }
        }//ctor()

        public void postChanges( bool ForceUpdate ) //bz# 5446
        {
            _CswNbtNode.postChanges( ForceUpdate );
            _RoleNodeObjClass.postChanges( ForceUpdate );
        }


        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass ); }
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
            _CswNbtObjClassDefault.beforeWriteNode();

            // BZ 5906
            UsernameProperty.ReadOnly = true;

			if( Role.WasModified )
			{
				if( false == _CswNbtResources.CurrentNbtUser.IsAdministrator() )
				{
					throw new CswDniException( "Only Administrators can change user roles", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to edit a user role." );
				}
				if( this.Username != "chemsw_admin" &&
					CswNbtNodeCaster.AsRole(_CswNbtResources.Nodes[Role.RelatedNodeId]).Name.Text == "chemsw_admin_role" )
				{
					throw new CswDniException( "New users may not be assigned to the 'chemsw_admin_role' role", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to assign a new user to the 'chemsw_admin_role' role." );
				}
			}

			// case 22512
			if( this.Username == "chemsw_admin" &&
				_CswNbtResources.CurrentNbtUser != null &&
				_CswNbtResources.CurrentNbtUser.Username != "chemsw_admin" &&
				false == ( _CswNbtResources.CurrentNbtUser is CswNbtSystemUser ) )
			{
				throw new CswDniException( "The 'chemsw_admin' user cannot be edited", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to edit the 'chemsw_admin' user account." );
			}

		}//beforeWriteNode()

        public override void afterWriteNode()
        {
            //bz # 6555
            if( AccountLocked.Checked != Tristate.True && AccountLocked.WasModified )
            {
                clearFailedLoginCount();
            }

            // BZ 9170
            _CswNbtResources.setConfigVariableValue( "cache_lastupdated", DateTime.Now.ToString() );

            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

            //prevent user from deleting their own user
            if( _CswNbtNode.NodeId == _CswNbtResources.CurrentUser.UserId )
            {
                throw ( new CswDniException( "You can not delete your own user account.", "Current user (" + _CswNbtResources.CurrentUser.Username + ") can not delete own UserClass node." ) );
            }

            ////prevent user from deleting ScheduleRunner
            //if (Username.ToLower() == "schedulerunner")
            //{
            //    throw new CswDniException("You cannot delete the ScheduleRunner user", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to delete the ScheduleRunner user");
            //}


            CswPrimaryKey RoleId = Role.RelatedNodeId;
            if( RoleId != null )
            {
                CswNbtNode RoleNode = _CswNbtResources.Nodes[RoleId];

                //prevent user from deleting admin if they are not an admin
                if( _RoleNodeObjClass.Administrator.Checked == Tristate.True && _CswNbtResources.CurrentNbtUser.IsAdministrator() != true )
                {
                    throw ( new CswDniException( "You can not delete administrator accounts because you are not an administrator.", "Block user account delete because login user (" + _CswNbtResources.CurrentUser.Username + ") is not an administrator." ) );
                }
            }
        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            // BZ 6941, 8288
            // Set the Default View to use the selected User, rather than the logged in User
            //DefaultView.User = this;
            QuickLaunchViews.User = this;

            // BZ 8288
            // Quick Launch Actions options should derive from Role's Action Permissions
            if( _RoleNode != null )
            {
                CswCommaDelimitedString NewYValues = new CswCommaDelimitedString();
				
				foreach( string YValue in _RoleNodeObjClass.ActionPermissions.YValues )
				{
					if( _CswNbtResources.Permit.can( CswNbtAction.ActionNameStringToEnum( YValue ), this ) )
					{
						NewYValues.Add( YValue );
					}
				}
                this.QuickLaunchActions.YValues = NewYValues;
            }

            //BZ 9933
            if( _CswNbtResources.CurrentNbtUser == null || !_CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                this.FailedLoginCount.Hidden = true;
                this.AccountLocked.Hidden = true;
            }
            _CswNbtObjClassDefault.afterPopulateProps();

        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        #endregion

        #region Object class specific properties

        public CswPrimaryKey UserId { get { return _CswNbtNode.NodeId; } }
        public CswPrimaryKey RoleId { get { return _RoleNode.NodeId; } }

        public CswNbtObjClassUser UserNode { get { return this; } }
        public CswNbtObjClassRole RoleNode { get { return _RoleNodeObjClass; } }
        public string Rolename { get { return _RoleNodeObjClass.Name.Text; } }
        public Int32 RoleTimeout { get { return CswConvert.ToInt32( _RoleNodeObjClass.Timeout.Value ); } }

        public CswNbtNodePropRelationship Role { get { return ( _CswNbtNode.Properties[RolePropertyName].AsRelationship ); } }
        public CswNbtNodePropLogical AccountLocked { get { return ( _CswNbtNode.Properties[AccountLockedPropertyName].AsLogical ); } }
        public CswNbtNodePropNumber FailedLoginCount { get { return ( _CswNbtNode.Properties[FailedLoginCountPropertyName].AsNumber ); } }
        public CswNbtNodePropPassword PasswordProperty { get { return ( _CswNbtNode.Properties[PasswordPropertyName].AsPassword ); } }
        public CswNbtNodePropText UsernameProperty { get { return ( _CswNbtNode.Properties[UsernamePropertyName].AsText ); } }
        public CswNbtNodePropText FirstNameProperty { get { return ( _CswNbtNode.Properties[FirstNamePropertyName].AsText ); } }
        public CswNbtNodePropText LastNameProperty { get { return ( _CswNbtNode.Properties[LastNamePropertyName].AsText ); } }
        public string FirstName { get { return FirstNameProperty.Text; } }
        public string LastName { get { return LastNameProperty.Text; } }
        public string Username { get { return UsernameProperty.Text; } }
        public CswNbtNodePropDate LastLogin { get { return ( _CswNbtNode.Properties[LastLoginPropertyName].AsDate ); } }
        public CswNbtNodePropViewPickList QuickLaunchViews { get { return _CswNbtNode.Properties[QuickLaunchViewsPropertyName].AsViewPickList; } }
        public CswNbtNodePropLogicalSet QuickLaunchActions { get { return _CswNbtNode.Properties[QuickLaunchActionsPropertyName].AsLogicalSet; } }
        public CswNbtNodePropText EmailProperty { get { return _CswNbtNode.Properties[EmailPropertyName].AsText; } }
        public string Email { get { return EmailProperty.Text; } }

        public string EncryptedPassword { get { return PasswordProperty.EncryptedPassword; } }

        public CswNbtNodePropNumber PageSizeProperty { get { return _CswNbtNode.Properties[PageSizePropertyName].AsNumber; } }
        public Int32 PageSize
        {
            get
            {
                Int32 ret = CswConvert.ToInt32( PageSizeProperty.Value );
                if( ret <= 0 ) ret = 10;
                return ret;
            }
        }

        #endregion


        public int getFailedLoginCount()
        {
            double LoginCount = FailedLoginCount.Value;
            if( double.IsNaN( LoginCount ) ) LoginCount = 0;
            if( LoginCount < 0 ) LoginCount = 0;
            return CswConvert.ToInt32( LoginCount );
        }

        public void incFailedLoginCount()
        {
            int failures = getFailedLoginCount();
            failures += 1;

            this.FailedLoginCount.Value = Convert.ToDouble( failures );

            if( failures >= CswConvert.ToInt32( _CswNbtResources.getConfigVariableValue( "failedloginlimit" ) ) )
            {
                this.AccountLocked.Checked = Tristate.True;
            }
        }

        public void clearFailedLoginCount()
        {
            this.FailedLoginCount.Value = 0;
        }

        public bool IsAccountLocked()
        {
            return this.AccountLocked.Checked == Tristate.True;
        }

        public bool IsAdministrator()
        {
            return _RoleNodeObjClass.Administrator.Checked == Tristate.True;
        }

    }//CswNbtObjClassUser

}//namespace ChemSW.Nbt.ObjClasses
