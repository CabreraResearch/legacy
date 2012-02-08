using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Class for accessing nodetype layouts
    /// Currently, this one is not implemented like the other MetaData* classes -- 
    /// it does not represent a single layout, but just a collection of functions for getting layout values
    /// </summary>
    public class CswNbtMetaDataNodeTypeLayoutMgr // : ICswNbtMetaDataObject
    {
        public enum LayoutType
        {
            Unknown,
            Add,
            Edit,
            Preview,
            Table
        }

        public LayoutType LayoutTypeForEditMode( string EditMode )
        {
            NodeEditMode RealEditMode = (NodeEditMode) Enum.Parse( typeof( NodeEditMode ), EditMode );
            return LayoutTypeForEditMode( RealEditMode );
        }
        public LayoutType LayoutTypeForEditMode( NodeEditMode EditMode )
        {
            LayoutType LType = LayoutType.Unknown;
            switch( EditMode )
            {
                case NodeEditMode.Add: LType = LayoutType.Add; break;
                case NodeEditMode.Preview: LType = LayoutType.Preview; break;
                case NodeEditMode.Table: LType = LayoutType.Table; break;
                default: LType = LayoutType.Edit; break;
            }
            return LType;
        } // LayoutTypeForEditMode()

        public class NodeTypeLayout
        {
            public LayoutType LayoutType = LayoutType.Unknown;
            //public CswNbtMetaDataNodeTypeProp Prop = null;
            //public CswNbtMetaDataNodeTypeTab Tab = null;
            public Int32 PropId = Int32.MinValue;
            public Int32 TabId = Int32.MinValue;
            public Int32 DisplayRow = Int32.MinValue;
            public Int32 DisplayColumn = Int32.MinValue;
        }
        
        private CswNbtMetaDataResources _CswNbtMetaDataResources;

        public CswNbtMetaDataNodeTypeLayoutMgr( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
        }

        public NodeTypeLayout getLayout(LayoutType LayoutType, CswNbtMetaDataNodeTypeProp Prop)
        {
            CswTimer GetLayoutTimer = new CswTimer();

            NodeTypeLayout Layout = null;
            CswTableSelect LayoutSelect = _CswNbtMetaDataResources.CswNbtResources.makeCswTableSelect( "getLayout_Select", "nodetype_layout" );
            DataTable LayoutTable = LayoutSelect.getTable( "where layouttype = '" + LayoutType.ToString() + "' and nodetypepropid = " + Prop.PropId.ToString() );
            if(LayoutTable.Rows.Count > 0)
            {
                Layout = new NodeTypeLayout();
                Layout.LayoutType = LayoutType;
                Layout.PropId = Prop.PropId;
                //Layout.Tab = _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeTab( CswConvert.ToInt32( LayoutTable.Rows[0]["nodetypetabsetid"] ) );
                Layout.TabId = CswConvert.ToInt32( LayoutTable.Rows[0]["nodetypetabsetid"] );
                Layout.DisplayRow = CswConvert.ToInt32( LayoutTable.Rows[0]["display_row"] );
                Layout.DisplayColumn = CswConvert.ToInt32( LayoutTable.Rows[0]["display_column"] );
            }

            _CswNbtMetaDataResources.CswNbtResources.logTimerResult( "CswNbtMetaDataNodeTypeLayoutMgr.getLayout()", GetLayoutTimer.ElapsedDurationInSecondsAsString );


            return Layout;

        } // getLayout()


        public void updatePropLayout( LayoutType LayoutType, Int32 NodeTypeId, Int32 PropId, Int32 TabId, Int32 DisplayRow, Int32 DisplayColumn )
        {
            if( LayoutType != LayoutType.Unknown && PropId != Int32.MinValue )
            {
                CswTableUpdate LayoutUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "updatePropLayout_Update", "nodetype_layout" );
                DataTable LayoutTable = LayoutUpdate.getTable( "where layouttype = '" + LayoutType.ToString() + "' and nodetypepropid = " + PropId.ToString() );
                DataRow Row = null;
                if( LayoutTable.Rows.Count > 0 )
                {
                    Row = LayoutTable.Rows[0];
                }
                else
                {
                    Row = LayoutTable.NewRow();
                    LayoutTable.Rows.Add( Row );
                }
                Row["layouttype"] = LayoutType.ToString();
                Row["nodetypeid"] = CswConvert.ToDbVal( NodeTypeId );
                Row["nodetypepropid"] = CswConvert.ToDbVal( PropId );

                if( TabId != Int32.MinValue && LayoutType == CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit )
                {
                    Row["nodetypetabsetid"] = CswConvert.ToDbVal( TabId );
                }
                if( DisplayRow != Int32.MinValue && DisplayColumn != Int32.MinValue )
                {
                    Row["display_row"] = CswConvert.ToDbVal( DisplayRow );
                    Row["display_column"] = CswConvert.ToDbVal( DisplayColumn );
                }
                else
                {
                    Row["display_row"] = CswConvert.ToDbVal( getCurrentMaxDisplayRow( NodeTypeId, TabId, LayoutType ) + 1 );
                    Row["display_column"] = CswConvert.ToDbVal( 1 );
                }
                LayoutUpdate.update( LayoutTable );
            } // if( Type != LayoutType.Unknown && Prop != null )
        } // updatePropLayout()

        public void updatePropLayout( LayoutType LayoutType, CswNbtMetaDataNodeTypeProp Prop, CswNbtMetaDataNodeTypeProp InsertAfterProp )
        {
            if( InsertAfterProp != null )
            {
                NodeTypeLayout InsertAfterPropLayout = getLayout( LayoutType, InsertAfterProp );
                if( InsertAfterPropLayout != null )
                {
                    Collection<CswNbtMetaDataNodeTypeProp> PropsToPush = new Collection<CswNbtMetaDataNodeTypeProp>();
                    CswTableUpdate LayoutUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "makeSpaceForProp_Update", "nodetype_layout" );
                    DataTable LayoutTable = LayoutUpdate.getTable( "where layouttype = '" + LayoutType.ToString() + "' and nodetypeid = " + InsertAfterProp.NodeType.NodeTypeId.ToString() );
                    foreach( DataRow Row in LayoutTable.Rows )
                    {
                        if( ( InsertAfterPropLayout.TabId == Int32.MinValue || InsertAfterPropLayout.TabId == CswConvert.ToInt32( Row["nodetypetabsetid"] ) ) &&
                            CswConvert.ToInt32( Row["display_column"] ) == InsertAfterPropLayout.DisplayColumn &&
                            CswConvert.ToInt32( Row["display_row"] ) > InsertAfterPropLayout.DisplayRow )
                        {
                            Row["display_row"] = CswConvert.ToDbVal( CswConvert.ToInt32( Row["display_row"] ) + 1 );
                        }
                    }
                    LayoutUpdate.update( LayoutTable );

                    updatePropLayout( LayoutType, Prop.NodeType.NodeTypeId, Prop.PropId, InsertAfterPropLayout.TabId, InsertAfterPropLayout.DisplayRow + 1, InsertAfterPropLayout.DisplayColumn );
                }
            }
        } // updatePropLayout()
    
        public void removePropFromLayout( LayoutType LayoutType, CswNbtMetaDataNodeTypeProp Prop )
        {
            if( LayoutType != LayoutType.Unknown && Prop != null )
            {
                CswTableUpdate LayoutUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "removePropFromLayout_Update", "nodetype_layout" );
                DataTable LayoutTable = LayoutUpdate.getTable( "where layouttype = '" + LayoutType.ToString() + "' and nodetypepropid = " + Prop.PropId.ToString() );
                foreach( DataRow Row in LayoutTable.Rows )
                {
                    Row.Delete();
                }
                LayoutUpdate.update( LayoutTable );
            } // if( Type != LayoutType.Unknown && Prop != null )
        } // removePropFromLayout()

        public Int32 getCurrentMaxDisplayRow( Int32 NodeTypeId, Int32 TabId, LayoutType LayoutType )
        {
            Int32 MaxRow = 0;
            CswTableSelect LayoutSelect = _CswNbtMetaDataResources.CswNbtResources.makeCswTableSelect( "getCurrentMaxDisplayRow_Select", "nodetype_layout" );
            DataTable LayoutTable = LayoutSelect.getTable( "where layouttype = '" + LayoutType.ToString() + "' and nodetypeid = " + NodeTypeId.ToString() );
            foreach( DataRow Row in LayoutTable.Rows )
            {
                Int32 ThisRow = CswConvert.ToInt32( Row["display_row"] );
                Int32 ThisTabId = CswConvert.ToInt32( Row["nodetypetabsetid"] );
                if( ThisRow > MaxRow &&
                    ( LayoutType != LayoutType.Edit ||
                      ( TabId == ThisTabId ) ) )
                {
                    MaxRow = ThisRow;
                }
            }
            return MaxRow;
        } // getCurrentMaxDisplayRow()

        public Collection<CswNbtMetaDataNodeTypeProp> getPropsInLayout( Int32 NodeTypeId, Int32 TabId, LayoutType LayoutType )
        {
            Collection<CswNbtMetaDataNodeTypeProp> ret = new Collection<CswNbtMetaDataNodeTypeProp>();

            CswTableSelect LayoutSelect = _CswNbtMetaDataResources.CswNbtResources.makeCswTableSelect( "getPropsInLayout_Select", "nodetype_layout" );
            string WhereClause = "where layouttype = '" + LayoutType.ToString() + "' and nodetypeid = " + NodeTypeId.ToString();
            if( LayoutType == CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit && TabId != Int32.MinValue )
            {
                WhereClause += "and nodetypetabsetid = " + TabId.ToString();
            }
            Collection<OrderByClause> OrderBy = new Collection<OrderByClause>() { 
                new OrderByClause( "display_row", OrderByType.Ascending ),
                new OrderByClause( "display_column", OrderByType.Ascending )
            };
            DataTable LayoutTable = LayoutSelect.getTable( WhereClause, OrderBy );
            foreach( DataRow Row in LayoutTable.Rows )
            {
                CswNbtMetaDataNodeTypeProp Prop = _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeProp( CswConvert.ToInt32( Row["nodetypepropid"] ) );
                if( Prop != null )
                    ret.Add( Prop );
            }
            return ret;
        } // getPropsInLayout()

        public Collection<CswNbtMetaDataNodeTypeProp> getPropsNotInLayout( CswNbtMetaDataNodeType NodeType, Int32 TabId, LayoutType LayoutType )
        {
            Collection<CswNbtMetaDataNodeTypeProp> ret = new Collection<CswNbtMetaDataNodeTypeProp>();
            foreach( CswNbtMetaDataNodeTypeProp Prop in NodeType.getNodeTypeProps())
            {
                ret.Add( Prop );
            }

            CswTableSelect LayoutSelect = _CswNbtMetaDataResources.CswNbtResources.makeCswTableSelect( "getPropsNotInLayout_Select", "nodetype_layout" );
            string WhereClause = "where layouttype = '" + LayoutType.ToString() + "' and nodetypeid = " + NodeType.NodeTypeId.ToString();
            if( LayoutType == CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit && TabId != Int32.MinValue )
            {
                WhereClause += "and nodetypetabsetid = " + TabId.ToString();
            }
            DataTable LayoutTable = LayoutSelect.getTable( WhereClause );
            foreach( DataRow Row in LayoutTable.Rows )
            {
                ret.Remove( _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeProp( CswConvert.ToInt32( Row["nodetypepropid"] ) ) );
            }
            return ret;
        } // getPropsNotInLayout()
        

        //public void makeSpaceForProp(CswNbtMetaDataNodeTypeLayout.LayoutType LayoutType, CswNbtMetaDataNodeTypeProp InsertAfterProp )
        //    // NodeTypeTab, Int32 DisplayColumn, Int32 DisplayRow)
        //{
        //    if( InsertAfterProp != null )
        //    {
        //        NodeTypeLayout InsertAfterPropLayout = getLayout( LayoutType, InsertAfterProp );
        //        if( InsertAfterPropLayout != null )
        //        {
        //            Collection<CswNbtMetaDataNodeTypeProp> PropsToPush = new Collection<CswNbtMetaDataNodeTypeProp>();
        //            CswTableUpdate LayoutUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "makeSpaceForProp_Update", "nodetype_layout" );
        //            DataTable LayoutTable = LayoutUpdate.getTable( "where layouttype = '" + LayoutType.ToString() + "' and nodetypeid = " + InsertAfterProp.NodeType.NodeTypeId.ToString() );
        //            foreach( DataRow Row in LayoutTable.Rows )
        //            {
        //                if( ( InsertAfterPropLayout.Tab == null || InsertAfterPropLayout.Tab.TabId == CswConvert.ToInt32( Row["nodetypetabsetid"] ) ) &&
        //                    CswConvert.ToInt32( Row["display_column"] ) == DisplayColumn &&
        //                    CswConvert.ToInt32( Row["display_row"] ) >= DisplayRow )
        //                {
        //                    Row["display_row"] = CswConvert.ToDbVal( CswConvert.ToInt32( Row["display_row"] ) + 1 );
        //                }
        //            }
        //            LayoutUpdate.update( LayoutTable );
        //        }
        //    }
        //} // makeSpaceForProp()

    } // public class CswNbtMetaDataNodeTypeLayout : ICswNbtMetaDataObject
} // namespace ChemSW.Nbt.MetaData
