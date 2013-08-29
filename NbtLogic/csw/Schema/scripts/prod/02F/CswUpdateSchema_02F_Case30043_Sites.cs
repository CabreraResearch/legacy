using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;

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

        public override void update()
        {
            // Case 30043 - CAF Migration: Sites/Locations/Work Units
            CswNbtSchemaUpdateImportMgr importMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "sites", "Site", "sites_view" );

            // Binding 
            importMgr.importBinding( "sitename", "Name", "" );
            importMgr.importBinding( "sitecode", "Location Code", "" );
            importMgr.importBinding( "controlzoneid", "Control Zone", "" );

            // Relationship

            importMgr.finalize();

        } // update()

    } // class CswUpdateSchema_02F_Case30041_Vendors

}//namespace ChemSW.Nbt.Schema