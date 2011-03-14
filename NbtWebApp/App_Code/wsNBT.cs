﻿using System;
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
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
		    return ex.Message;
		}

        /// <summary>
        /// Returns error as XElement
        /// </summary>
        private XElement xError ( Exception ex )
        {
            return ( new XElement( "error" ) {Value = "Error: " + error(ex) } );
        }
        
        /// <summary>
        /// Returns error as JProperty
        /// </summary>
        private JProperty jError( Exception ex )
        {
            return ( new JProperty( "error" ) {Value = "Error: " + error( ex )} );
        }

        //never used
        //private string result( string ReturnVal )
        //{
        //    return "<result>" + ReturnVal + "</result>";
        //}

		#endregion Session and Resource Management

		#region Web Methods


		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string authenticate( string AccessId, string UserName, string Password )
		{
		    JProperty ReturnVal = new JProperty( "AuthenticationStatus" );
			try
			{
				start();
				_SessionResources.CswSessionManager.setAccessId( AccessId );
				AuthenticationStatus AuthenticationStatus = _SessionResources.CswSessionManager.Authenticate( UserName, Password, CswWebControls.CswNbtWebTools.getIpAddress() );
				//ReturnVal = result( "<AuthenticationStatus>" + AuthenticationStatus + "</AuthenticationStatus>" );
				ReturnVal.Value = AuthenticationStatus.ToString();
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}

			return ( ReturnVal.ToString() );
		}//authenticate()


		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string deauthenticate()
		{
		    JProperty ReturnVal = new JProperty( "Deauthentication" );
			try
			{
				start();
				_SessionResources.CswSessionManager.DeAuthenticate();
				ReturnVal.Value = "Succeeded";
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}
			return ( ReturnVal.ToString() );
		}//deAuthenticate()

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
                    ReturnVal = XElement.Parse(ws.GetWelcomeItems( RoleId ));
                }
                else
                {
                    ReturnVal = XElement.Parse(ws.GetWelcomeItems( _CswNbtResources.CurrentNbtUser.RoleId.ToString() ));
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
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getViewTree()
		{
		    var ReturnVal = new XElement( "viewtree" );
			try
			{
				start();
				var ws = new CswNbtWebServiceView( _CswNbtResources );
				ReturnVal = XElement.Parse(ws.getViewTree(Session));
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
				ReturnVal = XElement.Parse(ws.getHeaderMenu());
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
            var ReturnNode = new XElement("menu");
            try
            {
                start();
                var ws = new CswNbtWebServiceMainMenu(_CswNbtResources);
                Int32 ViewId = CswConvert.ToInt32(ViewNum);
                if (Int32.MinValue != ViewId || !string.IsNullOrEmpty(SafeNodeKey))
                {
                    ReturnNode = ws.getMenu(ViewId, SafeNodeKey);
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
            string ParsedNokeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );

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
						if( !string.IsNullOrEmpty( ParsedNokeKey ) )
						{
							ParentNodeKey = new CswNbtNodeKey( _CswNbtResources, ParsedNokeKey );
						}
						var g = new CswNbtWebServiceGrid( _CswNbtResources, View, ParentNodeKey );
						ReturnJson = g.getGrid();
                        CswNbtWebServiceQuickLaunchItems.addToQuickLaunch(View, Session);
					}
				}
				end();
			}
			catch( Exception Ex )
			{
                ReturnJson.Add( jError( Ex ));
			}

            return ReturnJson.ToString();
		} // getGrid()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getTree( string ViewNum, string IDPrefix )
		{
		    var TreeNode = new XElement("tree");

			try
			{
				start();
			    Int32 ViewId = CswConvert.ToInt32( ViewNum );
                if( Int32.MinValue != ViewId )
                {
                    CswNbtView View = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
                    if( null != View )
                    {
                        var ws = new CswNbtWebServiceTree( _CswNbtResources );
                        TreeNode = ws.getTree( View, IDPrefix );
                        CswNbtWebServiceQuickLaunchItems.addToQuickLaunch( View, Session );
                    }
                }
			    end();
			}
			catch( Exception ex )
			{
                TreeNode = xError( ex );
			}

            return TreeNode;
		} // getTree()


		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getTabs( string EditMode, string SafeNodeKey, string NodeTypeId )
		{
		    var TabsNode = new XElement("tabs");
            try
            {
                start();
                string ParsedNokeKey = wsTools.FromSafeJavaScriptParam(SafeNodeKey);
                if (!string.IsNullOrEmpty(ParsedNokeKey) || EditMode == "AddInPopup")
                {
                    var ws = new CswNbtWebServiceTabsAndProps(_CswNbtResources);
                    var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse(typeof (CswNbtWebServiceTabsAndProps.NodeEditMode), EditMode);
                    TabsNode = ws.getTabs( RealEditMode, ParsedNokeKey, CswConvert.ToInt32( NodeTypeId ) );
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
				string ParsedNokeKey = wsTools.FromSafeJavaScriptParam(SafeNodeKey);
                if( !string.IsNullOrEmpty( ParsedNokeKey ) || EditMode == "AddInPopup" )
                {
                    var ws = new CswNbtWebServiceTabsAndProps(_CswNbtResources);
                    var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse(typeof (CswNbtWebServiceTabsAndProps.NodeEditMode), EditMode);
                    ReturnXml = ws.getProps( RealEditMode, ParsedNokeKey, TabId, CswConvert.ToInt32( NodeTypeId ) );
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
                string ParsedNokeKey = wsTools.FromSafeJavaScriptParam(SafeNodeKey);
                if( !string.IsNullOrEmpty( ParsedNokeKey ) )
                {
                    var ws = new CswNbtWebServiceTabsAndProps(_CswNbtResources);
                    var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse(typeof (CswNbtWebServiceTabsAndProps.NodeEditMode), EditMode);
                    ReturnXml = ws.getSingleProp( RealEditMode, ParsedNokeKey, PropId, CswConvert.ToInt32( NodeTypeId ),
                                                 NewPropXml);
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
		public XElement saveProps( string EditMode, string SafeNodeKey, string NewPropsXml, string NodeTypeId )
		{
            var ReturnVal = new XElement( "saveprops" );
			try
			{
				start();
                string ParsedNokeKey = wsTools.FromSafeJavaScriptParam(SafeNodeKey);
                if( !string.IsNullOrEmpty( ParsedNokeKey ) )
                {
                    var ws = new CswNbtWebServiceTabsAndProps(_CswNbtResources);
                    var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse(typeof (CswNbtWebServiceTabsAndProps.NodeEditMode), EditMode);
                    ReturnVal = XElement.Parse(ws.saveProps( RealEditMode, ParsedNokeKey, NewPropsXml, CswConvert.ToInt32( NodeTypeId ) ));
                }
			    end();
			}
			catch( Exception ex )
			{
				ReturnVal = xError( ex );
			}
			return ( ReturnVal );
		} // saveProps()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XmlDocument getAbout()
		{
            var ReturnVal = string.Empty;
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
		} // saveProps()

        

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string DeleteNode( string SafeNodeKey )
		{
			var ReturnVal = new JProperty("delete");
			try
			{
				start();
                string ParsedNokeKey = wsTools.FromSafeJavaScriptParam(SafeNodeKey);
                if( !string.IsNullOrEmpty( ParsedNokeKey ) )
                {
                    CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, ParsedNokeKey );
                    CswNbtNode NodeToDelete = _CswNbtResources.Nodes[NbtNodeKey.NodeId];
                    NodeToDelete.delete();
                    ReturnVal.Value = "Succeeded";
                }
			    end();
			}
			catch( Exception ex )
			{
                ReturnVal = jError( ex );
			}
			return ( ReturnVal.ToString() );
		}

		[WebMethod( EnableSession = true )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string MoveProp( string PropId, string NewRow, string NewColumn )
		{
            var ReturnVal = new JProperty( "moveprop" );
			try
			{
				start();
				var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
				bool ret = ws.moveProp( PropId, CswConvert.ToInt32( NewRow ), CswConvert.ToInt32( NewColumn ) );
				ReturnVal.Value = ret.ToString().ToLower();
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = jError( ex );
			}
			return ( ReturnVal.ToString() );
		}

		#endregion Web Methods

	}//wsNBT

} // namespace ChemSW.WebServices
