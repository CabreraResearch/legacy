using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Actions.KioskMode
{
    public class CswNbtKioskModeRuleDispense: CswNbtKioskModeRule
    {
        public CswNbtKioskModeRuleDispense( CswNbtResources NbtResources )
            : base( NbtResources )
        {
            _CswNbtResources = NbtResources;
        }

        public override void SetFields( ref OperationData OpData )
        {
            base.SetFields( ref OpData );
            OpData.Field1.Name = "Container:";
            OpData.Field2.Name = "Quantity:";
        }

        public override void ValidateFieldOne( ref OperationData OpData )
        {
            bool IsValid = _validateContainer( ref OpData );
            if( IsValid )
            {
                base.ValidateFieldOne( ref OpData );
            }
        }

        public override void ValidateFieldTwo( ref OperationData OpData )
        {
            bool IsValid = _validateDispenseAmmount( ref OpData );
            if( IsValid )
            {
                base.ValidateFieldTwo( ref OpData );
            }
        }

        public override void CommitOperation( ref OperationData OpData )
        {
            CswNbtObjClassContainer containerToDispense = _getNodeByBarcode( CswEnumNbtObjectClass.ContainerClass, OpData.Field1.Value, false );
            if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Edit, containerToDispense.NodeType ) && _CswNbtResources.Permit.can( CswEnumNbtActionName.DispenseContainer ) )
            {
                double quantityToDispense = CswConvert.ToDouble( OpData.Field2.Value );
                if( quantityToDispense > containerToDispense.Quantity.Quantity )
                {
                    OpData.Field2.StatusMsg = "Cannot dispense " + quantityToDispense + containerToDispense.Quantity.CachedUnitName + " when containter only has " + containerToDispense.Quantity.Gestalt;
                    OpData.Log.Add( DateTime.Now + " - ERROR: Attempted to dispense " + quantityToDispense + containerToDispense.Quantity.CachedUnitName + " when containter only has " + containerToDispense.Quantity.Gestalt );
                    OpData.Field2.ServerValidated = false;
                }
                else
                {
                    containerToDispense.DispenseOut( CswEnumNbtContainerDispenseType.Dispense, quantityToDispense, containerToDispense.Quantity.UnitId );
                    containerToDispense.postChanges( false );
                    OpData.Field1.SecondValue = " (current quantity: " + containerToDispense.Quantity.Quantity + containerToDispense.Quantity.CachedUnitName + ")";
                    OpData.Log.Add( DateTime.Now + " - Dispensed " + OpData.Field2.Value + " " + containerToDispense.Quantity.CachedUnitName + " out of container " + containerToDispense.Barcode.Barcode + ". " + containerToDispense.Quantity.Gestalt + " left in container" );
                    base.CommitOperation( ref OpData );
                }
            }
            else
            {
                string statusMsg = "You do not have permission to dispense Container (" + OpData.Field2.Value + ")";
                OpData.Field2.StatusMsg = statusMsg;
                OpData.Field2.ServerValidated = false;
                OpData.Log.Add( DateTime.Now + " - ERROR: " + statusMsg );
            }
        }

        #region Private Functions

        private bool _validateDispenseAmmount( ref OperationData OpData )
        {
            bool ret = false;
            if( CswTools.IsDouble( OpData.Field2.Value ) )
            {
                ret = true;
            }
            else
            {
                OpData.Field2.StatusMsg = "Error: " + OpData.Field2.Value + " is not a number";
            }
            return ret;
        }

        private bool _validateContainer( ref OperationData OpData )
        {
            bool ret = false;

            ICswNbtTree tree = _getTree( CswEnumNbtObjectClass.ContainerClass, OpData.Field1.Value, true );
            if( tree.getChildNodeCount() > 0 )
            {
                tree.goToNthChild( 0 );
                foreach( CswNbtTreeNodeProp treeNodeProp in tree.getChildNodePropsOfNode() )
                {
                    if( treeNodeProp.PropName.Equals( CswNbtObjClassContainer.PropertyName.Disposed ) )
                    {
                        bool disposed = CswConvert.ToBoolean( treeNodeProp.Field1 );
                        if( disposed )
                        {
                            OpData.Field1.StatusMsg = "Cannot perform " + OpData.Mode + " operation on disposed Container " + OpData.Field1.Value;
                            OpData.Log.Add( DateTime.Now + " - ERROR: " + OpData.Field1.StatusMsg );
                        }
                        ret = ( false == disposed );
                    }
                    else if( treeNodeProp.PropName.Equals( CswNbtObjClassContainer.PropertyName.Quantity ) )
                    {
                        OpData.Field1.SecondValue = " (current quantity: " + treeNodeProp.Gestalt + ")";
                        OpData.ScanTextLabel = "Enter a quantity (" + treeNodeProp.Field1 + ") :";
                    }
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
