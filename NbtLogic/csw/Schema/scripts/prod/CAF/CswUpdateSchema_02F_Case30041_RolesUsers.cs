using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30041_RolesUsers : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30041; }
        }

        public override string AppendToScriptName()
        {
            return "RolesUsers";
        }

        public override void update()
        {
                CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );
                ImpMgr.CAFimportOrder( "Role", "roles" );

                ImpMgr.importBinding( "roledescription", CswNbtObjClassRole.PropertyName.Description, "" );
                ImpMgr.importBinding( "rolename", CswNbtObjClassRole.PropertyName.Name, "" );
                ImpMgr.importBinding( "timeout", CswNbtObjClassRole.PropertyName.Timeout, "" );



                ImpMgr.CAFimportOrder( "User", "users", "users_view" );

                ImpMgr.importBinding( "disabled", CswNbtObjClassUser.PropertyName.Archived, "" );
                ImpMgr.importBinding( "namefirst", CswNbtObjClassUser.PropertyName.FirstName, "" );
                ImpMgr.importBinding( "namelast", CswNbtObjClassUser.PropertyName.LastName, "" );
                ImpMgr.importBinding( "password", CswNbtObjClassUser.PropertyName.Password, "" );
                ImpMgr.importBinding( "email", CswNbtObjClassUser.PropertyName.Email, "" );
                ImpMgr.importBinding( "employeeid", CswNbtObjClassUser.PropertyName.EmployeeId, "" );
                ImpMgr.importBinding( "navrows", CswNbtObjClassUser.PropertyName.PageSize, "" );
                ImpMgr.importBinding( "locked", CswNbtObjClassUser.PropertyName.AccountLocked, "" );
                ImpMgr.importBinding( "failedlogincount", CswNbtObjClassUser.PropertyName.FailedLoginCount, "" );
                ImpMgr.importBinding( "defaultlanguage", CswNbtObjClassUser.PropertyName.Language, "" );
                ImpMgr.importBinding( "phone", CswNbtObjClassUser.PropertyName.Phone, "" );
                ImpMgr.importBinding( "username", CswNbtObjClassUser.PropertyName.Username, "" );

                ImpMgr.importBinding( "defaultlocationid", CswNbtObjClassUser.PropertyName.DefaultLocation, CswEnumNbtSubFieldName.NodeID.ToString() );
                ImpMgr.importBinding( "roleid", CswNbtObjClassUser.PropertyName.Role, CswEnumNbtSubFieldName.NodeID.ToString() );
                ImpMgr.importBinding( "workunitid", CswNbtObjClassUser.PropertyName.CurrentWorkUnit, CswEnumNbtSubFieldName.NodeID.ToString() );

                /*
                   +defaultlocationid, 
                   +disabled, 
                   homeinventorygroupid, - We dont' have an inventory group property on Users
                   +namefirst, 
                   +namelast, 
                   +navrows, 
                   +password,
                   +roleid, 
                   +userid, 
                   +username, 
                   +workunitid, 
                   +locked, 
                   +failedlogincount, 
                   +email, 
                   +phone, 
                   +defaultlanguage,
                   ?? pending TDU investigation supervisorid, 
                   +employeeid
             
                */

                // Exclude issystemuser
                ImpMgr.finalize();

        } // update()

    } // class CswUpdateSchema_02F_Case30041_Vendors

}//namespace ChemSW.Nbt.Schema