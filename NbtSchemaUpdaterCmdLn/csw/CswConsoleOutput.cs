using System;
using System.Collections.Specialized;
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

        public bool CollectStatusMessages = false;
        public StringCollection Messages = new StringCollection();

        public void write( string Message, bool ForceWrite = false )
        {

            if( ( false == CollectStatusMessages ) || ( true == ForceWrite ) )
            {
                Console.Write( Message );
            }
            else
            {
                Messages.Add( Message );
            }

            _CswLogger.reportAppState( Message );

        }//write

    }//CswConsoleOutput

}//ChemSW.Nbt.Schema.CmdLn
