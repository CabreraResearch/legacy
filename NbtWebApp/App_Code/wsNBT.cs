using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;   // supports ScriptService attribute
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Config;
using ChemSW.Nbt.PropTypes;
using ChemSW.Session;
using ChemSW.Security;
using ChemSW.NbtWebControls;

namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// NBT Web service interface
    /// </summary>
    /// 
    [ScriptService]
    [WebService( Namespace = "http://localhost/NbtWebApp" )]
    [WebServiceBinding( ConformsTo = WsiProfiles.BasicProfile1_1 )]
    public class wsNBT : System.Web.Services.WebService
    {
        #region Session and Resource Management

        private CswNbtWebServiceResources __CswNbtWebServiceResources;
        private CswNbtWebServiceResources _CswNbtWebServiceResources
        {
            get
            {
                if( null == __CswNbtWebServiceResources )
                {
                    __CswNbtWebServiceResources = new CswNbtWebServiceResources( Context.Application,
                                                                                Context.Session,
                                                                                Context.Request,
                                                                                Context.Response,
                                                                                string.Empty,
                                                                                System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\etc",
                                                                                SetupMode.Web );
                }//if not created yet

                return ( __CswNbtWebServiceResources ); 
            }//get
        }//_CswNbtWebServiceResources

        private AuthenticationStatus start( string SessionId, ref string ExotericAuthenticationResult )
        {
            string EuphemisticAuthenticationStatus = string.Empty;
            AuthenticationStatus AuthenticationStatus = _CswNbtWebServiceResources.startSession( SessionId, ref EuphemisticAuthenticationStatus );

            ExotericAuthenticationResult = "<AuthenticationStatus>" + EuphemisticAuthenticationStatus + "</AuthenticationStatus>";

            return ( AuthenticationStatus );

        }//start() 



        private void end()
        {
            _CswNbtWebServiceResources.endSession( EndSessionMode.esmCommit );
        }

        private string error( Exception ex )
        {
            _CswNbtWebServiceResources.CswNbtResources.CswLogger.reportError( ex );
            _CswNbtWebServiceResources.endSession( EndSessionMode.esmRollback );
            return "<error>Error: " + ex.Message + "</error>";
        }

        private string result( string ReturnVal )
        {
            return "<result>" + ReturnVal + "</result>";
        }

        #endregion Session and Resource Management

        #region Web Methods


        [WebMethod]
        public string Authenticate( string AccessId, string UserName, string Password )
        {
            string ReturnVal = string.Empty;
            try
            {
                string ExotericAuthenticationResult = string.Empty;

                string EuphemisticAuthenticationStatus = string.Empty;
                string SessionId = string.Empty;
                AuthenticationStatus AuthenticationStatus = _CswNbtWebServiceResources.authenticate( AccessId, UserName, Password, ref EuphemisticAuthenticationStatus, ref SessionId );
                ExotericAuthenticationResult = "<AuthenticationStatus>" + EuphemisticAuthenticationStatus + "</AuthenticationStatus>";

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    ReturnVal = "<SessionId>" + SessionId + "</SessionId>";
                }

                ReturnVal += ExotericAuthenticationResult;

                ReturnVal = result( ReturnVal );

                end();
            }

            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }

            return ( ReturnVal );

        }//Authenticate()


        [WebMethod]
        public string deAuthenticate( string SessionId )
        {
            string ReturnVal = string.Empty;
            try
            {
                __CswNbtWebServiceResources.deAuthenticate( SessionId ); 
                ReturnVal = result( "SessionId " + SessionId + " removed"  );

                end();
            }

            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }

            return ( ReturnVal );

        }//Authenticate()


        [WebMethod( EnableSession = true )]
        public string ConnectTest()
        {
            // no session needed here
            return ( result( "Connected" ) );
        }


        [WebMethod( EnableSession = true )]
        public string ConnectTestFail()
        {
            // no session needed here

            // this exception needs to be UNCAUGHT
            throw new Exception( "Emulated connection failure" );
        }

        [WebMethod( EnableSession = true )]
        public string ConnectTestRandomFail()
        {
            // no session needed here

            // this exception needs to be UNCAUGHT
            Random r = new Random();
            Int32 rand = r.Next( 0, 3 );
            if(rand == 0)
                throw new Exception( "Emulated connection failure" );
            else
                return ( result( "Connected" ) );
        }


        [WebMethod]
        public string UpdateProperties( string SessionId, string ParentId, string UpdatedViewXml, bool ForMobile )
        {
            string ReturnVal = string.Empty;
            try
            {
                string EuphemisticAuthenticationStatus = string.Empty;
                if( AuthenticationStatus.Authenticated == start( SessionId, ref EuphemisticAuthenticationStatus ) )
                {

                    CswNbtWebServiceUpdateProperties wsUP = new CswNbtWebServiceUpdateProperties( _CswNbtWebServiceResources, ForMobile );
                    ReturnVal = result( wsUP.Run( ParentId, UpdatedViewXml ) );

                    end();
                }
                else
                {
                    ReturnVal = result( EuphemisticAuthenticationStatus );
                }
            }

            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }

            return ( ReturnVal );
        } // UpdateProperties()


        [WebMethod]
        public string RunView( string SessionId, string ParentId, bool ForMobile )
        {
            string ReturnVal = string.Empty;
            try
            {
                string EuphemisticAuthenticationStatus = string.Empty;
                if( AuthenticationStatus.Authenticated == start( SessionId, ref EuphemisticAuthenticationStatus ) )
                {

                    CswNbtWebServiceView wsView = new CswNbtWebServiceView( _CswNbtWebServiceResources, ForMobile );
                    ReturnVal = result( wsView.Run( ParentId ) );

                    end();
                }
                else
                {
                    ReturnVal = result( EuphemisticAuthenticationStatus );
                }
            }

            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }

            return ( ReturnVal );
        } // RunView()


        [WebMethod]
        public string JQueryGetViews( string SessionId )
        {
            CswTimer Timer = new CswTimer();
            string ReturnVal = string.Empty;
            try
            {
                string EuphemisticAuthenticationStatus = string.Empty;
                if( AuthenticationStatus.Authenticated == start( SessionId, ref EuphemisticAuthenticationStatus ) )
                {

                    _CswNbtWebServiceResources.CswNbtResources.logTimerResult( "before JQueryGetViews", Timer.ElapsedDurationInSecondsAsString );

                    CswNbtWebServiceJQuery ws = new CswNbtWebServiceJQuery( _CswNbtWebServiceResources );
                    ReturnVal = ws.getViews();

                    _CswNbtWebServiceResources.CswNbtResources.logTimerResult( "after JQueryGetViews", Timer.ElapsedDurationInSecondsAsString );

                    end();
                }
                else
                {
                    ReturnVal = result( EuphemisticAuthenticationStatus );
                }
            }

            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }
            _CswNbtWebServiceResources.CswNbtResources.logTimerResult( "end JQueryGetViews", Timer.ElapsedDurationInSecondsAsString );
            return ( ReturnVal );
        } // JQueryGetViews()

        [WebMethod]
        public string JQueryGetDashboard( string SessionId )
        {
            string ReturnVal = string.Empty;
            try
            {
                string EuphemisticAuthenticationStatus = string.Empty;
                if( AuthenticationStatus.Authenticated == start( SessionId, ref EuphemisticAuthenticationStatus ) )
                {
                    CswNbtWebServiceJQuery ws = new CswNbtWebServiceJQuery( _CswNbtWebServiceResources );
                    ReturnVal = ws.getDashboard();

                    end();
                }
                else
                {
                    ReturnVal = result( EuphemisticAuthenticationStatus );
                }
            }

            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }
            return ( ReturnVal );
        } // JQueryGetDashboard()

        [WebMethod]
        public string JQueryGetHeaderMenu( string SessionId )
        {
            string ReturnVal = string.Empty;
            try
            {
                string EuphemisticAuthenticationStatus = string.Empty;
                if( AuthenticationStatus.Authenticated == start( SessionId, ref EuphemisticAuthenticationStatus ) )
                {
                    CswNbtWebServiceJQuery ws = new CswNbtWebServiceJQuery( _CswNbtWebServiceResources );
                    ReturnVal = ws.getHeaderMenu();

                    end();
                }
                else
                {
                    ReturnVal = result( EuphemisticAuthenticationStatus );
                }
            }

            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }
            return ( ReturnVal );
        } // JQueryGetDashboard()

        [WebMethod]
        public string JQueryGetTree( string SessionId, Int32 ViewId )
        {
            string ReturnVal = string.Empty;
            try
            {
                string EuphemisticAuthenticationStatus = string.Empty;
                if( AuthenticationStatus.Authenticated == start( SessionId, ref EuphemisticAuthenticationStatus ) )
                {

                    CswNbtWebServiceJQuery ws = new CswNbtWebServiceJQuery( _CswNbtWebServiceResources );
                    ReturnVal = ws.getTree( ViewId );

                    end();
                }
                else
                {
                    ReturnVal = result( EuphemisticAuthenticationStatus );
                }
            }

            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }

            return ( ReturnVal );
        } // JQueryGetTree()


        [WebMethod]
        public string JQueryGetTabs( string SessionId, string NodePk )
        {
            string ReturnVal = string.Empty;
            try
            {
                string EuphemisticAuthenticationStatus = string.Empty;
                if( AuthenticationStatus.Authenticated == start( SessionId, ref EuphemisticAuthenticationStatus ) )
                {

                    CswNbtWebServiceJQuery ws = new CswNbtWebServiceJQuery( _CswNbtWebServiceResources );
                    ReturnVal = ws.getTabs( NodePk );

                    end();
                }
                else
                {
                    ReturnVal = result( EuphemisticAuthenticationStatus );
                }
            }

            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }

            return ( ReturnVal );
        } // JQueryGetTabs()


        [WebMethod]
        public string JQueryGetProps( string SessionId, string NodePk, string TabId )
        {
            string ReturnVal = string.Empty;
            try
            {
                string EuphemisticAuthenticationStatus = string.Empty;
                if( AuthenticationStatus.Authenticated == start( SessionId, ref EuphemisticAuthenticationStatus ) )
                {

                    CswNbtWebServiceJQuery ws = new CswNbtWebServiceJQuery( _CswNbtWebServiceResources );
                    ReturnVal = ws.getProps( NodePk, TabId );

                    end();
                }
                else
                {
                    ReturnVal = result( EuphemisticAuthenticationStatus );
                }
            }

            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }

            return ( ReturnVal );
        } // JQueryGetTab()

        #endregion Web Methods

    }//wsNBT

} // namespace ChemSW.WebServices
