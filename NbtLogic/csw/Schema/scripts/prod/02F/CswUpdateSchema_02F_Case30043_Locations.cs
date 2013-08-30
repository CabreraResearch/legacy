using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30043_Locations : CswUpdateSchemaTo
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

            #region CAF binding definitions for Locations
            CswNbtSchemaUpdateImportMgr ImportMgr_Locations = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "locations_level1", "Building" );

            // Binding 
            ImportMgr_Locations.importBinding( "locationlevel1name", "Name", "", null, null, 2 );
            //_importBinding( "locations_level1", "locationlevel1name", "Building", "Name", "", 2 );

            // Relationship
            ImportMgr_Locations.importRelationship( "locations_level1", "Building", "Location", 1 );

            ImportMgr_Locations.finalize();

            #endregion

        } // update()

    } // class CswUpdateSchema_02F_Case30041_Vendors

}//namespace ChemSW.Nbt.Schema