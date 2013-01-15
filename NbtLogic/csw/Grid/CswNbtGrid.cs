using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.Grid.ExtJs;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Grid
{
    /// <summary>
    /// Defines a grid
    /// </summary>
    public class CswNbtGrid
    {
        private CswNbtResources _CswNbtResources;
        public CswNbtGrid( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
        }

        private string _getUniquePrefix( CswNbtView View )
        {
            string ret = string.Empty;
            if( View.ViewId != null )
            {
                ret = View.ViewId.ToString();
            }
            else if( View.SessionViewId != null )
            {
                ret = View.SessionViewId.ToString();
            }
            return ret;
        } // _getUniquePrefix()

        public JObject TreeToJson( CswNbtView View, ICswNbtTree Tree, bool IsPropertyGrid = false, string GroupByCol = "" )
        {
            JObject Ret = new JObject();
            if( null != View )
            {
                string gridUniquePrefix = _getUniquePrefix( View );

                CswNbtGridExtJsGrid grid = new CswNbtGridExtJsGrid( gridUniquePrefix );
                if( string.IsNullOrEmpty( GroupByCol ) )
                {
                    GroupByCol = View.Root.GridGroupByCol;
                }
                grid.GroupByCol = GroupByCol;
                grid.title = View.ViewName;
                if( _CswNbtResources.CurrentNbtUser != null && _CswNbtResources.CurrentNbtUser.PageSize > 0 )
                {
                    grid.PageSize = _CswNbtResources.CurrentNbtUser.PageSize;
                }

                if( IsPropertyGrid && Tree.getChildNodeCount() > 0 )
                {
                    Tree.goToNthChild( 0 );
                }

                grid.Truncated = Tree.getCurrentNodeChildrenTruncated();

                CswNbtGridExtJsDataIndex nodeIdDataIndex = new CswNbtGridExtJsDataIndex( gridUniquePrefix, "nodeId" );
                CswNbtGridExtJsField nodeIdFld = new CswNbtGridExtJsField { dataIndex = nodeIdDataIndex };
                grid.fields.Add( nodeIdFld );
                CswNbtGridExtJsColumn nodeIdCol = new CswNbtGridExtJsColumn { header = "Internal ID", dataIndex = nodeIdDataIndex, hidden = true };
                grid.columns.Add( nodeIdCol );

                CswNbtGridExtJsDataIndex nodekeyDataIndex = new CswNbtGridExtJsDataIndex( gridUniquePrefix, "nodekey" );
                CswNbtGridExtJsField nodekeyFld = new CswNbtGridExtJsField { dataIndex = nodekeyDataIndex };
                grid.fields.Add( nodekeyFld );
                CswNbtGridExtJsColumn nodekeyCol = new CswNbtGridExtJsColumn { header = "Internal Key", dataIndex = nodekeyDataIndex, hidden = true };
                grid.columns.Add( nodekeyCol );

                CswNbtGridExtJsDataIndex nodenameDataIndex = new CswNbtGridExtJsDataIndex( gridUniquePrefix, "nodename" );
                CswNbtGridExtJsField nodenameFld = new CswNbtGridExtJsField { dataIndex = nodenameDataIndex };
                grid.fields.Add( nodenameFld );
                CswNbtGridExtJsColumn nodenameCol = new CswNbtGridExtJsColumn { header = "Internal Name", dataIndex = nodenameDataIndex, hidden = true };
                grid.columns.Add( nodenameCol );

                // View Properties determine Columns and Fields
                foreach( CswNbtViewProperty ViewProp in View.getOrderedViewProps( true ) )
                {
                    if( null != ViewProp )
                    {
                        ICswNbtMetaDataProp MetaDataProp = null;
                        if( ViewProp.Type == NbtViewPropType.NodeTypePropId )
                        {
                            MetaDataProp = ViewProp.NodeTypeProp;
                        }
                        else if( ViewProp.Type == NbtViewPropType.ObjectClassPropId )
                        {
                            MetaDataProp = ViewProp.ObjectClassProp;
                        }

                        // Because properties in the view might be by object class, but properties on the tree will always be by nodetype,
                        // we have to use name, not id, as the dataIndex
                        if( null != MetaDataProp )
                        {
                            string header = MetaDataProp.PropNameWithQuestionNo;
                            CswNbtGridExtJsDataIndex dataIndex = new CswNbtGridExtJsDataIndex( gridUniquePrefix, MetaDataProp.PropName ); // don't use PropNameWithQuestionNo here, because it won't match the propname from the tree

                            // Potential bug here!
                            // If the same property is added to the view more than once, we'll only use the grid definition for the first instance
                            if( false == grid.columnsContains( header ) )
                            {
                                CswNbtGridExtJsField fld = new CswNbtGridExtJsField { dataIndex = dataIndex };
                                CswNbtGridExtJsColumn col = new CswNbtGridExtJsColumn { header = header, dataIndex = dataIndex, hidden = ( false == ViewProp.ShowInGrid ) };
                                switch( ViewProp.FieldType )
                                {
                                    case CswNbtMetaDataFieldType.NbtFieldType.Number:
                                        fld.type = "number";
                                        col.xtype = extJsXType.numbercolumn;
                                        break;
                                    case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
                                        fld.type = "date";
                                        col.xtype = extJsXType.datecolumn;

                                        // case 26782 - Set dateformat as client date format
                                        string dateformat = string.Empty;
                                        string DateDisplayMode = CswNbtNodePropDateTime.DateDisplayMode.Date.ToString();
                                        if( ViewProp.Type == NbtViewPropType.NodeTypePropId && ViewProp.NodeTypeProp != null )
                                        {
                                            DateDisplayMode = ViewProp.NodeTypeProp.Extended;
                                        }
                                        else if( ViewProp.Type == NbtViewPropType.ObjectClassPropId && ViewProp.ObjectClassProp != null )
                                        {
                                            DateDisplayMode = ViewProp.ObjectClassProp.Extended;
                                        }
                                        if( DateDisplayMode == string.Empty ||
                                            DateDisplayMode == CswNbtNodePropDateTime.DateDisplayMode.Date.ToString() ||
                                            DateDisplayMode == CswNbtNodePropDateTime.DateDisplayMode.DateTime.ToString() )
                                        {
                                            dateformat += CswTools.DateFormatToExtJsDateFormat( _CswNbtResources.CurrentNbtUser.DateFormat );
                                            if( DateDisplayMode == CswNbtNodePropDateTime.DateDisplayMode.DateTime.ToString() )
                                            {
                                                dateformat += " ";
                                            }
                                        }
                                        if( DateDisplayMode == CswNbtNodePropDateTime.DateDisplayMode.Time.ToString() ||
                                            DateDisplayMode == CswNbtNodePropDateTime.DateDisplayMode.DateTime.ToString() )
                                        {
                                            dateformat += CswTools.DateFormatToExtJsDateFormat( _CswNbtResources.CurrentNbtUser.TimeFormat );
                                        }
                                        col.dateformat = dateformat;
                                        break;
                                }
                                if( ViewProp.Width > 0 )
                                {
                                    col.width = ViewProp.Width * 7; // approx. characters to pixels
                                }
                                grid.columns.Add( col );
                                grid.fields.Add( fld );
                            } // if( false == grid.columnsContains( header ) )
                        } // if(null != MetaDataProp )
                    } // if( ViewProp != null )
                } // foreach( CswNbtViewProperty ViewProp in View.getOrderedViewProps() )

                // Nodes in the Tree determine Rows
                for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
                {
                    CswNbtGridExtJsRow gridrow = new CswNbtGridExtJsRow( c );
                    Tree.goToNthChild( c );

                    gridrow.data.Add( nodeIdDataIndex, Tree.getNodeIdForCurrentPosition().ToString() );
                    gridrow.data.Add( nodekeyDataIndex, Tree.getNodeKeyForCurrentPosition().ToString() );
                    gridrow.data.Add( nodenameDataIndex, Tree.getNodeNameForCurrentPosition().ToString() );

                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Tree.getNodeKeyForCurrentPosition().NodeTypeId );
                    gridrow.canView = _CswNbtResources.Permit.isNodeWritable( Security.CswNbtPermit.NodeTypePermission.View,
                                                                              NodeType,
                                                                              NodeId: Tree.getNodeIdForCurrentPosition() );
                    gridrow.canEdit = ( _CswNbtResources.Permit.isNodeWritable( Security.CswNbtPermit.NodeTypePermission.Edit,
                                                                                NodeType,
                                                                                NodeId: Tree.getNodeIdForCurrentPosition() ) &&
                                        false == Tree.getNodeLockedForCurrentPosition() );
                    gridrow.canDelete = _CswNbtResources.Permit.isNodeWritable( Security.CswNbtPermit.NodeTypePermission.Delete,
                                                                                NodeType,
                                                                                NodeId: Tree.getNodeIdForCurrentPosition() );
                    gridrow.isLocked = Tree.getNodeLockedForCurrentPosition();
                    gridrow.isDisabled = ( false == Tree.getNodeIncludedForCurrentPosition() );

                    _TreeNodeToGrid( View, Tree, grid, gridrow );

                    Tree.goToParentNode();
                    grid.rows.Add( gridrow );

                }
                Ret = grid.ToJson();
            }
            return Ret;
        } // TreeToJson()

        private void _TreeNodeToGrid( CswNbtView View, ICswNbtTree Tree, CswNbtGridExtJsGrid grid, CswNbtGridExtJsRow gridrow )
        {
            string gridUniquePrefix = _getUniquePrefix( View );
            Collection<CswNbtTreeNodeProp> ChildProps = Tree.getChildNodePropsOfNode();

            foreach( CswNbtTreeNodeProp Prop in ChildProps )
            {
                // Potential bug here!
                // If the view defines the property by objectclass propname, but the nodetype propname differs, this might break
                CswNbtGridExtJsDataIndex dataIndex = new CswNbtGridExtJsDataIndex( gridUniquePrefix, Prop.PropName );

                bool IsHidden = Prop.Hidden;
                bool IsLocked = Tree.getNodeLockedForCurrentPosition();
                string newValue = string.Empty;
                if( false == IsHidden )
                {
                    CswPrimaryKey NodeId = Tree.getNodeIdForCurrentPosition();
                    CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( Prop.NodeTypePropId );

                    string oldValue = Prop.Gestalt;
                    if( string.IsNullOrEmpty( oldValue ) )
                    {
                        oldValue = null;
                    }

                    switch( Prop.FieldType )
                    {
                        case CswNbtMetaDataFieldType.NbtFieldType.Button:
                            if( false == IsLocked )
                            {
                                grid.buttons.Add( new CswNbtGridExtJsButton
                                {
                                    DataIndex = dataIndex.ToString(),
                                    RowNo = gridrow.RowNo,
                                    MenuOptions = "",
                                    SelectedText = oldValue ?? Prop.PropName,
                                    PropAttr = new CswPropIdAttr( NodeId, Prop.NodeTypePropId )
                                } );
                            }
                            break;
                        case CswNbtMetaDataFieldType.NbtFieldType.File:
                            string File = Prop.getPropColumnValue( MetaDataProp );
                            if( false == String.IsNullOrEmpty( File ) )
                            {
                                string LinkUrl = CswNbtNodePropBlob.getLink( Prop.JctNodePropId, NodeId, Prop.NodeTypePropId );
                                if( false == string.IsNullOrEmpty( LinkUrl ) )
                                {
                                    newValue = "<a target=\"blank\" href=\"" + LinkUrl + "\">" + ( oldValue ?? "File" ) + "</a>";
                                }
                            }
                            break;
                        case CswNbtMetaDataFieldType.NbtFieldType.Image:
                            string ImageUrl = CswNbtNodePropImage.getLink( Prop.JctNodePropId, NodeId, Prop.NodeTypePropId );
                            if( false == string.IsNullOrEmpty( ImageUrl ) )
                            {
                                newValue = "<a target=\"blank\" href=\"" + ImageUrl + "\">" + ( oldValue ?? "Image" ) + "</a>";
                            }
                            break;
                        case CswNbtMetaDataFieldType.NbtFieldType.Link:
                            string Href = Prop.getPropColumnValue( MetaDataProp );
                            if( false == string.IsNullOrEmpty( Href ) )
                            {
                                if( false == Href.Contains( "http" ) )
                                {
                                    Href = "http://" + Href;
                                }
                                newValue = "<a target=\"blank\" href=\"" + Href + "\">" + ( oldValue ?? "Link" ) + "</a>";
                            }
                            break;
                        case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                            newValue = CswConvert.ToDisplayString( CswConvert.ToTristate( oldValue ) );
                            break;
                        case CswNbtMetaDataFieldType.NbtFieldType.MOL:
                            string molUrl = CswNbtNodePropMol.getLink( Prop.JctNodePropId, NodeId, Prop.NodeTypePropId );
                            if( false == string.IsNullOrEmpty( molUrl ) )
                            {
                                newValue = "<a target=\"blank\" href=\"" + molUrl + "\">" + "Structure.jpg" + "</a>";
                            }
                            break;
                        default:
                            newValue = oldValue;
                            break;
                    }
                }
                gridrow.data[dataIndex] = newValue;
            } // foreach( JObject Prop in ChildProps )

            // Recurse, but add properties of child nodes to the same gridrow
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );
                _TreeNodeToGrid( View, Tree, grid, gridrow );
                Tree.goToParentNode();
            }
        } // _TreeNodeToGrid()


        public CswNbtGridExtJsGrid DataTableToGrid( DataTable DT, bool Editable = false, string GroupByCol = "" )
        {
            string gridUniquePrefix = DT.TableName;

            CswNbtGridExtJsGrid grid = new CswNbtGridExtJsGrid( gridUniquePrefix );
            grid.GroupByCol = GroupByCol;
            grid.title = DT.TableName;
            if( _CswNbtResources.CurrentNbtUser != null && _CswNbtResources.CurrentNbtUser.PageSize > 0 )
            {
                grid.PageSize = _CswNbtResources.CurrentNbtUser.PageSize;
            }

            foreach( DataColumn Column in DT.Columns )
            {
                CswNbtGridExtJsDataIndex dataIndex = new CswNbtGridExtJsDataIndex( gridUniquePrefix, Column.ColumnName );
                CswNbtGridExtJsField fld = new CswNbtGridExtJsField();
                fld.dataIndex = dataIndex;
                grid.fields.Add( fld );
                CswNbtGridExtJsColumn gridcol = new CswNbtGridExtJsColumn();
                gridcol.header = Column.ColumnName;
                gridcol.dataIndex = dataIndex;
                grid.columns.Add( gridcol );
            }

            Int32 RowNo = 0;
            foreach( DataRow Row in DT.Rows )
            {
                CswNbtGridExtJsRow gridrow = new CswNbtGridExtJsRow( RowNo );
                foreach( DataColumn Column in DT.Columns )
                {
                    gridrow.data[new CswNbtGridExtJsDataIndex( gridUniquePrefix, Column.ColumnName )] = Row[Column].ToString();
                }
                grid.rows.Add( gridrow );
                RowNo += 1;
            } // foreach( DataRow Row in DT.Rows )

            return grid;
        } // DataTableToGrid()

        public JObject DataTableToJSON( DataTable DT, bool Editable = false, string GroupByCol = "" )
        {
            CswNbtGridExtJsGrid grid = DataTableToGrid( DT, Editable, GroupByCol );
            return grid.ToJson();
        } // DataTableToJSON()


    } // class CswNbtGridExtJs
} // namespace ChemSW.Nbt.Grid
