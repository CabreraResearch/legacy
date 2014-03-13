

using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Actions.KioskModeRules.OperationClasses
{
    /// <summary>
    /// Default implementation of Openable methods
    /// </summary>
    public class CswNbtKioskModeTransferableImpl
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly ICswNbtKioskModeTransferable _TransferableObj;

        public CswNbtKioskModeTransferableImpl( CswNbtResources CswNbtResources, ICswNbtKioskModeTransferable TransferableObj )
        {
            _CswNbtResources = CswNbtResources;
            _TransferableObj = TransferableObj;
        }

        /// <summary>
        /// Default implementation on whether an item can be transfered. Verifies the Owner relationship property points to a User and the location prop is not null
        /// </summary>
        public bool CanTransfer( out string Error )
        {
            bool ret = ( null != _TransferableObj.Location ) && _TransferableObj.User.IsUserRelationship();
            Error = ( ret ? string.Empty : "Cannot transfer entity, properties are not configured correctly." );
            return ret;
        }

        /// <summary>
        /// Default implementation for moving an entity
        /// </summary>
        public void Transfer(CswNbtObjClassUser NewUser)
        {
            _TransferableObj.Location.SelectedNodeId = NewUser.DefaultLocationId;
            _TransferableObj.Location.SyncGestalt();
            _TransferableObj.Location.RefreshNodeName();

            _TransferableObj.User.RelatedNodeId = NewUser.NodeId;
            _TransferableObj.User.RefreshNodeName();
            _TransferableObj.User.SyncGestalt();
        }

    }
}
