using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Security;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassUser : CswNbtObjClass, ICswNbtUser
    {
        public static string ChemSWAdminUsername { get { return CswAuthenticator.ChemSWAdminUsername; } }
        
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
        public static string DateFormatPropertyName { get { return "Date Format"; } }
        public static string TimeFormatPropertyName { get { return "Time Format"; } }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;
        private CswNbtObjClassRole _RoleNodeObjClass = null;
        private CswNbtNode _RoleNode = null;
        //private CswNbtNode _UserNode = null;

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

        public new void postChanges( bool ForceUpdate ) //bz# 5446
        {
            _CswNbtNode.postChanges( ForceUpdate );
            _RoleNodeObjClass.postChanges( ForceUpdate );
        }


        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            // BZ 9170
            _CswNbtResources.setConfigVariableValue( "cache_lastupdated", DateTime.Now.ToString() );
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( OverrideUniqueValidation );

            // BZ 5906
            UsernameProperty.ReadOnly = true;

            if( Role.WasModified )
            {
                if( false == _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                {
                    throw new CswDniException( ErrorType.Warning, "Only Administrators can change user roles", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to edit a user role." );
                }
                if( this.Username != ChemSWAdminUsername &&
                    CswNbtNodeCaster.AsRole(_CswNbtResources.Nodes[Role.RelatedNodeId]).Name.Text == CswNbtObjClassRole.ChemSWAdminRoleName )
                {
                    throw new CswDniException( ErrorType.Warning, "New users may not be assigned to the '" + CswNbtObjClassRole.ChemSWAdminRoleName + "' role", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to assign a new user to the '" + CswNbtObjClassRole.ChemSWAdminRoleName + "' role." );
                }
            }

            // case 22512
            if( this.Username == ChemSWAdminUsername &&
                _CswNbtResources.CurrentNbtUser != null &&
                _CswNbtResources.CurrentNbtUser.Username != ChemSWAdminUsername &&
                false == ( _CswNbtResources.CurrentNbtUser is CswNbtSystemUser ) )
            {
                throw new CswDniException( ErrorType.Warning, "The '" + ChemSWAdminUsername + "' user cannot be edited", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to edit the '" + ChemSWAdminUsername + "' user account." );
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
                throw ( new CswDniException( ErrorType.Warning, "You can not delete your own user account.", "Current user (" + _CswNbtResources.CurrentUser.Username + ") can not delete own UserClass node." ) );
            }

            // case 22635 - prevent deleting chemsw admin user
            CswNbtNodePropWrapper UsernamePropWrapper = Node.Properties[UsernamePropertyName];
            if( UsernamePropWrapper.GetOriginalPropRowValue( UsernamePropWrapper.NodeTypeProp.FieldTypeRule.SubFields.Default.Column ) == ChemSWAdminUsername &&
                false == ( _CswNbtResources.CurrentNbtUser is CswNbtSystemUser ) )
            {
                throw new CswDniException( ErrorType.Warning, "The '" + ChemSWAdminUsername + "' user cannot be deleted", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to delete the '" + ChemSWAdminUsername + "' user." );
            }
            
            CswPrimaryKey RoleId = Role.RelatedNodeId;
            if( RoleId != null )
            {
                CswNbtNode RoleNode = _CswNbtResources.Nodes[RoleId];

                //prevent user from deleting admin if they are not an admin
                if( _RoleNodeObjClass.Administrator.Checked == Tristate.True && _CswNbtResources.CurrentNbtUser.IsAdministrator() != true )
                {
                    throw ( new CswDniException( ErrorType.Warning, "You can not delete administrator accounts because you are not an administrator.", "Block user account delete because login user (" + _CswNbtResources.CurrentUser.Username + ") is not an administrator." ) );
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
                
                foreach( CswNbtAction Action in _CswNbtResources.Actions )
                {
                    if( _CswNbtResources.Permit.can( Action, this ) )
                    {
                        NewYValues.Add( Action.DisplayName.ToString() );
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
        public CswNbtNodePropDateTime LastLogin { get { return ( _CswNbtNode.Properties[LastLoginPropertyName].AsDateTime ); } }
        public CswNbtNodePropViewPickList QuickLaunchViews { get { return _CswNbtNode.Properties[QuickLaunchViewsPropertyName].AsViewPickList; } }
        public CswNbtNodePropLogicalSet QuickLaunchActions { get { return _CswNbtNode.Properties[QuickLaunchActionsPropertyName].AsLogicalSet; } }
        public CswNbtNodePropText EmailProperty { get { return _CswNbtNode.Properties[EmailPropertyName].AsText; } }
        public string Email { get { return EmailProperty.Text; } }
        public string DateFormat
        {
            get
            {
                string ret = DateFormatProperty.Value;
                if( ret == string.Empty )
                {
                    ret = CswDateTime.DefaultDateFormat;
                }
                return ret;
            }
        }
        public CswNbtNodePropList DateFormatProperty { get { return ( _CswNbtNode.Properties[DateFormatPropertyName].AsList ); } }
        public string TimeFormat
        {
            get
            {
                string ret = TimeFormatProperty.Value;
                if( ret == string.Empty )
                {
                    ret = CswDateTime.DefaultTimeFormat;
                }
                return ret;
            }
        }
        public CswNbtNodePropList TimeFormatProperty { get { return ( _CswNbtNode.Properties[TimeFormatPropertyName].AsList ); } }

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
