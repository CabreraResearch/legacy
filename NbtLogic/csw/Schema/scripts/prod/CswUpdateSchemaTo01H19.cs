using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using System.IO;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-19
    /// </summary>
    public class CswUpdateSchemaTo01H19 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null; 

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 19 ); } }

        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H19( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            // case 20929
            // no object class props should have isrequired = 1, setvalonadd = 0, and defaultvalueid null

            CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H19_OCP_Update", "object_class_props" );
            DataTable OCPTable = OCPUpdate.getTable( "where isrequired = '" + CswConvert.ToDbVal( true ).ToString() + "'" +
                                                     "  and (setvalonadd = '" + CswConvert.ToDbVal( false ).ToString() + "' or setvalonadd is null)" +
                                                     "  and (defaultvalueid is null" +
                                                     "   or not exists (select jctnodepropid from jct_nodes_props where jctnodepropid = defaultvalueid))" );
            foreach( DataRow OCPRow in OCPTable.Rows )
            {
                OCPRow["setvalonadd"] = CswConvert.ToDbVal( true );
            }
            OCPUpdate.update( OCPTable );

        } // update()

    }//class CswUpdateSchemaTo01H19

}//namespace ChemSW.Nbt.Schema


