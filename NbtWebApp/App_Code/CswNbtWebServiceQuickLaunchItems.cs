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
				_Session[QuickLaunchViews] = _QuickLaunchHistory;
			} // if( IsNewSession )

			foreach( var Item in _QuickLaunchHistory )
			{
				var ThisItem = new XElement( "item" );
				ThisItem.SetAttributeValue( "type", Item.ItemType.ToString().ToLower() );
				ThisItem.SetAttributeValue( "itemid", Item.ItemId.ToString()); 
				ThisItem.SetAttributeValue( "text", Item.ItemName );
				ThisItem.SetAttributeValue( "viewmode", Item.ViewMode.ToString().ToLower() ); //unknown if !view
				ThisItem.SetAttributeValue( "url", Item.ItemUrl ); //empty if !action
				QuickLaunchNode.Add( ThisItem );
			} // foreach( var Tuple in QuickLaunchHistory )

			return QuickLaunchNode;

		} // getQuickLaunchItems()

		/// <summary>
		/// Resets the Quick Launch Item's XML to the user's stored quick launch items
		/// </summary>
		public XElement resetQuickLaunchItems( CswPrimaryKey UserId, CswNbtResources CswNbtResources, HttpSessionState Session )
		{
			var Reset = new XElement( "quicklaunch" );
			if( null != UserId )
			{
				_Session[QuickLaunchViews] = null;
				var QuickLaunchService = new CswNbtWebServiceQuickLaunchItems( CswNbtResources, Session );
				Reset = QuickLaunchService.getQuickLaunchItems( UserId );
			}
			return Reset;
		} // ResetQuickLaunchItems()

		/// <summary>
		/// Adds an item to the Session's QuickLaunchViews
		/// </summary>
		public static void addToQuickLaunch( CswNbtView View, HttpSessionState Session )
		{
			if( View.ViewId > 0 || View.SessionViewId > 0 )
			{
				LinkedList<CswNbtQuickLaunchItem> ViewHistoryList = null;
				if( null == Session[QuickLaunchViews] )
				{
					ViewHistoryList = new LinkedList<CswNbtQuickLaunchItem>();
				}
				else
				{
					ViewHistoryList = (LinkedList<CswNbtQuickLaunchItem>) Session[QuickLaunchViews];
				}
				var ThisView = new CswNbtQuickLaunchItem( View.ViewId, View.ViewName, View.ViewMode );

				if( ViewHistoryList.Contains( ThisView ) )
				{
					ViewHistoryList.Remove( ThisView );
				}
				ViewHistoryList.AddFirst( ThisView );
				Session[QuickLaunchViews] = ViewHistoryList;
			}
		}

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
			Int32 ReturnVal = -1;
			if( Ql1 > Ql2 )
			{
				ReturnVal = 1;
			}
			else if( Ql1 < Ql2 )
			{
				ReturnVal = -1;
			}
			else // Ql1 == Ql2
			{
				ReturnVal = 0;
			}
			return ReturnVal;
		}

		/// <summary>
		/// True if first QuickLaunchItem's ItemId is greater than ItemId of 2nd QuickLaunchItem
		/// and if the QuickLaunchItems have non-matching names and types
		/// </summary>
		public static bool operator >( CswNbtQuickLaunchItem Ql1, CswNbtQuickLaunchItem Ql2 )
		{
			bool GreaterThan = false;
			
			// if( ReferenceEquals( Ql1, Ql2 ) ) --still false
			// if( ( (object) Ql1 == null ) && ( (object) Ql2 != null ) ) --still false
			if( ( (object) Ql1 != null ) && ( (object) Ql2 == null ) )
			{
				GreaterThan = true;
			}

			if( null != Ql1 && null != Ql2 )
			{
				if( ( Ql1.ItemId > Ql2.ItemId ) &&
					( Ql1.ItemName != Ql2.ItemName ) &&
					( Ql1.ItemType != Ql2.ItemType ) )
				{
					GreaterThan = true;
				}
			}

			return GreaterThan;
		}
		
		/// <summary>
		/// True if first QuickLaunchItem's ItemId is less than ItemId of 2nd QuickLaunchItem
		/// and if the QuickLaunchItems have non-matching names and types
		/// </summary>
		public static bool operator <( CswNbtQuickLaunchItem Ql1, CswNbtQuickLaunchItem Ql2 )
		{
			bool LessThan = false;

			// if( ReferenceEquals( Ql1, Ql2 ) ) --still false
			// if( ( (object) Ql1 != null ) && ( (object) Ql2 == null ) ) --still false
			if( ( (object) Ql1 == null ) && ( (object) Ql2 != null ) )
			{
				LessThan = true;
			}

			if( null != Ql1 && null != Ql2 )
			{
				if( ( Ql1.ItemId < Ql2.ItemId ) &&
					( Ql1.ItemName != Ql2.ItemName ) &&
					( Ql1.ItemType != Ql2.ItemType ) )
				{
					LessThan = true;
				}
			}

			return LessThan;
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
			if( ReferenceEquals( Ql1, Ql2 ) )
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
			return !( Ql1 == Ql2 );
		}

		/// <summary>
		/// IEquatable implementation: Equals
		/// </summary>
		public override bool Equals( object Obj )
		{
			bool Equals = false;
			if( ( Obj is CswNbtQuickLaunchItem ) )
			{
				Equals = ( this == Obj );
			}
			return Equals;
		}

		/// <summary>
		/// IEquatable implementation: Equals
		/// </summary>
		public bool Equals( CswNbtQuickLaunchItem Obj )
		{
			return this == Obj;
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

