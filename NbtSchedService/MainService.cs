using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt.SchedService
{
    public partial class MainService : ServiceBase
    {
        CswNbtSchdItemRunner _CswNbtSchdItemRunner = null;
//        CswNbtSchdItemRunner _CswNbtSchdItemRunner = new CswNbtSchdItemRunner();

        public MainService()
        {
            InitializeComponent();
        }

        protected override void OnStart( string[] args )
        {
            try
            {
                _CswNbtSchdItemRunner = new CswNbtSchdItemRunner();
                _CswNbtSchdItemRunner.start();
            }

            catch( Exception Exception )
            {

                if ( null != _CswNbtSchdItemRunner )
                {
                    Exception OutgoingException = null; 
                    if ( !_CswNbtSchdItemRunner.logServiceRunException( Exception, ref OutgoingException ) )
                    {
                        if ( null != OutgoingException )
                        {
                            string Message = "The following exception was thrown while attempting to log an exception: " + OutgoingException.Message + " in " + OutgoingException.StackTrace + "; The original exception was: " + Exception.Message + " in " + Exception.StackTrace;
                            throw ( new Exception( Message ) );
                        }
                        else
                        {
                            //for some reason we need to re-throw as System.Exception for this to work 
                            //(the service doesn't like CswDniException???)
                            throw ( new System.Exception( Exception.Message + " in: " + Exception.StackTrace ) );
                        }
                    }
                }
                else
                {
                    throw ( new Exception( "Error starting NBT Schedule Service: CswNbtSchdItemRunner was not instanced") );
                }
                /*
                System.Diagnostics.EventLog EventLog1 = new System.Diagnostics.EventLog();

                if( !System.Diagnostics.EventLog.SourceExists( base.ServiceName ) )
                    System.Diagnostics.EventLog.CreateEventSource(
                       base.ServiceName, "Application" );

                EventLog1.Source = base.ServiceName;
                EventLog1.WriteEntry( Exception.Message , EventLogEntryType.Error );


                base.Stop();
                 */
            }

        }//OnStart()

        protected override void OnStop()
        {
            try
            {
                _CswNbtSchdItemRunner.stop();
            }

            catch( Exception Exception )
            {

                if( null != _CswNbtSchdItemRunner )
                {
                    Exception OutgoingException = null;
                    if( !_CswNbtSchdItemRunner.logServiceRunException( Exception, ref OutgoingException ) )
                    {
                        if( null != OutgoingException )
                        {
                            string Message = "The following exception was thrown while attempting to log an exception: " + OutgoingException.Message + " in " + OutgoingException.StackTrace + "; The original exception was: " + Exception.Message + " in " + Exception.StackTrace;
                            throw ( new Exception( Message ) );
                        }
                        else
                        {
                            //for some reason we need to re-throw as System.Exception for this to work 
                            //(the service doesn't like CswDniException???)
                            throw ( new System.Exception( Exception.Message + " in: " + Exception.StackTrace ) );
                        }
                    }
                }
            }

        }//OnStop()

    }//class MainService

}//namespace ChemSW.Nbt.SchedService
