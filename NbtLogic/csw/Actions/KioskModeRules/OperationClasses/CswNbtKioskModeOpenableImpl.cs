using System;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Actions.KioskModeRules.OperationClasses
{
    /// <summary>
    /// Default implementation of Openable methods
    /// </summary>
    public class CswNbtKioskModeOpenableImpl
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly ICswNbtKioskModeOpenable _OpenableObj;

        public CswNbtKioskModeOpenableImpl( CswNbtResources CswNbtResources, ICswNbtKioskModeOpenable OpenableObj )
        {
            _CswNbtResources = CswNbtResources;
            _OpenableObj = OpenableObj;
        }


        /// <summary>
        /// Default implementation that checks if the Openable has an Expiration Date and that the chemical has an open expiration interval set
        /// </summary>
        public bool CanOpen()
        {
            CswNbtPropertySetMaterial Material = _CswNbtResources.Nodes.GetNode( _OpenableObj.Material.RelatedNodeId );
            return ( DateTime.MinValue != _OpenableObj.ExpirationDate.DateTimeValue || ( false == String.IsNullOrEmpty( Material.OpenExpireInterval.CachedNodeName ) && false == Double.IsNaN( Material.OpenExpireInterval.Quantity ) ) );
        }

        public void OpenItem()
        {
            CswNbtNode Node = _CswNbtResources.Nodes.GetNode( _OpenableObj.Material.RelatedNodeId );
            CswNbtPropertySetMaterial Material = Node;

            if( false == CanOpen() )
            {
                throw new CswDniException( CswEnumErrorType.Warning,
                    "Cannot open container when Container does not have an expiration date set or the material does not have an open expiration interval set",
                    "Container.ExpirationDate isn't set or Container.Material.OpenExpirationInterval is not set, cannot open container" );
            }
            _OpenableObj.OpenedDate.DateTimeValue = DateTime.Now;
            _OpenableObj.ExpirationDate.DateTimeValue = ( _OpenableObj.ExpirationDate.DateTimeValue < Material.getDefaultOpenExpirationDate( DateTime.Now ) ? _OpenableObj.ExpirationDate.DateTimeValue : Material.getDefaultOpenExpirationDate( DateTime.Now ) );
            _OpenableObj.Open.setHidden( true, true );
        }
    }
}
