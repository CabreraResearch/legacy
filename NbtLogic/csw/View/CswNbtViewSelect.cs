using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
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
        /// <param name="CswNbtResources">A CswNbtResources object</param>
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
            // try cache first
            CswNbtView ReturnVal = null; // CswNbtResources.ViewCache.getView( ViewId );
            //if( ReturnVal == null )
            //{
            CswTableSelect ViewsTableSelect = _CswNbtResources.makeCswTableSelect( "restoreView_select", "node_views" );
            Int32 ViewId = NbtViewId.get();
            if( NbtViewId.isSet() )
            {
                DataTable ViewTable = ViewsTableSelect.getTable( "nodeviewid", ViewId );
                if( ViewTable.Rows.Count > 0 )
                {
                    string ViewAsString = ViewTable.Rows[0]["viewxml"].ToString();
                    ReturnVal = restoreView( ViewAsString );
                    ReturnVal.ViewId = NbtViewId; // BZ 8068

                    // Override XML values with values from row
                    ReturnVal.Visibility = (NbtViewVisibility) Enum.Parse( typeof( NbtViewVisibility ), ViewTable.Rows[0]["visibility"].ToString() );
                    ReturnVal.VisibilityRoleId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( ViewTable.Rows[0]["roleid"] ) );
                    ReturnVal.VisibilityUserId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( ViewTable.Rows[0]["userid"] ) );
                    ReturnVal.Category = ViewTable.Rows[0]["category"].ToString();
                    ReturnVal.ViewName = ViewTable.Rows[0]["viewname"].ToString();
                }
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Attempt to restore view failed.", "CswNbtViewSelect was handed an invalid NbtViewId in restoreView()." );
            }
            //}
            return ( ReturnVal );

        }//restoreView()


        ///// <summary>
        ///// Get a DataTable with a single view, by primary key
        ///// </summary>
        //public CswNbtView getView( Int32 ViewId )
        //{
        //    return CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
        //    //CswStaticSelect ViewsSelect = _CswNbtResources.makeCswStaticSelect( "CswNbtViewSelect.getView", "getViewInfo" );
        //    //ViewsSelect.S4Parameters.Add( "getviewid", ViewId.ToString() );
        //    //return ViewsSelect.getTable();
        //}

        /// <summary>
        /// Get a CswNbtView from the session view collection
        /// </summary>
        public CswNbtView getSessionView( CswNbtSessionDataId SessionViewId )
        {
            if( SessionViewId == null )
                throw new CswDniException( "CswNbtViewSelect.getSessionView(): SessionViewId is null" );

            CswNbtSessionDataItem SessionDataItem = _CswNbtResources.SessionDataMgr.getSessionDataItem( SessionViewId );
            if( SessionDataItem.DataType == CswNbtSessionDataItem.SessionDataType.View )
            {
                return SessionDataItem.View;
            }
            else
            {
                throw new CswDniException( "CswNbtViewSelect.getSessionView(): SessionViewId (" + SessionViewId.get() + ") is not a view" );
            }
        } // getSessionView()

        /// <summary>
        /// Save a view to the session view collection.  Sets the SessionViewId on the view.
        /// </summary>
        public CswNbtSessionDataId saveSessionView( CswNbtView View, bool IncludeInQuickLaunch )
        {
            return _CswNbtResources.SessionDataMgr.saveSessionData( View, IncludeInQuickLaunch );
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

        /// <summary>
        /// Get a DataTable with a single view, by name and visibility
        /// </summary>
        public DataTable getView( string ViewName, NbtViewVisibility Visibility, CswPrimaryKey VisibilityRoleId, CswPrimaryKey VisibilityUserId )
        {
            CswTableSelect ViewsTable = _CswNbtResources.makeCswTableSelect( "CswNbtViewSelect_viewExists_select", "node_views" );
            string WhereClause = "where viewname = '" + ViewName + "'";
            switch( Visibility )
            {
                case NbtViewVisibility.Global:
                    //Globally unique name
                    //WhereClause += " and visibility = 'Global'";
                    break;
                case NbtViewVisibility.Role:
                    WhereClause += " and visibility = 'Role' and roleid = " + VisibilityRoleId.PrimaryKey.ToString();
                    break;
                case NbtViewVisibility.User:
                    WhereClause += " and visibility = 'User' and userid = " + VisibilityUserId.PrimaryKey.ToString();
                    break;
                case NbtViewVisibility.Property:
                    WhereClause += " and visibility = 'Property'";  // This will probably return more than one match
                    break;
            }
            return ViewsTable.getTable( WhereClause );
        } // getViews()

        ///// <summary>
        ///// Returns whether a view exists with a given name
        ///// </summary>
        //public bool viewExists( string ViewName, bool IncludePropertyViews )
        //{
        //    DataTable ViewTable = getViews( ViewName, IncludePropertyViews );
        //    return ( ViewTable.Rows.Count > 0 );
        //}

        /// <summary>
        /// Get a DataTable of all views in the database
        /// </summary>
        public DataTable getAllViews()
        {
            CswStaticSelect ViewsSelect = _CswNbtResources.makeCswStaticSelect( "CswNbtViewSelect.getAllViews_select", "getAllViewInfo" );
            return ViewsSelect.getTable();
        }

        /// <summary>
        /// Get a DataTable of all views visible to the current user
        /// </summary>
        public Collection<CswNbtView> getVisibleViews( NbtViewRenderingMode ViewRenderingMode )
        {
            return getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser, false, false, false, ViewRenderingMode );
        }

        /// <summary>
        /// Get a DataTable of all views visible to the current user
        /// </summary>
        public Collection<CswNbtView> getVisibleViews( bool IncludeEmptyViews )
        {
            return getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser, IncludeEmptyViews, false, false, NbtViewRenderingMode.Any );
        }

        /// <summary>
        /// Get a DataTable of all views visible to the provided user
        /// </summary>
        public Collection<CswNbtView> getVisibleViews( ICswNbtUser User, bool IncludeEmptyViews, CswCommaDelimitedString LimitToViews = null )
        {
            return getVisibleViews( string.Empty, User, IncludeEmptyViews, false, false, NbtViewRenderingMode.Any, LimitToViews );
        }

        /// <summary>
        /// Get a DataTable of all views visible to the current user
        /// </summary>
        public Collection<CswNbtView> getVisibleViews( string OrderBy, bool IncludeEmptyViews )
        {
            return getVisibleViews( OrderBy, _CswNbtResources.CurrentNbtUser, IncludeEmptyViews, false, false, NbtViewRenderingMode.Any );
        }


        //private DataTable _LastVisibleViews = null;
        private string _LastOrderBy = string.Empty;
        private ICswNbtUser _LastUser = null;
        private bool _LastIncludeEmptyViews = false;

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
        public Collection<CswNbtView> getVisibleViews( string OrderBy, ICswNbtUser User, bool IncludeEmptyViews, bool MobileOnly, bool SearchableOnly, NbtViewRenderingMode ViewRenderingMode, CswCommaDelimitedString LimitToViews = null )
        {
            CswTimer VisibleViewsTimer = new CswTimer();

            DataTable ViewsTable = null;
            Collection<CswNbtView> VisibleViews = new Collection<CswNbtView>();
            //if( _LastVisibleViews != null &&
            //    _LastOrderBy == OrderBy &&
            //    _LastUser == User &&
            //    _LastIncludeEmptyViews == IncludeEmptyViews )
            //{
            //    ViewsTable = _LastVisibleViews;
            //}
            //else
            //{
            CswStaticSelect ViewsSelect = _CswNbtResources.makeCswStaticSelect( "getVisibleViews_select", "getVisibleViewInfo" );
            ViewsSelect.S4Parameters.Add( "getroleid", User.RoleId.PrimaryKey.ToString() );
            ViewsSelect.S4Parameters.Add( "getuserid", User.UserId.PrimaryKey.ToString() );
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
            ViewsSelect.S4Parameters.Add( "addclause", AddClause );
            if( OrderBy != string.Empty )
                ViewsSelect.S4Parameters.Add( "orderbyclause", OrderBy );
            else
                ViewsSelect.S4Parameters.Add( "orderbyclause", "lower(v.viewname)" );
            ViewsTable = ViewsSelect.getTable();

            _CswNbtResources.logTimerResult( "CswNbtView.getVisibleViews() data fetched", VisibleViewsTimer.ElapsedDurationInSecondsAsString );
            //}

            // BZ 7074 - Make sure the user has permissions to at least one root node
            foreach( DataRow Row in ViewsTable.Rows )
            {
                CswNbtView ThisView = new CswNbtView( _CswNbtResources );
                ThisView.LoadXml( Row["viewxml"].ToString() );

                if( ( ( ThisView.Root.ChildRelationships.Count > 0 &&
                            ( ThisView.Root.ChildRelationships.Where( R => R.SecondType != CswNbtViewRelationship.RelatedIdType.NodeTypeId ||
                                                                    _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, _CswNbtResources.MetaData.getNodeType( R.SecondId ), true, null, User ) ).Count() > 0 )
                        ) || IncludeEmptyViews ) &&
                    ThisView.IsFullyEnabled() &&
                    ( IncludeEmptyViews || ThisView.ViewMode != NbtViewRenderingMode.Grid || null != ThisView.findFirstProperty() ) &&
                    ( !SearchableOnly || ThisView.IsSearchable() ) )
                {
                    VisibleViews.Add( ThisView );
                }
            } // foreach( DataRow Row in ViewsTable.Rows )

            _LastIncludeEmptyViews = IncludeEmptyViews;
            _LastOrderBy = OrderBy;
            _LastUser = User;
            //_LastVisibleViews = ViewsTable;

            _CswNbtResources.logTimerResult( "CswNbtView.getVisibleViews() finished", VisibleViewsTimer.ElapsedDurationInSecondsAsString );

            return VisibleViews;
        }

        /// <summary>
        /// Get all views with visibility set to the current user
        /// </summary>
        public DataTable getUserViews()
        {
            CswStaticSelect ViewsSelect = _CswNbtResources.makeCswStaticSelect( "getUserViews_select", "getUserViewInfo" );
            ViewsSelect.S4Parameters.Add( "getuserid", _CswNbtResources.CurrentUser.UserId.PrimaryKey.ToString() );
            return ViewsSelect.getTable();
        }

        ///// <summary>
        ///// Returns an XElement of all views visible by the current user, filtered according to Mobile and IsSearchable
        ///// </summary>
        //public Collection<CswNbtView> getVisibleViews( ICswNbtUser User, bool MobileOnly, bool SearchableOnly, string OrderBy )
        //{
        //    CswTimer SearchableViewsTimer = new CswTimer();
        //    Collection<CswNbtView> VisibleViews = new Collection<CswNbtView>();

        //    CswStaticSelect ViewsSelect = _CswNbtResources.makeCswStaticSelect( "getSearchableViews_select", "getVisibleViewInfo" );
        //    ViewsSelect.S4Parameters.Add( "getroleid", User.RoleId.PrimaryKey.ToString() );
        //    ViewsSelect.S4Parameters.Add( "getuserid", User.UserId.PrimaryKey.ToString() );
        //    if( MobileOnly )
        //    {
        //        ViewsSelect.S4Parameters.Add( "addclause", "and formobile = '" + CswConvert.ToDbVal( true ) + "'" );
        //    }
        //    else
        //    {
        //        ViewsSelect.S4Parameters.Add( "addclause", " " );
        //    }
        //    if( !string.IsNullOrEmpty( OrderBy ) )
        //    {
        //        ViewsSelect.S4Parameters.Add( "orderbyclause", OrderBy );
        //    }
        //    else
        //    {
        //        ViewsSelect.S4Parameters.Add( "orderbyclause", "lower(v.viewname)" );
        //    }
        //    DataTable ViewsTable = ViewsSelect.getTable();

        //    _CswNbtResources.logTimerResult( "CswNbtView.getSearchableViews() data fetched", SearchableViewsTimer.ElapsedDurationInSecondsAsString );



        //    _CswNbtResources.logTimerResult( "CswNbtView.getSearchableViews() finished", SearchableViewsTimer.ElapsedDurationInSecondsAsString );

        //    return VisibleViews;
        //}
    }
}
