using ChemSW.Exceptions;

namespace ChemSW.Nbt.TreeEvents
{

    public class CswNbtTreeFactory : ICswNbtTreeFactory
    {

        private string _SchemaPath = "";
        public CswNbtTreeFactory( string SchemaPath )
        {
            _CswNbtResources = CswNbtResources;
            _SchemaPath = SchemaPath;
        }//ctor

        private CswNbtNodeCollection _CswNbtNodeCollection = null;
        public CswNbtNodeCollection CswNbtNodeCollection
        {
            get
            {
                if( _CswNbtNodeCollection == null )
                    throw new CswDniException( "CswNbtTreeFactory.CswNbtNodeCollection is null" );
                return _CswNbtNodeCollection;
            }
            set { _CswNbtNodeCollection = value; }
        }

        CswNbtResources _CswNbtResources = null;
        public CswNbtResources CswNbtResources
        {
            get
            {
                return ( _CswNbtResources );
            }
            set
            {
                _CswNbtResources = value;
            }
        }

        public ICswNbtTree makeTree( TreeMode TreeMode, CswNbtView View, bool IsFullyPopulated )
        {
            ICswNbtTree ReturnVal = null;
            CswNbtNodeWriter CswNbtNodeWriter = new CswNbtNodeWriter( _CswNbtResources );
            CswNbtNodeReader CswNbtNodeReader = new CswNbtNodeReader( _CswNbtResources );

            CswNbtTreeDomProxy CswNbtTreeDomProxy = new CswNbtTreeDomProxy( View, _CswNbtResources, CswNbtNodeWriter, CswNbtNodeCollection, IsFullyPopulated );

            CswNbtTreeDomProxy.onBeforeInsertNode += new CswNbtTreeDomProxy.CswNbtTreeModificationHandler( CswNbtTreeEventInsertNodeGeneric.handleBeforeInsertNode );
            CswNbtTreeDomProxy.onAfterInsertNode += new CswNbtTreeDomProxy.CswNbtTreeModificationHandler( CswNbtTreeEventInsertNodeGeneric.handleAfterInsertNode );

            ReturnVal = CswNbtTreeDomProxy;

            return ( ReturnVal );

        }//makeTree()

    }//class CswNbtTreeFactory

}//namespace ChemSW.Nbt.TableEvents


