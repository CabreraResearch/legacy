using System;
using ChemSW;
using CswPrintClient1.NbtLabels;
using CswPrintClient1.NbtSession;

namespace CswPrintClient1
{
    class ServiceThread
    {
        #region RegisterLpc
        public event RegisterEventHandler OnRegisterLpc = null;

        public class RegisterEventArgs
        {
            public bool Succeeded;
            public string Message;
            public string PrinterKey;
        }
        public delegate void RegisterEventHandler( RegisterEventArgs e );

        //these must match
        public delegate void RegisterInvoker( string accessid, string userid, string pwd, string lpcname, string descript );
        public void Register( string accessid, string userid, string pwd, string lpcname, string descript )
        {
            RegisterEventArgs e = new RegisterEventArgs();
            e.Succeeded = false;
            e.Message = "";

            //try login
            CookieManagerBehavior cookieBehavior = new CookieManagerBehavior();
            SessionClient mySession = new SessionClient();
            mySession.Endpoint.Behaviors.Add( cookieBehavior );

            NbtSession.CswWebSvcReturn ret = mySession.Init( new NbtSession.CswWebSvcSessionAuthenticateDataAuthenticationRequest()
            {
                CustomerId = accessid,
                UserName = userid,
                Password = pwd,
                IsMobile = false
            } );
            try
            {
                e.Message = "";
                if( ret.Authentication.AuthenticationStatus == "Authenticated" )
                {
                    try
                    {

                        //logged in
                        Labels2Client l = new NbtLabels.Labels2Client();
                        l.Endpoint.Behaviors.Add( cookieBehavior );
                        LabelPrinter lblPrn = new LabelPrinter();
                        lblPrn.LpcName = lpcname;
                        lblPrn.Description = descript;
                        CswPrintClient1.NbtLabels.CswNbtLabelPrinterReg Ret = l.registerLpc( lblPrn );
                        l.Close();
                        if( Ret.Status.Success == true )
                        {
                            e.PrinterKey = Ret.PrinterKey;
                            e.Message = "Registered PrinterKey=" + e.PrinterKey;
                            e.Succeeded = true;
                        }
                        else
                        {
                            e.Message = "Printer " + lblPrn.LpcName + " registration failed. ";
                            e.PrinterKey = string.Empty;
                            if( Ret.Status.Errors.Length > 0 )
                            {
                                e.Message += Ret.Status.Errors[0].Message.ToString();
                            }
                        }
                    }
                    finally
                    {
                        //LOGOUT
                        mySession.End();
                    }
                }
                else
                {
                    e.Message = "Authentication error: " + ret.Authentication.AuthenticationStatus;
                }
            }
            catch( Exception ex )
            {
                e.Message = ex.Message;
            }
            finally
            {
                mySession.Close();
            }



            if( OnRegisterLpc != null )
            {
                //return
                OnRegisterLpc( e );
            }
        }

        #endregion

        #region LabelById
        public event LabelByIdEventHandler OnLabelById = null;

        public class LabelByIdEventArgs
        {
            public bool Succeeded;
            public string Message;
            public string LabelData;
        }
        public delegate void LabelByIdEventHandler( LabelByIdEventArgs e );

