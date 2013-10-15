using System;
using System.ServiceModel;
using System.Xml;
//using ChemSW;
using NbtPrintLib.NbtPublic;

namespace NbtPrintLib
{
    public class CswPrintJobServiceThread
    {
        //private CookieManagerBehavior cookieBehavior = new CookieManagerBehavior();

        public abstract class ServiceThreadEventArgs
        {
            public bool Succeeded = false;
            public string Message = string.Empty;
        }

        #region Authentication and Session

        private NbtPublicClient _getClient( NbtAuth auth )
        {
            NbtPublicClient ret = new NbtPublicClient();
            string Url = auth.baseURL;
            ret.Endpoint.Address = new EndpointAddress( Url );
            ret.Endpoint.Binding = new WebHttpBinding()
            {
                AllowCookies = true,
                Security = new WebHttpSecurity()
                {
                    Mode = auth.useSSL ? WebHttpSecurityMode.Transport : WebHttpSecurityMode.None
                },
                ReaderQuotas = new XmlDictionaryReaderQuotas()
                    {
                        MaxStringContentLength = 20971520
                    }
            };

            //ret.Endpoint.Behaviors.Add( cookieBehavior );
            return ret;
        }

        public class NbtAuth
        {
            public string AccessId;
            public string UserId;
            public string Password;
            public bool useSSL;
            public string baseURL;
        }

        public delegate void AuthSuccessHandler( NbtPublicClient Client );

        private void _Authenticate( NbtAuth auth, ServiceThreadEventArgs e, AuthSuccessHandler success )
        {
            NbtPublicClient NbtClient = _getClient( auth );
            
            try
            {
                CswNbtWebServiceSessionCswNbtAuthReturn ret = NbtClient.SessionInit( new CswWebSvcSessionAuthenticateDataAuthenticationRequest()
                {
                    CustomerId = auth.AccessId,
                    UserName = auth.UserId,
                    Password = auth.Password,
                    IsMobile = true,
                    SuppressLog = true
                } );
                if( ret.Authentication.AuthenticationStatus == "Authenticated" )
                {
                    if( null != success )
                    {
                        try
                        {
                            success( NbtClient );
                        }
                        finally
                        {
                            NbtClient.SessionEnd();
                        }
                    }
                }
                else
                {
                    e.Message += "Authentication error: " + ret.Authentication.AuthenticationStatus;
                }
            }
            catch( Exception ex )
            {
                e.Message += "Authentication error: " + ex.Message;
            }
            finally
            {
                NbtClient.Close();
            }
        } // _Authenticate


        #endregion Authentication and Session

        #region RegisterLpc

        public class RegisterEventArgs : ServiceThreadEventArgs
        {
            public PrinterSetupData printer = new PrinterSetupData();
        }

        public delegate void RegisterEventHandler( RegisterEventArgs e );
        public event RegisterEventHandler OnRegisterLpc = null;

        public delegate void RegisterInvoker( NbtAuth auth, PrinterSetupData aprinter );

