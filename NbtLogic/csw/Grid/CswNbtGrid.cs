using System;
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
                    string header = MetaDataProp.PropName;
                    CswNbtGridExtJsDataIndex dataIndex = new CswNbtGridExtJsDataIndex( MetaDataProp.PropName );

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
                                col.dateformat = "m/d/Y";
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
                string value = Prop[CswNbtTreeNodes._AttrName_NodePropGestalt].ToString();
                gridrow.data[dataIndex] = value;
            } // foreach( JObject Prop in ChildProps )

            // Recurse, but add properties of child nodes to the same gridrow
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );
                _TreeNodeToGrid( View, Tree, grid, gridrow );
                Tree.goToParentNode();
            }
        } // _TreeNodeToGrid

        public JObject DataTableToJSON( DataTable DT, bool Editable = false )
        {
            CswNbtGridExtJsGrid grid = new CswNbtGridExtJsGrid();
            if( _CswNbtResources.CurrentNbtUser != null && _CswNbtResources.CurrentNbtUser.PageSize > 0 )
            {
                grid.PageSize = _CswNbtResources.CurrentNbtUser.PageSize;
            }

            foreach( DataColumn Column in DT.Columns )
            {
                CswNbtGridExtJsColumn gridcol = new CswNbtGridExtJsColumn();
                gridcol.header = Column.ColumnName;
                gridcol.dataIndex = new CswNbtGridExtJsDataIndex( Column.ColumnName );
                grid.columns.Add( gridcol );
            }

            foreach( DataRow Row in DT.Rows )
            {
                CswNbtGridExtJsRow gridrow = new CswNbtGridExtJsRow();
                foreach( DataColumn Column in DT.Columns )
                {
                    gridrow.data[new CswNbtGridExtJsDataIndex(Column.ColumnName)] = Row[Column].ToString();
                }
                grid.rows.Add( gridrow );
            } // foreach( DataRow Row in DT.Rows )

            return grid.ToJson();
        } // DataTableToJSON


    } // class CswNbtGridExtJs
} // namespace ChemSW.Nbt.Grid
