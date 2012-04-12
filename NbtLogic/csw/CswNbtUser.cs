using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Security;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
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

        private const string _FkSuffix = "_fk";
        private const string _DateSuffix = "_date";

        public CswNbtUser( CswNbtResources Resources, string Username )
        {
            _CswNbtResources = Resources;

            CswTableSelect UserSelect = _CswNbtResources.makeCswTableSelect( "CswNbtUser_User_Select", "vwNbtUser" );
            DataTable UserTable = UserSelect.getTable( "where username = '" + Username + "'" );
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

                    if( ObjectClassPropName == CswNbtObjClassUser.RolePropertyName )
                    {
                        _RoleId = CswConvert.ToInt32( Row["field1_fk"] );
                    }
                    if( ObjectClassPropName == CswNbtObjClassUser.PasswordPropertyName )
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
                }
            }

            CswTableSelect RoleSelect = _CswNbtResources.makeCswTableSelect( "CswNbtUser_Role_Select", "vwNbtRole" );
            DataTable RoleTable = RoleSelect.getTable( "where roleid = '" + _RoleId.ToString() + "'" );
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
            return CswConvert.ToBoolean( _RolePropDict[CswNbtObjClassRole.AdministratorPropertyName] );
        }

        public string Email
        {
            get { return _UserPropDict[CswNbtObjClassUser.EmailPropertyName]; }
        }

        public Int32 PageSize
        {
            get { return CswConvert.ToInt32( _UserPropDict[CswNbtObjClassUser.PageSizePropertyName] ); }
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
            get { return _UserPropDict[CswNbtObjClassUser.UsernamePropertyName]; }
        }

        public string Rolename
        {
            get { return _UserPropDict[CswNbtObjClassRole.NamePropertyName]; }
        }

        public Int32 RoleTimeout
        {
            get { return CswConvert.ToInt32( _RolePropDict[CswNbtObjClassRole.TimeoutPropertyName] ); }
        }

        public string FirstName
        {
            get { return _UserPropDict[CswNbtObjClassUser.FirstNamePropertyName]; }
        }

        public string LastName
        {
            get { return _UserPropDict[CswNbtObjClassUser.LastNamePropertyName]; }
        }

        public string DateFormat
        {
            get { return _UserPropDict[CswNbtObjClassUser.DateFormatPropertyName]; }
        }

        public string TimeFormat
        {
            get { return _UserPropDict[CswNbtObjClassUser.TimeFormatPropertyName]; }
        }

        public CswPrimaryKey DefaultLocationId
        {
            get { return new CswPrimaryKey( "nodes", CswConvert.ToInt32( _UserPropDict[CswNbtObjClassUser.DefaultLocationPropertyName + _FkSuffix] ) ); }
        }

        public CswPrimaryKey WorkUnitId
        {
            get { return new CswPrimaryKey( "nodes", CswConvert.ToInt32( _UserPropDict[CswNbtObjClassUser.WorkUnitPropertyName + _FkSuffix] ) ); }
        }

        public Int32 PasswordPropertyId
        {
            get { return _PasswordPropertyId; }
        }

        public bool PasswordIsExpired
        {
            get
            {
                DateTime ChangedDate = CswConvert.ToDateTime( _UserPropDict[CswNbtObjClassUser.PasswordPropertyName + _DateSuffix] );
                Int32 PasswordExpiryDays = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( "passwordexpiry_days" ) );
                return ( ChangedDate == DateTime.MinValue ||
                         ChangedDate.AddDays( PasswordExpiryDays ).Date <= DateTime.Now.Date );
            }
        }

    } // CswNbtUser

}//ChemSW.NbtResources
