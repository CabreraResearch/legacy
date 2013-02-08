using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Class for accessing nodetype layouts
    /// Currently, this one is not implemented like the other MetaData* classes -- 
    /// it does not represent a single layout, but just a collection of functions for getting layout values
    /// </summary>
    public class CswNbtMetaDataNodeTypeLayoutMgr // : ICswNbtMetaDataObject
    {
        public sealed class LayoutType : CswEnum<LayoutType>
        {
            private LayoutType( String Name ) : base( Name ) { }
            public static IEnumerable<LayoutType> _All { get { return All; } }
            public static implicit operator LayoutType( string str )
            {
                LayoutType ret = Parse( str );
                return ret ?? Unknown;
            }
            public static readonly LayoutType Unknown = new LayoutType( "Unknown" );
            public static readonly LayoutType Add = new LayoutType( "Add" );
            public static readonly LayoutType Edit = new LayoutType( "Edit" );
            public static readonly LayoutType Preview = new LayoutType( "Preview" );
            public static readonly LayoutType Table = new LayoutType( "Table" );
        }
        
        public LayoutType LayoutTypeForEditMode( NodeEditMode EditMode )
        {
            LayoutType LType = LayoutType.Unknown;
            switch( EditMode )
            {
                case NodeEditMode.Add: LType = LayoutType.Add; break;
                case NodeEditMode.Temp: LType = LayoutType.Add; break;
                case NodeEditMode.Preview: LType = LayoutType.Preview; break;
                case NodeEditMode.Table: LType = LayoutType.Table; break;
                default: LType = LayoutType.Edit; break;
            }
            return LType;
        } // LayoutTypeForEditMode()

        public class NodeTypeLayout
        {
            public LayoutType LayoutType = LayoutType.Unknown;
            public Int32 PropId = Int32.MinValue;
            public Int32 TabId = Int32.MinValue;
            public Int32 DisplayRow = Int32.MinValue;
            public Int32 DisplayColumn = Int32.MinValue;
            public string TabGroup = string.Empty;
            public NodeTypeLayout( DataRow LayoutRow )
            {
                LayoutType = LayoutRow["layouttype"].ToString();
                PropId = CswConvert.ToInt32( LayoutRow["nodetypepropid"] );
                TabId = CswConvert.ToInt32( LayoutRow["nodetypetabsetid"] );  // This is Int32.MinValue for non-Edit
                DisplayRow = CswConvert.ToInt32( LayoutRow["display_row"] );
                DisplayColumn = CswConvert.ToInt32( LayoutRow["display_column"] );
                TabGroup = CswConvert.ToString( LayoutRow["tabgroup"] );
            }
            public NodeTypeLayout( Int32 PropId, Int32 Row, Int32 Col, Int32 TabId, LayoutType Type = null, string TabGroup = "" )
            {
                this.LayoutType = Type ?? LayoutType.Edit;
                this.PropId = PropId;
                this.TabId = TabId;
                this.DisplayRow = Row;
                this.DisplayColumn = Col;
                this.TabGroup = TabGroup;
            }
        }

        private CswNbtMetaDataResources _CswNbtMetaDataResources;

        public CswNbtMetaDataNodeTypeLayoutMgr( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
        }

        /// <summary>
        /// Returns a dictionary of layout by tab
        /// </summary>
        public Dictionary<Int32, NodeTypeLayout> getLayout( LayoutType LayoutType, CswNbtMetaDataNodeTypeProp Prop )
        {
            return getLayout( LayoutType, Prop.PropId );
        }
        /// <summary>
        /// Returns a dictionary of layout by tab
        /// </summary>
        public Dictionary<Int32, NodeTypeLayout> getLayout( LayoutType LayoutType, Int32 PropId )
        {
            // A property could be on more than one tab
            Dictionary<Int32, NodeTypeLayout> LayoutByTab = new Dictionary<Int32, NodeTypeLayout>();
            CswTableSelect LayoutSelect = _CswNbtMetaDataResources.CswNbtResources.makeCswTableSelect( "getLayout_Select", "nodetype_layout" );
            DataTable LayoutTable = LayoutSelect.getTable( "where layouttype = '" + LayoutType.ToString() + "' and nodetypepropid = " + PropId.ToString() );
            foreach( DataRow LayoutRow in LayoutTable.Rows )
            {
                NodeTypeLayout Layout = new NodeTypeLayout( LayoutRow );
                LayoutByTab[Layout.TabId] = Layout;
            }
            return LayoutByTab;
        } // getLayout()

        /// <summary>
        /// Returns a layout for a property on a tab.
        /// If edit, be sure to supply a valid TabId.
        /// </summary>
        public NodeTypeLayout getLayout( LayoutType LayoutType, Int32 PropId, Int32 TabId )
        {
            NodeTypeLayout Layout = null;

            CswTableSelect LayoutSelect = _CswNbtMetaDataResources.CswNbtResources.makeCswTableSelect( "getLayout_Select", "nodetype_layout" );
            string WhereClause = "where layouttype = '" + LayoutType.ToString() + "' and nodetypepropid = " + PropId.ToString();
            if( LayoutType == CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit )
            {
                if( TabId != Int32.MinValue )
                {
                    WhereClause += " and nodetypetabsetid = " + TabId.ToString();
                }
                else
                {
                    throw new CswDniException( ErrorType.Error, "Missing Tab", "CswNbtMetaDataNodeTypeLayoutMgr.getLayout() requires a valid TabId for Edit layouts" );
                }
            }
            DataTable LayoutTable = LayoutSelect.getTable( WhereClause );
            if( LayoutTable.Rows.Count > 0 )
            {
                DataRow LayoutRow = LayoutTable.Rows[0];
                Layout = new NodeTypeLayout( LayoutRow );
            }
            return Layout;
        }

        public void updatePropLayout( LayoutType LayoutType, Int32 NodeTypeId, CswNbtMetaDataNodeTypeProp NtProp, bool DoMove, Int32 TabId = Int32.MinValue, Int32 DisplayRow = Int32.MinValue, Int32 DisplayColumn = Int32.MinValue, string TabGroup = "" )
        {
            if( LayoutType != LayoutType.Unknown && 
                null != NtProp )
            {
                if( DoMove )
                {
                    removePropFromLayout( LayoutType, NtProp, Int32.MinValue );
                }
                CswTableUpdate LayoutUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "updatePropLayout_Update", "nodetype_layout" );
                string WhereClause = "where layouttype = '" + LayoutType.ToString() + "' and nodetypepropid = " + NtProp.PropId.ToString();
                if( TabId != Int32.MinValue && LayoutType == CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit )
                {
                    WhereClause += " and nodetypetabsetid = " + TabId.ToString();
                }
                DataTable LayoutTable = LayoutUpdate.getTable( WhereClause );

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
                Row["nodetypepropid"] = CswConvert.ToDbVal( NtProp.PropId );

                if( LayoutType == CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit )
                {
                    if( TabId != Int32.MinValue )
                    {
                        Row["nodetypetabsetid"] = CswConvert.ToDbVal( TabId );
                    }
                    else
                    {
                        throw new CswDniException( ErrorType.Error, "Tab is required", "CswNbtMetaDataNodeTypeLayoutMgr.updatePropLayout() requires a valid TabId for Edit layouts" );
                    }
                }

                Int32 FinalDisplayRow = DisplayRow;
                Int32 FinalDisplayCol = DisplayColumn;
                
                //Very special case for 'Save'--it is always last
                if( FinalDisplayRow == Int32.MaxValue )
                {
                    CswNbtMetaDataObjectClassProp Ocp = NtProp.getObjectClassProp();
                    if( null == Ocp || Ocp.PropName != CswNbtObjClass.PropertyName.Save )
                    {
                        FinalDisplayRow = Int32.MaxValue - 1;
                    }
                }
                else
                {

                    if( FinalDisplayRow <= 0 )
                    {
                        FinalDisplayRow = getCurrentMaxDisplayRow( NodeTypeId, TabId, LayoutType ) + 1;
                    }
                    if( FinalDisplayCol <= 0 )
                    {
                        FinalDisplayCol = 1;
                    }
                }
                Row["display_row"] = CswConvert.ToDbVal( FinalDisplayRow );
                Row["display_column"] = CswConvert.ToDbVal( FinalDisplayCol );

                Row["tabgroup"] = TabGroup;

                LayoutUpdate.update( LayoutTable );
            } // if( Type != LayoutType.Unknown && Prop != null )
        } // updatePropLayout()

        public void updatePropLayout( LayoutType LayoutType, CswNbtMetaDataNodeTypeProp Prop, CswNbtMetaDataNodeTypeProp InsertAfterProp, bool DoMove )
        {
            bool Added = false;
            if( DoMove )
            {
                removePropFromLayout( LayoutType, Prop, Int32.MinValue );
            }
            if( InsertAfterProp != null )
            {
                Dictionary<Int32, NodeTypeLayout> InsertAfterPropLayouts = getLayout( LayoutType, InsertAfterProp );
                if( InsertAfterPropLayouts.Values.Count > 0 )
                {
                    foreach( Int32 TabId in InsertAfterPropLayouts.Keys )
                    {
                        NodeTypeLayout InsertAfterPropLayout = InsertAfterPropLayouts[TabId];

                        // Make space for the new prop
                        CswTableUpdate LayoutUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "makeSpaceForProp_Update", "nodetype_layout" );
                        string WhereClause = "where layouttype = '" + LayoutType.ToString() + "' and nodetypeid = " + InsertAfterProp.NodeTypeId.ToString();
                        if( TabId != Int32.MinValue && LayoutType == CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit )
                        {
                            WhereClause += " and nodetypetabsetid = " + TabId.ToString();
                        }
                        DataTable LayoutTable = LayoutUpdate.getTable( WhereClause );

                        foreach( DataRow Row in LayoutTable.Rows )
                        {
                            if( CswConvert.ToInt32( Row["display_column"] ) == InsertAfterPropLayout.DisplayColumn &&
                                CswConvert.ToInt32( Row["display_row"] ) > InsertAfterPropLayout.DisplayRow )
                            {
                                Row["display_row"] = CswConvert.ToDbVal( CswConvert.ToInt32( Row["display_row"] ) + 1 );
                            }
                        }
                        LayoutUpdate.update( LayoutTable );

                        // Add new prop to the layout
                        updatePropLayout( LayoutType, Prop.NodeTypeId, Prop, false, TabId, InsertAfterPropLayout.DisplayRow + 1, InsertAfterPropLayout.DisplayColumn, InsertAfterPropLayout.TabGroup );
                        Added = true;
                    } // foreach( Int32 TabId in InsertAfterPropLayouts.Keys )
                } // if( InsertAfterPropLayouts.Values.Count > 0 )
            } // if( InsertAfterProp != null )

            if( false == Added )
            {
                // Just add it somewhere
                updatePropLayout( LayoutType, Prop.NodeTypeId, Prop, false );
            }
        } // updatePropLayout()

        public void removePropFromLayout( LayoutType LayoutType, CswNbtMetaDataNodeTypeProp Prop, Int32 TabId )
        {
            if( LayoutType != LayoutType.Unknown && Prop != null )
            {
                CswTableUpdate LayoutUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "removePropFromLayout_Update", "nodetype_layout" );
                string WhereClause = "where layouttype = '" + LayoutType.ToString() + "' and nodetypepropid = " + Prop.PropId.ToString();
                if( TabId != Int32.MinValue && LayoutType == LayoutType.Edit )
                {
                    WhereClause += " and nodetypetabsetid = " + TabId.ToString();
                }
                DataTable LayoutTable = LayoutUpdate.getTable( WhereClause );
                foreach( DataRow Row in LayoutTable.Rows )
                {
                    Row.Delete();
                }
                LayoutUpdate.update( LayoutTable );
            } // if( Type != LayoutType.Unknown && Prop != null )
        } // removePropFromLayout()

        public void removePropFromAllLayouts( CswNbtMetaDataNodeTypeProp Prop )
        {
            if( Prop != null )
            {
                removePropFromAllLayouts( Prop.PropId );
            }
        } // removePropFromAllLayouts()

        public void removePropFromAllLayouts( Int32 PropId )
        {
            CswTableUpdate LayoutUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "removePropFromAllLayouts_Update", "nodetype_layout" );
            string WhereClause = "where nodetypepropid = " + PropId.ToString();
            DataTable LayoutTable = LayoutUpdate.getTable( WhereClause );
            foreach( DataRow Row in LayoutTable.Rows )
            {
                Row.Delete();
            }
            LayoutUpdate.update( LayoutTable );
        } // removePropFromAllLayouts()

        public Int32 getCurrentMaxDisplayRow( Int32 NodeTypeId, Int32 TabId, LayoutType LayoutType )
        {
            return getCurrentMaxDisplayRow( _CswNbtMetaDataResources.CswNbtResources, NodeTypeId, TabId, LayoutType );
        } // getCurrentMaxDisplayRow()

        public static Int32 getCurrentMaxDisplayRow( CswNbtResources NbtResources, Int32 NodeTypeId, Int32 TabId, LayoutType LayoutType )
        {
            Int32 MaxRow = 0;
            CswTableSelect LayoutSelect = NbtResources.makeCswTableSelect( "getCurrentMaxDisplayRow_Select", "nodetype_layout" );
            string WhereClause = "where layouttype = '" + LayoutType.ToString() + "' and nodetypeid = " + NodeTypeId.ToString() + " and display_row != " + Int32.MaxValue;
            if( TabId != Int32.MinValue && LayoutType == LayoutType.Edit )
            {
                WhereClause += " and nodetypetabsetid = " + TabId.ToString();
            }
            DataTable LayoutTable = LayoutSelect.getTable( WhereClause );
            foreach( DataRow Row in LayoutTable.Rows )
            {
                Int32 ThisRow = CswConvert.ToInt32( Row["display_row"] );
                if( ThisRow > MaxRow )
                {
                    MaxRow = ThisRow;
                }
            }
            return MaxRow;
        } // getCurrentMaxDisplayRow()

        public IEnumerable<CswNbtMetaDataNodeTypeProp> getPropsInLayout( Int32 NodeTypeId, Int32 TabId, LayoutType LayoutType )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getLayoutProps( NodeTypeId, TabId, LayoutType );
        } // getPropsInLayout()

        public IEnumerable<CswNbtMetaDataNodeTypeProp> getPropsNotInLayout( CswNbtMetaDataNodeType NodeType, Int32 TabId, LayoutType LayoutType )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getLayoutProps( NodeType.NodeTypeId, TabId, LayoutType, false );
        } // getPropsNotInLayout()

    } // public class CswNbtMetaDataNodeTypeLayout : ICswNbtMetaDataObject
} // namespace ChemSW.Nbt.MetaData
