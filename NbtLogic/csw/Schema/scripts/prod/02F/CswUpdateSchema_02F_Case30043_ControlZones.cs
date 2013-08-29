using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30043_ControlZones : CswUpdateSchemaTo
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
            CswNbtSchemaUpdateImportMgr ImportMgr_ControlZones = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "cispro_controlzones", "Control Zone" );

            // Binding
            ImportMgr_ControlZones.importBinding( "controlzonename", "Name", "" );
            ImportMgr_ControlZones.importBinding( "exemptqtyfactor", "MAQ Offset %", "" );

            // Relationship
            //none

            ImportMgr_ControlZones.finalize();

        } // update()

    } // class CswUpdateSchema_02F_Case30043_ControlZones

}//namespace ChemSW.Nbt.Schema