
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Actions.KioskModeRules.OperationClasses
{
    /// <summary>
    /// Default implementation of Openable methods
    /// </summary>
    public class CswNbtKioskModeMoveableImpl
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly ICswNbtKioskModeMoveable _MoveableObj;

        public CswNbtKioskModeMoveableImpl( CswNbtResources CswNbtResources, ICswNbtKioskModeMoveable MoveableObj )
        {
            _CswNbtResources = CswNbtResources;
            _MoveableObj = MoveableObj;
        }

        /// <summary>
        /// Default implementation on whether an item can be moved. Verifies the Location property is not null
        /// </summary>
        public bool CanMove( out string Error )
        {
            bool ret = null != _MoveableObj.Location;
            Error = ( ret ? string.Empty : "Cannot Move because Location property is null" );
            return ret;
        }

        /// <summary>
        /// Default implementation for moving an entity
        /// </summary>
        public void Move( CswNbtObjClassLocation locationToMoveTo )
        {
            _MoveableObj.Location.SelectedNodeId = locationToMoveTo.NodeId;
            _MoveableObj.Location.SyncGestalt();
            _MoveableObj.Location.RefreshNodeName();
        }
    }
}
