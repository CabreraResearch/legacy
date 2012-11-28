using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Web;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Grid;
using ChemSW.Nbt.Logic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceGrid
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtView _View;
        private CswNbtNodeKey _ParentNodeKey;
        private CswNbtGrid _CswNbtGrid;
        private bool _ForReport = false;
        private bool _ActionEnabled = false;
        private Collection<CswViewBuilderProp> _PropsInGrid = null;

        private CswCommaDelimitedString _PropNamesOnDisplay = new CswCommaDelimitedString();
        private class _NodeTypePermission
        {
            public _NodeTypePermission( CswNbtMetaDataNodeType NodeType, CswNbtResources Resources )
            {
                NodeTypeId = NodeType.FirstVersionNodeTypeId;
                CanEdit = Resources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Edit, NodeType );
                CanView = Resources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.View, NodeType );
                CanDelete = Resources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Delete, NodeType );
                CanCreate = Resources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create, NodeType );
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
                    if( false == _Permissions.ContainsKey( NodeType.FirstVersionNodeTypeId ) )
                    {
                        _Permissions.Add( NodeType.FirstVersionNodeTypeId, Permission );
                    }
                }
            }

            _CswNbtGrid = new CswNbtGrid( _CswNbtResources );
            _PropsInGrid = new Collection<CswViewBuilderProp>();
            _getGridProperties( _View.Root.ChildRelationships, _PropsInGrid );
        } //ctor

        public JObject runGrid( bool IncludeInQuickLaunch, bool GetAllRowsNow = false, bool IsPropertyGrid = false, string GroupByCol = "" )
        {
            _View.SaveToCache( IncludeInQuickLaunch );
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, false, false, false );
            return _CswNbtGrid.TreeToJson( _View, Tree, IsPropertyGrid: ( IsPropertyGrid || _View.Visibility == NbtViewVisibility.Property ), GroupByCol: GroupByCol );
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

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, false, false, false );
            Tree.goToRoot();
            if( _View.Visibility == NbtViewVisibility.Property )
            {
                Tree.goToNthChild( 0 );
            }
            Int32 NodeCount = Tree.getChildNodeCount();
            bool IsTruncated = false;
            if( NodeCount > 0 )
            {
                foreach( CswViewBuilderProp VbProp in
                    from _VbProp
                        in _PropsInGrid
                    orderby _VbProp.ViewProp.Order, _VbProp.PropName
                    select _VbProp )
                {
                    if( false == DT.Columns.Contains( VbProp.PropName ) )
                    {
                        DT.Columns.Add( VbProp.PropName );
                    }
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

                    _recurse( Tree, DT, ref Row );

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

        private void _recurse( ICswNbtTree Tree, DataTable DT, ref DataRow Row )
        {
            int childNodeCount = Tree.getChildNodeCount();
            if( childNodeCount > 0 )
            {
                for( int i = 0; i < childNodeCount; i++ )
                {
                    Tree.goToNthChild( i );
                    foreach( JObject Prop in Tree.getChildNodePropsOfNode() )
                    {
                        if( DT.Columns.Contains( Prop["propname"].ToString() ) )
                        {
                            Row[Prop["propname"].ToString()] = Prop["gestalt"].ToString();
                        }
                    }
                    _recurse( Tree, DT, ref Row );
                    Tree.goToParentNode();
                }
            }
        }

        /// <summary>
        /// Returns a thin JArray of grid row values
        /// </summary>
        public JArray getThinGridRows( Int32 MaxRows, bool AlwaysShowHeader = false )
        {
            JArray RetRows = new JArray();
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, false, false, false );
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

        public JObject getGridRowCount()
        {
            JObject Ret = new JObject();
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, false, false, false );
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




    } // class CswNbtWebServiceGrid

} // namespace ChemSW.Nbt.WebServices
