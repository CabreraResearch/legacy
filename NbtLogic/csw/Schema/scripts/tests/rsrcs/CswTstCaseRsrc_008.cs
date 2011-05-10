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
    public class CswTstCaseRsrc_008
    {

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswTestCaseRsrc _CswTestCaseRsrc = null;
        public CswTstCaseRsrc_008( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTestCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
        }//ctor


        public string Purpose = "Read data_dicitionary tablecolid";

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
