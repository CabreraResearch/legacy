using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtContainerDispenseTransactionBuilder
    {
        private CswNbtResources _CswNbtResources;

        #region Constructor

        public CswNbtContainerDispenseTransactionBuilder( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Record a container dispense transaction
        /// </summary>
        /// <param name="DispenseType"></param>
        /// <param name="Quantity">Quantity adjustment (negative for dispenses, disposes, and wastes, positive for receiving and add)</param>
        /// <param name="UnitId"></param>
        /// <param name="RequestItemId"></param>
        /// <param name="SourceContainer"></param>
        /// <param name="DestinationContainer"></param>
        public void create( CswEnumNbtContainerDispenseType DispenseType, double Amount, CswPrimaryKey UnitId, CswPrimaryKey RequestItemId = null,
                                                      CswNbtObjClassContainer SrcContainer = null, CswNbtObjClassContainer DestinationContainer = null )
        {
            CswNbtMetaDataObjectClass ContDispTransOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerDispenseTransactionClass );
            CswNbtMetaDataNodeType ContDispTransNT = ContDispTransOC.FirstNodeType;
            if( ContDispTransNT != null )
            {
                _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ContDispTransNT.NodeTypeId, delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassContainerDispenseTransaction ContDispTransNode = NewNode;
                    if( SrcContainer != null )
                    {
                        ContDispTransNode.SourceContainer.RelatedNodeId = SrcContainer.NodeId;
                        ContDispTransNode.RemainingSourceContainerQuantity.Quantity = SrcContainer.Quantity.Quantity;
                        if( DispenseType == CswEnumNbtContainerDispenseType.Dispose )
                        {
                            ContDispTransNode.RemainingSourceContainerQuantity.Quantity = 0;
                        }
                        ContDispTransNode.RemainingSourceContainerQuantity.UnitId = SrcContainer.Quantity.UnitId;
                    }
                    if( DestinationContainer != null )
                    {
                        ContDispTransNode.DestinationContainer.RelatedNodeId = DestinationContainer.NodeId;
                    }
                    ContDispTransNode.Dispenser.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
                    ContDispTransNode.QuantityDispensed.Quantity = Amount;
                    ContDispTransNode.QuantityDispensed.UnitId = UnitId;
                    ContDispTransNode.Type.Value = DispenseType.ToString();
                    ContDispTransNode.DispensedDate.DateTimeValue = DateTime.Now;
                    if( null != RequestItemId && Int32.MinValue != RequestItemId.PrimaryKey )
                    {
                        ContDispTransNode.RequestItem.RelatedNodeId = RequestItemId;
                    }
                    //ContDispTransNode.postChanges( false );
                } );
            } // if( ContDispTransNT != null )
        } // _createContainerTransactionNode

        #endregion Public Methods

        #region Private Methods

        
        #endregion Private Methods
    }
}
