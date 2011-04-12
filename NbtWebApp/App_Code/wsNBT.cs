using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Web.Services;
using System.Web.Script.Services;   // supports ScriptService attribute
using ChemSW.Core;
using ChemSW.Config;
using ChemSW.Nbt.Security;
using ChemSW.Security;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Statistics;
using ChemSW.Exceptions;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Formatting = Newtonsoft.Json.Formatting;
using System.Xml.Linq;
using System.Collections.Generic;
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
		private CswNbtStatisticsEvents _CswNbtStatisticsEvents;

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
			_CswNbtStatisticsEvents = _SessionResources.CswNbtStatisticsEvents;

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
			return ex.Message;
		}

		/// <summary>
		/// Returns error as XElement
		/// </summary>
		private XElement xError( Exception ex )
		{
			return ( new XElement( "error" ) { Value = "Error: " + error( ex ) } );
		}

		/// <summary>
		/// Returns error as JProperty
		/// </summary>
		private JProperty jError( Exception ex )
		{
			return ( new JProperty( "error" ) { Value = "Error: " + error( ex ) } );
		}

		//never used
		//private string result( string ReturnVal )
		//{
		//    return "<result>" + ReturnVal + "</result>";
		//}

		#endregion Session and Resource Management

		#region Web Methods

		private static readonly string _IDPrefix = string.Empty;

		#region Background Tasks

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string authenticate( string AccessId, string UserName, string Password )
		{
			JObject ReturnVal = new JObject();
			try
			{
				start();
				try
				{
					_SessionResources.CswSessionManager.setAccessId( AccessId );
				}
				catch( CswDniException ex )
				{
					if( !ex.Message.Contains( "There is no configuration information for this AccessId" ) )
						throw ex;
				}

				AuthenticationStatus AuthenticationStatus = _SessionResources.CswSessionManager.Authenticate( UserName, Password, CswWebControls.CswNbtWebTools.getIpAddress() );

				// case 21211
				if( AuthenticationStatus == AuthenticationStatus.Authenticated )
				{
					CswLicenseManager LicenseManager = new CswLicenseManager( _CswNbtResources );
					Int32 PasswordExpiryDays = CswConvert.ToInt32( _CswNbtResources.getConfigVariableValue( "passwordexpiry_days" ) );

					if( _CswNbtResources.CurrentNbtUser.PasswordProperty.ChangedDate == DateTime.MinValue ||
						_CswNbtResources.CurrentNbtUser.PasswordProperty.ChangedDate.AddDays( PasswordExpiryDays ).Date <= DateTime.Now.Date )
					{
						// BZ 9077 - Password expired
						AuthenticationStatus = AuthenticationStatus.ExpiredPassword;
						ReturnVal.Add( new JProperty( "nodeid", _CswNbtResources.CurrentNbtUser.UserNode.NodeId.ToString() ) );
						CswNbtNodeKey FakeKey = new CswNbtNodeKey( _CswNbtResources );
						FakeKey.NodeId = _CswNbtResources.CurrentNbtUser.UserNode.NodeId;
						FakeKey.NodeSpecies = _CswNbtResources.CurrentNbtUser.UserNode.Node.NodeSpecies;
						FakeKey.NodeTypeId = _CswNbtResources.CurrentNbtUser.UserNode.NodeTypeId;
						FakeKey.ObjectClassId = _CswNbtResources.CurrentNbtUser.UserNode.ObjectClass.ObjectClassId;
						ReturnVal.Add( new JProperty( "cswnbtnodekey", wsTools.ToSafeJavaScriptParam( FakeKey.ToString() ) ) );
						ReturnVal.Add( new JProperty( "passwordpropid", CswNbtWebServiceTabsAndProps.makePropIdAttribute( _CswNbtResources.CurrentNbtUser.UserNode.Node, _CswNbtResources.CurrentNbtUser.PasswordProperty.NodeTypeProp ) ) );
					}
					else if( LicenseManager.MustShowLicense( _CswNbtResources.CurrentUser ) )
					{
						// BZ 8133 - make sure they've seen the License
						AuthenticationStatus = AuthenticationStatus.ShowLicense;
					}
				}
				ReturnVal.Add( new JProperty( "AuthenticationStatus", AuthenticationStatus.ToString() ) );
				end();
			}
			catch( Exception ex )
			{
				ReturnVal.Add( jError( ex ) );
			}

			return ( ReturnVal.ToString() );
		}//authenticate()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string deauthenticate()
		{
			JObject ReturnVal = new JObject();
			try
			{
				start();
				_SessionResources.CswSessionManager.DeAuthenticate();
				ReturnVal.Add( new JProperty( "Deauthentication", "Succeeded" ) );
				end();
			}
			catch( Exception ex )
			{
				ReturnVal.Add( jError( ex ) );
			}
			return ( ReturnVal.ToString() );
		}//deAuthenticate()

		#endregion

		#region Render Core UI

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getWelcomeItems( string RoleId )
		{
			var ReturnVal = new XElement( "welcome" );
			try
			{
				start();

				var ws = new CswNbtWebServiceWelcomeItems( _CswNbtResources );
				// Only administrators can get welcome content for other roles
				if( RoleId != string.Empty && _CswNbtResources.CurrentNbtUser.IsAdministrator() )
				{
					ReturnVal = XElement.Parse( ws.GetWelcomeItems( RoleId ) );
				}
				else
				{
					ReturnVal = XElement.Parse( ws.GetWelcomeItems( _CswNbtResources.CurrentNbtUser.RoleId.ToString() ) );
				}

				end();
			}
			catch( Exception ex )
			{
				ReturnVal = xError( ex );
			}
			return ReturnVal;
		} // getWelcomeItems()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getQuickLaunchItems()
		{
			var QuickLaunchItems = new XElement( "quicklaunch" );
			try
			{
				start();

				CswPrimaryKey UserId = _CswNbtResources.CurrentNbtUser.UserId;
				var ws = new CswNbtWebServiceQuickLaunchItems( _CswNbtResources, Session );
				if( null != UserId )
				{
					QuickLaunchItems.Add( ws.getQuickLaunchItems( UserId ) );
				}

				end();
			}
			catch( Exception ex )
			{
				QuickLaunchItems = xError( ex );
			}

			return QuickLaunchItems;
		} // getQuickLaunchItems()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getViewTree()
		{
			var ReturnVal = new XElement( "viewtree" );
			try
			{
				start();
				var ws = new CswNbtWebServiceView( _CswNbtResources );
				ReturnVal = XElement.Parse( ws.getViewTree( Session ) );
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = xError( ex );
			}
			return ReturnVal;
		} // getViews()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getDashboard()
		{
			var ReturnVal = new XElement( "dashboard" );
			try
			{
				start();
				var ws = new CswNbtWebServiceHeader( _CswNbtResources );
				ReturnVal = XElement.Parse( ws.getDashboard() );
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = xError( ex );
			}
			return ReturnVal;
		} // getDashboard()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getHeaderMenu()
		{
			var ReturnVal = new XElement( "header" );
			try
			{
				start();
				var ws = new CswNbtWebServiceHeader( _CswNbtResources );
				ReturnVal = XElement.Parse( ws.getHeaderMenu() );
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = xError( ex );
			}
			return ReturnVal;
		} // getHeaderMenu()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getMainMenu( string ViewNum, string SafeNodeKey )
		{
			var ReturnNode = new XElement( "menu" );
			try
			{
				start();
				var ws = new CswNbtWebServiceMainMenu( _CswNbtResources );
				Int32 ViewId = CswConvert.ToInt32( ViewNum );
				if( Int32.MinValue != ViewId || !string.IsNullOrEmpty( SafeNodeKey ) )
				{
					ReturnNode = ws.getMenu( ViewId, SafeNodeKey );
				}
				end();
			}
			catch( Exception ex )
			{
				ReturnNode = xError( ex );
			}
			return ReturnNode;
		} // getMainMenu()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string getGrid( string ViewPk, string SafeNodeKey )
		{
			var ReturnJson = new JObject();
			string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );

			try
			{
				start();
				Int32 ViewId = CswConvert.ToInt32( ViewPk );
				if( Int32.MinValue != ViewId )
				{
					CswNbtView View = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
					if( null != View )
					{
						CswNbtNodeKey ParentNodeKey = null;
						if( !string.IsNullOrEmpty( ParsedNodeKey ) )
						{
							ParentNodeKey = new CswNbtNodeKey( _CswNbtResources, ParsedNodeKey );
						}
						var g = new CswNbtWebServiceGrid( _CswNbtResources, View, ParentNodeKey );
						ReturnJson = g.getGrid();
						CswNbtWebServiceQuickLaunchItems.addToQuickLaunch( View, Session );
					}
				}
				end();
			}
			catch( Exception Ex )
			{
				ReturnJson.Add( jError( Ex ) );
			}

			return ReturnJson.ToString();
		} // getGrid()

		#region Views

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string getViewGrid( bool All )
		{
			JObject ReturnJson = new JObject();
			try
			{
				start();
				CswNbtWebServiceView ws = new CswNbtWebServiceView( _CswNbtResources );
				ReturnJson = ws.getViewGrid( All );
				end();
			}
			catch( Exception Ex )
			{
				ReturnJson.Add( jError( Ex ) );
			}

			return ReturnJson.ToString();
		} // getViewGrid()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XmlDocument getViewInfo( string ViewId )
		{
			XmlDocument ReturnXml = null;
			try
			{
				start();
				Int32 nViewId = CswConvert.ToInt32( ViewId );
				if( nViewId != Int32.MinValue )
				{
					CswNbtView View = CswNbtViewFactory.restoreView( _CswNbtResources, nViewId );
					ReturnXml = View.ToXml();
				}
				end();
			}
			catch( Exception ex )
			{
				ReturnXml = new XmlDocument();
				ReturnXml.LoadXml( "<error>" + error( ex ) + "</error>" );
			}

			return ReturnXml;
		} // getViewInfo()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement saveViewInfo( string ViewId, string ViewXml )
		{
			XElement ReturnXml = new XElement("result");
			try
			{
				start();
				Int32 nViewId = CswConvert.ToInt32( ViewId );
				if( nViewId != Int32.MinValue )
				{
					CswNbtView View = CswNbtViewFactory.restoreView( _CswNbtResources, nViewId );
					View.LoadXml( ViewXml );
					View.save();

					//if( View.Visibility != NbtViewVisibility.Property )
					//    CswViewListTree.ClearCache( Session );
					_CswNbtResources.ViewCache.clearFromCache( View );
					_CswNbtResources.ViewSelect.clearCache();

					ReturnXml.Add(new XAttribute("succeeded", "true"));
				}
				end();
			}
			catch( Exception ex )
			{
				ReturnXml.Add( xError( ex ) );
			}

			return ReturnXml;
		} // saveViewInfo()


		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string copyView( string ViewId )
		{
			JObject ReturnJson = new JObject();
			try
			{
				start();
				Int32 nViewId = CswConvert.ToInt32( ViewId );
				if( nViewId != Int32.MinValue )
				{
					CswNbtView SourceView = CswNbtViewFactory.restoreView( _CswNbtResources, nViewId );
					CswNbtView NewView = new CswNbtView( _CswNbtResources );
					string NewViewNameOrig = SourceView.ViewName;
					string Suffix = " Copy";
					if( !NewViewNameOrig.EndsWith( Suffix) && NewViewNameOrig.Length < (CswNbtView.ViewNameLength - Suffix.Length - 2 ))
						NewViewNameOrig = NewViewNameOrig + Suffix;
					string NewViewName = NewViewNameOrig;
					if( NewViewNameOrig.Length > ( CswNbtView.ViewNameLength - 2 ) )
						NewViewNameOrig = NewViewNameOrig.Substring( 0, ( CswNbtView.ViewNameLength - 2 ) );
					Int32 Increment = 1;
					while(!CswNbtView.ViewIsUnique(_CswNbtResources, Int32.MinValue, NewViewName, SourceView.Visibility, SourceView.VisibilityUserId, SourceView.VisibilityRoleId))
					{
						Increment++;
						NewViewName = NewViewNameOrig + " " + Increment.ToString();
					}

					NewView.makeNew( NewViewName, SourceView.Visibility, SourceView.VisibilityRoleId, SourceView.VisibilityUserId, SourceView );
					NewView.save();
					ReturnJson.Add( new JProperty( "copyviewid", NewView.ViewId.ToString() ) );
				}
				end();
			}
			catch( Exception Ex )
			{
				ReturnJson.Add( jError( Ex ) );
			}

			return ReturnJson.ToString();
		} // copyView()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string deleteView( string ViewId )
		{
			JObject ReturnJson = new JObject();
			try
			{
				start();
				Int32 nViewId = CswConvert.ToInt32( ViewId );
				if( nViewId != Int32.MinValue )
				{
					CswNbtView DoomedView = CswNbtViewFactory.restoreView( _CswNbtResources, nViewId );
					DoomedView.Delete();
					ReturnJson.Add( new JProperty( "succeeded", "true" ) );
				}
				end();
			}
			catch( Exception Ex )
			{
				ReturnJson.Add( jError( Ex ) );
			}

			return ReturnJson.ToString();
		} // deleteView()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string createView( string ViewName, string ViewMode, string Visibility, string VisibilityRoleId, string VisibilityUserId )
		{
			JObject ReturnJson = new JObject();
			try
			{
				start();

				NbtViewRenderingMode RealViewMode = NbtViewRenderingMode.Unknown;
				Enum.TryParse<NbtViewRenderingMode>( ViewMode, out RealViewMode );
				NbtViewVisibility RealVisibility = NbtViewVisibility.Unknown;
				Enum.TryParse<NbtViewVisibility>( ViewMode, out RealVisibility );
				CswPrimaryKey RealVisibilityRoleId = null;
				CswPrimaryKey RealVisibilityUserId = null;
				if( RealVisibility == NbtViewVisibility.Role )
				{
					RealVisibilityRoleId = new CswPrimaryKey();
					RealVisibilityRoleId.FromString( VisibilityRoleId );
				}
				else if( RealVisibility == NbtViewVisibility.User )
				{
					RealVisibilityUserId = new CswPrimaryKey();
					RealVisibilityUserId.FromString( VisibilityUserId );
				}

				CswNbtView NewView = new CswNbtView( _CswNbtResources );
				NewView.makeNew( ViewName, RealVisibility, RealVisibilityRoleId, RealVisibilityUserId, null );
				NewView.ViewMode = RealViewMode;
				NewView.save();
				ReturnJson.Add( new JProperty( "newviewid", NewView.ViewId ) );
				end();
			}
			catch( Exception Ex )
			{
				ReturnJson.Add( jError( Ex ) );
			}

			return ReturnJson.ToString();
		} // createView()

		#endregion Views

		/// <summary>
		/// Generates a tree of nodes from the view
		/// </summary>
		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getTreeOfView( string ViewNum, string IDPrefix, bool IsFirstLoad, string ParentNodeKey, string IncludeNodeKey, bool IncludeNodeRequired, bool UsePaging )
		{
			var TreeNode = new XElement( "tree" );

			try
			{
				start();
				Int32 ViewId = CswConvert.ToInt32( ViewNum );
				if( Int32.MinValue != ViewId )
				{
					CswNbtView View = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
					var ws = new CswNbtWebServiceTree( _CswNbtResources );

					CswNbtNodeKey RealParentNodeKey = null;
					if( !string.IsNullOrEmpty( ParentNodeKey ) )
						RealParentNodeKey = new CswNbtNodeKey( _CswNbtResources, wsTools.FromSafeJavaScriptParam( ParentNodeKey ) );

					CswNbtNodeKey RealIncludeNodeKey = null;
					if( !string.IsNullOrEmpty( IncludeNodeKey ) )
						RealIncludeNodeKey = new CswNbtNodeKey( _CswNbtResources, wsTools.FromSafeJavaScriptParam( IncludeNodeKey ) );

					TreeNode = ws.getTree( View, IDPrefix, IsFirstLoad, RealParentNodeKey, RealIncludeNodeKey, IncludeNodeRequired, UsePaging );

					CswNbtWebServiceQuickLaunchItems.addToQuickLaunch( View, Session );
				}
				end();
			}
			catch( Exception ex )
			{
				TreeNode = xError( ex );
			}

			return TreeNode;
		} // getTreeOfView()

		/// <summary>
		/// Generates a tree of nodes from the view
		/// </summary>
		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getTreeOfNode( string IDPrefix, string NodePk )
		{
			var TreeNode = new XElement( "tree" );

			try
			{
				start();

				if( string.Empty != NodePk )
				{
					CswPrimaryKey NodeId = new CswPrimaryKey();
					NodeId.FromString( NodePk );
					CswNbtNode Node = _CswNbtResources.Nodes[NodeId];
					CswNbtView View = Node.NodeType.CreateDefaultView();
					View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( NodeId );

					var ws = new CswNbtWebServiceTree( _CswNbtResources );
					TreeNode = ws.getTree( View, IDPrefix, true, null, null, false, false );
					CswNbtWebServiceQuickLaunchItems.addToQuickLaunch( View, Session );
				}
				end();
			}
			catch( Exception ex )
			{
				TreeNode = xError( ex );
			}

			return TreeNode;
		} // getTreeOfNode()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getNodes( string NodeTypeId, string ObjectClassId, string ObjectClass )
		{
			var ResultXml = new XElement( "nodes" );

			try
			{
				start();

				Int32 RealNodeTypeId = CswConvert.ToInt32( NodeTypeId );
				Int32 RealObjectClassId = CswConvert.ToInt32( ObjectClassId );
				CswNbtMetaDataObjectClass.NbtObjectClass RealObjectClass = CswNbtMetaDataObjectClass.NbtObjectClass.Unknown;
				Enum.TryParse<CswNbtMetaDataObjectClass.NbtObjectClass>( ObjectClass, true, out RealObjectClass );

				Collection<CswNbtNode> Nodes = null;
				if( RealNodeTypeId != Int32.MinValue )
				{
					CswNbtMetaDataNodeType MetaDataNodeType = _CswNbtResources.MetaData.getNodeType( RealNodeTypeId );
					Nodes = MetaDataNodeType.getNodes( true, false );
				}
				else
				{
					CswNbtMetaDataObjectClass MetaDataObjectClass = null;
					if( RealObjectClassId != Int32.MinValue )
						MetaDataObjectClass = _CswNbtResources.MetaData.getObjectClass( RealObjectClassId );
					else if( RealObjectClass != CswNbtMetaDataObjectClass.NbtObjectClass.Unknown )
						MetaDataObjectClass = _CswNbtResources.MetaData.getObjectClass( RealObjectClass );
					Nodes = MetaDataObjectClass.getNodes( true, false );
				}

				foreach( CswNbtNode Node in Nodes )
				{
					ResultXml.Add(
						new XElement( "node",
							new XAttribute( "id", Node.NodeId.ToString() ),
							new XAttribute( "name", Node.NodeName ) ) );
				}

				end();
			}
			catch( Exception ex )
			{
				ResultXml = xError( ex );
			}

			return ResultXml;
		} // getNodes()

		#endregion Render Core UI

		#region Tabs and Props

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getTabs( string EditMode, string SafeNodeKey, string NodeTypeId )
		{
			var TabsNode = new XElement( "tabs" );
			try
			{
				start();
				string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );
				if( !string.IsNullOrEmpty( ParsedNodeKey ) || EditMode == "AddInPopup" )
				{
					var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
					var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse( typeof( CswNbtWebServiceTabsAndProps.NodeEditMode ), EditMode );
					TabsNode = ws.getTabs( RealEditMode, ParsedNodeKey, CswConvert.ToInt32( NodeTypeId ) );
				}
				end();
			}
			catch( Exception ex )
			{
				TabsNode = xError( ex );
			}

			return TabsNode;
		} // getTabs()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XmlDocument getProps( string EditMode, string SafeNodeKey, string TabId, string NodeTypeId )
		{
			XmlDocument ReturnXml = null;
			try
			{
				start();
				string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );
				if( !string.IsNullOrEmpty( ParsedNodeKey ) || EditMode == "AddInPopup" )
				{
					var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
					var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse( typeof( CswNbtWebServiceTabsAndProps.NodeEditMode ), EditMode );
					ReturnXml = ws.getProps( RealEditMode, ParsedNodeKey, TabId, CswConvert.ToInt32( NodeTypeId ) );
				}
				end();
			}
			catch( Exception ex )
			{
				ReturnXml = new XmlDocument();
				ReturnXml.LoadXml( "<error>" + error( ex ) + "</error>" );
			}
			return ReturnXml;
		} // getProps()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XmlDocument getSingleProp( string EditMode, string SafeNodeKey, string PropId, string NodeTypeId, string NewPropXml )
		{
			XmlDocument ReturnXml = null;
			try
			{
				start();
				string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );
				if( !string.IsNullOrEmpty( ParsedNodeKey ) )
				{
					var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
					var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse( typeof( CswNbtWebServiceTabsAndProps.NodeEditMode ), EditMode );
					ReturnXml = ws.getSingleProp( RealEditMode, ParsedNodeKey, PropId, CswConvert.ToInt32( NodeTypeId ),
												 NewPropXml );
				}
				end();
			}
			catch( Exception ex )
			{
				ReturnXml = new XmlDocument();
				ReturnXml.LoadXml( "<error>" + error( ex ) + "</error>" );
			}
			return ReturnXml;
		} // getProps()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string saveProps( string EditMode, string SafeNodeKey, string NewPropsXml, string NodeTypeId, string ViewId )
		{
			JObject ReturnVal = new JObject();
			try
			{
				start();
				string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );
				//if( !string.IsNullOrEmpty( ParsedNodeKey ) )
				//{
				var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
				var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse( typeof( CswNbtWebServiceTabsAndProps.NodeEditMode ), EditMode );
				ReturnVal = ws.saveProps( RealEditMode, ParsedNodeKey, NewPropsXml, CswConvert.ToInt32( NodeTypeId ), CswConvert.ToInt32( ViewId ) );
				//}
				end();
			}
			catch( Exception ex )
			{
				ReturnVal.Add( jError( ex ) );
			}
			return ( ReturnVal.ToString() );
		} // saveProps()


		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string copyPropValues( string SourceNodeKey, string[] CopyNodeIds, string[] PropIds )
		{
			JObject ReturnVal = new JObject();
			try
			{
				start();
				string ParsedSourceNodeKey = wsTools.FromSafeJavaScriptParam( SourceNodeKey );
				var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
				bool ret = ws.copyPropValues( ParsedSourceNodeKey, CopyNodeIds, PropIds );
				ReturnVal.Add( new JProperty( "succeeded", ret.ToString().ToLower() ) );
				end();
			}
			catch( Exception ex )
			{
				ReturnVal.Add( jError( ex ) );
			}
			return ( ReturnVal.ToString() );
		} // copyPropValue()	


		#endregion Tabs and Props

		#region Misc

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XmlDocument getAbout()
		{
			string ReturnVal = string.Empty;
			try
			{
				start();
				var ws = new CswNbtWebServiceHeader( _CswNbtResources );
				ReturnVal = ws.makeVersionXml();
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = "<error>" + error( ex ) + "</error>";
			}
			XmlDocument Doc = new XmlDocument();
			Doc.LoadXml( ReturnVal.Replace( "&", "&amp;" ) );
			return Doc;
		} // getAbout()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getNodeTypes()
		{
			XElement ReturnVal = new XElement( "nodetypes" );
			try
			{
				start();
				var ws = new CswNbtWebServiceMetaData( _CswNbtResources );
				ReturnVal = ws.getNodeTypes();
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = xError( ex );
			}
			return ( ReturnVal );
		} // getNodeTypes()


		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string getLicense()
		{
			JObject ReturnVal = new JObject();
			try
			{
				start();
				CswLicenseManager LicenseManager = new CswLicenseManager( _CswNbtResources );
				ReturnVal.Add( new JProperty( "license", LicenseManager.LatestLicenseText ) );
				end();
			}
			catch( Exception ex )
			{
				ReturnVal.Add( jError( ex ) );
			}
			return ( ReturnVal.ToString() );
		}

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string acceptLicense()
		{
			JObject ReturnVal = new JObject();
			try
			{
				start();
				CswLicenseManager LicenseManager = new CswLicenseManager( _CswNbtResources );
				LicenseManager.RecordLicenseAcceptance( _CswNbtResources.CurrentUser );
				ReturnVal.Add( new JProperty( "result", "succeeded" ) );
				end();
			}
			catch( Exception ex )
			{
				ReturnVal.Add( jError( ex ) );
			}
			return ( ReturnVal.ToString() );
		}

		#endregion Misc

		#region Search

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getClientSearchXml( string ViewIdNum, string SelectedNodeTypeIdNum )
		{
			XElement SearchNode = new XElement( "search" );
			try
			{
				start();
				var ws = new CswNbtWebServiceSearch( _CswNbtResources );
				SearchNode = ws.getSearchXml( ViewIdNum, SelectedNodeTypeIdNum );
				end();
			}
			catch( Exception ex )
			{
				SearchNode = xError( ex );
			}
			return SearchNode;
		}

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getNodeTypeSearchProps( string RelatedIdType, string ObjectPk )
		{
			XElement SearchNode = new XElement( "search" );
			try
			{
				start();
				var ws = new CswNbtWebServiceSearch( _CswNbtResources );
				SearchNode = ( ws.getNodeTypeProps( RelatedIdType, ObjectPk ) );
				end();
			}
			catch( Exception ex )
			{
				SearchNode = xError( ex );
			}

			return SearchNode;
		} // getSearch()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getSearchableViews( string IsMobile, string OrderBy )
		{
			var SearchNode = new XElement( "searchableviews" );
			try
			{
				start();

				ICswNbtUser UserId = _CswNbtResources.CurrentNbtUser;
				bool ForMobile = CswConvert.ToBoolean( IsMobile );
				XElement Views = _CswNbtResources.ViewSelect.getSearchableViews( UserId, ForMobile, OrderBy ); ;
				SearchNode.Add( Views );
				end();
			}
			catch( Exception ex )
			{
				SearchNode = xError( ex );
			}

			return SearchNode;
		} // getSearch()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string doViewSearch( string SearchJson )
		{
			JObject SearchResultView = new JObject();
			try
			{
				start();

				var ws = new CswNbtWebServiceSearch( _CswNbtResources );
				CswNbtView ResultsView = ws.doViewBasedSearch( SearchJson );
				ResultsView.SaveToCache();

				SearchResultView.Add( new JProperty( "sessionviewid", ResultsView.SessionViewId.ToString() ) );
				SearchResultView.Add( new JProperty( "viewmode", ResultsView.ViewMode.ToString().ToLower() ) );
				end();
			}
			catch( Exception ex )
			{
				SearchResultView.Add( jError( ex ) );
			}

			return SearchResultView.ToString();
		} // getSearch()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string doNodeTypeSearch( string SearchJson )
		{
			JObject SessionViewId = new JObject();
			try
			{
				start();

				var ws = new CswNbtWebServiceSearch( _CswNbtResources );
				CswNbtView ResultsView = ws.doNodesSearch( SearchJson );
				ResultsView.SaveToCache();
				//var RenderElement = getTreeOfView( ResultsView.SessionViewId.ToString(), _IDPrefix, string.Empty, string.Empty );
				SessionViewId.Add( new JProperty( "sessionviewid", ResultsView.SessionViewId.ToString() ) );
				SessionViewId.Add( new JProperty( "viewmode", ResultsView.ViewMode ) );
				end();
			}
			catch( Exception ex )
			{
				SessionViewId.Add( jError( ex ) );
			}
			return SessionViewId.ToString();
		}

		#endregion Search

		#region Node DML
		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string DeleteNodes( string[] NodePks )
		{
			var ReturnVal = new JObject();
			bool ret = true;
			try
			{
				start();
				foreach( string NodePk in NodePks )
				{
					CswPrimaryKey RealNodePk = new CswPrimaryKey();
					RealNodePk.FromString( NodePk );
					if( RealNodePk.PrimaryKey != Int32.MinValue )
					{
						CswNbtWebServiceNode ws = new CswNbtWebServiceNode( _CswNbtResources, _CswNbtStatisticsEvents );
						ret = ret && ws.DeleteNode( RealNodePk );
					}
				}
				ReturnVal.Add( new JProperty( "Succeeded", ret.ToString().ToLower() ) );
				end();
			}
			catch( Exception ex )
			{
				ReturnVal.Add( jError( ex ) );
			}
			return ( ReturnVal.ToString() );
		}


		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string CopyNode( string NodePk )
		{
			var ReturnVal = new JObject();
			try
			{
				start();
				CswPrimaryKey RealNodePk = new CswPrimaryKey();
				RealNodePk.FromString( NodePk );
				if( RealNodePk.PrimaryKey != Int32.MinValue )
				{
					CswNbtWebServiceNode ws = new CswNbtWebServiceNode( _CswNbtResources, _CswNbtStatisticsEvents );
					CswPrimaryKey NewNodePk = ws.CopyNode( RealNodePk );
					if( NewNodePk != null )
					{
						ReturnVal.Add( new JProperty( "NewNodeId", NewNodePk.ToString() ) );
					}
					else
					{
						ReturnVal.Add( new JProperty( "NewNodeId", "" ) );
					}
				}
				end();
			}
			catch( Exception ex )
			{
				ReturnVal.Add( jError( ex ) );
			}
			return ( ReturnVal.ToString() );
		}


		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string MoveProp( string PropId, string NewRow, string NewColumn, string EditMode )
		{
			var ReturnVal = new JObject();
			try
			{
				start();
				var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
				var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse( typeof( CswNbtWebServiceTabsAndProps.NodeEditMode ), EditMode );
				bool ret = ws.moveProp( PropId, CswConvert.ToInt32( NewRow ), CswConvert.ToInt32( NewColumn ), RealEditMode );
				ReturnVal.Add( new JProperty( "moveprop", ret.ToString().ToLower() ) );
				end();
			}
			catch( Exception ex )
			{
				ReturnVal.Add( jError( ex ) );
			}
			return ( ReturnVal.ToString() );
		} // MoveProp()
		#endregion Node DML

		#region Welcome Region

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getWelcomeButtonIconList()
		{
			XElement ReturnVal = new XElement( "buttonicons" );
			try
			{
				start();
				var ws = new CswNbtWebServiceWelcomeItems( _CswNbtResources );
				ReturnVal = ws.getButtonIconList();
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = xError( ex );
			}
			return ( ReturnVal );
		} // getWelcomeButtonIconList()


		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string addWelcomeItem( string RoleId, string Type, string ViewId, string NodeTypeId, string Text, string IconFileName )
		{
			string ReturnVal = string.Empty;
			try
			{
				start();
				CswNbtWebServiceWelcomeItems ws = new CswNbtWebServiceWelcomeItems( _CswNbtResources );
				// Only administrators can add welcome content to other roles
				string UseRoleId = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
				if( RoleId != string.Empty && _CswNbtResources.CurrentNbtUser.IsAdministrator() )
					UseRoleId = RoleId;
				CswNbtWebServiceWelcomeItems.WelcomeComponentType ComponentType = (CswNbtWebServiceWelcomeItems.WelcomeComponentType) Enum.Parse( typeof( CswNbtWebServiceWelcomeItems.WelcomeComponentType ), Type );
				ws.AddWelcomeItem( ComponentType, NbtWebControls.CswViewListTree.ViewType.View, CswConvert.ToInt32( ViewId ), CswConvert.ToInt32( NodeTypeId ), Text, Int32.MinValue, Int32.MinValue, IconFileName, UseRoleId );
				ReturnVal = "{ \"Succeeded\": \"true\" }";
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = error( ex );
			}
			return ( ReturnVal );
		} // addWelcomeItem()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string deleteWelcomeItem( string RoleId, string WelcomeId )
		{
			bool ret = false;
			string ReturnVal = string.Empty;
			try
			{
				start();
				CswNbtWebServiceWelcomeItems ws = new CswNbtWebServiceWelcomeItems( _CswNbtResources );
				// Only administrators can add welcome content to other roles
				string UseRoleId = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
				if( RoleId != string.Empty && _CswNbtResources.CurrentNbtUser.IsAdministrator() )
					UseRoleId = RoleId;
				ret = ws.DeleteWelcomeItem( UseRoleId, CswConvert.ToInt32( WelcomeId ) );
				ReturnVal = "{ \"Succeeded\": \"" + ret.ToString().ToLower() + "\" }";
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = error( ex );
			}
			return ( ReturnVal );
		} // deleteWelcomeItem()



		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string moveWelcomeItems( string RoleId, string WelcomeId, string NewRow, string NewColumn )
		{
			bool ret = false;
			string ReturnVal = string.Empty;
			try
			{
				start();
				CswNbtWebServiceWelcomeItems ws = new CswNbtWebServiceWelcomeItems( _CswNbtResources );
				// Only administrators can move welcome content for other roles
				string UseRoleId = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
				if( RoleId != string.Empty && _CswNbtResources.CurrentNbtUser.IsAdministrator() )
					UseRoleId = RoleId;
				ret = ws.MoveWelcomeItems( UseRoleId, CswConvert.ToInt32( WelcomeId ), CswConvert.ToInt32( NewRow ), CswConvert.ToInt32( NewColumn ) );
				ReturnVal = "{ \"Succeeded\": \"" + ret.ToString().ToLower() + "\" }";
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = error( ex );
			}
			return ( ReturnVal );
		} // moveWelcomeItems()

		#endregion Welcome Region

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string isAdministrator()
		{
			JObject ReturnVal = new JObject();
			try
			{
				start();
				ReturnVal.Add( new JProperty( "Administrator", _CswNbtResources.CurrentNbtUser.IsAdministrator().ToString().ToLower() ) );
				end();
			}
			catch( Exception ex )
			{
				ReturnVal.Add( jError( ex ) );
			}
			return ( ReturnVal.ToString() );
		} // isAdministrator()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string fileForProp()
		{
			JObject ReturnVal = new JObject( new JProperty( "success", false.ToString().ToLower() ) );
			try
			{
				start();

				// putting these in the param list causes the webservice to fail with
				// "System.InvalidOperationException: Request format is invalid: application/octet-stream"
				string FileName = Context.Request["qqfile"];
				string PropId = Context.Request["propid"];

				if( !string.IsNullOrEmpty( FileName ) && !string.IsNullOrEmpty( PropId ) )
				{
					// Unfortunately, Context.Request.ContentType is always application/octet-stream
					// So we have to detect the content type
					string[] SplitFileName = FileName.Split( '.' );
					string Extension = SplitFileName[SplitFileName.Length - 1];
					string ContentType = "application/" + Extension;
					switch( Extension )
					{
						case "jpg":
						case "jpeg":
							ContentType = "image/pjpeg";
							break;
						case "gif":
							ContentType = "image/gif";
							break;
						case "png":
							ContentType = "image/png";
							break;
					}

					if( Context.Request.InputStream != null )
					{
						// Read the binary data
						BinaryReader br = new BinaryReader( Context.Request.InputStream );
						long Length = Context.Request.InputStream.Length;
						byte[] FileData = new byte[Length];
						for( long CurrentIndex = 0; CurrentIndex < Length; CurrentIndex++ )
						{
							FileData[CurrentIndex] = br.ReadByte();
						}

						// Save the binary data
						CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
						bool ret = ws.SetPropBlobValue( FileData, FileName, ContentType, PropId );

						ReturnVal = new JObject( new JProperty( "success", ret.ToString().ToLower() ) );

					} // if( Context.Request.InputStream != null )
				} // if( FileName != string.Empty && PropId != string.Empty )

				end();
			}
			catch( Exception ex )
			{
				//ReturnVal.Add( jError( ex ) );
				ReturnVal = new JObject( new JProperty( "error", ex.Message ) );
			}
			return ( ReturnVal.ToString() );
		} // fileForProp()


		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string clearProp( string PropId, bool IncludeBlob )
		{
			JObject ReturnVal = new JObject( new JProperty( "Succeeded", false.ToString().ToLower() ) );
			try
			{
				start();

				CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
				bool ret = ws.ClearPropValue( PropId, IncludeBlob );
				ReturnVal = new JObject( new JProperty( "Succeeded", ret.ToString().ToLower() ) );

				end();
			}
			catch( Exception ex )
			{
				ReturnVal.Add( jError( ex ) );
			}
			return ( ReturnVal.ToString() );
		} // clearProp()

		#endregion Web Methods

	}//wsNBT

} // namespace ChemSW.WebServices
