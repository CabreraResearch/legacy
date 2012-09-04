using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Security
{
    /// <summary>
    /// This is a read-only store of current user information
    /// To modify the user, you have to fetch the actual user Node using the UserId
    /// </summary>

    public class CswNbtUser : ICswNbtUser
    {
        private CswNbtResources _CswNbtResources;

        private Dictionary<string, string> _UserPropDict;
        private Dictionary<string, string> _RolePropDict;

        private Int32 _RoleId;
        private Int32 _UserId;

        private Int32 _UserNodeTypeId;
        private Int32 _UserObjectClassId;
        private Int32 _RoleNodeTypeId;
        private Int32 _RoleObjectClassId;

        private Int32 _PasswordPropertyId;

        public Int32 UserNodeTypeId { get { return _UserNodeTypeId; } }
        public Int32 UserObjectClassId { get { return _UserObjectClassId; } }
        public Int32 RoleNodeTypeId { get { return _RoleNodeTypeId; } }
        public Int32 RoleObjectClassId { get { return _RoleObjectClassId; } }

        public Dictionary<string, string> Cookies { get; set; }

        private const string _FkSuffix = "_fk";
        private const string _DateSuffix = "_date";

        public CswNbtUser( CswNbtResources Resources, string Username )
        {
            _CswNbtResources = Resources;

            // We can't use a CswTableSelect on a view, apparently.  So we'll use a direct select
            //CswTableSelect UserSelect = _CswNbtResources.makeCswTableSelect( "CswNbtUser_User_Select", "vwNbtUser" );
            //DataTable UserTable = UserSelect.getTable( "where username = '" + Username + "'" );
            string UserSelect = "select * from vwNbtUser where lower(username) = '" + Username.ToLower() + "'";
            DataTable UserTable = _CswNbtResources.execArbitraryPlatformNeutralSqlSelect( "CswNbtUser_User_Select", UserSelect );

            if( UserTable.Rows.Count > 0 )
            {
                _UserPropDict = new Dictionary<string, string>();
                _UserId = CswConvert.ToInt32( UserTable.Rows[0]["nodeid"] );
                _UserNodeTypeId = CswConvert.ToInt32( UserTable.Rows[0]["nodetypeid"] );
                _UserObjectClassId = CswConvert.ToInt32( UserTable.Rows[0]["objectclassid"] );
                foreach( DataRow Row in UserTable.Rows )
                {
                    string ObjectClassPropName = Row["propname"].ToString();
                    string ObjectClassPropValue = Row["gestalt"].ToString();
                    string ObjectClassPropValueFk = Row["field1_fk"].ToString();
                    string ObjectClassPropValueDate = Row["field1_date"].ToString();

                    if( ObjectClassPropName == CswNbtObjClassUser.PropertyName.Role )
                    {
                        _RoleId = CswConvert.ToInt32( Row["field1_fk"] );
                    }
                    if( ObjectClassPropName == CswNbtObjClassUser.PropertyName.Password )
                    {
                        _PasswordPropertyId = CswConvert.ToInt32( Row["nodetypepropid"] );
                    }

                    _UserPropDict[ObjectClassPropName] = ObjectClassPropValue;
                    if( ObjectClassPropValueFk != string.Empty )
                    {
                        _UserPropDict[ObjectClassPropName + _FkSuffix] = ObjectClassPropValueFk;
                    }
                    if( ObjectClassPropValueDate != string.Empty )
                    {
                        _UserPropDict[ObjectClassPropName + _DateSuffix] = ObjectClassPropValueDate;
                    }
                } // foreach( DataRow Row in UserTable.Rows )
            } // if( UserTable.Rows.Count > 0 )

            //CswTableSelect RoleSelect = _CswNbtResources.makeCswTableSelect( "CswNbtUser_Role_Select", "vwNbtRole" );
            //DataTable RoleTable = RoleSelect.getTable( "where roleid = '" + _RoleId.ToString() + "'" );
            string RoleSelect = "select * from vwNbtRole where nodeid = '" + _RoleId.ToString() + "'";
            DataTable RoleTable = _CswNbtResources.execArbitraryPlatformNeutralSqlSelect( "CswNbtUser_Role_Select", RoleSelect );
            if( RoleTable.Rows.Count > 0 )
            {
                _RolePropDict = new Dictionary<string, string>();
                _RoleNodeTypeId = CswConvert.ToInt32( RoleTable.Rows[0]["nodetypeid"] );
                _RoleObjectClassId = CswConvert.ToInt32( RoleTable.Rows[0]["objectclassid"] );
                foreach( DataRow Row in RoleTable.Rows )
                {
                    string ObjectClassPropName = Row["propname"].ToString();
                    string ObjectClassPropValue = Row["gestalt"].ToString();
                    _RolePropDict[ObjectClassPropName] = ObjectClassPropValue;
                }
            }
        } // constructor

        public bool IsAdministrator()
        {
            return CswConvert.ToBoolean( _RolePropDict[CswNbtObjClassRole.PropertyName.Administrator] );
        }

        public string Email
        {
            get { return _UserPropDict[CswNbtObjClassUser.PropertyName.Email]; }
        }

        public Int32 PageSize
        {
            get { return CswConvert.ToInt32( _UserPropDict[CswNbtObjClassUser.PropertyName.PageSize] ); }
        }

        public CswPrimaryKey UserId
        {
            get { return new CswPrimaryKey( "nodes", _UserId ); }
        }

        public CswPrimaryKey RoleId
        {
            get { return new CswPrimaryKey( "nodes", _RoleId ); }
        }

        public string Username
        {
            get { return _UserPropDict[CswNbtObjClassUser.PropertyName.Username]; }
        }

        public string Rolename
        {
            get { return _RolePropDict[CswNbtObjClassRole.PropertyName.Name]; }
        }

        public Int32 RoleTimeout
        {
            get { return CswConvert.ToInt32( _RolePropDict[CswNbtObjClassRole.PropertyName.Timeout] ); }
        }

        public string FirstName
        {
            get { return _UserPropDict[CswNbtObjClassUser.PropertyName.FirstName]; }
        }

        public string LastName
        {
            get { return _UserPropDict[CswNbtObjClassUser.PropertyName.LastName]; }
        }

        public string DateFormat
        {
            get
            {
                string ret = _UserPropDict[CswNbtObjClassUser.PropertyName.DateFormat];
                if( string.Empty == ret )
                {
                    ret = CswDateTime.DefaultDateFormat.ToString();
                }
                return ret;
            }
        } // DateFormat

        public string TimeFormat
        {
            get
            {
                string ret = _UserPropDict[CswNbtObjClassUser.PropertyName.TimeFormat];
                if( string.Empty == ret )
                {
                    ret = CswDateTime.DefaultTimeFormat.ToString();
                }
                return ret;
            }
        } // TimeFormat

        public CswPrimaryKey DefaultLocationId
        {
            get
            {
                CswPrimaryKey ret = null;
                if( _UserPropDict.ContainsKey( CswNbtObjClassUser.PropertyName.DefaultLocation + _FkSuffix ) )
                {
                    ret = new CswPrimaryKey( "nodes", CswConvert.ToInt32( _UserPropDict[CswNbtObjClassUser.PropertyName.DefaultLocation + _FkSuffix] ) );
                }
                return ret;
            }
        }

        public CswPrimaryKey WorkUnitId
        {
            get
            {
                CswPrimaryKey ret = null;
                if( _UserPropDict.ContainsKey( CswNbtObjClassUser.PropertyName.WorkUnit + _FkSuffix ) )
                {
                    ret = new CswPrimaryKey( "nodes", CswConvert.ToInt32( _UserPropDict[CswNbtObjClassUser.PropertyName.WorkUnit + _FkSuffix] ) );
                }
                return ret;
            }
        }

        public Int32 PasswordPropertyId
        {
            get { return _PasswordPropertyId; }
        }

        public bool PasswordIsExpired
        {
            get
            {
                DateTime ChangedDate = DateTime.MinValue;
                if( _UserPropDict.ContainsKey( CswNbtObjClassUser.PropertyName.Password + _DateSuffix ) )
                {
                    ChangedDate = CswConvert.ToDateTime( _UserPropDict[CswNbtObjClassUser.PropertyName.Password + _DateSuffix] );
                }
                Int32 PasswordExpiryDays = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( "passwordexpiry_days" ) );
                return ( ChangedDate == DateTime.MinValue ||
                         ChangedDate.AddDays( PasswordExpiryDays ).Date <= DateTime.Now.Date );
            }
        }

    } // CswNbtUser

}//ChemSW.NbtResources
