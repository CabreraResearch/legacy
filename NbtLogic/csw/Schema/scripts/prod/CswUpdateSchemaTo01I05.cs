using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01I-05
    /// </summary>
    public class CswUpdateSchemaTo01I05 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;
        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 05 ); } }
        public CswUpdateSchemaTo01I05( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }


        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }


        public void update()
        {
            _CswNbtSchemaModTrnsctn.createAction(CswNbtActionName.Import_Inspection_Questions, true, string.Empty, "Inspections");
        }//Update()

    }//class CswUpdateSchemaTo01I05

}//namespace ChemSW.Nbt.Schema


