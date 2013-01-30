using System;
using ChemSW;
using CswPrintClient1.NbtLabels;
using CswPrintClient1.NbtSession;

namespace CswPrintClient1
{
    class ServiceThread
    {
        private Labels2Client labelClient = new NbtLabels.Labels2Client();
        private SessionClient sessionClient = new SessionClient();
        private CookieManagerBehavior cookieBehavior = new CookieManagerBehavior();

        public ServiceThread()
        {
            labelClient.Endpoint.Behaviors.Add( cookieBehavior );
            sessionClient.Endpoint.Behaviors.Add( cookieBehavior );
        }

        public abstract class ServiceThreadEventArgs
        {
            public bool Succeeded = false;
            public string Message = string.Empty;
        }

        #region Authentication and Session

        public class NbtAuth
        {
            public string AccessId;
            public string UserId;
            public string Password;
        }

        public delegate void AuthSuccessHandler();
        public delegate void AuthFailureHandler(string ErrorMessage);

        private void _Authenticate( NbtAuth auth, AuthSuccessHandler success, AuthFailureHandler failure )
        {
            NbtSession.CswWebSvcReturn ret = sessionClient.Init( new NbtSession.CswWebSvcSessionAuthenticateDataAuthenticationRequest()
            {
                CustomerId = auth.AccessId,
                UserName = auth.UserId,
                Password = auth.Password,
                IsMobile = false
            } );
            try
            {
                if( ret.Authentication.AuthenticationStatus == "Authenticated" )
                {
                    if( null != success )
                    {
                        try
                        {
                            success();
                        }
                        finally
                        {
                            sessionClient.End();
                        }
                    }
                }
                else
                {
                    if( null != failure )
                    {
                        failure("Authentication error: " + ret.Authentication.AuthenticationStatus);
                    }
                }
            }
            catch( Exception ex )
            {
                if( null != failure )
                {
                    failure( "Authentication error: " + ex.Message );
                }
            }
            finally
            {
                sessionClient.Close();
            }
        } // _Authenticate


        #endregion Authentication and Session

        #region RegisterLpc

        public class RegisterEventArgs : ServiceThreadEventArgs
        {
            public string PrinterKey;
        }

        public delegate void RegisterEventHandler( RegisterEventArgs e );
        public event RegisterEventHandler OnRegisterLpc = null;

        public delegate void RegisterInvoker( NbtAuth auth, string lpcname, string descript );

        public void Register( NbtAuth auth, string lpcname, string descript )
        {
            RegisterEventArgs e = new RegisterEventArgs();

            _Authenticate( auth,
                           delegate() // Success
                               {
                                   LabelPrinter lblPrn = new LabelPrinter();
                                   lblPrn.LpcName = lpcname;
                                   lblPrn.Description = descript;
                                   
                                   CswPrintClient1.NbtLabels.CswNbtLabelPrinterReg Ret = labelClient.registerLpc( lblPrn );
                                   labelClient.Close();

                                   if( Ret.Status.Success )
                                   {
                                       e.PrinterKey = Ret.PrinterKey;
                                       e.Message = "Registered PrinterKey=" + e.PrinterKey;
                                       e.Succeeded = true;
                                   }
                                   else
                                   {
                                       e.Message = "Printer \"" + lblPrn.LpcName + "\" registration failed. ";
                                       e.PrinterKey = string.Empty;
                                       if( Ret.Status.Errors.Length > 0 )
                                       {
                                           e.Message += Ret.Status.Errors[0].Message;
                                       }
                                   }
                               },
                           delegate( string msg ) // Failure
                               {
                                   e.Message = msg;
                               }
                        );

            if( OnRegisterLpc != null )
            {
                OnRegisterLpc( e );
            }
        } // Register()

        #endregion

        #region LabelById

        public class LabelByIdEventArgs : ServiceThreadEventArgs
        {
            public string LabelData;
        }
        
        public delegate void LabelByIdEventHandler( LabelByIdEventArgs e );
        public event LabelByIdEventHandler OnLabelById = null;

