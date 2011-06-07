using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using ChemSW.Exceptions;
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




		public ICswNbtTree getTreeFromView( CswNbtView View, bool ForceReInit, bool FetchAllPrior, bool SingleLevelOnly, bool IncludeSystemNodes )
		{
			CswNbtNodeKey ParentNodeKey = null;
			return getTreeFromView( View, ForceReInit, ref ParentNodeKey, null, Int32.MinValue, FetchAllPrior, SingleLevelOnly, null, IncludeSystemNodes );
		}

		public ICswNbtTree getTreeFromView( CswNbtView View, bool ForceReInit, bool FetchAllPrior, bool SingleLevelOnly, bool IncludeSystemNodes, bool RequireViewPermissions )
		{
			CswNbtNodeKey ParentNodeKey = null;
			return getTreeFromView( _CswNbtResources.CurrentNbtUser, View, ForceReInit, ref ParentNodeKey, null, Int32.MinValue, FetchAllPrior, SingleLevelOnly, null, IncludeSystemNodes, RequireViewPermissions );
		}

		public ICswNbtTree getTreeFromView( ICswNbtUser RunAsUser, CswNbtView View, bool ForceReInit, bool FetchAllPrior, bool SingleLevelOnly, bool IncludeSystemNodes )
		{
		    CswNbtNodeKey ParentNodeKey = null;
		    return getTreeFromView( RunAsUser, View, ForceReInit, ref ParentNodeKey, null, Int32.MinValue, FetchAllPrior, SingleLevelOnly, null, IncludeSystemNodes, true );
		}

		public ICswNbtTree getTreeFromView( CswNbtView View, bool ForceReInit, ref CswNbtNodeKey ParentNodeKey, CswNbtViewRelationship ChildRelationshipToStartWith, Int32 PageSize, bool FetchAllPrior, bool SingleLevelOnly, CswNbtNodeKey IncludedKey, bool IncludeSystemNodes )
		{
			return getTreeFromView( _CswNbtResources.CurrentNbtUser, View, ForceReInit, ref ParentNodeKey, ChildRelationshipToStartWith, PageSize, FetchAllPrior, SingleLevelOnly, IncludedKey, IncludeSystemNodes, true );
		}

        public ICswNbtTree getTreeFromView( ICswNbtUser RunAsUser, 
											//CswNbtTreeKey CswNbtTreeKey, 
											CswNbtView View, 
											bool ForceReInit,
											ref CswNbtNodeKey ParentNodeKey, 
											CswNbtViewRelationship ChildRelationshipToStartWith, 
											Int32 PageSize, 
											bool FetchAllPrior, 
											bool SingleLevelOnly, 
											CswNbtNodeKey IncludedKey, 
											bool IncludeSystemNodes,
											bool RequireViewPermissions )
        {
			ICswNbtTree ReturnVal = _makeTree( View, ( FetchAllPrior && !SingleLevelOnly ) ); // CswNbtTreeKey );
            CswNbtTreeLoaderFactory CswNbtTreeLoaderFactory = new CswNbtTreeLoaderFactory( _CswNbtResources );

            CswNbtTreeLoader CswNbtTreeLoader = null;
            CswNbtTreeLoader = CswNbtTreeLoaderFactory.makeTreeLoaderFromXmlView( RunAsUser, ReturnVal, View, IncludeSystemNodes );
			CswNbtTreeLoader.load( ref ParentNodeKey, ChildRelationshipToStartWith, PageSize, FetchAllPrior, SingleLevelOnly, IncludedKey, RequireViewPermissions );
			
			//ReturnVal.goToRoot();

            return ( ReturnVal );

        }//getTreeFromView()

        //public ICswNbtTree getTreeFromSearchString( CswNbtTreeKey CswNbtTreeKey, CswNbtSearch NbtSearch )
        //{
        //    ICswNbtTree ReturnVal = _makeTree( CswNbtTreeKey );
        //    //            CswNbtTree ReturnVal = new CswNbtTree( _CswNbtResources, SchemaPath );

        //    CswNbtTreeLoaderFactory CswNbtTreeLoaderFactory = new CswNbtTreeLoaderFactory( _CswNbtResources );

        //    CswNbtTreeLoader CswNbtTreeLoader = null;
        //    CswNbtTreeLoader = CswNbtTreeLoaderFactory.makeTreeLoaderFromSearchString( ReturnVal, NbtSearch );

        //    CswNbtNodeKey ParentNodeKey = null;
        //    CswNbtTreeLoader.load(ref ParentNodeKey, null, Int32.MinValue, false, false, null);

        //    return ( ReturnVal );
        //}

    }//CswNbtTreeBuilder

}//namespace ChemSW.Nbt
