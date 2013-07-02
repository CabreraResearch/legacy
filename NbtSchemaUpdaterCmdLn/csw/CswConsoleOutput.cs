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
        private string _AccessId = string.Empty;
        public CswConsoleOutput( ICswLogger CswLogger, string AccessId )
        {
            _AccessId = AccessId;
            _CswLogger = CswLogger;
        }//ctor 

        public void write( string Message, bool WriteAccessId = true )
        {
            //if( true == WriteAccessId )
            //{
            //    Console.Write( _AccessId + ": " + Message );
            //}
            //else
            //{
            Console.Write( Message );
            //}

            _CswLogger.reportAppState( Message );

        }//write

    }//CswConsoleOutput

}//ChemSW.Nbt.Schema.CmdLn
