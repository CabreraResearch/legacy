using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Actions.KioskMode
{
    public class CswNbtKioskModeRuleDispose: CswNbtKioskModeRule
    {
        public CswNbtKioskModeRuleDispose( CswNbtResources NbtResources )
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
            //Intentionally do nothing here - Dispense mode only has one field to work with
        }

        public override void CommitOperation( ref OperationData OpData )
        {
            CswNbtObjClassContainer containerToDispose = _getNodeByBarcode( CswEnumNbtObjectClass.ContainerClass, OpData.Field1.Value, false );
            if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Edit, containerToDispose.NodeType ) && _CswNbtResources.Permit.can( CswEnumNbtActionName.DisposeContainer ) )
            {
                if( CswEnumTristate.True == containerToDispose.Disposed.Checked )
                {
                    OpData.Field1.StatusMsg = "Container " + OpData.Field1.Value + " is already disposed";
                    OpData.Log.Add( DateTime.Now + " - ERROR: Attempted to dispose already disposed container " + OpData.Field1.Value );
                }
                else
                {
                    containerToDispose.DisposeContainer();
                    containerToDispose.postChanges( false );
                    OpData.Log.Add( DateTime.Now + " - Disposed container " + OpData.Field1.Value );
                    OpData.Field1.Value = string.Empty;
                }
                OpData.Field1.ServerValidated = false;
            }
            else
            {
                string statusMsg = "You do not have permission to dispose Container (" + OpData.Field1.Value + ")";
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
                ret = true;
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
