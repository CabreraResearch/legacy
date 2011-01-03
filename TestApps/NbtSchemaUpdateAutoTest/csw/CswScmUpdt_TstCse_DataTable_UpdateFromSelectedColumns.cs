using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.DB;
using ChemSW.Nbt.Schema;
using ChemSW.Core;

namespace ChemSW.Nbt.SchemaUpdaterAutoTest
{
    //9102
    //when you retrieve the data table using a list of select columns, 
    //the update performed with that Table results in the deletion of the record.
    public class CswScmUpdt_TstCse_DataTable_UpdateFromSelectedColumns : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_DataTable_UpdateFromSelectedColumns()
            : base( "Update From Selected Columns" )
        {
            _AppVenue = AppVenue.Generic;
        }//ctor

        //bz # 9018
        public override void runTest()
        {

            string ArbitraryTableName = "arbitrarytable";
            Int32 MaterialsPk = Int32.MinValue;
            try
            {
                //Setup table to test with
                _CswNbtSchemaModTrnsctn.beginTransaction();

                string ArbitraryTablePkColumn = "arbitrarytableid";
                string ArbitraryValueColumn = "arbitraryvaluecolumn";

                Int32 PkOfInsertedRow = Int32.MinValue;

                //*********** BEGIN SET UP
                //build an arbitrary table with some arbitrary rows with arbitrary column values (arbitrarily)

                /*
                _CswNbtSchemaModTrnsctn.addTable( ArbitraryTableName, ArbitraryTablePkColumn );
                _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName, ArbitraryValueColumn, "arbitrary value column", true, false, 254 );
                CswTableUpdate CswArbitraryTableUpdateForSetup = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "CswScmUpdt_TstCse_DataTable_UpdateFromSelectedColumns_TestSetup", ArbitraryTableName );
                DataTable DataTableForSeup = CswArbitraryTableUpdateForSetup.getEmptyTable();
                DataRow NewRow_1 = DataTableForSeup.NewRow();
                DataTableForSeup.Rows.Add( NewRow_1 );
                DataRow NewRow_2 = DataTableForSeup.NewRow();
                DataTableForSeup.Rows.Add( NewRow_2 );
                DataRow NewRow_3 = DataTableForSeup.NewRow();
                DataTableForSeup.Rows.Add( NewRow_3 );

                DataTableForSeup.Rows[ 0 ][ ArbitraryValueColumn ] = "valone";
                DataTableForSeup.Rows[ 1 ][ ArbitraryValueColumn ] = "valtwo";
                DataTableForSeup.Rows[ 2 ][ ArbitraryValueColumn ] = "valthree";

                PkOfInsertedRow = Convert.ToInt32( DataTableForSeup.Rows[ 0 ][ ArbitraryTablePkColumn ] );

                CswArbitraryTableUpdateForSetup.update( DataTableForSeup );

                _CswNbtSchemaModTrnsctn.commitTransaction();
                //*********** END SET UP

                //add arbitrary values to arbitrary table
                _CswNbtSchemaModTrnsctn.beginTransaction();
                CswTableUpdate CswArbitraryTableUpdateForTest = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "CswScmUpdt_TstCse_DataTable_UpdateFromSelectedColumns_TestItself", ArbitraryTableName );
                StringCollection SelectColumns = new StringCollection();
                SelectColumns.Add( ArbitraryTablePkColumn );
                SelectColumns.Add( ArbitraryValueColumn );
                DataTable DataTableForTest = CswArbitraryTableUpdateForTest.getTable( SelectColumns, ArbitraryTablePkColumn, PkOfInsertedRow, string.Empty, false );
                DataTableForTest.Rows[ 0 ][ ArbitraryValueColumn ] = "valnu";
                _CswNbtSchemaModTrnsctn.commitTransaction();

                CswTableSelect CswTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "CswScmUpdt_TstCse_DataTable_UpdateFromSelectedColumns_ProbeSelect", ArbitraryTableName );
                DataTable DataTableProbe = CswTableSelect.getTable( SelectColumns, ArbitraryTablePkColumn, PkOfInsertedRow, string.Empty, false );

                if ( DataTableProbe.Rows.Count != 1 )
                {
                    throw ( new CswScmUpdt_Exception( "Update of a record from a CswTableUpdate with specified columns resulted in record deletion " ) );
                }
                */

                //*********** BEGIN SET UP
                _CswNbtSchemaModTrnsctn.beginTransaction();

                CswTableUpdate CswTableUpdateSetup = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Name + "_setupdata", "materials" );
                DataTable MaterialsTable = CswTableUpdateSetup.getEmptyTable();
                DataRow NewMaterialsRow = MaterialsTable.NewRow();
                MaterialsTable.Rows.Add( NewMaterialsRow );
                MaterialsPk = Convert.ToInt32( MaterialsTable.Rows[ 0 ][ "materialid" ] );
                MaterialsTable.Rows[ 0 ][ "materialname" ] = Name + "--test row";
                CswTableUpdateSetup.update( MaterialsTable );

                _CswNbtSchemaModTrnsctn.commitTransaction();




                //*********** END SET UP


                _CswNbtSchemaModTrnsctn.beginTransaction();

                CswTableSelect CswTableSelectNodeTypes = _CswNbtSchemaModTrnsctn.makeCswTableSelect( Name + "_nodetypesprobe", "nodetypes" );
                DataTable NodetypesTable = CswTableSelectNodeTypes.getTable( " where lower(tablename)='materials'" );
                Int32 NodeTypeId = Convert.ToInt32( NodetypesTable.Rows[ 0 ][ "nodetypeid" ] );

                CswNbtMetaDataNodeType NodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( NodeTypeId );
                CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "CswNbtNodePropCollDataRelational_update", NodeType.TableName );
                string PkColumnName = _CswNbtSchemaModTrnsctn.getPrimeKeyColName( NodeType.TableName );