        public void Register( NbtAuth auth, PrinterSetupData aprinter )
        {
            RegisterEventArgs e = new RegisterEventArgs();

            _Authenticate( auth, e,
                           delegate( NbtPublicClient NbtClient ) // Success
                           {
                               try
                               {
                                   LabelPrinter lblPrn = new LabelPrinter();
                                   lblPrn.LpcName = aprinter.LPCname;
                                   lblPrn.Description = aprinter.Description;

                                   CswNbtLabelPrinterReg Ret = NbtClient.LpcRegister( lblPrn );

                                   if( Ret.Status.Success )
                                   {
                                       e.printer.PrinterKey = Ret.PrinterKey;
                                       e.printer.Message = "Registered PrinterKey=" + e.printer.PrinterKey;
                                       e.printer.Succeeded = true;
                                   }
                                   else
                                   {
                                       e.printer.Message = "Printer \"" + aprinter.LPCname + "\" registration failed. ";
                                       e.printer.PrinterKey = string.Empty;
                                       if( Ret.Status.Errors.Length > 0 )
                                       {
                                           e.printer.Message += Ret.Status.Errors[0].Message;
                                       }
                                   }
                               } //try
                               catch( Exception Error )
                               {
                                   e.Message = "Printer registration failed. Please check server settings.";
                                   e.printer.Message = "Printer registration failed. Please check server settings.";
                               }
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
            public PrinterSetupData printer = new PrinterSetupData();
        }

        public delegate void LabelByIdEventHandler( LabelByIdEventArgs e );
        public event LabelByIdEventHandler OnLabelById = null;

        public delegate void LabelByIdInvoker( NbtAuth auth, string labelid, string targetid, PrinterSetupData aprinter );
        public void LabelById( NbtAuth auth, string labelid, string targetid, PrinterSetupData aprinter )
        {
            LabelByIdEventArgs e = new LabelByIdEventArgs();
            e.printer.PrinterName = aprinter.PrinterName;

            _Authenticate( auth, e,
                           delegate( NbtPublicClient NbtClient ) // Success
                           {
                               NbtPrintLabelRequestGet nbtLabelget = new NbtPrintLabelRequestGet();
                               nbtLabelget.LabelId = labelid;
                               nbtLabelget.TargetId = targetid;

                               CswNbtLabelEpl epl = NbtClient.LpcGetLabel( nbtLabelget );

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
            public PrinterSetupData printer = new PrinterSetupData();
            public NbtAuth auth = new NbtAuth();
        }

        public event NextJobEventHandler OnNextJob = null;
        public delegate void NextJobEventHandler( NextJobEventArgs e );

        public delegate void NextJobInvoker( NbtAuth auth, PrinterSetupData aprinter );
        public void NextJob( NbtAuth auth, PrinterSetupData aprinter )
        {
            NextJobEventArgs e = new NextJobEventArgs();
            e.printer.PrinterKey = aprinter.PrinterKey;
            e.printer.PrinterName = aprinter.PrinterName;
            e.printer.LPCname = aprinter.LPCname;
            e.auth.AccessId = auth.AccessId;
            e.auth.UserId = auth.UserId;
            e.auth.Password = auth.Password;
            e.auth.baseURL = auth.baseURL;
            e.auth.useSSL = auth.useSSL;

            _Authenticate( auth, e,
                           delegate( NbtPublicClient NbtClient ) // Success
                           {
                               CswNbtLabelJobRequest labelReq = new CswNbtLabelJobRequest();
                               labelReq.PrinterKey = e.printer.PrinterKey;

                               CswNbtLabelJobResponse Ret = NbtClient.LpcGetNextJob( labelReq );

                               if( Ret.Status.Success )
                               {
                                   e.Succeeded = true;
                                   e.Job = Ret;
                                   aprinter.LPCname = Ret.PrinterName;
                               }
                               else
                               {
                                   e.Message = "Error calling NextLabelJob web service. ";
                                   if( Ret.Status.Errors.Length > 0 )
                                   {
                                       e.Message += Ret.Status.Errors[0].Message;
                                   }
                               }
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

        public delegate void UpdateJobEventHandler( UpdateJobEventArgs e );
        public event UpdateJobEventHandler OnUpdateJob = null;

        //these must match
        public delegate void UpdateJobInvoker( NbtAuth Auth, string jobKey, bool success, string errorMsg );

        public void updateJob( NbtAuth auth, string jobKey, bool success, string errorMsg )
        {
            UpdateJobEventArgs e = new UpdateJobEventArgs();

            _Authenticate( auth, e,
                           delegate( NbtPublicClient NbtClient ) // Success
                           {
                               CswNbtLabelJobUpdateRequest Request = new CswNbtLabelJobUpdateRequest();
                               Request.JobKey = jobKey;
                               Request.Succeeded = success;
                               Request.ErrorMessage = errorMsg;

                               CswNbtLabelJobUpdateResponse Ret = NbtClient.LpcUpdateJob( Request );

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
                           }
                );

            if( OnUpdateJob != null )
            {
                OnUpdateJob( e );
            }
        } // updateJob()

        #endregion updateJob

    } // class ServiceThread

} // namespace CswPrintClient1
