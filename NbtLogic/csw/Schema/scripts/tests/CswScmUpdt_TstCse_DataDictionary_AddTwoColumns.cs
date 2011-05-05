using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
//using ChemSW.TblDn;
using ChemSW.Nbt.Schema;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    //bz # 8604
    public class CswScmUpdt_TstCse_DataDictionary_AddTwoColumns : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_DataDictionary_AddTwoColumns()
            : base( "Add two columns to data_dictionary" )
        {
        }//ctor

        private string _TestColumnNameOne = "test_column_one";
        private string _TestColumnNameTwo = "test_column_two";
        private string _DdTableName = "data_dictionary";

        public override void runTest()
        {

            _CswNbtSchemaModTrnsctn.beginTransaction();

            try
            {
                _CswNbtSchemaModTrnsctn.addColumn( _TestColumnNameOne, DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, _DdTableName, DataDictionaryUniqueType.None, false, string.Empty );
                _CswNbtSchemaModTrnsctn.addColumn( _TestColumnNameTwo, DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, _DdTableName, DataDictionaryUniqueType.None, false, string.Empty );
            }

            catch ( Exception Exception )
            {
                throw ( new CswScmUpdt_Exception( "An unexpected exception was thrown when adding two columns to data_dictionary:"  + Exception.Message ) );
            }//catch()
                


            if ( !_CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _DdTableName, _TestColumnNameOne ) )
                throw ( new CswScmUpdt_Exception( "Column " + _TestColumnNameOne + " was not created in data base " ) );

            if ( !_CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _DdTableName, _TestColumnNameTwo ) )
                throw ( new CswScmUpdt_Exception( "Column " + _TestColumnNameTwo + " was not created in data base " ) );

            if ( !_CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _DdTableName, _TestColumnNameOne ) )
                throw ( new CswScmUpdt_Exception( "Column " + _TestColumnNameOne + " was not created in meta data " ) );

            if ( !_CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _DdTableName, _TestColumnNameTwo ) )
                throw ( new CswScmUpdt_Exception( "Column " + _TestColumnNameTwo + " was not created in meta data " ) );


            _CswNbtSchemaModTrnsctn.rollbackTransaction();

       }//runTest()



    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
