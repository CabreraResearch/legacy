namespace ChemSW.Nbt.Actions.KioskModeRules.OperationClasses
{
    /// <summary>
    /// Default implementation of Openable methods
    /// </summary>
    public class CswNbtKioskModeStatusableImpl
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly ICswNbtKioskModeStatusable _StatusableObj;

        public CswNbtKioskModeStatusableImpl( CswNbtResources CswNbtResources, ICswNbtKioskModeStatusable StatusableObj )
        {
            _CswNbtResources = CswNbtResources;
            _StatusableObj = StatusableObj;
        }

        /// <summary>
        /// Default implementation on whether an item can have it's status updated. Verifies the property is not null
        /// </summary>
        public bool CanChangeStatus( out string Error )
        {
            bool ret = null != _StatusableObj.Status;
            Error = ( ret ? string.Empty : "Cannot update Status because Status property is null" );
            return ret;
        }

        /// <summary>
        /// Default implementation for moving an entity
        /// </summary>
        public void ChangeStatus( string NewStatus )
        {
            _StatusableObj.Status.Value = NewStatus;
        }
    }
}
