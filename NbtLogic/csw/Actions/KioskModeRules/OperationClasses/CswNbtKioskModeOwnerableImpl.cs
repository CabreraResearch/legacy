
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Actions.KioskModeRules.OperationClasses
{
    /// <summary>
    /// Default implementation of Openable methods
    /// </summary>
    public class CswNbtKioskModeOwnerableImpl
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly ICswNbtKioskModeOwnerable _OwnerableObj;

        public CswNbtKioskModeOwnerableImpl( CswNbtResources CswNbtResources, ICswNbtKioskModeOwnerable OwnerableObj )
        {
            _CswNbtResources = CswNbtResources;
            _OwnerableObj = OwnerableObj;
        }

        /// <summary>
        /// Default implementation on whether an item have the owner updated. Verifies the Owner relationship property points to a User
        /// </summary>
        public bool CanMove( out string Error )
        {
            bool ret = _OwnerableObj.User.IsUserRelationship();
            Error = ( ret ? string.Empty : "Cannot modify Owner property because it is not configured to be a User relationship" );
            return ret;
        }

        /// <summary>
        /// Default implementation for moving an entity
        /// </summary>
        public void UpdateOwner( CswNbtObjClassUser NewOwner )
        {
            _OwnerableObj.User.RelatedNodeId = NewOwner.NodeId;
            _OwnerableObj.User.RefreshNodeName();
            _OwnerableObj.User.SyncGestalt();
        }
    }
}