                //bz # 9102: This is the way of getting the record that causes the updated record disappear

                CswCommaDelimitedString SelectColumns = new CswCommaDelimitedString();
                foreach ( CswNbtMetaDataNodeTypeProp CurrentNodeTypeProp in NodeType.NodeTypeProps )
                {
                    foreach ( CswNbtSubField CurrentSubField in CurrentNodeTypeProp.FieldTypeRule.SubFields )
                    {
                        if ( CurrentSubField.RelationalColumn != string.Empty )
                            SelectColumns.Add( CurrentSubField.RelationalColumn );
                    }
                }//iterate node type props to set up select columns

//                SelectColumns.Add( "deleted" );

                DataTable DataTable = CswTableUpdate.getTable( SelectColumns, PkColumnName, MaterialsPk, string.Empty, false );

                //                DataTable DataTable = CswTableUpdate.getTable( " where materialid=" + MaterialsPk.ToString() );



                DataTable.Rows[ 0 ][ "materialname" ] = "nu name";

                CswTableUpdate.update( DataTable );

                _CswNbtSchemaModTrnsctn.commitTransaction();

                CswTableSelect CswTableSelectMaterials = _CswNbtSchemaModTrnsctn.makeCswTableSelect( Name + "_selectmaterials", "materials" );
                DataTable DataTableMaterials = CswTableSelectMaterials.getTable( " where materialid=" + MaterialsPk.ToString() );
                if ( DataTableMaterials.Rows.Count != 1 )
                    throw ( new CswScmUpdt_Exception( "Update of a record from a CswTableUpdate with specified columns resulted in deletion of materials record " + MaterialsPk.ToString() ) );

            }

            finally
            {
                //Cleanup test case
                //_CswNbtSchemaModTrnsctn.beginTransaction();
                //CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "CswNbtNodePropCollDataRelational_cleanup", "materials");
                //DataTable MaterialsTable = CswTableUpdate.getTable( " where materialid=" + MaterialsPk.ToString() );
                //if ( 1 == MaterialsTable.Rows.Count )
                //{
                //    MaterialsTable.Rows[ 0 ].Delete();
                //    CswTableUpdate.update( MaterialsTable );
                //    _CswNbtSchemaModTrnsctn.commitTransaction();
                //}
                //else
                //{
                //    _CswNbtSchemaModTrnsctn.rollbackTransaction();
                //}

            }

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
