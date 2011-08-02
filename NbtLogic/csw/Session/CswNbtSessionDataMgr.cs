using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Handles interactions with the session view collection
    /// for searches and quick launch
    /// </summary>
    public class CswNbtSessionDataMgr
    {
        public const string SessionDataTableName = "session_data";
        public const string SessionDataColumn_PrimaryKey = "sessiondataid";
        public const string SessionDataColumn_SessionId = "sessionid";
        public const string SessionDataColumn_SessionDataType = "sessiondatatype";
        public const string SessionDataColumn_Name = "name";
        public const string SessionDataColumn_ActionId = "actionid";
        public const string SessionDataColumn_ViewId = "viewid";
        public const string SessionDataColumn_ViewMode = "viewmode";
        public const string SessionDataColumn_ViewXml = "viewxml";
        public const string SessionDataColumn_QuickLaunch = "quicklaunch";

        private CswNbtResources _CswNbtResources;

        public CswNbtSessionDataMgr( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
        }

        private string SessionId
        {
            get
            {
                return _CswNbtResources.Session.SessionId;
            }
        }

        /// <summary>
        /// Retrieve a session data item
        /// </summary>
        public CswNbtSessionDataItem getSessionDataItem( CswNbtSessionDataId SessionDataId )
        {
            CswTableSelect SessionDataSelect = _CswNbtResources.makeCswTableSelect( "getSessionDataItem_select", "session_data" );
            DataTable SessionDataTable = SessionDataSelect.getTable( "sessiondataid", SessionDataId.get() );
            CswNbtSessionDataItem ret = null;
            if( SessionDataTable.Rows.Count > 0 )
            {
                ret = new CswNbtSessionDataItem( _CswNbtResources, SessionDataTable.Rows[0] );
            }
            return ret;
        }

        public XElement getQuickLaunchXml( CswCommaDelimitedString UserQuickLaunchViews, CswCommaDelimitedString UserQuickLaunchActions )
        {
            XElement Root = new XElement( "items" );

            CswTableSelect SessionDataSelect = _CswNbtResources.makeCswTableSelect( "getQuickLaunchXml_select", "session_data" );
            Collection<OrderByClause> OrderBy = new Collection<OrderByClause>();
            OrderBy.Add( new OrderByClause( SessionDataColumn_PrimaryKey, OrderByType.Descending ) );

            string WhereClause = @"where " + SessionDataColumn_SessionId + "='" + _CswNbtResources.Session.SessionId + "' and "
                                 + SessionDataColumn_QuickLaunch + " = '" + CswConvert.ToDbVal( true ).ToString() + "'";
            DataTable SessionDataTable = SessionDataSelect.getTable( WhereClause, OrderBy );

            Collection<DataRow> RowsToProcess = new Collection<DataRow>();
            Int32 RowCount = 0;
            foreach( DataRow Row in SessionDataTable.Rows )
            {
                string ActionId = CswConvert.ToString( Row[SessionDataColumn_ActionId] );
                string ViewId = CswConvert.ToString( Row[SessionDataColumn_ViewId] );
                if( UserQuickLaunchViews.Contains( ViewId ) || UserQuickLaunchActions.Contains( ActionId ) )
                {
                    RowsToProcess.Add( Row );
                }
                else if( RowCount < 5 )
                {
                    RowsToProcess.Add( Row );
                    RowCount++;
                }
            }

            foreach( DataRow Row in RowsToProcess )
            {
                XElement ThisItem = new XElement( "item" );
                Int32 ItemId = CswConvert.ToInt32( Row[SessionDataColumn_PrimaryKey] );
                ThisItem.SetAttributeValue( "launchtype", Row[SessionDataColumn_SessionDataType].ToString() );
                ThisItem.SetAttributeValue( "text", Row[SessionDataColumn_Name].ToString() );
                ThisItem.SetAttributeValue( "viewmode", Row[SessionDataColumn_ViewMode].ToString() );
                ThisItem.SetAttributeValue( "itemid", new CswNbtSessionDataId( ItemId ).ToString() );

                Int32 ActionId = CswConvert.ToInt32( Row[SessionDataColumn_ActionId] );
                if( ActionId != Int32.MinValue )
                {
                    ThisItem.SetAttributeValue( "actionname", _CswNbtResources.Actions[ActionId].Name.ToString() );
                    ThisItem.SetAttributeValue( "actionurl", _CswNbtResources.Actions[ActionId].Url );
                }
                Root.Add( ThisItem );
            }
            return Root;
        } // getQuickLaunchXml()

        /// <summary>
        /// Save an action to the session data collection.
        /// </summary>
        public CswNbtSessionDataId saveSessionData( CswNbtAction Action, bool IncludeInQuickLaunch )
        {
            CswTableUpdate SessionViewsUpdate = _CswNbtResources.makeCswTableUpdate( "saveSessionView_update", SessionDataTableName );
            DataTable SessionViewTable = null;
            SessionViewTable = SessionViewsUpdate.getTable( SessionDataColumn_ActionId, Action.ActionId, "where sessionid = '" + SessionId + "'", false );

            DataRow SessionViewRow = null;
            if( SessionViewTable.Rows.Count > 0 )
            {
                SessionViewRow = SessionViewTable.Rows[0];
            }
            else
            {
                SessionViewRow = SessionViewTable.NewRow();
                SessionViewTable.Rows.Add( SessionViewRow );
            }

            SessionViewRow[SessionDataColumn_Name] = Action.Name.ToString();
            SessionViewRow[SessionDataColumn_SessionId] = SessionId;
            SessionViewRow[SessionDataColumn_SessionDataType] = CswNbtSessionDataItem.SessionDataType.Action.ToString();
            SessionViewRow[SessionDataColumn_ActionId] = CswConvert.ToDbVal( Action.ActionId );
            SessionViewRow[SessionDataColumn_QuickLaunch] = CswConvert.ToDbVal( IncludeInQuickLaunch );
            SessionViewsUpdate.update( SessionViewTable );

            return new CswNbtSessionDataId( CswConvert.ToInt32( SessionViewRow[SessionDataColumn_PrimaryKey] ) );

        } // saveSessionData(Action)

        /// <summary>
        /// Save a view to the session data collection.  Sets the SessionViewId on the view.
        /// </summary>
        public CswNbtSessionDataId saveSessionData( CswNbtView View, bool IncludeInQuickLaunch, bool ForceNewSessionId = false )
        {
            CswTableUpdate SessionViewsUpdate = _CswNbtResources.makeCswTableUpdate( "saveSessionView_update", SessionDataTableName );
            DataTable SessionViewTable = null;
            if( View.SessionViewId != null && View.SessionViewId.isSet() )
                SessionViewTable = SessionViewsUpdate.getTable( SessionDataColumn_PrimaryKey, View.SessionViewId.get(), "where sessionid = '" + SessionId + "'", false );
            else if( !ForceNewSessionId && View.ViewId != null && View.ViewId.isSet() )
                SessionViewTable = SessionViewsUpdate.getTable( SessionDataColumn_ViewId, View.ViewId.get(), "where sessionid = '" + SessionId + "'", false );
            else
                SessionViewTable = SessionViewsUpdate.getEmptyTable();

            DataRow SessionViewRow = null;
            if( SessionViewTable.Rows.Count > 0 )
            {
                SessionViewRow = SessionViewTable.Rows[0];
            }
            else
            {
                SessionViewRow = SessionViewTable.NewRow();
                SessionViewTable.Rows.Add( SessionViewRow );
            }

            SessionViewRow[SessionDataColumn_Name] = View.ViewName;
            SessionViewRow[SessionDataColumn_ViewId] = CswConvert.ToDbVal( View.ViewId.get() );
            SessionViewRow[SessionDataColumn_ViewMode] = View.ViewMode.ToString();
            SessionViewRow[SessionDataColumn_ViewXml] = View.ToString();
            SessionViewRow[SessionDataColumn_SessionId] = SessionId;
            SessionViewRow[SessionDataColumn_SessionDataType] = CswNbtSessionDataItem.SessionDataType.View.ToString();
            SessionViewRow[SessionDataColumn_QuickLaunch] = CswConvert.ToDbVal( IncludeInQuickLaunch );
            SessionViewsUpdate.update( SessionViewTable );

            return new CswNbtSessionDataId( CswConvert.ToInt32( SessionViewRow[SessionDataColumn_PrimaryKey] ) );

        } // saveSessionData(View)

        /// <summary>
        /// Remove a view from the session view cache
        /// </summary>
        public void removeSessionData( CswNbtView View )
        {
            removeSessionData( View.SessionViewId );
        }

        /// <summary>
        /// Remove a view from the session view cache
        /// </summary>
        public void removeSessionData( CswNbtSessionDataId SessionDataId )
        {
            if( SessionDataId != null )
            {
                CswTableUpdate SessionDataUpdate = _CswNbtResources.makeCswTableUpdate( "removeSessionData_update", SessionDataTableName );
                DataTable SessionDataTable = SessionDataUpdate.getTable( SessionDataColumn_PrimaryKey, SessionDataId.get() );
                //DataRow SessionDataRow = null;
                if( SessionDataTable.Rows.Count > 0 )
                {
                    SessionDataTable.Rows[0].Delete();
                    SessionDataUpdate.update( SessionDataTable );
                }
            }
        } // removeSessionData()

        /// <summary>
        /// Remove all data for a given session
        /// </summary>
        public void removeAllSessionData( string SessionId )
        {
            if( SessionId != string.Empty )
            {
				if( _CswNbtResources.IsInitializedForDbAccess )
				{
					CswTableUpdate SessionDataUpdate = _CswNbtResources.makeCswTableUpdate( "removeSessionData_update", SessionDataTableName );
					DataTable SessionDataTable = SessionDataUpdate.getTable( "where " + SessionDataColumn_SessionId + " = '" + SessionId + "'" );
					if( SessionDataTable.Rows.Count > 0 )
					{
						Collection<DataRow> DoomedRows = new Collection<DataRow>();
						foreach( DataRow Row in SessionDataTable.Rows )
							DoomedRows.Add( Row );
						foreach( DataRow Row in DoomedRows )
							Row.Delete();
						SessionDataUpdate.update( SessionDataTable );
					}
				}
            }
        } // removeSessionData()



    } // class CswNbtSessionViewMgr


} // namespace ChemSW.Nbt



