using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Test Case: 001, part 01
    /// </summary>
    public class CswTstCaseRsrc_001
    {

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswTestCaseRsrc _CswTestCaseRsrc = null;
        public CswTstCaseRsrc_001( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTestCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
        }//ctor


        public string Purpose = "Add Columns";

        public string TestColumnNameOne { get { return ( _CswTestCaseRsrc.getTestColumnName( TestColumnNames.TestColumn01 ) ); } }
        public string TestColumnNameTwo { get { return ( _CswTestCaseRsrc.getTestColumnName( TestColumnNames.TestColumn02 ) ); } }
        public string TestTableName { get { return ( _CswTestCaseRsrc.getTestTableName( TestTableNames.TestTable01 ) ); } }

        public void testAddColumnValues( TestColumnNames TestColumnName ) { _CswTestCaseRsrc.testAddColumnValues( TestTableNames.TestTable01, TestColumnName ); }

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
