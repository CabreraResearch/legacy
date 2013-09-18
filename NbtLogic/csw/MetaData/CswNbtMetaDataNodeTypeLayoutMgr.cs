using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
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
        public class NodeTypeLayout
        {
            public CswEnumNbtLayoutType LayoutType = CswEnumNbtLayoutType.Unknown;
            public Int32 NodeTypeId = Int32.MinValue;
            public Int32 PropId = Int32.MinValue;
            public Int32 TabId = Int32.MinValue;
            public Int32 DisplayRow = Int32.MinValue;
            public Int32 DisplayColumn = Int32.MinValue;
            public string TabGroup = string.Empty;

            public NodeTypeLayout( DataRow LayoutRow )
            {
                LayoutType = LayoutRow["layouttype"].ToString();
                NodeTypeId = CswConvert.ToInt32( LayoutRow["nodetypeid"] );
                PropId = CswConvert.ToInt32( LayoutRow["nodetypepropid"] );
                TabId = CswConvert.ToInt32( LayoutRow["nodetypetabsetid"] );  // This is Int32.MinValue for non-Edit
                DisplayRow = CswConvert.ToInt32( LayoutRow["display_row"] );
                DisplayColumn = CswConvert.ToInt32( LayoutRow["display_column"] );
                TabGroup = CswConvert.ToString( LayoutRow["tabgroup"] );
            }
            public NodeTypeLayout( Int32 NodeTypeId, Int32 PropId, Int32 Row, Int32 Col, Int32 TabId, CswEnumNbtLayoutType Type = null, string TabGroup = "" )
            {
                this.LayoutType = Type ?? CswEnumNbtLayoutType.Edit;
                this.NodeTypeId = NodeTypeId;
                this.PropId = PropId;
                this.TabId = TabId;
                this.DisplayRow = Row;
                this.DisplayColumn = Col;
                this.TabGroup = TabGroup;
            }
        }

        private Dictionary<Int32, Collection<NodeTypeLayout>> _Cache = new Dictionary<Int32, Collection<NodeTypeLayout>>();

        private void _CacheLayout( Int32 NodeTypeId )
        {
            if( false == _Cache.ContainsKey( NodeTypeId ) )
            {
                // Cache every layout for this nodetype, for performance
                Collection<NodeTypeLayout> Layouts = new Collection<NodeTypeLayout>();
                CswTableSelect LayoutSelect = _CswNbtMetaDataResources.CswNbtResources.makeCswTableSelect( "getLayout_Select", "nodetype_layout" );
                DataTable LayoutTable = LayoutSelect.getTable( "where nodetypeid = " + NodeTypeId );
                foreach( DataRow LayoutRow in LayoutTable.Rows )
                {
                    NodeTypeLayout Layout = new NodeTypeLayout( LayoutRow );
                    Layouts.Add( Layout );
                }
                _Cache[NodeTypeId] = Layouts;
            }
        }

        private CswNbtMetaDataResources _CswNbtMetaDataResources;

        public CswNbtMetaDataNodeTypeLayoutMgr( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
        }

        /// <summary>
        /// Clear all properties from a layout
        /// </summary>
        public void clearLayout( CswEnumNbtLayoutType LayoutType, Int32 NodeTypeId )
        {
            CswTableUpdate LayoutUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "clearLayout_Update", "nodetype_layout" );
            string WhereClause = "where layouttype = '" + LayoutType.ToString() + "' and nodetypeid = " + NodeTypeId.ToString();
            DataTable LayoutTable = LayoutUpdate.getTable( WhereClause );
            foreach( DataRow Row in LayoutTable.Rows )
            {
                Row.Delete();
            }
            LayoutUpdate.update( LayoutTable );

            if( _Cache.ContainsKey( NodeTypeId ) )
            {
                _Cache.Remove( NodeTypeId );
            }
        } // clearLayout()

        /// <summary>
        /// Returns a dictionary of layout by tab
        /// </summary>
        public Dictionary<Int32, NodeTypeLayout> getLayout( CswEnumNbtLayoutType LayoutType, CswNbtMetaDataNodeTypeProp Prop )
        {
            return getLayout( LayoutType, Prop.NodeTypeId, Prop.PropId );
        }
        /// <summary>
        /// Returns a dictionary of layout by tab
        /// </summary>
        public Dictionary<Int32, NodeTypeLayout> getLayout( CswEnumNbtLayoutType LayoutType, Int32 NodeTypeId, Int32 PropId )
        {
            _CacheLayout( NodeTypeId );
            Dictionary<Int32, NodeTypeLayout> LayoutByTab = new Dictionary<Int32, NodeTypeLayout>();
            foreach( NodeTypeLayout Layout in _Cache[NodeTypeId].Where( Layout => Layout.LayoutType == LayoutType &&
                                                                                  Layout.PropId == PropId ) )
            {
                LayoutByTab[Layout.TabId] = Layout;
            }
            return LayoutByTab;
        } // getLayout()

        /// <summary>
        /// Returns a layout for a property on a tab.
        /// If edit, be sure to supply a valid TabId.
        /// </summary>
        public NodeTypeLayout getLayout( CswEnumNbtLayoutType LayoutType, Int32 NodeTypeId, Int32 PropId, Int32 TabId )
        {
            //NodeTypeLayout Layout = null;

            //CswTableSelect LayoutSelect = _CswNbtMetaDataResources.CswNbtResources.makeCswTableSelect( "getLayout_Select", "nodetype_layout" );
            //string WhereClause = "where layouttype = '" + LayoutType.ToString() + "' and nodetypepropid = " + PropId.ToString();
            //if( LayoutType == CswEnumNbtLayoutType.Edit )
            //{
            //    if( TabId != Int32.MinValue )
            //    {
            //        WhereClause += " and nodetypetabsetid = " + TabId.ToString();
            //    }
            //    else
            //    {
            //        throw new CswDniException( CswEnumErrorType.Error, "Missing Tab", "CswNbtMetaDataNodeTypeLayoutMgr.getLayout() requires a valid TabId for Edit layouts" );
            //    }
            //}
            //DataTable LayoutTable = LayoutSelect.getTable( WhereClause );
            //if( LayoutTable.Rows.Count > 0 )
            //{
            //    DataRow LayoutRow = LayoutTable.Rows[0];
            //    Layout = new NodeTypeLayout( LayoutRow );
            //}
            //return Layout;
            _CacheLayout( NodeTypeId );
            return _Cache[NodeTypeId].FirstOrDefault( Layout => Layout.LayoutType == LayoutType &&
                                                                Layout.PropId == PropId &&
                                                                ( LayoutType != CswEnumNbtLayoutType.Edit || Layout.TabId == TabId ) );
        }

        public void updatePropLayout( CswEnumNbtLayoutType LayoutType, Int32 NodeTypeId, CswNbtMetaDataNodeTypeProp NtProp, bool DoMove, Int32 TabId = Int32.MinValue, Int32 DisplayRow = Int32.MinValue, Int32 DisplayColumn = Int32.MinValue, string TabGroup = "" )
        {
            if( LayoutType != CswEnumNbtLayoutType.Unknown &&
                null != NtProp )
            {
                if( DoMove )
                {
                    removePropFromLayout( LayoutType, NtProp, Int32.MinValue );
                }
                CswTableUpdate LayoutUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "updatePropLayout_Update", "nodetype_layout" );
                string WhereClause = "where layouttype = '" + LayoutType.ToString() + "' and nodetypepropid = " + NtProp.PropId.ToString();
                if( TabId != Int32.MinValue && LayoutType == CswEnumNbtLayoutType.Edit )
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
                Row["auditlevel"] = CswConvert.ToDbVal( NtProp.AuditLevel ); // layout audit goes with property audit

                if( LayoutType == CswEnumNbtLayoutType.Edit )
                {
                    if( TabId != Int32.MinValue )
                    {
                        Row["nodetypetabsetid"] = CswConvert.ToDbVal( TabId );
                    }
                    else
                    {
                        throw new CswDniException( CswEnumErrorType.Error, "Tab is required", "CswNbtMetaDataNodeTypeLayoutMgr.updatePropLayout() requires a valid TabId for Edit layouts" );
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
            } // if( Type != CswEnumNbtLayoutType.Unknown && Prop != null )

            if( _Cache.ContainsKey( NodeTypeId ) )
            {
                _Cache.Remove( NodeTypeId );
            }
        } // updatePropLayout()

        public void updatePropLayout( CswEnumNbtLayoutType LayoutType, CswNbtMetaDataNodeTypeProp Prop, CswNbtMetaDataNodeTypeProp InsertAfterProp, bool DoMove )
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
                        if( TabId != Int32.MinValue && LayoutType == CswEnumNbtLayoutType.Edit )
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

            if( _Cache.ContainsKey( Prop.NodeTypeId ) )
            {
                _Cache.Remove( Prop.NodeTypeId );
            }
        } // updatePropLayout()

        public void removePropFromLayout( CswEnumNbtLayoutType LayoutType, CswNbtMetaDataNodeTypeProp Prop, Int32 TabId )
        {
            if( LayoutType != CswEnumNbtLayoutType.Unknown && Prop != null )
            {
                CswTableUpdate LayoutUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "removePropFromLayout_Update", "nodetype_layout" );
                string WhereClause = "where layouttype = '" + LayoutType.ToString() + "' and nodetypepropid = " + Prop.PropId.ToString();
                if( TabId != Int32.MinValue && LayoutType == CswEnumNbtLayoutType.Edit )
                {
                    WhereClause += " and nodetypetabsetid = " + TabId.ToString();
                }
                DataTable LayoutTable = LayoutUpdate.getTable( WhereClause );
                foreach( DataRow Row in LayoutTable.Rows )
                {
                    Row.Delete();
                }
                LayoutUpdate.update( LayoutTable );
            } // if( Type != CswEnumNbtLayoutType.Unknown && Prop != null )

            if( _Cache.ContainsKey( Prop.NodeTypeId ) )
            {
                _Cache.Remove( Prop.NodeTypeId );
            }
        } // removePropFromLayout()

        public void removePropFromAllLayouts( CswNbtMetaDataNodeTypeProp Prop )
        {
            if( Prop != null )
            {
                removePropFromAllLayouts( Prop.PropId );
            }

            if( _Cache.ContainsKey( Prop.NodeTypeId ) )
            {
                _Cache.Remove( Prop.NodeTypeId );
            }
        } // removePropFromAllLayouts()

        private void removePropFromAllLayouts( Int32 PropId )
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

        public Int32 getCurrentMaxDisplayRow( Int32 NodeTypeId, Int32 TabId, CswEnumNbtLayoutType LayoutType )
        {
            //CswTableSelect LayoutSelect = NbtResources.makeCswTableSelect( "getCurrentMaxDisplayRow_Select", "nodetype_layout" );
            //string WhereClause = "where layouttype = '" + LayoutType.ToString() + "' and nodetypeid = " + NodeTypeId.ToString() + " and display_row != " + Int32.MaxValue;
            //if( TabId != Int32.MinValue && LayoutType == CswEnumNbtLayoutType.Edit )
            //{
            //    WhereClause += " and nodetypetabsetid = " + TabId.ToString();
            //}
            //DataTable LayoutTable = LayoutSelect.getTable( WhereClause );
            //foreach( DataRow Row in LayoutTable.Rows )
            //{
            //    Int32 ThisRow = CswConvert.ToInt32( Row["display_row"] );
            //    if( ThisRow > MaxRow )
            //    {
            //        MaxRow = ThisRow;
            //    }
            //}
            _CacheLayout( NodeTypeId );

            return ( from Layout in _Cache[NodeTypeId]
                     where Layout.LayoutType == LayoutType && Layout.DisplayRow != Int32.MaxValue
                     select Layout.DisplayRow ).Concat( new[] { 0 } ).Max();

        } // getCurrentMaxDisplayRow()

        public IEnumerable<CswNbtMetaDataNodeTypeProp> getPropsInLayout( Int32 NodeTypeId, Int32 TabId, CswEnumNbtLayoutType LayoutType, CswDateTime Date = null )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getLayoutProps( NodeTypeId, TabId, LayoutType, Date );
        } // getPropsInLayout()

        public IEnumerable<CswNbtMetaDataNodeTypeProp> getPropsNotInLayout( CswNbtMetaDataNodeType NodeType, Int32 TabId, CswEnumNbtLayoutType LayoutType, CswDateTime Date = null )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getLayoutProps( NodeType.NodeTypeId, TabId, LayoutType, Date, false );
        } // getPropsNotInLayout()

        public void updateLayoutAuditLevel( CswNbtMetaDataNodeTypeProp NtProp, string AuditLevel )
        {
            CswTableUpdate LayoutUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "updatePropAuditLevel_Update", "nodetype_layout" );
            string WhereClause = "where nodetypepropid = " + NtProp.PropId.ToString();
            DataTable LayoutTable = LayoutUpdate.getTable( WhereClause );
            foreach( DataRow Row in LayoutTable.Rows )
            {
                Row["auditlevel"] = AuditLevel;
            }
            LayoutUpdate.update( LayoutTable );
        }
    } // public class CswNbtMetaDataNodeTypeLayout : ICswNbtMetaDataObject
} // namespace ChemSW.Nbt.MetaData
