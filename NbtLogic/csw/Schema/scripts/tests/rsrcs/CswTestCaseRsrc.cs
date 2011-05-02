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
    public class CswTestCaseRsrc
    {

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        public CswTestCaseRsrc( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }//ctor

        public string makeTestCaseDescription( string TestCasePurpose, string SubPurpose )
        {
            return ( "Test Case: " + TestCasePurpose + "--" + SubPurpose );
        }

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
