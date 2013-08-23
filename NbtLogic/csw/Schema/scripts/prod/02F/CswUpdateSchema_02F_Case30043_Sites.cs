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

            #region CAF binding definitions for Sites
            CswNbtSchemaUpdateImportMgr ImportMgr_Sites = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, 2, "sites", "Site" );

            // Binding 
            ImportMgr_Sites.importBinding( "siteid", "Legacy Id", "", "sites", "Site" );
            ImportMgr_Sites.importBinding( "sitename", "Name", "", "sites", "Site" );
            ImportMgr_Sites.importBinding( "sitecode", "Location Code", "", "sites", "Site" );

            // Relationship
            ImportMgr_Sites.importRelationship( "sites", "Site", "Control Zone", 1 );

            ImportMgr_Sites.finalize();

            #endregion

        } // update()

    } // class CswUpdateSchema_02F_Case30041_Vendors

}//namespace ChemSW.Nbt.Schema