﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Web;
using ChemSW.Core;
using ChemSW.Exceptions;
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
        private CswGridData _CswGridData;
        private bool _CanEdit = true;
        private bool _CanDelete = true;
        private Collection<CswViewBuilderProp> _PropsInGrid = null;
        private string _FirstPropInGrid = string.Empty;
        public enum GridReturnType
        {
            Xml,
            Json
        };

        public CswNbtWebServiceGrid( CswNbtResources CswNbtResources, CswNbtView View, CswNbtNodeKey ParentNodeKey = null )
        {
            _CswNbtResources = CswNbtResources;
            _View = View;

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
                    foreach( CswNbtMetaDataNodeType NT in SecondOc.getNodeTypes() )
                    {
                        FirstLevelNodeTypes.Add( NT );
                    }
                }
                else if( Relationship.SecondType == NbtViewRelatedIdType.NodeTypeId &&
                         Relationship.SecondId != Int32.MinValue )
                {
                    FirstLevelNodeTypes.Add( _CswNbtResources.MetaData.getNodeType( Relationship.SecondId ) );
                }

                foreach( CswNbtMetaDataNodeType NodeType in FirstLevelNodeTypes )
                {
                    _CanEdit = ( _CanEdit &&
                                 _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Edit, NodeType ) );
                    _CanDelete = ( _CanDelete &&
                                   _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Delete, NodeType ) );
                    //exit if we already know both are false
                    if( !_CanEdit && !_CanDelete ) break;
                }

            }

            _CswGridData = new CswGridData( _CswNbtResources );
            _PropsInGrid = new Collection<CswViewBuilderProp>();
            _getGridProperties( _View.Root.ChildRelationships, _PropsInGrid );
        } //ctor

        public JObject runGrid( bool IncludeInQuickLaunch )
        {
            _View.SaveToCache( IncludeInQuickLaunch );
            return _getGridOuterJson();
        } // getGrid()

        private void _getGridProperties( Collection<CswNbtViewRelationship> ChildRelationships, Collection<CswViewBuilderProp> Ret )
        {
            CswCommaDelimitedString ColumnNames = new CswCommaDelimitedString();
            Collection<CswNbtViewProperty> PropsAtThisLevel = new Collection<CswNbtViewProperty>();
            Collection<CswNbtViewRelationship> NextChildRelationships = new Collection<CswNbtViewRelationship>();

            //Iterate all Relationships at this level first. This ensures our properties are properly collected.
            foreach( CswNbtViewRelationship Relationship in ChildRelationships )
            {
                foreach( CswNbtViewProperty Property in Relationship.Properties )
                {
                    PropsAtThisLevel.Add( Property );
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

        /// <summary>
        /// Returns a JSON Object of Column Names, Definition and Rows representing a jqGrid-consumable JSON object
        /// </summary>
        private JObject _getGridOuterJson()
        {
            JObject RetObj = new JObject();
            RetObj["nodetypeid"] = _View.ViewMetaDataTypeId;

            JArray GridOrderedColumnDisplayNames = _makeHiddenColumnNames();
            _AddIconColumnName( ref GridOrderedColumnDisplayNames );
            _CswGridData.getGridColumnNamesJson( GridOrderedColumnDisplayNames, _PropsInGrid );

            JArray GridColumnDefinitions = _CswGridData.getGridColumnDefinitionJson( _PropsInGrid );
            _AddIconColumnDefinition( ref GridColumnDefinitions );
            _AddHiddenColumnDefiniton( GridColumnDefinitions );

            _CswGridData.GridWidth = ( _View.Width * 7 );
            if( _View.Visibility != NbtViewVisibility.Property )
            {
                _CswGridData.GridTitle = _View.ViewName;
            }
            _CswGridData.CanEdit = _CanEdit;
            _CswGridData.CanDelete = _CanDelete;

            // Sort
            CswNbtViewProperty SortProp = _View.getSortProperty();
            if( SortProp != null )
            {
                _CswGridData.GridSortName = SortProp.NodeTypeProp.PropName.ToUpperInvariant().Replace( " ", "_" );
            }
            else
            {
                _CswGridData.GridSortName = "nodename";
            }

            RetObj["jqGridOpt"] = _CswGridData.makeJqGridJSON( GridOrderedColumnDisplayNames, GridColumnDefinitions, null );

            return RetObj;
        } // getGridOuterJson()

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

        public void ExportCsv(HttpContext Context)
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
        public JArray getThinGridRows( Int32 MaxRows )
        {
            JArray RetRows = new JArray();
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, false );
            if( _View.Visibility == NbtViewVisibility.Property )
            {
                Tree.goToNthChild( 0 );
            }

            Int32 NodeCount = Tree.getChildNodeCount();
            bool IsTruncated = false;
            if( NodeCount > 0 )
            {
                JArray HeaderRow = new JArray();
                CswCommaDelimitedString HeaderCols = new CswCommaDelimitedString();
                RetRows.Add( HeaderRow );
                foreach( CswViewBuilderProp VbProp in _PropsInGrid )
                {
                    HeaderRow.Add( VbProp.PropName );
                    HeaderCols.Add( VbProp.PropName );
                }

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
            return RetRows;
        } // getGridOuterJson()

        /// <summary>
        /// Returns a JSON Object of all Grid Rows
        /// </summary>
        public JObject getAllGridRows( bool IsReport )
        {
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, false );
            Int32 StartingNode = 0;
            Int32 EndingNode = Tree.getChildNodeCount();
            if( _View.Visibility == NbtViewVisibility.Property )
            {
                Tree.goToNthChild( 0 );
                EndingNode = Tree.getChildNodeCount();
            }
            return _getGridRows( Tree, 1, _CswGridData.PageSize, StartingNode, EndingNode, IsReport );
        } // getGridOuterJson()

        /// <summary>
        /// Returns a JSON Object of Grid Rows for a specific page
        /// </summary>
        public JObject getGridRowsByPage( Int32 PageNumber, Int32 PageSize, bool IsReport )
        {
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, false );
            Int32 StartingNode = PageSize * PageNumber;
            Int32 EndingNode = PageSize * ( PageNumber + 1 );
            return _getGridRows( Tree, PageNumber, PageSize, StartingNode, EndingNode, IsReport );
        } // getGridOuterJson()

        private JObject _getGridRows( ICswNbtTree Tree, Int32 PageNumber, Int32 PageSize, Int32 StartingNode, Int32 EndingNode, bool IsReport )
        {
            JObject RetObj = new JObject();
            JArray GridRows = new JArray();
            Int32 NodeCount = Tree.getChildNodeCount();
            bool IsTruncated = false;
            if( NodeCount > 0 )
            {
                for( Int32 C = StartingNode; ( C < EndingNode || IsReport ) && C < NodeCount; C += 1 )
                {
                    Tree.goToNthChild( C );

                    GridRows.Add( _getGridRow( Tree, _PropsInGrid ) );

                    IsTruncated = IsTruncated || Tree.getCurrentNodeChildrenTruncated();

                    Tree.goToParentNode();
                }

                if( IsTruncated )
                {
                    GridRows.Add( _getTruncatedGridRow( _PropsInGrid.First() ) );
                }

            }

            Int32 PageCount;
            if( IsReport )
            {
                PageCount = 1;
            }
            else
            {
                PageCount = ( ( NodeCount + PageSize - 1 ) / PageSize );
            }

            RetObj["total"] = PageCount;
            RetObj["page"] = PageNumber + 1;
            RetObj["records"] = NodeCount;
            RetObj["rows"] = GridRows;
            RetObj["wastruncated"] = IsTruncated;
            return RetObj;
        }

        /// <summary>
        /// Adds required columns for edit/add/delete functions
        /// </summary>
        private JArray _makeHiddenColumnNames()
        {
            JArray Ret = new JArray();
            Ret.Add( "jqgridid" ); //better to use int for jqGrid key
            Ret.Add( "cswnbtnodekey" ); //we'll want CswNbtNodeKey for add/edit/delete
            Ret.Add( "nodename" );
            return Ret;
        } // _makeHiddenColumnNames()

        /// <summary>
        /// Generates a JSON property with the definitional data for a jqGrid Column Array
        /// </summary>
        private void _AddHiddenColumnDefiniton( JArray ColumnDefArray )
        {
            //we'll want NodeName for edit/delete
            ColumnDefArray.AddFirst( new JObject(
                                new JProperty( "name", "nodename" ),
                                new JProperty( "index", "nodename" ),
                                new JProperty( "hidden", true )
                                ) );

            //we'll want CswNbtNodeKey for add/edit/delete
            ColumnDefArray.AddFirst( new JObject(
                                new JProperty( "name", "cswnbtnodekey" ),
                                new JProperty( "index", "cswnbtnodekey" ),
                                new JProperty( "hidden", true )
                                ) );

            //better to use int for jqGrid key
            ColumnDefArray.AddFirst( new JObject(
                                new JProperty( "name", "jqgridid" ),
                                new JProperty( "index", "jqgridid" ),
                                new JProperty( "key", true ),
                                new JProperty( "hidden", true )
                                ) );

        } // _AddHiddenColumnDefiniton()

        private void _AddIconColumnDefinition( ref JArray ColumnDefArray )
        {
            ColumnDefArray.AddFirst( new JObject(
                                new JProperty( "name", "Icon" ),
                                new JProperty( "index", "Icon" ),
                                new JProperty( "formatter", "image" ),
                                new JProperty( CswGridData.JqGridJsonOptions.width.ToString(), "40" )
                                ) );
        }
        private void _AddIconColumnName( ref JArray ColumnNameArray )
        {
            ColumnNameArray.Add( "Icon" );
        }

        private JObject _getGridRow( ICswNbtTree Tree, Collection<CswViewBuilderProp> PropsInGrid )
        {
            JObject ThisNodeObj = new JObject();

            CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();
            string ThisNodeName = Tree.getNodeNameForCurrentPosition();
            CswNbtMetaDataNodeType ThisNodeType = _CswNbtResources.MetaData.getNodeType( ThisNodeKey.NodeTypeId );
            string ThisNodeIcon = ThisNodeType.IconFileName;
            string ThisNodeKeyString = ThisNodeKey.ToString();
            string ThisNodeId = ThisNodeKey.NodeId.PrimaryKey.ToString();
            bool ThisNodeLocked = Tree.getNodeLockedForCurrentPosition();

            ThisNodeObj["jqgridid"] = ThisNodeId;
            ThisNodeObj["cswnbtnodekey"] = ThisNodeKeyString;
            string Icon = "<img src=\'";
            if( ThisNodeLocked )
            {
                Icon += "Images/quota/lock.gif\' title=\'Quota exceeded";
            }
            else
            {
                Icon += "Images/icons/" + ThisNodeIcon;
            }
            Icon += "\'/>";
            ThisNodeObj["Icon"] = Icon;
            ThisNodeObj["nodename"] = ThisNodeName;

            _addPropsRecursive( Tree, ThisNodeObj, PropsInGrid );

            return ThisNodeObj;

        } // _treeNodeJObject()

        private void _addPropsRecursive( ICswNbtTree Tree, JObject NodeObj, Collection<CswViewBuilderProp> PropsInGrid )
        {
            foreach( JObject Prop in Tree.getChildNodePropsOfNode() )
            {
                _addSafeCellContent( _CswNbtResources, Prop, NodeObj, PropsInGrid );
            }
            // Recurse
            for( Int32 i = 0; i < Tree.getChildNodeCount(); i++ )
            {
                Tree.goToNthChild( i );
                _addPropsRecursive( Tree, NodeObj, PropsInGrid );
                Tree.goToParentNode();
            }
        } // _addPropsRecursive()

        private JObject _getTruncatedGridRow( CswViewBuilderProp FirstPropInGrid )
        {
            JObject ThisNodeObj = new JObject();

            string ThisNodeName = "Truncated";

            ThisNodeObj["jqgridid"] = "-1";
            ThisNodeObj["cswnbtnodekey"] = string.Empty;
            string Icon = "<img src=\'";
            Icon += "Images/icons/truncated.gif";
            Icon += "\'/>";
            ThisNodeObj["Icon"] = Icon;
            ThisNodeObj["nodename"] = ThisNodeName;

            if( string.IsNullOrEmpty( _FirstPropInGrid ) )
            {
                _FirstPropInGrid = FirstPropInGrid.PropName + "_" + FirstPropInGrid.MetaDataPropId;
            }

            ThisNodeObj[_FirstPropInGrid] = "Results Truncated Here.";

            return ThisNodeObj;

        } // _treeNodeJObject()


        /// <summary>
        /// Translates property value into human readable text.
        /// Currently only handles Logical fieldtype.
        /// </summary>
        private void _addSafeCellContent( CswNbtResources CswNbtResources, JObject DirtyElement, JObject ParentObj, IEnumerable<CswViewBuilderProp> PropsInGrid )
        {
            if( null != DirtyElement )
            {
                string CleanPropName = DirtyElement["propname"].ToString().Trim().ToLower().Replace( " ", "_" );
                string DirtyValue = DirtyElement["gestalt"].ToString();
                string PropFieldTypeString = DirtyElement["fieldtype"].ToString();
                string PropId = DirtyElement["nodetypepropid"].ToString();
                CswNbtMetaDataNodeTypeProp Prop = CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropId ) );

                var PropFieldType = CswNbtMetaDataFieldType.getFieldTypeFromString( PropFieldTypeString );
                string CleanValue;
                switch( PropFieldType )
                {
                    case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                        CleanValue = CswConvert.ToDisplayString( CswConvert.ToTristate( DirtyValue ) );
                        break;
                    default:
                        CleanValue = DirtyValue;
                        break;
                }
                foreach( CswViewBuilderProp VbProp in PropsInGrid )
                {
                    if( Prop != null && VbProp.PropNameUnique == CleanPropName && 
                        ( VbProp.AssociatedPropIds.Contains( Prop.FirstPropVersionId ) ||
                          VbProp.AssociatedPropIds.Contains( Prop.ObjectClassPropId ) ) )
                    {
                        CleanPropName += "_" + VbProp.MetaDataPropId;
                    }
                }
                if( string.IsNullOrEmpty( _FirstPropInGrid ) )
                {
                    _FirstPropInGrid = CleanPropName;
                }
                ParentObj[CleanPropName] = CleanValue;
            }
        }

    } // class CswNbtWebServiceGrid

} // namespace ChemSW.Nbt.WebServices
