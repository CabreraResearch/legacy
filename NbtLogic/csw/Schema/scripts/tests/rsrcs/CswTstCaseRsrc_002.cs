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
    public class CswTstCaseRsrc_002
    {

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswTestCaseRsrc _CswTestCaseRsrc = null;
        public CswTstCaseRsrc_002( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTestCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
        }//ctor


        public string Purpose = "Rolllback add columns";

        public string TestColumnNameOne { get { return ( _CswTestCaseRsrc.getTestColumnName( TestColumnNames.TestColumn01 ) ); } }
        public string TestColumnNameTwo { get { return ( _CswTestCaseRsrc.getTestColumnName( TestColumnNames.TestColumn02 ) ); } }
        public string TestTableName { get { return ( _CswTestCaseRsrc.getTestTableName( TestTableNames.TestTable02 ) ); } }

        public void testAddColumnValues( TestColumnNames TestColumnName ) { _CswTestCaseRsrc.testAddColumnValues( TestTableNames.TestTable02, TestColumnName ); }

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
