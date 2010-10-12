
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
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

    public class CswScmUpdt_TstCse_DataTable_Transaction_NukeRolledBackUpdate : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_DataTable_Transaction_NukeRolledBackUpdate( )
            : base( "Reject rolled back column" )
        {
            _AppVenue = AppVenue.Generic;
        }//ctor

        //bz # 9018
        public override void runTest()
        {
            string ArbitraryTableName = "arbitrarytable";
            string ArbitraryTablePkColumn = "arbitrarytableid";
            string ArbitraryValueColumn = "arbitraryvaluecolumn";

            //Set up 
            //build an arbitrary table
            _CswNbtSchemaModTrnsctn.beginTransaction();
            _CswNbtSchemaModTrnsctn.addTable( ArbitraryTableName, ArbitraryTablePkColumn );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName, ArbitraryValueColumn, "arbitrary value column", true, false, 254 );
            _CswNbtSchemaModTrnsctn.commitTransaction();

            //add arbitrary values to arbitrary table
            _CswNbtSchemaModTrnsctn.beginTransaction();
            CswTableUpdate CswArbitraryTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "CswScmUpdt_TstCse_DataTable_Transaction_NukeRolledBackUpdate_update", ArbitraryTableName );
            CswArbitraryTableUpdate.StorageMode = StorageMode.Cached; // causes the rolback behavior we want

            DataTable DataTableArbitrary = CswArbitraryTableUpdate.getTable();
            DataTableArbitrary.Rows.Add( DataTableArbitrary.NewRow() );
            DataTableArbitrary.Rows.Add( DataTableArbitrary.NewRow() );
            DataTableArbitrary.Rows.Add( DataTableArbitrary.NewRow() );

            string EmptyValue = "snot";

            DataTableArbitrary.Rows[ 0 ][ ArbitraryValueColumn ] = EmptyValue;
            DataTableArbitrary.Rows[ 1 ][ ArbitraryValueColumn ] = EmptyValue;
            DataTableArbitrary.Rows[ 2 ][ ArbitraryValueColumn ] = EmptyValue;
            CswArbitraryTableUpdate.update( DataTableArbitrary );

            _CswNbtSchemaModTrnsctn.commitTransaction();

            //Now begin the test 'proper'
            string Val_Row_1 = "eenie";
            string Val_Row_2 = "meeny";
            string Val_Row_3 = "minie";
            DataTableArbitrary = CswArbitraryTableUpdate.getTable();

            _CswNbtSchemaModTrnsctn.beginTransaction();
            DataTableArbitrary.Rows[ 0 ][ ArbitraryValueColumn ] = Val_Row_1;
            CswArbitraryTableUpdate.update( DataTableArbitrary );
            _CswNbtSchemaModTrnsctn.commitTransaction();

            _CswNbtSchemaModTrnsctn.beginTransaction();
            DataTableArbitrary.Rows[ 1 ][ ArbitraryValueColumn ] = Val_Row_2;
            _CswNbtSchemaModTrnsctn.rollbackTransaction();

            _CswNbtSchemaModTrnsctn.beginTransaction();
            DataTableArbitrary.Rows[ 2 ][ ArbitraryValueColumn ] = Val_Row_3;
            CswArbitraryTableUpdate.update( DataTableArbitrary );
            _CswNbtSchemaModTrnsctn.commitTransaction();

            
            Collection<OrderByClause> OrderByClauses = new Collection<OrderByClause>();
            OrderByClauses.Add( new OrderByClause( ArbitraryTablePkColumn, OrderByType.Ascending ) );
            DataTableArbitrary = CswArbitraryTableUpdate.getTable( "where " + ArbitraryTablePkColumn + "> 0", OrderByClauses );
            if ( DataTableArbitrary.Rows[ 0 ][ ArbitraryValueColumn ].ToString() != Val_Row_1 )
                throw ( new CswScmUpdt_Exception( "Row one does not have value " + Val_Row_1 ) );

            if ( DataTableArbitrary.Rows[ 1 ][ ArbitraryValueColumn ].ToString() == Val_Row_2 )
                throw ( new CswScmUpdt_Exception( "Row two has rolled-back value " + Val_Row_2 + "; it's value should be: " + EmptyValue ) );

            if ( DataTableArbitrary.Rows[ 2 ][ ArbitraryValueColumn ].ToString() != Val_Row_3 )
                throw ( new CswScmUpdt_Exception( "Row three does not have value " + Val_Row_3 ) );


            //tear down of arbitrary table
            _CswNbtSchemaModTrnsctn.beginTransaction();

            _CswNbtSchemaModTrnsctn.dropTable( ArbitraryTableName );

            _CswNbtSchemaModTrnsctn.commitTransaction();


        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
