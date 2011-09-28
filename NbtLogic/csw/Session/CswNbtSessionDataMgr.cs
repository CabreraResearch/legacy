using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using Newtonsoft.Json.Linq;

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
        public const string SessionDataColumn_KeepInQuickLaunch = "keepinquicklaunch";

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

        public JObject getQuickLaunchJson()
        {
            JObject ReturnVal = new JObject();

            CswTableSelect SessionDataSelect = _CswNbtResources.makeCswTableSelect( "getQuickLaunchXml_select", "session_data" );
            Collection<OrderByClause> OrderBy = new Collection<OrderByClause>();
            OrderBy.Add( new OrderByClause( SessionDataColumn_PrimaryKey, OrderByType.Descending ) );

            string WhereClause = @"where " + SessionDataColumn_SessionId + "='" + _CswNbtResources.Session.SessionId + "' and "
                                 + SessionDataColumn_QuickLaunch + " = '" + CswConvert.ToDbVal( true ).ToString() + "'";
            DataTable SessionDataTable = SessionDataSelect.getTable( WhereClause, OrderBy );

            Int32 RowCount = 0;
            foreach( DataRow Row in SessionDataTable.Rows )
            {
                bool KeepInQuickLaunch = CswConvert.ToBoolean( Row[SessionDataColumn_KeepInQuickLaunch] );
                if( KeepInQuickLaunch )
                {
                    _addQuickLaunchProp( Row, ReturnVal );
                }
                else if( RowCount < 5 )
                {
                    _addQuickLaunchProp( Row, ReturnVal );
                    RowCount++;
                }
            }

            return ReturnVal;
        } // getQuickLaunchJson()

        private void _addQuickLaunchProp( DataRow Row, JObject ParentObj )
        {
            Int32 ItemId = CswConvert.ToInt32( Row[SessionDataColumn_PrimaryKey] );

            JObject QlObj = new JObject();
            ParentObj["item_" + ItemId] = QlObj;

            Int32 ActionId = CswConvert.ToInt32( Row[SessionDataColumn_ActionId] );
            if( ActionId != Int32.MinValue )
            {
                _addQuickLaunchAction( QlObj, Row[SessionDataColumn_SessionDataType], Row[SessionDataColumn_Name], _CswNbtResources.Actions[ActionId].Name, ItemId, _CswNbtResources.Actions[ActionId].Url );
            }
            else
            {
                _addQuickLaunchView( QlObj, Row[SessionDataColumn_SessionDataType], Row[SessionDataColumn_Name], Row[SessionDataColumn_ViewMode], ItemId );
            }
        }

        private void _addQuickLaunchView( JObject ParentObj, object LaunchType, object Text, object ViewMode, Int32 ItemId )
        {
            ParentObj["launchtype"] = CswConvert.ToString( LaunchType );
            ParentObj["text"] = CswConvert.ToString( Text );
            ParentObj["viewmode"] = CswConvert.ToString( ViewMode );
            ParentObj["itemid"] = new CswNbtSessionDataId( ItemId ).ToString();
        }

		private void _addQuickLaunchAction( JObject ParentObj, object LaunchType, object Text, object ActionName, Int32 ItemId, object ActionUrl )
        {
            ParentObj["launchtype"] = CswConvert.ToString( LaunchType );
            ParentObj["text"] = CswConvert.ToString( Text );
			ParentObj["itemid"] = new CswNbtSessionDataId( ItemId ).ToString();
            ParentObj["actionname"] = CswConvert.ToString( ActionName );
            ParentObj["actionurl"] = CswConvert.ToString( ActionUrl );
        }
        /// <summary>
        /// Save an action to the session data collection.
        /// </summary>
        public CswNbtSessionDataId saveSessionData( CswNbtAction Action, bool IncludeInQuickLaunch, bool KeepInQuickLaunch = false )
        {
            CswTableUpdate SessionViewsUpdate = _CswNbtResources.makeCswTableUpdate( "saveSessionView_update", SessionDataTableName );
            DataTable SessionViewTable = null;
            SessionViewTable = SessionViewsUpdate.getTable( SessionDataColumn_ActionId, Action.ActionId, "where sessionid = '" + SessionId + "'", false );

            DataRow SessionViewRow = _getSessionViewRow( SessionViewTable, Action.Name, CswNbtSessionDataItem.SessionDataType.Action, IncludeInQuickLaunch, KeepInQuickLaunch );
            SessionViewRow[SessionDataColumn_ActionId] = CswConvert.ToDbVal( Action.ActionId );
            SessionViewsUpdate.update( SessionViewTable );

            return new CswNbtSessionDataId( CswConvert.ToInt32( SessionViewRow[SessionDataColumn_PrimaryKey] ) );

        } // saveSessionData(Action)

        /// <summary>
        /// Save a view to the session data collection.  Sets the SessionViewId on the view.
        /// </summary>
        public CswNbtSessionDataId saveSessionData( CswNbtView View, bool IncludeInQuickLaunch, bool ForceNewSessionId = false, bool KeepInQuickLaunch = false )
        {
            CswTableUpdate SessionViewsUpdate = _CswNbtResources.makeCswTableUpdate( "saveSessionView_update", SessionDataTableName );
            DataTable SessionViewTable = null;
            if( View.SessionViewId != null && View.SessionViewId.isSet() )
                SessionViewTable = SessionViewsUpdate.getTable( SessionDataColumn_PrimaryKey, View.SessionViewId.get(), "where sessionid = '" + SessionId + "'", false );
            else if( !ForceNewSessionId && View.ViewId != null && View.ViewId.isSet() )
                SessionViewTable = SessionViewsUpdate.getTable( SessionDataColumn_ViewId, View.ViewId.get(), "where sessionid = '" + SessionId + "'", false );
            else
                SessionViewTable = SessionViewsUpdate.getEmptyTable();

            DataRow SessionViewRow = _getSessionViewRow( SessionViewTable, View.ViewName, CswNbtSessionDataItem.SessionDataType.View, IncludeInQuickLaunch, KeepInQuickLaunch );
            SessionViewRow[SessionDataColumn_ViewId] = CswConvert.ToDbVal( View.ViewId.get() );
            SessionViewRow[SessionDataColumn_ViewMode] = View.ViewMode.ToString();
            SessionViewRow[SessionDataColumn_ViewXml] = View.ToString();
            SessionViewsUpdate.update( SessionViewTable );

            return new CswNbtSessionDataId( CswConvert.ToInt32( SessionViewRow[SessionDataColumn_PrimaryKey] ) );

        } // saveSessionData(View)

        private DataRow _getSessionViewRow( DataTable SessionViewTable, object Name, CswNbtSessionDataItem.SessionDataType DataType, bool IncludeInQuickLaunch, bool KeepInQuickLaunch )
        {
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

            SessionViewRow[SessionDataColumn_Name] = CswConvert.ToString( Name );
            SessionViewRow[SessionDataColumn_SessionId] = SessionId;
            SessionViewRow[SessionDataColumn_SessionDataType] = DataType.ToString();
            SessionViewRow[SessionDataColumn_QuickLaunch] = CswConvert.ToDbVal( IncludeInQuickLaunch );
            SessionViewRow[SessionDataColumn_KeepInQuickLaunch] = CswConvert.ToDbVal( KeepInQuickLaunch );

            return SessionViewRow;
        } // _getSessionViewRow()

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



