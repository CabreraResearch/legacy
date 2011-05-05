
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

namespace ChemSW.Nbt.Schema
{

    public class CswScmUpdt_TstCse_DataTable_Transaction_NukeStaleCoumnlValue : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_DataTable_Transaction_NukeStaleCoumnlValue( )
            : base( "Reject stale column value" )
        {
            _AppVenue = AppVenue.Generic;
        }//ctor

        //bz # 9018
        public override void runTest()
        {
            string ArbitraryTableName = "foo_arbitrarytable";
            string ArbitraryTablePkColumn = "foo_arbitrarytableid";
            string ArbitraryValueColumn = "foo_arbitraryvaluecolumn";
            string SecondArbitraryValueColumn = "foo_secondarbitraryvaluecolumn";
            string ArbitraryValueColumnValue = "A";
            string ArbitraryValueColumnValueDelta = "B";
            string SecondArbitraryValueColumnValue = "X";
            string SecondArbitraryValueColumnValueDelta = "Y";

            //*******Begin Set up *****************
            //build an arbitrary table
            _CswNbtSchemaModTrnsctn.beginTransaction();
            _CswNbtSchemaModTrnsctn.addTable( ArbitraryTableName, ArbitraryTablePkColumn );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName, ArbitraryValueColumn, "arbitrary value column", true, false, 254 );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName, SecondArbitraryValueColumn, "second arbitrary value column", true, false, 254 );
            _CswNbtSchemaModTrnsctn.commitTransaction();

            //add arbitrary values to arbitrary table
            _CswNbtSchemaModTrnsctn.beginTransaction();
            CswTableUpdate CswArbitraryTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "CswScmUpdt_TstCse_DataTable_Transaction_NukeStaleCoumnlValue_ArbitraryTableUpdate", ArbitraryTableName );
            CswArbitraryTableUpdate.StorageMode = StorageMode.Cached; // causes the rolback behavior we want


            DataTable DataTableArbitrary = CswArbitraryTableUpdate.getTable();
            DataTableArbitrary.Rows.Add( DataTableArbitrary.NewRow() );

            DataTableArbitrary.Rows[ 0 ][ ArbitraryValueColumn ] = ArbitraryValueColumnValue;
            DataTableArbitrary.Rows[ 0 ][ SecondArbitraryValueColumn ] = SecondArbitraryValueColumnValue;
            CswArbitraryTableUpdate.update( DataTableArbitrary );

            _CswNbtSchemaModTrnsctn.commitTransaction();
            //*******End Set up *****************

            //*******Begin Test Proper Phase 1: Post when modifying with new value*****************
            DataTableArbitrary = CswArbitraryTableUpdate.getTable();

            _CswNbtSchemaModTrnsctn.beginTransaction();
            DataTableArbitrary.Rows[ 0 ][ ArbitraryValueColumn ] = ArbitraryValueColumnValueDelta;
            CswArbitraryTableUpdate.update( DataTableArbitrary );
            _CswNbtSchemaModTrnsctn.rollbackTransaction();

            _CswNbtSchemaModTrnsctn.beginTransaction();
            DataTableArbitrary.Rows[ 0 ][ SecondArbitraryValueColumn ] = SecondArbitraryValueColumnValueDelta;
            CswArbitraryTableUpdate.update( DataTableArbitrary );
            _CswNbtSchemaModTrnsctn.commitTransaction();

            DataTableArbitrary = CswArbitraryTableUpdate.getTable();

            if ( DataTableArbitrary.Rows[ 0 ][ SecondArbitraryValueColumn ].ToString() != SecondArbitraryValueColumnValueDelta )
                throw ( new CswScmUpdt_Exception( "Column  " + SecondArbitraryValueColumn + " does not have the committed value " + SecondArbitraryValueColumnValueDelta ) );

            if ( DataTableArbitrary.Rows[ 0 ][ ArbitraryValueColumn ].ToString() == ArbitraryValueColumnValueDelta )
                throw ( new CswScmUpdt_Exception( "Column  " + ArbitraryValueColumn + " has the rolled back value (with another value modication)" + ArbitraryValueColumnValueDelta ) );

            //*******Begin Test Proper Phase 1: Post when leaving the table alone*****************
            DataTableArbitrary = CswArbitraryTableUpdate.getTable();

            _CswNbtSchemaModTrnsctn.beginTransaction();
            DataTableArbitrary.Rows[ 0 ][ ArbitraryValueColumn ] = ArbitraryValueColumnValueDelta;
            CswArbitraryTableUpdate.update( DataTableArbitrary );
            _CswNbtSchemaModTrnsctn.rollbackTransaction();

            _CswNbtSchemaModTrnsctn.beginTransaction();
            CswArbitraryTableUpdate.update( DataTableArbitrary );
            _CswNbtSchemaModTrnsctn.commitTransaction();

            if ( DataTableArbitrary.Rows[ 0 ][ ArbitraryValueColumn ].ToString() == ArbitraryValueColumnValueDelta )
                throw ( new CswScmUpdt_Exception( "Column  " + ArbitraryValueColumn + " has the rolled back value (with no other value modification)" + ArbitraryValueColumnValueDelta ) );


            //tear down of arbitrary table
            _CswNbtSchemaModTrnsctn.beginTransaction();

            _CswNbtSchemaModTrnsctn.dropTable( ArbitraryTableName );

            _CswNbtSchemaModTrnsctn.commitTransaction();


        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
