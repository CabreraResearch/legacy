using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Security
{
    public enum CswSystemUserNames { SysUsr_ObjClassCustomer, SysUsr_SchedSvc, SysUsr_SchemaUpdt, SysUsr__SchemaImport, SysUsr_NbtWebSvcMgr, SysUsr_DbConnectTest, SysUsr_Test }
    public class CswNbtSystemUser : ICswNbtUser
    {
        private ICswResources _Resources;
        private CswSystemUserNames _SystemUserName;

        public CswNbtSystemUser( ICswResources CswNbtResources, CswSystemUserNames SystemUserName )
        {
            _Resources = CswNbtResources;
            _SystemUserName = SystemUserName;
        }

        //public bool CheckCreatePermission( int NodeTypeId )
        //{
        //    return true;
        //}
        //public bool CheckActionPermission( CswNbtActionName ActionName )
        //{
        //    return true;
        //}

        //public bool CheckPermission( NodeTypePermission Permission, int NodeTypeId, CswNbtNode Node, CswNbtMetaDataNodeTypeProp MetaDataProp )
        //{
        //    return true;
        //}

        public bool IsAdministrator() { return true; }
        //public bool canEditPassword( CswNbtNode UserNode = null ) { return true; }
        public CswNbtObjClassRole RoleNode { get { return null; } }
        public CswNbtObjClassUser UserNode { get { return null; } }

        public CswPrimaryKey RoleId { get { return null; } }
        public CswPrimaryKey UserId { get { return null; } }

        public Int32 UserNodeTypeId { get { return Int32.MinValue; } }
        public Int32 UserObjectClassId { get { return Int32.MinValue; } }
        public Int32 RoleNodeTypeId { get { return Int32.MinValue; } }
        public Int32 RoleObjectClassId { get { return Int32.MinValue; } }

        public Int32 PasswordPropertyId { get { return Int32.MinValue; } }
        public bool PasswordIsExpired { get { return false; } }

        public CswNbtNodePropText UsernameProperty { get { return null; } }
        public CswNbtNodePropPassword PasswordProperty { get { return null; } }
        public CswNbtNodePropText FirstNameProperty { get { return null; } }
        public CswNbtNodePropText LastNameProperty { get { return null; } }
        public CswNbtNodePropText EmailProperty { get { return null; } }
        public CswNbtNodePropLocation DefaultLocationProperty { get { return null; } }
        public CswNbtNodePropRelationship WorkUnitProperty { get { return null; } }
        public CswPrimaryKey DefaultLocationId { get { return null; } }
        public CswPrimaryKey DefaultPrinterId { get { return null; } }
        public CswPrimaryKey WorkUnitId { get { return null; } }
        public CswPrimaryKey JurisdictionId { get { return null; } }
        public string Language { get { return string.Empty; } }

        public Int32 RoleTimeout { get { return Int32.MinValue; } }

        public string Username { get { return _SystemUserName.ToString(); } }
        public string Rolename { get { return string.Empty; } }
        public string FirstName { get { return string.Empty; } }
        public string LastName { get { return _SystemUserName.ToString(); } }
        public string Email { get { return string.Empty; } }
        public string DateFormat { get { return string.Empty; } }
        public string TimeFormat { get { return string.Empty; } }
        public Int32 PageSize { get { return 50; } }
        public Dictionary<string, string> Cookies { get; set; }

        public void postChanges( bool ForceUpdate )
        {
            // do nothing
        }
    }
}
