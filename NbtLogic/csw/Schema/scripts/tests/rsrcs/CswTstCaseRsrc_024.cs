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
    public class CswTstCaseRsrc_024
    {

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswTestCaseRsrc _CswTestCaseRsrc = null;
        private CswAuditMetaData _CswAuditMetaData = new CswAuditMetaData();
        public CswTstCaseRsrc_024( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {

            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTestCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
        }//ctor


        private Int32 _TheNumberOfRowsToAffect = 20;

        public string Purpose = "Basic audit mechanism for update";

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



        public bool compareUpdateAuditData( ref string MisMatchMessage )
        {

            return ( _CswTestCaseRsrc.doTableValuesMatchTestValues( TestRecordsFromUpdate, _ArbitraryUpdateTestValues, ref MisMatchMessage ) );

        }//compareTargetAndAuditedData()


        public bool compareInsertAuditData( ref string MisMatchMessage )
        {

            return ( _CswTestCaseRsrc.doTableValuesMatchTestValues( TestRecordsFromInsert, _ArbitraryInsertTestValues, ref MisMatchMessage ) );

        }//compareTargetAndAuditedData()


        public DataTable TestRecordsFromUpdate
        {
            get
            {

                CswCommaDelimitedString CswCommaDelimitedString = new Core.CswCommaDelimitedString();
                string AuditTablename = _CswAuditMetaData.makeAuditTableName( ArbitraryTableName_01 );
                string ArbitraryTablePkCol = CswTools.makePkColNameFromTableName( AuditTablename );
                Collection<OrderByClause> OrderByClauses = new Collection<OrderByClause>();
                OrderByClauses.Add( new OrderByClause( ArbitraryTablePkCol, OrderByType.Ascending ) );

                CswArbitrarySelect CswArbitrarySelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( Purpose, "select * from " + AuditTablename + " order by " + ArbitraryTablePkCol + " asc" );

                DataTable TargetTable = CswArbitrarySelect.getTable( _TheNumberOfRowsToAffect + 1, _TheNumberOfRowsToAffect + _TheNumberOfRowsToAffect + 1, false, false );

                return ( TargetTable );

            }
        }//UpdateAuditTestRecords

        public DataTable TestRecordsFromInsert
        {
            get
            {

                CswCommaDelimitedString CswCommaDelimitedString = new Core.CswCommaDelimitedString();
                string AuditTablename = _CswAuditMetaData.makeAuditTableName( ArbitraryTableName_01 );
                string ArbitraryTablePkCol = CswTools.makePkColNameFromTableName( AuditTablename );
                Collection<OrderByClause> OrderByClauses = new Collection<OrderByClause>();
                OrderByClauses.Add( new OrderByClause( ArbitraryTablePkCol, OrderByType.Ascending ) );

                CswArbitrarySelect CswArbitrarySelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( Purpose, "select * from " + AuditTablename + " order by " + ArbitraryTablePkCol + " asc" );

                DataTable TargetTable = CswArbitrarySelect.getTable( 1, _TheNumberOfRowsToAffect + 1, false, false );

                return ( TargetTable );

            }
        }//UpdateAuditTestRecords

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
            _CswTestCaseRsrc.fillTableWithArbitraryData( ArbitraryTableName_01, _ArbitraryInsertTestValues );
        }


        public void updateArbitraryTableData()
        {
            _CswTestCaseRsrc.fillTableWithArbitraryData( ArbitraryTableName_01, _ArbitraryUpdateTestValues );
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
