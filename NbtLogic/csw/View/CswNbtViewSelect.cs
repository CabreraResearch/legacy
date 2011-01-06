using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Core;
using ChemSW.DB;
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
        /// Get a DataTable with a single view, by primary key
        /// </summary>
        public DataTable getView( Int32 ViewId )
        {
            CswStaticSelect ViewsSelect = _CswNbtResources.makeCswStaticSelect( "CswNbtViewSelect.getView", "getViewInfo" );
            ViewsSelect.S4Parameters.Add( "getviewid", ViewId.ToString() );
            return ViewsSelect.getTable();
        }

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
        public DataTable getVisibleViews( bool IncludeEmptyViews )
        {
            return getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser, IncludeEmptyViews, false );
        }

        /// <summary>
        /// Get a DataTable of all views visible to the provided user
        /// </summary>
        public DataTable getVisibleViews( ICswNbtUser User, bool IncludeEmptyViews )
        {
            return getVisibleViews( string.Empty, User, IncludeEmptyViews, false );
        }

        /// <summary>
        /// Get a DataTable of all views visible to the current user
        /// </summary>
        public DataTable getVisibleViews( string OrderBy, bool IncludeEmptyViews )
        {
            return getVisibleViews( OrderBy, _CswNbtResources.CurrentNbtUser, IncludeEmptyViews, false );
        }


        private DataTable _LastVisibleViews = null;
        private string _LastOrderBy = string.Empty;
        private ICswNbtUser _LastUser = null;
        private bool _LastIncludeEmptyViews = false;

        /// <summary>
        /// Reset the cached visibile views table
        /// </summary>
        public void clearCache()
        {
            _LastVisibleViews = null;
        }

        /// <summary>
        /// Get a DataTable of all views visible to the current user
        /// </summary>
        public DataTable getVisibleViews( string OrderBy, ICswNbtUser User, bool IncludeEmptyViews, bool MobileOnly )
        {
            DataTable ViewsTable = null;
            if( _LastVisibleViews != null &&
                _LastOrderBy == OrderBy &&
                _LastUser == User &&
                _LastIncludeEmptyViews == IncludeEmptyViews )
            {
                ViewsTable = _LastVisibleViews;
            }
            else
            {
                CswTimer VisibleViewsTimer = new CswTimer();

                CswStaticSelect ViewsSelect = _CswNbtResources.makeCswStaticSelect( "getVisibleViews_select", "getVisibleViewInfo" );
                ViewsSelect.S4Parameters.Add( "getroleid", User.RoleId.PrimaryKey.ToString() );
                ViewsSelect.S4Parameters.Add( "getuserid", User.UserId.PrimaryKey.ToString() );
                if( MobileOnly )
                    ViewsSelect.S4Parameters.Add( "addclause", "and formobile = '" + CswConvert.ToDbVal( true ) + "'" );
                else
                    ViewsSelect.S4Parameters.Add( "addclause", " " );
                if( OrderBy != string.Empty )
                    ViewsSelect.S4Parameters.Add( "orderbyclause", OrderBy );
                else
                    ViewsSelect.S4Parameters.Add( "orderbyclause", "lower(v.viewname)" );
                ViewsTable = ViewsSelect.getTable();

                _CswNbtResources.logTimerResult( "CswNbtView.getVisibleViews() data fetched", VisibleViewsTimer.ElapsedDurationInSecondsAsString );

                // BZ 7074 - Make sure the user has permissions to at least one root node
                Collection<DataRow> RowsToRemove = new Collection<DataRow>();
                foreach( DataRow Row in ViewsTable.Rows )
                {
                    CswNbtView ThisView = (CswNbtView) CswNbtViewFactory.restoreView( _CswNbtResources, Row["viewxml"].ToString() );
                    if( ThisView.Root.ChildRelationships.Count > 0 || !IncludeEmptyViews )      // BZ 8136
                    {
                        bool skipme = true;
                        if( ThisView.IsFullyEnabled() )
                        {
                            foreach( CswNbtViewRelationship R in ThisView.Root.ChildRelationships )
                            {
                                if( R.SecondType != CswNbtViewRelationship.RelatedIdType.NodeTypeId ||
                                    User.CheckPermission( NodeTypePermission.View, R.SecondId, null, null ) )
                                {
                                    skipme = false;
                                }
                            }
                        }
                        if( skipme )
                        {
                            RowsToRemove.Add( Row );
                        }
                    }
                }
                foreach( DataRow Row in RowsToRemove )
                    ViewsTable.Rows.Remove( Row );

                _LastIncludeEmptyViews = IncludeEmptyViews;
                _LastOrderBy = OrderBy;
                _LastUser = User;
                _LastVisibleViews = ViewsTable;

                _CswNbtResources.logTimerResult( "CswNbtView.getVisibleViews() finished", VisibleViewsTimer.ElapsedDurationInSecondsAsString );
            }

            return ViewsTable;
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
    }
}
