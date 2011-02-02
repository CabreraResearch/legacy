using System;
using System.Web.Services;
using System.Web.Script.Services;   // supports ScriptService attribute
using ChemSW.Core;
using ChemSW.Config;
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

        private CswSessionResourcesNbt _SessionResources;
        private CswNbtResources _CswNbtResources;

        private string _FilesPath
        {
            get
            {
                return ( System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\etc" );
            }
        }

        private void start()
        {
            _SessionResources = new CswSessionResourcesNbt( Context.Application, Context.Session, Context.Request, Context.Response, string.Empty, _FilesPath, SetupMode.Web );
            _CswNbtResources = _SessionResources.CswNbtResources;

        }//start() 



        private void end()
        {
            if( _CswNbtResources != null )
            {
                _CswNbtResources.finalize();
                _CswNbtResources.release();
            }
            if( _SessionResources != null )
                _SessionResources.setCache();
        }

        private string error( Exception ex )
        {
            _CswNbtResources.CswLogger.reportError( ex );
            _CswNbtResources.Rollback();
            return "<error>Error: " + ex.Message + "</error>";
        }

        private string result( string ReturnVal )
        {
            return "<result>" + ReturnVal + "</result>";
        }

        #endregion Session and Resource Management

        #region Web Methods


        [WebMethod( EnableSession = true )]
        public string authenticate( string AccessId, string UserName, string Password )
        {
            string ReturnVal = string.Empty;
            try
            {
                start();
                _SessionResources.CswSessionManager.setAccessId( AccessId );
                AuthenticationStatus AuthenticationStatus = _SessionResources.CswSessionManager.Authenticate( UserName, Password, CswWebControls.CswNbtWebTools.getIpAddress() );
                ReturnVal = result( "<AuthenticationStatus>" + AuthenticationStatus + "</AuthenticationStatus>" );
                end();
            }
            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }
            return ( ReturnVal );
        }//authenticate()


        [WebMethod( EnableSession = true )]
        public string deAuthenticate()
        {
            string ReturnVal = string.Empty;
            try
            {
                _SessionResources.CswSessionManager.DeAuthenticate();
                end();
            }
            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }
            return ( ReturnVal );
        }//deAuthenticate()

        [WebMethod( EnableSession = true )]
        public string getWelcomeItems( string RoleId )
        {
            CswTimer Timer = new CswTimer();
            string ReturnVal = string.Empty;
            try
            {
                start();

                CswNbtWebServiceWelcomeItems ws = new CswNbtWebServiceWelcomeItems( _CswNbtResources );
                // Only administrators can get welcome content for other roles
                if( RoleId != string.Empty && _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                    ReturnVal = ws.GetWelcomeItems( RoleId );
                else
                    ReturnVal = ws.GetWelcomeItems( _CswNbtResources.CurrentNbtUser.RoleId.ToString() );

                end();
            }
            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }
            return ( ReturnVal );
        } // getWelcomeItems()

        [WebMethod( EnableSession = true )]
        public string getQuickLaunchItems()
        {
            CswTimer Timer = new CswTimer();
            string ReturnVal = string.Empty;
            try
            {
                start();

                CswPrimaryKey UserId = _CswNbtResources.CurrentNbtUser.UserId;
                CswNbtWebServiceQuickLaunchItems ql = new CswNbtWebServiceQuickLaunchItems( _CswNbtResources );
                if( null != UserId )
                {
                    ReturnVal = ql.getQuickLaunchItems( UserId );
                }

                end();
            }
            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }
            return ( ReturnVal );
        } // getQuickLaunchItems()

        [WebMethod( EnableSession = true )]
        public string getViews()
        {
            CswTimer Timer = new CswTimer();
            string ReturnVal = string.Empty;
            try
            {
                start();
                CswNbtWebServiceTreeView ws = new CswNbtWebServiceTreeView( _CswNbtResources );
                ReturnVal = ws.getViews();
                end();
            }
            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }
            return ( ReturnVal );
        } // getViews()

        [WebMethod( EnableSession = true )]
        public string getDashboard()
        {
            string ReturnVal = string.Empty;
            try
            {
                start();
                CswNbtWebServiceHeader ws = new CswNbtWebServiceHeader( _CswNbtResources );
                ReturnVal = ws.getDashboard();
                end();
            }
            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }
            return ( ReturnVal );
        } // getDashboard()

        [WebMethod( EnableSession = true )]
        public string getHeaderMenu()
        {
            string ReturnVal = string.Empty;
            try
            {
                start();
                CswNbtWebServiceHeader ws = new CswNbtWebServiceHeader( _CswNbtResources );
                ReturnVal = ws.getHeaderMenu();
                end();
            }
            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }
            return ( ReturnVal );
        } // getHeaderMenu()

        [WebMethod( EnableSession = true )]
        public string getTree( Int32 ViewId )
        {
            string ReturnVal = string.Empty;
            try
            {
                start();
                CswNbtWebServiceTreeView ws = new CswNbtWebServiceTreeView( _CswNbtResources );
                ReturnVal = ws.getTree( ViewId );
                end();
            }
            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }
            return ( ReturnVal );
        } // getTree()


        [WebMethod( EnableSession = true )]
        public string getTabs( string NodePk )
        {
            string ReturnVal = string.Empty;
            try
            {
                start();
                CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                ReturnVal = ws.getTabs( NodePk );
                end();
            }
            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }
            return ( ReturnVal );
        } // getTabs()

        [WebMethod( EnableSession = true )]
        public string getProps( string NodePk, string TabId )
        {
            string ReturnVal = string.Empty;
            try
            {
                start();
                CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                ReturnVal = ws.getProps( NodePk, TabId );
                end();
            }
            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }
            return ( ReturnVal );
        } // getProps()

        [WebMethod( EnableSession = true )]
        public string saveProps( string NodePk, string NewPropsXml )
        {
            string ReturnVal = string.Empty;
            try
            {
                start();
                CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                ReturnVal = ws.saveProps( NodePk, NewPropsXml );
                end();
            }
            catch( Exception ex )
            {
                ReturnVal = error( ex );
            }
            return ( ReturnVal );
        } // saveProps()

        #endregion Web Methods

    }//wsNBT

} // namespace ChemSW.WebServices
