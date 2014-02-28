using System;
using ChemSW.Nbt.Actions.KioskModeRules.OperationClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Actions.KioskMode
{
    public class CswNbtKioskModeRuleOpen: CswNbtKioskModeRule
    {
        public CswNbtKioskModeRuleOpen( CswNbtResources NbtResources )
            : base( NbtResources )
        {
            _CswNbtResources = NbtResources;
        }

        public override void ValidateFieldOne( ref OperationData OpData )
        {
            bool IsValid = _validateContainer( ref OpData );
            if( IsValid )
            {
                base.ValidateFieldOne( ref OpData );
            }
            //Intentionally overwrite what base.ValidateFieldOne does
            OpData.Field1.Active = true;
            OpData.Field2.Active = false;
        }

        public override void SetFields( ref OperationData OpData )
        {
            base.SetFields( ref OpData );
            OpData.Field1.Name = "Container:";
            OpData.Field2.ServerValidated = true;
        }

        public override void ValidateFieldTwo( ref OperationData OpData )
        {
            //Intentionally do nothing here - Open mode only has one field to work with
        }

        public override void CommitOperation( ref OperationData OpData )
        {
            CswNbtNode NodeToOpen = _CswNbtResources.Nodes[OpData.Field1.NodeId];
            if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Edit, NodeToOpen.getNodeType() ) )
            {
                ICswNbtKioskModeOpenable containerToOpen = (ICswNbtKioskModeOpenable) NodeToOpen.ObjClass;
                containerToOpen.OpenItem();
                NodeToOpen.postChanges( false );
                OpData.Log.Add( DateTime.Now + " - Opened container " + OpData.Field1.Value + " expiration date set to: " + containerToOpen.ExpirationDate.Gestalt );
                OpData.Field1.Value = string.Empty;
                OpData.Field1.ServerValidated = false;
            }
            else
            {
                string statusMsg = "You do not have permission to open Container (" + OpData.Field1.Value + ")";
                OpData.Field1.StatusMsg = statusMsg;
                OpData.Field1.ServerValidated = false;
                OpData.Log.Add( DateTime.Now + " - ERROR: " + statusMsg );
            }
        }

        #region Private Functions

        private bool _validateContainer( ref OperationData OpData )
        {
            bool ret = false;

            ICswNbtTree tree = _getTree( CswEnumNbtObjectClass.ContainerClass, OpData.Field1.Value, false );
            if( tree.getChildNodeCount() > 0 )
            {
                tree.goToNthChild( 0 );
                ICswNbtKioskModeOpenable PotentialOpenable = (ICswNbtKioskModeOpenable) tree.getNodeForCurrentPosition().ObjClass;
                if( PotentialOpenable.CanOpen() )
                {
                    OpData.Field1.NodeIdStr = tree.getNodeIdForCurrentPosition().ToString();
                    ret = true;
                }
                else
                {
                    OpData.Field1.StatusMsg = "Cannot open Container with barcode " + OpData.Field1.Value + " because it does not have an Expiration date or the material does not have an expiration interval.";
                    OpData.Field1.ServerValidated = false;
                    OpData.Log.Add( DateTime.Now + " - ERROR: " + OpData.Field1.StatusMsg );
                }
            }
            else
            {
                OpData.Field1.StatusMsg = "Could not find a Container with barcode " + OpData.Field1.Value;
                OpData.Field1.ServerValidated = false;
                OpData.Log.Add( DateTime.Now + " - ERROR: " + OpData.Field1.StatusMsg );
            }

            return ret;
        }

        #endregion
    }
}
