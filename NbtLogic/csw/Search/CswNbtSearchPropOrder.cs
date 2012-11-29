using System;
using System.Collections.Generic;
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
        private Dictionary<CswNbtMetaDataNodeType, Dictionary<Int32, Int32>> _PropOrderDict = new Dictionary<CswNbtMetaDataNodeType, Dictionary<Int32, Int32>>();

        public CswNbtSearchPropOrder( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
        }

        public Int32 getPropOrder(Int32 NodeTypePropId, CswNbtNodeKey NodeKey, CswNbtView View = null)
        {
            Int32 ret = 0;
            Dictionary<Int32, Int32> dict = getPropOrderDict( NodeKey, View );
            if( null != dict )
            {
                ret = dict[NodeTypePropId];
            }
            return ret;
        } // getPropOrder()

        /// <summary>
        /// Returns the order in which properties should appear in the table
        /// </summary>
        public Dictionary<Int32, Int32> getPropOrderDict(CswNbtNodeKey NodeKey, CswNbtView View = null)
        {
            Dictionary<Int32, Int32> ret = null;
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeKey.NodeTypeId );
            if( null != NodeType )
            {
                if( false == _PropOrderDict.ContainsKey( NodeType ) )
                {
                    Dictionary<Int32, Int32> dict = new Dictionary<Int32, Int32>();

                    // View order goes first
                    if( View != null )
                    {
                        CswNbtViewRelationship ViewRel = (CswNbtViewRelationship) View.FindViewNodeByUniqueId( NodeKey.ViewNodeUniqueId );
                        if( ViewRel != null )
                        {
                            foreach( CswNbtViewProperty ViewProp in ViewRel.Properties )
                            {
                                Int32 ThisOrder = 0;
                                foreach( CswNbtViewProperty OtherViewProp in ViewRel.Properties )
                                {
                                    if( ( OtherViewProp.Order != Int32.MinValue && OtherViewProp.Order < ViewProp.Order ) ||
                                        ViewProp.Order == Int32.MinValue )
                                    {
                                        ThisOrder++;
                                    }
                                }
                                while( dict.ContainsValue( ThisOrder ) )
                                {
                                    ThisOrder++;
                                }
                                dict.Add( ViewProp.NodeTypePropId, ThisOrder );
                            } // foreach( CswNbtViewProperty ViewProp in ViewRel.Properties )
                        } // if( ViewRel != null )
                    } // if( _View != null )


                    // Table layout goes second
                    Int32 maxOrder = ( dict.Values.Count > 0 ) ? dict.Values.Max() : 0;
                    if( false == _TableLayoutDict.Keys.Contains( NodeType ) )
                    {
                        _TableLayoutDict[NodeType] = _CswNbtResources.MetaData.NodeTypeLayout.getPropsInLayout( NodeType.NodeTypeId, Int32.MinValue, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table );
                    }
                    foreach( CswNbtMetaDataNodeTypeProp Prop in _TableLayoutDict[NodeType]
                                                                    .Where( Prop => false == dict.ContainsKey( Prop.PropId ) ) )
                    {
                        CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout propTableLayout = Prop.getTableLayout();
                        if( propTableLayout.DisplayRow > 0 )
                        {
                            Int32 ThisOrder = maxOrder + propTableLayout.DisplayRow;
                            while( dict.ContainsValue( ThisOrder ) )
                            {
                                ThisOrder++;
                            }
                            dict.Add( Prop.PropId, ThisOrder );
                        }
                    } // foreach( CswNbtMetaDataNodeTypeProp Prop in _TableLayoutDict[thisNode.NodeType] )


                    // Everything else in alphabetical order
                    maxOrder = ( dict.Values.Count > 0 ) ? dict.Values.Max() : 0;
                    foreach( CswNbtMetaDataNodeTypeProp Prop in NodeType.getNodeTypeProps()
                                                                    .Where( Prop => false == dict.ContainsKey( Prop.PropId ) )
                                                                    .OrderBy( Prop => Prop.PropName ) )
                    {
                        maxOrder++;
                        dict.Add( Prop.PropId, maxOrder );
                    }
                    _PropOrderDict.Add( NodeType, dict );
                } // if( false == _PropOrderDict.ContainsKey( thisNode.NodeType ) )

                ret = _PropOrderDict[NodeType];
            } // if(null != NodeType)
            return ret;
        } //getPropOrderDict()

    } // class CswNbtSearchPropOrder
} // namespace ChemSW.Nbt



