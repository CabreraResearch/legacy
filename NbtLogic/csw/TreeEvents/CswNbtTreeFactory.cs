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

            CswNbtTreeDomProxy CswNbtTreeDomProxy = new CswNbtTreeDomProxy( View, _CswNbtResources, CswNbtNodeCollection, IsFullyPopulated );

            CswNbtTreeDomProxy.onBeforeInsertNode += CswNbtTreeEventInsertNodeGeneric.handleBeforeInsertNode;
            CswNbtTreeDomProxy.onAfterInsertNode += CswNbtTreeEventInsertNodeGeneric.handleAfterInsertNode;

            ReturnVal = CswNbtTreeDomProxy;

            return ( ReturnVal );

        }//makeTree()

    }//class CswNbtTreeFactory

}//namespace ChemSW.Nbt.TableEvents


