using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions.KioskModeRules.OperationClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Search;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Actions.KioskMode
{
    public class CswNbtKioskModeRuleMove : CswNbtKioskModeRule
    {
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
            CswNbtNode itemToMove = _CswNbtResources.Nodes[OpData.Field2.NodeId];
            CswNbtMetaDataNodeType thisNT = itemToMove.getNodeType();
            string itemType = thisNT.NodeTypeName;

            if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Edit, itemToMove.getNodeType() ) && false == thisNT.getLocationProperty().ReadOnly )
            {
                ICswNbtKioskModeMoveable AsMoveable = (ICswNbtKioskModeMoveable) itemToMove.ObjClass;
                string specificStatusError = string.Empty;
                if( AsMoveable.CanMove( out specificStatusError ) )
                {
                    CswNbtObjClassLocation locationToMoveTo = _CswNbtResources.Nodes[OpData.Field1.NodeId];
                    AsMoveable.Move( locationToMoveTo );
                    itemToMove.postChanges( false );
                    OpData.Log.Add( DateTime.Now + " - Moved " + itemType + " " + OpData.Field2.Value + " to " + locationToMoveTo.Location.Gestalt + " > " + locationToMoveTo.Name.Text + " (" + OpData.Field1.Value + ")" );
                    base.CommitOperation( ref OpData );
                }
                else
                {
                    OpData.Field2.FoundObjClass = string.Empty;
                    OpData.Field2.StatusMsg = specificStatusError;
                    OpData.Field2.ServerValidated = false;
                    OpData.Log.Add( DateTime.Now + " - ERROR: " + specificStatusError );
                }
            }
            else
            {
                string statusMsg = "You do not have permission to edit " + itemType + " (" + OpData.Field2.Value + ")";
                OpData.Field2.FoundObjClass = string.Empty;
                OpData.Field2.StatusMsg = statusMsg;
                OpData.Field2.ServerValidated = false;
                OpData.Log.Add( DateTime.Now + " - ERROR: " + statusMsg );
            }
        }

        #region Private Functions

        private bool _validateLocation( ref OperationData OpData )
        {
            bool ret = false;
            ICswNbtTree tree = _getTree( CswEnumNbtObjectClass.LocationClass, OpData.Field1.Value, true );
            if( tree.getChildNodeCount() > 0 )
            {
                tree.goToNthChild( 0 );
                CswNbtMetaDataObjectClassProp propLoc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass ).getObjectClassProp( CswNbtObjClassLocation.PropertyName.Location );
                CswNbtObjClassLocation anode = tree.getCurrentNode();
                string containingLocation = string.IsNullOrEmpty( anode.Location.Gestalt ) ? "" : anode.Location.Gestalt + " > ";
                OpData.Field1.SecondValue = "(" + containingLocation + tree.getNodeNameForCurrentPosition() + ")";
                OpData.Field1.NodeIdStr = tree.getNodeIdForCurrentPosition().ToString();
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
                SearchTerm = OpData.Field2.Value,
                SearchType = CswEnumSqlLikeMode.Exact
            };
            ICswNbtTree tree = search.Results();

            int childCount = tree.getChildNodeCount();
            for( int i = 0; i < childCount; i++ )
            {
                tree.goToNthChild( i );
                CswNbtNode node = tree.getNodeForCurrentPosition();
                CswNbtMetaDataNodeType nodeType = node.getNodeType();
                CswNbtMetaDataNodeTypeProp barcodeProp = (CswNbtMetaDataNodeTypeProp) nodeType.getBarcodeProperty();

                if( null != barcodeProp )
                {
                    string barcodeValue = node.Properties[barcodeProp].AsBarcode.Barcode;
                    string ObjClass = node.ObjClass.ObjectClass.ObjectClass;

                    if( string.Equals( barcodeValue, OpData.Field2.Value, StringComparison.CurrentCultureIgnoreCase ) )
                    {
                        if( ObjClass == CswEnumNbtObjectClass.EquipmentAssemblyClass )
                        {
                            OpData.Field2.FoundObjClass = CswEnumNbtObjectClass.EquipmentAssemblyClass;
                            OpData.Field2.NodeIdStr = tree.getNodeIdForCurrentPosition().ToString();
                            ret = true;
                        }

                        if( ObjClass == CswEnumNbtObjectClass.EquipmentClass )
                        {
                            OpData.Field2.FoundObjClass = CswEnumNbtObjectClass.EquipmentClass;
                            OpData.Field2.NodeIdStr = tree.getNodeIdForCurrentPosition().ToString();
                            ret = true;
                        }

                        if( ObjClass == CswEnumNbtObjectClass.ContainerClass )
                        {
                            OpData.Field2.FoundObjClass = CswEnumNbtObjectClass.ContainerClass;
                            OpData.Field2.NodeIdStr = tree.getNodeIdForCurrentPosition().ToString();
                            ret = true;
                        }
                    }
                }
                tree.goToParentNode();
            }

            if( string.IsNullOrEmpty( OpData.Field2.FoundObjClass ) )
            {
                string StatusMsg = "";
                bool first = true;
                if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
                {
                    StatusMsg = CswEnumNbtObjectClass.ContainerClass.Replace( "Class", "" );
                    first = false;
                }
                if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.IMCS ) )
                {
                    if( first )
                    {
                        StatusMsg = CswEnumNbtObjectClass.EquipmentClass.Replace( "Class", "" );
                    }
                    else
                    {
                        StatusMsg += ", " + CswEnumNbtObjectClass.EquipmentClass.Replace( "Class", "" );
                    }
                    StatusMsg += " or " + CswEnumNbtObjectClass.EquipmentAssemblyClass.Replace( "Class", "" );
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
