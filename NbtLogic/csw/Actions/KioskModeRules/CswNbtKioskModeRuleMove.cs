using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Search;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Actions.KioskMode
{
    public class CswNbtKioskModeRuleMove: CswNbtKioskModeRule
    {
        public CswNbtResources _CswNbtResources;

        public CswNbtKioskModeRuleMove( CswNbtResources NbtResources )
            : base( NbtResources )
        {
            _CswNbtResources = NbtResources;
        }

        public override void SetFields( ref OperationData OpData )
        {
            base.SetFields( ref OpData );
            OpData.Field1.Name = "Location:";
            OpData.Field2.Name = "Item:";
        }

        public override void ValidateFieldOne( ref OperationData OpData )
        {
            bool IsValid = _validateLocation( ref OpData );
            if( IsValid )
            {
                base.ValidateFieldOne( ref OpData );
            }
        }

        public override void ValidateFieldTwo( ref OperationData OpData )
        {
            bool IsValid = _validateItem( ref OpData );
            if( IsValid )
            {
                base.ValidateFieldTwo( ref OpData );
            }
        }

        public override void CommitOperation( ref OperationData OpData )
        {
            CswNbtNode itemToMove = _getNodeByBarcode( OpData.Field2.FoundObjClass, OpData.Field2.Value, true );
            string locationPropName = "Location";
            switch( OpData.Field2.FoundObjClass )
            {
                case NbtObjectClass.EquipmentClass:
                    locationPropName = CswNbtObjClassEquipment.PropertyName.Location;
                    break;
                case NbtObjectClass.EquipmentAssemblyClass:
                    locationPropName = CswNbtObjClassEquipmentAssembly.PropertyName.Location;
                    break;
                case NbtObjectClass.ContainerClass:
                    locationPropName = CswNbtObjClassContainer.PropertyName.Location;
                    break;
            }
            string itemType = itemToMove.getNodeType().NodeTypeName;

            if( _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Edit, itemToMove.getNodeType() ) && false == itemToMove.Properties[locationPropName].ReadOnly )
            {
                CswNbtObjClassLocation locationToMoveTo = _getNodeByBarcode( NbtObjectClass.LocationClass, OpData.Field1.Value, true );
                itemToMove.Properties[locationPropName].AsLocation.SelectedNodeId = locationToMoveTo.NodeId;
                itemToMove.postChanges( false );
                OpData.Log.Add( DateTime.Now + " - Moved " + itemType + " " + OpData.Field2.Value + " to " + locationToMoveTo.Name.Text + " (" + OpData.Field1.Value + ")" );
                base.CommitOperation( ref OpData );
            }
            else
            {
                string statusMsg = "You do not have permission to edit " + itemType + " (" + OpData.Field2.Value + ")";
                OpData.Field2.StatusMsg = statusMsg;
                OpData.Field2.ServerValidated = false;
                OpData.Log.Add( DateTime.Now + " - ERROR: " + statusMsg );
            }
        }

        #region Private Functions

        private bool _validateLocation( ref OperationData OpData )
        {
            bool ret = false;
            ICswNbtTree tree = _getTree( NbtObjectClass.LocationClass, OpData.Field1.Value, true );
            if( tree.getChildNodeCount() > 0 )
            {
                ret = true;
            }
            else
            {
                OpData.Field1.StatusMsg = "Could not find a Location with barcode " + OpData.Field1.Value;
                OpData.Field1.ServerValidated = false;
                OpData.Log.Add( DateTime.Now + " - ERROR: " + OpData.Field1.StatusMsg );
            }
            return ret;
        }

        private bool _validateItem( ref OperationData OpData )
        {
            bool ret = false;

            CswNbtSearch search = new CswNbtSearch( _CswNbtResources )
            {
                SearchTerm = OpData.Field2.Value
            };
            ICswNbtTree tree = search.Results();

            int childCount = tree.getChildNodeCount();
            for( int i = 0; i < childCount; i++ )
            {
                tree.goToNthChild( i );
                CswNbtNode node = tree.getNodeForCurrentPosition();
                string ObjClass = node.ObjClass.ObjectClass.ObjectClass;

                if( ObjClass == NbtObjectClass.EquipmentAssemblyClass && null == OpData.Field2.FoundObjClass )
                {
                    OpData.Field2.FoundObjClass = NbtObjectClass.EquipmentAssemblyClass;
                    ret = true;
                }

                if( ObjClass == NbtObjectClass.EquipmentClass && null == OpData.Field2.FoundObjClass )
                {
                    OpData.Field2.FoundObjClass = NbtObjectClass.EquipmentClass;
                    ret = true;
                }

                if( ObjClass == NbtObjectClass.ContainerClass && null == OpData.Field2.FoundObjClass )
                {
                    OpData.Field2.FoundObjClass = NbtObjectClass.ContainerClass;
                    ret = true;
                }

                tree.goToParentNode();
            }

            if( null == OpData.Field2.FoundObjClass )
            {
                string StatusMsg = "";
                bool first = true;
                if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Containers ) )
                {
                    StatusMsg = NbtObjectClass.ContainerClass.Replace( "Class", "" );
                    first = false;
                }
                if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.IMCS ) )
                {
                    if( first )
                    {
                        StatusMsg = NbtObjectClass.EquipmentClass.Replace( "Class", "" );
                    }
                    else
                    {
                        StatusMsg += ", " + NbtObjectClass.EquipmentClass.Replace( "Class", "" );
                    }
                    StatusMsg += " or " + NbtObjectClass.EquipmentAssemblyClass.Replace( "Class", "" );
                }
                StatusMsg = "Could not find " + StatusMsg + " with barcode " + OpData.Field2.Value;

                OpData.Field2.StatusMsg = StatusMsg;
                OpData.Field2.ServerValidated = false;
                OpData.Log.Add( DateTime.Now + " - ERROR: " + StatusMsg );
            }

            return ret;
        }

        #endregion
    }
}
