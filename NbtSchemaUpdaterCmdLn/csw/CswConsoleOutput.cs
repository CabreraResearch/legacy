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
        public bool ReportAccessIds = false;
        public StringCollection Messages = new StringCollection();

        public void write( string Message, bool ForceWrite = false, bool SuppressAccessId = false )
        {

            if( ( false == CollectStatusMessages ) || ( true == ForceWrite ) )
            {
                if( ReportAccessIds && ( false == SuppressAccessId ) )
                {
                    Message = _AccessId + ": " + Message;
                }

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
