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
    public class CswTstCaseRsrc_023
    {

		private CswTestCaseRsrc _CswTestCaseRsrc;
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
		public CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn
		{
			set
			{
				_CswNbtSchemaModTrnsctn = value;
				_CswTestCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			}
		}
		private CswAuditMetaData _CswAuditMetaData = new CswAuditMetaData();

        public static string Purpose = "Rollback of audit data";

        public string ArbitraryTableName_01 { get { return ( _CswTestCaseRsrc.getFakeTestTableName( TestTableNamesFake.TestTable01 ) ); } }

        public string ArbitraryColumnName_01 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ) ); } }
        public string ArbitraryColumnName_02 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn02 ) ); } }
        public string ArbitraryColumnName_03 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn03 ) ); } }

        public string ArbitraryColumnValue { get { return ( "snot" ); } }

        public Int32 TotalTestRows { get { return ( 10 ); } }



        public void makeArbitraryTable()
        {
            _CswNbtSchemaModTrnsctn.addTable( ArbitraryTableName_01, ( ArbitraryTableName_01 + "id" ).Replace( "_", "" ) );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName_01, ArbitraryColumnName_01, ArbitraryColumnName_01, false, false, 20 );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName_01, ArbitraryColumnName_02, ArbitraryColumnName_02, false, false, 20 );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName_01, ArbitraryColumnName_03, ArbitraryColumnName_03, false, false, 20 );

        }


        private Dictionary<string, List<string>> __ArbitraryTestValues;
        private Dictionary<string, List<string>> _ArbitraryTestValues
        {
            get
            {
                if( null == __ArbitraryTestValues )
                {
                    __ArbitraryTestValues = _CswTestCaseRsrc.makeArbitraryTestValues( 20, "_valsnot_" );
                }

                return ( __ArbitraryTestValues );

            }//get

        }//_ArbitraryTestValues



        public bool compareTargetAndAuditedData( ref string MisMatchMessage )
        {


            CswTableSelect CswTableSelectTargetTable = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "Compare table 1", ArbitraryTableName_01 );
            DataTable TargetTable = CswTableSelectTargetTable.getTable( " where 1=1", new Collection<OrderByClause> { new OrderByClause( CswTools.makePkColNameFromTableName( ArbitraryTableName_01 ), OrderByType.Ascending ) } );


            CswCommaDelimitedString CswCommaDelimitedString = new Core.CswCommaDelimitedString();
            foreach( DataColumn CurrentColumn in TargetTable.Columns )
            {
                CswCommaDelimitedString.Add( CurrentColumn.ColumnName );
            }

            CswTableSelect CswTableSelectAuditTable = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "Compare table 1", _CswAuditMetaData.makeAuditTableName( ArbitraryTableName_01 ) );
            DataTable AuditTable = CswTableSelectAuditTable.getTable( CswCommaDelimitedString, "where 1=1", new Collection<OrderByClause> { new OrderByClause( CswTools.makePkColNameFromTableName( _CswAuditMetaData.makeAuditTableName( ArbitraryTableName_01 ) ), OrderByType.Ascending ) } );


            return ( _CswTestCaseRsrc.doTableValuesMatch( TargetTable, AuditTable, CswCommaDelimitedString, ref MisMatchMessage ) );


        }//compareTargetAndAuditedData()

        public void makeArbitraryTableData()
        {
            _CswTestCaseRsrc.fillTableWithArbitraryData( ArbitraryTableName_01, _ArbitraryTestValues );
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
