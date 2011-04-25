using System;
using System.Threading;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ChemSW.Exceptions;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Log;
using ChemSW.Nbt.Config;
using ChemSW.Nbt.Schema;
using ChemSW.Nbt;


namespace ChemSW.Nbt.Schema.CmdLn
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswConsoleOutput
    {

        ICswLogger _CswLogger = null;
        public CswConsoleOutput( ICswLogger CswLogger )
        {
            _CswLogger = CswLogger;
        }//ctor 

        public void write( string Message )
        {
            Console.Write( Message );
            _CswLogger.reportAppState( Message ); 
        }//write

    }//CswConsoleOutput

}//ChemSW.Nbt.Schema.CmdLn
