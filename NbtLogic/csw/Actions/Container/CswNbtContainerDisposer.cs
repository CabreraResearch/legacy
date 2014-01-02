using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtContainerDisposer
    {
        private CswNbtObjClassContainer _Container;
        private CswNbtResources _CswNbtResources;
        private CswNbtContainerDispenseTransactionBuilder _ContainerDispenseTransactionBuilder;

        #region Constructor

        public CswNbtContainerDisposer( CswNbtResources CswNbtResources, CswNbtObjClassContainer Container )
        {
            _CswNbtResources = CswNbtResources;
            _ContainerDispenseTransactionBuilder = new CswNbtContainerDispenseTransactionBuilder( _CswNbtResources );
            _Container = Container;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Checks permission and disposes a container (does not post changes!)
        /// </summary>
        public void Dispose( bool OverridePermissions = false )
        {
            if( OverridePermissions || CswNbtObjClassContainer.canContainer( _CswNbtResources, _CswNbtResources.Actions[CswEnumNbtActionName.DisposeContainer], _Container.getPermissionGroupId() ) )
            {
                _ContainerDispenseTransactionBuilder.create( CswEnumNbtContainerDispenseType.Dispose, -_Container.Quantity.Quantity, _Container.Quantity.UnitId, SrcContainer: _Container );
                _Container.Quantity.Quantity = 0;
                _Container.Disposed.Checked = CswEnumTristate.True;
                _Container.CreateContainerLocationNode( CswEnumNbtContainerLocationTypeOptions.Dispose );
                _Container.Node.IconFileNameOverride = "x.png";
                _Container.Node.Searchable = false;
            }
        }

        /// <summary>
        /// Checks permission and undisposes a container (does not post changes!)
        /// </summary>
        public void Undispose( bool OverridePermissions = false, bool CreateContainerLocation = true )
        {
            if( OverridePermissions || CswNbtObjClassContainer.canContainer( _CswNbtResources, _CswNbtResources.Actions[CswEnumNbtActionName.UndisposeContainer], _Container.getPermissionGroupId() ) )
            {
                CswNbtObjClassContainerDispenseTransaction ContDispTransNode = _getMostRecentDisposeTransaction( _Container.NodeId );

                if( ContDispTransNode != null )
                {
                    _Container.Quantity.Quantity = -ContDispTransNode.QuantityDispensed.Quantity;
                    _Container.Quantity.UnitId = ContDispTransNode.QuantityDispensed.UnitId;
                    ContDispTransNode.Node.delete( OverridePermissions: true );
                }
                _Container.Disposed.Checked = CswEnumTristate.False;

                if( CreateContainerLocation )
                {
                    _Container.CreateContainerLocationNode( CswEnumNbtContainerLocationTypeOptions.Undispose );
                }
                _Container.Node.IconFileNameOverride = "";
            }
        }

        #endregion Public Methods

        #region Private Methods

        private CswNbtObjClassContainerDispenseTransaction _getMostRecentDisposeTransaction( CswPrimaryKey NodeId )
        {
            CswNbtObjClassContainerDispenseTransaction ContDispTransNode = null;
            CswNbtMetaDataObjectClass ContDispTransOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerDispenseTransactionClass );
            CswNbtMetaDataNodeType ContDispTransNT = ContDispTransOC.FirstNodeType;
            if( ContDispTransNT != null )
            {
                CswNbtView DisposedContainerTransactionsView = new CswNbtView( _CswNbtResources );
                DisposedContainerTransactionsView.ViewName = "ContDispTransDisposed";
                CswNbtViewRelationship ParentRelationship = DisposedContainerTransactionsView.AddViewRelationship( ContDispTransNT, false );

                DisposedContainerTransactionsView.AddViewPropertyAndFilter(
                    ParentRelationship,
                    ContDispTransNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.SourceContainer ),
                    NodeId.PrimaryKey.ToString(),
                    CswEnumNbtSubFieldName.NodeID,
                    false,
                    CswEnumNbtFilterMode.Equals
                    );

                DisposedContainerTransactionsView.AddViewPropertyAndFilter(
                    ParentRelationship,
                    ContDispTransNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.Type ),
                    CswEnumNbtContainerDispenseType.Dispose.ToString(),
                    CswEnumNbtSubFieldName.Value,
                    false,
                    CswEnumNbtFilterMode.Equals
                    );

                ICswNbtTree DispenseTransactionTree = _CswNbtResources.Trees.getTreeFromView( DisposedContainerTransactionsView, false, true, false );
                int NumOfTransactions = DispenseTransactionTree.getChildNodeCount();
                if( NumOfTransactions > 0 )
                {
                    DispenseTransactionTree.goToNthChild( 0 );
                    ContDispTransNode = DispenseTransactionTree.getNodeForCurrentPosition();
                }
            }
            return ContDispTransNode;
        }

        #endregion Private Methods
    }
}
