using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.Schema;
using ChemSW.Audit;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Test Case: 001, part 01
    /// </summary>
    public class CswTstCaseRsrc_025
    {

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswTestCaseRsrc _CswTestCaseRsrc = null;
        private CswAuditMetaData _CswAuditMetaData = new CswAuditMetaData();
        public CswTstCaseRsrc_025( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {

            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTestCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
        }//ctor


        private Int32 _TheNumberOfRowsToAffect = 20;

        public string Purpose = "Basic audit mechanism for delete";

        public string ArbitraryTableName_01 { get { return ( _CswTestCaseRsrc.getFakeTestTableName( TestTableNamesFake.TestTable01 ) ); } }

        public string ArbitraryColumnName_01 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ) ); } }
        public string ArbitraryColumnName_02 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn02 ) ); } }
        public string ArbitraryColumnName_03 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn03 ) ); } }

        public string ArbitraryColumnValue { get { return ( "snot" ); } }

        public Int32 TotalTestRows { get { return ( 10 ); } }



        public void makeArbitraryTable()
        {
            _CswNbtSchemaModTrnsctn.addTable( ArbitraryTableName_01, ( ArbitraryTableName_01 + "id" ).Replace( "_", "" ) );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName_01, ArbitraryColumnName_01, ArbitraryColumnName_01, false, false, 60 );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName_01, ArbitraryColumnName_02, ArbitraryColumnName_02, false, false, 60 );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName_01, ArbitraryColumnName_03, ArbitraryColumnName_03, false, false, 60 );

        }


        public List<Int32> PksOfAuditedRecords = new List<int>(); 


        public bool compareInsertAuditData( ref string MisMatchMessage )
        {

            return ( _CswTestCaseRsrc.doTableValuesMatchTestValues( AuditRecordsFromInsert, _ArbitraryInsertTestValues, ref MisMatchMessage ) );

        }//compareTargetAndAuditedData()


        public bool compareUpdateAuditData( ref string MisMatchMessage )
        {

            return ( _CswTestCaseRsrc.doTableValuesMatchTestValues( AuditRecordsFromUpdate, _ArbitraryUpdateTestValues, ref MisMatchMessage ) );

        }//compareTargetAndAuditedData()


        public bool compareDeleteAuditData( ref string MisMatchMessage )
        {
            //we expect the delete audit records to have the last values that were applied to the table -- it records the state of th e
            //record when it was deleted
            return ( _CswTestCaseRsrc.doTableValuesMatchTestValues( AuditRecordsFromDelete, _ArbitraryUpdateTestValues, ref MisMatchMessage ) );

        }//compareTargetAndAuditedData()


        public DataTable AuditRecordsFromInsert
        {
            get
            {

                CswCommaDelimitedString CswCommaDelimitedString = new Core.CswCommaDelimitedString();
                string AuditTablename = _CswAuditMetaData.makeAuditTableName( ArbitraryTableName_01 );
                string ArbitraryTablePkCol = CswTools.makePkColNameFromTableName( AuditTablename );
                Collection<OrderByClause> OrderByClauses = new Collection<OrderByClause>();
                OrderByClauses.Add( new OrderByClause( ArbitraryTablePkCol, OrderByType.Ascending ) );

                CswArbitrarySelect CswArbitrarySelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( Purpose, "select * from " + AuditTablename + " order by " + ArbitraryTablePkCol + " asc" );

                DataTable TargetTable = CswArbitrarySelect.getTable( 0, _TheNumberOfRowsToAffect, false, false );

                return ( TargetTable );

            }
        }//TestRecordsFromInsert


        public DataTable AuditRecordsFromUpdate
        {
            get
            {

                CswCommaDelimitedString CswCommaDelimitedString = new Core.CswCommaDelimitedString();
                string AuditTablename = _CswAuditMetaData.makeAuditTableName( ArbitraryTableName_01 );
                string ArbitraryTablePkCol = CswTools.makePkColNameFromTableName( AuditTablename );
                Collection<OrderByClause> OrderByClauses = new Collection<OrderByClause>();
                OrderByClauses.Add( new OrderByClause( ArbitraryTablePkCol, OrderByType.Ascending ) );

                CswArbitrarySelect CswArbitrarySelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( Purpose, "select * from " + AuditTablename + " order by " + ArbitraryTablePkCol + " asc" );

                DataTable TargetTable = CswArbitrarySelect.getTable( _TheNumberOfRowsToAffect, _TheNumberOfRowsToAffect + _TheNumberOfRowsToAffect, false, false );

                return ( TargetTable );

            }
        }//TestRecordsFromUpdate


        public DataTable AuditRecordsFromDelete
        {
            get
            {

                CswCommaDelimitedString CswCommaDelimitedString = new Core.CswCommaDelimitedString();
                string AuditTablename = _CswAuditMetaData.makeAuditTableName( ArbitraryTableName_01 );
                string ArbitraryTablePkCol = CswTools.makePkColNameFromTableName( AuditTablename );
                Collection<OrderByClause> OrderByClauses = new Collection<OrderByClause>();
                OrderByClauses.Add( new OrderByClause( ArbitraryTablePkCol, OrderByType.Ascending ) );

                CswArbitrarySelect CswArbitrarySelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( Purpose, "select * from " + AuditTablename + " order by " + ArbitraryTablePkCol + " asc" );

                DataTable TargetTable = CswArbitrarySelect.getTable( ( _TheNumberOfRowsToAffect * 2 ), ( _TheNumberOfRowsToAffect * 3 ) + 1, false, false );

                return ( TargetTable );

            }
        }//TestRecordsFromDelete



        private Dictionary<string, List<string>> __ArbitraryInsertTestValues;
        private Dictionary<string, List<string>> _ArbitraryInsertTestValues
        {
            get
            {
                if( null == __ArbitraryInsertTestValues )
                {
                    __ArbitraryInsertTestValues = _CswTestCaseRsrc.makeArbitraryTestValues( _TheNumberOfRowsToAffect, "_valsnot_" );
                }

                return ( __ArbitraryInsertTestValues );

            }//get

        }//_ArbitraryInsertTestValues


        private Dictionary<string, List<string>> __ArbitraryUpdateTestValues;
        private Dictionary<string, List<string>> _ArbitraryUpdateTestValues
        {
            get
            {
                if( null == __ArbitraryUpdateTestValues )
                {
                    __ArbitraryUpdateTestValues = _CswTestCaseRsrc.makeArbitraryTestValues( _TheNumberOfRowsToAffect, "_valsbar_" );
                }

                return ( __ArbitraryUpdateTestValues );

            }//get

        }//_ArbitraryUpdateTestValues



        public void makeArbitraryTableData()
        {
            _CswTestCaseRsrc.fillTableWithArbitraryData( ArbitraryTableName_01, _ArbitraryInsertTestValues, PksOfAuditedRecords );
        }


        public void updateArbitraryTableData()
        {
            _CswTestCaseRsrc.fillTableWithArbitraryData( ArbitraryTableName_01, _ArbitraryUpdateTestValues );
        }

        public void deleteArbitraryTableData()
        {
            _CswTestCaseRsrc.deleteArbitraryTableData( ArbitraryTableName_01 );
        }

        public void dropArbitraryTables()
        {
            _CswNbtSchemaModTrnsctn.dropTable( ArbitraryTableName_01 );
        }

        public void verifyTablesDropped()
        {
            _CswTestCaseRsrc.assertTableIsAbsent( ArbitraryTableName_01 );
        }


        private string _OriginalAuditSetting_Audit = string.Empty;
        public void setAuditingOn()
        {
            _OriginalAuditSetting_Audit = _CswNbtSchemaModTrnsctn.getConfigVariableValue( _CswAuditMetaData.AuditConfgVarName );

            if( "1" != _OriginalAuditSetting_Audit )
            {
                _CswNbtSchemaModTrnsctn.setConfigVariableValue( _CswAuditMetaData.AuditConfgVarName, "1" );
            }

        }//setAuditingOn()

        public void restoreAuditSetting()
        {
            if( _CswNbtSchemaModTrnsctn.getConfigVariableValue( _CswAuditMetaData.AuditConfgVarName ) != _OriginalAuditSetting_Audit )
            {
                _CswNbtSchemaModTrnsctn.setConfigVariableValue( _CswAuditMetaData.AuditConfgVarName, _OriginalAuditSetting_Audit );
            }
        }//setAuditingOn()

        public void assertAuditSettingIsRestored()
        {
            string CurrentAuditSetting = _CswNbtSchemaModTrnsctn.getConfigVariableValue( _CswAuditMetaData.AuditConfgVarName );

            if( CurrentAuditSetting != _OriginalAuditSetting_Audit )
            {
                throw ( new CswDniException( "Current audit configuration setting (" + CurrentAuditSetting + ") does not match the original setting (" + _OriginalAuditSetting_Audit + ")" ) );
            }

        }//assertAuditSettingIsRestored()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
