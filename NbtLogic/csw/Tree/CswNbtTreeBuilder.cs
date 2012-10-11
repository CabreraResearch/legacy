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
            //_CswDbResources = CswNbtResources.CswDbResources;
            _CswNbtTreeFactory = CswNbtTreeFactory;
        }//ctor

        //public bool isTreeViewValid( string TreeViewDoc , ref string ErrorMessage )
        //{
        //    bool ReturnVal = true;

        //    _validateTreeView( TreeViewDoc );

        //    if(  _ValidationErrors.Length > 0 )
        //    {
        //        ReturnVal = false;
        //        ErrorMessage = _ValidationErrors;
        //    }

        //    _ValidationErrors = "";

        //    return ( ReturnVal );

        //}//validateTreeView()



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


        //private string _SchemaDoc = "";
        //public string SchemaDoc
        //{
        //    set
        //    {
        //        _SchemaDoc = value;
        //        _loadSchemaDoc();
        //    }//
        //    get
        //    {
        //        return ( _SchemaDoc );
        //    }
        //}//SchemaDoc property

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

        //XmlReaderSettings _XmlReaderSettings = null;
        //private void _loadSchemaDoc()
        //{
        //    _XmlReaderSettings = new XmlReaderSettings();
        //    string SchemaFileFqn = _SchemaPath + "\\" + _SchemaDoc ;

        //    if( ! File.Exists( SchemaFileFqn ) )
        //        throw( new CswDniException( "Schema file does not exist " + SchemaFileFqn ) );

        //    _XmlReaderSettings.Schemas.Add( null, SchemaFileFqn );
        //    _XmlReaderSettings.ValidationType = ValidationType.Schema;
        //    _XmlReaderSettings.ValidationEventHandler += new ValidationEventHandler( _handleValidationError );

        //}//_loadSchemaDoc()


        //private void _validateTreeView( string TreeViewDoc )
        //{
        //    _loadSchemaDoc();

        //    if( null == _XmlReaderSettings )
        //        throw ( new System.Exception( "There is no schema document: set the SchemaPath property" ) );

        //    XmlReader XmlReader = null;

        //    try
        //    {
        //        TextReader TreeViewStringReader = new StringReader( TreeViewDoc );
        //        XmlReader = XmlReader.Create( TreeViewStringReader, _XmlReaderSettings );

        //        while( XmlReader.Read() ) { }

        //    }//try

        //    catch( System.Exception Exception )
        //    {
        //        _ValidationErrors = Exception.Message;
        //    }

        //    finally
        //    {
        //        XmlReader.Close();

        //    }

        //}////_validateTreeView()

        private string _ValidationErrors = "";
        private void _handleValidationError( object sender,
            ValidationEventArgs ValidationEventArgs )
        {
            _ValidationErrors += ValidationEventArgs.Message + "; ";
        }




        private TreeMode _TreeMode = TreeMode.DomProxy;
        private ICswNbtTree _makeTree( CswNbtView View, bool IsFullyPopulated ) //CswNbtTreeKey CswNbtTreeKey )
        {
            return ( _CswNbtTreeFactory.makeTree( _TreeMode, View, IsFullyPopulated ) ); //, CswNbtTreeKey ) );

        }//_makeTree()
        private ICswNbtTree _makeTree( bool IsFullyPopulated ) //CswNbtTreeKey CswNbtTreeKey )
        {
            return ( _CswNbtTreeFactory.makeTree( _TreeMode, null, IsFullyPopulated ) ); //, CswNbtTreeKey ) );

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
        public ICswNbtTree getTreeFromView( CswNbtView View, bool ForceReInit, ref CswNbtNodeKey ParentNodeKey, CswNbtViewRelationship ChildRelationshipToStartWith, Int32 PageSize, bool FetchAllPrior, bool SingleLevelOnly, CswNbtNodeKey IncludedKey, bool IncludeSystemNodes )
        {
            return getTreeFromView( View, true, IncludeSystemNodes );
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        public ICswNbtTree getTreeFromView( CswNbtView View, bool IncludeSystemNodes, bool IncludeHiddenNodes )
        {
            return getTreeFromView( _CswNbtResources.CurrentNbtUser, View, true, IncludeSystemNodes, IncludeHiddenNodes );
        }

        /// <summary>
        /// Instance a Tree from a View
        /// </summary>
        public ICswNbtTree getTreeFromView( CswNbtView View, bool RequireViewPermissions, bool IncludeSystemNodes, bool IncludeHiddenNodes )
        {
            return getTreeFromView( _CswNbtResources.CurrentNbtUser, View, RequireViewPermissions, IncludeSystemNodes, IncludeHiddenNodes );
        }

        /// <summary>
        /// Instance a Tree from a View
        /// </summary>
        public ICswNbtTree getTreeFromView( ICswNbtUser RunAsUser, CswNbtView View, bool RequireViewPermissions, bool IncludeSystemNodes, bool IncludeHiddenNodes )
        {
            ICswNbtTree ReturnVal = _makeTree( View, true );

            CswNbtTreeLoaderFromXmlViewByLevel TreeLoader = new CswNbtTreeLoaderFromXmlViewByLevel( _CswNbtResources, RunAsUser, ReturnVal, View, IncludeSystemNodes, IncludeHiddenNodes );
            TreeLoader.load( RequireViewPermissions );

            return ( ReturnVal );

        }//getTreeFromView()


        /// <summary>
        /// Instance a Tree from a Universal Search
        /// </summary>
        public ICswNbtTree getTreeFromSearch( string SearchTerm, string WhereClause, bool RequireViewPermissions, bool IncludeSystemNodes, bool IncludeHiddenNodes )
        {
            return getTreeFromSearch( _CswNbtResources.CurrentNbtUser, SearchTerm, WhereClause, RequireViewPermissions, IncludeSystemNodes, IncludeHiddenNodes );
        }

        /// <summary>
        /// Instance a Tree from a Universal Search
        /// </summary>
        public ICswNbtTree getTreeFromSearch( ICswNbtUser RunAsUser, string SearchTerm, string WhereClause, bool RequireViewPermissions, bool IncludeSystemNodes, bool IncludeHiddenNodes )
        {
            ICswNbtTree ReturnVal = _makeTree( true );

            CswNbtTreeLoaderFromSearchByLevel TreeLoader = new CswNbtTreeLoaderFromSearchByLevel( _CswNbtResources, RunAsUser, ReturnVal, SearchTerm, WhereClause, IncludeSystemNodes, IncludeHiddenNodes );
            TreeLoader.load( RequireViewPermissions );

            return ( ReturnVal );

        }//getTreeFromSearch()

        ///// <summary>
        ///// Instance a Tree from Raw XML
        ///// </summary>
        //public ICswNbtTree getTreeFromXml( CswNbtView View, XmlDocument XmlDoc )
        //{
        //    ICswNbtTree ReturnVal = _makeTree( View, true );
        //    ReturnVal.setRawTreeXml( XmlDoc );
        //    return ( ReturnVal );
        //}

    }//CswNbtTreeBuilder

}//namespace ChemSW.Nbt
