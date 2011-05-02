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
    public class CswProdUpdtRsrc
    {

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        public CswProdUpdtRsrc( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }//ctor

        public string makeTestCaseDescription( CswSchemaVersion CswSchemaVersion )
        {
            return ( "Update to schema version " + CswSchemaVersion.ToString() );
        }

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
