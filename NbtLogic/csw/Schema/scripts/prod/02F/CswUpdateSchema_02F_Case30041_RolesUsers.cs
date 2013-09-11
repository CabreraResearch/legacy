using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
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

        public override string ScriptName
        {
            get { return "02F_Case30041_RolesUsers"; }
        }

        public override void update()
        {
            {
                CswNbtSchemaUpdateImportMgr RoleImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "roles", "Role" );

                // Bindings
                RoleImpMgr.importBinding( "roledescription", CswNbtObjClassRole.PropertyName.Description, "" );
                RoleImpMgr.importBinding( "rolename", CswNbtObjClassRole.PropertyName.Name, "" );
                RoleImpMgr.importBinding( "timeout", CswNbtObjClassRole.PropertyName.Timeout, "" );

                // Relationships
                // none

                RoleImpMgr.finalize();
            }
            {
                // Bindings
                CswNbtSchemaUpdateImportMgr UserImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "users", "User" );
                UserImpMgr.importBinding( "disabled", CswNbtObjClassUser.PropertyName.Archived, "" );
                UserImpMgr.importBinding( "namefirst", CswNbtObjClassUser.PropertyName.FirstName, "" );
                UserImpMgr.importBinding( "namelast", CswNbtObjClassUser.PropertyName.LastName, "" );
                UserImpMgr.importBinding( "password", CswNbtObjClassUser.PropertyName.Password, "" );
                UserImpMgr.importBinding( "email", CswNbtObjClassUser.PropertyName.Email, "" );
                UserImpMgr.importBinding( "employeeid", CswNbtObjClassUser.PropertyName.EmployeeId, "" );
                UserImpMgr.importBinding( "navrows", CswNbtObjClassUser.PropertyName.PageSize, "" );
                UserImpMgr.importBinding( "locked", CswNbtObjClassUser.PropertyName.AccountLocked, "" );
                UserImpMgr.importBinding( "failedlogincount", CswNbtObjClassUser.PropertyName.FailedLoginCount, "" );
                UserImpMgr.importBinding( "defaultlanguage", CswNbtObjClassUser.PropertyName.Language, "" );
                UserImpMgr.importBinding( "phone", CswNbtObjClassUser.PropertyName.Phone, "" );
                UserImpMgr.importBinding( "username", CswNbtObjClassUser.PropertyName.Username, "" );

                // Relationships
                UserImpMgr.importBinding( "defaultlocationid", CswNbtObjClassUser.PropertyName.DefaultLocation, "" );
                //UserImpMgr.importBinding( "homeinventorygroupid", CswNbtObjClassUser.PropertyName. );
                UserImpMgr.importBinding( "roleid", CswNbtObjClassUser.PropertyName.Role, "" );
                UserImpMgr.importBinding( "workunitid", CswNbtObjClassUser.PropertyName.WorkUnit, "" );


                //TODO: complete relationships

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
                UserImpMgr.finalize( WhereClause: " issystemuser != '1' " );
            }

        } // update()

    } // class CswUpdateSchema_02F_Case30041_Vendors

}//namespace ChemSW.Nbt.Schema