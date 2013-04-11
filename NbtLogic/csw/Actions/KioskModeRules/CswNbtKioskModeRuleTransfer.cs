using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Actions.KioskMode
{
    public class CswNbtKioskModeRuleTransfer: CswNbtKioskModeRule
    {
        public CswNbtKioskModeRuleTransfer( CswNbtResources NbtResources )
            : base( NbtResources )
        {
            _CswNbtResources = NbtResources;
        }

        public override void SetFields( ref OperationData OpData )
        {
            base.SetFields( ref OpData );
            OpData.Field1.Name = "User:";
            OpData.Field2.Name = "Item:";
        }

        public override void ValidateFieldOne( ref OperationData OpData )
        {
            bool IsValid = _validateUser( ref OpData );
            if( IsValid )
            {
                base.ValidateFieldOne( ref OpData );
            }
        }

        public override void ValidateFieldTwo( ref OperationData OpData )
        {
            bool IsValid = _validateContainer( ref OpData );
            if( IsValid )
            {
                base.ValidateFieldTwo( ref OpData );
            }
        }

        public override void CommitOperation( ref OperationData OpData )
        {
            CswNbtObjClassContainer containerNode = _getNodeByBarcode( CswEnumNbtObjectClass.ContainerClass, OpData.Field2.Value, true );

            if( _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Edit, containerNode.NodeType ) )
            {
                CswNbtObjClassContainer containerToTransfer = _getNodeByBarcode( CswEnumNbtObjectClass.ContainerClass, OpData.Field2.Value, true );
                CswNbtObjClassUser newTransferOwner = _getNodeByBarcode( CswEnumNbtObjectClass.UserClass, OpData.Field1.Value, true );
                containerToTransfer.TransferContainer( newTransferOwner );
                containerToTransfer.postChanges( false );
                CswNbtObjClassLocation newLocationNode = _CswNbtResources.Nodes[newTransferOwner.DefaultLocationId];
                OpData.Log.Add( DateTime.Now + " - Transferred container " + OpData.Field2.Value + " ownership to " + newTransferOwner.Username + " (" + OpData.Field1.Value + ") at " + newLocationNode.Name.Text );
                base.CommitOperation( ref OpData );
            }
            else
            {
                string statusMsg = "You do not have permission to edit Container (" + OpData.Field2.Value + ")";
                OpData.Field2.StatusMsg = statusMsg;
                OpData.Field2.ServerValidated = false;
                OpData.Log.Add( DateTime.Now + " - ERROR: " + statusMsg );
            }
        }

        #region Private Functions

        private bool _validateUser( ref OperationData OpData )
        {
            bool ret = false;
            ICswNbtTree tree = _getTree( CswEnumNbtObjectClass.UserClass, OpData.Field1.Value, true );
            if( tree.getChildNodeCount() > 0 )
            {
                ret = true;
            }
            else
            {
                OpData.Field1.StatusMsg = "Could not find a User with barcode " + OpData.Field1.Value;
                OpData.Field1.ServerValidated = false;
                OpData.Log.Add( DateTime.Now + " - ERROR: " + OpData.Field1.StatusMsg );
            }
            return ret;
        }

        private bool _validateContainer( ref OperationData OpData )
        {
            bool ret = false;

            ICswNbtTree tree = _getTree( CswEnumNbtObjectClass.ContainerClass, OpData.Field2.Value, true );
            if( tree.getChildNodeCount() > 0 )
            {
                ret = true;
            }
            else
            {
                OpData.Field2.StatusMsg = "Could not find a Container with barcode " + OpData.Field2.Value;
                OpData.Field2.ServerValidated = false;
                OpData.Log.Add( DateTime.Now + " - ERROR: " + OpData.Field2.StatusMsg );
            }

            return ret;
        }

        #endregion
    }
}
