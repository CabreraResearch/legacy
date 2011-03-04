using System;
using System.Data;
using System.Linq;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.PropTypes;
using System.Collections.Generic;

namespace ChemSW.Nbt.WebServices
{
	/// <summary>
	/// Webservice for the list of Quick Launch links
	/// </summary>
	public class CswNbtWebServiceQuickLaunchItems : CompositeControl
	{
		private readonly CswNbtResources _CswNbtResources;
		private const string ActionName = "actionname";
		private const string ActionPk = "actionid";
		private const string ActionSelected = "Include";
		private HttpSessionState _Session;
		private LinkedList<CswNbtQuickLaunchItem> _QuickLaunchHistory = null;
		private readonly bool _IsNewSession;

		public const string QuickLaunchViews = "QuickLaunchViews";
		public CswNbtWebServiceQuickLaunchItems( CswNbtResources CswNbtResources, HttpSessionState Session )
		{
			_CswNbtResources = CswNbtResources;
			_IsNewSession = ( null == Session[QuickLaunchViews] );
			_Session = Session;
			if( !_IsNewSession )
			{
				_QuickLaunchHistory = (LinkedList<CswNbtQuickLaunchItem>) Session[QuickLaunchViews];
			}
			else
			{
				_QuickLaunchHistory = new LinkedList<CswNbtQuickLaunchItem>();
			}

		} //ctor

		/// <summary>
		/// Returns Quick Launch Items including History in Session
		/// </summary>
		public XElement getQuickLaunchItems( CswPrimaryKey UserId )
		{
			var QuickLaunchNode = new XElement( "items" );
			if( _IsNewSession )
			{
				CswNbtNode UserNode = _CswNbtResources.Nodes.GetNode( UserId );
				if( null != UserNode )
				{
					CswNbtObjClassUser UserOc = CswNbtNodeCaster.AsUser( UserNode );

					//Add the user's stored views to QuickLaunchHistory
					CswCommaDelimitedString UserQuickLaunchViews = UserOc.QuickLaunchViews.SelectedViewIds;
					foreach( var QuickLaunchItem in UserQuickLaunchViews.Where( View => !String.IsNullOrEmpty( View ) )
						.Select( CswConvert.ToInt32 )
						.Where( ViewId => Int32.MinValue != ViewId )
						.Select( ViewId => CswNbtViewFactory.restoreView( _CswNbtResources, ViewId ) )
						.Where( QuickLaunchView => null != QuickLaunchView && QuickLaunchView.IsFullyEnabled() )
						.Select( QuickLaunchView => new CswNbtQuickLaunchItem( QuickLaunchView.ViewId, QuickLaunchView.ViewName, QuickLaunchView.ViewMode ) ) )
					{
						if( _QuickLaunchHistory.Contains( QuickLaunchItem ) )
						{
							_QuickLaunchHistory.Remove( QuickLaunchItem );
						}
						_QuickLaunchHistory.AddFirst( QuickLaunchItem );
					}

					//Add the user's stored actions to QuickLaunchHistory
					CswNbtNodePropLogicalSet ActionsLogicalSet = UserOc.QuickLaunchActions;
					DataTable ActionsTable = ActionsLogicalSet.GetDataAsTable( ActionName, ActionPk );
					foreach( var QuickLaunchItem in ( from DataRow ActionRow in ActionsTable.Rows
														where CswConvert.ToBoolean( ActionRow[ActionSelected] )
														select _CswNbtResources.Actions[CswNbtAction.ActionNameStringToEnum( ActionRow[ActionName].ToString() )]
														into ThisAction where null != ThisAction select ThisAction )
						.Select( ThisAction => new CswNbtQuickLaunchItem( ThisAction.ActionId, ThisAction.Name.ToString(), ThisAction.Url ) ) )
					{
						if( _QuickLaunchHistory.Contains( QuickLaunchItem ) )
						{
							_QuickLaunchHistory.Remove( QuickLaunchItem );
						}
						_QuickLaunchHistory.AddFirst( QuickLaunchItem );
					}
				}
			} // if( IsNewSession )

			_Session[QuickLaunchViews] = _QuickLaunchHistory;

			foreach( var Item in _QuickLaunchHistory )
			{
				var ThisItem = new XElement( "item" );
				ThisItem.SetAttributeValue( "type", Item.ItemType.ToString() );
				ThisItem.SetAttributeValue( "itemid", Item.ItemId.ToString() ); 
				ThisItem.SetAttributeValue( "text", Item.ItemName );
				ThisItem.SetAttributeValue( "viewmode", Item.ViewMode ); //unknown if !view
				ThisItem.SetAttributeValue( "url", Item.ItemUrl ); //empty if !action
				QuickLaunchNode.Add( ThisItem );
			} // foreach( var Tuple in QuickLaunchHistory )

			return QuickLaunchNode;

		} // getQuickLaunchItems()

