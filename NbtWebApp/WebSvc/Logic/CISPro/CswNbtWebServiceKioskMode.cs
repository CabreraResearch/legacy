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

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceKioskMode
    {
        private static CswNbtResources _CswNbtResources;

        public CswNbtWebServiceKioskMode( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
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

            public NbtObjectClass ExpectedObjClass( string OpMode, int FieldNumber )
            {
                NbtObjectClass Ret = NbtObjectClass.LocationClass;
                switch( OpMode )
                {
                    case "Move":
                        if( FieldNumber == 1 )
                        {
                            Ret = NbtObjectClass.LocationClass;
                        }
                        else
                        {
                            Ret = NbtObjectClass.ContainerClass;
                        }
                        break;
                    case "Owner":
                        if( FieldNumber == 1 )
                        {
                            Ret = NbtObjectClass.UserClass;
                        }
                        else
                        {
                            Ret = NbtObjectClass.ContainerClass;
                        }
                        break;
                    case "Transfer":
                        if( FieldNumber == 1 )
                        {
                            Ret = NbtObjectClass.UserClass;
                        }
                        else
                        {
                            Ret = NbtObjectClass.ContainerClass;
                        }
                        break;
                    case "DispenseContainer":
                        //TODO: dispense container
                        break;
                    case "DisposeContainer":
                        //TODO: dispose container
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
                name = "Move",
                imgUrl = "Images/newicons/KioskMode/Move.png"
            } );
            kioskModeData.AvailableModes.Add( new Mode
            {
                name = "Owner",
                imgUrl = "Images/newicons/KioskMode/Owner.png"
            } );
            kioskModeData.AvailableModes.Add( new Mode
            {
                name = "Transfer",
                imgUrl = "Images/newicons/KioskMode/Transfer.png"
            } );
            CswNbtPermit permissions = new CswNbtPermit( NbtResources );
            if( permissions.can( CswNbtActionName.DispenseContainer ) )
            {
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswNbtActionName.DispenseContainer.ToString(),
                    imgUrl = "Images/newicons/KioskMode/Dispense.png"
                } );
            }
            if( permissions.can( CswNbtActionName.DisposeContainer ) )
            {
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswNbtActionName.DisposeContainer.ToString(),
                    imgUrl = "Images/newicons/KioskMode/Dispose.png"
                } );
            }

            kioskModeData.AvailableModes.Add( new Mode
            {
                name = "Reset",
                imgUrl = "Images/newicons/KioskMode/Reset.png"
            } );

            Return.Data = kioskModeData;
        }

        public static void HandleScan( ICswResources CswResources, KioskModeDataReturn Return, KioskModeData KioskModeData )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            if( false == string.IsNullOrEmpty( KioskModeData.OperationData.Field2.Value ) && false == KioskModeData.OperationData.Field2.ServerValidated )
            {
                NbtObjectClass expectedOC = KioskModeData.OperationData.Field2.ExpectedObjClass( KioskModeData.OperationData.Mode, 2 );
                bool barcodeValid = _validateBarcode( NbtResources, expectedOC, KioskModeData.OperationData.Field2.Value );
                if( false == barcodeValid )
                {
                    KioskModeData.OperationData.Field2.StatusMsg = "Error: item does not exist";
                }
                else
                {
                    KioskModeData.OperationData.Field2.ServerValidated = true;
                    KioskModeData.OperationData.Field2.StatusMsg = "";
                }
            }
            else if( false == string.IsNullOrEmpty( KioskModeData.OperationData.Field1.Value ) && false == KioskModeData.OperationData.Field1.ServerValidated )
            {
                NbtObjectClass expectedOC = KioskModeData.OperationData.Field2.ExpectedObjClass( KioskModeData.OperationData.Mode, 1 );
                bool barcodeValid = _validateBarcode( NbtResources, expectedOC, KioskModeData.OperationData.Field1.Value );
                if( false == barcodeValid )
                {
                    KioskModeData.OperationData.Field1.StatusMsg = "Error: item does not exist";
                }
                else
                {
                    KioskModeData.OperationData.Field1.ServerValidated = true;
                    KioskModeData.OperationData.Field1.StatusMsg = "";
                    if( expectedOC.Equals( NbtObjectClass.LocationClass ) )
                    {
                        CswNbtObjClassLocation scannedLocation = _getNodeByBarcode( NbtResources, expectedOC, KioskModeData.OperationData.Field2.Value );
                        KioskModeData.OperationData.Field1.SecondValue = " (" + scannedLocation.Name.Text + ")";
                    }
                }
            }
            else if( false == string.IsNullOrEmpty( KioskModeData.OperationData.Mode ) )
            {
                _setFields( KioskModeData.OperationData );
            }
            Return.Data = KioskModeData;
        }

        public static void CommitOperation( ICswResources CswResources, KioskModeDataReturn Return, KioskModeData KioskModeData )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            OperationData OpData = KioskModeData.OperationData;
            switch( OpData.Mode )
            {
                case "Move":
                    CswNbtObjClassContainer containerToMove = _getNodeByBarcode( NbtResources, NbtObjectClass.ContainerClass, OpData.Field2.Value );
                    CswNbtObjClassLocation locationToMoveTo = _getNodeByBarcode( NbtResources, NbtObjectClass.LocationClass, OpData.Field1.Value );
                    containerToMove.MoveContainer( locationToMoveTo.NodeId );
                    containerToMove.postChanges( false );
                    OpData.Log.Add( DateTime.Now + " - Moved container " + OpData.Field2.Value + " to " + locationToMoveTo.Name.Text + " (" + OpData.Field1.Value + ")" );
                    break;
                case "Owner":
                    CswNbtObjClassContainer containerNode = _getNodeByBarcode( NbtResources, NbtObjectClass.ContainerClass, OpData.Field2.Value );
                    CswNbtObjClassUser newOwnerNode = _getNodeByBarcode( NbtResources, NbtObjectClass.UserClass, OpData.Field1.Value );
                    containerNode.Owner.RelatedNodeId = newOwnerNode.NodeId;
                    containerNode.Owner.RefreshNodeName();
                    containerNode.postChanges( false );
                    OpData.Log.Add( DateTime.Now + " - Changed owner of container " + OpData.Field2.Value + " to " + newOwnerNode.FirstName + " " + newOwnerNode.LastName + " (" + OpData.Field1.Value + ")" );
                    break;
                case "Transfer":
                    CswNbtObjClassContainer containerToTransfer = _getNodeByBarcode( NbtResources, NbtObjectClass.ContainerClass, OpData.Field2.Value );
                    CswNbtObjClassUser newTransferOwner = _getNodeByBarcode( NbtResources, NbtObjectClass.UserClass, OpData.Field1.Value );
                    containerToTransfer.Owner.RelatedNodeId = newTransferOwner.NodeId;
                    containerToTransfer.Owner.RefreshNodeName();
                    containerToTransfer.MoveContainer( newTransferOwner.DefaultLocationId );
                    containerToTransfer.postChanges( false );
                    CswNbtObjClassLocation newLocationNode = NbtResources.Nodes[newTransferOwner.DefaultLocationId];
                    OpData.Log.Add( DateTime.Now + " - Transfered container ownership" + OpData.Field2.Value + " to " + newTransferOwner.FirstName + " " + newTransferOwner.LastName + " (" + OpData.Field1.Value + ")  and changed location to " + newLocationNode.Name.Text );
                    break;
                case "DispenseContainer":
                    //TODO: dispense container
                    break;
                case "DisposeContainer":
                    //TODO: dispose container
                    break;
            }
            OpData.Field2.Value = string.Empty;
            OpData.Field2.SecondValue = string.Empty;
            OpData.Field2.ServerValidated = false;
            KioskModeData.OperationData = OpData;
            Return.Data = KioskModeData;
        }

        #region Private Methods

        private static void _setFields( OperationData OpData )
        {
            OpData.ModeServerValidated = true;
            OpData.ModeStatusMsg = string.Empty;
            switch( OpData.Mode )
            {
                case "Move":
                    OpData.Field1.Name = "Location";
                    OpData.Field2.Name = "Item";
                    break;
                case "Owner":
                    OpData.Field1.Name = "User";
                    OpData.Field2.Name = "Item";
                    break;
                case "Transfer":
                    OpData.Field1.Name = "User";
                    OpData.Field2.Name = "Item";
                    break;
                case "DispenseContainer":
                    //TODO: dispense container
                    break;
                case "DisposeContainer":
                    //TODO: dispose container
                    break;
                default:
                    OpData.ModeStatusMsg = "Error: Scanned mode does not exist or is unavailable";
                    OpData.ModeServerValidated = false;
                    break;
            }
        }

        private static bool _validateBarcode( CswNbtResources NbtResources, NbtObjectClass ObjClass, string Barcode )
        {
            ICswNbtTree tree = _getTree( NbtResources, ObjClass, Barcode );
            int childCount = tree.getChildNodeCount();
            return childCount > 0 ? true : false; //true if the item with this barcode exists
        }

        private static CswNbtNode _getNodeByBarcode( CswNbtResources NbtResources, NbtObjectClass ObjClass, string Barcode )
        {
            CswNbtNode Ret = null;
            ICswNbtTree tree = _getTree( NbtResources, ObjClass, Barcode );
            int childCount = tree.getChildNodeCount();
            if( childCount > 0 )
            {
                tree.goToNthChild( 0 );
                Ret = tree.getNodeForCurrentPosition();
            }
            return Ret;
        }

        private static CswPrimaryKey _getNodeIdByBarcode( CswNbtResources NbtResources, NbtObjectClass ObjClass, string Barcode )
        {
            CswPrimaryKey Ret = null;
            ICswNbtTree tree = _getTree( NbtResources, ObjClass, Barcode );
            int childCount = tree.getChildNodeCount();
            if( childCount > 0 )
            {
                tree.goToNthChild( 0 );
                Ret = tree.getNodeIdForCurrentPosition();
            }
            return Ret;
        }

        private static ICswNbtTree _getTree( CswNbtResources NbtResources, NbtObjectClass ObjClass, string Barcode )
        {
            CswNbtMetaDataObjectClass metaDataOC = NbtResources.MetaData.getObjectClass( ObjClass );
            CswNbtMetaDataObjectClassProp barcodeOCP = metaDataOC.getBarcodeProp();
            CswNbtView view = new CswNbtView( NbtResources );
            CswNbtViewRelationship parent = view.AddViewRelationship( metaDataOC, true );
            view.AddViewPropertyAndFilter( parent,
                MetaDataProp: barcodeOCP,
                Value: Barcode,
                SubFieldName: CswNbtSubField.SubFieldName.Barcode,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals
            );

            ICswNbtTree tree = NbtResources.Trees.getTreeFromView( view, true, false, false );
            return tree;
        }

        #endregion
    }

}