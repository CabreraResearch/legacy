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

            CswNbtGridExtJsField nodeIdFld = new CswNbtGridExtJsField();
            nodeIdFld.name = "nodeid";
            grid.fields.Add( nodeIdFld );
            CswNbtGridExtJsColumn nodeIdCol = new CswNbtGridExtJsColumn();
            nodeIdCol.header = "Internal ID";
            nodeIdCol.dataIndex = "nodeid";
            nodeIdCol.hidden = true;
            grid.columns.Add( nodeIdCol );

            CswNbtGridExtJsField nodekeyFld = new CswNbtGridExtJsField();
            nodekeyFld.name = "nodekey";
            grid.fields.Add( nodekeyFld );
            CswNbtGridExtJsColumn nodekeyCol = new CswNbtGridExtJsColumn();
            nodekeyCol.header = "Internal Key";
            nodekeyCol.dataIndex = "nodekey";
            nodekeyCol.hidden = true;
            grid.columns.Add( nodekeyCol );

            CswNbtGridExtJsField nodenameFld = new CswNbtGridExtJsField();
            nodenameFld.name = "nodename";
            grid.fields.Add( nodenameFld );
            CswNbtGridExtJsColumn nodenameCol = new CswNbtGridExtJsColumn();
            nodenameCol.header = "Internal Name";
            nodenameCol.dataIndex = "nodename";
            nodenameCol.hidden = true;
            grid.columns.Add( nodenameCol );

            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                CswNbtGridExtJsRow gridrow = new CswNbtGridExtJsRow();
                Tree.goToNthChild( c );

                gridrow.data.Add( "nodeid", Tree.getNodeIdForCurrentPosition().ToString() );
                gridrow.data.Add( "nodekey", Tree.getNodeKeyForCurrentPosition().ToString() );
                gridrow.data.Add( "nodename", Tree.getNodeNameForCurrentPosition().ToString() );

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
                string header = Prop[CswNbtTreeNodes._AttrName_NodePropName].ToString();
                string dataIndex = Prop[CswNbtTreeNodes._AttrName_NodePropId].ToString();
                string value = Prop[CswNbtTreeNodes._AttrName_NodePropGestalt].ToString();
                Int32 propid = CswConvert.ToInt32( Prop[CswNbtTreeNodes._AttrName_NodePropId].ToString() );

                if( false == grid.columnsContains( header ) )
                {
                    // Potential bug here
                    // If the same property is added to the view more than once, we'll only use the grid definition for the first instance

                    CswNbtViewProperty ViewProp = View.findPropertyById( NbtViewPropType.NodeTypePropId, propid );
                    if( ViewProp == null )
                    {
                        CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( propid );
                        if( MetaDataProp != null && MetaDataProp.ObjectClassPropId != Int32.MinValue )
                        {
                            ViewProp = View.findPropertyById( NbtViewPropType.ObjectClassPropId, MetaDataProp.ObjectClassPropId );
                        }
                    }

                    CswNbtGridExtJsField fld = new CswNbtGridExtJsField();
                    fld.name = dataIndex;
                    CswNbtGridExtJsColumn col = new CswNbtGridExtJsColumn();
                    col.header = header;
                    col.dataIndex = dataIndex;
                    col.hidden = ( false == ViewProp.ShowInGrid );
                    switch( ViewProp.FieldType )
                    {
                        case CswNbtMetaDataFieldType.NbtFieldType.Number:
                            col.xtype = extJsXType.numbercolumn;
                            fld.type = "number";
                            break;
                        case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
                            col.xtype = extJsXType.datecolumn;
                            col.dateformat = "m/d/Y";
                            fld.type = "date";
                            break;
                    }
                    if( ViewProp.Width > 0 )
                    {
                        col.width = ViewProp.Width * 7; // approx. characters to pixels
                    }
                    grid.columns.Add( col );
                    grid.fields.Add( fld );
                }
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
                gridcol.dataIndex = Column.ColumnName;
                grid.columns.Add( gridcol );
            }

            foreach( DataRow Row in DT.Rows )
            {
                CswNbtGridExtJsRow gridrow = new CswNbtGridExtJsRow();
                foreach( DataColumn Column in DT.Columns )
                {
                    gridrow.data[Column.ColumnName] = Row[Column].ToString();
                }
                grid.rows.Add( gridrow );
            } // foreach( DataRow Row in DT.Rows )

            return grid.ToJson();
        } // DataTableToJSON


    } // class CswNbtGridExtJs
} // namespace ChemSW.Nbt.Grid
