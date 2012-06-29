﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.Data;
using System.Linq;
using System.Reflection;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.Grid.ExtJs;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;

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

        public JObject TreeToJson( CswNbtView View, ICswNbtTree Tree, bool IsPropertyGrid = false )
        {
            CswNbtGridExtJsGrid grid = new CswNbtGridExtJsGrid();
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

            CswNbtGridExtJsDataIndex nodeIdDataIndex = new CswNbtGridExtJsDataIndex( "nodeId" );
            CswNbtGridExtJsField nodeIdFld = new CswNbtGridExtJsField();
            nodeIdFld.dataIndex = nodeIdDataIndex;
            grid.fields.Add( nodeIdFld );
            CswNbtGridExtJsColumn nodeIdCol = new CswNbtGridExtJsColumn();
            nodeIdCol.header = "Internal ID";
            nodeIdCol.dataIndex = nodeIdDataIndex;
            nodeIdCol.hidden = true;
            grid.columns.Add( nodeIdCol );

            CswNbtGridExtJsDataIndex nodekeyDataIndex = new CswNbtGridExtJsDataIndex( "nodekey" );
            CswNbtGridExtJsField nodekeyFld = new CswNbtGridExtJsField();
            nodekeyFld.dataIndex = nodekeyDataIndex;
            grid.fields.Add( nodekeyFld );
            CswNbtGridExtJsColumn nodekeyCol = new CswNbtGridExtJsColumn();
            nodekeyCol.header = "Internal Key";
            nodekeyCol.dataIndex = nodekeyDataIndex;
            nodekeyCol.hidden = true;
            grid.columns.Add( nodekeyCol );

            CswNbtGridExtJsDataIndex nodenameDataIndex = new CswNbtGridExtJsDataIndex( "nodename" );
            CswNbtGridExtJsField nodenameFld = new CswNbtGridExtJsField();
            nodenameFld.dataIndex = nodenameDataIndex;
            grid.fields.Add( nodenameFld );
            CswNbtGridExtJsColumn nodenameCol = new CswNbtGridExtJsColumn();
            nodenameCol.header = "Internal Name";
            nodenameCol.dataIndex = nodenameDataIndex;
            nodenameCol.hidden = true;
            grid.columns.Add( nodenameCol );

            // View Properties determine Columns and Fields
            foreach( CswNbtViewProperty ViewProp in View.getOrderedViewProps( true ) )
            {
                if( ViewProp != null )
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
                    string header = MetaDataProp.PropNameWithQuestionNo;
                    CswNbtGridExtJsDataIndex dataIndex = new CswNbtGridExtJsDataIndex( MetaDataProp.PropName );  // don't use PropNameWithQuestionNo here, because it won't match the propname from the tree

                    // Potential bug here!
                    // If the same property is added to the view more than once, we'll only use the grid definition for the first instance
                    if( false == grid.columnsContains( header ) )
                    {
                        CswNbtGridExtJsField fld = new CswNbtGridExtJsField();
                        fld.dataIndex = dataIndex;
                        CswNbtGridExtJsColumn col = new CswNbtGridExtJsColumn();
                        col.header = header;
                        col.dataIndex = dataIndex;
                        col.hidden = ( false == ViewProp.ShowInGrid );
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
                                if( ViewProp.NodeTypeProp.Extended == string.Empty ||
                                    ViewProp.NodeTypeProp.Extended == CswNbtNodePropDateTime.DateDisplayMode.Date.ToString() ||
                                    ViewProp.NodeTypeProp.Extended == CswNbtNodePropDateTime.DateDisplayMode.DateTime.ToString() )
                                {
                                    dateformat += CswTools.DateFormatToExtJsDateFormat( _CswNbtResources.CurrentNbtUser.DateFormat );
                                    if( ViewProp.NodeTypeProp.Extended == CswNbtNodePropDateTime.DateDisplayMode.DateTime.ToString() )
                                    {
                                        dateformat += " ";
                                    }
                                }
                                if( ViewProp.NodeTypeProp.Extended == CswNbtNodePropDateTime.DateDisplayMode.Time.ToString() ||
                                    ViewProp.NodeTypeProp.Extended == CswNbtNodePropDateTime.DateDisplayMode.DateTime.ToString() )
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
                } // if( ViewProp != null )
            } // foreach( CswNbtViewProperty ViewProp in View.getOrderedViewProps() )

            // Nodes in the Tree determine Rows
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                CswNbtGridExtJsRow gridrow = new CswNbtGridExtJsRow();
                Tree.goToNthChild( c );

                gridrow.data.Add( nodeIdDataIndex, Tree.getNodeIdForCurrentPosition().ToString() );
                gridrow.data.Add( nodekeyDataIndex, Tree.getNodeKeyForCurrentPosition().ToString() );
                gridrow.data.Add( nodenameDataIndex, Tree.getNodeNameForCurrentPosition().ToString() );

                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Tree.getNodeKeyForCurrentPosition().NodeTypeId );
                gridrow.canView = _CswNbtResources.Permit.can( Security.CswNbtPermit.NodeTypePermission.View,
                                                               NodeType,
                                                               NodeId: Tree.getNodeIdForCurrentPosition() );
                gridrow.canEdit = ( _CswNbtResources.Permit.can( Security.CswNbtPermit.NodeTypePermission.Edit,
                                                               NodeType,
                                                               NodeId: Tree.getNodeIdForCurrentPosition() ) &&
                                    false == Tree.getNodeLockedForCurrentPosition() );
                gridrow.canDelete = _CswNbtResources.Permit.can( Security.CswNbtPermit.NodeTypePermission.Delete,
                                                                 NodeType,
                                                                 NodeId: Tree.getNodeIdForCurrentPosition() );
                gridrow.isLocked = Tree.getNodeLockedForCurrentPosition();

                _TreeNodeToGrid( View, Tree, grid, gridrow );

                Tree.goToParentNode();
                grid.rows.Add( gridrow );
            }

            return grid.ToJson();
        } // TreeToJson()

        private void _TreeNodeToGrid( CswNbtView View, ICswNbtTree Tree, CswNbtGridExtJsGrid grid, CswNbtGridExtJsRow gridrow )
        {
            JArray ChildProps = Tree.getChildNodePropsOfNode();

            foreach( JObject Prop in ChildProps )
            {
                // Potential bug here!
                // If the view defines the property by objectclass propname, but the nodetype propname differs, this might break
                CswNbtGridExtJsDataIndex dataIndex = new CswNbtGridExtJsDataIndex( Prop[CswNbtTreeNodes._AttrName_NodePropName].ToString() );

                CswPrimaryKey NodeId = Tree.getNodeIdForCurrentPosition();
                CswNbtMetaDataFieldType.NbtFieldType FieldType = Prop[CswNbtTreeNodes._AttrName_NodePropFieldType].ToString();
                Int32 JctNodePropId = CswConvert.ToInt32( Prop[CswNbtTreeNodes._AttrName_JctNodePropId] );
                Int32 NodeTypePropId = CswConvert.ToInt32( Prop[CswNbtTreeNodes._AttrName_NodePropId] );
                string PropName = CswConvert.ToString( Prop[CswNbtTreeNodes._AttrName_NodePropName] );

                string oldValue = Prop[CswNbtTreeNodes._AttrName_NodePropGestalt].ToString();
                string newValue = string.Empty;
                switch( FieldType )
                {
                    case CswNbtMetaDataFieldType.NbtFieldType.Button:
                    // This will require significant work on the client to rearrange how we handle ajax events
                    //CswPropIdAttr PropAttr=new CswPropIdAttr(NodeId, NodeTypePropId);
                    //string url = "wsNBT.asmx/onObjectClassButtonClick?NodeTypePropAttr=" + PropAttr.ToString();
                    //newValue = "<a href='" + url + "'>" + ( oldValue ?? PropName ) + "</a>";
                    //break;
                    case CswNbtMetaDataFieldType.NbtFieldType.File:
                        string LinkUrl = CswNbtNodePropBlob.getLink( JctNodePropId, NodeId, NodeTypePropId );
                        if( false == string.IsNullOrEmpty( LinkUrl ) )
                        {
                            newValue = "<a target=\"blank\" href=\"" + LinkUrl + "\">" + ( oldValue ?? "File" ) + "</a>";
                        }
                        break;
                    case CswNbtMetaDataFieldType.NbtFieldType.Image:
                        string ImageUrl = CswNbtNodePropImage.getLink( JctNodePropId, NodeId, NodeTypePropId );
                        if( false == string.IsNullOrEmpty( ImageUrl ) )
                        {
                            newValue = "<a target=\"blank\" href=\"" + ImageUrl + "\">" + ( oldValue ?? "Image" ) + "</a>";
                        }
                        break;
                    case CswNbtMetaDataFieldType.NbtFieldType.Link:
                        CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
                        CswNbtSubField.PropColumn HrefColumn = MetaDataProp.getFieldTypeRule().SubFields[CswNbtSubField.SubFieldName.Href].Column;
                        string Href = string.Empty;
                        if( null != Prop[HrefColumn.ToString().ToLower()] )
                        {
                            Href = Prop[HrefColumn.ToString().ToLower()].ToString();
                            if( false == string.IsNullOrEmpty( Href ) )
                            {
                                newValue = "<a target=\"blank\" href=\"" + Href + "\">" + ( oldValue ?? "Link" ) + "</a>";
                            }
                        }
                        break;
                    case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                        newValue = CswConvert.ToDisplayString( CswConvert.ToTristate( oldValue ) );
                        break;
                    default:
                        newValue = oldValue;
                        break;
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


        public CswNbtGridExtJsGrid DataTableToGrid( DataTable DT, bool Editable = false )
        {
            CswNbtGridExtJsGrid grid = new CswNbtGridExtJsGrid();
            grid.title = DT.TableName;
            if( _CswNbtResources.CurrentNbtUser != null && _CswNbtResources.CurrentNbtUser.PageSize > 0 )
            {
                grid.PageSize = _CswNbtResources.CurrentNbtUser.PageSize;
            }

            foreach( DataColumn Column in DT.Columns )
            {
                CswNbtGridExtJsDataIndex dataIndex = new CswNbtGridExtJsDataIndex( Column.ColumnName );
                CswNbtGridExtJsField fld = new CswNbtGridExtJsField();
                fld.dataIndex = dataIndex;
                grid.fields.Add( fld );
                CswNbtGridExtJsColumn gridcol = new CswNbtGridExtJsColumn();
                gridcol.header = Column.ColumnName;
                gridcol.dataIndex = dataIndex;
                grid.columns.Add( gridcol );
            }

            foreach( DataRow Row in DT.Rows )
            {
                CswNbtGridExtJsRow gridrow = new CswNbtGridExtJsRow();
                foreach( DataColumn Column in DT.Columns )
                {
                    gridrow.data[new CswNbtGridExtJsDataIndex( Column.ColumnName )] = Row[Column].ToString();
                }
                grid.rows.Add( gridrow );
            } // foreach( DataRow Row in DT.Rows )

            return grid;
        } // DataTableToGrid()

        public JObject DataTableToJSON( DataTable DT, bool Editable = false )
        {
            CswNbtGridExtJsGrid grid = DataTableToGrid( DT, Editable );
            return grid.ToJson();
        } // DataTableToJSON()


    } // class CswNbtGridExtJs
} // namespace ChemSW.Nbt.Grid
