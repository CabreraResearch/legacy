using System;
using System.Collections.ObjectModel;
using ChemSW.Core;

namespace ChemSW.Nbt.WebServices
{

    public class wsTreeOfView : System.Web.Services.WebService
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly string _IdPrefix;
        private CswNbtView _View;

        public wsTreeOfView( CswNbtResources CswNbtResources, CswNbtView View, string IdPrefix )
        {
            _CswNbtResources = CswNbtResources;
            _IdPrefix = IdPrefix;
            _View = View;
        } //ctor

        public string CachedTreeName
        {
            get
            {
                CswDelimitedString ret = new CswDelimitedString( '_' );
                ret.Add( "tree_" );
                ret.Add( _View.ViewId.ToString() );
                ret.Add( _IdPrefix );
                return ret.ToString();
            }
        }

        public void saveTreeToCache( ICswNbtTree Tree )
        {
            _CswNbtResources.CswSuperCycleCache.put( CachedTreeName, Tree );
        }

        public void deleteTreeFromCache()
        {
            _CswNbtResources.CswSuperCycleCache.delete( CachedTreeName );
        }

        public ICswNbtTree getTreeFromCache()
        {
            ICswNbtTree Tree = null;
            string CacheTreeName = CachedTreeName;
            if( _CswNbtResources.CswSuperCycleCache != null && _View != null )
            {
                ICswNbtTree CacheTree = (ICswNbtTree) _CswNbtResources.CswSuperCycleCache.get( CacheTreeName );
                if( CacheTree != null )
                {
                    // Make a local copy to iterate, to avoid race conditions with other threads
                    Tree = _CswNbtResources.Trees.getTreeFromXml( _View, CacheTree.getRawTreeXml() );
                }
                else
                {
                    // Refetch the tree
                    Tree = _CswNbtResources.Trees.getTreeFromView( _View, false );
                }
            }
            return Tree;
        } // _getCachedTree()

        public Collection<CswNbtNodeKey> getNextPageOfNodes( ICswNbtTree Tree, Int32 Level, Int32 ParentRangeStart, Int32 ParentRangeEnd )
        {
            Collection<CswNbtNodeKey> ret = new Collection<CswNbtNodeKey>();
            Collection<CswNbtNodeKey> NodeKeys = Tree.getKeysForLevel( Level );
            foreach( CswNbtNodeKey NodeKey in NodeKeys )
            {
                Int32 ParentCount = CswConvert.ToInt32( NodeKey.NodeCountPath[Level - 2] );
                if( ParentCount >= ParentRangeStart &&
                    ParentCount <= ParentRangeEnd )
                {
                    ret.Add( NodeKey );
                }
            }
            return ret;
        }

    }//wsNBT

} // namespace ChemSW.WebServices