        public delegate void LabelByIdInvoker( NbtAuth auth, string labelid, string targetid );
        public void LabelById( NbtAuth auth, string labelid, string targetid )
        {
            LabelByIdEventArgs e = new LabelByIdEventArgs();
            
            _Authenticate( auth,
                           delegate() // Success
                               {
                                   NbtPrintLabelRequestGet nbtLabelget = new NbtPrintLabelRequestGet();
                                   nbtLabelget.LabelId = labelid;
                                   nbtLabelget.TargetId = targetid;

                                   CswNbtLabelEpl epl = labelClient.getLabel( nbtLabelget );
                                   labelClient.Close();

                                   if( epl.Status.Success )
                                   {
                                       if( epl.Data.Labels.Length < 1 )
                                       {
                                           e.Message = "No labels returned.";
                                       }
                                       else
                                       {
                                           e.Succeeded = true;
                                           foreach( PrintLabel p in epl.Data.Labels )
                                           {
                                               e.LabelData += p.EplText + "\r\n";
                                           }
                                       }
                                   }
                                   else
                                   {
                                       e.Message += epl.Status.Errors[0].Message;
                                   }
                               },
                           delegate( string msg ) // Failure
                               {
                                   e.Message = msg;
                               }
                        );

            if( OnLabelById != null )
            {
                OnLabelById( e );
            }
        } // LabelById()

        #endregion

        #region GetNextPrintJob

        public class NextJobEventArgs : ServiceThreadEventArgs
        {
            public CswNbtLabelJobResponse Job;
        }

        public event NextJobEventHandler OnNextJob = null;
        public delegate void NextJobEventHandler( NextJobEventArgs e );

        public delegate void NextJobInvoker( NbtAuth auth, string printerkey );
        public void NextJob( NbtAuth auth, string printerkey )
        {
            NextJobEventArgs e = new NextJobEventArgs();

            _Authenticate( auth,
                           delegate() // Success
                               {
                                   CswNbtLabelJobRequest labelReq = new CswNbtLabelJobRequest();
                                   labelReq.PrinterKey = printerkey;

                                   CswNbtLabelJobResponse Ret = labelClient.getNextLpcJob( labelReq );
                                   labelClient.Close();

                                   if( Ret.Status.Success )
                                   {
                                       e.Succeeded = true;
                                       e.Job = Ret;
                                   }
                                   else
                                   {
                                       e.Message = "Error calling NextLabelJob web service. ";
                                       if( Ret.Status.Errors.Length > 0 )
                                       {
                                           e.Message += Ret.Status.Errors[0].Message;
                                       }
                                   }
                               },
                           delegate( string msg ) // Failure
                               {
                                   e.Message = msg;
                               }
                        );

            if( OnNextJob != null )
            {
                OnNextJob( e );
            }
        }

        #endregion

        #region updateJob

        public class UpdateJobEventArgs : ServiceThreadEventArgs
        {
        }

        public delegate void UpdateJobEventHandler(UpdateJobEventArgs e);
        public event UpdateJobEventHandler OnUpdateJob = null;

        //these must match
        public delegate void UpdateJobInvoker( NbtAuth Auth, string jobKey );
        public void updateJob( NbtAuth auth, string jobKey )
        {
            UpdateJobEventArgs e = new UpdateJobEventArgs();

            _Authenticate( auth,
                delegate() // Success
                    {
                        CswNbtLabelJobUpdateRequest Request = new CswNbtLabelJobUpdateRequest();
                        Request.JobKey = jobKey;

                        CswNbtLabelJobUpdateResponse Ret = labelClient.updateLpcJob( Request );

                        if( Ret.Status.Success )
                        {
                            e.Succeeded = true;
                        }
                        else
                        {
                            e.Message = "Error updating job: ";
                            if( Ret.Status.Errors.Length > 0 )
                            {
                                e.Message += Ret.Status.Errors[0].Message;
                            }
                        }
                    },
                delegate( string msg ) // Failure
                    {
                    }
            );

            if( OnUpdateJob != null )
            {
                OnUpdateJob( e );
            }
        } // updateJob()

        #endregion updateJob

    }

}
