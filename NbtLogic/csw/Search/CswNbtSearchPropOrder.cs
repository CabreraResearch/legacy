using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using System.Linq;

namespace ChemSW.Nbt.Search
{
    /// <summary>
    /// Determines order of properties in searches and table views
    /// </summary>
    public class CswNbtSearchPropOrder
    {
        private CswNbtResources _CswNbtResources;
        private Dictionary<CswNbtMetaDataNodeType, IEnumerable<CswNbtMetaDataNodeTypeProp>> _TableLayoutDict = new Dictionary<CswNbtMetaDataNodeType, IEnumerable<CswNbtMetaDataNodeTypeProp>>();
        private Dictionary<CswNbtMetaDataNodeType, SortedSet<SearchOrder>> _PropOrderDict = new Dictionary<CswNbtMetaDataNodeType, SortedSet<SearchOrder>>();


        /// <summary>
        /// Enum: Source for the order of a property in a search
        /// </summary>
        public sealed class PropOrderSourceType : CswEnum<PropOrderSourceType>
        {
            private PropOrderSourceType( string Name ) : base( Name ) { }
            public static IEnumerable<PropOrderSourceType> _All { get { return All; } }

            public static explicit operator PropOrderSourceType( string str )
            {
                PropOrderSourceType ret = Parse( str );
                return ret ?? Unknown;
            }

            public static readonly PropOrderSourceType Unknown = new PropOrderSourceType( "Unknown" );
            public static readonly PropOrderSourceType View = new PropOrderSourceType( "View" );
            public static readonly PropOrderSourceType Table = new PropOrderSourceType( "Table" );
            public static readonly PropOrderSourceType Results = new PropOrderSourceType( "Results" );
        }

        public CswNbtSearchPropOrder( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
        }

        public class SearchOrder : IComparable<SearchOrder>
        {
            public Int32 NodeTypePropId;
            public PropOrderSourceType Source;
            public Int32 Order;

            public int CompareTo( SearchOrder other )
            {
                return Order.CompareTo( other.Order );
            }
        } // SearchOrder

        public Int32 getPropOrder(Int32 NodeTypePropId, CswNbtNodeKey NodeKey, CswNbtView View = null)
        {
            Int32 ret = 0;
            SortedSet<SearchOrder> dict = getPropOrderDict( NodeKey, View );
            if( null != dict )
            {
                foreach( SearchOrder ThisOrder in dict.Where( ThisOrder => ThisOrder.NodeTypePropId == NodeTypePropId ) )
                {
                    ret = ThisOrder.Order;
                }
            }
            return ret;
        } // getPropOrder()

        /// <summary>
        /// Returns the order in which properties should appear in the table
        /// </summary>
        public SortedSet<SearchOrder> getPropOrderDict( CswNbtNodeKey NodeKey, CswNbtView View = null )
        {
            SortedSet<SearchOrder> ret = null;
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeKey.NodeTypeId );
            if( null != NodeType )
            {
                if( false == _PropOrderDict.ContainsKey( NodeType ) )
                {
                    ret = new SortedSet<SearchOrder>();

                    // View order goes first
                    if( View != null )
                    {
                        CswNbtViewRelationship ViewRel = (CswNbtViewRelationship) View.FindViewNodeByUniqueId( NodeKey.ViewNodeUniqueId );
                        if( ViewRel != null )
                        {
                            foreach( CswNbtViewProperty ViewProp in ViewRel.Properties )
                            {
                                SearchOrder ThisOrder = new SearchOrder
                                                            {
                                                                NodeTypePropId = ViewProp.NodeTypePropId,
                                                                Source = PropOrderSourceType.View,
                                                                Order = 0
                                                            };
                                
                                
                                foreach( CswNbtViewProperty OtherViewProp in ViewRel.Properties )
                                {
                                    if( ( OtherViewProp.Order != Int32.MinValue && OtherViewProp.Order < ViewProp.Order ) ||
                                        ViewProp.Order == Int32.MinValue )
                                    {
                                        ThisOrder.Order += 1;
                                    }
                                }
                                ret.Add( ThisOrder );
                            } // foreach( CswNbtViewProperty ViewProp in ViewRel.Properties )
                        } // if( ViewRel != null )
                    } // if( _View != null )


                    // Table layout goes second
                    Int32 maxOrder = ( ret.Count > 0 ) ? ret.Max().Order : 0;
                    if( false == _TableLayoutDict.Keys.Contains( NodeType ) )
                    {
                        _TableLayoutDict[NodeType] = _CswNbtResources.MetaData.NodeTypeLayout.getPropsInLayout( NodeType.NodeTypeId, Int32.MinValue, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table );
                    }
                    foreach( CswNbtMetaDataNodeTypeProp Prop in _TableLayoutDict[NodeType])
                    {
                        SearchOrder ThisOrder = new SearchOrder
                                                    {
                                                        NodeTypePropId = Prop.PropId,
                                                        Source = PropOrderSourceType.Table,
                                                    };
                        if( false == ret.Contains( ThisOrder ) )
                        {
                            CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout propTableLayout = Prop.getTableLayout();
                            if( propTableLayout.DisplayRow > 0 )
                            {
                                ThisOrder.Order = maxOrder + propTableLayout.DisplayRow;
                                ret.Add( ThisOrder );
                            }
                        }
                    } // foreach( CswNbtMetaDataNodeTypeProp Prop in _TableLayoutDict[thisNode.NodeType] )


                    // Everything else in alphabetical order
                    maxOrder = ( ret.Count > 0 ) ? ret.Max().Order : 0;
                    foreach( CswNbtMetaDataNodeTypeProp Prop in NodeType.getNodeTypeProps()
                                                                    .OrderBy( Prop => Prop.PropName ) )
                    {
                        SearchOrder ThisOrder = new SearchOrder
                                                    {
                                                        NodeTypePropId = Prop.PropId,
                                                        Source = PropOrderSourceType.Results,
                                                    };
                        if( false == ret.Contains( ThisOrder ) )
                        {
                            maxOrder++;
                            ThisOrder.Order = maxOrder;
                            ret.Add( ThisOrder );
                        }
                    }
                    _PropOrderDict.Add( NodeType, ret );
                } // if( false == _PropOrderDict.ContainsKey( thisNode.NodeType ) )

                ret = _PropOrderDict[NodeType];
            } // if(null != NodeType)
            return ret;
        } //getPropOrderDict()

    } // class CswNbtSearchPropOrder
} // namespace ChemSW.Nbt



