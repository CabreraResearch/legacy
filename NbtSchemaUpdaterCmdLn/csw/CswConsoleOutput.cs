using System;
using ChemSW.Log;


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
