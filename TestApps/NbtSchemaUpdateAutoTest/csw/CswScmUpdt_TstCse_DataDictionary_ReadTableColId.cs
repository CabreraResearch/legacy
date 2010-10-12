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

    public class CswScmUpdt_TstCse_DataDictionary_ReadTableColId : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_DataDictionary_ReadTableColId( )
            : base( "Read data_dicitionary tablecolid" )
        {
        }//ctor


        public override void runTest()
        {

            _CswNbtSchemaModTrnsctn.beginTransaction();


            _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( "nodetype_props", "nodetypepropid" );

            Int32 TableColId = _CswNbtSchemaModTrnsctn.CswDataDictionary.TableColId;

            _CswNbtSchemaModTrnsctn.commitTransaction();



        }//_testAddColumnValues

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
