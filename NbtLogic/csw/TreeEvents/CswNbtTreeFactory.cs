using ChemSW.Exceptions;

namespace ChemSW.Nbt.TreeEvents
{

    public class CswNbtTreeFactory : ICswNbtTreeFactory
    {

        public CswNbtTreeFactory( )
        {
            _CswNbtResources = CswNbtResources;
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

            CswNbtTreeNodes CswNbtTreeNodes = new CswNbtTreeNodes( "", View, _CswNbtResources, CswNbtNodeCollection, IsFullyPopulated );

            CswNbtTreeNodes.onBeforeInsertNode += CswNbtTreeEventInsertNodeGeneric.handleBeforeInsertNode;
            CswNbtTreeNodes.onAfterInsertNode += CswNbtTreeEventInsertNodeGeneric.handleAfterInsertNode;

            ReturnVal = CswNbtTreeNodes;

            return ( ReturnVal );

        }//makeTree()

    }//class CswNbtTreeFactory

}//namespace ChemSW.Nbt.TableEvents


