using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.DB;
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
        public const string ChemSWAdminUsername = CswAuthenticator.ChemSWAdminUsername;

        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string AccountLocked = "AccountLocked";
            public const string Archived = "Archived";
            public const string Barcode = "Barcode";
            public const string DateFormat = "Date Format";
            public const string DefaultLocation = "Default Location";
            public const string DefaultPrinter = "Default Printer";
            public const string Email = "Email";
            public const string EmployeeId = "Employee ID";
            public const string FailedLoginCount = "FailedLoginCount";
            public const string FavoriteActions = "Favorite Actions";
            public const string FavoriteViews = "Favorite Views";
            public const string FirstName = "First Name";
            public const string Jurisdiction = "Jurisdiction";
            public const string Language = "Language";
            public const string LastLogin = "Last Login";
            public const string LastName = "Last Name";
            public const string LogLevel = "Log Level";
            public const string PageSize = "Page Size";
            public const string Password = "Password";
            public const string Phone = "Phone";
            public const string Role = "Role";
            public const string TimeFormat = "Time Format";
            public const string Username = "Username";
            public const string DefaultBalance = "Default Balance";
            public const string CurrentWorkUnit = "Current Work Unit";
            public const string CachedData = "Cached Data";
            public const string AvailableWorkUnits = "Available Work Units";
            public const string CostCode = "Cost Code";
            public const string C3ACDPreferredSuppliers = "C3 ACD Preferred Suppliers";
        }

        public CswNbtObjClassUser( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

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
        }

        // _RoleNode

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
        }

        // _RoleNodeObjClass

        private void _initRole()
        {
            if( Node.NodeId != null )
            {
                __RoleNode = _CswNbtResources.Nodes[RoleId];
                if( __RoleNode != null )
                {
                    __RoleNodeObjClass = (CswNbtObjClassRole) __RoleNode;
                }

            } // if( Node.NodeId != null )
        }

        // _initRole()

        public void postChanges( bool ForceUpdate ) //bz# 5446
        {
            _CswNbtNode.postChanges( ForceUpdate );
            _RoleNodeObjClass.postChanges( ForceUpdate );
        }

        public Dictionary<string, string> Cookies { get; set; }

        public Int32 UserNodeTypeId
        {
            get { return NodeTypeId; }
        }

        public Int32 UserObjectClassId
        {
            get { return ObjectClass.ObjectClassId; }
        }

        public Int32 RoleNodeTypeId
        {
            get { return RoleNode.NodeTypeId; }
        }

        public Int32 RoleObjectClassId
        {
            get { return RoleNode.ObjectClass.ObjectClassId; }
        }

        public Int32 PasswordPropertyId
        {
            get { return PasswordProperty.NodeTypePropId; }
        }

        public bool PasswordIsExpired
        {
            get { return PasswordProperty.IsExpired; }
        }

        string ICswUser.CachedData
        {
            get { return CachedData.Text; }
        }

        public static bool IsUserNameUnique( CswNbtResources Resources, string UserName )
        {
            CswNbtObjClassUser ExistingUserNode = Resources.Nodes.makeUserNodeFromUsername( UserName );
            return ( null == ExistingUserNode ||
                    false == CswTools.IsPrimaryKey( ExistingUserNode.NodeId ) );
        }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassUser
        /// </summary>
        public static implicit operator CswNbtObjClassUser( CswNbtNode Node )
        {
            CswNbtObjClassUser ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.UserClass ) )
            {
                ret = (CswNbtObjClassUser) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        protected override void beforeWriteNodeLogic( bool Creating )
        {
            if( _unableToWriteNodeInvalidUserName() )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Usernames may only contains letters, numbers, underscores, periods and dashes.",
                                          "Username contains invalid characters: " + this.Username );
            }

            if( UsernameProperty.Text != string.Empty ) // case 25616
            {
                UsernameProperty.setReadOnly( value: true, SaveToDb: true ); // BZ 5906
            }

            // case 22512
            if( Username == ChemSWAdminUsername &&
                _CswNbtResources.CurrentNbtUser != null &&
                _CswNbtResources.CurrentNbtUser.Username != ChemSWAdminUsername &&
                false == ( _CswNbtResources.CurrentNbtUser is CswNbtSystemUser ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "The '" + ChemSWAdminUsername + "' user cannot be edited",
                                          "Current user (" + _CswNbtResources.CurrentUser.Username +
                                          ") attempted to edit the '" + ChemSWAdminUsername + "' user account." );
            }

            if( AvailableWorkUnits.Value.Count == 0 && null != CurrentWorkUnitProperty.RelatedNodeId )
            {
                AvailableWorkUnits.AddValue( CurrentWorkUnitProperty.RelatedNodeId.ToString() );
            }
        }

        //beforeWriteNode()

        private bool _unableToWriteNodeInvalidUserName()
        {
            return
                false == String.IsNullOrEmpty( this.UsernameProperty.Text ) &&
                false == CswTools.IsValidUsername( this.UsernameProperty.Text ) &&
                ( this.UsernameProperty.wasAnySubFieldModified() ||
                 ( this.AccountLocked.wasAnySubFieldModified() && this.AccountLocked.Checked == CswEnumTristate.False ) );
        }

        protected override void afterWriteNodeLogic()
        {
            CachedData.setHidden( value: true, SaveToDb: true );
            // BZ 9170
            _CswNbtResources.ConfigVbls.setConfigVariableValue( "cache_lastupdated", DateTime.Now.ToString() );
        }

        //afterWriteNode()

        protected override void beforeDeleteNodeLogic()
        {
            //prevent user from deleting their own user
            if( _CswNbtNode.NodeId == _CswNbtResources.CurrentUser.UserId )
            {
                throw ( new CswDniException( CswEnumErrorType.Warning, "You can not delete your own user account.",
                                           "Current user (" + _CswNbtResources.CurrentUser.Username +
                                           ") can not delete own UserClass node." ) );
            }

            // case 22635 - prevent deleting chemsw admin user
            CswNbtNodePropWrapper UsernamePropWrapper = Node.Properties[PropertyName.Username];
            if(
                UsernamePropWrapper.GetOriginalPropRowValue(
                    UsernamePropWrapper.NodeTypeProp.getFieldTypeRule().SubFields.Default.Column ) == ChemSWAdminUsername &&
                false == ( _CswNbtResources.CurrentNbtUser is CswNbtSystemUser ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "The '" + ChemSWAdminUsername + "' user cannot be deleted",
                                          "Current user (" + _CswNbtResources.CurrentUser.Username +
                                          ") attempted to delete the '" + ChemSWAdminUsername + "' user." );
            }

            CswPrimaryKey RoleId = Role.RelatedNodeId;
            if( RoleId != null )
            {
                CswNbtNode RoleNode = _CswNbtResources.Nodes[RoleId];

                //prevent user from deleting admin if they are not an admin
                if( _RoleNodeObjClass.Administrator.Checked == CswEnumTristate.True &&
                    _CswNbtResources.CurrentNbtUser.IsAdministrator() != true )
                {
                    throw ( new CswDniException( CswEnumErrorType.Warning,
                                               "You can not delete administrator accounts because you are not an administrator.",
                                               "Block user account delete because login user (" +
                                               _CswNbtResources.CurrentUser.Username + ") is not an administrator." ) );
                }
            }

            //case 28010 - delete all view assigned to this user
            _CswNbtResources.ViewSelect.deleteViewsByUserId( NodeId );
        }

        protected override void afterPopulateProps()
        {
            UsernameProperty.SetOnPropChange( OnUserNamePropChange );
            AvailableWorkUnits.SetOnPropChange( OnAvailableWorkUnitsChange );
            CurrentWorkUnitProperty.SetOnPropChange( OnCurrentWorkUnitPropertyChange );
            AccountLocked.SetOnPropChange( onAccountLockedPropChange );

            AvailableWorkUnits.InitOptions = InitAvailableWorkUnitsOptions;

            _updateAvailableWorkUnits();

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


            //case 27793: these are the properties that a user cannot edit -- not even his own
            if( ( null == _CswNbtResources.CurrentNbtUser ) ||
                ( false == _CswNbtResources.CurrentNbtUser.IsAdministrator() ) )
            {

                this.Role.setReadOnly( true, false );
            }

            //case 27793: Prevent non-adminsitrators from editing paswords, except their own
            if( IsPasswordReadOnly )
            {
                this.PasswordProperty.setReadOnly( true, false );
            }
            else
            {
                this.PasswordProperty.setReadOnly( false, false );
            }

            Role.SetOnPropChange( onRolePropChange );
            DateFormatProperty.SetOnPropChange( onDateFormatPropChange );
            TimeFormatProperty.SetOnPropChange( onTimeFormatPropChange );

            //Case 31084: only an administrator can edit other users' profiles
            if( ( null == _CswNbtResources.CurrentNbtUser ) || ( false == _CswNbtResources.CurrentNbtUser.IsAdministrator() && UserId != _CswNbtResources.CurrentNbtUser.UserId ) )
            {
                this.Node.setReadOnly( true, false );
            }
        }

        //afterPopulateProps()

        public bool IsPasswordReadOnly
        {
            get
            {
                bool ReturnVal = ( null == _CswNbtResources.CurrentNbtUser ) || 
                                   ( ( this.NodeId != _CswNbtResources.CurrentNbtUser.UserId ) && 
                                     ( false == _CswNbtResources.CurrentNbtUser.IsAdministrator() ) );
                return ( ReturnVal );
            }//get

        }//IsPasswordReadOnly

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            //case 24525 - add default filter to ignore archived users in relationship props
            CswNbtView view = ParentRelationship.View;
            CswNbtMetaDataObjectClass userOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp archivedOCP = userOC.getObjectClassProp( PropertyName.Archived );
            view.AddViewPropertyAndFilter( ParentRelationship, archivedOCP, FilterMode: CswEnumNbtFilterMode.NotEquals, Value: CswEnumTristate.True.ToString() );
        }

        #endregion Inherited Events

        #region Object class specific properties

        public CswPrimaryKey UserId { get { return _CswNbtNode.NodeId; } }
        public CswPrimaryKey RoleId { get { return Role.RelatedNodeId; } }

        public CswNbtObjClassUser UserNode { get { return this; } }
        public CswNbtObjClassRole RoleNode { get { return _RoleNodeObjClass; } }
        public string Rolename { get { return _RoleNodeObjClass.Name.Text; } }
        public Int32 RoleTimeout { get { return CswConvert.ToInt32( _RoleNodeObjClass.Timeout.Value ); } }

        public CswNbtNodePropRelationship Role { get { return ( _CswNbtNode.Properties[PropertyName.Role] ); } }
        private void onRolePropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            if( null != _CswNbtResources.CurrentNbtUser &&
                CswTools.IsPrimaryKey( Role.RelatedNodeId ) &&
                Role.RelatedNodeId.PrimaryKey != CswConvert.ToInt32( Role.GetOriginalPropRowValue( CswEnumNbtPropColumn.Field1_FK ) ) )
            {
                if( false == _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "Only Administrators can change user roles",
                                               "Current user (" + _CswNbtResources.CurrentUser.Username +
                                               ") attempted to edit a user role." );
                }
                if( this.Username != ChemSWAdminUsername &&
                    ( (CswNbtObjClassRole) _CswNbtResources.Nodes[Role.RelatedNodeId] ).Name.Text ==
                    CswNbtObjClassRole.ChemSWAdminRoleName )
                {
                    throw new CswDniException( CswEnumErrorType.Warning,
                                               "New users may not be assigned to the '" +
                                               CswNbtObjClassRole.ChemSWAdminRoleName + "' role",
                                               "Current user (" + _CswNbtResources.CurrentUser.Username +
                                               ") attempted to assign a new user to the '" +
                                               CswNbtObjClassRole.ChemSWAdminRoleName + "' role." );
                }
            }
        }

        public CswNbtNodePropLogical AccountLocked { get { return ( _CswNbtNode.Properties[PropertyName.AccountLocked] ); } }
        private void onAccountLockedPropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            if( CswEnumTristate.True != AccountLocked.Checked )
            {
                clearFailedLoginCount();
            }
        }
        public CswNbtNodePropNumber FailedLoginCount { get { return ( _CswNbtNode.Properties[PropertyName.FailedLoginCount] ); } }
        public CswNbtNodePropPassword PasswordProperty { get { return ( _CswNbtNode.Properties[PropertyName.Password] ); } }
        public CswNbtNodePropText UsernameProperty { get { return ( _CswNbtNode.Properties[PropertyName.Username] ); } }
        public CswNbtNodePropText FirstNameProperty { get { return ( _CswNbtNode.Properties[PropertyName.FirstName] ); } }
        public CswNbtNodePropText LastNameProperty { get { return ( _CswNbtNode.Properties[PropertyName.LastName] ); } }
        public string FirstName { get { return FirstNameProperty.Text; } }
        public string LastName { get { return LastNameProperty.Text; } }
        public string Username { get { return UsernameProperty.Text; } }
        public CswNbtNodePropDateTime LastLogin { get { return ( _CswNbtNode.Properties[PropertyName.LastLogin] ); } }
        public CswNbtNodePropViewPickList FavoriteViews { get { return _CswNbtNode.Properties[PropertyName.FavoriteViews]; } }
        public CswNbtNodePropLogicalSet FavoriteActions { get { return _CswNbtNode.Properties[PropertyName.FavoriteActions]; } }
        public CswNbtNodePropText EmailProperty { get { return _CswNbtNode.Properties[PropertyName.Email]; } }
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
        public CswNbtNodePropList DateFormatProperty { get { return ( _CswNbtNode.Properties[PropertyName.DateFormat] ); } }
        private void onDateFormatPropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            if( false == string.IsNullOrEmpty( DateFormatProperty.Value ) &&
                CswResources.UnknownEnum == (CswEnumDateFormat) DateFormatProperty.Value )
            {
                string SupportedFormats = "'" + CswEnumDateFormat.Mdyyyy + "', ";
                SupportedFormats += "'" + CswEnumDateFormat.dMyyyy + "', ";
                SupportedFormats += "'" + CswEnumDateFormat.yyyyMMdd_Dashes + "', ";
                SupportedFormats += "'" + CswEnumDateFormat.yyyyMd + "', ";
                SupportedFormats += "'" + CswEnumDateFormat.ddMMMyyyy + "'";
                throw new CswDniException( "Cannot use '" + DateFormatProperty.Value + "' as a value for Date Format. The only supported formats are: " + SupportedFormats );
            }
        }
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
        public CswNbtNodePropList TimeFormatProperty { get { return ( _CswNbtNode.Properties[PropertyName.TimeFormat] ); } }
        private void onTimeFormatPropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            if( false == string.IsNullOrEmpty( TimeFormatProperty.Value ) &&
                CswResources.UnknownEnum == (CswEnumTimeFormat) TimeFormatProperty.Value )
            {
                string SupportedFormats = "'" + CswEnumTimeFormat.Hmmss + "', ";
                SupportedFormats += "'" + CswEnumTimeFormat.hmmsstt + "'";
                throw new CswDniException( "Cannot use '" + TimeFormatProperty.Value + "' as a value for Time Format. The only supported formats are: " + SupportedFormats );
            }
        }

        public string EncryptedPassword { get { return PasswordProperty.EncryptedPassword; } }

        public CswNbtNodePropNumber PageSizeProperty { get { return _CswNbtNode.Properties[PropertyName.PageSize]; } }
        public Int32 PageSize
        {
            get
            {
                Int32 ret = CswConvert.ToInt32( PageSizeProperty.Value );
                if( ret <= 0 ) ret = 10;
                return ret;
            }
        }
        public CswNbtNodePropLocation DefaultLocationProperty { get { return _CswNbtNode.Properties[PropertyName.DefaultLocation]; } }
        public CswPrimaryKey DefaultLocationId { get { return DefaultLocationProperty.SelectedNodeId; } }
        public CswNbtNodePropRelationship DefaultPrinterProperty { get { return _CswNbtNode.Properties[PropertyName.DefaultPrinter]; } }
        public CswPrimaryKey DefaultPrinterId { get { return DefaultPrinterProperty.RelatedNodeId; } }
        public CswNbtNodePropRelationship DefaultBalanceProperty { get { return _CswNbtNode.Properties[PropertyName.DefaultBalance]; } }
        public CswPrimaryKey DefaultBalanceId { get { return DefaultBalanceProperty.RelatedNodeId; } }
        public CswNbtNodePropRelationship CurrentWorkUnitProperty { get { return _CswNbtNode.Properties[PropertyName.CurrentWorkUnit]; } }
        public void OnCurrentWorkUnitPropertyChange( CswNbtNodeProp Prop, bool Creating )
        {
            CswPrimaryKey UsersCurrentWorkUnitId = CurrentWorkUnitId;
            //Case 30817
            //if( null != UsersWorkUnitId )
            if( false == CswTools.IsPrimaryKey( UsersCurrentWorkUnitId ) )
            {
                UsersCurrentWorkUnitId = GetFirstAvailableWorkUnitNodeId();

            }

            if( false == AvailableWorkUnits.CheckValue( UsersCurrentWorkUnitId.ToString() ) )
            {
                if( false == _CswNbtResources.CurrentNbtUser is CswNbtSystemUser &&
                    _CswNbtResources.CurrentNbtUser.Username != ChemSWAdminUsername )
                {
                    throw new CswDniException( CswEnumErrorType.Warning,
                                               CurrentWorkUnitProperty.CachedNodeName +
                                               " is not an available Work Unit for user " + Username,
                                               _CswNbtResources.CurrentNbtUser.Username + " attempted to assign User: " +
                                               Username + " to Work Unit: " + UsersCurrentWorkUnitId +
                                               " when Users available Work Units are: " + AvailableWorkUnits.Value );
                }
                // We add the work unit to the list and then check it!
                AvailableWorkUnits.AddValue( UsersCurrentWorkUnitId.ToString() );
                CurrentWorkUnitProperty.RelatedNodeId = UsersCurrentWorkUnitId;
                CurrentWorkUnitProperty.SyncGestalt();

                _updateAvailableWorkUnits();
            }
        }

        public CswPrimaryKey CurrentWorkUnitId { get { return CurrentWorkUnitProperty.RelatedNodeId; } }
        public CswNbtNodePropLogical Archived { get { return _CswNbtNode.Properties[PropertyName.Archived]; } }
        public CswNbtNodePropRelationship JurisdictionProperty { get { return _CswNbtNode.Properties[PropertyName.Jurisdiction]; } }
        public CswPrimaryKey JurisdictionId { get { return JurisdictionProperty.RelatedNodeId; } }
        public CswNbtNodePropBarcode Barcode { get { return ( _CswNbtNode.Properties[PropertyName.Barcode] ); } }
        public CswNbtNodePropList LanguageProperty { get { return ( _CswNbtNode.Properties[PropertyName.Language] ); } }
        public string Language
        {
            get
            {
                return ( LanguageProperty.Value );
            }
        }
        public CswNbtNodePropText Phone { get { return _CswNbtNode.Properties[PropertyName.Phone]; } }
        public CswNbtNodePropText EmployeeId { get { return _CswNbtNode.Properties[PropertyName.EmployeeId]; } }

        private void OnUserNamePropChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( false == Prop.Empty )
            {
                Prop.setReadOnly( true, true );
            }
        }

        public CswNbtNodePropMultiList AvailableWorkUnits { get { return _CswNbtNode.Properties[PropertyName.AvailableWorkUnits]; } }
        public void OnAvailableWorkUnitsChange( CswNbtNodeProp Prop, bool Creating )
        {
            _updateAvailableWorkUnits();

            if( null == CurrentWorkUnitId || false == AvailableWorkUnits.CheckValue( CurrentWorkUnitId.ToString() ) )
            {
                CswPrimaryKey pk = CswConvert.ToPrimaryKey( AvailableWorkUnits.Value[0] ); //we're always guarenteed there's at least one
                CurrentWorkUnitProperty.RelatedNodeId = pk;
                CurrentWorkUnitProperty.SyncGestalt();
            }
        }

        public CswNbtNodePropText CostCode { get { return _CswNbtNode.Properties[PropertyName.CostCode]; } }

        [DataContract]
        public class Cache
        {
            private CswNbtObjClassUser _CurrentUser = null;
            private CswNbtResources _Resources = null;

            public Cache( CswNbtResources Resources )
            {
                _Resources = Resources;
            }


            [DataContract]
            public class Cart
            {
                private int get( ref int val )
                {
                    if( val < 0 )
                    {
                        val = 0;
                    }
                    return val;
                }

                private int pending = 0;
                private int submitted = 0;
                private int recurring = 0;
                private int favorite = 0;

                [DataMember( IsRequired = false )]
                public Int32 PendingRequestItems
                {
                    get { return get( ref pending ); }
                    set { pending = get( ref value ); }
                }

                [DataMember( IsRequired = false )]
                public Int32 SubmittedRequestItems
                {
                    get { return get( ref submitted ); }
                    set { submitted = get( ref value ); }
                }

                [DataMember( IsRequired = false )]
                public Int32 RecurringRequestItems
                {
                    get { return get( ref recurring ); }
                    set { recurring = get( ref value ); }
                }

                [DataMember( IsRequired = false )]
                public Int32 FavoriteRequestItems
                {
                    get { return get( ref favorite ); }
                    set { favorite = get( ref value ); }
                }
            }

            [DataMember]
            public Cart CartCounts = new Cart();

            public void update( CswNbtResources Resources )
            {
                _Resources = _Resources ?? Resources;
                if( null != _Resources )
                {
                    _CurrentUser = _CurrentUser ?? _Resources.Nodes[_Resources.CurrentNbtUser.UserId];
                    if( null != _CurrentUser )
                    {
                        _CurrentUser.CurrentCache = this;
                        _CurrentUser.postChanges( ForceUpdate: false );
                    }
                }
            }
        }

        private Cache _CurrentCache;
        public Cache CurrentCache
        {
            get
            {
                return _CurrentCache ?? ( _CurrentCache = getCurrentUserCache( _CswNbtResources ) );
            }
            set
            {
                _CurrentCache = value;
                string Memo = CswSerialize<Cache>.ToString( _CurrentCache );
                CachedData.Text = Memo;
            }
        }

        public static Cache getCurrentUserCache( CswNbtResources Resources )
        {
            Cache Ret = new Cache( Resources );
            if( false == string.IsNullOrEmpty( Resources.CurrentNbtUser.CachedData ) )
            {
                Ret = CswSerialize<Cache>.ToObject( Resources.CurrentNbtUser.CachedData );
            }
            return Ret;
        }

        public CswNbtNodePropMemo CachedData { get { return _CswNbtNode.Properties[PropertyName.CachedData]; } }
        // Note for C3ACDPreferredSuppliers: Comma delimited list of companyids
        public CswNbtNodePropMemo C3ACDPreferredSuppliers { get { return _CswNbtNode.Properties[PropertyName.C3ACDPreferredSuppliers]; } }

        #endregion Object class specific properties

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
                this.AccountLocked.Checked = CswEnumTristate.True;
            }
        }

        public void clearFailedLoginCount()
        {
            this.FailedLoginCount.Value = 0;
        }

        public bool IsAccountLocked()
        {
            return this.AccountLocked.Checked == CswEnumTristate.True;
        }

        public bool IsAdministrator()
        {
            return _RoleNodeObjClass.Administrator.Checked == CswEnumTristate.True;
        }

        public bool IsArchived()
        {
            return this.Archived.Checked == CswEnumTristate.True;
        }

        #region Permissions Logic


        /// <summary>
        /// Returns the Permission node (if one exists) for this User for the given Permission GroupId
        /// </summary>
        public CswNbtPropertySetPermission getPermissionForGroup( CswPrimaryKey PermissionGroupId )
        {
            CswNbtPropertySetPermission PermissionNode = null;
            Dictionary<CswPrimaryKey, CswPrimaryKey> UserPerms = getUserPermissions();
            if( null != UserPerms && UserPerms.ContainsKey( PermissionGroupId ) )
            {
                CswPrimaryKey PermissionId = UserPerms[PermissionGroupId];
                PermissionNode = _CswNbtResources.Nodes[PermissionId];
            }
            return PermissionNode;
        }

        /// <summary>
        /// Returns whether the user has any permission to any group
        /// </summary>
        /// <param name="PermObjectClass">Limit to one class of permission</param>
        /// <param name="RequireEdit">Limit to Edit permissions</param>
        public bool hasUserPermissions( CswEnumNbtObjectClass PermObjectClass = null, bool RequireEdit = false )
        {
            return getUserPermissions( PermObjectClass, RequireEdit ).Keys.Count > 0;
        }

        private Dictionary<CswPrimaryKey, CswPrimaryKey> _UserPermissions;

        /// <summary>
        /// Returns a dictionary of Permission Groups and Permissions (primary keys only), if any exists for this User
        /// </summary>
        /// <param name="PermObjectClass">Limit to one class of permission</param>
        /// <param name="RequireEdit">Limit to Edit permissions</param>
        public Dictionary<CswPrimaryKey, CswPrimaryKey> getUserPermissions( CswEnumNbtObjectClass PermObjectClass = null, bool RequireEdit = false )
        {
            Dictionary<CswPrimaryKey, CswPrimaryKey> ret;

            // Check the cache for the general case
            if( null == PermObjectClass &&
                false == RequireEdit &&
                null != _UserPermissions &&
                _UserPermissions.Keys.Count > 0 )
            {
                ret = _UserPermissions;
            }
            else
            {
                ret = new Dictionary<CswPrimaryKey, CswPrimaryKey>();


                if( CswTools.IsPrimaryKey( CurrentWorkUnitId ) && CswTools.IsPrimaryKey( RoleId ) )
                {
                    CswNbtMetaDataPropertySet PermissionSet = _CswNbtResources.MetaData.getPropertySet( CswEnumNbtPropertySetName.PermissionSet );

                    // This performs better than having it in its own 'with' query in large scale data, 
                    // because it avoids a full table scan on jct_nodes_props.
                    string pvalQuery = @"from jct_propertyset_ocprop jpocp
                                                    join object_class_props ocp on ocp.objectclasspropid = jpocp.objectclasspropid
                                                    join nodetype_props ntp on ntp.objectclasspropid = ocp.objectclasspropid
                                                    join jct_nodes_props j on j.nodetypepropid = ntp.nodetypepropid
                                                   where jpocp.propertysetid = :permsetid";

                    string SQLQuery = @"with perms as (select n.nodeid,
                                                              (select j.field1_fk " + pvalQuery + @" and j.nodeid = n.nodeid and ocp.propname = :roleocp) userrole,
                                                              (select j.field1_fk " + pvalQuery + @" and j.nodeid = n.nodeid and ocp.propname = :workunitocp) userworkunit,
                                                              (select j.field1_fk " + pvalQuery + @" and j.nodeid = n.nodeid and ocp.propname = :permgroupocp) userpermgroup,
                                                              (select j.field1 " + pvalQuery + @" and j.nodeid = n.nodeid and ocp.propname = :applyallrolesocp) applyallroles,
                                                              (select j.field1 " + pvalQuery + @" and j.nodeid = n.nodeid and ocp.propname = :applyallworkunitsocp) applyallworkunits,
                                                              (select j.field1 " + pvalQuery + @" and j.nodeid = n.nodeid and ocp.propname = :editocp) edit
                                                         from nodes n
                                                         join nodetypes nt on n.nodetypeid = nt.nodetypeid
                                                         join object_class oc on nt.objectclassid = oc.objectclassid
                                                        where n.istemp = 0";
                    if( null != PermObjectClass )
                    {
                        CswNbtMetaDataObjectClass PermOC = _CswNbtResources.MetaData.getObjectClass( PermObjectClass );
                        if( null != PermOC )
                        {
                            SQLQuery += @"                and oc.objectclassid = " + PermOC.ObjectClassId;
                        }
                        else
                        {
                            throw new CswDniException( CswEnumErrorType.Error, "Server Configuration Error", "getUserPermissions() filtered to an invalid or disabled object class (" + PermObjectClass + ")" );
                        }
                    }
                    else
                    {
                        SQLQuery += @"                    and oc.objectclassid in (select jpsoc.objectclassid 
                                                                                 from jct_propertyset_objectclass jpsoc 
                                                                                where jpsoc.propertysetid = :permsetid)";
                    }
                    SQLQuery += @"                   )
                                       select * 
                                         from perms
                                        where (perms.userrole = :role or perms.userrole is null) 
                                          and (perms.userworkunit = :workunit or perms.userworkunit is null)";
                    if( RequireEdit )
                    {
                        SQLQuery += @"        and perms.edit = '" + CswConvert.ToDbVal( (CswEnumTristate) CswEnumTristate.True ) + @"' ";
                    }
                    SQLQuery += @"          order by userpermgroup, applyallroles, applyallworkunits";
                    CswArbitrarySelect Query = _CswNbtResources.makeCswArbitrarySelect( "getUserPermissions", SQLQuery );

                    Query.addParameter( "roleocp", CswNbtPropertySetPermission.PropertyName.Role );
                    Query.addParameter( "workunitocp", CswNbtPropertySetPermission.PropertyName.WorkUnit );
                    Query.addParameter( "permgroupocp", CswNbtPropertySetPermission.PropertyName.PermissionGroup );
                    Query.addParameter( "applyallrolesocp", CswNbtPropertySetPermission.PropertyName.ApplyToAllRoles );
                    Query.addParameter( "applyallworkunitsocp", CswNbtPropertySetPermission.PropertyName.ApplyToAllWorkUnits );
                    Query.addParameter( "editocp", CswNbtPropertySetPermission.PropertyName.Edit );
                    Query.addParameter( "permsetid", PermissionSet.PropertySetId.ToString() );
                    Query.addParameter( "role", RoleId.PrimaryKey.ToString() );
                    Query.addParameter( "workunit", CurrentWorkUnitId.PrimaryKey.ToString() );

                    DataTable DataTable = Query.getTable();
                    foreach( DataRow Row in DataTable.Rows )
                    {
                        CswPrimaryKey PermGroupPk = new CswPrimaryKey( "nodes", CswConvert.ToInt32( Row["userpermgroup"].ToString() ) );
                        CswPrimaryKey PermPk = new CswPrimaryKey( "nodes", CswConvert.ToInt32( Row["nodeid"] ) );
                        if( false == ret.ContainsKey( PermGroupPk ) )
                        {
                            ret.Add( PermGroupPk, PermPk );
                        }
                    }
                } // if( CswTools.IsPrimaryKey( CurrentWorkUnitId ) && CswTools.IsPrimaryKey( RoleId ) )

                // Cache the general case for next time
                if( null == PermObjectClass && false == RequireEdit )
                {
                    _UserPermissions = ret;
                }
            } // if-else( null == PermObjectClass && false == RequireEdit )

            return ret;
        } // getUserPermissions()

        #endregion Permissions Logic

        #region Work Unit Logic

        public Dictionary<string, string> InitAvailableWorkUnitsOptions()
        {
            Dictionary<string, string> opts = new Dictionary<string, string>();

            CswNbtMetaDataObjectClass WorkUnitOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.WorkUnitClass );
            foreach( KeyValuePair<CswPrimaryKey, string> workUnit in WorkUnitOC.getNodeIdAndNames( false, false, RequireViewPermissions: false ) )
            {
                opts[workUnit.Key.ToString()] = workUnit.Value;
            }

            return opts;
        }

        /// <summary>
        /// Gets the Default Work Unit or if that Node does not exist the first found Work Unit (not null safe!)
        /// </summary>
        /// <returns></returns>
        public CswPrimaryKey GetFirstAvailableWorkUnitNodeId()
        {
            CswNbtMetaDataObjectClass WorkUnitOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.WorkUnitClass );
            Dictionary<CswPrimaryKey, string> WorkUnits = WorkUnitOC.getNodeIdAndNames( false, false, RequireViewPermissions: false );
            CswPrimaryKey ret = WorkUnits.OrderBy( entry => entry.Key.PrimaryKey ).First().Key;
            return ret;
        }

        private void _updateAvailableWorkUnits()
        {
            //Case 30817 (Case 30843: moved to here from onBeforeCreate)
            if( CswTools.IsPrimaryKey( CurrentWorkUnitId ) &&
                false == AvailableWorkUnits.CheckValue( CurrentWorkUnitId.ToString() ) )
            {
                AvailableWorkUnits.AddValue( CurrentWorkUnitId.ToString() );
                CurrentWorkUnitProperty.RelatedNodeId = CurrentWorkUnitId;
                CurrentWorkUnitProperty.SyncGestalt();
            }

            CswNbtView View = _CswNbtResources.ViewSelect.restoreView( CurrentWorkUnitProperty.NodeTypeProp.ViewId );

            View.Clear();
            View.SetVisibility( CswEnumNbtViewVisibility.Property, null, null );

            CswNbtMetaDataObjectClass WorkUnitOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.WorkUnitClass );
            CswNbtViewRelationship WorkUnitParent = View.AddViewRelationship( WorkUnitOC, false );

            foreach( string WorkUnitNodeId in AvailableWorkUnits.Value )
            {
                CswPrimaryKey pk = CswConvert.ToPrimaryKey( WorkUnitNodeId );
                WorkUnitParent.NodeIdsToFilterIn.Add( pk );
            }

            CurrentWorkUnitProperty.OverrideView( View );
        }

        #endregion Work Unit Logic

    }//CswNbtObjClassUser

}//namespace ChemSW.Nbt.ObjClasses
