using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Web;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Logic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.Grid;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceGrid
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtView _View;
        private CswNbtNodeKey _ParentNodeKey;
        //private CswNbtActGrid _CswNbtActGrid;
        private CswNbtGrid _CswNbtGrid;
        private bool _ForReport = false;
        private bool _ActionEnabled = false;
        private Collection<CswViewBuilderProp> _PropsInGrid = null;
        private string _FirstPropInGrid = string.Empty;
        public enum GridReturnType
        {
            Xml,
            Json
        };
        private CswCommaDelimitedString _PropNamesOnDisplay = new CswCommaDelimitedString();
        private class _NodeTypePermission
        {
            public _NodeTypePermission( CswNbtMetaDataNodeType NodeType, CswNbtResources Resources )
            {
                NodeTypeId = NodeType.FirstVersionNodeTypeId;
                CanEdit = Resources.Permit.can( CswNbtPermit.NodeTypePermission.Edit, NodeType );
                CanView = Resources.Permit.can( CswNbtPermit.NodeTypePermission.View, NodeType );
                CanDelete = Resources.Permit.can( CswNbtPermit.NodeTypePermission.Delete, NodeType );
                CanCreate = Resources.Permit.can( CswNbtPermit.NodeTypePermission.Create, NodeType );
            }

            public Int32 NodeTypeId { get; private set; }
            public bool CanView { get; private set; }
            public bool CanEdit { get; private set; }
            public bool CanDelete { get; private set; }
            public bool CanCreate { get; private set; }
        }

        private Dictionary<Int32, _NodeTypePermission> _Permissions = new Dictionary<int, _NodeTypePermission>();

        public CswNbtWebServiceGrid( CswNbtResources CswNbtResources, CswNbtView View, bool ForReport, CswNbtNodeKey ParentNodeKey = null )
        {
            _CswNbtResources = CswNbtResources;
            _View = View;
            _ForReport = ForReport;

            if( _View.ViewMode != NbtViewRenderingMode.Grid )
            {
                throw new CswDniException( ErrorType.Error, "Cannot create a grid using a view type of " + _View.ViewMode, "Cannot create a grid view if the view is not a grid." );
            }

            _ParentNodeKey = ParentNodeKey;

            Collection<CswNbtViewRelationship> FirstLevelRelationships = new Collection<CswNbtViewRelationship>();
            if( null != _ParentNodeKey && _View.Visibility == NbtViewVisibility.Property )
            {
                foreach( CswNbtViewRelationship Relationship in _View.Root.ChildRelationships.SelectMany( NodeRelationship => NodeRelationship.ChildRelationships ) )
                {
                    FirstLevelRelationships.Add( Relationship );
                }
            }
            else
            {
                FirstLevelRelationships = _View.Root.ChildRelationships;
            }
            // Case 21778
            // Maybe do this in Permit someday; however, the meaning of Edit and Delete is very specific in this context:
            // only evaluating visibility of the option to edit or delete root nodetypes of a view
            foreach( CswNbtViewRelationship Relationship in FirstLevelRelationships )
            {
                Collection<CswNbtMetaDataNodeType> FirstLevelNodeTypes = new Collection<CswNbtMetaDataNodeType>();

                if( Relationship.SecondType == NbtViewRelatedIdType.ObjectClassId &&
                    Relationship.SecondId != Int32.MinValue )
                {
                    CswNbtMetaDataObjectClass SecondOc = _CswNbtResources.MetaData.getObjectClass( Relationship.SecondId );
                    if( null != SecondOc )
                    {
                        foreach( CswNbtMetaDataNodeType NT in SecondOc.getNodeTypes() )
                        {
                            FirstLevelNodeTypes.Add( NT );
                        }
                    }
                }
                else if( Relationship.SecondType == NbtViewRelatedIdType.NodeTypeId &&
                         Relationship.SecondId != Int32.MinValue )
                {
                    CswNbtMetaDataNodeType SecondNt = _CswNbtResources.MetaData.getNodeType( Relationship.SecondId );
                    if( null != SecondNt )
                    {
                        FirstLevelNodeTypes.Add( SecondNt );
                    }
                }

                foreach( CswNbtMetaDataNodeType NodeType in FirstLevelNodeTypes )
                {
                    _NodeTypePermission Permission = new _NodeTypePermission( NodeType, _CswNbtResources );
                    _ActionEnabled = false == _ForReport &&
                                     ( _ActionEnabled || Permission.CanView || Permission.CanDelete || Permission.CanDelete );
                    _Permissions.Add( NodeType.FirstVersionNodeTypeId, Permission );
                }
            }

            _CswNbtGrid = new CswNbtGrid( _CswNbtResources );
            _PropsInGrid = new Collection<CswViewBuilderProp>();
            _getGridProperties( _View.Root.ChildRelationships, _PropsInGrid );
        } //ctor

        public JObject runGrid( bool IncludeInQuickLaunch, bool GetAllRowsNow = false )
        {
            _View.SaveToCache( IncludeInQuickLaunch );
            //JObject RetObj = _getGridOuterJson();
            //if( GetAllRowsNow )
            //{
            //    RetObj["data"] = getAllGridRows();
            //}
            //return RetObj;
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, false );
            return _CswNbtGrid.TreeToJson( _View, Tree, ( _View.Visibility == NbtViewVisibility.Property ) );
        } // runGrid()

        private void _getGridProperties( IEnumerable<CswNbtViewRelationship> ChildRelationships, Collection<CswViewBuilderProp> Ret )
        {
            CswCommaDelimitedString ColumnNames = new CswCommaDelimitedString();
            Collection<CswNbtViewProperty> PropsAtThisLevel = new Collection<CswNbtViewProperty>();
            Collection<CswNbtViewRelationship> NextChildRelationships = new Collection<CswNbtViewRelationship>();

            //Iterate all Relationships at this level first. This ensures our properties are properly collected.
            foreach( CswNbtViewRelationship Relationship in ChildRelationships )
            {
                foreach( CswNbtViewProperty Property in from CswNbtViewProperty _Property
                                                        in Relationship.Properties
                                                        orderby _Property.Order, _Property.Name
                                                        where _Property.ShowInGrid
                                                        select _Property )
                {
                    PropsAtThisLevel.Add( Property );
                    _PropNamesOnDisplay.Add( Property.Name );
                }
                //This will make recursion smoother: we're always iterating the collection of relationships at the same distance from root.
                foreach( CswNbtViewRelationship ChildRelationship in Relationship.ChildRelationships )
                {
                    NextChildRelationships.Add( ChildRelationship );
                }
            }
            //Now iterate props
            foreach( CswNbtViewProperty Prop in PropsAtThisLevel )
            {
                CswViewBuilderProp VbProp = new CswViewBuilderProp( Prop );
                string PropName = Prop.Name.Trim().Replace( " ", "_" ).ToLower();
                if( false == ColumnNames.Contains( PropName ) )
                {
                    ColumnNames.Add( PropName );
                    Ret.Add( VbProp );

                }
                else
                {
                    //The Tree XML won't give us anything to map a "duplicate" prop/column back to the column definition. Leave our own breadcrumbs.
                    foreach( CswViewBuilderProp RetProp in Ret )
                    {
                        if( RetProp.PropNameUnique == PropName && false == RetProp.AssociatedPropIds.Contains( VbProp.MetaDataPropId.ToString() ) )
                        {
                            RetProp.AssociatedPropIds.Add( VbProp.MetaDataPropId.ToString() );
                        }
                    }
                }
            }

            //Now recurse, damn you.
            if( NextChildRelationships.Count > 0 )
            {
                _getGridProperties( NextChildRelationships, Ret );
            }
        }

        ///// <summary>
        ///// Returns a JSON Object of Column Names, Definition and Rows representing a jqGrid-consumable JSON object
        ///// </summary>
        //private JObject _getGridOuterJson()
        //{
        //    JObject RetObj = new JObject();
        //    RetObj["nodetypeid"] = _View.ViewMetaDataTypeId;

        //    JArray GridOrderedColumnDisplayNames = _makeDefaultColumnNames();
        //    _CswNbtActGrid.getGridColumnNamesJson( GridOrderedColumnDisplayNames, _PropsInGrid );

        //    JArray GridColumnDefinitions = _CswNbtActGrid.getGridColumnDefinitionJson( _PropsInGrid );
        //    _addDefaultColumnDefiniton( GridColumnDefinitions );

        //    if( _View.Visibility != NbtViewVisibility.Property )
        //    {
        //        _CswNbtActGrid.GridTitle = _View.ViewName;
        //    }

        //    // Sort
        //    CswNbtViewProperty SortProp = _View.getSortProperty();
        //    if( SortProp != null )
        //    {
        //        if( null != SortProp.NodeTypeProp )
        //        {
        //            _CswNbtActGrid.GridSortName = SortProp.NodeTypeProp.PropName.ToUpperInvariant().Replace( " ", "_" );
        //        }
        //        else if( null != SortProp.ObjectClassProp )
        //        {
        //            _CswNbtActGrid.GridSortName = SortProp.ObjectClassProp.PropName.ToUpperInvariant().Replace( " ", "_" );
        //        }
        //        else
        //        {
        //            _CswNbtActGrid.GridSortName = "nodename";
        //        }
        //    }
        //    else
        //    {
        //        _CswNbtActGrid.GridSortName = "nodename";
        //    }

        //    RetObj["jqGridOpt"] = _CswNbtActGrid.makeJqGridJSON( GridOrderedColumnDisplayNames, GridColumnDefinitions, null );

        //    return RetObj;
        //} // getGridOuterJson()

        private void _ensureIndex( JArray Array, Int32 Position )
        {
            if( Position >= Array.Count )
            {
                for( Int32 I = Array.Count; I <= Position; I += 1 )
                {
                    Array.Add( "" );
                }
            }

        }

        public void ExportCsv( HttpContext Context )
        {
            DataTable DT = new DataTable();

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, false );
            Tree.goToRoot();
            if( _View.Visibility == NbtViewVisibility.Property )
            {
                Tree.goToNthChild( 0 );
            }
            Int32 NodeCount = Tree.getChildNodeCount();
            bool IsTruncated = false;
            if( NodeCount > 0 )
            {
                foreach( CswViewBuilderProp VbProp in _PropsInGrid )
                {
                    DT.Columns.Add( VbProp.PropName );
                }

                for( Int32 C = 0; C < NodeCount; C += 1 )
                {
                    Tree.goToNthChild( C );

                    DataRow Row = DT.NewRow();
                    foreach( JObject Prop in Tree.getChildNodePropsOfNode() )
                    {
                        if( DT.Columns.Contains( Prop["propname"].ToString() ) )
                        {
                            Row[Prop["propname"].ToString()] = Prop["gestalt"].ToString();
                        }
                    }
                    DT.Rows.Add( Row );

                    IsTruncated = IsTruncated || Tree.getCurrentNodeChildrenTruncated();

                    Tree.goToParentNode();
                }

                if( IsTruncated )
                {
                    DataRow Row = DT.NewRow();
                    Row[DT.Columns[0]] = "Results Truncated";
                    DT.Rows.Add( Row );
                }
            }

            wsTools.ReturnCSV( Context, DT );
        } // ExportCsv()

        /// <summary>
        /// Returns a thin JArray of grid row values
        /// </summary>
        public JArray getThinGridRows( Int32 MaxRows, bool AlwaysShowHeader = false )
        {
            JArray RetRows = new JArray();
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, false );
            if( _View.Visibility == NbtViewVisibility.Property )
            {
                Tree.goToNthChild( 0 );
            }

            Int32 NodeCount = Tree.getChildNodeCount();
            if( MaxRows == Int32.MinValue )
            {
                MaxRows = NodeCount;
            }
            bool IsTruncated = false;
            if( AlwaysShowHeader || NodeCount > 0 )
            {
                JArray HeaderRow = new JArray();
                CswCommaDelimitedString HeaderCols = new CswCommaDelimitedString();
                RetRows.Add( HeaderRow );
                foreach( CswViewBuilderProp VbProp in _PropsInGrid )
                {
                    HeaderRow.Add( VbProp.PropName );
                    HeaderCols.Add( VbProp.PropName );
                }

                if( NodeCount > 0 )
                {
                    for( Int32 C = 0; C < MaxRows && C < NodeCount; C += 1 )
                    {
                        Tree.goToNthChild( C );

                        JArray ThisRow = new JArray();
                        RetRows.Add( ThisRow );
                        foreach( JObject Prop in Tree.getChildNodePropsOfNode() )
                        {
                            Int32 ColumnIdx = HeaderCols.IndexOf( Prop["propname"].ToString() );
                            if( ColumnIdx >= 0 )
                            {
                                _ensureIndex( ThisRow, ColumnIdx );
                                ThisRow[ColumnIdx] = Prop["gestalt"];
                            }
                        }

                        IsTruncated = IsTruncated || Tree.getCurrentNodeChildrenTruncated();

                        Tree.goToParentNode();
                    }

                    if( IsTruncated )
                    {
                        RetRows.Add( new JArray( "Results Truncated" ) );
                    }
                }
            }
            return RetRows;
        } // getGridOuterJson()

        ///// <summary>
        ///// Returns a JSON Object of all Grid Rows
        ///// </summary>
        //public JObject getAllGridRows()
        //{
        //    JObject Ret = new JObject();
        //    ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, false );
        //    Int32 StartingNode = 0;
        //    Int32 EndingNode = Tree.getChildNodeCount();
        //    if( _View.Visibility == NbtViewVisibility.Property &&
        //        EndingNode > 0 )
        //    {
        //        Tree.goToNthChild( 0 );
        //        EndingNode = Tree.getChildNodeCount();
        //    }
        //    if( EndingNode > 0 )
        //    {
        //        Ret = _getGridRows( Tree, 1, _CswNbtActGrid.PageSize, StartingNode, EndingNode );
        //    }
        //    return Ret;
        //} // getGridOuterJson()


        public JObject getGridRowCount()
        {
            JObject Ret = new JObject();
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, false );
            Int32 rowCount = Tree.getChildNodeCount();
            if( _View.Visibility == NbtViewVisibility.Property &&
                rowCount > 0 )
            {
                Tree.goToNthChild( 0 );
                rowCount = Tree.getChildNodeCount();
            }
            Ret["rowCount"] = rowCount;
            return Ret;
        } // getGridOuterJson()


        //private JObject _getGridRows( ICswNbtTree Tree, Int32 PageNumber, Int32 PageSize, Int32 StartingNode, Int32 EndingNode )
        //{
        //    JObject RetObj = new JObject();
        //    JArray GridRows = new JArray();
        //    Int32 NodeCount = Tree.getChildNodeCount();
        //    bool IsTruncated = false;
        //    if( NodeCount > 0 )
        //    {
        //        for( Int32 C = StartingNode; ( C < EndingNode || _ForReport ) && C < NodeCount; C += 1 )
        //        {
        //            Tree.goToNthChild( C );


        //            GridRows.Add( _getGridRow( Tree, _PropsInGrid ) );

        //            IsTruncated = IsTruncated || Tree.getCurrentNodeChildrenTruncated();

        //            Tree.goToParentNode();
        //        }

        //        if( IsTruncated )
        //        {
        //            GridRows.Add( _getTruncatedGridRow( _PropsInGrid.First() ) );
        //        }

        //    }

        //    Int32 PageCount;
        //    if( _ForReport )
        //    {
        //        PageCount = 1;
        //    }
        //    else
        //    {
        //        PageCount = ( ( NodeCount + PageSize - 1 ) / PageSize );
        //    }

        //    RetObj["total"] = PageCount;
        //    RetObj["page"] = PageNumber + 1;
        //    RetObj["records"] = NodeCount;
        //    RetObj["rows"] = GridRows;
        //    RetObj["wastruncated"] = IsTruncated;
        //    return RetObj;
        //}

        ///// <summary>
        ///// Adds required columns for edit/add/delete functions
        ///// </summary>
        //private JArray _makeDefaultColumnNames()
        //{
        //    JArray Ret = new JArray();
        //    Ret.Add( "jqgridid" ); //better to use int for jqGrid key
        //    Ret.Add( "cswnbtnodekey" ); //we'll want CswNbtNodeKey for add/edit/delete
        //    Ret.Add( "nodename" );
        //    if( _ActionEnabled )
        //    {
        //        Ret.Add( "Action" );
        //    }

        ///// <summary>
        ///// Generates a JSON property with the definitional data for a jqGrid Column Array
        ///// </summary>
        //private void _addDefaultColumnDefiniton( JArray ColumnDefArray )
        //{
        //    if( _ActionEnabled )
        //    {
        //        ColumnDefArray.AddFirst( new JObject(
        //                                    new JProperty( "name", "Action" ),
        //                                    new JProperty( "index", "Action" ),
        //                                    new JProperty( "formatter", "image" ),
        //                                    new JProperty( "fixed", true ),
        //                                    new JProperty( CswNbtActGrid.JqGridJsonOptions.width.ToString(), 66 )
        //                                    ) );
        //    }

        //    //we'll want NodeName for edit/delete
        //    ColumnDefArray.AddFirst( new JObject(
        //                        new JProperty( "name", "nodename" ),
        //                        new JProperty( "index", "nodename" ),
        //                        new JProperty( "hidden", true ),
        //                        new JProperty( CswNbtActGrid.JqGridJsonOptions.width.ToString(), 0 )
        //                        ) );

        //    //we'll want CswNbtNodeKey for add/edit/delete
        //    ColumnDefArray.AddFirst( new JObject(
        //                        new JProperty( "name", "cswnbtnodekey" ),
        //                        new JProperty( "index", "cswnbtnodekey" ),
        //                        new JProperty( "hidden", true ),
        //                        new JProperty( CswNbtActGrid.JqGridJsonOptions.width.ToString(), 0 )
        //                        ) );

        //    //better to use int for jqGrid key
        //    ColumnDefArray.AddFirst( new JObject(
        //                        new JProperty( "name", "jqgridid" ),
        //                        new JProperty( "index", "jqgridid" ),
        //                        new JProperty( "key", true ),
        //                        new JProperty( "hidden", true ),
        //                        new JProperty( CswNbtActGrid.JqGridJsonOptions.width.ToString(), 0 )
        //                        ) );

        //} // _addDefaultColumnDefiniton()

        //private JObject _getGridRow( ICswNbtTree Tree, Collection<CswViewBuilderProp> PropsInGrid )
        //{
        //    JObject ThisNodeObj = new JObject();

        //    CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();
        //    string ThisNodeName = Tree.getNodeNameForCurrentPosition();
        //    CswNbtMetaDataNodeType ThisNodeType = _CswNbtResources.MetaData.getNodeType( ThisNodeKey.NodeTypeId );
        //    if( null == ThisNodeType )
        //    {
        //        CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
        //        if( null != ThisNode )
        //        {
        //            ThisNodeType = ThisNode.getNodeType();
        //        }
        //    }
        //    if( null != ThisNodeType )
        //    {
        //        bool ActionEnabled = false == _ForReport &&
        //                             _Permissions.ContainsKey( ThisNodeType.FirstVersionNodeTypeId );
        //        bool CanView = ActionEnabled &&
        //                       _Permissions[ThisNodeType.FirstVersionNodeTypeId].CanView;
        //        bool CanEdit = ActionEnabled &&
        //                       _Permissions[ThisNodeType.FirstVersionNodeTypeId].CanEdit;
        //        bool CanDelete = ActionEnabled &&
        //                       _Permissions[ThisNodeType.FirstVersionNodeTypeId].CanDelete;
        /*bool CanCopy = ActionEnabled &&
//        string ThisNodeKeyString = ThisNodeKey.ToString();
//        string ThisNodeId = ThisNodeKey.NodeId.PrimaryKey.ToString();
//        JArray Actions = new JArray();

//        if( Tree.getNodeLockedForCurrentPosition() )
//        {
//            Actions.Add( "islocked" );
//            if( CanView )
//            {
//                Actions.Add( "canview" );
//            }
//        }
//        else
//        {
//            if( CanEdit )
//            {
//                Actions.Add( "canedit" );
//            }
//            else if( CanView )
//            {
//                Actions.Add( "canview" );
//            }
//            /*if( CanCopy )
//            {
//                Actions.Add( "cancopy" );
//            }*/
        //        }

        //        ThisNodeObj["jqgridid"] = ThisNodeId;
        //        ThisNodeObj["cswnbtnodekey"] = ThisNodeKeyString;

        //        //ThisNodeObj["Icon"] = Icon;
        //        ThisNodeObj["nodename"] = ThisNodeName;

        //        _addPropsRecursive( Tree, ThisNodeObj, PropsInGrid, ThisNodeKey );
        //    }
        //    return ThisNodeObj;

        //} // _treeNodeJObject()

        //private void _addPropsRecursive( ICswNbtTree Tree, JObject NodeObj, Collection<CswViewBuilderProp> PropsInGrid, CswNbtNodeKey NodeKey )
        //{
        //    foreach( JObject Prop in Tree.getChildNodePropsOfNode() )
        //    {
        //        string PropName = Prop["propname"].ToString();
        //        if( _PropNamesOnDisplay.Contains( PropName ) )
        //        {
        //            _addSafeCellContent( _CswNbtResources, Prop, NodeObj, PropsInGrid, NodeKey );
        //         }
        //        Tree.goToNthChild( i );
        //        _addPropsRecursive( Tree, NodeObj, PropsInGrid, NodeKey );
        //        Tree.goToParentNode();
        //    }
        //} // _addPropsRecursive()

        //private JObject _getTruncatedGridRow( CswViewBuilderProp FirstPropInGrid )
        //{
        //    JObject ThisNodeObj = new JObject();

        //    string ThisNodeName = "Truncated";

        //    ThisNodeObj["jqgridid"] = "-1";
        //    ThisNodeObj["cswnbtnodekey"] = string.Empty;
        //    string Icon = "<img src=\'";
        //    Icon += "Images/icons/truncated.gif";
        //    Icon += "\'/>";
        //    ThisNodeObj["Icon"] = Icon;
        //    ThisNodeObj["nodename"] = ThisNodeName;

        //    if( string.IsNullOrEmpty( _FirstPropInGrid ) )
        //    {
        //        _FirstPropInGrid = FirstPropInGrid.PropName + "_" + FirstPropInGrid.MetaDataPropId;
        //    }

        //    ThisNodeObj[_FirstPropInGrid] = "Results Truncated Here.";

        //    return ThisNodeObj;

        //} // _treeNodeJObject()


        ///// <summary>
        ///// Translates property value into human readable text.
        ///// Currently only handles Logical fieldtype.
        ///// </summary>
        //private void _addSafeCellContent( CswNbtResources CswNbtResources, JObject TreePropObj, JObject RetObj, IEnumerable<CswViewBuilderProp> PropsInGrid, CswNbtNodeKey NodeKey )
        //{
        //    if( null != TreePropObj )
        //    {
        //        string CleanPropName = TreePropObj["propname"].ToString().Trim().ToLower().Replace( " ", "_" );
        //        string DirtyValue = TreePropObj["gestalt"].ToString();
        //        string PropFieldTypeString = TreePropObj["fieldtype"].ToString();
        //        Int32 PropId = CswConvert.ToInt32( TreePropObj["nodetypepropid"] );
        //        Int32 JctNodePropId = CswConvert.ToInt32( TreePropObj["jctnodepropid"] );
        //        CswNbtMetaDataNodeTypeProp Prop = CswNbtResources.MetaData.getNodeTypeProp( PropId );

        //        var PropFieldType = CswNbtMetaDataFieldType.getFieldTypeFromString( PropFieldTypeString );
        //        string CleanValue = "";
        //        string UrlString = "";
        //        switch( PropFieldType )
        //        {
        //            case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
        //                CleanValue = default( string );
        //                DateTime Date = CswConvert.ToDateTime( DirtyValue );
        //                if( DateTime.MinValue != Date )
        //                {
        //                    CswDateTime CswDate = new CswDateTime( CswNbtResources, Date, CswDateTime.DateFormat.yyyyMMdd_Dashes, CswDateTime.TimeFormat.Hmmss );
        //                    CleanValue = CswDate.ToClientAsDateString();
        //                }
        //                break;
        //            case CswNbtMetaDataFieldType.NbtFieldType.File:
        //                UrlString = CswNbtNodePropBlob.getLink( JctNodePropId, NodeKey.NodeId, PropId );
        //                if( false == string.IsNullOrEmpty( UrlString ) )
        //                {
        //                    if( string.IsNullOrEmpty( DirtyValue ) )
        //                    {
        //                        DirtyValue = "File";
        //                    }
        //                    CleanValue = "<a href='" + UrlString + "'>" + DirtyValue + "</a>";
        //                }
        //                break;
        //            case CswNbtMetaDataFieldType.NbtFieldType.Image:
        //                UrlString = CswNbtNodePropImage.getLink( JctNodePropId, NodeKey.NodeId, PropId );
        //                if( false == string.IsNullOrEmpty( UrlString ) )
        //                {
        //                    if( string.IsNullOrEmpty( DirtyValue ) )
        //                    {
        //                        DirtyValue = "Image";
        //                    }
        //                    CleanValue = "<a href='" + UrlString + "'>" + DirtyValue + "</a>";
        //                }
        //                break;
        //            case CswNbtMetaDataFieldType.NbtFieldType.Logical:
        //                CleanValue = CswConvert.ToDisplayString( CswConvert.ToTristate( DirtyValue ) );
        //                break;
        //            default:
        //                CleanValue = DirtyValue;
        //                break;
        //        }
        //        foreach( CswViewBuilderProp VbProp in PropsInGrid )
        //        {
        //            if( Prop != null && VbProp.PropNameUnique == CleanPropName &&
        //                ( VbProp.AssociatedPropIds.Contains( Prop.FirstPropVersionId ) ||
        //                  VbProp.AssociatedPropIds.Contains( Prop.ObjectClassPropId ) ) )
        //            {
        //                CleanPropName += "_" + VbProp.MetaDataPropId;
        //            }
        //        }
        //        if( string.IsNullOrEmpty( _FirstPropInGrid ) )
        //        {
        //            _FirstPropInGrid = CleanPropName;
        //        }
        //        RetObj[CleanPropName] = CleanValue;
        //    }
        //}

    } // class CswNbtWebServiceGrid

} // namespace ChemSW.Nbt.WebServices
