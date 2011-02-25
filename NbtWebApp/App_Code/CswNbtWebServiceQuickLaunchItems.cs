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
		
		private readonly CswNbtResources _CswNbtResources;
		private const string ActionName = "actionname";
		private const string ActionPk = "actionid";
		private const string ActionSelected = "Include";
		private const string QuickLaunchViews = "QuickLaunchViews";
		private const string QuickLaunch = "quicklaunch";

		private LinkedList<Tuple<Int32, string, string, QuickLaunchType>> _QuickLaunchHistory = null;
		private readonly bool _IsNewSession;

		public CswNbtWebServiceQuickLaunchItems( CswNbtResources CswNbtResources, HttpSessionState Session )
		{
			_CswNbtResources = CswNbtResources;
			_IsNewSession = ( null == Session[QuickLaunchViews] );
			if( !_IsNewSession )
			{
				_QuickLaunchHistory = (LinkedList<Tuple<Int32, string, string, QuickLaunchType>>) Session[QuickLaunchViews];
			}
			else
			{
				_QuickLaunchHistory = new LinkedList<Tuple<int, string, string, QuickLaunchType>>();
			}

		}

		/// <summary>
		/// Returns Quick Launch Items including History in Session
		/// </summary>
		public XmlDocument getQuickLaunchItems( CswPrimaryKey UserId, HttpSessionState Session )
		{
			var ReturnXml = new XmlDocument();
			XmlNode QuickLaunchNode = CswXmlDocument.SetDocumentElement( ReturnXml, QuickLaunch );
			if( _IsNewSession )
			{
				CswNbtNode UserNode = _CswNbtResources.Nodes.GetNode( UserId );
				if( null != UserNode )
				{
					CswNbtObjClassUser UserOc = CswNbtNodeCaster.AsUser( UserNode );

					//Add the user's stored views to QuickLaunchHistory
					CswCommaDelimitedString UserQuickLaunchViews = UserOc.QuickLaunchViews.SelectedViewIds;
					foreach( var ThisTupleView in UserQuickLaunchViews.Where( View => !String.IsNullOrEmpty( View ) )
						.Select( CswConvert.ToInt32 )
						.Where( ViewId => Int32.MinValue != ViewId )
						.Select( ViewId => CswNbtViewFactory.restoreView( _CswNbtResources, ViewId ) )
						.Where( QuickLaunchView => null != QuickLaunchView && QuickLaunchView.IsFullyEnabled() )
						.Select( QuickLaunchView => new Tuple<Int32, string, string, QuickLaunchType>( QuickLaunchView.ViewId, QuickLaunchView.ViewName, QuickLaunchView.ViewMode.ToString(), QuickLaunchType.View ) ) )
					{
						if( _QuickLaunchHistory.Contains( ThisTupleView ) )
						{
							_QuickLaunchHistory.Remove( ThisTupleView );
						}
						_QuickLaunchHistory.AddFirst( ThisTupleView );
					}

					//Add the user's stored actions to QuickLaunchHistory
					CswNbtNodePropLogicalSet ActionsLogicalSet = UserOc.QuickLaunchActions;
					DataTable ActionsTable = ActionsLogicalSet.GetDataAsTable( ActionName, ActionPk );
					foreach( var ThisTupleAction in ( from DataRow ActionRow in ActionsTable.Rows
						                                where CswConvert.ToBoolean( ActionRow[ActionSelected] )
						                                select _CswNbtResources.Actions[CswNbtAction.ActionNameStringToEnum( ActionRow[ActionName].ToString() )]
						                                into ThisAction where null != ThisAction select ThisAction )
						.Select( ThisAction => new Tuple<Int32, string, string, QuickLaunchType>( ThisAction.ActionId, ThisAction.Name.ToString(), ThisAction.Url, QuickLaunchType.Action ) ) )
					{
						if( _QuickLaunchHistory.Contains( ThisTupleAction ) )
						{
							_QuickLaunchHistory.Remove( ThisTupleAction );
						}
						_QuickLaunchHistory.AddFirst( ThisTupleAction );
					}
				}
			} // if( IsNewSession )

			Session[QuickLaunchViews] = _QuickLaunchHistory;

			foreach( var Tuple in _QuickLaunchHistory )
			{
				XmlNode ThisItem = CswXmlDocument.AppendXmlNode( QuickLaunchNode, "item" );
				CswXmlDocument.AppendXmlAttribute( ThisItem, "type", Tuple.Item4.ToString() ); //QuickLaunchType
				CswXmlDocument.AppendXmlAttribute( ThisItem, "itemid", Tuple.Item1.ToString() ); //viewid/actionid
				CswXmlDocument.AppendXmlAttribute( ThisItem, "text", Tuple.Item2 ); //ViewName/ActionName
				switch( Tuple.Item4 )
				{
					case QuickLaunchType.View:
						{
							CswXmlDocument.AppendXmlAttribute( ThisItem, "viewmode", Tuple.Item3.ToLower() );
							break;
						}
					case QuickLaunchType.Action:
						{
							CswXmlDocument.AppendXmlAttribute( ThisItem, "url", Tuple.Item3.ToLower() );
							break;
						}
				}
			} // foreach( var Tuple in QuickLaunchHistory )
			
			return ReturnXml;

		} // getQuickLaunchItems()

		/// <summary>
		/// Resets the Quick Launch Item's XML to the user's stored quick launch items
		/// </summary>
		public XmlDocument resetQuickLaunchItems( CswPrimaryKey UserId, HttpSessionState Session )
		{
			var ReturnXML = new XmlDocument();
			CswNbtNode UserNode = _CswNbtResources.Nodes.GetNode( UserId );
			if( null != UserNode )
			{
				LinkedList<Tuple<Int32, string, string>> QuickLaunchHistory = null;
				ReturnXML = getQuickLaunchItems( UserId, Session );
				Session[QuickLaunchViews] = QuickLaunchHistory;
			}
			return ReturnXML;

		} // ResetQuickLaunchItems()

	} // class CswNbtWebServiceWelcomeItems
} // namespace ChemSW.Nbt.WebServices

