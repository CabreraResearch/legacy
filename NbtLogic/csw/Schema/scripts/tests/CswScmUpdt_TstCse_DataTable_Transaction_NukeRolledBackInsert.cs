
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

namespace ChemSW.Nbt.Schema
{

    public class CswScmUpdt_TstCse_DataTable_Transaction_NukeRolledBackInsert : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_DataTable_Transaction_NukeRolledBackInsert( )
            : base( "Update same table after rollback of insert" )
        {
            _AppVenue = AppVenue.Generic;
        }//ctor

        //bz # 9018
        public override void runTest()
        {

            _CswNbtSchemaModTrnsctn.beginTransaction();

            CswTableUpdate CswUpdateDd = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "CswScmUpdt_TstCse_DataTable_Transaction_NukeRolledBackInsert_dd_update", "data_dictionary" );
            CswUpdateDd.StorageMode = StorageMode.Cached; // causes the rolback behavior we want
            DataTable DataTable = CswUpdateDd.getTable( " where lower(tablename)='data_dictionary'" );

            DataRow DataRow = DataTable.NewRow();
            DataRow[ "columnname" ] = "foo first";
            DataTable.Rows.Add( DataRow );

            CswUpdateDd.update( DataTable );
             
            _CswNbtSchemaModTrnsctn.rollbackTransaction();


            _CswNbtSchemaModTrnsctn.beginTransaction();


            DataRow[ "columnname" ] = "foo";

            CswUpdateDd.update( DataTable );

            _CswNbtSchemaModTrnsctn.commitTransaction();


        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