        //these must match
        public delegate void LabelByIdInvoker( string accessid, string userid, string pwd, string labelid, string targetid );
        public void LabelById( string accessid, string userid, string pwd, string labelid, string targetid )
        {
            LabelByIdEventArgs e = new LabelByIdEventArgs();
            e.Succeeded = false;
            e.Message = "";

            //try login
            CookieManagerBehavior cookieBehavior = new CookieManagerBehavior();
            SessionClient mySession = new SessionClient();
            mySession.Endpoint.Behaviors.Add( cookieBehavior );

            NbtSession.CswWebSvcReturn ret = mySession.Init( new NbtSession.CswWebSvcSessionAuthenticateDataAuthenticationRequest()
            {
                CustomerId = accessid,
                UserName = userid,
                Password = pwd,
                IsMobile = false
            } );
            try
            {
                e.Message = "";
                if( ret.Authentication.AuthenticationStatus == "Authenticated" )
                {
                    try
                    {

                        //logged in
                        Labels2Client l = new NbtLabels.Labels2Client();
                        l.Endpoint.Behaviors.Add( cookieBehavior );

                        NbtPrintLabelRequestGet nbtLabelget = new NbtPrintLabelRequestGet();
                        nbtLabelget.LabelId = labelid;
                        nbtLabelget.TargetId = targetid;
                        CswNbtLabelEpl epl = l.getLabel( nbtLabelget );
                        if( epl.Status.Success == true )
                        {

                            if( epl.Data.Labels.Length < 1 )
                            {
                                e.Succeeded = false;
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
                            e.Succeeded = false;
                            e.Message += epl.Status.Errors[0].Message.ToString();
                        }
                        l.Close();
                    }
                    finally
                    {
                        //LOGOUT
                        mySession.End();
                    }
                }
                else
                {
                    e.Message = "Authentication error: " + ret.Authentication.AuthenticationStatus;
                }
            }
            catch( Exception ex )
            {
                e.Message = ex.Message;
            }
            finally
            {
                mySession.Close();
            }



            if( OnLabelById != null )
            {
                //return
                OnLabelById( e );
            }
        }

        #endregion



        #region GetNextPrintJob
        public event NextJobEventHandler OnNextJob = null;

        public class NextJobEventArgs
        {
            public bool Succeeded;
            public string Message;
            public string JobNo;
            public string JobOwner;
            public string LabelCount;
            public string LabelName;
            public string PrinterData;
        }
        public delegate void NextJobEventHandler( NextJobEventArgs e );

        //these must match
        public delegate void NextJobInvoker( string accessid, string userid, string pwd, string printerkey );
        public void NextJob( string accessid, string userid, string pwd, string printerkey )
        {
            NextJobEventArgs e = new NextJobEventArgs();
            e.Succeeded = false;
            e.Message = "";

            //try login
            CookieManagerBehavior cookieBehavior = new CookieManagerBehavior();
            SessionClient mySession = new SessionClient();
            mySession.Endpoint.Behaviors.Add( cookieBehavior );

            NbtSession.CswWebSvcReturn ret = mySession.Init( new NbtSession.CswWebSvcSessionAuthenticateDataAuthenticationRequest()
            {
                CustomerId = accessid,
                UserName = userid,
                Password = pwd,
                IsMobile = false
            } );
            try
            {
                e.Message = "";
                if( ret.Authentication.AuthenticationStatus == "Authenticated" )
                {
                    try
                    {

                        //logged in
                        Labels2Client l = new NbtLabels.Labels2Client();
                        l.Endpoint.Behaviors.Add( cookieBehavior );

                        CswNbtLabelJobRequest labelReq = new CswNbtLabelJobRequest();
                        labelReq.PrinterKey = printerkey;
                        CswPrintClient1.NbtLabels.CswNbtLabelJobResponse Ret = l.getNextLpcJob( labelReq );
                        l.Close();
                        if( Ret.Status.Success == true )
                        {
                            e.Succeeded = true;
                            e.JobNo = Ret.JobNo;
                            e.JobOwner = Ret.JobOwner;
                            e.LabelCount = Ret.LabelCount;
                            e.LabelName = Ret.LabelName;
                            e.PrinterData = Ret.LabelData;
                        }
                        else
                        {
                            e.Succeeded = false;
                            e.Message = "Error calling NextLabelJob web service. ";
                            if( Ret.Status.Errors.Length > 0 )
                            {
                                e.Message += Ret.Status.Errors[0].Message;
                            }
                        }
                    }
                    finally
                    {
                        //LOGOUT
                        mySession.End();
                    }
                }
                else
                {
                    e.Message = "Authentication error: " + ret.Authentication.AuthenticationStatus;
                }
            }
            catch( Exception ex )
            {
                e.Message = ex.Message;
            }
            finally
            {
                mySession.Close();
            }



            if( OnNextJob != null )
            {
                //return
                OnNextJob( e );
            }
        }

        #endregion


    }

}
