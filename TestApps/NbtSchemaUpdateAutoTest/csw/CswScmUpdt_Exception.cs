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
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswScmUpdt_Exception: CswDniException
    {

        public CswScmUpdt_Exception( string Message ): base( Message )
        {

        }//ctor

    }//CswSchemaUpdaterTestException

}//ChemSW.Nbt.Schema
