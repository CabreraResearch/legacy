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

            #region CAF binding definitions for Control Zones
            CswNbtSchemaUpdateImportMgr ImportMgr_ControlZones = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "cispro_controlzones", "Control Zone" );

            // Binding
            ImportMgr_ControlZones.importBinding( "controlzoneid", "Legacy Id", "", "cispro_controlzones", "Control Zone", 1 );
            ImportMgr_ControlZones.importBinding( "controlzonename", "Name", "", "cispro_controlzones", "Control Zone", 1 );
            ImportMgr_ControlZones.importBinding( "exemptqtyfactor", "MAQ Offset %", "", "cispro_controlzones", "Control Zone", 1 );

            // Relationship
            //none

            ImportMgr_ControlZones.finalize();

            #endregion

        } // update()

    } // class CswUpdateSchema_02F_Case30043_ControlZones

}//namespace ChemSW.Nbt.Schema