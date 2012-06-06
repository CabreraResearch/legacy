using System;
using System.Linq;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Actions
{
    public class CswNbtActSubmitRequest
    {


        #region Private, core methods

        private CswNbtResources _CswNbtResources = null;
        private CswNbtMetaDataObjectClass _RequestOc = null;
        private CswNbtMetaDataObjectClass _RequestItemOc = null;

        #endregion Private, core methods

        #region Constructor

        private CswNbtActSystemViews _SystemViews;
        public CswNbtActSubmitRequest( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            if( false == _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.CISPro ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot use the Submit Request action without the required module.", "Attempted to constuct CswNbtActSubmitRequest without the required module." );
            }

            _SystemViews = new CswNbtActSystemViews( _CswNbtResources, CswNbtActSystemViews.SystemViewName.CISProRequestCart, null );
            CartView = _SystemViews.SystemView;
            _RequestOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass );
            _RequestItemOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
            refreshCartCounts();
        }

        #endregion Constructor

        #region Public methods

        public CswNbtView CartView;
        public Int32 CartContentCount = 0;
        public Int32 CartCount = 0;
        #endregion Public methods

        public Int32 refreshCartCounts()
        {
            CartContentCount = 0;
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( CartView, true, false );
            CartCount = Tree.getChildNodeCount();
            if( CartCount == 1 )
            {
                Tree.goToNthChild( 0 );
                CswNbtNode RequestNode = Tree.getNodeForCurrentPosition();
                if( RequestNode.ObjClass.ObjectClass == _RequestOc )
                {
                    CartContentCount = Tree.getChildNodeCount();
                }
            }
            else if( CartCount > 1 )
            {
                throw new CswDniException( ErrorType.Warning, "Only one pending request may be open at a time.", "There is more than one Pending request assigned to the current user." );
            }
            else if( CartCount == 0 )
            {
                CswNbtMetaDataNodeType RequestNt = _RequestOc.getLatestVersionNodeTypes().FirstOrDefault();
                if( null == RequestNt )
                {
                    throw new CswDniException( ErrorType.Warning,
                                                "Cannot Submit Request without a valid Request object.",
                                                "No Request NodeType could be found." );
                }
                CswNbtNode RequestNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( RequestNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                RequestNode.postChanges( true );
                refreshCartCounts();
            }
            return CartContentCount;
        }
    }


}
