using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Test Case: 001, part 03
    /// </summary>
    public class CswTestCase_001_03_003 : ICswUpdateSchemaTo
    {

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'T', 03  ); } }

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name,_CswTstCaseRsrc_001.Purpose, "Tear down add column operation" ) ); } }


        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_001 _CswTstCaseRsrc_001 = null;
        public CswTestCase_001_03_003( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_001 = new CswTstCaseRsrc_001( _CswNbtSchemaModTrnsctn );
        }//ctor



        public void update()
        {

            _CswNbtSchemaModTrnsctn.dropColumn( _CswTstCaseRsrc_001.TestTableName, _CswTstCaseRsrc_001.TestColumnNameOne );
            _CswNbtSchemaModTrnsctn.dropColumn( _CswTstCaseRsrc_001.TestTableName, _CswTstCaseRsrc_001.TestColumnNameTwo ); 

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
