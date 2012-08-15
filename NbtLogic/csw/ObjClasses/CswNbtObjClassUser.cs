using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

        public const string RolePropertyName = "Role";
        public const string AccountLockedPropertyName = "AccountLocked";
        public const string FailedLoginCountPropertyName = "FailedLoginCount";
        public const string PasswordPropertyName = "Password";
        public const string UsernamePropertyName = "Username";
        public const string FirstNamePropertyName = "First Name";
        public const string LastNamePropertyName = "Last Name";
        public const string LastLoginPropertyName = "Last Login";
        public const string FavoriteViewsPropertyName = "Favorite Views";
        public const string FavoriteActionsPropertyName = "Favorite Actions";
        public const string EmailPropertyName = "Email";
        public const string PageSizePropertyName = "Page Size";
        public const string DateFormatPropertyName = "Date Format";
        public const string TimeFormatPropertyName = "Time Format";
        public const string DefaultLocationPropertyName = "Default Location";
        public const string WorkUnitPropertyName = "Work Unit";
        public const string LogLevelPropertyName = "Log Level";
        public static string ArchivedPropertyName { get { return "Archived"; } }

        

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;


        public CswNbtObjClassUser( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        private CswNbtNode __RoleNode = null;
        private CswNbtNode _RoleNode
        {
            get
            {
                if( __RoleNode == null )
                {
                    _initRole();
                }
                return __RoleNode;
            }
        } // _RoleNode

        private CswNbtObjClassRole __RoleNodeObjClass = null;
        private CswNbtObjClassRole _RoleNodeObjClass
        {
            get
            {
                if( __RoleNodeObjClass == null )
                {
                    _initRole();
                }
                return __RoleNodeObjClass;
            }
        } // _RoleNodeObjClass

        private void _initRole()
        {
            if( Node.NodeId != null )
            {
                //CswNbtMetaDataObjectClass User_ObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
                //CswNbtMetaDataObjectClass Role_ObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
                //CswNbtMetaDataObjectClassProp UserName_ObjectClassProp = User_ObjectClass.getObjectClassProp( CswNbtObjClassUser.UsernamePropertyName );
                //CswNbtMetaDataObjectClassProp Role_ObjectClassProp = User_ObjectClass.getObjectClassProp( CswNbtObjClassUser.RolePropertyName );

                //// generate the view
                //CswNbtView View = new CswNbtView( _CswNbtResources );
                //View.ViewName = "CswNbtObjClassUser(" + Node.NodeId.ToString() + ")";
                //CswNbtViewRelationship UserRelationship = View.AddViewRelationship( User_ObjectClass, false );
                //UserRelationship.NodeIdsToFilterIn.Add( Node.NodeId );
                //CswNbtViewRelationship RoleRelationship = View.AddViewRelationship( UserRelationship, PropOwnerType.First, Role_ObjectClassProp, false );

                //// generate the tree
                //ICswNbtTree UserTree = _CswNbtResources.Trees.getTreeFromView( View, false, true, false, true );


                //// get user node
                //UserTree.goToRoot();
                //if( UserTree.getChildNodeCount() > 0 )
                //{
                //    UserTree.goToNthChild( 0 );

                //    //get role node
                //    if( UserTree.getChildNodeCount() > 0 )
                //    {
                //        UserTree.goToNthChild( 0 );
                //        _RoleNode = UserTree.getNodeForCurrentPosition();
                //        _RoleNodeObjClass = (CswNbtObjClassRole) _RoleNode;
                //    }
                //}

                __RoleNode = _CswNbtResources.Nodes[RoleId];
                if( __RoleNode != null )
                {
                    __RoleNodeObjClass = (CswNbtObjClassRole) __RoleNode;
                }

            } // if( Node.NodeId != null )
        } // _initRole()

        public new void postChanges( bool ForceUpdate ) //bz# 5446
        {
            _CswNbtNode.postChanges( ForceUpdate );
            _RoleNodeObjClass.postChanges( ForceUpdate );
        }

        public Dictionary<string, string> Cookies { get; set; }

        public Int32 UserNodeTypeId { get { return NodeTypeId; } }
        public Int32 UserObjectClassId { get { return ObjectClass.ObjectClassId; } }
        public Int32 RoleNodeTypeId { get { return RoleNode.NodeTypeId; } }
        public Int32 RoleObjectClassId { get { return RoleNode.ObjectClass.ObjectClassId; } }

        public Int32 PasswordPropertyId { get { return PasswordProperty.NodeTypePropId; } }
        public bool PasswordIsExpired { get { return PasswordProperty.IsExpired; } }

        public static string getValidUserName( string Name )
        {
            return Regex.Replace( Name, "[^a-zA-Z0-9_]+", "" );
        }

        public static bool IsUserNameUnique( CswNbtResources Resources, string UserName )
        {
            CswNbtObjClassUser ExistingUserNode = Resources.Nodes.makeUserNodeFromUsername( UserName );
            return ( null == ExistingUserNode ||
                     false == CswTools.IsPrimaryKey( ExistingUserNode.NodeId ) );
        }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassUser
        /// </summary>
        public static implicit operator CswNbtObjClassUser( CswNbtNode Node )
        {
            CswNbtObjClassUser ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.UserClass ) )
            {
                ret = (CswNbtObjClassUser) Node.ObjClass;
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
            if( _unableToWriteNodeInvalidUserName() )
            {
                throw new CswDniException( ErrorType.Warning, "Username must contain alphanumeric characters only.", "Username contains invalid characters: " + this.Username );
            }

            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );

            if( UsernameProperty.Text != string.Empty ) // case 25616
            {
                UsernameProperty.setReadOnly( value: true, SaveToDb: true );   // BZ 5906
            }

            if( Role.WasModified )
            {
                if( false == _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                {
                    throw new CswDniException( ErrorType.Warning, "Only Administrators can change user roles", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to edit a user role." );
                }
                if( this.Username != ChemSWAdminUsername &&
                    ( (CswNbtObjClassRole) _CswNbtResources.Nodes[Role.RelatedNodeId] ).Name.Text == CswNbtObjClassRole.ChemSWAdminRoleName )
                {
                    throw new CswDniException( ErrorType.Warning, "New users may not be assigned to the '" + CswNbtObjClassRole.ChemSWAdminRoleName + "' role", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to assign a new user to the '" + CswNbtObjClassRole.ChemSWAdminRoleName + "' role." );
                }
            }

            // case 22512
            if( Username == ChemSWAdminUsername &&
                _CswNbtResources.CurrentNbtUser != null &&
                _CswNbtResources.CurrentNbtUser.Username != ChemSWAdminUsername &&
                false == ( _CswNbtResources.CurrentNbtUser is CswNbtSystemUser ) )
            {
                throw new CswDniException( ErrorType.Warning, "The '" + ChemSWAdminUsername + "' user cannot be edited", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to edit the '" + ChemSWAdminUsername + "' user account." );
            }

        }//beforeWriteNode()

        private bool _unableToWriteNodeInvalidUserName()
        {
            return
                false == String.IsNullOrEmpty( this.UsernameProperty.Text ) &&
                false == CswTools.IsAlphaNumeric( this.UsernameProperty.Text ) &&
                ( this.UsernameProperty.WasModified ||
                ( this.AccountLocked.WasModified && this.AccountLocked.Checked == Tristate.False ) );
        }

        public override void afterWriteNode()
        {
            //bz # 6555
            if( AccountLocked.Checked != Tristate.True && AccountLocked.WasModified )
            {
                clearFailedLoginCount();
            }

            // BZ 9170
            _CswNbtResources.ConfigVbls.setConfigVariableValue( "cache_lastupdated", DateTime.Now.ToString() );

            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

            //prevent user from deleting their own user
            if( _CswNbtNode.NodeId == _CswNbtResources.CurrentUser.UserId )
            {
                throw ( new CswDniException( ErrorType.Warning, "You can not delete your own user account.", "Current user (" + _CswNbtResources.CurrentUser.Username + ") can not delete own UserClass node." ) );
            }

            // case 22635 - prevent deleting chemsw admin user
            CswNbtNodePropWrapper UsernamePropWrapper = Node.Properties[UsernamePropertyName];
            if( UsernamePropWrapper.GetOriginalPropRowValue( UsernamePropWrapper.NodeTypeProp.getFieldTypeRule().SubFields.Default.Column ) == ChemSWAdminUsername &&
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
            FavoriteViews.User = this;

            // BZ 8288
            // Favorite Actions options should derive from Role's Action Permissions
            if( _RoleNode != null )
            {
                CswCommaDelimitedString NewYValues = new CswCommaDelimitedString();

                foreach( CswNbtAction Action in _CswNbtResources.Actions )
                {
                    if( _CswNbtResources.Permit.can( Action, this ) && Action.ShowInList )
                    {
                        NewYValues.Add( Action.DisplayName.ToString() );
                    }
                }
                this.FavoriteActions.YValues = NewYValues;
            }

            //BZ 9933
            if( _CswNbtResources.CurrentNbtUser == null || !_CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                this.FailedLoginCount.setHidden( value: true, SaveToDb: false );
                this.AccountLocked.setHidden( value: true, SaveToDb: false );
            }
            _CswNbtObjClassDefault.afterPopulateProps();

        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            //case 24525 - add default filter to ignore archived users in relationship props
            CswNbtView view = ParentRelationship.View;
            CswNbtMetaDataObjectClass userOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp archivedOCP = userOC.getObjectClassProp( CswNbtObjClassUser.ArchivedPropertyName );
            view.AddViewPropertyAndFilter( ParentRelationship, archivedOCP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Value: Tristate.True.ToString() );

            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {



            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        public static string makeRandomPassword( Int32 Length = 12 )
        {
            string RetString = string.Empty;

            CswCommaDelimitedString Characters = new CswCommaDelimitedString() { "a", "b", "c", "d", "e", "f", "g", "i", "j", "k", "m", "n", "o", "p", "q", "r", "s", "t", "w", "x", "y", "z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", ",", ";", "?", "/", "*", "\"" };
            Random Random = new Random();

            for( Int32 I = 0; I <= Length; I += 1 )
            {
                Int32 Next = Random.Next( 0, Characters.Count - 1 );
                if( Next % 2 == 0 )
                {
                    RetString += Characters[Next].ToUpper();
                }
                else
                {
                    RetString += Characters[Next].ToLower();
                }
            }
            return RetString;
        }

        #endregion

        #region Object class specific properties

        public CswPrimaryKey UserId { get { return _CswNbtNode.NodeId; } }
        public CswPrimaryKey RoleId { get { return Role.RelatedNodeId; } }

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
        public CswNbtNodePropViewPickList FavoriteViews { get { return _CswNbtNode.Properties[FavoriteViewsPropertyName].AsViewPickList; } }
        public CswNbtNodePropLogicalSet FavoriteActions { get { return _CswNbtNode.Properties[FavoriteActionsPropertyName].AsLogicalSet; } }
        public CswNbtNodePropText EmailProperty { get { return _CswNbtNode.Properties[EmailPropertyName].AsText; } }
        public string Email { get { return EmailProperty.Text; } }
        public string DateFormat
        {
            get
            {
                string ret = DateFormatProperty.Value;
                if( ret == string.Empty )
                {
                    ret = CswDateTime.DefaultDateFormat.ToString();
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
                    ret = CswDateTime.DefaultTimeFormat.ToString();
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
        public CswNbtNodePropLocation DefaultLocationProperty { get { return _CswNbtNode.Properties[DefaultLocationPropertyName]; } }
        public CswPrimaryKey DefaultLocationId { get { return DefaultLocationProperty.SelectedNodeId; } }
        public CswNbtNodePropRelationship WorkUnitProperty { get { return _CswNbtNode.Properties[WorkUnitPropertyName]; } }
        public CswPrimaryKey WorkUnitId { get { return WorkUnitProperty.RelatedNodeId; } }
        public CswNbtNodePropLogical Archived { get { return _CswNbtNode.Properties[ArchivedPropertyName]; } }

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

            if( failures >= CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( "failedloginlimit" ) ) )
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

        public bool IsArchived()
        {
            return this.Archived.Checked == Tristate.True;
        }

    }//CswNbtObjClassUser

}//namespace ChemSW.Nbt.ObjClasses
