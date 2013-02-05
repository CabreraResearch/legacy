using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
namespace ChemSW.Nbt
{
    public class CswNbtViewSelect
    {
        private CswNbtResources _CswNbtResources;
        public CswNbtViewSelect( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }


        /// <summary>
        /// Restore a CswNbtViewBase that was saved as a string
        /// </summary>
        /// <param name="ViewAsString">View saved as a string</param>
        public CswNbtView restoreView( string ViewAsString )
        {
            CswNbtView Ret = null;

            if( ViewAsString != string.Empty )
            {
                CswNbtView RelationshipView = new CswNbtView( _CswNbtResources );
                RelationshipView.LoadXml( ViewAsString );
                Ret = RelationshipView;
            }
            return Ret;
        }


        public CswNbtView restoreView( CswNbtViewId NbtViewId )
        {
            CswNbtView ReturnVal = null;
            CswTableSelect ViewsTableSelect = _CswNbtResources.makeCswTableSelect( "restoreView_select", "node_views" );

            if( NbtViewId.isSet() )
            {
                Int32 ViewId = NbtViewId.get();
                if( Int32.MinValue != ViewId )
                {
                    DataTable ViewTable = ViewsTableSelect.getTable( "nodeviewid", ViewId );
                    if( ViewTable.Rows.Count > 0 )
                    {
                        string ViewAsString = ViewTable.Rows[0]["viewxml"].ToString();
                        ReturnVal = restoreView( ViewAsString );
                        ReturnVal.ViewId = NbtViewId;
                        ReturnVal.Visibility = (NbtViewVisibility) ViewTable.Rows[0]["visibility"].ToString();
                        ReturnVal.VisibilityRoleId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( ViewTable.Rows[0]["roleid"] ) );
                        ReturnVal.VisibilityUserId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( ViewTable.Rows[0]["userid"] ) );
                        ReturnVal.Category = ViewTable.Rows[0]["category"].ToString();
                        ReturnVal.ViewName = ViewTable.Rows[0]["viewname"].ToString();
                        ReturnVal.IsDemo = CswConvert.ToBoolean( ViewTable.Rows[0]["isdemo"].ToString() );
                    }
                }
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Attempt to restore view failed.", "CswNbtViewSelect was handed an invalid NbtViewId in restoreView()." );
            }
            return ( ReturnVal );

        }//restoreView()

        public List<CswNbtView> restoreViews( string ViewName )
        {
            return restoreViews( ViewName, NbtViewVisibility.Unknown, Int32.MinValue );
        }
        public List<CswNbtView> restoreViews( string ViewName, NbtViewVisibility Visibility, Int32 VisibilityId )
        {
            List<CswNbtView> ReturnVal = new List<CswNbtView>();

            CswTableSelect ViewSelect = _CswNbtResources.makeCswTableSelect( "CswNbtViewSelect_restoreViews_select", "node_views" );
            CswCommaDelimitedString SelectCols = new CswCommaDelimitedString()
                                                     {
                                                         "nodeviewid", 
                                                         "viewname"
                                                     };
            ViewName = ViewName.ToLower().Trim();
            if( ViewName.Length > 200 )
            {
                ViewName = ViewName.Substring( 0, 200 );
            }

            string WhereClause = string.Empty;
            if( Visibility != NbtViewVisibility.Unknown )
            {
                WhereClause = "where visibility='" + Visibility.ToString() + "'";
            }
            else if( Visibility == NbtViewVisibility.Role )
            {
                if( Int32.MinValue != VisibilityId )
                {
                    WhereClause += " and roleid='" + VisibilityId.ToString() + "'";
                }
            }
            else if( Visibility == NbtViewVisibility.User )
            {
                if( Int32.MinValue != VisibilityId )
                {
                    WhereClause += " and userid='" + VisibilityId.ToString() + "'";
                }
            }

            DataTable ViewTable = ViewSelect.getTable( SelectCols, string.Empty, Int32.MinValue, WhereClause, false );
            foreach( DataRow CurrentRow in ViewTable.Rows )
            {
                string CurrentViewName = CswConvert.ToString( CurrentRow["viewname"] ).ToLower().Trim();
                if( ViewName == CurrentViewName ||
                    CurrentViewName.Contains( ViewName ) )
                {
                    ReturnVal.Add( _CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( CurrentRow["nodeviewid"] ) ) ) );
                }
            }

            return ( ReturnVal );

        }//restoreViews()

        /// <summary>
        /// Get a CswNbtView from the session view collection
        /// </summary>
        public CswNbtView getSessionView( CswNbtSessionDataId SessionViewId )
        {
            if( SessionViewId == null )
            {
                throw new CswDniException( "CswNbtViewSelect.getSessionView(): SessionViewId is null" );
            }

            CswNbtSessionDataItem SessionDataItem = _CswNbtResources.SessionDataMgr.getSessionDataItem( SessionViewId );

            if( null == SessionDataItem ||
                SessionDataItem.DataType != CswNbtSessionDataItem.SessionDataType.View )
            {
                throw new CswDniException( "CswNbtViewSelect.getSessionView(): SessionViewId (" + SessionViewId.get() + ") is not a view" );
            }

            return SessionDataItem.View;

        } // getSessionView()

        

        /// <summary>
        /// Save a view to the session view collection.  Sets the SessionViewId on the view.
        /// </summary>
        public CswNbtSessionDataId saveSessionView( CswNbtView View, bool IncludeInQuickLaunch, bool KeepInQuickLaunch )
        {
            return _CswNbtResources.SessionDataMgr.saveSessionData( View, IncludeInQuickLaunch, false, KeepInQuickLaunch );
        } // saveSessionView()

        /// <summary>
        /// Remove a view from the session view cache
        /// </summary>
        public void removeSessionView( CswNbtView View )
        {
            _CswNbtResources.SessionDataMgr.removeSessionData( View );
        }

        /// <summary>
        /// Remove a view from the session view cache
        /// </summary>
        public void removeSessionView( CswNbtSessionDataId SessionViewId )
        {
            _CswNbtResources.SessionDataMgr.removeSessionData( SessionViewId );
        } // removeSessionView()

        public void deleteViewByName( string ViewName )
        {
            CswTableUpdate ViewDelete = _CswNbtResources.makeCswTableUpdate( "CswNbtViewDelete_" + ViewName, "node_views" );
            DataTable ViewTable = ViewDelete.getTable( "where viewname = '" + ViewName + "'" );
            foreach( DataRow Row in ViewTable.Rows )
            {
                Row.Delete();
            }
            ViewDelete.update( ViewTable );
        }

        /// <summary>
        /// Get a DataTable with a single view, by name and visibility
        /// </summary>
        public DataTable getView( string ViewName, NbtViewVisibility Visibility, CswPrimaryKey VisibilityRoleId, CswPrimaryKey VisibilityUserId )
        {
            CswTableSelect ViewsTable = _CswNbtResources.makeCswTableSelect( "CswNbtViewSelect_viewExists_select", "node_views" );
            string WhereClause = "where viewname = '" + ViewName + "'";
            if( Visibility == NbtViewVisibility.Role )
            {
                WhereClause += " and visibility = 'Role' and roleid = " + VisibilityRoleId.PrimaryKey.ToString();
            }
            else if( Visibility == NbtViewVisibility.User )
            {
                WhereClause += " and visibility = 'User' and userid = " + VisibilityUserId.PrimaryKey.ToString();
            }
            else
            {
                WhereClause += " and visibility = '" + Visibility.ToString() + "'";
            }
            return ViewsTable.getTable( WhereClause );
        } // getViews()

        /// <summary>
        /// Get a DataTable of all views in the database
        /// </summary>
        public DataTable getAllViews()
        {
            CswStaticSelect ViewsSelect = _CswNbtResources.makeCswStaticSelect( "CswNbtViewSelect.getAllViews_select", "getAllViewInfo" );
            return ViewsSelect.getTable();
        }

        /// <summary>
        /// Get a DataTable of all enabled views in the database
        /// </summary>
        public DataTable getAllEnabledViews()
        {
            Dictionary<CswNbtViewId, CswNbtView> AllEnabledViews = new Dictionary<CswNbtViewId, CswNbtView>();
            return getAllEnabledViews( out AllEnabledViews );
        }

        /// <summary>
        /// Get a DataTable of all enabled views in the database
        /// </summary>
        public DataTable getAllEnabledViews(out Dictionary<CswNbtViewId, CswNbtView> AllEnabledViews )
        {
            CswStaticSelect ViewsSelect = _CswNbtResources.makeCswStaticSelect( "CswNbtViewSelect.getAllViews_select", "getAllViewInfo" );
            Dictionary<CswNbtViewId, CswNbtView> OutEnabledViews = new Dictionary<CswNbtViewId, CswNbtView>();
            CswNbtNode ChemSwAdminUser = _CswNbtResources.Nodes.makeUserNodeFromUsername( CswNbtObjClassUser.ChemSWAdminUsername );
            CswNbtNode ChemSwAdminRole = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
            bool ExcludeCswAdmin = ( _CswNbtResources.CurrentNbtUser.Username != CswNbtObjClassUser.ChemSWAdminUsername ||
                                     _CswNbtResources.CurrentNbtUser.Rolename != CswNbtObjClassRole.ChemSWAdminRoleName );

            DataTable AllViews = ViewsSelect.getTable();
            Collection<DataRow> DoomedRows = new Collection<DataRow>();
            foreach( DataRow Row in AllViews.Rows )
            {
                CswNbtViewId ViewId = new CswNbtViewId( CswConvert.ToInt32( Row["nodeviewid"] ) );
                CswNbtView View = _CswNbtResources.ViewSelect.restoreView( ViewId );
                if( false == View.IsFullyEnabled() ||
                    ( ExcludeCswAdmin &&
                      ( ( View.Visibility == NbtViewVisibility.Role &&
                          View.VisibilityRoleId == ChemSwAdminRole.NodeId ) ||
                        ( View.Visibility == NbtViewVisibility.User &&
                          View.VisibilityUserId == ChemSwAdminUser.NodeId ) ) ) )
                {
                    DoomedRows.Add( Row );
                }
                else
                {
                    OutEnabledViews.Add( ViewId, View );
                }
            }
            foreach( DataRow DoomedRow in DoomedRows )
            {
                DoomedRow.Delete();
            }
            AllEnabledViews = OutEnabledViews;
            AllViews.AcceptChanges();
            return AllViews;
        }

        /// <summary>
        /// Get a DataTable of all views visible to the current user
        /// </summary>
        public Dictionary<CswNbtViewId, CswNbtView> getVisibleViews( NbtViewRenderingMode ViewRenderingMode )
        {
            return getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser, false, false, false, ViewRenderingMode );
        }

        /// <summary>
        /// Get a DataTable of all views visible to the current user
        /// </summary>
        public Dictionary<CswNbtViewId, CswNbtView> getVisibleViews( bool IncludeEmptyViews )
        {
            return getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser, IncludeEmptyViews, false, false, NbtViewRenderingMode.Any );
        }

        /// <summary>
        /// Get a DataTable of all views visible to the provided user
        /// </summary>
        public Dictionary<CswNbtViewId, CswNbtView> getVisibleViews( ICswNbtUser User, bool IncludeEmptyViews, CswCommaDelimitedString LimitToViews = null )
        {
            return getVisibleViews( string.Empty, User, IncludeEmptyViews, false, false, NbtViewRenderingMode.Any, LimitToViews );
        }

        /// <summary>
        /// Get a DataTable of all views visible to the current user
        /// </summary>
        public Dictionary<CswNbtViewId, CswNbtView> getVisibleViews( string OrderBy, bool IncludeEmptyViews )
        {
            return getVisibleViews( OrderBy, _CswNbtResources.CurrentNbtUser, IncludeEmptyViews, false, false, NbtViewRenderingMode.Any );
        }

        /// <summary>
        /// Reset the cached visibile views table
        /// </summary>
        public void clearCache()
        {
            //_LastVisibleViews = null;
        }

        /// <summary>
        /// Get a Collection of all views visible to the current user
        /// </summary>
        public Dictionary<CswNbtViewId, CswNbtView> getVisibleViews( string OrderBy, ICswNbtUser User, bool IncludeEmptyViews, bool MobileOnly, bool SearchableOnly, NbtViewRenderingMode ViewRenderingMode, CswCommaDelimitedString LimitToViews = null )
        {
            DataTable ViewsTable = null;
            return getVisibleViews( OrderBy, User, IncludeEmptyViews, MobileOnly, SearchableOnly, ViewRenderingMode, out ViewsTable, LimitToViews );
        }

        /// <summary>
        /// Get a Collection of all views visible to the current user, and the DataTable
        /// </summary>
        public Dictionary<CswNbtViewId, CswNbtView> getVisibleViews( string OrderBy, ICswNbtUser User, bool IncludeEmptyViews, bool MobileOnly, bool SearchableOnly, NbtViewRenderingMode ViewRenderingMode, out DataTable ViewsTable, CswCommaDelimitedString LimitToViews = null )
        {
            CswTimer VisibleViewsTimer = new CswTimer();
            ViewsTable = null;

            Dictionary<CswNbtViewId, CswNbtView> Ret = new Dictionary<CswNbtViewId, CswNbtView>();
            if( null == LimitToViews || LimitToViews.Count > 0 )
            {
                CswStaticSelect ViewsSelect = _CswNbtResources.makeCswStaticSelect( "getVisibleViews_select", "getVisibleViewInfo" );
                ViewsSelect.S4Parameters.Add( "getroleid", new CswStaticParam( "getroleid", User.RoleId.PrimaryKey.ToString() ) );
                ViewsSelect.S4Parameters.Add( "getuserid", new CswStaticParam( "getuserid", User.UserId.PrimaryKey.ToString() ) );
                string AddClause = " ";
                if( MobileOnly )
                {
                    AddClause += "and formobile = '" + CswConvert.ToDbVal( true ) + "'";
                }
                if( ViewRenderingMode != NbtViewRenderingMode.Any )
                {
                    AddClause += "and viewmode = '" + ViewRenderingMode.ToString() + "'";
                }
                if( null != LimitToViews )
                {
                    AddClause += "and nodeviewid in (" + LimitToViews.ToString() + ")";
                }
                ViewsSelect.S4Parameters.Add( "addclause", new CswStaticParam( "addclause", AddClause, true ) );
                if( OrderBy != string.Empty )
                {
                    ViewsSelect.S4Parameters.Add( "orderbyclause", new CswStaticParam( "orderbyclause", OrderBy ) );
                }
                else
                {
                    ViewsSelect.S4Parameters.Add( "orderbyclause", new CswStaticParam( "orderbyclause", "lower(v.viewname)" ) );
                }
                ViewsTable = ViewsSelect.getTable();

                _CswNbtResources.logTimerResult( "CswNbtView.getVisibleViews() data fetched", VisibleViewsTimer.ElapsedDurationInSecondsAsString );

                Collection<DataRow> RowsToRemove = new Collection<DataRow>();
                foreach( DataRow Row in ViewsTable.Rows )
                {
                    CswNbtView ThisView = new CswNbtView( _CswNbtResources );
                    ThisView.LoadXml( Row["viewxml"].ToString() );

                    if( isVisible( ThisView, User, IncludeEmptyViews, SearchableOnly ) )
                    {
                        Ret.Add( ThisView.ViewId, ThisView );
                    }
                    else
                    {
                        RowsToRemove.Add( Row );
                    }


                } // foreach( DataRow Row in ViewsTable.Rows )
                foreach( DataRow Row in RowsToRemove )
                {
                    ViewsTable.Rows.Remove( Row );
                }
            } // if( null == LimitToViews || LimitToViews.Count > 0 )

            _CswNbtResources.logTimerResult( "CswNbtView.getVisibleViews() finished", VisibleViewsTimer.ElapsedDurationInSecondsAsString );

            return Ret;
        }

        /// <summary>
        /// Checks to see if a view is visible to a given user
        /// </summary>
        public bool isVisible( CswNbtView View, ICswNbtUser User, bool IncludeEmptyViews, bool SearchableOnly )
        {
            return ((View.Root.ChildRelationships.Count > 0 &&
                     (
                         View.Root.ChildRelationships.Any(R => R.SecondType != NbtViewRelatedIdType.NodeTypeId ||
                                                               _CswNbtResources.Permit.canNodeType(
                                                                   CswNbtPermit.NodeTypePermission.View,
                                                                   _CswNbtResources.MetaData.getNodeType(R.SecondId),
                                                                   User) ||
                                                               _CswNbtResources.Permit.canAnyTab(
                                                                   CswNbtPermit.NodeTypePermission.View,
                                                                   _CswNbtResources.MetaData.getNodeType(R.SecondId),
                                                                   User)))
                    ) || IncludeEmptyViews) &&
                   View.IsFullyEnabled() &&
                   (IncludeEmptyViews || View.ViewMode != NbtViewRenderingMode.Grid || null != View.findFirstProperty()) &&
                   (!SearchableOnly || View.IsSearchable());
        }

        /// <summary>
        /// Get all views with visibility set to the current user
        /// </summary>
        public DataTable getUserViews()
        {
            Dictionary<CswNbtViewId, CswNbtView> AllEnabledViews = new Dictionary<CswNbtViewId, CswNbtView>();
            return getUserViews( out AllEnabledViews );
        }

        /// <summary>
        /// Get all views with visibility set to the current user
        /// </summary>
        public DataTable getUserViews( out Dictionary<CswNbtViewId, CswNbtView> AllEnabledViews )
        {
            DataTable Ret = null;
            Dictionary<CswNbtViewId, CswNbtView> OutEnabledViews = new Dictionary<CswNbtViewId, CswNbtView>();
            CswStaticSelect ViewsSelect = _CswNbtResources.makeCswStaticSelect( "getUserViews_select", "getUserViewInfo" );
            ViewsSelect.S4Parameters.Add( "getuserid", new CswStaticParam( "getuserid", _CswNbtResources.CurrentUser.UserId.PrimaryKey.ToString() ) );
            Ret = ViewsSelect.getTable();

            foreach( DataRow Row in Ret.Rows )
            {
                CswNbtView ThisView = new CswNbtView( _CswNbtResources );
                ThisView.LoadXml( Row["viewxml"].ToString() );

                OutEnabledViews.Add( ThisView.ViewId, ThisView );
            }
            AllEnabledViews = OutEnabledViews;

            return Ret;
        }
    }
}
