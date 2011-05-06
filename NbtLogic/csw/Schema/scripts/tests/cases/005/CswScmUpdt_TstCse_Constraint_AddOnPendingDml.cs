using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.Schema
{


    public class CswScmUpdt_TstCse_Constraint_AddOnPendingDml : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_Constraint_AddOnPendingDml()
            : base( "Add constraint with pending DML transaction" )
        {
        }//ctor

        private string _PrimeKeyTableName = "pk_Table";
        private string _PrimeKeyColumnName = "pk_Table_col";
        private string _ArbitraryValCol = "arbitraryvalue";
        private string _ArbitraryValColEx = "arbitraryvalueex";

        private string _ForeignKeyTableName = "fk_Table";
        private string _ForeignKeyTableFkCol = "fk_table_col";

        public override void runTest()
        {
            _CswNbtSchemaModTrnsctn.beginTransaction();


            _CswNbtSchemaModTrnsctn.addTable( _PrimeKeyTableName, _PrimeKeyColumnName );
            _CswNbtSchemaModTrnsctn.addColumn( _ArbitraryValCol, DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, _PrimeKeyTableName, DataDictionaryUniqueType.None, false, string.Empty );

            _CswNbtSchemaModTrnsctn.addTable( _ForeignKeyTableName, _ForeignKeyTableFkCol );
            _CswNbtSchemaModTrnsctn.addColumn( _ArbitraryValCol, DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, _ForeignKeyTableName, DataDictionaryUniqueType.None, false, string.Empty );

            _CswNbtSchemaModTrnsctn.addForeignKeyColumn( _ForeignKeyTableName, _PrimeKeyColumnName, "test column", false, true, _PrimeKeyTableName, _PrimeKeyColumnName );


            CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "test script", _ForeignKeyTableName );
            DataTable DataTable = CswTableUpdate.getTable();
            DataRow DataRow = DataTable.NewRow();
            DataTable.Rows.Add( DataRow );
            DataRow[_ArbitraryValCol] = "foo";
            CswTableUpdate.update( DataTable );




            _CswNbtSchemaModTrnsctn.rollbackTransaction();


            /*
            _CswNbtSchemaModTrnsctn.addStringColumn( "nodetypes", "tablename", "Table in which nodes of this nodetype are kept", false, false, 40 ); //, Int32.MinValue, string.Empty );


            CswNbtMetaDataObjectClass GenericObjectClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass );

            CswNbtMetaDataNodeType PackageNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( GenericObjectClass.ObjectClassId, "Package", "CISPro" );


            _CswNbtSchemaModTrnsctn.addTable( "packages", "packageid" );
            _CswNbtSchemaModTrnsctn.addStringColumn( "packages", "nodename", "Name for this node row", true, false, 100 ); //, Int32.MinValue, string.Empty );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( "packages", "deleted", "logical delete flag", true, false );//, Int32.MinValue, string.Empty );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( "packages", "pendingupdate", "pending background update", true, false );//, Int32.MinValue, string.Empty );


            _CswNbtSchemaModTrnsctn.addForeignKeyColumn( "packages", "nodetypeid", "NodeTypeId for this node row", true, false, "nodetypes", "nodetypeid" ); //, Int32.MinValue, string.Empty );
            */



        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
