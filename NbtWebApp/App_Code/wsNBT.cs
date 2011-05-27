﻿using System;
using System.Collections;
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
using ChemSW.NbtWebControls;
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
using ChemSW.Nbt;

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

		private CswSessionResourcesNbt _CswSessionResources;
		private CswNbtResources _CswNbtResources;
		private CswNbtStatisticsEvents _CswNbtStatisticsEvents;

		private string _FilesPath
		{
			get
			{
				return ( System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\etc" );
			}
		}

		private void _initResources()
		{
			_CswSessionResources = new CswSessionResourcesNbt( Context.Application, Context.Request, Context.Response, string.Empty, _FilesPath, SetupMode.Web );
			_CswNbtResources = _CswSessionResources.CswNbtResources;
			_CswNbtStatisticsEvents = _CswSessionResources.CswNbtStatisticsEvents;
		}//start() 

		private void _deInitResources()
		{
			_CswSessionResources.endSession();
			if( _CswNbtResources != null )
			{
				_CswNbtResources.finalize();
				_CswNbtResources.release();
			}
		}

		#endregion Session and Resource Management

		#region Error Handling

		private string error( Exception ex, out string Message, out string Detail )
		{
			_CswNbtResources.CswLogger.reportError( ex );
			_CswNbtResources.Rollback();

			if( ex is CswDniException )
			{
				Message = ( (CswDniException) ex ).MsgFriendly;
				Detail = ( (CswDniException) ex ).MsgEscoteric + "; " + ex.StackTrace;
			}
			else
			{
				Message = "An internal error occurred";
				Detail = ex.Message + "; " + ex.StackTrace;
			}
			return ex.Message;
		}

		/// <summary>
		/// Returns error as XElement
		/// </summary>
		private XElement _xError( Exception ex )
		{
			string Message = string.Empty;
			string Detail = string.Empty;
			error( ex, out Message, out Detail );

			return new XElement( "error",
				new XAttribute( "message", Message ),
				new XAttribute( "detail", Detail ) );
		}



		/*
		 * The two _xAddAuthenticationStatus() methods must _not_ add the authentication status as a element. 
		 * I tried that, and it turned out that in the JQuery world the code that displays an xml tree
		 * ends up seeing the authentication node even if it is a peer of the tree and not in the tree. 
		 * Please trust me: we're talking major whackadelia. But it works fine as an attribute. 
		 */
		private void _xAddAuthenticationStatus( XElement XElement, AuthenticationStatus AuthenticationStatusIn )
		{
			XElement.SetAttributeValue( "authenticationstatus", AuthenticationStatusIn.ToString() );
			if( _CswSessionResources != null && _CswSessionResources.CswSessionManager != null )
				XElement.SetAttributeValue( "timeout", _CswSessionResources.CswSessionManager.TimeoutDate.ToString() );
		}//_xAuthenticationStatus()


		private void _xAddAuthenticationStatus( XmlDocument XmlDocument, AuthenticationStatus AuthenticationStatusIn )
		{
			if( XmlDocument.DocumentElement == null )
				CswXmlDocument.SetDocumentElement( XmlDocument, "root" );
			CswXmlDocument.AppendXmlAttribute( XmlDocument.DocumentElement, "authenticationstatus", AuthenticationStatusIn.ToString() );
			if( _CswSessionResources != null && _CswSessionResources.CswSessionManager != null )
				CswXmlDocument.AppendXmlAttribute( XmlDocument.DocumentElement, "timeout", _CswSessionResources.CswSessionManager.TimeoutDate.ToString() );
		}//_xAuthenticationStatus()

		private void _jAddAuthenticationStatus( JObject JObj, AuthenticationStatus AuthenticationStatusIn )
		{
			JObj.Add( new JProperty( "AuthenticationStatus", AuthenticationStatusIn.ToString() ) );
			if( _CswSessionResources != null && _CswSessionResources.CswSessionManager != null )
				JObj.Add( new JProperty( "timeout", _CswSessionResources.CswSessionManager.TimeoutDate.ToString() ) );
		}//_jAuthenticationStatus()



		/// <summary>
		/// Returns error as XmlDocument
		/// </summary>
		private XmlDocument xmlError( Exception ex )
		{
			string Message = string.Empty;
			string Detail = string.Empty;
			error( ex, out Message, out Detail );

			XmlDocument ErrorXmlDoc = new XmlDocument();
			CswXmlDocument.SetDocumentElement( ErrorXmlDoc, "error" );
			CswXmlDocument.AppendXmlAttribute( ErrorXmlDoc.DocumentElement, "message", Message );
			CswXmlDocument.AppendXmlAttribute( ErrorXmlDoc.DocumentElement, "detail", Detail );
			return ErrorXmlDoc;
		}

		/// <summary>
		/// Returns error as JProperty
		/// </summary>
		private JObject jError( Exception ex )
		{
			string Message = string.Empty;
			string Detail = string.Empty;
			error( ex, out Message, out Detail );

			return new JObject(
				new JProperty( "error",
						new JObject(
							new JProperty( "message", Message ),
							new JProperty( "detail", Detail ) ) ) );
		}

		#endregion Error Handling

		#region Web Methods

		private static readonly string _IDPrefix = string.Empty;

		#region Authentication

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string authenticate( string AccessId, string UserName, string Password )
		{
			JObject ReturnVal = new JObject();
			try
			{
				_initResources();

				try
				{
					_CswSessionResources.CswSessionManager.setAccessId( AccessId );
				}
				catch( CswDniException ex )
				{
					if( !ex.Message.Contains( "There is no configuration information for this AccessId" ) )
						throw ex;
				}

				AuthenticationStatus AuthenticationStatus = _CswSessionResources.CswSessionManager.beginSession( UserName, Password, CswWebControls.CswNbtWebTools.getIpAddress() );

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
						CswPropIdAttr PasswordPropIdAttr = new CswPropIdAttr( _CswNbtResources.CurrentNbtUser.UserNode.Node, _CswNbtResources.CurrentNbtUser.PasswordProperty.NodeTypeProp );
						ReturnVal.Add( new JProperty( "passwordpropid", PasswordPropIdAttr.ToString() ) );
					}
					else if( LicenseManager.MustShowLicense( _CswNbtResources.CurrentUser ) )
					{
						// BZ 8133 - make sure they've seen the License
						AuthenticationStatus = AuthenticationStatus.ShowLicense;
					}

					// initial quick launch setup
					CswNbtWebServiceQuickLaunchItems wsQL = new CswNbtWebServiceQuickLaunchItems( _CswNbtResources );
					wsQL.initQuickLaunchItems();
				}

				_CswSessionResources.purgeExpiredSessions(); //bury the overhead of nuking old sessions in the overhead of authenticating

				_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );
				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}


			return ( ReturnVal.ToString() );
		}//authenticate()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string deauthenticate()
		{
			JObject ReturnVal = new JObject();

			try
			{
				_initResources();

				_CswSessionResources.CswSessionManager.clearSession();
				ReturnVal.Add( new JProperty( "Deauthentication", "Succeeded" ) );
				_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus.Deauthenticated );
				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			return ( ReturnVal.ToString() );

		}//deAuthenticate()

		#endregion Authentication

		#region Render Core UI


		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getQuickLaunchItems()
		{
			XElement ReturnVal = new XElement( "quicklaunch" );
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					CswPrimaryKey UserId = _CswNbtResources.CurrentNbtUser.UserId;
					var ws = new CswNbtWebServiceQuickLaunchItems( _CswNbtResources ); //, new CswWebClientStorageCookies( Context.Request, Context.Response ) ); // , Session );
					if( null != UserId )
					{
						ReturnVal.Add( ws.getQuickLaunchItems() );
					}
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}

			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;

		} // getQuickLaunchItems()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getViewTree( bool IsSearchable, bool UseSession )
		{
			XElement ReturnVal = new XElement( "viewtree" );
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					var ws = new CswNbtWebServiceView( _CswNbtResources );
					//ReturnVal = XElement.Parse( ws.getViewTree( Session, IsSearchable, UseSession ) );
					ReturnVal = XElement.Parse( ws.getViewTree( IsSearchable ) );
					_deInitResources();
				}
			}
			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}

			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;

		} // getViews()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getDashboard()
		{
			XElement ReturnVal = new XElement( "dashboard" );
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					var ws = new CswNbtWebServiceHeader( _CswNbtResources );
					ReturnVal = XElement.Parse( ws.getDashboard() );
				}

				_deInitResources();
			}

			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}

			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;


		} // getDashboard()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getHeaderMenu()
		{
			XElement ReturnVal = new XElement( "header" );
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					var ws = new CswNbtWebServiceHeader( _CswNbtResources );
					ReturnVal = XElement.Parse( ws.getHeaderMenu() );
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}

			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;

		} // getHeaderMenu()


		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getMainMenu( string ViewId, string SafeNodeKey )
		{
			XElement ReturnVal = new XElement( "menu" );
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					var ws = new CswNbtWebServiceMainMenu( _CswNbtResources );
					CswNbtView View = _getView( ViewId );
					ReturnVal = ws.getMenu( View, SafeNodeKey );
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}

			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;


		} // getMainMenu()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string getGrid( string ViewId, string SafeNodeKey, string ShowEmpty )
		{
			JObject ReturnVal = new JObject();
			string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );

			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					bool ShowEmptyGrid = CswConvert.ToBoolean( ShowEmpty );
					CswNbtView View = _getView( ViewId );
					if( null != View )
					{
						CswNbtNodeKey ParentNodeKey = null;
						if( !string.IsNullOrEmpty( ParsedNodeKey ) )
						{
							ParentNodeKey = new CswNbtNodeKey( _CswNbtResources, ParsedNodeKey );
						}
						var g = new CswNbtWebServiceGrid( _CswNbtResources, View, ParentNodeKey );
						ReturnVal = g.getGrid( ShowEmptyGrid );
						//CswNbtWebServiceQuickLaunchItems.addToQuickLaunch( View ); //, Session );
						View.SaveToCache( true );
					}
				}

				_deInitResources();
			}
			catch( Exception Ex )
			{
				ReturnVal = jError( Ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		} // getGrid()

		/// <summary>
		/// Generates a tree of nodes from the view
		/// </summary>
		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getTreeOfView( string ViewId, string IDPrefix, bool IsFirstLoad, string ParentNodeKey, string IncludeNodeKey, bool IncludeNodeRequired,
									   bool UsePaging, string ShowEmpty, bool ForSearch, bool IncludeInQuickLaunch )
		{
			XElement ReturnVal = new XElement( "tree" );

			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					bool ShowEmptyTree = CswConvert.ToBoolean( ShowEmpty );
					CswNbtView View = _getView( ViewId );
					if( null != View )
					{
						var ws = new CswNbtWebServiceTree( _CswNbtResources );

						CswNbtNodeKey RealParentNodeKey = null;
						if( !string.IsNullOrEmpty( ParentNodeKey ) )
							RealParentNodeKey = new CswNbtNodeKey( _CswNbtResources, wsTools.FromSafeJavaScriptParam( ParentNodeKey ) );

						CswNbtNodeKey RealIncludeNodeKey = null;
						if( !string.IsNullOrEmpty( IncludeNodeKey ) )
							RealIncludeNodeKey = new CswNbtNodeKey( _CswNbtResources, wsTools.FromSafeJavaScriptParam( IncludeNodeKey ) );

						ReturnVal = ws.getTree( View, IDPrefix, IsFirstLoad, RealParentNodeKey, RealIncludeNodeKey, IncludeNodeRequired, UsePaging, ShowEmptyTree, ForSearch, IncludeInQuickLaunch );

						//CswNbtWebServiceQuickLaunchItems.addToQuickLaunch( View ); //, Session );
						//View.SaveToCache(true);
					}
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}


			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;

		} // getTreeOfView()

		/// <summary>
		/// Generates a tree of nodes from the view
		/// </summary>
		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getTreeOfNode( string IDPrefix, string NodePk )
		{
			XElement ReturnVal = new XElement( "tree" );

			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					if( string.Empty != NodePk )
					{
						CswPrimaryKey NodeId = new CswPrimaryKey();
						NodeId.FromString( NodePk );
						CswNbtNode Node = _CswNbtResources.Nodes[NodeId];
						CswNbtView View = Node.NodeType.CreateDefaultView();
						View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( NodeId );

						var ws = new CswNbtWebServiceTree( _CswNbtResources );
						ReturnVal = ws.getTree( View, IDPrefix, true, null, null, false, false, false, false, true );
						//CswNbtWebServiceQuickLaunchItems.addToQuickLaunch( View ); //, Session );
						View.SaveToCache( true );
					}
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}


			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;


		} // getTreeOfNode()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getNodes( string NodeTypeId, string ObjectClassId, string ObjectClass )
		{
			XElement ReturnVal = new XElement( "nodes" );

			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

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
						ReturnVal.Add(
							new XElement( "node",
								new XAttribute( "id", Node.NodeId.ToString() ),
								new XAttribute( "name", Node.NodeName ) ) );
					}

				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}


			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;


		} // getNodes()

		#endregion Render Core UI

		#region View Editing

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string getViewGrid( bool All, string SelectedViewId )
		{
			JObject ReturnVal = new JObject();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();
				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					CswNbtWebServiceView ws = new CswNbtWebServiceView( _CswNbtResources );
					ReturnVal = ws.getViewGrid( All );

					// This translates CswNbtSessionDataIds into CswNbtViewIds for the client
					CswNbtView SelectedView = _getView( SelectedViewId );
					if( SelectedView != null && SelectedView.ViewId != null && SelectedView.ViewId.isSet() )
					{
						ReturnVal.Add( new JProperty( "selectedpk", SelectedView.ViewId.get().ToString() ) );
					}
				}
				_deInitResources();
			}
			catch( Exception Ex )
			{
				ReturnVal = jError( Ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus ); 
			
			return ReturnVal.ToString();

		} // getViewGrid()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XmlDocument getViewInfo( string ViewId )
		{
			XmlDocument ReturnVal = new XmlDocument();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					CswNbtView View = _getView( ViewId );
					if( null != View )
					{
						ReturnVal = View.ToXml();
					}

				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = xmlError( ex );
			}

			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );
			return ReturnVal;
		} // getViewInfo()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement saveViewInfo( string ViewId, string ViewXml )
		{
			XElement ReturnVal = new XElement( "result" );

			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					CswNbtView View = _getView( ViewId );
					if( null != View )
					{
						View.LoadXml( ViewXml );
						View.save();

						//if( View.Visibility != NbtViewVisibility.Property )
						//    CswViewListTree.ClearCache( Session );
						_CswNbtResources.ViewSelect.removeSessionView( View );
						_CswNbtResources.ViewSelect.clearCache();

						ReturnVal.Add( new XAttribute( "succeeded", "true" ) );
					}
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}


			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;


		} // saveViewInfo()



		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getViewChildOptions( string ViewXml, string ArbitraryId, string StepNo )
		{
			XElement ReturnVal = new XElement( "result" );

			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					CswNbtWebServiceView ws = new CswNbtWebServiceView( _CswNbtResources );
					ReturnVal = ws.getViewChildOptions( ViewXml, ArbitraryId, CswConvert.ToInt32( StepNo ) );
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}


			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;

		} // getViewChildOptions()


		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string copyView( string ViewId )
		{
			JObject ReturnVal = new JObject();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					CswNbtView SourceView = _getView( ViewId );
					if( null != SourceView )
					{
						CswNbtView NewView = new CswNbtView( _CswNbtResources );
						string NewViewNameOrig = SourceView.ViewName;
						string Suffix = " Copy";
						if( !NewViewNameOrig.EndsWith( Suffix ) && NewViewNameOrig.Length < ( CswNbtView.ViewNameLength - Suffix.Length - 2 ) )
							NewViewNameOrig = NewViewNameOrig + Suffix;
						string NewViewName = NewViewNameOrig;
						if( NewViewNameOrig.Length > ( CswNbtView.ViewNameLength - 2 ) )
							NewViewNameOrig = NewViewNameOrig.Substring( 0, ( CswNbtView.ViewNameLength - 2 ) );
						Int32 Increment = 1;
						while( !CswNbtView.ViewIsUnique( _CswNbtResources, new CswNbtViewId(), NewViewName, SourceView.Visibility, SourceView.VisibilityUserId, SourceView.VisibilityRoleId ) )
						{
							Increment++;
							NewViewName = NewViewNameOrig + " " + Increment.ToString();
						}

						NewView.makeNew( NewViewName, SourceView.Visibility, SourceView.VisibilityRoleId, SourceView.VisibilityUserId, SourceView );
						NewView.save();
						ReturnVal.Add( new JProperty( "copyviewid", NewView.ViewId.ToString() ) );
					}
				}

				_deInitResources();
			}
			catch( Exception Ex )
			{
				ReturnVal = jError( Ex );
			}

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );
			return ReturnVal.ToString();

		} // copyView()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string deleteView( string ViewId )
		{
			JObject ReturnVal = new JObject();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{

				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					CswNbtView DoomedView = _getView( ViewId );
					if( null != DoomedView )
					{
						DoomedView.Delete();
						ReturnVal.Add( new JProperty( "succeeded", true ) );
					}
				}

				_deInitResources();
			}
			catch( Exception Ex )
			{
				ReturnVal = jError( Ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		} // deleteView()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string createView( string ViewName, string ViewMode, string Visibility, string VisibilityRoleId, string VisibilityUserId )
		{
			JObject ReturnVal = new JObject();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{

				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					NbtViewRenderingMode RealViewMode = NbtViewRenderingMode.Unknown;
					Enum.TryParse<NbtViewRenderingMode>( ViewMode, out RealViewMode );
					NbtViewVisibility RealVisibility = NbtViewVisibility.Unknown;
					Enum.TryParse<NbtViewVisibility>( Visibility, out RealVisibility );
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
					ReturnVal.Add( new JProperty( "newviewid", NewView.ViewId.ToString() ) );
				}

				_deInitResources();
			}
			catch( Exception Ex )
			{
				ReturnVal = jError( Ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		} // createView()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getViewPropFilterUI( string ViewXml, string PropArbitraryId )
		{
			XElement ReturnVal = new XElement( "nodetypeprops" );
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					var ws = new wsViewBuilder( _CswNbtResources );
					ReturnVal = ws.getViewBuilderProps( ViewXml, PropArbitraryId );
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}

			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;

		}


		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement makeViewPropFilter( string ViewXml, string PropFiltJson )
		{
			XElement ReturnVal = new XElement( "nodetypeprops" );
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					var ws = new wsViewBuilder( _CswNbtResources );
					ReturnVal = ws.getViewPropFilter( ViewXml, PropFiltJson );
				}

				_deInitResources();
			}

			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}


			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;

		}

		#endregion View Editing

		#region Tabs and Props

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getTabs( string EditMode, string NodeId, string SafeNodeKey, string NodeTypeId )
		{
			XElement ReturnVal = new XElement( "tabs" );
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );
					//if( !string.IsNullOrEmpty( ParsedNodeKey ) || ( EditMode == "AddInPopup" && !string.IsNullOrEmpty( NodeTypeId ) ) )
					//{
					var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
					var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse( typeof( CswNbtWebServiceTabsAndProps.NodeEditMode ), EditMode );
					ReturnVal = ws.getTabs( RealEditMode, NodeId, ParsedNodeKey, CswConvert.ToInt32( NodeTypeId ) );
					//}
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}


			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;

		} // getTabs()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XmlDocument getProps( string EditMode, string NodeId, string SafeNodeKey, string TabId, string NodeTypeId )
		{
			XmlDocument ReturnVal = new XmlDocument();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );
					//if( !string.IsNullOrEmpty( ParsedNodeKey ) || EditMode == "AddInPopup" )
					//{
					var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
					var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse( typeof( CswNbtWebServiceTabsAndProps.NodeEditMode ), EditMode );
					ReturnVal = ws.getProps( RealEditMode, NodeId, ParsedNodeKey, TabId, CswConvert.ToInt32( NodeTypeId ) );
					//}
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = xmlError( ex );
			}


			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;
		} // getProps()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XmlDocument getSingleProp( string EditMode, string NodeId, string SafeNodeKey, string PropId, string NodeTypeId, string NewPropXml )
		{
			XmlDocument ReturnVal = new XmlDocument();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();

				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );
					//if( !string.IsNullOrEmpty( ParsedNodeKey ) )
					//{
					var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
					var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse( typeof( CswNbtWebServiceTabsAndProps.NodeEditMode ), EditMode );
					ReturnVal = ws.getSingleProp( RealEditMode, NodeId, ParsedNodeKey, PropId, CswConvert.ToInt32( NodeTypeId ), NewPropXml );
					//}
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = xmlError( ex );
			}

			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;
		} // getSingleProp()


		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getPropNames( string Type, string Id )
		{
			XElement ReturnVal = new XElement( "properties" );
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					Int32 nId = CswConvert.ToInt32( Id );

					if( nId != Int32.MinValue )
					{
						ICollection Props = null;
						string PropType = string.Empty;
						if( Type == "NodeTypeId" )
						{
							CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( nId );
							Props = NodeType.NodeTypeProps;
							PropType = CswNbtViewProperty.CswNbtPropType.NodeTypePropId.ToString();
						}
						else if( Type == "ObjectClassId" )
						{
							CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( nId );
							Props = ObjectClass.ObjectClassProps;
							PropType = CswNbtViewProperty.CswNbtPropType.ObjectClassPropId.ToString();
						}

						foreach( ICswNbtMetaDataProp Prop in Props )
						{
							ReturnVal.Add(
								new XElement( "prop",
									new XAttribute( "proptype", PropType ),
									new XAttribute( "propname", Prop.PropNameWithQuestionNo ),
									new XAttribute( "propid", Prop.PropId.ToString() ) ) );
						}
					} // if( nId != Int32.MinValue )

				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}


			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;

		} // getPropNames()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string saveProps( string EditMode, string NodeId, string SafeNodeKey, string NewPropsXml, string NodeTypeId, string ViewId )
		{
			JObject ReturnVal = new JObject();

			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );
					//if( !string.IsNullOrEmpty( ParsedNodeKey ) )
					//{
					var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
					var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse( typeof( CswNbtWebServiceTabsAndProps.NodeEditMode ), EditMode );
					CswNbtView View = _getView( ViewId );
					ReturnVal = ws.saveProps( RealEditMode, NodeId, ParsedNodeKey, NewPropsXml, CswConvert.ToInt32( NodeTypeId ), View );
					//}
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		} // saveProps()


		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string copyPropValues( string SourceNodeKey, string[] CopyNodeIds, string[] PropIds )
		{
			JObject ReturnVal = new JObject();


			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					string ParsedSourceNodeKey = wsTools.FromSafeJavaScriptParam( SourceNodeKey );
					var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
					bool ret = ws.copyPropValues( ParsedSourceNodeKey, CopyNodeIds, PropIds );
					ReturnVal.Add( new JProperty( "succeeded", ret ) );
				}

				_deInitResources();

			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		} // copyPropValue()	


		#endregion Tabs and Props

		#region Misc

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XmlDocument getAbout()
		{
			XmlDocument ReturnVal = new XmlDocument();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					var ws = new CswNbtWebServiceHeader( _CswNbtResources );
					string Data = ws.makeVersionXml();
					ReturnVal.LoadXml( Data.Replace( "&", "&amp;" ) );

				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = xmlError( ex );
			}

			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );
			return ReturnVal;
		} // getAbout()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getNodeTypes()
		{
			XElement ReturnVal = new XElement( "nodetypes" );
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					var ws = new CswNbtWebServiceMetaData( _CswNbtResources );
					ReturnVal = ws.getNodeTypes();
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}

			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;

		} // getNodeTypes()


		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string getLicense()
		{
			JObject ReturnVal = new JObject();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					CswLicenseManager LicenseManager = new CswLicenseManager( _CswNbtResources );
					ReturnVal.Add( new JProperty( "license", LicenseManager.LatestLicenseText ) );
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		}

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string acceptLicense()
		{
			JObject ReturnVal = new JObject();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					CswLicenseManager LicenseManager = new CswLicenseManager( _CswNbtResources );
					LicenseManager.RecordLicenseAcceptance( _CswNbtResources.CurrentUser );
					ReturnVal.Add( new JProperty( "result", "succeeded" ) );
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		}

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string fileForProp()
		{
			JObject ReturnVal = new JObject( new JProperty( "success", false.ToString().ToLower() ) );
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

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

				}
				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		} // fileForProp()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string getLabels( string PropId )
		{
			JObject ReturnVal = new JObject();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					CswNbtWebServicePrintLabels ws = new CswNbtWebServicePrintLabels( _CswNbtResources );
					ReturnVal = ws.getLabels( PropId );
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		}

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string getEPLText( string PropId, string PrintLabelNodeId )
		{
			JObject ReturnVal = new JObject();

			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					CswNbtWebServicePrintLabels ws = new CswNbtWebServicePrintLabels( _CswNbtResources );
					ReturnVal.Add( new JProperty( "epl", ws.getEPLText( PropId, PrintLabelNodeId ) ) );
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		}


		#endregion Misc

		#region Search

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getClientSearchXml( string ViewId, string SelectedNodeTypeIdNum, string IdPrefix, string NodeKey )
		{
			XElement ReturnVal = new XElement( "search" );

			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					var ws = new CswNbtWebServiceSearch( _CswNbtResources, IdPrefix );
					CswNbtView View = _getView( ViewId );
					ReturnVal = ws.getSearchXml( View, SelectedNodeTypeIdNum, NodeKey );
				}

				_deInitResources();
			}

			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}

			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;

		}

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getNodeTypeSearchProps( string RelatedIdType, string NodeTypeOrObjectClassId, string IdPrefix, string NodeKey )
		{
			XElement ReturnVal = new XElement( "search" );
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					var ws = new CswNbtWebServiceSearch( _CswNbtResources, IdPrefix );
					ReturnVal = ( ws.getSearchProps( RelatedIdType, NodeTypeOrObjectClassId, NodeKey ) );
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}


			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;

		} // getSearch()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getSearchableViews( string IsMobile, string OrderBy )
		{
			XElement ReturnVal = new XElement( "result" );

			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					ICswNbtUser UserId = _CswNbtResources.CurrentNbtUser;
					bool ForMobile = CswConvert.ToBoolean( IsMobile );
					var ws = new CswNbtWebServiceView( _CswNbtResources );
					//SearchNode =  ws.getSearchableViewTree( UserId, ForMobile, true, OrderBy ); 
				}

				_deInitResources();

			}

			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}

			return ReturnVal;

		} // getSearch()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string doViewSearch( object SearchJson )
		{
			JObject ReturnVal = new JObject();

			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					var ws = new CswNbtWebServiceSearch( _CswNbtResources );
					CswNbtView ResultsView = ws.doViewBasedSearch( SearchJson );
					ResultsView.clearSessionViewId();
					ResultsView.SaveToCache( true );

					ReturnVal.Add( new JProperty( "sessionviewid", ResultsView.SessionViewId.ToString() ) );
					ReturnVal.Add( new JProperty( "viewmode", ResultsView.ViewMode.ToString().ToLower() ) );
				}

				_deInitResources();
			}

			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		} // getSearch()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string doNodeTypeSearch( object SearchJson )
		{
			JObject ReturnVal = new JObject();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					var ws = new CswNbtWebServiceSearch( _CswNbtResources );
					CswNbtView ResultsView = ws.doNodesSearch( SearchJson );
					ResultsView.SaveToCache( true );
					ReturnVal.Add( new JProperty( "sessionviewid", ResultsView.SessionViewId.ToString() ) );
					ReturnVal.Add( new JProperty( "viewmode", ResultsView.ViewMode.ToString().ToLower() ) );
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		}

		#endregion Search

		#region Node DML
		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string DeleteNodes( string[] NodePks, string[] NodeKeys )
		{
			JObject ReturnVal = new JObject();
			List<CswPrimaryKey> NodePrimaryKeys = new List<CswPrimaryKey>();
			bool ret = true;
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					if( NodeKeys.Length > 0 )
					{
						foreach( string NodeKey in NodeKeys )
						{
							string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( NodeKey );
							CswNbtNodeKey NbtNodeKey = null;
							if( !string.IsNullOrEmpty( ParsedNodeKey ) )
							{
								NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, ParsedNodeKey );
								if( null != NbtNodeKey.NodeId )
								{
									NodePrimaryKeys.Add( NbtNodeKey.NodeId );
								}
							}
						}
					}
					if( NodePks.Length > 0 )
					{
						foreach( string NodePk in NodePks )
						{
							CswPrimaryKey PrimaryKey = new CswPrimaryKey();
							PrimaryKey.FromString( NodePk );
							if( PrimaryKey.PrimaryKey != Int32.MinValue && !NodePrimaryKeys.Contains( PrimaryKey ) )
							{
								NodePrimaryKeys.Add( PrimaryKey );
							}
						}
					}
					if( NodePrimaryKeys.Count > 0 )
					{
						foreach( CswPrimaryKey Npk in NodePrimaryKeys )
						{
							CswNbtWebServiceNode ws = new CswNbtWebServiceNode( _CswNbtResources, _CswNbtStatisticsEvents );
							ret = ret && ws.DeleteNode( Npk );
						}
					}

					ReturnVal.Add( new JProperty( "Succeeded", ret.ToString().ToLower() ) );
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		}


		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string CopyNode( string NodePk )
		{
			JObject ReturnVal = new JObject();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

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
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		}


		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string MoveProp( string PropId, string NewRow, string NewColumn, string EditMode )
		{
			JObject ReturnVal = new JObject();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
					var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse( typeof( CswNbtWebServiceTabsAndProps.NodeEditMode ), EditMode );
					bool ret = ws.moveProp( PropId, CswConvert.ToInt32( NewRow ), CswConvert.ToInt32( NewColumn ), RealEditMode );
					ReturnVal.Add( new JProperty( "moveprop", ret.ToString().ToLower() ) );
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		} // MoveProp()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string clearProp( string PropId, bool IncludeBlob )
		{
			JObject ReturnVal = new JObject( new JProperty( "Succeeded", false.ToString().ToLower() ) );
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
					bool ret = ws.ClearPropValue( PropId, IncludeBlob );
					ReturnVal = new JObject( new JProperty( "Succeeded", ret.ToString().ToLower() ) );
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		} // clearProp()

		#endregion Node DML

		#region Welcome Region


		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getWelcomeItems( string RoleId )
		{
			XElement ReturnVal = new XElement( "welcome" );
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

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
				}

				_deInitResources();
			}

			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}


			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;


		} // getWelcomeItems()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getWelcomeButtonIconList()
		{
			XElement ReturnVal = new XElement( "buttonicons" );
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					var ws = new CswNbtWebServiceWelcomeItems( _CswNbtResources );
					ReturnVal = ws.getButtonIconList();
				}

				_deInitResources();
			}

			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}

			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal;

		} // getWelcomeButtonIconList()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string addWelcomeItem( string RoleId, string Type, string WelcomePkVal, string NodeTypeId, string Text, string IconFileName )
		{
			JObject ReturnVal = new JObject();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					CswNbtWebServiceWelcomeItems ws = new CswNbtWebServiceWelcomeItems( _CswNbtResources );
					// Only administrators can add welcome content to other roles
					string UseRoleId = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
					if( RoleId != string.Empty && _CswNbtResources.CurrentNbtUser.IsAdministrator() )
						UseRoleId = RoleId;
					CswNbtWebServiceWelcomeItems.WelcomeComponentType ComponentType = (CswNbtWebServiceWelcomeItems.WelcomeComponentType) Enum.Parse( typeof( CswNbtWebServiceWelcomeItems.WelcomeComponentType ), Type );
					ws.AddWelcomeItem( ComponentType, NbtWebControls.CswViewListTree.ViewType.View, CswConvert.ToInt32( WelcomePkVal ), CswConvert.ToInt32( NodeTypeId ), Text, Int32.MinValue, Int32.MinValue, IconFileName, UseRoleId );
					ReturnVal.Add( new JProperty( "Succeeded", true ) );
					//ReturnVal = "{ \"Succeeded\": \"true\" }";
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		} // addWelcomeItem()

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string deleteWelcomeItem( string RoleId, string WelcomeId )
		{
			bool ret = false;
			JObject ReturnVal = new JObject();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					CswNbtWebServiceWelcomeItems ws = new CswNbtWebServiceWelcomeItems( _CswNbtResources );
					// Only administrators can add welcome content to other roles
					string UseRoleId = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
					if( RoleId != string.Empty && _CswNbtResources.CurrentNbtUser.IsAdministrator() )
						UseRoleId = RoleId;
					ret = ws.DeleteWelcomeItem( UseRoleId, CswConvert.ToInt32( WelcomeId ) );
					//ReturnVal = "{ \"Succeeded\": \"" + ret.ToString().ToLower() + "\" }";
					ReturnVal.Add( "Succeeded", ret );
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		} // deleteWelcomeItem()



		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string moveWelcomeItems( string RoleId, string WelcomeId, string NewRow, string NewColumn )
		{
			bool ret = false;
			//string ReturnVal = string.Empty;
			JObject ReturnVal = new JObject();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					CswNbtWebServiceWelcomeItems ws = new CswNbtWebServiceWelcomeItems( _CswNbtResources );
					// Only administrators can move welcome content for other roles
					string UseRoleId = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
					if( RoleId != string.Empty && _CswNbtResources.CurrentNbtUser.IsAdministrator() )
						UseRoleId = RoleId;
					ret = ws.MoveWelcomeItems( UseRoleId, CswConvert.ToInt32( WelcomeId ), CswConvert.ToInt32( NewRow ), CswConvert.ToInt32( NewColumn ) );
					//ReturnVal = "{ \"Succeeded\": \"" + ret.ToString().ToLower() + "\" }";
					ReturnVal.Add( "Succeeded", ret );
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		} // moveWelcomeItems()

		#endregion Welcome Region

		#region Permissions

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string isAdministrator()
		{
			JObject ReturnVal = new JObject();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{
					ReturnVal.Add( new JProperty( "Administrator", _CswNbtResources.CurrentNbtUser.IsAdministrator().ToString().ToLower() ) );
				}

				_deInitResources();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		} // isAdministrator()

		#endregion Permissions

		#region Connectivity
		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement ConnectTest()
		{
			// no session needed here
			XElement Connected = new XElement( "Connected" );
			_xAddAuthenticationStatus( Connected, AuthenticationStatus.Authenticated );  // we don't want to trigger session timeouts
			return ( Connected );
		}


		[WebMethod( EnableSession = false )]
		public void ConnectTestFail()
		{
			// no session needed here

			// this exception needs to be UNCAUGHT
			throw new Exception( "Emulated connection failure" );
		}

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement ConnectTestRandomFail()
		{
			// no session needed here

			// this exception needs to be UNCAUGHT
			Random r = new Random();
			Int32 rand = r.Next( 0, 3 );
			if( rand == 0 )
			{
				throw new Exception( "Emulated connection failure" );
			}
			else
			{
				XElement Connected = new XElement( "Connected" );
				return ( Connected );
			}
		}
		#endregion Connectivity

		#region Mobile
		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string UpdateProperties( string SessionId, string ParentId, string UpdatedViewXml, bool ForMobile )
		{
			JObject ReturnVal = new JObject();
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					CswNbtWebServiceMobileUpdateProperties wsUP = new CswNbtWebServiceMobileUpdateProperties( _CswNbtResources, ForMobile );
					string ViewXml = wsUP.Run( ParentId, UpdatedViewXml ).ToString();
					ReturnVal.Add( new JProperty( "xml", ViewXml ) );
				}

				_deInitResources();
			}

			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}


			_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

			return ReturnVal.ToString();

		} // UpdateProperties()


		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement GetViewsList( string SessionId, string ParentId, bool ForMobile )
		{
			XElement ReturnVal = new XElement( "views" );
			AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
			try
			{
				_initResources();
				AuthenticationStatus = _CswSessionResources.attemptRefresh();

				if( AuthenticationStatus.Authenticated == AuthenticationStatus )
				{

					ICswNbtUser CurrentUser = _CswNbtResources.CurrentNbtUser;
					if( null != CurrentUser )
					{
						CswNbtWebServiceMobileView wsView = new CswNbtWebServiceMobileView( _CswNbtResources, ForMobile );
						ReturnVal = wsView.getViewsList( ParentId, CurrentUser );
					}

				}

				_deInitResources();
			}

			catch( Exception ex )
			{
				ReturnVal = _xError( ex );
			}

			_xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );
			return ReturnVal;
		} // GetViews()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement GetView( string SessionId, string ParentId, bool ForMobile )
        {
            XElement ReturnVal = new XElement( "views" );
            AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _CswSessionResources.attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    ICswNbtUser CurrentUser = _CswNbtResources.CurrentNbtUser;
                    if( null != CurrentUser )
                    {
                        CswNbtWebServiceMobileView wsView = new CswNbtWebServiceMobileView( _CswNbtResources, ForMobile );
                        ReturnVal = wsView.getView( ParentId, CurrentUser );
                    }

                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }

            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );
            return ReturnVal;
        } // GetViews()

		#endregion Mobile

        #region test
        [WebMethod( EnableSession = true )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string GetTestData()
        {
            JObject RetJson = new JObject( new JProperty( "A", "Static Page 1" ), new JProperty( "B", "Static Page 2" ), new JProperty("C", "Dynamic Page A" ), new JProperty( "D", "Dynamic Page B" ) );
            return ( RetJson.ToString() );
        } // RunView()
        #endregion test
				#endregion Web Methods

		private CswNbtView _getView( string ViewId )
		{
			CswNbtView View = null;
			if( CswNbtViewId.isViewIdString( ViewId ) )
			{
				CswNbtViewId realViewid = new CswNbtViewId( ViewId );
				View = _CswNbtResources.ViewSelect.restoreView( realViewid );
			}
			else if( CswNbtSessionDataId.isSessionDataIdString( ViewId ) )
			{
				CswNbtSessionDataId SessionViewid = new CswNbtSessionDataId( ViewId );
				View = _CswNbtResources.ViewSelect.getSessionView( SessionViewid );
			}
			return View;
		} // _getView()


	}//wsNBT

} // namespace ChemSW.WebServices
