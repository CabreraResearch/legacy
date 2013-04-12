using System;
using System.Threading;


namespace ChemSW.Nbt.Schema.CmdLn
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaUpdateThread
    {
        private CswSchemaUpdater _CswSchemaUpdater = null;
        public CswSchemaUpdateThread( CswSchemaUpdater CswSchemaUpdater )
        {
            _CswSchemaUpdater = CswSchemaUpdater;
        }//ctor

        private CswEnumSchemaUpdateState _UpdateState = CswEnumSchemaUpdateState.Idle;
        public CswEnumSchemaUpdateState UpdateState
        {
            get
            {
                return ( _UpdateState );
            }
        }

        private string _Message = string.Empty;
        public string Message
        {
            get
            {
                return ( _Message );
            }
        }



        public void start()
        {
            _UpdateState = CswEnumSchemaUpdateState.Running;
            _Message = string.Empty;
            _UpdateThread = new Thread( new ThreadStart( _doUpdate ) );
            _UpdateThread.Start();


        }//start() 


        private Thread _UpdateThread = null;
        private void _doUpdate()
        {
            CmdLn.CswEnumSchemaUpdateState FinalState;
            try
            {
                 if( _CswSchemaUpdater.runNextVersionedScript() )
                {
                    FinalState = CmdLn.CswEnumSchemaUpdateState.Succeeded;

                }
                else
                {
                    FinalState = CmdLn.CswEnumSchemaUpdateState.Failed;
                    _Message = _CswSchemaUpdater.ErrorMessage;
                }

            }

            catch( Exception Exception )
            {
                FinalState = CmdLn.CswEnumSchemaUpdateState.Failed;
                _Message = "Update to schema version " + _CswSchemaUpdater.LatestVersion.ToString() + " failed: " + Exception.Message;
            }

            _UpdateState = FinalState;

        }//_doUpdate()

    }//CswSchemaUpdateThread

}//ChemSW.Nbt.Schema.CmdLn
