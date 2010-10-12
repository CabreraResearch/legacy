using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.DB;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.SchemaUpdaterAutoTest
{

    public class CswScmUpdt_TstCse_Table_RollbackDrop : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_Table_RollbackDrop( )
            : base( "Drop table rollback" )
        {
        }//ctor

        private string _TestTableName = "test_table";
        private Int32 _TotalTestRows = 100;
        private Int32 _getTblRowCount()
        {
            CswTableSelect TableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "getTblRowCount_select", _TestTableName );
            //DataTable DataTable = TableSelect.getTable();
            //return ( DataTable.Rows.Count );
            return TableSelect.getRecordCount();
        }//_getTblRowCount()

        public override void runTest()
        {

            _CswNbtSchemaModTrnsctn.beginTransaction();


            _CswNbtSchemaModTrnsctn.addTable( _TestTableName, _TestTableName  + "id" );

            CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "CswScmUpdt_TstCse_Table_RollbackDrop_runtest_update", _TestTableName );
            DataTable DataTable = TableUpdate.getTable();
            Int32 TotalRows = 1;
            while( TotalRows  <= _TotalTestRows )
            {
                DataRow NewRow = DataTable.NewRow();
                DataTable.Rows.Add( NewRow );
                TotalRows++;
            }
            TableUpdate.update( DataTable );

            _CswNbtSchemaModTrnsctn.commitTransaction(); ;

            Int32 PriorDDRowCount = _getTblRowCount();
            if ( PriorDDRowCount != _TotalTestRows )
                throw ( new CswScmUpdt_Exception( "Tablerows of test table retrieved did not match: added " + _TotalTestRows.ToString() + "; got back " + PriorDDRowCount ) );
            
            _CswNbtSchemaModTrnsctn.beginTransaction();

            _CswNbtSchemaModTrnsctn.dropTable( _TestTableName );

            _CswNbtSchemaModTrnsctn.rollbackTransaction();


            if( ! _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( _TestTableName ) )
                throw( new CswScmUpdt_Exception( "Rollback of drop table on " + _TestTableName + " failed"  )) ;


            Int32 AfterTblRowCount = _getTblRowCount();
            if ( AfterTblRowCount < PriorDDRowCount )
                throw( new CswScmUpdt_Exception( "Roll back of drop table on " + _TestTableName + " failed to restore all rows"  ) );

            //clean up after ourselves
            _CswNbtSchemaModTrnsctn.beginTransaction();

            _CswNbtSchemaModTrnsctn.dropTable( _TestTableName );

            _CswNbtSchemaModTrnsctn.commitTransaction();

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropTableRollback

}//ChemSW.Nbt.Schema
