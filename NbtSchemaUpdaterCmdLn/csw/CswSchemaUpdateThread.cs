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
using ChemSW.Nbt.Config;
using ChemSW.Nbt.Schema;
using ChemSW.Nbt;


namespace ChemSW.Nbt.Schema.CmdLn
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public enum UpdateState { Idle, Running, Complete };
    public class CswSchemaUpdateThread
    {
        private CswSchemaUpdater _CswSchemaUpdater = null;
        public CswSchemaUpdateThread( CswSchemaUpdater CswSchemaUpdater )
        {
            _CswSchemaUpdater = CswSchemaUpdater;
        }//ctor

        private UpdateState _UpdateState = UpdateState.Idle;
        public UpdateState UpdateState
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
            _Message = string.Empty;
            _UpdateState = UpdateState.Running;
            _UpdateThread = new Thread( new ThreadStart( _doUpdate ) );
            _UpdateThread.Start();


        }//start() 


        private Thread _UpdateThread = null;
        private void _doUpdate()
        {
            try
            {
                if( _CswSchemaUpdater.Update() )
                {
                    _Message = "Update to schema version " + _CswSchemaUpdater.LatestVersion.ToString() + " is complete";
                }
                else
                {
                    _Message = _CswSchemaUpdater.ErrorMessage;
                }

            }

            catch( Exception Exception )
            {
                _Message = "Update to schema version " + _CswSchemaUpdater.LatestVersion.ToString() + " failed: " + Exception.Message;
            }

            _UpdateState = CmdLn.UpdateState.Complete;

        }//_doUpdate()


    }//CswSchemaUpdateThread

}//ChemSW.Nbt.Schema.CmdLn
