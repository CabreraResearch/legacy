using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Web.Services;
using System.Web.Script.Services;   // supports ScriptService attribute
using ChemSW.Core;
using ChemSW.Config;
using ChemSW.Security;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using System.Xml.Linq;
using System.Collections.Generic;

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

		private const string QuickLaunchViews = "QuickLaunchViews";
		/// <summary>
		/// Append to QuickLaunch
		/// </summary>
		private void addToQuickLaunch(CswNbtView View)
		{
			if( ( View.ViewId > 0 ) || ( View.ViewId <= 0 && View.SessionViewId > 0 ) )
			{
				//Tuple == Item1: itemid (view/action), Item2: name, Item3: url(Action),viewmod(View), Item4: item type (Action/View)
				LinkedList<Tuple<Int32, string, string, CswNbtWebServiceQuickLaunchItems.QuickLaunchType>> ViewHistoryList = null;
				if( null == Session[QuickLaunchViews] )
				{
					ViewHistoryList = new LinkedList<Tuple<int, string, string, CswNbtWebServiceQuickLaunchItems.QuickLaunchType>>();
				}
				else
				{
					ViewHistoryList = (LinkedList<Tuple<Int32, string, string, CswNbtWebServiceQuickLaunchItems.QuickLaunchType>>) Session[QuickLaunchViews];
				}
				var ThisView = new Tuple<Int32, string, string, CswNbtWebServiceQuickLaunchItems.QuickLaunchType>
					( View.ViewId, View.ViewName, View.ViewMode.ToString(), CswNbtWebServiceQuickLaunchItems.QuickLaunchType.View );

				if( ViewHistoryList.Contains( ThisView ) )
				{
					ViewHistoryList.Remove( ThisView );
				}
				ViewHistoryList.AddFirst( ThisView );
				Session[QuickLaunchViews] = ViewHistoryList;
			}
		} // addToQuickLaunch()

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
				//ReturnVal = result( "<AuthenticationStatus>" + AuthenticationStatus + "</AuthenticationStatus>" );
				ReturnVal = "{ \"AuthenticationStatus\": \"" + AuthenticationStatus + "\" }";
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
						start();
						_SessionResources.CswSessionManager.DeAuthenticate();
						 ReturnVal = "{ \"Deauthentication\": \"Succeeded\" }";
						end();
			}
			catch( Exception ex )
			{
				ReturnVal = error( ex );
			}
			return ( ReturnVal );
		}//deAuthenticate()

		[WebMethod( EnableSession = true )]
		public XmlDocument getWelcomeItems( string RoleId )
		{
			CswTimer Timer = new CswTimer();
			string ReturnVal = string.Empty;
			try
			{
				start();

				var ws = new CswNbtWebServiceWelcomeItems( _CswNbtResources );
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
			//return ( ReturnVal );
			XmlDocument Doc = new XmlDocument();
			Doc.LoadXml( ReturnVal );
			return Doc;
		} // getWelcomeItems()

		[WebMethod( EnableSession = true )]
		public XmlDocument getQuickLaunchItems()
		{
			var ReturnXML = new XmlDocument();
			try
			{
				start();

				CswPrimaryKey UserId = _CswNbtResources.CurrentNbtUser.UserId;
				var ql = new CswNbtWebServiceQuickLaunchItems( _CswNbtResources, Session );
				if( null != UserId )
				{
					ReturnXML = ql.getQuickLaunchItems( UserId, Session );
				}

				end();
			}
			catch( Exception ex )
			{
				ReturnXML.LoadXml( error( ex ) );
			}

			return ReturnXML;
		} // getQuickLaunchItems()


		//[WebMethod( EnableSession = true )]
		//public XmlDocument getViews()
		//{
		//    CswTimer Timer = new CswTimer();
		//    string ReturnVal = string.Empty;
		//    try
		//    {
		//        start();
		//        CswNbtWebServiceTreeView ws = new CswNbtWebServiceTreeView( _CswNbtResources );
		//        ReturnVal = ws.getViews();
		//        end();
		//    }
		//    catch( Exception ex )
		//    {
		//        ReturnVal = error( ex );
		//    }
		//    //return ( ReturnVal );
		//    XmlDocument Doc = new XmlDocument();
		//    Doc.LoadXml( ReturnVal );
		//    return Doc;
		//} // getViews()

		[WebMethod( EnableSession = true )]
		public XmlDocument getViewTree()
		{
			string ReturnVal = string.Empty;
			try
			{
				start();
				var ws = new CswNbtWebServiceView( _CswNbtResources );
				ReturnVal = ws.getViewTree(Session);
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = error( ex );
			}
			//return ( ReturnVal );
			XmlDocument Doc = new XmlDocument();
			Doc.LoadXml( ReturnVal );
			return Doc;
		} // getViews()

		[WebMethod( EnableSession = true )]
		public XmlDocument getDashboard()
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
			//return ( ReturnVal );
			XmlDocument Doc = new XmlDocument();
			Doc.LoadXml( ReturnVal );
			return Doc;
		} // getDashboard()

		[WebMethod( EnableSession = true )]
		public XmlDocument getHeaderMenu()
		{
			string ReturnVal = string.Empty;
			try
			{
				start();
				var ws = new CswNbtWebServiceHeader( _CswNbtResources );
				ReturnVal = ws.getHeaderMenu();
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = error( ex );
			}
			//return ( ReturnVal );
			XmlDocument Doc = new XmlDocument();
			Doc.LoadXml( ReturnVal );
			return Doc;
		} // getHeaderMenu()		[WebMethod( EnableSession = true )]

		[WebMethod( EnableSession = true )]
		public XmlDocument getMainMenu( Int32 ViewId, string NodePk )
		{
			string ReturnVal = string.Empty;
			try
			{
				start();
				var ws = new CswNbtWebServiceMainMenu( _CswNbtResources );
				ReturnVal = ws.getMenu(ViewId, NodePk);
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = error( ex );
			}
			//return ( ReturnVal );
			XmlDocument Doc = new XmlDocument();
			Doc.LoadXml( ReturnVal );
			return Doc;
		} // getMainMenu()

		[WebMethod( EnableSession = true )]
		public XmlDocument getGridXml( Int32 ViewId )
		{
			var ReturnXml = new XmlDocument();
			try
			{
				start();
				CswNbtView View = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
				if( null != View )
				{
					var g = new CswNbtWebServiceGrid( _CswNbtResources, View);
					string XDocString = g.getGrid( CswNbtWebServiceGrid.GridReturnType.Xml );
					ReturnXml.LoadXml( XDocString );
					addToQuickLaunch( View );
				}
				end();
			}
			catch( Exception ex )
			{
				ReturnXml.LoadXml( error( ex ) );
			}

			return ReturnXml;
		} // getGrid()

		[WebMethod( EnableSession = true )]
		public string getGridJson( Int32 ViewId )
		{
			var ReturnJSON = string.Empty;
			try
			{
				start();
				CswNbtView View = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
				if( null != View )
				{
					var g = new CswNbtWebServiceGrid( _CswNbtResources, View );
					ReturnJSON = g.getGrid( CswNbtWebServiceGrid.GridReturnType.Json );
					addToQuickLaunch( View );
				}
				end();
			}
			catch( Exception ex )
			{
				ReturnJSON = ( error( ex ) );
			}

			return ReturnJSON;
		}

		[WebMethod( EnableSession = true )]
		public XmlDocument getTree( Int32 ViewId, string IDPrefix )
		{
			var XmlString = string.Empty;
			var ReturnXml = new XmlDocument();
			try
			{
				start();
				CswNbtView View = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
				if( null != View )
				{
					var ws = new CswNbtWebServiceTree( _CswNbtResources );
					XmlString = ws.getTree( View, IDPrefix );
					ReturnXml.LoadXml( XmlString );
					addToQuickLaunch( View );
				}
				end();
			}
			catch( Exception ex )
			{
				ReturnXml.LoadXml( error( ex ) );
			}
			
			return ReturnXml;
		} // getTree()


		[WebMethod( EnableSession = true )]
		public XmlDocument getTabs( string EditMode, string NodePk, string NodeTypeId )
		{
			string ReturnVal = string.Empty;
			try
			{
				start();
				var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
				CswNbtWebServiceTabsAndProps.NodeEditMode RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse( typeof( CswNbtWebServiceTabsAndProps.NodeEditMode ), EditMode );
				ReturnVal = ws.getTabs( RealEditMode, NodePk, CswConvert.ToInt32( NodeTypeId ) );
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = error( ex );
			}
			//return ( ReturnVal );
			XmlDocument Doc = new XmlDocument();
			Doc.LoadXml( ReturnVal );
			return Doc;
		} // getTabs()

		[WebMethod( EnableSession = true )]
		public XmlDocument getProps( string EditMode, string NodePk, string TabId, string NodeTypeId )
		{
			string ReturnVal = string.Empty;
			try
			{
				start();
				var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
				CswNbtWebServiceTabsAndProps.NodeEditMode RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse( typeof( CswNbtWebServiceTabsAndProps.NodeEditMode ), EditMode );
				ReturnVal = ws.getProps( RealEditMode, NodePk, TabId, CswConvert.ToInt32( NodeTypeId ) );
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = error( ex );
			}
			//return ( ReturnVal );
			XmlDocument Doc = new XmlDocument();
			Doc.LoadXml( ReturnVal );
			return Doc;
		} // getProps()

		[WebMethod( EnableSession = true )]
		public string saveProps( string EditMode, string NodePk, string NewPropsXml, string NodeTypeId )
		{
			string ReturnVal = string.Empty;
			try
			{
				start();
				var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
				CswNbtWebServiceTabsAndProps.NodeEditMode RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse( typeof( CswNbtWebServiceTabsAndProps.NodeEditMode ), EditMode );
				ReturnVal = ws.saveProps( RealEditMode, NodePk, NewPropsXml, CswConvert.ToInt32( NodeTypeId ) );
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
