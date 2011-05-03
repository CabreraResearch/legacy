using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_002_01_005 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'T', 05 ); } }

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrcTableManipulations.Purpose, "Verify tear down" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_001 _CswTstCaseRsrcTableManipulations = null;
        public CswTestCase_002_01_005( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrcTableManipulations = new CswTstCaseRsrc_001( _CswNbtSchemaModTrnsctn );

            
            
        }//ctor


        private string _TestValStem = "Test val ";

        public void update()
        {
            _CswNbtSchemaModTrnsctn.beginTransaction();

            _CswNbtSchemaModTrnsctn.addColumn( _CswTstCaseRsrcTableManipulations.TestColumnNameOne, DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, _CswTstCaseRsrcTableManipulations.TestTableName, DataDictionaryUniqueType.None, false, string.Empty );


            if( !_CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _CswTstCaseRsrcTableManipulations.TestTableName, _CswTstCaseRsrcTableManipulations.TestColumnNameOne ) )
                throw ( new CswDniException( "Column " + _CswTstCaseRsrcTableManipulations.TestColumnNameOne + " was not created in data base " ) );

            if( !_CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _CswTstCaseRsrcTableManipulations.TestTableName, _CswTstCaseRsrcTableManipulations.TestColumnNameOne ) )
                throw ( new CswDniException( "Column " + _CswTstCaseRsrcTableManipulations.TestColumnNameOne + " was not created in meta data " ) );

            CswTableUpdate TestTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "CswScmUpdt_TstCse_Column_RollbackAdd_update", _CswTstCaseRsrcTableManipulations.TestTableName );

            Int32 TotalUpdated = 0;
            DataTable TestTable = TestTableUpdate.getTable();
            foreach( DataRow CurrentRow in TestTable.Rows )
            {
                CurrentRow[_CswTstCaseRsrcTableManipulations.TestColumnNameOne] = "Test val " + TestTable.Rows.IndexOf( CurrentRow ).ToString();
                TotalUpdated++;
            }

            TestTableUpdate.update( TestTable );

            Int32 TotalUpdatedInfact = 0;
            TestTable = TestTableUpdate.getTable();
            foreach( DataRow CurrentRow in TestTable.Rows )
            {
                if( ( _TestValStem + TestTable.Rows.IndexOf( CurrentRow ) ) == CurrentRow[_CswTstCaseRsrcTableManipulations.TestColumnNameOne].ToString() )
                    TotalUpdatedInfact++;
            }

            if( TotalUpdatedInfact != TotalUpdated )
                throw ( new CswDniException( "Error adding column " + _CswTstCaseRsrcTableManipulations.TestColumnNameOne + ": updated " + TotalUpdated.ToString() + " rows but retrieved " + TotalUpdatedInfact.ToString() + " with that value" ) );

            _CswNbtSchemaModTrnsctn.rollbackTransaction();


            if( _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _CswTstCaseRsrcTableManipulations.TestTableName, _CswTstCaseRsrcTableManipulations.TestColumnNameOne ) )
                throw ( new CswDniException( "Added column " + _CswTstCaseRsrcTableManipulations.TestColumnNameOne + " was not rolled back from the database " ) );

            if( _CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _CswTstCaseRsrcTableManipulations.TestTableName, _CswTstCaseRsrcTableManipulations.TestColumnNameOne ) )
                throw ( new CswDniException( "Added column " + _CswTstCaseRsrcTableManipulations.TestColumnNameOne + " was not rolled back from the meta data " ) );



        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
