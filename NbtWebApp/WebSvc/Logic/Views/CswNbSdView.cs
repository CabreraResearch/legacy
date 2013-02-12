using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Search;
using ChemSW.Nbt.Security;
using NbtWebApp.WebSvc.Returns;


namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// View Select Return Object
    /// </summary>
    [DataContract]
    public class CswNbtViewReturn : CswWebSvcReturn
    {
        public CswNbtViewReturn()
        {
            Data = new ViewSelect.Response();
        }
        [DataMember]
        public ViewSelect.Response Data;
    }

    public class CswNbtSdView
    {
        private const string ActionName = "actionname";
        private const string ActionPk = "actionid";
        private const string ActionSelected = "Include";


        private CswNbtResources _CswNbtResources;

        public CswNbtSdView( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        /// <summary>
        /// Create a new Category object and append it to the outer View Select return or fetch an existing Category from the return for use.
        /// </summary>
        private static ViewSelect.Response.Category _getCategory( ref ViewSelect.Response ViewSelectReturn, string Category )
        {
            ViewSelect.Response.Category Ret = ViewSelectReturn.categories.FirstOrDefault( Cat =>
                                                            string.Equals( Cat.category, Category, StringComparison.InvariantCultureIgnoreCase ) );

            if( Ret == null )
            {
                Ret = new ViewSelect.Response.Category( Category );

                // Insertion sort on category name
                if( Category == "Uncategorized" )
                {
                    // Last
                    ViewSelectReturn.categories.Add( Ret );
                }
                else if( Category == "Favorites" || Category == "Recent" )
                {
                    // First
                    ViewSelectReturn.categories.Insert( 0, Ret );
                }
                else
                {
                    // Alphabetical
                    Int32 insertAt = -1;
                    for( Int32 i = 0; i < ViewSelectReturn.categories.Count; i += 1 )
                    {
                        string ThisCat = ViewSelectReturn.categories[i].category;
                        if( ThisCat == "Uncategorized" ||
                            ( ThisCat != "Favorites" &&
                              ThisCat != "Recent" &&
                              Category.CompareTo( ThisCat ) <= 0 ) )
                        {
                            insertAt = i;
                            break;
                        }
                    } // for( Int32 i = 0; i < ret.Count; i++ )

                    if( insertAt >= 0 )
                    {
                        ViewSelectReturn.categories.Insert( insertAt, Ret );
                    }
                    else
                    {
                        ViewSelectReturn.categories.Add( Ret );
                    }
                }
            }
            return Ret;
        } // _addCategory()

        /// <summary>
        /// Create a new Item object and append it to a Category in the View Select return
        /// </summary>
        private static ViewSelect.Response.Item _addViewSelectObj( ref ViewSelect.Response ViewSelectReturn, string Category, string Name, ItemType Type, string Icon, string Id )
        {
            if( Category == string.Empty )
            {
                Category = "Uncategorized";
            }
            ViewSelect.Response.Category Cat = _getCategory( ref ViewSelectReturn, Category );

            return _addViewSelectObj( Cat, Name, Type, Icon, Id );
        }

        /// <summary>
        /// Create a new Item object and append it to a Category in the View Select return
        /// </summary>
        private static ViewSelect.Response.Item _addViewSelectObj( ViewSelect.Response.Category Cat, string Name, ItemType Type, string Icon, string Id )
        {
            ViewSelect.Response.Item Ret = new ViewSelect.Response.Item( Type )
            {
                name = Name,
                itemid = Id,
                iconurl = Icon
            };

            Cat.items.Add( Ret );

            return Ret;
        }

        public static ViewSelect.Response getViewSelectRecent( CswNbtResources CswNbtResources )
        {
            ViewSelect.Response Ret = new ViewSelect.Response();
            ViewSelect.Response.Category Recent = _getCategory( ref Ret, "Recent" );
            CswNbtResources.SessionDataMgr.getQuickLaunchJson( ref Recent );
            return Ret;
        } // getViewSelectRecent()

        /// <summary>
        /// WCF wrapper around getViewSelect
        /// </summary>
        public static void getViewSelectWebSvc( ICswResources CswResources, CswNbtViewReturn ViewReturn, ViewSelect.Request Request )
        {
            if( false == Request.LimitToRecent )
            {
                ViewReturn.Data = getViewSelect( (CswNbtResources) CswResources, Request );
            }
            else
            {
                ViewReturn.Data = getViewSelectRecent( (CswNbtResources) CswResources );
            }
        }


        public static ViewSelect.Response getViewSelect( CswNbtResources CswNbtResources, ViewSelect.Request Request )
        {
            ViewSelect.Response ret = new ViewSelect.Response();

            // Favorites and Recent
            ICswNbtUser User = CswNbtResources.CurrentNbtUser;
            if( User != null )
            {
                CswNbtObjClassUser UserOc = CswNbtResources.Nodes[User.UserId];

                // Recent
                if( Request.IncludeRecent )
                {
                    ViewSelect.Response.Category RecentCategory = _getCategory( ref ret, "Recent" );
                    CswNbtResources.SessionDataMgr.getQuickLaunchJson( ref RecentCategory );
                }

                ViewSelect.Response.Category FavoritesCategory = _getCategory( ref ret, "Favorites" );
                //Add the user's stored views to Favorites
                foreach( CswNbtView View in UserOc.FavoriteViews.SelectedViews.Values.Where( View => View.IsFullyEnabled() ) )
                {
                    ViewSelect.Response.Item ViewItem = _addViewSelectObj( FavoritesCategory, 
                                                                           View.ViewName, 
                                                                           ItemType.View, 
                                                                           View.IconFileName, 
                                                                           View.ViewId.ToString() );
                    ViewItem.mode = View.ViewMode.ToString();
                }

                //Add the user's stored actions to Favorites
                DataTable ActionsTable = UserOc.FavoriteActions.GetDataAsTable( ActionName, ActionPk );
                foreach( CswNbtAction Action in ( from DataRow ActionRow in ActionsTable.Rows
                                                  where CswConvert.ToBoolean( ActionRow[ActionSelected] )
                                                  select CswNbtAction.ActionNameStringToEnum( CswConvert.ToString( ActionRow[ActionPk] ) )
                                                      into NbtActionName
                                                      select CswNbtResources.Actions[NbtActionName]
                                                          into ThisAction
                                                          where null != ThisAction
                                                          select ThisAction ) )
                {
                    if( Action.ShowInList ) //case 26555 - filter out actions like 'Multi Edit' or 'Edit View'
                    {
                        ViewSelect.Response.Item ActionItem = _addViewSelectObj( FavoritesCategory, 
                                                                                 Action.DisplayName, 
                                                                                 ItemType.Action,
                                                                                 CswNbtMetaDataObjectClass.IconPrefix16 + Action.IconFileName, 
                                                                                 Action.ActionId.ToString() );
                        ActionItem.url = Action.Url;
                    }
                }
            }

            // Views
            Dictionary<CswNbtViewId, CswNbtView> Views = CswNbtResources.ViewSelect.getVisibleViews( "lower(NVL(v.category, v.viewname)), lower(v.viewname)", CswNbtResources.CurrentNbtUser, false, false, Request.IsSearchable, NbtViewRenderingMode.Any );

            foreach( CswNbtView View in Views.Values )
            {
                ViewSelect.Response.Item ViewItem = _addViewSelectObj( ref ret, 
                                                                       View.Category, 
                                                                       View.ViewName, 
                                                                       ItemType.View, 
                                                                       View.IconFileName, 
                                                                       View.ViewId.ToString() );
                ViewItem.mode = View.ViewMode.ToString();
            }

            if( false == Request.IsSearchable )
            {
                // Actions
                foreach( CswNbtAction Action in CswNbtResources.Actions )
                {
                    if( Action.ShowInList &&
                        //Case 23687: "View By Location" Action is toast. Bye-bye "loc_use_images" config var check.
                        CswNbtResources.Permit.can( Action.Name ) )
                    {
                        ViewSelect.Response.Item ActionItem = _addViewSelectObj( ref ret, 
                                                                                 Action.Category, 
                                                                                 Action.DisplayName, 
                                                                                 ItemType.Action,
                                                                                 CswNbtMetaDataObjectClass.IconPrefix16 + Action.IconFileName, 
                                                                                 Action.ActionId.ToString() );
                        ActionItem.url = Action.Url;
                    }
                }

                // Reports
                CswNbtMetaDataObjectClass ReportMetaDataObjectClass = CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ReportClass );
                CswNbtView ReportView = ReportMetaDataObjectClass.CreateDefaultView();
                ReportView.ViewName = "CswViewTree.DataBinding.ReportView";
                ICswNbtTree ReportTree = CswNbtResources.Trees.getTreeFromView( CswNbtResources.CurrentNbtUser, ReportView, true, false, false );
                for( int i = 0; i < ReportTree.getChildNodeCount(); i++ )
                {
                    ReportTree.goToNthChild( i );

                    CswNbtObjClassReport ReportNode = ReportTree.getNodeForCurrentPosition();
                    _addViewSelectObj( ref ret, 
                                       ReportNode.Category.Text, 
                                       ReportNode.ReportName.Text, 
                                       ItemType.Report,
                                       CswNbtMetaDataObjectClass.IconPrefix16 + "doc.png", 
                                       ReportNode.NodeId.ToString() );

                    ReportTree.goToParentNode();
                }

                // Searches
                Collection<CswNbtSearch> Searches = CswNbtResources.SearchManager.getSearches();
                foreach(CswNbtSearch Search in Searches)
                {
                    _addViewSelectObj( ref ret,
                                       Search.Category, 
                                       Search.Name, 
                                       ItemType.Search, 
                                       CswNbtMetaDataObjectClass.IconPrefix16 + "magglass.png", 
                                       Search.SearchId.ToString() );
                }
            } // if( false == Request.IsSearchable )

            return ret;
        } // getViewSelect()
    } // class CswNbtSdView

} // namespace ChemSW.Nbt.WebServices
