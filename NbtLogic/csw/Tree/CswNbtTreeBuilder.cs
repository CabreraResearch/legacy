using System;
using System.Xml;
using System.Xml.Schema;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt
{


    public class CswNbtTreeBuilder
    {
        CswNbtResources _CswNbtResources = null;
        ICswNbtTreeFactory _CswNbtTreeFactory = null;
        public CswNbtTreeBuilder( CswNbtResources CswNbtResources, ICswNbtTreeFactory CswNbtTreeFactory )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtTreeFactory = CswNbtTreeFactory;
        }//ctor

        private string _SchemaPath = "";
        public string SchemaPath
        {
            set
            {
                _SchemaPath = value;
            }//
            get
            {
                return ( _SchemaPath );
            }
        }//SchemaPath property

        private bool _InTestMode = false;
        public bool InTestMode
        {
            set
            {
                _InTestMode = value;
            }//
            get
            {
                return ( _InTestMode );
            }
        }//InTestMode property

        private string _ValidationErrors = "";
        private void _handleValidationError( object sender,
            ValidationEventArgs ValidationEventArgs )
        {
            _ValidationErrors += ValidationEventArgs.Message + "; ";
        }




        private TreeMode _TreeMode = TreeMode.DomProxy;
        private ICswNbtTree _makeTree( CswNbtView View, bool IsFullyPopulated )
        {
            return ( _CswNbtTreeFactory.makeTree( _TreeMode, View, IsFullyPopulated ) );

        }//_makeTree()
        private ICswNbtTree _makeTree( bool IsFullyPopulated )
        {
            return ( _CswNbtTreeFactory.makeTree( _TreeMode, null, IsFullyPopulated ) );

        }//_makeTree()


        /// <summary>
        /// Deprecated
        /// </summary>
        public ICswNbtTree getTreeFromView( CswNbtView View, bool ForceReInit, bool FetchAllPrior, bool SingleLevelOnly, bool IncludeSystemNodes, Int32 PageSize = Int32.MinValue )
        {
            return getTreeFromView( View, true, IncludeSystemNodes );
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        public ICswNbtTree getTreeFromView( CswNbtView View, bool ForceReInit, bool FetchAllPrior, bool SingleLevelOnly, bool IncludeSystemNodes, bool RequireViewPermissions )
        {
            return getTreeFromView( View, true, IncludeSystemNodes );
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        public ICswNbtTree getTreeFromView( ICswNbtUser RunAsUser, CswNbtView View, bool ForceReInit, bool FetchAllPrior, bool SingleLevelOnly, bool IncludeSystemNodes )
        {
            return getTreeFromView( RunAsUser, View, true, IncludeSystemNodes );
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        public ICswNbtTree getTreeFromView( CswNbtView View, bool ForceReInit, ref CswNbtNodeKey ParentNodeKey, CswNbtViewRelationship ChildRelationshipToStartWith, Int32 PageSize, bool FetchAllPrior, bool SingleLevelOnly, CswNbtNodeKey IncludedKey, bool IncludeSystemNodes )
        {
            return getTreeFromView( View, true, IncludeSystemNodes );
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        public ICswNbtTree getTreeFromView( CswNbtView View, bool IncludeSystemNodes )
        {
            return getTreeFromView( _CswNbtResources.CurrentNbtUser, View, true, IncludeSystemNodes );
        }

        /// <summary>
        /// Instance a Tree from a View
        /// </summary>
        public ICswNbtTree getTreeFromView( CswNbtView View, bool RequireViewPermissions, bool IncludeSystemNodes )
        {
            return getTreeFromView( _CswNbtResources.CurrentNbtUser, View, RequireViewPermissions, IncludeSystemNodes );
        }

        /// <summary>
        /// Instance a Tree from a View
        /// </summary>
        public ICswNbtTree getTreeFromView( ICswNbtUser RunAsUser, CswNbtView View, bool RequireViewPermissions, bool IncludeSystemNodes )
        {
            ICswNbtTree ReturnVal = _makeTree( View, true );

            CswNbtTreeLoaderFromXmlViewByLevel TreeLoader = new CswNbtTreeLoaderFromXmlViewByLevel( _CswNbtResources, RunAsUser, ReturnVal, View, IncludeSystemNodes );
            TreeLoader.load( RequireViewPermissions );

            return ( ReturnVal );

        }//getTreeFromView()


        /// <summary>
        /// Instance a Tree from a Universal Search
        /// </summary>
        public ICswNbtTree getTreeFromSearch( string SearchTerm, string WhereClause, bool RequireViewPermissions, bool IncludeSystemNodes )
        {
            return getTreeFromSearch( _CswNbtResources.CurrentNbtUser, SearchTerm, WhereClause, RequireViewPermissions, IncludeSystemNodes );
        }

        /// <summary>
        /// Instance a Tree from a Universal Search
        /// </summary>
        public ICswNbtTree getTreeFromSearch( ICswNbtUser RunAsUser, string SearchTerm, string WhereClause, bool RequireViewPermissions, bool IncludeSystemNodes )
        {
            ICswNbtTree ReturnVal = _makeTree( true );

            CswNbtTreeLoaderFromSearchByLevel TreeLoader = new CswNbtTreeLoaderFromSearchByLevel( _CswNbtResources, RunAsUser, ReturnVal, SearchTerm, WhereClause, IncludeSystemNodes );
            TreeLoader.load( RequireViewPermissions );

            return ( ReturnVal );

        }//getTreeFromSearch()

    }//CswNbtTreeBuilder

}//namespace ChemSW.Nbt
