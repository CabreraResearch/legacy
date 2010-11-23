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

        private CswNbtWebServiceResources _CswNbtWebServiceResources;

        private AuthenticationStatus start( string AccessId, string UserName, string Password )
        {
            _CswNbtWebServiceResources = new CswNbtWebServiceResources( Context.Application,
                                                                        Context.Session,
                                                                        Context.Request,
                                                                        Context.Response,
                                                                        string.Empty,
                                                                        System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\etc",
                                                                        SetupMode.Web );

            _CswNbtWebServiceResources.startSession( AccessId, UserName, Password );


            return ( AuthenticationStatus.Authenticated );
        }


        private void start()
        {
            _CswNbtWebServiceResources = new CswNbtWebServiceResources( Context.Application,
                                                                        Context.Session,
                                                                        Context.Request,
                                                                        Context.Response,
                                                                        string.Empty,
                                                                        System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\etc",
                                                                        SetupMode.Web );
            _CswNbtWebServiceResources.startSession();
        }


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
                AuthenticationStatus AuthenticationStatus = start( AccessId, UserName, Password );
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    ReturnVal = _CswNbtWebServiceResources.CswNbtResources.CurrentNbtUser.RoleTimeout.ToString();
                }
                else
                {
                    switch( AuthenticationStatus )
                    {
                        case ChemSW.Security.AuthenticationStatus.Locked:
                            ReturnVal = "Account Locked";
                            break;

                        case ChemSW.Security.AuthenticationStatus.TooManyUsers:
                            ReturnVal = "Too many users; try again later";
                            break;

                        default:
                            ReturnVal = "Login failed";
                            break;
                    }


                }//if-else we authenticated


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
        public string UpdateProperties( string ParentId, string UpdatedViewXml )
        {
            string ReturnVal = string.Empty;
            try
            {
                start();

                CswNbtWebServiceUpdateProperties wsUP = new CswNbtWebServiceUpdateProperties( _CswNbtWebServiceResources );
                ReturnVal = result( wsUP.Run( ParentId, UpdatedViewXml ) );

                end();
            }
            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }
            return ( ReturnVal );
        } // UpdateProperties()


        [WebMethod( EnableSession = true )]
        public string RunView( string ParentId )
        {
            string ReturnVal = string.Empty;
            try
            {
                start();

                CswNbtWebServiceView wsView = new CswNbtWebServiceView( _CswNbtWebServiceResources );
                ReturnVal = result( wsView.Run( ParentId ) );

                end();
            }

            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }

            return ( ReturnVal );
        } // RunView()

        #endregion Web Methods

    }//wsNBT

} // namespace ChemSW.WebServices
