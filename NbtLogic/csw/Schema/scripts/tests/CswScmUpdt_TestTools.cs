using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Config;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
//using ChemSW.TblDn;
using ChemSW.Nbt.Schema;
//using ChemSW.Nbt.TableEvents;
using ChemSW.Nbt.TreeEvents;
using ChemSW.Log;
using ChemSW.DB;



namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswScmUpdt_TestTools
    {
        //private CswNbtResources _CswNbtResources;
        //private CswDbResources _CswDbResources;
        public CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn = null;
        public CswScmUpdt_TestTools()
        {
        }//ctor

        public void fillTableWithArbitraryData( string TableName, string ColumnName, string ValueStem, Int32 TotalRows )
        {
            Int32 ArbitraryValue = 0;
            CswTableUpdate CswTableUpdate = CswNbtSchemaModTrnsctn.makeCswTableUpdate( "fillTableWithArbitraryData_update", TableName );
            DataTable PkTableTable = CswTableUpdate.getTable();
            for( Int32 idx = 0; idx < TotalRows; idx++ )
            {
                DataRow NewRow = PkTableTable.NewRow();
                NewRow[ColumnName] = ValueStem + ":" + ( +ArbitraryValue ).ToString();
                PkTableTable.Rows.Add( NewRow );
            }
            CswTableUpdate.update( PkTableTable );

        }//fillTableWithArbitraryData()

        public void addArbitraryForeignKeyRecords( string PkTable, string FkTable, string ReferenceColumnName, string FkTableArbitraryValueColumnName, string FkTableValueStem )
        {

            CswTableSelect PkTableSelect = CswNbtSchemaModTrnsctn.makeCswTableSelect( "addArbitraryForeignKeyRecords_pktable_select", PkTable );
            DataTable PkTableTable = PkTableSelect.getTable();


            CswTableUpdate FkTableUpdate = CswNbtSchemaModTrnsctn.makeCswTableUpdate( "addArbitraryForeignKeyRecords_fktable_update", FkTable );
            DataTable FkTableTable = FkTableUpdate.getTable();
            Int32 ArbitraryValue = 0;
            foreach( DataRow CurrentRow in PkTableTable.Rows )
            {
                Int32 PkTablePk = CswConvert.ToInt32( CurrentRow[ReferenceColumnName] );

                DataRow NewFkTableRow = FkTableTable.NewRow();
                NewFkTableRow[ReferenceColumnName] = PkTablePk;
                NewFkTableRow[FkTableArbitraryValueColumnName] = FkTableValueStem + ": " + ( ++ArbitraryValue ).ToString();
                FkTableTable.Rows.Add( NewFkTableRow );
            }

            FkTableUpdate.update( FkTableTable );

        }//addArbitraryForeignKeyRecords()

        public bool isExceptionRecordDeletionConstraintViolation( Exception Exception )
        {
            //This is fine for oracle but will need to be pymorphed when we support sql server
            return (
                Exception.Message.Contains( "integrity constraint" ) &&
                 Exception.Message.Contains( "child record found" )
             );
        }//isRecordDeletionConstraintViolation()

        public bool isExceptionTableDropConstraintViolation( Exception Exception )
        {
            //This is fine for oracle but will need to be pymorphed when we support sql server
            return ( Exception.Message.Contains( "keys in table referenced by foreign keys" ) );
        }//isRecordDeletionConstraintViolation()

    }//CswSchemaUpdaterAutoTest

}//ChemSW.Nbt.Schema
