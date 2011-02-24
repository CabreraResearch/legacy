using System;
using System.Data;
using System.Linq;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.PropTypes;
using System.Collections.Generic;
using System.Xml;

namespace ChemSW.Nbt.WebServices
{
	/// <summary>
	/// Webservice for the list of Quick Launch links
	/// </summary>
	public class CswNbtWebServiceQuickLaunchItems : CompositeControl
	{
		private CswNbtResources _CswNbtResources;
		private const string ActionName = "actionname";
		private const string ActionPk = "actionid";
		private const string ActionSelected = "Include";
		private const string QuickLaunchViews = "QuickLaunchViews";
		private const string QuickLaunch = "quicklaunch";

		public CswNbtWebServiceQuickLaunchItems( CswNbtResources CswNbtResources )
		{
			_CswNbtResources = CswNbtResources;
		}

		/// <summary>
		/// Types of Quick Launch Links
		/// </summary>
		public enum QuickLaunchType
		{
			/// <summary>
			/// Link to a View
			/// </summary>
			View,
			/// <summary>
			/// Link to an Action
			/// </summary>
			Action,
			/// <summary>
			/// Undefined
			/// </summary>
			Unknown
		}

		public XmlDocument getQuickLaunchItems( CswPrimaryKey UserId, HttpSessionState Session )
		{
			var ReturnXML = new XmlDocument();
			CswNbtNode UserNode = _CswNbtResources.Nodes.GetNode( UserId );
			if( null != UserNode )
			{
				
				bool IsNewSession = ( null == Session[QuickLaunchViews] );
				

				// Add Recent Views from Session First
				Stack<Tuple<Int32, string, string>> QuickLaunchHistory = null;
				if( !IsNewSession )
				{
					QuickLaunchHistory = (Stack<Tuple<Int32, string, string>>) Session[QuickLaunchViews];
				}
				else
				{
					QuickLaunchHistory = new Stack<Tuple<int, string, string>>();
				}

				ReturnXML = generateQuickLaunchItemsXml( UserNode, QuickLaunchHistory, Session, IsNewSession );

			}
			return ReturnXML;

		} // getQuickLaunchItems()

		/// <summary>
		/// Generates the Quick Launch Items XML which either includes session history or is only stored content
		/// </summary>
		private XmlDocument generateQuickLaunchItemsXml( CswNbtNode UserNode, Stack<Tuple<Int32, string, string>> QuickLaunchHistory, HttpSessionState Session, bool IsNewSession = true )
		{
			var ReturnXML = new XmlDocument();
			XmlNode QuickLaunchNode = CswXmlDocument.SetDocumentElement( ReturnXML, QuickLaunch );
			
			if( IsNewSession )
			{
				//Add the user's stored views to QuickLaunchHistory
				CswNbtObjClassUser UserOC = CswNbtNodeCaster.AsUser( UserNode );
				CswCommaDelimitedString UserQuickLaunchViews = UserOC.QuickLaunchViews.SelectedViewIds;
				foreach( CswNbtView QuickLaunchView in UserQuickLaunchViews.Where( View => !String.IsNullOrEmpty( View ) )
					.Select( View => CswConvert.ToInt32( View ) )
					.Where( ViewId => Int32.MinValue != ViewId )
					.Select( ViewId => CswNbtViewFactory.restoreView( _CswNbtResources, ViewId ) )
					.Where( QuickLaunchView => null != QuickLaunchView && QuickLaunchView.IsFullyEnabled() ) )
				{
					var ThisView = new Tuple<Int32, string, string>( QuickLaunchView.ViewId, QuickLaunchView.ViewName, QuickLaunchView.ViewMode.ToString() );
					if( !QuickLaunchHistory.Contains( ThisView ) )
					{
						QuickLaunchHistory.Push( ThisView );
					}
				} // foreach( CswNbtView QuickLaunchView...
				
				//This ensures that the user's Quick Launch views stay at bottom of the stack
				Session[QuickLaunchViews] = QuickLaunchHistory;
			} // if( IsNewSession )

			foreach( var Tuple in QuickLaunchHistory )
			{
				XmlNode ThisItem = CswXmlDocument.AppendXmlNode( QuickLaunchNode, "item" );
				CswXmlDocument.AppendXmlAttribute( ThisItem, "type", QuickLaunchType.View.ToString() );
				CswXmlDocument.AppendXmlAttribute( ThisItem, "viewid", Tuple.Item1.ToString() );
				CswXmlDocument.AppendXmlAttribute( ThisItem, "text", Tuple.Item2 );
				CswXmlDocument.AppendXmlAttribute( ThisItem, "viewmode", Tuple.Item3 );
			} // foreach( var Tuple in QuickLaunchHistory )

			if( IsNewSession )
			{
				CswNbtNodePropLogicalSet ActionsLogicalSet = ( _CswNbtResources.CurrentNbtUser.UserNode ).QuickLaunchActions;
				DataTable ActionsTable = ActionsLogicalSet.GetDataAsTable( ActionName, ActionPk );
				foreach( CswNbtAction ThisAction in from DataRow ActionRow in ActionsTable.Rows
				                                    where CswConvert.ToBoolean( ActionRow[ActionSelected] )
				                                    select _CswNbtResources.Actions[CswNbtAction.ActionNameStringToEnum( ActionRow[ActionName].ToString() )]
				                                    into ThisAction
				                                    where null != ThisAction
				                                    select ThisAction )
				{
					XmlNode ThisItem = CswXmlDocument.AppendXmlNode( QuickLaunchNode, "item" );
					CswXmlDocument.AppendXmlAttribute( ThisItem, "type", QuickLaunchType.Action.ToString() );
					CswXmlDocument.AppendXmlAttribute( ThisItem, "viewid", ThisAction.ActionId.ToString() );
					CswXmlDocument.AppendXmlAttribute( ThisItem, "text", ThisAction.Name.ToString() );
					CswXmlDocument.AppendXmlAttribute( ThisItem, "url", ThisAction.Url );
				} // foreach( CswNbtAction ThisAction...
			} // if( IsNewSession )

			return ReturnXML;
		} // getUsersStoredViews()

		/// <summary>
		/// Resets the Quick Launch Item's XML to the user's stored quick launch items
		/// </summary>
		public XmlDocument resetQuickLaunchItems( CswPrimaryKey UserId, HttpSessionState Session )
		{
			var ReturnXML = new XmlDocument();
			CswNbtNode UserNode = _CswNbtResources.Nodes.GetNode( UserId );
			if( null != UserNode )
			{
				Stack<Tuple<Int32, string, string>> QuickLaunchHistory = null;
				ReturnXML = generateQuickLaunchItemsXml( UserNode, QuickLaunchHistory, Session );
				Session[QuickLaunchViews] = QuickLaunchHistory;
			}
			return ReturnXML;

		} // ResetQuickLaunchItems()

	} // class CswNbtWebServiceWelcomeItems
} // namespace ChemSW.Nbt.WebServices

