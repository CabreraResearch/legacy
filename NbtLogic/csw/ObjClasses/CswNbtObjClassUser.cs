using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.Security;
using ChemSW.Security;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassUser : CswNbtObjClass, ICswNbtUser
    {
        public const string ChemSWAdminUsername = CswAuthenticator.ChemSWAdminUsername;

        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string Role = "Role";
            public const string AccountLocked = "AccountLocked";
            public const string FailedLoginCount = "FailedLoginCount";
            public const string Password = "Password";
            public const string Username = "Username";
            public const string FirstName = "First Name";
            public const string Language = "Language";
            public const string LastName = "Last Name";
            public const string LastLogin = "Last Login";
            public const string FavoriteViews = "Favorite Views";
            public const string FavoriteActions = "Favorite Actions";
            public const string Email = "Email";
            public const string PageSize = "Page Size";
            public const string DateFormat = "Date Format";
            public const string TimeFormat = "Time Format";
            public const string DefaultLocation = "Default Location";
            public const string DefaultPrinter = "Default Printer";
            public const string WorkUnit = "Work Unit";
            public const string LogLevel = "Log Level";
            public const string Archived = "Archived";
            public const string Jurisdiction = "Jurisdiction";
            public const string Barcode = "Barcode";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;


        public CswNbtObjClassUser( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }

        //ctor()

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

        public new void postChanges( bool ForceUpdate ) //bz# 5446
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

        public static string getValidUserName( string Name )
        {
            return Regex.Replace( Name, "[^a-zA-Z0-9_.]+", "" );
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

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
        }

        public override void afterCreateNode()
        {
        }

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( _unableToWriteNodeInvalidUserName() )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Username must contain alphanumeric characters only.",
                                          "Username contains invalid characters: " + this.Username );
            }

            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );

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

        }

        //beforeWriteNode()

        private bool _unableToWriteNodeInvalidUserName()
        {
            return
                false == String.IsNullOrEmpty( this.UsernameProperty.Text ) &&
                false == CswTools.IsValidUsername( this.UsernameProperty.Text ) &&
                ( this.UsernameProperty.WasModified ||
                 ( this.AccountLocked.WasModified && this.AccountLocked.Checked == CswEnumTristate.False ) );
        }

        public override void afterWriteNode()
        {
            //bz # 6555
            if( AccountLocked.Checked != CswEnumTristate.True && AccountLocked.WasModified )
            {
                clearFailedLoginCount();
            }

            // BZ 9170
            _CswNbtResources.ConfigVbls.setConfigVariableValue( "cache_lastupdated", DateTime.Now.ToString() );

            _CswNbtObjClassDefault.afterWriteNode();
        }

        //afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

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

        //beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }

        //afterDeleteNode()        

        protected override void afterPopulateProps()
        {

            UsernameProperty.SetOnPropChange( OnUserNamePropChange );

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
            if( IsPasswordReadyOnly )
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

            //_CswNbtObjClassDefault.afterPopulateProps();

            _CswNbtObjClassDefault.triggerAfterPopulateProps();


        }

        //afterPopulateProps()

        public bool IsPasswordReadyOnly
        {
            get
            {
                bool ReturnVal = false;

                if( ( null == _CswNbtResources.CurrentNbtUser ) || ( ( this.NodeId != _CswNbtResources.CurrentNbtUser.UserId ) && ( false == _CswNbtResources.CurrentNbtUser.IsAdministrator() ) ) )
                {
                    ReturnVal = true;
                }

                return ( ReturnVal );

            }//get

        }//IsPasswordReadyOnly

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            //case 24525 - add default filter to ignore archived users in relationship props
            CswNbtView view = ParentRelationship.View;
            CswNbtMetaDataObjectClass userOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp archivedOCP = userOC.getObjectClassProp( PropertyName.Archived );
            view.AddViewPropertyAndFilter( ParentRelationship, archivedOCP, FilterMode: CswEnumNbtFilterMode.NotEquals, Value: CswEnumTristate.True.ToString() );

            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        
        #endregion

        #region Object class specific properties

        public CswPrimaryKey UserId { get { return _CswNbtNode.NodeId; } }
        public CswPrimaryKey RoleId { get { return Role.RelatedNodeId; } }

        public CswNbtObjClassUser UserNode { get { return this; } }
        public CswNbtObjClassRole RoleNode { get { return _RoleNodeObjClass; } }
        public string Rolename { get { return _RoleNodeObjClass.Name.Text; } }
        public Int32 RoleTimeout { get { return CswConvert.ToInt32( _RoleNodeObjClass.Timeout.Value ); } }

        public CswNbtNodePropRelationship Role { get { return ( _CswNbtNode.Properties[PropertyName.Role] ); } }
        private void onRolePropChange( CswNbtNodeProp NodeProp )
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
        private void onDateFormatPropChange( CswNbtNodeProp NodeProp )
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
        private void onTimeFormatPropChange( CswNbtNodeProp NodeProp )
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
        public CswNbtNodePropRelationship WorkUnitProperty { get { return _CswNbtNode.Properties[PropertyName.WorkUnit]; } }
        public CswPrimaryKey WorkUnitId { get { return WorkUnitProperty.RelatedNodeId; } }
        public CswNbtNodePropLogical Archived { get { return _CswNbtNode.Properties[PropertyName.Archived]; } }
        public CswNbtNodePropRelationship JurisdictionProperty { get { return _CswNbtNode.Properties[PropertyName.Jurisdiction]; } }
        public CswPrimaryKey JurisdictionId { get { return JurisdictionProperty.RelatedNodeId; } }
        public CswNbtNodePropBarcode Barcode { get { return ( _CswNbtNode.Properties[PropertyName.Barcode] ); } }
        public CswNbtNodePropList LanguageProperty { get { return ( _CswNbtNode.Properties[PropertyName.Language] ); } }
        public string Language { get { return ( LanguageProperty.Value ); } }


        private void OnUserNamePropChange( CswNbtNodeProp Prop )
        {
            if( false == Prop.Empty )
            {
                Prop.setReadOnly( true, true );
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

        private Dictionary<CswPrimaryKey, CswNbtPropertySetPermission> _NodePermissions;
        public Dictionary<CswPrimaryKey, CswNbtPropertySetPermission> NodePermissions
        {
            get
            {
                if( null == _NodePermissions )
                {
                    _NodePermissions = new Dictionary<CswPrimaryKey, CswNbtPropertySetPermission>();
                    _updateNodePermissions();
                }
                return _NodePermissions;
            }
        }

        /// <summary>
        /// Returns the Permission node (if one exists) for this User for the given Permission GroupId
        /// </summary>
        public CswNbtPropertySetPermission getPermissionForGroup( CswPrimaryKey PermissionGroupId )
        {
            CswNbtPropertySetPermission PermissionNode = null;
            if( null == _NodePermissions )
            {
                _NodePermissions = new Dictionary<CswPrimaryKey, CswNbtPropertySetPermission>();
                _updateNodePermissions();
            }
            if( _NodePermissions.ContainsKey( PermissionGroupId ) )
            {
                PermissionNode = _NodePermissions[PermissionGroupId];
            }
            return PermissionNode;
        }

        //public 

        private void _updateNodePermissions()
        {
            if( null == _NodePermissions )
            {
                _NodePermissions = new Dictionary<CswPrimaryKey, CswNbtPropertySetPermission>();
            }
            else
            {
                _NodePermissions.Clear();
            }
            if( CswTools.IsPrimaryKey( WorkUnitId ) && CswTools.IsPrimaryKey( RoleId ) )
            {
                Collection<CswPrimaryKey> UserPermissions = getUserPermissions( RoleId.PrimaryKey, WorkUnitId.PrimaryKey );
                foreach( CswPrimaryKey PermissionId in UserPermissions )
                {
                    CswNbtPropertySetPermission PermNode = _CswNbtResources.Nodes[PermissionId];
                    if( null != PermNode )
                    {
                        _NodePermissions.Add( PermNode.PermissionGroup.RelatedNodeId, PermNode );
                    }
                }
            }
        }

        public Collection<CswPrimaryKey> getUserPermissions( Int32 RolePK, Int32 WorkUnitPK )
        {
            CswNbtMetaDataPropertySet PermissionSet = _CswNbtResources.MetaData.getPropertySet( CswEnumNbtPropertySetName.PermissionSet );
            #region SQL Query
            String SQLQuery = @"with pval as
    (select j.nodeid, j.field1, j.field1_fk, ocp.objectclasspropid from jct_nodes_props j
    join nodetype_props ntp on j.nodetypepropid = ntp.nodetypepropid
    join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid),
    roleocps as
    (select jpocp.objectclasspropid from jct_propertyset_ocprop jpocp
    join object_class_props ocp on ocp.objectclasspropid = jpocp.objectclasspropid
    where jpocp.propertysetid = :permsetid and ocp.propname = :roleocp),
    workunitocps as
    (select jpocp.objectclasspropid from jct_propertyset_ocprop jpocp
    join object_class_props ocp on ocp.objectclasspropid = jpocp.objectclasspropid
    where jpocp.propertysetid = :permsetid and ocp.propname = :workunitocp),
    permgrpocps as
    (select jpocp.objectclasspropid from jct_propertyset_ocprop jpocp
    join object_class_props ocp on ocp.objectclasspropid = jpocp.objectclasspropid
    where jpocp.propertysetid = :permsetid and ocp.propname = :permgroupocp),
    applyrolesocps as
    (select jpocp.objectclasspropid from jct_propertyset_ocprop jpocp
    join object_class_props ocp on ocp.objectclasspropid = jpocp.objectclasspropid
    where jpocp.propertysetid = :permsetid and ocp.propname = :applyallrolesocp),
    applyworkunitsocps as
    (select jpocp.objectclasspropid from jct_propertyset_ocprop jpocp
    join object_class_props ocp on ocp.objectclasspropid = jpocp.objectclasspropid
    where jpocp.propertysetid = :permsetid and ocp.propname = :applyallworkunitsocp)
select * from (
    select n.nodeid,
    (select p.field1_fk from pval p where p.nodeid = n.nodeid and p.objectclasspropid in (select * from roleocps) ) userrole,
    (select p.field1_fk from pval p where p.nodeid = n.nodeid and p.objectclasspropid in (select * from workunitocps) ) userworkunit,
    (select p.field1_fk from pval p where p.nodeid = n.nodeid and p.objectclasspropid in (select * from permgrpocps) ) userpermgroup,
    (select p.field1 from pval p where p.nodeid = n.nodeid and p.objectclasspropid in (select * from applyrolesocps) ) applyallroles,
    (select p.field1 from pval p where p.nodeid = n.nodeid and p.objectclasspropid in (select * from applyworkunitsocps) ) applyallworkunits
    from nodes n
        join nodetypes nt on n.nodetypeid = nt.nodetypeid
        join object_class oc on nt.objectclassid = oc.objectclassid
    where n.nodeid is not null
     and n.istemp = 0
        and oc.objectclassid in (select jpsoc.objectclassid from jct_propertyset_objectclass jpsoc where jpsoc.propertysetid = :permsetid )
    order by userpermgroup, applyallroles, applyallworkunits
) perms
    where (perms.userrole = :role or perms.userrole is null) 
    and (perms.userworkunit = :workunit or perms.userworkunit is null)";
            #endregion SQL Query

            CswArbitrarySelect Query = _CswNbtResources.makeCswArbitrarySelect( "getUserPermissions", SQLQuery );
            #region SQL Params
            Query.addParameter( "roleocp", CswNbtPropertySetPermission.PropertyName.Role );
            Query.addParameter( "workunitocp", CswNbtPropertySetPermission.PropertyName.WorkUnit );
            Query.addParameter( "permgroupocp", CswNbtPropertySetPermission.PropertyName.PermissionGroup );
            Query.addParameter( "applyallrolesocp", CswNbtPropertySetPermission.PropertyName.ApplyToAllRoles );
            Query.addParameter( "applyallworkunitsocp", CswNbtPropertySetPermission.PropertyName.ApplyToAllWorkUnits );
            Query.addParameter( "permsetid", PermissionSet.PropertySetId.ToString() );
            Query.addParameter( "role", RolePK.ToString() );
            Query.addParameter( "workunit", WorkUnitPK.ToString() );
            #endregion SQL Params
            DataTable DataTable = Query.getTable();
            Collection<CswPrimaryKey> UserPermissions = new Collection<CswPrimaryKey>();
            Int32 PermGroupId = 0;
            foreach( DataRow Row in DataTable.Rows )
            {
                if( PermGroupId != CswConvert.ToInt32( Row["userpermgroup"].ToString() ) )
                {
                    PermGroupId = CswConvert.ToInt32( Row["userpermgroup"].ToString() );
                    UserPermissions.Add( new CswPrimaryKey( "nodes", CswConvert.ToInt32( Row["nodeid"] ) ) );
                }
            }
            return UserPermissions;
        }

    }//CswNbtObjClassUser

}//namespace ChemSW.Nbt.ObjClasses
