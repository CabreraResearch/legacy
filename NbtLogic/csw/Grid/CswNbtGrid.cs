﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.Grid.ExtJs;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
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

        public JObject TreeToJson( string Title, CswNbtView View, ICswNbtTree Tree, bool IsPropertyGrid = false, string GroupByCol = "" )
        {
            JObject Ret = new JObject();
            if( null != View )
            {
                string gridUniquePrefix = _getUniquePrefix( View );

                CswExtJsGrid grid = new CswExtJsGrid( gridUniquePrefix );
                if( string.IsNullOrEmpty( GroupByCol ) )
                {
                    GroupByCol = View.GridGroupByCol;
                }
                grid.groupfield = GroupByCol;
                grid.title = Title;
                if( _CswNbtResources.CurrentNbtUser != null && _CswNbtResources.CurrentNbtUser.PageSize > 0 )
                {
                    grid.PageSize = _CswNbtResources.CurrentNbtUser.PageSize;
                }

                if( IsPropertyGrid && Tree.getChildNodeCount() > 0 )
                {
                    Tree.goToNthChild( 0 );
                }

                grid.Truncated = Tree.getCurrentNodeChildrenTruncated();

                CswExtJsGridDataIndex nodeIdDataIndex = new CswExtJsGridDataIndex( gridUniquePrefix, "nodeId" );
                {
                    CswExtJsGridField nodeIdFld = new CswExtJsGridField { dataIndex = nodeIdDataIndex };
                    grid.fields.Add( nodeIdFld );
                    CswExtJsGridColumn nodeIdCol = new CswExtJsGridColumn { header = "Internal ID", dataIndex = nodeIdDataIndex, hidden = true };
                    grid.columns.Add( nodeIdCol );
                }
                CswExtJsGridDataIndex nodekeyDataIndex = new CswExtJsGridDataIndex( gridUniquePrefix, "nodekey" );
                {
                    CswExtJsGridField nodekeyFld = new CswExtJsGridField { dataIndex = nodekeyDataIndex };
                    grid.fields.Add( nodekeyFld );
                    CswExtJsGridColumn nodekeyCol = new CswExtJsGridColumn { header = "Internal Key", dataIndex = nodekeyDataIndex, hidden = true };
                    grid.columns.Add( nodekeyCol );
                }
                CswExtJsGridDataIndex nodenameDataIndex = new CswExtJsGridDataIndex( gridUniquePrefix, "nodename" );
                {
                    CswExtJsGridField nodenameFld = new CswExtJsGridField { dataIndex = nodenameDataIndex };
                    grid.fields.Add( nodenameFld );
                    CswExtJsGridColumn nodenameCol = new CswExtJsGridColumn { header = "Internal Name", dataIndex = nodenameDataIndex, hidden = true };
                    grid.columns.Add( nodenameCol );
                }
                CswExtJsGridDataIndex NodeTypeDataIndex = new CswExtJsGridDataIndex( gridUniquePrefix, "nodetypeid" );
                {
                    CswExtJsGridField NodeTypeField = new CswExtJsGridField { dataIndex = NodeTypeDataIndex };
                    grid.fields.Add( NodeTypeField );
                    CswExtJsGridColumn NodeTypeColumn = new CswExtJsGridColumn { header = "Internal Type ID", dataIndex = NodeTypeDataIndex, hidden = true };
                    grid.columns.Add( NodeTypeColumn );
                }
                CswExtJsGridDataIndex ObjectClassDataIndex = new CswExtJsGridDataIndex( gridUniquePrefix, "objectclassid" );
                {
                    CswExtJsGridField ObjectClassField = new CswExtJsGridField { dataIndex = ObjectClassDataIndex };
                    grid.fields.Add( ObjectClassField );
                    CswExtJsGridColumn ObjectClassColumn = new CswExtJsGridColumn { header = "Internal Parent Type ID", dataIndex = ObjectClassDataIndex, hidden = true };
                    grid.columns.Add( ObjectClassColumn );
                }
                // View Properties determine Columns and Fields
                foreach( CswNbtViewProperty ViewProp in View.getOrderedViewProps( true ) )
                {
                    if( null != ViewProp )
                    {
                        ICswNbtMetaDataProp MetaDataProp = null;
                        if( ViewProp.Type == CswEnumNbtViewPropType.NodeTypePropId )
                        {
                            MetaDataProp = ViewProp.NodeTypeProp;
                        }
                        else if( ViewProp.Type == CswEnumNbtViewPropType.ObjectClassPropId )
                        {
                            MetaDataProp = ViewProp.ObjectClassProp;
                        }

                        // Because properties in the view might be by object class, but properties on the tree will always be by nodetype,
                        // we have to use name, not id, as the dataIndex
                        if( null != MetaDataProp )
                        {
                            string header = MetaDataProp.PropNameWithQuestionNo;
                            CswExtJsGridDataIndex dataIndex = new CswExtJsGridDataIndex( gridUniquePrefix, MetaDataProp.PropName ); // don't use PropNameWithQuestionNo here, because it won't match the propname from the tree

                            // Potential bug here!
                            // If the same property is added to the view more than once, we'll only use the grid definition for the first instance
                            if( false == grid.columnsContains( header ) )
                            {
                                CswExtJsGridField fld = new CswExtJsGridField { dataIndex = dataIndex };
                                CswExtJsGridColumn col = new CswExtJsGridColumn { header = header, dataIndex = dataIndex, hidden = ( false == ViewProp.ShowInGrid ) };
                                switch( ViewProp.FieldType )
                                {
                                    case CswEnumNbtFieldType.Button:
                                        col.MenuDisabled = true;
                                        col.IsSortable = false;
                                        break;
                                    case CswEnumNbtFieldType.Number:
                                        fld.type = "number";
                                        col.xtype = CswEnumExtJsXType.numbercolumn;
                                        break;
                                    case CswEnumNbtFieldType.DateTime:
                                        fld.type = "date";
                                        col.xtype = CswEnumExtJsXType.datecolumn;

                                        // case 26782 - Set dateformat as client date format
                                        string dateformat = string.Empty;
                                        string DateDisplayMode = CswEnumNbtDateDisplayMode.Date.ToString();
                                        if( ViewProp.Type == CswEnumNbtViewPropType.NodeTypePropId && ViewProp.NodeTypeProp != null )
                                        {
                                            DateDisplayMode = ViewProp.NodeTypeProp.Extended;
                                        }
                                        else if( ViewProp.Type == CswEnumNbtViewPropType.ObjectClassPropId && ViewProp.ObjectClassProp != null )
                                        {
                                            DateDisplayMode = ViewProp.ObjectClassProp.Extended;
                                        }
                                        if( DateDisplayMode == string.Empty ||
                                            DateDisplayMode == CswEnumNbtDateDisplayMode.Date.ToString() ||
                                            DateDisplayMode == CswEnumNbtDateDisplayMode.DateTime.ToString() )
                                        {
                                            dateformat += CswTools.ConvertNetToPHP( _CswNbtResources.CurrentNbtUser.DateFormat );
                                            if( DateDisplayMode == CswEnumNbtDateDisplayMode.DateTime.ToString() )
                                            {
                                                dateformat += " ";
                                            }
                                        }
                                        if( DateDisplayMode == CswEnumNbtDateDisplayMode.Time.ToString() ||
                                            DateDisplayMode == CswEnumNbtDateDisplayMode.DateTime.ToString() )
                                        {
                                            dateformat += CswTools.ConvertNetToPHP( _CswNbtResources.CurrentNbtUser.TimeFormat );
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
                    CswExtJsGridRow gridrow = new CswExtJsGridRow( c, gridUniquePrefix );
                    Tree.goToNthChild( c );

                    CswNbtTreeNode TreeNode = Tree.getCurrentTreeNode();

                    gridrow.data.Add( nodeIdDataIndex, Tree.getNodeIdForCurrentPosition().ToString() );
                    gridrow.data.Add( nodekeyDataIndex, Tree.getNodeKeyForCurrentPosition().ToString() );
                    gridrow.data.Add( nodenameDataIndex, Tree.getNodeNameForCurrentPosition().ToString() );
                    gridrow.data.Add( NodeTypeDataIndex, TreeNode.NodeTypeId.ToString() );
                    gridrow.data.Add( ObjectClassDataIndex, TreeNode.ObjectClassId.ToString() );

                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Tree.getNodeKeyForCurrentPosition().NodeTypeId );

                    gridrow.canView = _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, NodeType ) &&
                                      _CswNbtResources.Permit.isNodeWritable( CswEnumNbtNodeTypePermission.View, NodeType, Tree.getNodeIdForCurrentPosition() );
                    gridrow.canEdit = ( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Edit, NodeType ) &&
                                        ( _CswNbtResources.CurrentNbtUser.IsAdministrator() ||
                                          _CswNbtResources.Permit.isNodeWritable( CswEnumNbtNodeTypePermission.Edit,
                                                                                  NodeType,
                                                                                  NodeId: Tree.getNodeIdForCurrentPosition() ) ) &&
                                        false == Tree.getNodeLockedForCurrentPosition() );

                    gridrow.canDelete = ( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Delete,
                                                                                  NodeType ) &&
                                          _CswNbtResources.Permit.isNodeWritable( CswEnumNbtNodeTypePermission.Delete,
                                                                                  NodeType,
                                                                                  NodeId: Tree.getNodeIdForCurrentPosition() )
                                                                                  );
                    gridrow.isLocked = Tree.getNodeLockedForCurrentPosition();
                    gridrow.isDisabled = ( false == Tree.getNodeIncludedForCurrentPosition() );
                    if( null != _CswNbtResources.CurrentNbtUser )
                    {
                        gridrow.isFavorite = Tree.getNodeFavoritedForCurrentPosition();
                    }

                    _TreeNodeToGrid( View, Tree, grid, gridrow );

                    Tree.goToParentNode();
                    grid.rowData.rows.Add( gridrow );

                }
                Ret = grid.ToJson();
            }
            return Ret;
        } // TreeToJson()

        private void _TreeNodeToGrid( CswNbtView View, ICswNbtTree Tree, CswExtJsGrid grid, CswExtJsGridRow gridrow )
        {
            string gridUniquePrefix = _getUniquePrefix( View );
            Collection<CswNbtTreeNodeProp> ChildProps = Tree.getChildNodePropsOfNode();

            foreach( CswNbtTreeNodeProp Prop in ChildProps )
            {
                // Potential bug here!
                // If the view defines the property by objectclass propname, but the nodetype propname differs, this might break
                CswExtJsGridDataIndex dataIndex = new CswExtJsGridDataIndex( gridUniquePrefix, Prop.PropName );

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
                        case CswEnumNbtFieldType.Button:
                            if( false == IsLocked )
                            {
                                CswNbtFieldTypeRuleButton buttonFTR = (CswNbtFieldTypeRuleButton) MetaDataProp.getFieldTypeRule();
                                grid.rowData.btns.Add( new CswExtJsGridButton
                                {
                                    DataIndex = dataIndex.ToString(),
                                    RowNo = gridrow.RowNo,
                                    MenuOptions = Prop[buttonFTR.MenuOptionsSubField.Column],
                                    SelectedText = oldValue ?? Prop.PropName,
                                    PropAttr = new CswPropIdAttr( NodeId, Prop.NodeTypePropId ).ToString(),
                                    Mode = String.IsNullOrEmpty( MetaDataProp.Extended ) ? "button" : MetaDataProp.Extended
                                } );
                            }
                            break;
                        case CswEnumNbtFieldType.File:
                            string LinkUrl = CswNbtNodePropBlob.getLink( Prop.JctNodePropId, NodeId );
                            if( false == string.IsNullOrEmpty( LinkUrl ) && false == string.IsNullOrEmpty( oldValue ) )
                            {
                                newValue = "<a target=\"blank\" href=\"" + LinkUrl + "\">" + ( oldValue ) + "</a>";
                            }
                            break;
                        case CswEnumNbtFieldType.Image:
                            string ImageUrl = CswNbtNodePropImage.getLink( Prop.JctNodePropId, NodeId );
                            if( false == string.IsNullOrEmpty( ImageUrl ) )
                            {
                                newValue = "<a target=\"blank\" href=\"" + ImageUrl + "\">" + ( oldValue ?? "Image" ) + "</a>";
                            }
                            break;
                        case CswEnumNbtFieldType.Link:
                            
                            string Href = CswNbtNodePropLink.GetFullURL( MetaDataProp.Attribute1, Prop.Field1_Big, MetaDataProp.Attribute2 );
                            if( false == string.IsNullOrEmpty( Href ) )
                            {
                                newValue = "<a target=\"blank\" href=\"" + Href + "\">" + ( oldValue ?? "Link" ) + "</a>";
                            }
                            break;
                        case CswEnumNbtFieldType.Logical:
                            newValue = CswConvert.ToDisplayString( CswConvert.ToTristate( oldValue ) );
                            break;
                        case CswEnumNbtFieldType.MOL:
                            string molUrl = CswNbtNodePropMol.getLink( Prop.JctNodePropId, NodeId );
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


        public CswExtJsGrid DataTableToGrid( DataTable DT, bool Editable = false, string GroupByCol = "", CswEnumExtJsXType GroupByColType = null, bool IncludeEditFields = true, Dictionary<string, Type> ColumnTypeOverrides = null )
        {

            string NodeIdColName = "nodeid";
            string MenuOptionsColName = "menuoptions";
            string gridUniquePrefix = DT.TableName;

            CswExtJsGrid grid = new CswExtJsGrid( gridUniquePrefix, IncludeEditFields );

            grid.groupfield = GroupByCol;
            grid.title = DT.TableName;
            if( _CswNbtResources.CurrentNbtUser != null && _CswNbtResources.CurrentNbtUser.PageSize > 0 )
            {
                grid.PageSize = _CswNbtResources.CurrentNbtUser.PageSize;
            }

            //CswExtJsGridDataIndex nodeIdDataIndex = new CswExtJsGridDataIndex( gridUniquePrefix, NodeIdColName );
            //{
            //    CswExtJsGridField nodeIdFld = new CswExtJsGridField { dataIndex = nodeIdDataIndex };
            //    grid.fields.Add( nodeIdFld );
            //    CswExtJsGridColumn nodeIdCol = new CswExtJsGridColumn { header = "nodeId", dataIndex = nodeIdDataIndex, hidden = true };
            //    grid.columns.Add( nodeIdCol );
            //}


            foreach( DataColumn CurrentColumn in DT.Columns )
            {


                Type CurrentColumnType = null;
                if( null != ColumnTypeOverrides )
                {
                    if( false == ColumnTypeOverrides.ContainsKey( CurrentColumn.ColumnName ) )
                    {

                        CurrentColumnType = CurrentColumn.DataType;
                    }
                    else
                    {
                        CurrentColumnType = ColumnTypeOverrides[CurrentColumn.ColumnName];
                    }
                }
                else
                {
                    CurrentColumnType = CurrentColumn.DataType;

                }


                CswExtJsGridDataIndex dataIndex = new CswExtJsGridDataIndex( gridUniquePrefix, CurrentColumn.ColumnName );
                CswExtJsGridField fld = new CswExtJsGridField();
                grid.fields.Add( fld );
                fld.dataIndex = dataIndex;

                CswExtJsGridColumn gridcol = new CswExtJsGridColumn();
                gridcol.header = CurrentColumn.ColumnName;
                gridcol.dataIndex = dataIndex;
                gridcol.width = Math.Max( 100, ( 7 + CurrentColumn.ColumnName.Length*8 ) );

                if( ( NodeIdColName.ToLower() == CurrentColumn.ColumnName.ToLower() ) || ( MenuOptionsColName.ToLower() == CurrentColumn.ColumnName.ToLower() ) )
                {
                    gridcol.hidden = true;

                }

                if( CurrentColumnType == typeof( string ) )
                {
                    fld.type = "string";
                }
                else if( CurrentColumnType == typeof( bool ) )
                {
                    fld.type = "bool";
                    gridcol.xtype = CswEnumExtJsXType.booleancolumn;
                }
                else if( CurrentColumnType == typeof( Int32 ) ||
                    ( GroupByColType != null &&
                      CurrentColumn.ColumnName.ToLower().Equals( GroupByCol.ToLower() ) &&
                      GroupByColType.Equals( CswEnumExtJsXType.numbercolumn ) ) )
                {
                    fld.type = "number";
                    gridcol.xtype = CswEnumExtJsXType.numbercolumn;
                    gridcol.Format = "0";
                }
                else if( CurrentColumnType == typeof( DateTime ) ||
                    ( GroupByColType != null &&
                      CurrentColumn.ColumnName.ToLower().Equals( GroupByCol.ToLower() ) &&
                      GroupByColType.Equals( CswEnumExtJsXType.datecolumn ) ) )
                {
                    string userDateFormat = _CswNbtResources.CurrentNbtUser.DateFormat;
                    string userTimeFormat = _CswNbtResources.CurrentNbtUser.TimeFormat;
                    gridcol.dateformat = CswTools.ConvertNetToPHP( userDateFormat ) + " " + CswTools.ConvertNetToPHP( userTimeFormat );

                    fld.type = "date";
                    gridcol.xtype = CswEnumExtJsXType.datecolumn;
                    gridcol.Format = "m/d/y H:i:s";
                }
                else if( CurrentColumnType == typeof( sbyte ) ) //sbyte indidcates a "button" column :-( 
                {
                    gridcol.xtype = CswEnumExtJsXType.gridcolumn;
                    gridcol.MenuDisabled = true;
                    gridcol.IsSortable = false;
                }

                grid.columns.Add( gridcol );

            }

            Int32 RowNo = 0;
            foreach( DataRow Row in DT.Rows )
            {
                CswExtJsGridRow gridrow = new CswExtJsGridRow( RowNo, gridUniquePrefix );

                string NodeIdForGridRowData = string.Empty;
                foreach( DataColumn CurrentColumn in DT.Columns )
                {


                    Type CurrentColumnType = null;
                    if( null != ColumnTypeOverrides )
                    {
                        if( false == ColumnTypeOverrides.ContainsKey( CurrentColumn.ColumnName ) )
                        {

                            CurrentColumnType = CurrentColumn.DataType;
                        }
                        else
                        {
                            CurrentColumnType = ColumnTypeOverrides[CurrentColumn.ColumnName];
                        }
                    }
                    else
                    {
                        CurrentColumnType = CurrentColumn.DataType;

                    }


                    CswExtJsGridDataIndex index = null;

                    index = new CswExtJsGridDataIndex( gridUniquePrefix, CurrentColumn.ColumnName );
                    gridrow.data[index] = CswConvert.ToString( Row[CurrentColumn] );

                    if( Row.Table.Columns.Contains( NodeIdColName ) && DBNull.Value != Row[NodeIdColName] && typeof( sbyte ) == CurrentColumnType )
                    {
                        string MenuOptions = string.Empty;
                        if( Row.Table.Columns.Contains( MenuOptionsColName ) )
                        {
                            MenuOptions = Row[MenuOptionsColName].ToString();
                        }

                        NodeIdForGridRowData = Row[NodeIdColName].ToString();
                        CswExtJsGridButton CurrentButton = new CswExtJsGridButton
                        {
                            DataIndex = index.ToString(),
                            RowNo = RowNo,
                            MenuOptions = MenuOptions,
                            SelectedText = CurrentColumn.ColumnName,
                            PropAttr = NodeIdForGridRowData
                        };//nu the button

                        grid.rowData.btns.Add( CurrentButton );//add the button

                    }//if it's the hi-jacked data tabe that means BUTTON

                }//iterate collumns

                grid.rowData.rows.Add( gridrow );
                RowNo += 1;

            } // foreach( DataRow Row in DT.Rows )

            return grid;

        } // DataTableToGrid()

        public JObject DataTableToJSON( DataTable DT, bool Editable = false, string GroupByCol = "", CswEnumExtJsXType GroupByColType = null )
        {
            CswExtJsGrid grid = DataTableToGrid( DT, Editable, GroupByCol, GroupByColType );
            return grid.ToJson();
        } // DataTableToJSON()

    } // class CswNbtGridExtJs
} // namespace ChemSW.Nbt.Grid
