using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.DB;
using ChemSW.Nbt.Schema;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_007_01 : CswUpdateSchemaTo
    {

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_007.Purpose, "add columns" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_007 _CswTstCaseRsrc_007 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_007_01( CswSchemaVersion CswSchemaVersion )
        {
            _CswSchemaVersion = CswSchemaVersion;
        }//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_007 = new CswTstCaseRsrc_007( _CswNbtSchemaModTrnsctn );
			
			try
            {
                _CswNbtSchemaModTrnsctn.addColumn( _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ), DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, _CswTstCaseRsrc.getRealTestTableName(TestTableNamesReal.DataDictionary), DataDictionaryUniqueType.None, false, string.Empty );
                _CswNbtSchemaModTrnsctn.addColumn( _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn02 ), DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, _CswTstCaseRsrc.getRealTestTableName( TestTableNamesReal.DataDictionary ), DataDictionaryUniqueType.None, false, string.Empty );
            }

            catch ( Exception Exception )
            {
                throw ( new CswDniException( "An unexpected exception was thrown when adding two columns to data_dictionary:"  + Exception.Message ) );
            }//catch()  
        
        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
