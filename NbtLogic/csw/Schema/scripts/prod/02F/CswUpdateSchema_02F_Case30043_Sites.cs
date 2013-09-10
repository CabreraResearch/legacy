using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30043_Sites : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30043; }
        }

        public override string ScriptName
        {
            get { return "02F_Case30043_Sites"; }
        }

        public override void update()
        {
            // Case 30043 - CAF Migration: Sites/Locations/Work Units

            // View creation script
            //create or replace view sites_view as
            //select s.*, cz.controlzoneid
            //from sites s
            //left outer join cispro_controlzones cz on (cz.siteid = s.siteid);

            CswNbtSchemaUpdateImportMgr importMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "sites", "Site", "sites_view" );

            // Binding 
            importMgr.importBinding( "sitename", CswNbtObjClassLocation.PropertyName.Name, "" );
            importMgr.importBinding( "sitecode", CswNbtObjClassLocation.PropertyName.LocationCode, "" );
            importMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, "" );

            // Relationship

            importMgr.finalize( null, null, true );

        } // update()

    } // class CswUpdateSchema_02F_Case30041_Vendors

}//namespace ChemSW.Nbt.Schema