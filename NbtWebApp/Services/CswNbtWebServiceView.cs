using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Grid;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceView
    {
        public enum ItemType
        {
            Root,
            View,
            //ViewCategory, 
            Category,
            Action,
            Report,
            //ReportCategory, 
            Search,
            RecentView,
            Unknown
        };

        private const string ActionName = "actionname";
        private const string ActionPk = "actionid";
        private const string ActionSelected = "Include";


        private CswNbtResources _CswNbtResources;

        public CswNbtWebServiceView( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        private JObject _getCategory( ref JArray ret, string Category )
        {
            JObject CatItemsJObj = null;

            for( Int32 i = 0; i < ret.Count; i++ )
            {
                string ThisCat = ret[i]["category"].ToString();
                if( ThisCat == Category )
                {
                    CatItemsJObj = (JObject) ret[i]["items"];
                }
            }

            if( CatItemsJObj == null )
            {
                JObject CatJObjToAdd = new JObject();
                CatJObjToAdd["category"] = Category;
                CatJObjToAdd["items"] = new JObject();
                CatItemsJObj = (JObject) CatJObjToAdd["items"];

                // Insertion sort on category name
                if( Category == "Uncategorized" )
                {
                    // Last
                    ret.Add( CatJObjToAdd );
                }
                else if( Category == "Favorites" || Category == "Recent" )
                {
                    // First
                    ret.AddFirst( CatJObjToAdd );
                }
                else
                {
                    // Alphabetical
                    Int32 insertAt = -1;
                    for( Int32 i = 0; i < ret.Count; i++ )
                    {
                        string ThisCat = ret[i]["category"].ToString();
                        if( ThisCat == "Uncategorized" )
                        {
                            insertAt = i;
                            break;
                        }
                        else if( ThisCat != "Favorites" && ThisCat != "Recent" )
                        {
                            if( Category.CompareTo( ThisCat ) <= 0 )
                            {
                                insertAt = i;
                                break;
                            }
                        }
                    } // for( Int32 i = 0; i < ret.Count; i++ )

                    if( insertAt >= 0 )
                    {
                        ret.Insert( insertAt, CatJObjToAdd );
                    }
                    else
                    {
                        ret.Add( CatJObjToAdd );
                    }
                }
            }
            return CatItemsJObj;
        } // _addCategory()

        private JObject _addViewSelectObj( ref JArray ret, string Category, string Name, ItemType Type, string Icon, string Id )
        {
            if( Category == string.Empty )
            {
                Category = "Uncategorized";
            }
            JObject CatItemsJObj = _getCategory( ref ret, Category );

            JObject NewObj = new JObject();
            NewObj["name"] = Name;
            NewObj["type"] = Type.ToString();
            NewObj["id"] = Id;
            NewObj["iconurl"] = Icon;
            CatItemsJObj[Name] = NewObj;

            return NewObj;
        }

        public JArray getViewSelectRecent()
        {
            JArray ret = new JArray();
            JObject RecentItemsJObj = _getCategory( ref ret, "Recent" );
            _CswNbtResources.SessionDataMgr.getQuickLaunchJson( ref RecentItemsJObj );
            return ret;
        } // getViewSelectRecent()


        public JArray getViewSelect( bool IsSearchable, bool IncludeRecent )
        {
            JArray ret = new JArray();

            // Favorites and Recent
            ICswNbtUser User = _CswNbtResources.CurrentNbtUser;
            if( User != null )
            {
                CswNbtNode UserNode = _CswNbtResources.Nodes[User.UserId];
                CswNbtObjClassUser UserOc = (CswNbtObjClassUser) UserNode;

                // Recent
                if( IncludeRecent )
                {
                    JObject RecentItemsJObj = _getCategory( ref ret, "Recent" );
                    _CswNbtResources.SessionDataMgr.getQuickLaunchJson( ref RecentItemsJObj );
                }

                //Add the user's stored views to Favorites
                foreach( CswNbtView View in UserOc.FavoriteViews.SelectedViews.Values.Where( View => View.IsFullyEnabled() ) )
                {
                    JObject ViewObj = _addViewSelectObj( ref ret, "Favorites", View.ViewName, ItemType.View, View.IconFileName, View.ViewId.ToString() );
                    ViewObj["viewid"] = View.ViewId.ToString();
                    ViewObj["viewmode"] = View.ViewMode.ToString();
                    ViewObj["viewname"] = View.ViewName.ToString();
                }

                //Add the user's stored actions to Favorites
                DataTable ActionsTable = UserOc.FavoriteActions.GetDataAsTable( ActionName, ActionPk );
                foreach( CswNbtAction Action in ( from DataRow ActionRow in ActionsTable.Rows
                                                  where CswConvert.ToBoolean( ActionRow[ActionSelected] )
                                                  select CswNbtAction.ActionNameStringToEnum( CswConvert.ToString( ActionRow[ActionPk] ) )
                                                      into NbtActionName
                                                      select _CswNbtResources.Actions[NbtActionName]
                                                          into ThisAction
                                                          where null != ThisAction
                                                          select ThisAction ) )
                {
                    if( Action.ShowInList ) //case 26555 - filter out actions like 'Multi Edit' or 'Edit View'
                    {
                        JObject ActionObj = _addViewSelectObj( ref ret, "Favorites", Action.DisplayName, ItemType.Action,
                            CswNbtMetaDataObjectClass.IconPrefix16 + "wizard.png", Action.ActionId.ToString() );
                        ActionObj["actionid"] = Action.ActionId.ToString();
                        ActionObj["actionurl"] = Action.Url;
                        ActionObj["actionname"] = Action.Name.ToString();   // not using CswNbtAction.ActionNameEnumToString here
                    }
                }
            }

            // Views
            Dictionary<CswNbtViewId, CswNbtView> Views = _CswNbtResources.ViewSelect.getVisibleViews( "lower(NVL(v.category, v.viewname)), lower(v.viewname)", _CswNbtResources.CurrentNbtUser, false, false, IsSearchable, NbtViewRenderingMode.Any );

            foreach( CswNbtView View in Views.Values )
            {
                JObject ViewObj = _addViewSelectObj( ref ret, View.Category, View.ViewName, ItemType.View, View.IconFileName, View.ViewId.ToString() );
                ViewObj["viewid"] = View.ViewId.ToString();
                ViewObj["viewmode"] = View.ViewMode.ToString();
                ViewObj["viewname"] = View.ViewName.ToString();
            }

            if( !IsSearchable )
            {
                // Actions
                foreach( CswNbtAction Action in _CswNbtResources.Actions )
                {
                    if( Action.ShowInList &&
                        //Case 23687: "View By Location" Action is toast. Bye-bye "loc_use_images" config var check.
                        _CswNbtResources.Permit.can( Action.Name ) )
                    {
                        JObject ActionObj = _addViewSelectObj( ref ret, Action.Category, Action.DisplayName, ItemType.Action, 
                                                               CswNbtMetaDataObjectClass.IconPrefix16 + "wizard.png", Action.ActionId.ToString() );
                        ActionObj["actionid"] = Action.ActionId.ToString();
                        ActionObj["actionurl"] = Action.Url;
                        ActionObj["actionname"] = Action.Name.ToString();   // not using CswNbtAction.ActionNameEnumToString here
                    }
                }

                // Reports
                CswNbtMetaDataObjectClass ReportMetaDataObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
                CswNbtView ReportView = ReportMetaDataObjectClass.CreateDefaultView();
                ReportView.ViewName = "CswViewTree.DataBinding.ReportView";
                ICswNbtTree ReportTree = _CswNbtResources.Trees.getTreeFromView( ReportView, true, true, false, false );
                for( int i = 0; i < ReportTree.getChildNodeCount(); i++ )
                {
                    ReportTree.goToNthChild( i );

                    CswNbtObjClassReport ReportNode = (CswNbtObjClassReport) ReportTree.getNodeForCurrentPosition();
                    JObject ReportObj = _addViewSelectObj( ref ret, ReportNode.Category.Text, ReportNode.ReportName.Text, ItemType.Report, "Images/view/report.gif", ReportNode.NodeId.ToString() );
                    ReportObj["reportid"] = ReportNode.NodeId.ToString();

                    ReportTree.goToParentNode();
                }
            }

            return ret;
        } // getViewSelect()


        public JObject getViewGrid( bool All )
        {
            JObject ReturnVal = new JObject();
            CswNbtGrid gd = new CswNbtGrid( _CswNbtResources );
            //gd.PkColumn = "nodeviewid";

            JArray JColumnNames = new JArray();
            JArray JColumnDefs = new JArray();
            JArray JRows = new JArray();

            bool IsAdmin = _CswNbtResources.CurrentNbtUser.IsAdministrator();

            Dictionary<CswNbtViewId, CswNbtView> Views = new Dictionary<CswNbtViewId, CswNbtView>();
            DataTable ViewsTable = null;
            if( IsAdmin )
            {
                if( All )
                {
                    ViewsTable = _CswNbtResources.ViewSelect.getAllEnabledViews();
                }
                else
                {
                    Views = _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser,
                                                                         true, false, false, NbtViewRenderingMode.Any,
                                                                         out ViewsTable, null );
                }
            }
            else
            {
                ViewsTable = _CswNbtResources.ViewSelect.getUserViews();
            }

            if( ViewsTable != null )
            {
                ViewsTable.Columns.Add( "viewid" );      // string CswNbtViewId
                foreach( DataRow Row in ViewsTable.Rows )
                {
                    Row["viewid"] = new CswNbtViewId( CswConvert.ToInt32( Row["nodeviewid"] ) ).ToString();
                }
            }

            //if( null == ViewsTable )
            //{
            //    gd.makeJqColumn( null, "NODEVIEWID", JColumnNames, JColumnDefs, false, true );
            //    gd.makeJqColumn( null, "VIEWID", JColumnNames, JColumnDefs, true, true );
            //    gd.makeJqColumn( null, "VIEWNAME", JColumnNames, JColumnDefs, false, false );
            //    gd.makeJqColumn( null, "VIEWMODE", JColumnNames, JColumnDefs, false, false );
            //    gd.makeJqColumn( null, "VISIBILITY", JColumnNames, JColumnDefs, false, false );
            //    gd.makeJqColumn( null, "CATEGORY", JColumnNames, JColumnDefs, false, false );
            //    gd.makeJqColumn( null, "ROLENAME", JColumnNames, JColumnDefs, false, false );
            //    gd.makeJqColumn( null, "USERNAME", JColumnNames, JColumnDefs, false, false );
            //    foreach( CswNbtView View in Views.Values )
            //    {
            //        if( View.IsFullyEnabled() )
            //        {
            //            string RoleName = string.Empty;
            //            CswNbtNode Role = _CswNbtResources.Nodes.GetNode( View.VisibilityRoleId );
            //            if( null != Role )
            //            {
            //                RoleName = Role.NodeName;
            //            }
            //            string UserName = string.Empty;
            //            CswNbtNode User = _CswNbtResources.Nodes.GetNode( View.VisibilityUserId );
            //            if( null != User )
            //            {
            //                UserName = User.NodeName;
            //            }
            //            JObject RowObj = new JObject();
            //            JRows.Add( RowObj );
            //            gd.makeJqCell( RowObj, "NODEVIEWID", View.ViewId.get().ToString() );
            //            gd.makeJqCell( RowObj, "VIEWID", View.ViewId.ToString() );
            //            gd.makeJqCell( RowObj, "VIEWNAME", View.ViewName );
            //            gd.makeJqCell( RowObj, "VIEWMODE", View.ViewMode.ToString() );
            //            gd.makeJqCell( RowObj, "VISIBILITY", View.Visibility.ToString() );
            //            gd.makeJqCell( RowObj, "CATEGORY", View.Category );
            //            gd.makeJqCell( RowObj, "ROLENAME", RoleName );
            //            gd.makeJqCell( RowObj, "USERNAME", UserName );
            //        }
            //    }
            //    ReturnVal = gd.makeJqGridJSON( JColumnNames, JColumnDefs, JRows );
            //}
            //else
            //{
            if( ViewsTable != null )
            {
                if( ViewsTable.Columns.Contains( "viewxml" ) )
                    ViewsTable.Columns.Remove( "viewxml" );
                if( ViewsTable.Columns.Contains( "roleid" ) )
                    ViewsTable.Columns.Remove( "roleid" );
                if( ViewsTable.Columns.Contains( "userid" ) )
                    ViewsTable.Columns.Remove( "userid" );
                if( ViewsTable.Columns.Contains( "mssqlorder" ) )
                    ViewsTable.Columns.Remove( "mssqlorder" );

                if( !IsAdmin )
                {
                    if( ViewsTable.Columns.Contains( "visibility" ) )
                        ViewsTable.Columns.Remove( "visibility" );
                    if( ViewsTable.Columns.Contains( "username" ) )
                        ViewsTable.Columns.Remove( "username" );
                    if( ViewsTable.Columns.Contains( "rolename" ) )
                        ViewsTable.Columns.Remove( "rolename" );
                }

                ChemSW.Nbt.Grid.ExtJs.CswNbtGridExtJsGrid grid = gd.DataTableToGrid( ViewsTable );
                grid.getColumn( "nodeviewid" ).hidden = true;
                grid.getColumn( "viewid" ).hidden = true;

                ReturnVal = grid.ToJson();
            } // if(ViewsTable != null)

            return ReturnVal;
        } // getViewGrid()

        public JObject getViewChildOptions( string ViewJson, string ArbitraryId, Int32 StepNo )
        {
            JObject ret = new JObject();

            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.LoadJson( ViewJson );

            if( View.ViewId != null )
            {
                CswNbtViewNode SelectedViewNode = View.FindViewNodeByArbitraryId( ArbitraryId );
                if( View.ViewMode != NbtViewRenderingMode.Unknown || View.Root.ChildRelationships.Count == 0 )
                {
                    if( SelectedViewNode is CswNbtViewRelationship )
                    {
                        if( StepNo == 3 && View.ViewMode != NbtViewRenderingMode.List )
                        {
                            // Potential child relationships

                            CswNbtViewRelationship CurrentRelationship = (CswNbtViewRelationship) SelectedViewNode;
                            Int32 CurrentLevel = 0;
                            CswNbtViewNode Parent = CurrentRelationship;
                            while( !( Parent is CswNbtViewRoot ) )
                            {
                                CurrentLevel++;
                                Parent = Parent.Parent;
                            }

                            // Child options are all relations to this nodetype
                            Int32 CurrentId = CurrentRelationship.SecondId;

                            Collection<CswNbtViewRelationship> Relationships = null;
                            if( CurrentRelationship.SecondType == NbtViewRelatedIdType.ObjectClassId )
                            {
                                Relationships = getObjectClassRelatedNodeTypesAndObjectClasses( CurrentId, View, CurrentLevel );
                            }
                            else if( CurrentRelationship.SecondType == NbtViewRelatedIdType.NodeTypeId )
                            {
                                Relationships = getNodeTypeRelatedNodeTypesAndObjectClasses( CurrentId, View, CurrentLevel );
                            }
                            //else
                            //    throw new CswDniException( "A Data Misconfiguration has occurred", "CswViewEditor2._initNextOptions() has a selected node which is neither a NodeTypeNode nor an ObjectClassNode" );

                            foreach( CswNbtViewRelationship R in from CswNbtViewRelationship _R in Relationships orderby _R.SecondName select _R )
                            {
                                if( !CurrentRelationship.ChildRelationships.Contains( R ) )
                                {
                                    R.Parent = CurrentRelationship;
                                    string Label = String.Empty;

                                    if( R.PropOwner == NbtViewPropOwnerType.First )
                                    {
                                        Label = R.SecondName + " (by " + R.PropName + ")";
                                    }
                                    else if( R.PropOwner == NbtViewPropOwnerType.Second )
                                    {
                                        Label = R.SecondName + " (by " + R.SecondName + "'s " + R.PropName + ")";
                                    }

                                    //if( isSelectable( R.SecondType, R.SecondId ) )
                                    //    R.Selectable = true;
                                    //else
                                    //    R.Selectable = false;


                                    JProperty RProp = R.ToJson( Label, true );
                                    ret.Add( RProp );

                                } //  if( !CurrentRelationship.ChildRelationships.Contains( R ) )
                            } // foreach( CswNbtViewRelationship R in Relationships )
                        } // if( StepNo == 3)
                        else if( StepNo == 4 )
                        {
                            // Potential child properties

                            CswNbtViewRelationship CurrentRelationship = (CswNbtViewRelationship) SelectedViewNode;

                            ICollection PropsCollection = null;
                            if( CurrentRelationship.SecondType == NbtViewRelatedIdType.ObjectClassId )
                            {
                                PropsCollection = _getObjectClassPropsCollection( CurrentRelationship.SecondId );
                            }
                            else if( CurrentRelationship.SecondType == NbtViewRelatedIdType.NodeTypeId )
                            {
                                PropsCollection = _getNodeTypePropsCollection( CurrentRelationship.SecondId );
                            }
                            else
                            {
                                throw new CswDniException( ErrorType.Error, "A Data Misconfiguration has occurred", "CswViewEditor.initPropDataTable() has a selected node which is neither a NodeTypeNode nor an ObjectClassNode" );
                            }

                            foreach( CswNbtMetaDataNodeTypeProp ThisProp in from CswNbtMetaDataNodeTypeProp _ThisProp in PropsCollection orderby _ThisProp.PropNameWithQuestionNo select _ThisProp )
                            {
                                // BZs 7085, 6651, 6644, 7092
                                if( ThisProp.getFieldTypeRule().SearchAllowed ||
                                    ThisProp.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Button )
                                {
                                    CswNbtViewProperty ViewProp = View.AddViewProperty( null, (CswNbtMetaDataNodeTypeProp) ThisProp );
                                    if( !CurrentRelationship.Properties.Contains( ViewProp ) )
                                    {
                                        ViewProp.Parent = CurrentRelationship;

                                        string PropName = ViewProp.MetaDataProp.PropNameWithQuestionNo;
                                        if( false == ThisProp.getNodeType().IsLatestVersion() )
                                            PropName += "&nbsp;(v" + ThisProp.getNodeType().VersionNo + ")";

                                        JProperty PropJProp = ViewProp.ToJson( PropName, true );
                                        ret.Add( PropJProp );

                                    } // if( !CurrentRelationship.Properties.Contains( ViewProp ) )
                                } // if( ThisProp.FieldTypeRule.SearchAllowed )
                            } // foreach (DataRow Row in Props.Rows)
                        } // if-else(StepNo == 3)
                    } // if( SelectedViewNode is CswNbtViewRelationship )
                    else if( SelectedViewNode is CswNbtViewRoot )
                    {
                        // Set NextOptions to be all viewable nodetypes and objectclasses
                        foreach( CswNbtMetaDataNodeType LatestNodeType in from CswNbtMetaDataNodeType _LatestNodeType in _CswNbtResources.MetaData.getNodeTypesLatestVersion() orderby _LatestNodeType.NodeTypeName select _LatestNodeType )
                        {
                            if( _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, LatestNodeType ) )
                            {
                                // This is purposefully not the typical way of creating CswNbtViewRelationships.
                                CswNbtViewRelationship R = new CswNbtViewRelationship( _CswNbtResources, View, LatestNodeType.getFirstVersionNodeType(), false );
                                R.Parent = SelectedViewNode;

                                //if( isSelectable( R.SecondType, R.SecondId ) )
                                //    R.Selectable = true;
                                //else
                                //    R.Selectable = false;

                                bool IsChildAlready = false;
                                foreach( CswNbtViewRelationship ChildRel in from CswNbtViewRelationship _ChildRel in ( (CswNbtViewRoot) SelectedViewNode ).ChildRelationships orderby _ChildRel.SecondName select _ChildRel )
                                {
                                    if( ChildRel.SecondType == R.SecondType && ChildRel.SecondId == R.SecondId )
                                    {
                                        IsChildAlready = true;
                                    }
                                }

                                if( !IsChildAlready )
                                {
                                    JProperty RProp = R.ToJson( LatestNodeType.NodeTypeName, true );
                                    ret.Add( RProp );
                                }
                            }
                        }

                        foreach( CswNbtMetaDataObjectClass ObjectClass in
                            from CswNbtMetaDataObjectClass _ObjectClass
                                in _CswNbtResources.MetaData.getObjectClasses()
                            orderby _ObjectClass.ObjectClass
                            where _ObjectClass.ObjectClass != CswNbtMetaDataObjectClass.NbtObjectClass.Unknown
                            select _ObjectClass )
                        {
                            // This is purposefully not the typical way of creating CswNbtViewRelationships.

                            CswNbtViewRelationship R = new CswNbtViewRelationship( _CswNbtResources, View, ObjectClass, false );
                            R.Parent = SelectedViewNode;

                            //if( isSelectable( R.SecondType, R.SecondId ) )
                            //    R.Selectable = true;
                            //else
                            //    R.Selectable = false;

                            if( !( (CswNbtViewRoot) SelectedViewNode ).ChildRelationships.Contains( R ) )
                            {
                                JProperty RProp = R.ToJson( "Any " + ObjectClass.ObjectClass, true );
                                ret.Add( RProp );
                            }
                        }
                    } // else if( SelectedViewNode is CswNbtViewRoot )
                    else if( SelectedViewNode is CswNbtViewProperty )
                    {
                        ret.Add( new JProperty( "filters", "" ) );
                    }
                    else if( SelectedViewNode is CswNbtViewPropertyFilter )
                    {
                    }

                } // if( _View.ViewMode != NbtViewRenderingMode.List || _View.Root.ChildRelationships.Count == 0 )
            } // if( _View != null )

            return ret;
        } // getViewChildOptions()

        public JObject getRuntimeViewFilters( CswNbtView View )
        {
            JObject ret = new JObject();
            if( View != null )
            {
                // We need the property arbitrary id, so we're doing this by property, not by filter.  
                // However, we're filtering to only those properties that have filters that have ShowAtRuntime == true
                foreach( CswNbtViewProperty Property in from CswNbtViewProperty _Property in View.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewProperty ) orderby _Property.MetaDataProp.PropNameWithQuestionNo select _Property )
                {
                    JProperty PropertyJson = Property.ToJson( ShowAtRuntimeOnly: true );
                    if( ( (JObject) PropertyJson.Value["filters"] ).Count > 0 )
                    {
                        // case 26166 - collapse redundant filters
                        bool foundMatch = false;
                        foreach( JProperty OtherPropertyJson in ret.Properties() )
                        {
                            if( PropertyJson.Value["name"].ToString() == OtherPropertyJson.Value["name"].ToString() &&
                                PropertyJson.Value["fieldtype"].ToString() == OtherPropertyJson.Value["fieldtype"].ToString() )
                            {
                                foundMatch = true;
                            }
                        }
                        if( false == foundMatch )
                        {
                            ret.Add( PropertyJson );
                        }
                    }
                }
            }
            return ret;
        } // getRuntimeViewFilters()

        public JObject updateRuntimeViewFilters( CswNbtView View, JObject NewFiltersJson )
        {
            foreach( JProperty NewFilterProp in NewFiltersJson.Properties() )
            {
                string FilterArbitraryId = NewFilterProp.Name;
                JObject NewFilter = (JObject) NewFilterProp.Value;
                if( NewFilter.Children().Count() > 0 )
                {
                    // case 26166 - apply to all matching properties
                    CswNbtViewPropertyFilter ViewPropFilter = (CswNbtViewPropertyFilter) View.FindViewNodeByArbitraryId( FilterArbitraryId );
                    CswNbtViewProperty ViewParentProp = (CswNbtViewProperty) ViewPropFilter.Parent;
                    foreach( CswNbtViewPropertyFilter OtherPropFilter in View.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewPropertyFilter ) )
                    {
                        CswNbtViewProperty OtherParentProp = ( (CswNbtViewProperty) OtherPropFilter.Parent );
                        if( OtherParentProp.Name == ViewParentProp.Name &&
                            OtherParentProp.FieldType == ViewParentProp.FieldType &&
                            OtherPropFilter.Value == ViewPropFilter.Value )
                        {
                            OtherPropFilter.FilterMode = (CswNbtPropFilterSql.PropertyFilterMode) NewFilter["filter"].ToString();
                            OtherPropFilter.SubfieldName = (CswNbtSubField.SubFieldName) NewFilter["subfieldname"].ToString();
                            OtherPropFilter.Value = NewFilter["filtervalue"].ToString();
                        }
                    }
                }
            }

            View.SaveToCache( true, true );

            JObject ret = new JObject();
            ret["newviewid"] = View.SessionViewId.ToString();
            return ret;
        }

        #region Helper Functions

        private Collection<CswNbtViewRelationship> getNodeTypeRelatedNodeTypesAndObjectClasses( Int32 FirstVersionId, CswNbtView View, Int32 Level )
        {
            Collection<CswNbtViewRelationship> Relationships = new Collection<CswNbtViewRelationship>();

            // If we're doing a grid, we can only pick things in which the provided nodetype has a relationship to, 
            // rather than things that are related to the provided nodetype.
            // If this is a property grid, then the above rule does not apply to the first level.
            bool Restrict = ( View.ViewMode == NbtViewRenderingMode.Grid || View.ViewMode == NbtViewRenderingMode.Table ) &&
                            ( View.Visibility != NbtViewVisibility.Property || Level >= 2 );

            CswNbtMetaDataNodeType FirstVersionNodeType = _CswNbtResources.MetaData.getNodeType( FirstVersionId );
            CswNbtMetaDataObjectClass ObjectClass = FirstVersionNodeType.getObjectClass();

            CswStaticSelect RelationshipPropsSelect = _CswNbtResources.makeCswStaticSelect( "getRelationsForNodeTypeId_select", "getRelationsForNodeTypeId" );
            RelationshipPropsSelect.S4Parameters.Add( "getnodetypeid", new CswStaticParam( "getnodetypeid", FirstVersionNodeType.NodeTypeId ) );
            //RelationshipPropsQueryCaddy.S4Parameters.Add("getroleid", _CswNbtResources.CurrentUser.RoleId);
            DataTable RelationshipPropsTable = RelationshipPropsSelect.getTable();

            foreach( DataRow PropRow in RelationshipPropsTable.Rows )
            {
                // Ignore relationships that don't have a target
                if( PropRow["fktype"].ToString() != String.Empty &&
                    PropRow["fkvalue"].ToString() != String.Empty )
                {
                    CswNbtMetaDataNodeTypeProp ThisProp = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropRow["propid"] ) );

                    if( ( PropRow["proptype"].ToString() == NbtViewPropIdType.NodeTypePropId.ToString() &&
                          PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) &&
                        ( PropRow["fktype"].ToString() == NbtViewRelatedIdType.NodeTypeId.ToString() &&
                          PropRow["fkvalue"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) )
                    {
                        if( _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, FirstVersionNodeType ) )
                        {
                            // Special case -- relationship to my own type
                            // We need to create two relationships from this

                            CswNbtViewRelationship R1 = View.AddViewRelationship( null, NbtViewPropOwnerType.First, ThisProp, false );
                            //CswNbtViewRelationship R1 = View.MakeEmptyViewRelationship();
                            //R1.setProp( PropOwnerType.First, ThisProp );
                            //R1.setFirst( FirstVersionNodeType );
                            //R1.setSecond( FirstVersionNodeType );

                            //Relationships.Add( R1 );
                            _InsertRelationship( Relationships, R1 );

                            if( !Restrict )
                            {
                                // Copy it
                                //CswNbtViewRelationship R2 = new CswNbtViewRelationship( _CswNbtResources, View, R1 );
                                //R2.setProp( PropOwnerType.Second, ThisProp );
                                CswNbtViewRelationship R2 = View.AddViewRelationship( null, NbtViewPropOwnerType.Second, ThisProp, false );
                                //Relationships.Add( R2 );
                                _InsertRelationship( Relationships, R2 );
                            }
                        }
                    }
                    else if( ( PropRow["proptype"].ToString() == NbtViewPropIdType.NodeTypePropId.ToString() &&
                               PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) &&
                             ( PropRow["fktype"].ToString() == NbtViewRelatedIdType.ObjectClassId.ToString() &&
                               PropRow["fkvalue"].ToString() == ObjectClass.ObjectClassId.ToString() ) )
                    {
                        // Special case -- relationship to my own class
                        // We need to create two relationships from this

                        CswNbtViewRelationship R1 = View.AddViewRelationship( null, NbtViewPropOwnerType.First, ThisProp, false );
                        //R1.setProp( PropOwnerType.First, ThisProp );
                        R1.overrideFirst( FirstVersionNodeType );
                        R1.overrideSecond( ObjectClass );
                        //Relationships.Add( R1 );
                        _InsertRelationship( Relationships, R1 );

                        if( !Restrict )
                        {
                            // Copy it
                            //CswNbtViewRelationship R2 = new CswNbtViewRelationship( _CswNbtResources, View, R1 );
                            //R2.setProp( PropOwnerType.Second, ThisProp );
                            CswNbtViewRelationship R2 = View.AddViewRelationship( null, NbtViewPropOwnerType.Second, ThisProp, false );
                            R2.overrideFirst( FirstVersionNodeType );
                            R2.overrideSecond( ObjectClass );
                            //Relationships.Add( R2 );
                            _InsertRelationship( Relationships, R2 );
                        }
                    }
                    else
                    {
                        CswNbtViewRelationship R = null;
                        if( PropRow["proptype"].ToString() == NbtViewPropIdType.NodeTypePropId.ToString() &&
                            PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() )
                        {
                            // my relation to something else
                            R = View.AddViewRelationship( null, NbtViewPropOwnerType.First, ThisProp, false );
                            if( PropRow["fktype"].ToString() == NbtViewRelatedIdType.ObjectClassId.ToString() )
                                R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            else
                                R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );

                            if( R.SecondType != NbtViewRelatedIdType.NodeTypeId ||
                                _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, _CswNbtResources.MetaData.getNodeType( R.SecondId ) ) )
                            {
                                //Relationships.Add( R );
                                _InsertRelationship( Relationships, R );
                            }
                        }
                        else if( ( PropRow["fktype"].ToString() == NbtViewRelatedIdType.NodeTypeId.ToString() &&
                                   PropRow["fkvalue"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) ||
                                 ( PropRow["fktype"].ToString() == NbtViewRelatedIdType.ObjectClassId.ToString() &&
                                   PropRow["fkvalue"].ToString() == ObjectClass.ObjectClassId.ToString() ) )
                        {
                            if( !Restrict )
                            {
                                // something else's relation to me or my object class
                                R = View.AddViewRelationship( null, NbtViewPropOwnerType.Second, ThisProp, false );
                                if( PropRow["proptype"].ToString() == NbtViewPropIdType.ObjectClassPropId.ToString() )
                                    R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                else
                                    R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["typeid"] ) ) );

                                if( R.SecondType != NbtViewRelatedIdType.NodeTypeId ||
                                    _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, _CswNbtResources.MetaData.getNodeType( R.SecondId ) ) )
                                {
                                    //Relationships.Add( R );
                                    _InsertRelationship( Relationships, R );
                                }
                            }
                        }
                        else
                        {
                            throw new CswDniException( ErrorType.Error, "An unexpected data condition has occurred", "CswDataSourceNodeType.getRelatedNodeTypesAndObjectClasses found a relationship which did not match the original nodetypeid" );
                        }
                        if( R != null )
                            R.overrideFirst( FirstVersionNodeType );

                    }
                }
            }

            return Relationships;
        }

        private Collection<CswNbtViewRelationship> getObjectClassRelatedNodeTypesAndObjectClasses( Int32 ObjectClassId, CswNbtView View, Int32 Level )
        {
            Collection<CswNbtViewRelationship> Relationships = new Collection<CswNbtViewRelationship>();

            // If we're doing a grid, we can only pick things in which the provided nodetype has a relationship to, 
            // rather than things that are related to the provided nodetype.
            // If this is a property grid, then the above rule does not apply to the first level.
            bool Restrict = ( View.ViewMode == NbtViewRenderingMode.Grid || View.ViewMode == NbtViewRenderingMode.Table ) &&
                            ( View.Visibility != NbtViewVisibility.Property || Level >= 2 );

            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );

            CswStaticSelect RelationshipPropsSelect = _CswNbtResources.makeCswStaticSelect( "getRelationsForObjectClassId_select", "getRelationsForObjectClassId" );
            RelationshipPropsSelect.S4Parameters.Add( "getobjectclassid", new CswStaticParam( "getobjectclassid", ObjectClassId ) );
            DataTable RelationshipPropsTable = RelationshipPropsSelect.getTable();

            foreach( DataRow PropRow in RelationshipPropsTable.Rows )
            {
                // Ignore relationships that don't have a target
                if( PropRow["fktype"].ToString() != String.Empty &&
                     PropRow["fkvalue"].ToString() != String.Empty )
                {
                    if( ( PropRow["proptype"].ToString() == NbtViewPropIdType.ObjectClassPropId.ToString() &&
                          PropRow["typeid"].ToString() == ObjectClassId.ToString() ) &&
                        ( PropRow["fktype"].ToString() == NbtViewRelatedIdType.ObjectClassId.ToString() &&
                          PropRow["fkvalue"].ToString() == ObjectClassId.ToString() ) )
                    {
                        CswNbtMetaDataObjectClassProp ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );

                        // Special case -- relationship to my own class
                        // We need to create two relationships from this
                        CswNbtViewRelationship R1 = View.AddViewRelationship( null, NbtViewPropOwnerType.First, ThisProp, false );
                        R1.overrideFirst( ObjectClass );
                        R1.overrideSecond( ObjectClass );
                        //Relationships.Add( R1 );
                        _InsertRelationship( Relationships, R1 );

                        if( !Restrict )
                        {
                            // Copy it
                            //CswNbtViewRelationship R2 = new CswNbtViewRelationship( _CswNbtResources, View, R1 );
                            //R2.setProp( PropOwnerType.Second, ThisProp );
                            CswNbtViewRelationship R2 = View.AddViewRelationship( null, NbtViewPropOwnerType.Second, ThisProp, false );
                            R2.overrideFirst( ObjectClass );
                            R2.overrideSecond( ObjectClass );
                            //Relationships.Add( R2 );
                            _InsertRelationship( Relationships, R2 );
                        }
                    }
                    else
                    {
                        CswNbtViewRelationship R = null;
                        if( PropRow["proptype"].ToString() == NbtViewPropIdType.ObjectClassPropId.ToString() &&
                            PropRow["typeid"].ToString() == ObjectClassId.ToString() )
                        {
                            // my relation to something else
                            CswNbtMetaDataObjectClassProp ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );
                            R = View.AddViewRelationship( null, NbtViewPropOwnerType.First, ThisProp, false );
                            if( PropRow["fktype"].ToString() == NbtViewRelatedIdType.ObjectClassId.ToString() )
                                R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            else
                                R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            R.overrideFirst( ObjectClass );
                            //Relationships.Add( R );
                            _InsertRelationship( Relationships, R );
                        }
                        else if( PropRow["fktype"].ToString() == NbtViewRelatedIdType.ObjectClassId.ToString() && PropRow["fkvalue"].ToString() == ObjectClassId.ToString() )
                        {
                            if( !Restrict )
                            {
                                // something else's relation to me
                                if( PropRow["proptype"].ToString() == NbtViewPropIdType.ObjectClassPropId.ToString() )
                                {
                                    CswNbtMetaDataObjectClassProp ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );
                                    R = View.AddViewRelationship( null, NbtViewPropOwnerType.Second, ThisProp, false );
                                    R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                else
                                {
                                    CswNbtMetaDataNodeTypeProp ThisProp = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropRow["propid"] ) );
                                    R = View.AddViewRelationship( null, NbtViewPropOwnerType.Second, ThisProp, false );
                                    R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                R.overrideFirst( ObjectClass );
                                //Relationships.Add( R );
                                _InsertRelationship( Relationships, R );
                            }
                        }
                        else
                        {
                            throw new CswDniException( ErrorType.Error, "An unexpected data condition has occurred", "CswDataSourceObjectClass.getRelatedNodeTypesAndObjectClasses found a relationship which did not match the original objectclassid" );
                        }
                    }
                }
            }

            return Relationships;
        }

        private void _InsertRelationship( Collection<CswNbtViewRelationship> Relationships, CswNbtViewRelationship AddMe )
        {
            Int32 InsertAt = Relationships.Count;
            for( Int32 i = 0; i < Relationships.Count; i++ )
            {
                if( Relationships[i].SecondName.CompareTo( AddMe.SecondName ) > 0 ||
                    ( Relationships[i].SecondName.CompareTo( AddMe.SecondName ) == 0 &&
                      Relationships[i].PropName.CompareTo( AddMe.PropName ) >= 0 ) )
                {
                    InsertAt = i;
                    break;
                }
            }
            Relationships.Insert( InsertAt, AddMe );
        } // _InsertRelationship


        private ICollection _getNodeTypePropsCollection( Int32 NodeTypeId )
        {
            // Need to generate a set of all Props, including latest version props and
            // all historical ones from previous versions that are no longer included in the latest.
            SortedList PropsByName = new SortedList();
            SortedList PropsById = new SortedList();

            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
            CswNbtMetaDataNodeType ThisVersionNodeType = _CswNbtResources.MetaData.getNodeTypeLatestVersion( NodeType );
            while( ThisVersionNodeType != null )
            {
                foreach( CswNbtMetaDataNodeTypeProp ThisProp in from CswNbtMetaDataNodeTypeProp _ThisProp in ThisVersionNodeType.getNodeTypeProps() orderby _ThisProp.PropName select _ThisProp )
                {
                    //string ThisKey = ThisProp.PropName.ToLower(); //+ "_" + ThisProp.FirstPropVersionId.ToString();
                    if( !PropsByName.ContainsKey( ThisProp.PropNameWithQuestionNo.ToLower() ) &&
                        !PropsById.ContainsKey( ThisProp.FirstPropVersionId ) )
                    {
                        PropsByName.Add( ThisProp.PropNameWithQuestionNo.ToLower(), ThisProp );
                        PropsById.Add( ThisProp.FirstPropVersionId, ThisProp );
                    }
                }
                ThisVersionNodeType = ThisVersionNodeType.getPriorVersionNodeType();
            }
            return PropsByName.Values;
        }

        private ICollection _getObjectClassPropsCollection( Int32 ObjectClassId )
        {
            // Need to generate all properties on all nodetypes of this object class
            SortedList AllProps = new SortedList();
            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );
            foreach( CswNbtMetaDataNodeType NodeType in from CswNbtMetaDataNodeType _NodeType in ObjectClass.getNodeTypes() orderby _NodeType.NodeTypeName select _NodeType )
            {
                ICollection NodeTypeProps = _getNodeTypePropsCollection( NodeType.NodeTypeId );
                foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in from CswNbtMetaDataNodeTypeProp _NodeTypeProp in NodeTypeProps orderby _NodeTypeProp.PropName select _NodeTypeProp )
                {
                    string ThisKey = NodeTypeProp.PropName.ToLower(); //+ "_" + NodeTypeProp.FirstPropVersionId.ToString();
                    if( !AllProps.ContainsKey( ThisKey ) )
                        AllProps.Add( ThisKey, NodeTypeProp );
                }
            }
            return AllProps.Values;
        }

        #endregion Helper Functions

    } // class CswNbtWebServiceView

} // namespace ChemSW.Nbt.WebServices
