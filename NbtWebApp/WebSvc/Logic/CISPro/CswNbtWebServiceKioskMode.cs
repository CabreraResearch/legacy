using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using NbtWebApp.WebSvc.Returns;
using ChemSW.Nbt.MetaData;
using System.Collections.ObjectModel;
using ChemSW.Nbt.Search;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceKioskMode
    {
        private static CswNbtResources _CswNbtResources;

        public CswNbtWebServiceKioskMode( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        private sealed class Modes
        {
            public const string Move = "move";
            public const string Owner = "owner";
            public const string Transfer = "transfer";
            public const string Dispense = "dispense";
            public const string Dispose = "dispose";
            public const string Status = "status";
            public static readonly CswCommaDelimitedString All = new CswCommaDelimitedString()
            {
                Move, Owner, Transfer, Dispense, Dispose, Status
            };
        }

        #region Data Contracts

        [DataContract]
        public class KioskModeDataReturn : CswWebSvcReturn
        {
            public KioskModeDataReturn()
            {
                Data = new KioskModeData();
            }
            [DataMember]
            public KioskModeData Data;
        }

        [DataContract]
        public class KioskModeData
        {
            [DataMember]
            public Collection<Mode> AvailableModes = new Collection<Mode>();
            [DataMember]
            public OperationData OperationData;
        }

        [DataContract]
        public class Mode
        {
            [DataMember]
            public string name = string.Empty;
            [DataMember]
            public string imgUrl = string.Empty;
        }

        [DataContract]
        public class ActiveMode
        {
            [DataMember]
            public string field1Name = string.Empty;
            [DataMember]
            public string field2Name = string.Empty;
        }

        [DataContract]
        public class OperationData
        {
            [DataMember]
            public string Mode = string.Empty;
            [DataMember]
            public string ModeStatusMsg = string.Empty;
            [DataMember]
            public bool ModeServerValidated = false;
            [DataMember]
            public Collection<string> Log = new Collection<string>();
            [DataMember]
            public Field Field1;
            [DataMember]
            public Field Field2;
            [DataMember]
            public string LastItemScanned;
            [DataMember]
            public string ScanTextLabel;
        }

        [DataContract]
        public class Field
        {
            [DataMember]
            public string Name = string.Empty;
            [DataMember]
            public string Value = string.Empty;
            [DataMember]
            public string StatusMsg = string.Empty;
            [DataMember]
            public bool ServerValidated = false;
            [DataMember]
            public string SecondValue = string.Empty;
            [DataMember]
            public string FoundObjClass;

            public NbtObjectClass GetBarcodeExpectedOC( CswNbtResources NbtResources, ref OperationData OpData )
            {
                NbtObjectClass Ret = null;

                CswNbtWebServiceSearch searchSvc = new CswNbtWebServiceSearch( NbtResources, null );
                CswNbtSearch search = searchSvc.getSearch( Value, Int32.MinValue, Int32.MinValue );
                ICswNbtTree tree = search.Results();
                int childCount = tree.getChildNodeCount();
                for( int i = 0; i < childCount; i++ )
                {
                    tree.goToNthChild( i );
                    CswNbtNode node = tree.getNodeForCurrentPosition();
                    string ObjClass = node.ObjClass.ObjectClass.ObjectClass;

                    if( NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Containers ) )
                    {
                        if( ObjClass == NbtObjectClass.ContainerClass )
                        {
                            Ret = NbtObjectClass.ContainerClass;
                            FoundObjClass = Ret;
                        }
                    }

                    if( NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.IMCS ) )
                    {
                        if( ObjClass == NbtObjectClass.EquipmentAssemblyClass )
                        {
                            Ret = NbtObjectClass.EquipmentAssemblyClass;
                            FoundObjClass = Ret;
                        }

                        if( ObjClass == NbtObjectClass.EquipmentClass )
                        {
                            Ret = NbtObjectClass.EquipmentClass;
                            FoundObjClass = Ret;
                        }
                    }
                    tree.goToParentNode();
                }

                if( null == Ret ) //Now is a good time to validate the field since we know what we just searched for
                {
                    bool first = true;
                    if( NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Containers ) )
                    {
                        StatusMsg = NbtObjectClass.ContainerClass.Replace( "Class", "" );
                        first = false;
                    }
                    if( NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.IMCS ) )
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
                    StatusMsg = "Could not find " + StatusMsg;
                    OpData.Log.Add( DateTime.Now + " - ERROR: " + StatusMsg );
                }

                return Ret;
            }

            public NbtObjectClass ExpectedObjClass( CswNbtResources NbtResources, int FieldNumber, ref OperationData OpData )
            {
                NbtObjectClass Ret = null;
                string OpMode = OpData.Mode.ToLower();
                switch( OpMode )
                {
                    case Modes.Move:
                        if( FieldNumber == 1 )
                        {
                            Ret = NbtObjectClass.LocationClass;
                        }
                        else
                        {
                            Ret = GetBarcodeExpectedOC( NbtResources, ref OpData );
                        }
                        break;
                    case Modes.Owner:
                        if( FieldNumber == 1 )
                        {
                            Ret = NbtObjectClass.UserClass;
                        }
                        else
                        {
                            Ret = NbtObjectClass.ContainerClass;
                        }
                        break;
                    case Modes.Transfer:
                        if( FieldNumber == 1 )
                        {
                            Ret = NbtObjectClass.UserClass;
                        }
                        else
                        {
                            Ret = NbtObjectClass.ContainerClass;
                        }
                        break;
                    case Modes.Dispense:
                        if( FieldNumber == 1 )
                        {
                            Ret = NbtObjectClass.ContainerClass;
                        }
                        break;
                    case Modes.Dispose:
                        Ret = NbtObjectClass.ContainerClass;
                        break;
                    case Modes.Status:
                        if( FieldNumber == 2 )
                        {
                            Ret = GetBarcodeExpectedOC( NbtResources, ref OpData );
                        }
                        break;
                }
                return Ret;
            }
        }

        #endregion

        public static void GetAvailableModes( ICswResources CswResources, KioskModeDataReturn Return, KioskModeData KioskModeData )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswNbtObjClassRole currentUserRoleNode = NbtResources.Nodes.makeRoleNodeFromRoleName( NbtResources.CurrentNbtUser.Rolename );

            KioskModeData kioskModeData = new KioskModeData();

            kioskModeData.AvailableModes.Add( new Mode
            {
                name = CswTools.UppercaseFirst( Modes.Move ),
                imgUrl = "Images/newicons/KioskMode/Move_code39.png"
            } );

            if( NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Containers ) )
            {
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswTools.UppercaseFirst( Modes.Owner ),
                    imgUrl = "Images/newicons/KioskMode/Owner_code39.png"
                } );
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswTools.UppercaseFirst( Modes.Transfer ),
                    imgUrl = "Images/newicons/KioskMode/Transfer_code39.png"
                } );
                CswNbtPermit permissions = new CswNbtPermit( NbtResources );
                if( permissions.can( CswNbtActionName.DispenseContainer ) )
                {
                    kioskModeData.AvailableModes.Add( new Mode
                    {
                        name = CswTools.UppercaseFirst( Modes.Dispense ),
                        imgUrl = "Images/newicons/KioskMode/Dispense_code39.png"
                    } );
                }
                if( permissions.can( CswNbtActionName.DisposeContainer ) )
                {
                    kioskModeData.AvailableModes.Add( new Mode
                    {
                        name = CswTools.UppercaseFirst( Modes.Dispose ),
                        imgUrl = "Images/newicons/KioskMode/Dispose_code39.png"
                    } );
                }
            }

            if( NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.IMCS ) )
            {
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswTools.UppercaseFirst( Modes.Status ),
                    imgUrl = "Images/newicons/KioskMode/Status_code39.png"
                } );
            }

            kioskModeData.AvailableModes.Add( new Mode
            {
                name = "Reset",
                imgUrl = "Images/newicons/KioskMode/Reset_code39.png"
            } );

            Return.Data = kioskModeData;
        }

        public static void HandleScan( ICswResources CswResources, KioskModeDataReturn Return, KioskModeData KioskModeData )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            if( _isModeScan( KioskModeData.OperationData.LastItemScanned ) )
            {
                KioskModeData.OperationData.Mode = KioskModeData.OperationData.LastItemScanned;
                _setFields( KioskModeData.OperationData );
            }
            else
            {
                if( false == string.IsNullOrEmpty( KioskModeData.OperationData.Field2.Value ) && false == KioskModeData.OperationData.Field2.ServerValidated )
                {
                    NbtObjectClass expectedOC = KioskModeData.OperationData.Field2.ExpectedObjClass( NbtResources, 2, ref KioskModeData.OperationData );
                    KioskModeData.OperationData.Field2 = _validateField( NbtResources, expectedOC, KioskModeData.OperationData.Field2, ref KioskModeData.OperationData );
                }
                else if( false == string.IsNullOrEmpty( KioskModeData.OperationData.Field1.Value ) && false == KioskModeData.OperationData.Field1.ServerValidated )
                {
                    NbtObjectClass expectedOC = KioskModeData.OperationData.Field1.ExpectedObjClass( NbtResources, 1, ref KioskModeData.OperationData );
                    KioskModeData.OperationData.Field1 = _validateField( NbtResources, expectedOC, KioskModeData.OperationData.Field1, ref KioskModeData.OperationData );
                }
                else
                {
                    KioskModeData.OperationData.ModeStatusMsg = "Error: Scanned mode does not exist or is unavailable";
                    KioskModeData.OperationData.ModeServerValidated = false;
                }
            }
            Return.Data = KioskModeData;
        }

        public static void CommitOperation( ICswResources CswResources, KioskModeDataReturn Return, KioskModeData KioskModeData )
        {
            bool resetField2 = true;
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            OperationData OpData = KioskModeData.OperationData;
            string mode = OpData.Mode.ToLower();
            switch( mode )
            {
                case Modes.Move:
                    CswNbtNode itemToMove = _getNodeByBarcode( NbtResources, OpData.Field2.FoundObjClass, OpData.Field2.Value, true );
                    CswNbtObjClassLocation locationToMoveTo = _getNodeByBarcode( NbtResources, NbtObjectClass.LocationClass, OpData.Field1.Value, true );

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
                    itemToMove.Properties[locationPropName].AsLocation.SelectedNodeId = locationToMoveTo.NodeId;
                    itemToMove.postChanges( false );

                    OpData.Log.Add( DateTime.Now + " - Moved " + OpData.Field2.FoundObjClass.Replace( "Class", "" ) + " " + OpData.Field2.Value + " to " + locationToMoveTo.Name.Text + " (" + OpData.Field1.Value + ")" );
                    break;
                case Modes.Owner:
                    CswNbtObjClassContainer containerNode = _getNodeByBarcode( NbtResources, NbtObjectClass.ContainerClass, OpData.Field2.Value, true );
                    CswNbtObjClassUser newOwnerNode = _getNodeByBarcode( NbtResources, NbtObjectClass.UserClass, OpData.Field1.Value, true );
                    containerNode.Owner.RelatedNodeId = newOwnerNode.NodeId;
                    containerNode.Owner.RefreshNodeName();
                    containerNode.postChanges( false );
                    OpData.Log.Add( DateTime.Now + " - Changed owner of container " + OpData.Field2.Value + " to " + newOwnerNode.FirstName + " " + newOwnerNode.LastName + " (" + OpData.Field1.Value + ")" );
                    break;
                case Modes.Transfer:
                    CswNbtObjClassContainer containerToTransfer = _getNodeByBarcode( NbtResources, NbtObjectClass.ContainerClass, OpData.Field2.Value, true );
                    CswNbtObjClassUser newTransferOwner = _getNodeByBarcode( NbtResources, NbtObjectClass.UserClass, OpData.Field1.Value, true );
                    containerToTransfer.Owner.RelatedNodeId = newTransferOwner.NodeId;
                    containerToTransfer.Owner.RefreshNodeName();
                    containerToTransfer.MoveContainer( newTransferOwner.DefaultLocationId );
                    containerToTransfer.postChanges( false );
                    CswNbtObjClassLocation newLocationNode = NbtResources.Nodes[newTransferOwner.DefaultLocationId];
                    OpData.Log.Add( DateTime.Now + " - Transferred container " + OpData.Field2.Value + " ownership to " + newTransferOwner.FirstName + " " + newTransferOwner.LastName + " (" + OpData.Field1.Value + ") at " + newLocationNode.Name.Text );
                    break;
                case Modes.Dispense:
                    CswNbtObjClassContainer containerToDispense = _getNodeByBarcode( NbtResources, NbtObjectClass.ContainerClass, OpData.Field1.Value, false );
                    double quantityToDispense = CswConvert.ToDouble( OpData.Field2.Value );
                    if( quantityToDispense > containerToDispense.Quantity.Quantity )
                    {
                        OpData.Field2.StatusMsg = "Cannot dispense " + quantityToDispense + containerToDispense.Quantity.CachedUnitName + " when containter only has " + containerToDispense.Quantity.Gestalt;
                        OpData.Log.Add( DateTime.Now + " - ERROR: Attempted to dispense " + quantityToDispense + containerToDispense.Quantity.CachedUnitName + " when containter only has " + containerToDispense.Quantity.Gestalt );
                        OpData.Field2.ServerValidated = false;
                        resetField2 = false;
                    }
                    else
                    {
                        containerToDispense.DispenseOut( CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispense, quantityToDispense, containerToDispense.Quantity.UnitId );
                        containerToDispense.postChanges( false );
                        OpData.Field1.SecondValue = " (current quantity: " + containerToDispense.Quantity.Quantity + containerToDispense.Quantity.CachedUnitName + ")";
                        OpData.Log.Add( DateTime.Now + " - Dispensed " + OpData.Field2.Value + " " + containerToDispense.Quantity.CachedUnitName + " out of container " + containerToDispense.Barcode.Barcode + ". " + containerToDispense.Quantity.Gestalt + " left in container" );
                    }
                    break;
                case Modes.Dispose:
                    CswNbtObjClassContainer containerToDispose = _getNodeByBarcode( NbtResources, NbtObjectClass.ContainerClass, OpData.Field1.Value, false );
                    if( Tristate.True == containerToDispose.Disposed.Checked )
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
                    OpData.Field1.SecondValue = string.Empty;
                    OpData.Field1.ServerValidated = false;
                    resetField2 = false;
                    break;
                case Modes.Status:

                    CswNbtNode item = _getNodeByBarcode( NbtResources, OpData.Field2.FoundObjClass, OpData.Field2.Value, false );

                    string statusPropName = "Status";
                    switch( OpData.Field2.FoundObjClass )
                    {
                        case NbtObjectClass.EquipmentClass:
                            locationPropName = CswNbtObjClassEquipment.PropertyName.Status;
                            break;
                        case NbtObjectClass.EquipmentAssemblyClass:
                            locationPropName = CswNbtObjClassEquipmentAssembly.PropertyName.Status;
                            break;
                    }

                    item.Properties[statusPropName].AsList.Value = OpData.Field1.Value;
                    item.postChanges( false );

                    OpData.Log.Add( DateTime.Now + " - Status of " + OpData.Field2.FoundObjClass.Replace( "Class", "" ) + " " + OpData.Field2.Value + " changed to \"" + OpData.Field1.Value + "\"" );
                    resetField2 = true;
                    break;
            }
            if( resetField2 )
            {
                OpData.Field2.Value = string.Empty;
                OpData.Field2.SecondValue = string.Empty;
                OpData.Field2.ServerValidated = false;
            }
            KioskModeData.OperationData = OpData;
            Return.Data = KioskModeData;
        }

        #region Private Methods

        private static void _setFields( OperationData OpData )
        {
            OpData.ModeServerValidated = true;
            OpData.ModeStatusMsg = string.Empty;
            OpData.Field1 = new Field();
            OpData.Field2 = new Field();
            OpData.ScanTextLabel = "Scan a barcode:";
            string selectedOp = OpData.Mode.ToLower();
            switch( selectedOp )
            {
                case Modes.Move:
                    OpData.Field1.Name = "Location:";
                    OpData.Field2.Name = "Item:";
                    break;
                case Modes.Owner:
                    OpData.Field1.Name = "User:";
                    OpData.Field2.Name = "Item:";
                    break;
                case Modes.Transfer:
                    OpData.Field1.Name = "User:";
                    OpData.Field2.Name = "Item:";
                    break;
                case Modes.Dispense:
                    OpData.Field1.Name = "Container:";
                    OpData.Field2.Name = "Quantity:";
                    break;
                case Modes.Dispose:
                    OpData.Field1.Name = "Container:";
                    OpData.Field2.ServerValidated = true;
                    break;
                case Modes.Status:
                    OpData.Field1.Name = "Status:";
                    OpData.Field2.Name = "Equipment:";
                    break;
                default:
                    OpData.ModeStatusMsg = "Error: Scanned mode does not exist or is unavailable";
                    OpData.ModeServerValidated = false;
                    break;
            }
        }

        private static Field _validateField( CswNbtResources NbtResources, NbtObjectClass ObjClass, Field Field, ref OperationData OpData )
        {
            bool IsValid = false;
            string loweredMode = OpData.Mode.ToLower();

            if( null != ObjClass )
            {
                ICswNbtTree tree = _getTree( NbtResources, ObjClass, Field.Value, false );
                if( null != tree )
                {
                    int childCount = tree.getChildNodeCount();
                    if( childCount > 0 )
                    {
                        if( false == loweredMode.Equals( Modes.Dispose ) && ObjClass == NbtObjectClass.ContainerClass )
                        {
                            IsValid = _validateContainer( loweredMode, tree, Field, ObjClass, ref OpData );
                        }
                        else
                        {
                            IsValid = true;
                        }
                    }
                    else
                    {
                        Field.StatusMsg = "Could not find " + ObjClass.ToString().Replace( "Class", "" );
                        OpData.Log.Add( DateTime.Now + " - ERROR: " + Field.StatusMsg );
                    }
                }
            }
            else
            {
                if( loweredMode.Equals( Modes.Dispense ) )
                {
                    if( CswTools.IsDouble( Field.Value ) )
                    {
                        IsValid = true;
                    }
                    else
                    {
                        Field.StatusMsg = "Error: " + Field.Value + " is not a number";
                    }
                }

                if( loweredMode.Equals( Modes.Status ) )
                {
                    if( _matchingStatusFound( OpData.Field1.Value ) )
                    {
                        IsValid = true;
                    }
                    else
                    {
                        Field.StatusMsg = "Could not find a matching status of: " + OpData.Field1.Value;
                    }
                }
            }

            if( IsValid )
            {
                Field.ServerValidated = true;
                Field.StatusMsg = "";
                if( NbtObjectClass.LocationClass.Equals( ObjClass ) )
                {
                    CswNbtObjClassLocation scannedLocation = _getNodeByBarcode( NbtResources, NbtObjectClass.LocationClass, Field.Value, true );
                    Field.SecondValue = " (" + scannedLocation.Name.Text + ")";
                }
            }

            return Field;
        }

        private static bool _validateContainer( string loweredMode, ICswNbtTree tree, Field Field, NbtObjectClass ObjClass, ref OperationData OpData )
        {
            bool IsValid = true;
            tree.goToNthChild( 0 );
            foreach( CswNbtTreeNodeProp treeNodeProp in tree.getChildNodePropsOfNode() )
            {
                if( treeNodeProp.PropName.Equals( CswNbtObjClassContainer.PropertyName.Disposed ) )
                {
                    bool disposed = CswConvert.ToBoolean( treeNodeProp.Field1 );
                    if( disposed )
                    {
                        Field.StatusMsg = "Cannot perform " + OpData.Mode + " operation on disposed " + ObjClass.Value.Replace( "Class", "" ) + " " + Field.Value;
                        OpData.Log.Add( DateTime.Now + " - ERROR: " + Field.StatusMsg );
                    }
                    IsValid = ( false == disposed );
                }
                else if( treeNodeProp.PropName.Equals( CswNbtObjClassContainer.PropertyName.Quantity ) )
                {
                    if( loweredMode.Equals( Modes.Dispense ) )
                    {
                        Field.SecondValue = " (current quantity: " + treeNodeProp.Gestalt + ")";
                        OpData.ScanTextLabel = "Enter a quantity (" + treeNodeProp.Field1 + ") :";
                    }
                }
            }
            return IsValid;
        }


        private static CswNbtNode _getNodeByBarcode( CswNbtResources NbtResources, NbtObjectClass ObjClass, string Barcode, bool IncludeDefaultFilters )
        {
            CswNbtNode Ret = null;
            ICswNbtTree tree = _getTree( NbtResources, ObjClass, Barcode, IncludeDefaultFilters );
            int childCount = tree.getChildNodeCount();
            if( childCount > 0 )
            {
                tree.goToNthChild( 0 );
                Ret = tree.getNodeForCurrentPosition();
            }
            return Ret;
        }

        private static CswPrimaryKey _getNodeIdByBarcode( CswNbtResources NbtResources, NbtObjectClass ObjClass, string Barcode, bool IncludeDefaultFilters )
        {
            CswPrimaryKey Ret = null;
            ICswNbtTree tree = _getTree( NbtResources, ObjClass, Barcode, IncludeDefaultFilters );
            int childCount = tree.getChildNodeCount();
            if( childCount > 0 )
            {
                tree.goToNthChild( 0 );
                Ret = tree.getNodeIdForCurrentPosition();
            }
            return Ret;
        }

        private static ICswNbtTree _getTree( CswNbtResources NbtResources, NbtObjectClass ObjClass, string Barcode, bool IncludeDefaultFilters )
        {
            ICswNbtTree tree = null;
            CswNbtMetaDataObjectClass metaDataOC = NbtResources.MetaData.getObjectClass( ObjClass );
            CswNbtMetaDataObjectClassProp barcodeOCP = metaDataOC.getBarcodeProp();
            if( null != barcodeOCP )
            {
                CswNbtView view = new CswNbtView( NbtResources );
                CswNbtViewRelationship parent = view.AddViewRelationship( metaDataOC, IncludeDefaultFilters );
                view.AddViewPropertyAndFilter( parent,
                    MetaDataProp: barcodeOCP,
                    Value: Barcode,
                    SubFieldName: CswNbtSubField.SubFieldName.Barcode,
                    FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals
                );

                if( ObjClass.Equals( NbtObjectClass.ContainerClass ) )
                {
                    CswNbtMetaDataObjectClassProp disposedOCP = metaDataOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Disposed );
                    view.AddViewProperty( parent, disposedOCP );

                    CswNbtMetaDataObjectClassProp quantityOCP = metaDataOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Quantity );
                    view.AddViewProperty( parent, quantityOCP );
                }

                tree = NbtResources.Trees.getTreeFromView( view, true, false, false );
            }
            return tree;
        }

        private static bool _isModeScan( string ScannedMode )
        {
            bool Ret = false;
            foreach( string mode in Modes.All )
            {
                if( mode.Equals( ScannedMode.ToLower() ) )
                {
                    Ret = true;
                }
            }
            return Ret;
        }

        private static bool _matchingStatusFound( string status )
        {
            //TODO: match a status loosely to an equipment/assembly status
            return true;
        }

        #endregion
    }

}