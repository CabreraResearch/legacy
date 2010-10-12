using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
//using ChemSW.TblDn;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.SchemaUpdaterAutoTest
{

    public class CswScmUpdt_TstCse_DataDictionary_ReadTableColIdAfterDdlOp : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_DataDictionary_ReadTableColIdAfterDdlOp( )
            : base( "Read data_dicitionary tablecolid after DDL" )
        {
        }//ctor


        public override void runTest()
        {

            _CswNbtSchemaModTrnsctn.beginTransaction();


            _CswNbtSchemaModTrnsctn.addTable( "packages_snot", "packageid" );
            //_CswNbtSchemaModTrnsctn.addForeignKeyColumn( "packages_snot", "nodetypeid", "NodeTypeId for this node row", true, false, "nodetypes", "nodetypeid", Int32.MinValue, string.Empty );
            _CswNbtSchemaModTrnsctn.addStringColumn( "packages_snot", "nodename", "Name for this node row", true, false, 100 ); //, Int32.MinValue, string.Empty );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( "packages_snot", "deleted", "logical delete flag", true, false );//, Int32.MinValue, string.Empty );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( "packages_snot", "pendingupdate", "pending background update", true, false );//, Int32.MinValue, string.Empty );
            _CswNbtSchemaModTrnsctn.addForeignKeyColumn( "packages_snot", "manufacturerid", "FK to vendors for manufacturer name", true, false, "vendors", "vendorid" );
            _CswNbtSchemaModTrnsctn.addStringColumn( "packages_snot", "manufacturer", "cached manufacturer name", true, false, 100 );
            _CswNbtSchemaModTrnsctn.addForeignKeyColumn( "packages_snot", "materialid", "FK to materials", true, false, "materials", "materialid" );
            _CswNbtSchemaModTrnsctn.addStringColumn( "packages_snot", "materialname", "cached material name", true, false, 100 );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( "packages_snot", "obsolete", "Obsolescense", true, false );
            _CswNbtSchemaModTrnsctn.addStringColumn( "packages_snot", "productdescription", "description of product", true, false, 254 );
            _CswNbtSchemaModTrnsctn.addStringColumn( "packages_snot", "productno", "product number", true, false, 50 );
            _CswNbtSchemaModTrnsctn.addForeignKeyColumn( "packages_snot", "supplierid", "FK to vendors for supplier name", true, false, "vendors", "vendorid" );
            _CswNbtSchemaModTrnsctn.addStringColumn( "packages_snot", "supplier", "cached supplier name", true, false, 100 );

            _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( "packages_snot", "manufacturerid" );
            Int32 TableColId = _CswNbtSchemaModTrnsctn.CswDataDictionary.TableColId;


            //clean up after ourselves
            _CswNbtSchemaModTrnsctn.rollbackTransaction();



        }//_testAddColumnValues

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
