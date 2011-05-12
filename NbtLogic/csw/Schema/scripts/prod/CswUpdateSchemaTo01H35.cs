
using System.Collections.Generic;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-35
    /// </summary>
    public class CswUpdateSchemaTo01H35 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 35 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H35( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            // case 21744
            List<CswNbtView> AllFeInspectionPoints = _CswNbtSchemaModTrnsctn.restoreViews( "All FE Inspection Points" );
            foreach( CswNbtView View in AllFeInspectionPoints )
            {
                View.Category = "Inspections";
                View.save();
            }

        } // update()

    }//class CswUpdateSchemaTo01H35

}//namespace ChemSW.Nbt.Schema