		/// <summary>
		/// Resets the Quick Launch Item's XML to the user's stored quick launch items
		/// </summary>
		public XElement resetQuickLaunchItems( CswPrimaryKey UserId )
		{
			XElement QuickLaunchNode = null;
			CswNbtNode UserNode = _CswNbtResources.Nodes.GetNode( UserId );
			if( null != UserNode )
			{
				LinkedList<Tuple<Int32, string, string>> QuickLaunchHistory = null;
				QuickLaunchNode = getQuickLaunchItems( UserId );
				_Session[QuickLaunchViews] = QuickLaunchHistory;
			}
			return QuickLaunchNode;

		} // ResetQuickLaunchItems()

	} // class CswNbtWebServiceWelcomeItems

	/// <summary>
	/// An instance of this class represents a single Quick Launch Item
	/// </summary>
	public class CswNbtQuickLaunchItem : IComparer<CswNbtQuickLaunchItem>, IEquatable<CswNbtQuickLaunchItem>
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
		
		public readonly Int32 ItemId = Int32.MinValue;
		public readonly string ItemName = string.Empty;
		public readonly string ItemUrl = string.Empty;
		public readonly NbtViewRenderingMode ViewMode = NbtViewRenderingMode.Unknown;
		public readonly QuickLaunchType ItemType = QuickLaunchType.Unknown;

		/// <summary>
		/// Instances a Quick Launch Action item
		/// </summary>
		public CswNbtQuickLaunchItem( Int32 ActionId, String ActionName, String ActionUrl )
		{
			ItemId = ActionId;
			ItemName = ActionName;
			ItemUrl = ActionUrl;
			ItemType = QuickLaunchType.Action;
		} //ctor Action

		/// <summary>
		/// Instances a Quick Launch View item
		/// </summary>
		public CswNbtQuickLaunchItem( Int32 ViewId, String ViewName, NbtViewRenderingMode Mode )
		{
			ItemId = ViewId;
			ItemName = ViewName;
			ViewMode = Mode;
			ItemType = QuickLaunchType.View;
		} //ctor View


		#region IComparer
		public int Compare( CswNbtQuickLaunchItem Ql1, CswNbtQuickLaunchItem Ql2)
		{
			Int32 ReturnVal = Int32.MinValue;
			if( Ql1 == Ql2 )
			{
				ReturnVal = 0;
			}
			return ReturnVal;
		}
		#endregion

		#region IEquatable
		/// <summary>
		/// IEquatable implementation: ==
		/// </summary>
		public static bool operator ==( CswNbtQuickLaunchItem Ql1, CswNbtQuickLaunchItem Ql2 )
		{
			bool Equals = false;
			// If both are null, or both are same instance, return true.
			if( System.Object.ReferenceEquals( Ql1, Ql2 ) )
			{
				Equals = true;
			}

			// If one is null, but not both, return false.
			if( ( (object) Ql1 == null ) || ( (object) Ql2 == null ) )
			{
				Equals = false;
			}

			// Now we know neither are null.  Compare values.
			if( ( Ql1.ItemId == Ql2.ItemId ) &&
				( Ql1.ItemName == Ql2.ItemName ) &&
				( Ql1.ItemType == Ql2.ItemType ) )
			{
				Equals = true;
			}

			return Equals;
		}

		/// <summary>
		/// IEquatable implementation: !=
		/// </summary>
		public static bool operator !=( CswNbtQuickLaunchItem Ql1, CswNbtQuickLaunchItem Ql2 )
		{
			return !( Ql1.ItemId == Ql2.ItemId && 
					  Ql1.ItemName == Ql2.ItemName &&  
					  Ql1.ItemType == Ql2.ItemType );
		}

		/// <summary>
		/// IEquatable implementation: Equals
		/// </summary>
		public override bool Equals( object obj )
		{
			bool Equals = false;
			if( ( obj is CswNbtQuickLaunchItem ) )
			{
				Equals = ( this == (CswNbtQuickLaunchItem) obj );
			}
			return Equals;
		}

		/// <summary>
		/// IEquatable implementation: Equals
		/// </summary>
		public bool Equals( CswNbtQuickLaunchItem obj )
		{
			return this == (CswNbtQuickLaunchItem) obj;
		}

		/// <summary>
		/// IEquatable implementation: GetHashCode
		/// </summary>
		public override int GetHashCode()
		{
			return ItemId;
		}

		#endregion IEquatable

	}
} // namespace ChemSW.Nbt.WebServices

