using System;
using System.Collections.Generic;
using System.Xml.Schema;
using ChemSW.Core;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt
{


    public class CswNbtTreeBuilder
    {
        private CswNbtResources _CswNbtResources = null;
        private CswNbtNodeCollection _CswNbtNodeCollection = null;

        public CswNbtTreeBuilder( CswNbtResources CswNbtResources, CswNbtNodeCollection CswNbtNodeCollection )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtNodeCollection = CswNbtNodeCollection;
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


        private ICswNbtTree _makeTree( CswNbtView View, bool IsFullyPopulated )
        {
            return new CswNbtTreeNodes( "", View, _CswNbtResources, _CswNbtNodeCollection, IsFullyPopulated );
        }

        /// <summary>
        /// Instance a Tree from a View
        /// </summary>
        public ICswNbtTree getTreeFromView( CswNbtView View, bool RequireViewPermissions, bool IncludeSystemNodes, bool IncludeHiddenNodes, bool IncludeTempNodes = false, Int32 PerLevelNodeLimit = Int32.MinValue )
        {
            return getTreeFromView( _CswNbtResources.CurrentNbtUser, View, RequireViewPermissions, IncludeSystemNodes, IncludeHiddenNodes, IncludeTempNodes, PerLevelNodeLimit );
        }

        /// <summary>
        /// Instance a Tree from a View
        /// </summary>
        public ICswNbtTree getTreeFromView( ICswNbtUser RunAsUser, CswNbtView View, bool RequireViewPermissions, bool IncludeSystemNodes, bool IncludeHiddenNodes, bool IncludeTempNodes = false, Int32 PerLevelNodeLimit = Int32.MinValue )
        {
            ICswNbtTree ReturnVal = _makeTree( View, true );

            CswNbtTreeLoaderFromXmlViewByLevel TreeLoader = new CswNbtTreeLoaderFromXmlViewByLevel( _CswNbtResources, RunAsUser, ReturnVal, View, IncludeSystemNodes, IncludeHiddenNodes, IncludeTempNodes );
            TreeLoader.load( RequireViewPermissions, PerLevelNodeLimit );

            return ( ReturnVal );

        }//getTreeFromView()


        /// <summary>
        /// Instance a Tree from a Universal Search
        /// </summary>
        public ICswNbtTree getTreeFromSearch( string SearchTerm, CswEnumSqlLikeMode SearchType, string WhereClause,
                                              bool RequireViewPermissions, bool IncludeSystemNodes, bool IncludeHiddenNodes, bool SingleNodetype, bool OnlyMergeableNodeTypes, Int32 PerLevelNodeLimit = Int32.MinValue, List<string> ExcludeNodeIds = null )
        {
            return getTreeFromSearch( _CswNbtResources.CurrentNbtUser, SearchTerm, SearchType, WhereClause, RequireViewPermissions, IncludeSystemNodes, IncludeHiddenNodes, SingleNodetype, OnlyMergeableNodeTypes, PerLevelNodeLimit, ExcludeNodeIds );
        }

        /// <summary>
        /// Instance a Tree from a Universal Search
        /// </summary>
        public ICswNbtTree getTreeFromSearch( ICswNbtUser RunAsUser, string SearchTerm, CswEnumSqlLikeMode SearchType, string WhereClause,
                                              bool RequireViewPermissions, bool IncludeSystemNodes, bool IncludeHiddenNodes, bool SingleNodetype, bool OnlyMergeableNodeTypes, Int32 PerLevelNodeLimit = Int32.MinValue, List<string> ExcludeNodeIds = null )
        {
            ICswNbtTree ReturnVal = _makeTree( null, true );
            CswNbtTreeLoaderFromSearchByLevel TreeLoader = new CswNbtTreeLoaderFromSearchByLevel( _CswNbtResources, RunAsUser, ReturnVal, SearchTerm, SearchType, WhereClause,
                                                                                                  IncludeSystemNodes, IncludeHiddenNodes, SingleNodetype, OnlyMergeableNodeTypes, ExcludeNodeIds );
            TreeLoader.load( RequireViewPermissions, PerLevelNodeLimit );

            return ( ReturnVal );

        }//getTreeFromSearch()

    }//CswNbtTreeBuilder

}//namespace ChemSW.Nbt
