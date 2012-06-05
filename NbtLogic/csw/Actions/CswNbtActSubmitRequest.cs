using System;
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
            refreshCartCount();
        }

        #endregion Constructor

        #region Public methods

        public CswNbtView CartView;
        public Int32 CartCount = 0;

        #endregion Public methods

        public Int32 refreshCartCount()
        {
            CartCount = 0;
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( CartView, true, false );
            if( Tree.getChildNodeCount() > 0 )
            {
                Tree.goToNthChild( 0 );
                CswNbtNode RequestNode = Tree.getNodeForCurrentPosition();
                if( RequestNode.ObjClass.ObjectClass == _RequestOc )
                {
                    CartCount = Tree.getChildNodeCount();
                }
            }
            return CartCount;
        }
    }


}
