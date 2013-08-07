using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Search;

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

        #region Get Session Data

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

        #endregion Get Session Data

        #region QuickLaunch

        public void getQuickLaunchJson( ref ViewSelect.Response.Category Category )
        {
            CswTableSelect SessionDataSelect = _CswNbtResources.makeCswTableSelect( "getQuickLaunchXml_select", "session_data" );
            Collection<OrderByClause> OrderBy = new Collection<OrderByClause>();
            OrderBy.Add( new OrderByClause( SessionDataColumn_PrimaryKey, CswEnumOrderByType.Descending ) );

            string WhereClause = @"where " + SessionDataColumn_SessionId + "='" + _CswNbtResources.Session.SessionId + "' and "
                                 + SessionDataColumn_QuickLaunch + " = '" + CswConvert.ToDbVal( true ).ToString() + "'";
            DataTable SessionDataTable = SessionDataSelect.getTable( WhereClause, OrderBy );

            Int32 RowCount = 0;
            foreach( DataRow Row in SessionDataTable.Rows )
            {
                bool KeepInQuickLaunch = CswConvert.ToBoolean( Row[SessionDataColumn_KeepInQuickLaunch] );
                if( KeepInQuickLaunch )
                {
                    _addQuickLaunchProp( Row, Category );
                }
                else if( RowCount < 5 )
                {
                    _addQuickLaunchProp( Row, Category );
                    RowCount++;
                }
            }
        } // getQuickLaunchJson()

        private void _addQuickLaunchProp( DataRow Row, ViewSelect.Response.Category Category )
        {
            Int32 ItemId = CswConvert.ToInt32( Row[SessionDataColumn_PrimaryKey] );

            CswEnumNbtSessionDataType SessionType = CswConvert.ToString( Row[SessionDataColumn_SessionDataType] );
            string Name = Row[SessionDataColumn_Name].ToString();
            CswNbtSessionDataId SessionDataId = new CswNbtSessionDataId( ItemId );

            if( SessionType == CswEnumNbtSessionDataType.Action )
            {
                Int32 ActionId = CswConvert.ToInt32( Row[SessionDataColumn_ActionId] );
                CswNbtAction Action = _CswNbtResources.Actions[ActionId];
                _addQuickLaunchAction( Category, Name, SessionDataId, Action );
            }
            else if( SessionType == CswEnumNbtSessionDataType.View )
            {
                //Int32 ViewId = CswConvert.ToInt32( Row[SessionDataColumn_ViewId] );
                _addQuickLaunchView( Category, Name, SessionDataId, Row[SessionDataColumn_ViewMode].ToString() );
            }
            else if( SessionType == CswEnumNbtSessionDataType.Search )
            {
                _addQuickLaunchSearch( Category, Name, SessionDataId );
            }
        } // _addQuickLaunchProp()

        private void _addQuickLaunchView( ViewSelect.Response.Category Category, string Text, CswNbtSessionDataId SessionDataId, string ViewMode )
        {
            Category.items.Add(
                new ViewSelect.Response.Item( CswEnumNbtViewItemType.View )
            {
                name = Text,
                itemid = SessionDataId.ToString(),
                iconurl = "Images/view/view" + ViewMode.ToString().ToLower() + ".gif",
                mode = ViewMode
            } );
        }

        private void _addQuickLaunchAction( ViewSelect.Response.Category Category, string Text, CswNbtSessionDataId SessionDataId, CswNbtAction Action )
        {
            Category.items.Add(
                new ViewSelect.Response.Item( CswEnumNbtViewItemType.Action )
                {
                    name = Text,
                    itemid = SessionDataId.ToString(),
                    iconurl = CswNbtMetaDataObjectClass.IconPrefix16 + Action.IconFileName,
                    url = CswConvert.ToString( Action.Url )
                } );
        }

        private void _addQuickLaunchSearch( ViewSelect.Response.Category Category, string Text, CswNbtSessionDataId SessionDataId )
        {
            Category.items.Add(
                new ViewSelect.Response.Item( CswEnumNbtViewItemType.Search )
                {
                    name = Text,
                    itemid = SessionDataId.ToString(),
                    iconurl = CswNbtMetaDataObjectClass.IconPrefix16 + "magglass.png"
                } );
        }

        #endregion QuickLaunch

        #region Save Session Data

        /// <summary>
        /// Save an action to the session data collection.
        /// </summary>
        public CswNbtSessionDataId saveSessionData( CswNbtAction Action, bool IncludeInQuickLaunch, bool KeepInQuickLaunch = false )
        {
            CswTableUpdate SessionViewsUpdate = _CswNbtResources.makeCswTableUpdate( "saveSessionView_update", SessionDataTableName );
            DataTable SessionViewTable = null;
            SessionViewTable = SessionViewsUpdate.getTable( SessionDataColumn_ActionId, Action.ActionId, "where sessionid = '" + SessionId + "'", false );

            DataRow SessionViewRow = _getSessionViewRow( SessionViewTable, Action.DisplayName, CswEnumNbtSessionDataType.Action, IncludeInQuickLaunch && Action.ShowInList, KeepInQuickLaunch );
            SessionViewRow[SessionDataColumn_ActionId] = CswConvert.ToDbVal( Action.ActionId );
            SessionViewsUpdate.update( SessionViewTable );

            return new CswNbtSessionDataId( CswConvert.ToInt32( SessionViewRow[SessionDataColumn_PrimaryKey] ) );

        } // saveSessionData(Action)

        /// <summary>
        /// Save a search to the session data collection.
        /// </summary>
        public CswNbtSessionDataId saveSessionData( CswNbtSearch Search, bool IncludeInQuickLaunch, bool KeepInQuickLaunch = false )
        {
            CswTableUpdate SessionViewsUpdate = _CswNbtResources.makeCswTableUpdate( "saveSessionView_update", SessionDataTableName );
            DataTable SessionViewTable = null;
            if( Search.SessionDataId != null )
            {
                SessionViewTable = SessionViewsUpdate.getTable( SessionDataColumn_PrimaryKey, Search.SessionDataId.get(), "where sessionid = '" + SessionId + "'", false );
            }
            else
            {
                SessionViewTable = SessionViewsUpdate.getEmptyTable();
            }
            DataRow SessionViewRow = _getSessionViewRow( SessionViewTable, Search.Name, CswEnumNbtSessionDataType.Search, IncludeInQuickLaunch, KeepInQuickLaunch );
            //SessionViewRow[SessionDataColumn_SearchId] = CswConvert.ToDbVal( Search.SearchId );
            SessionViewRow[SessionDataColumn_ViewXml] = Search.ToString();
            SessionViewsUpdate.update( SessionViewTable );

            return new CswNbtSessionDataId( CswConvert.ToInt32( SessionViewRow[SessionDataColumn_PrimaryKey] ) );

        } // saveSessionData(Search)

        /// <summary>
        /// Save a view to the session data collection.  Sets the SessionViewId on the view.
        /// </summary>
        public CswNbtSessionDataId saveSessionData( CswNbtView View, bool IncludeInQuickLaunch, bool KeepInQuickLaunch = false, bool UpdateCache = false )
        {
            CswTableUpdate SessionViewsUpdate = _CswNbtResources.makeCswTableUpdate( "saveSessionView_update", SessionDataTableName );
            DataTable SessionViewTable;
            if( View.SessionViewId != null && View.SessionViewId.isSet() ) //Get existing session view by SessionViewId
                SessionViewTable = SessionViewsUpdate.getTable( SessionDataColumn_PrimaryKey, View.SessionViewId.get(), "where sessionid = '" + SessionId + "'", false );
            else if( View.ViewId != null && View.ViewId.isSet() ) //Get existing session view by ViewId
                SessionViewTable = SessionViewsUpdate.getTable( SessionDataColumn_ViewId, View.ViewId.get(), "where sessionid = '" + SessionId + "'", false );
            else //Save new Session View
                SessionViewTable = SessionViewsUpdate.getEmptyTable();
            if( SessionViewTable.Rows.Count == 0 )
            {
                UpdateCache = true;
            }

            DataRow SessionViewRow = _getSessionViewRow( SessionViewTable, View.ViewName, CswEnumNbtSessionDataType.View, IncludeInQuickLaunch, KeepInQuickLaunch );
            if( UpdateCache )//Overwrite
            {
                SessionViewRow[SessionDataColumn_ViewId] = CswConvert.ToDbVal( View.ViewId.get() );
                SessionViewRow[SessionDataColumn_ViewMode] = View.ViewMode.ToString();
                SessionViewRow[SessionDataColumn_ViewXml] = View.ToString();
                SessionViewsUpdate.update( SessionViewTable );
            }

            return new CswNbtSessionDataId( CswConvert.ToInt32( SessionViewRow[SessionDataColumn_PrimaryKey] ) );

        } // saveSessionData(View)

        private DataRow _getSessionViewRow( DataTable SessionViewTable, object Name, CswEnumNbtSessionDataType DataType, bool IncludeInQuickLaunch, bool KeepInQuickLaunch )
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
            if( CswConvert.ToBoolean( SessionViewRow[SessionDataColumn_QuickLaunch] ) == false )
            {
                // Only set false to true, not true to false.  See case 23999.
                SessionViewRow[SessionDataColumn_QuickLaunch] = CswConvert.ToDbVal( IncludeInQuickLaunch );
            }
            SessionViewRow[SessionDataColumn_KeepInQuickLaunch] = CswConvert.ToDbVal( KeepInQuickLaunch );

            return SessionViewRow;
        } // _getSessionViewRow()

        #endregion Save Session Data

        #region Remove Session Data

        /// <summary>
        /// Remove a view from the session view cache
        /// </summary>
        public void removeSessionData( CswNbtView View )
        {
            removeSessionData( View.SessionViewId );

            // Also remove views that match by viewid
            CswTableUpdate SessionDataUpdate = _CswNbtResources.makeCswTableUpdate( "removeSessionData_View_update", SessionDataTableName );
            string WhereClause = @"where " + SessionDataColumn_SessionId + @"='" + _CswNbtResources.Session.SessionId + @"' 
                                     and " + SessionDataColumn_ViewId + @" = '" + View.ViewId.get() + @"'";

            DataTable SessionDataTable = SessionDataUpdate.getTable( WhereClause );
            foreach( DataRow Row in SessionDataTable.Rows )
            {
                Row.Delete();
            }
            SessionDataUpdate.update( SessionDataTable );
        } // removeSessionData(CswNbtView)

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
        } // removeSessionData(CswNbtSessionDataId)


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
                    }//there are session records

                    CswArbitrarySelect SessionNodeSelect = _CswNbtResources.makeCswArbitrarySelect( "removeSessionData_update_nodes",
                                                                                                    "select nodeid from nodes where istemp = 1 and sessionid = :sessionid " );
                    SessionNodeSelect.addParameter( "sessionid", SessionId );

                    DataTable NodesTable = SessionNodeSelect.getTable();
                    if( NodesTable.Rows.Count > 0 )
                    {
                        Collection<CswNbtNode> DoomedNodes = new Collection<CswNbtNode>();
                        foreach( DataRow Row in NodesTable.Rows )
                        {
                            CswPrimaryKey NodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( Row["nodeid"] ) );
                            if( CswTools.IsPrimaryKey( NodeId ) )
                            {
                                CswNbtNode TempNode = _CswNbtResources.Nodes[NodeId];
                                if( null != TempNode )
                                {
                                    DoomedNodes.Add( TempNode );
                                }
                            }
                        }

                        foreach( CswNbtNode DoomedNode in DoomedNodes )
                        {
                            DoomedNode.delete( DeleteAllRequiredRelatedNodes : true, OverridePermissions : true );
                        }

                    }//there are nodes rows

                }//Db resources are initialzied

            }//SessionId is not empty

        } // removeAllSessionData()

        #endregion Remove Session Data

    } // class CswNbtSessionViewMgr

} // namespace ChemSW.Nbt