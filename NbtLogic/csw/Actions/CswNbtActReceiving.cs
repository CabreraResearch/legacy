using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using NbtWebApp.Actions.Receiving;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.Actions
{
    public class CswNbtActReceiving
    {
        #region Private, core methods

        private CswNbtResources _CswNbtResources;
        private CswNbtSdTabsAndProps _CswNbtSdTabsAndProps;
        private CswNbtMetaDataObjectClass _ContainerOc = null;
        private CswPrimaryKey _MaterialId = null;

        #endregion Private, core methods

        #region Constructor

        public CswNbtActReceiving( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSdTabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );
        }

        public CswNbtActReceiving( CswNbtResources CswNbtResources, CswPrimaryKey MaterialNodeId )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSdTabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Cannot use the Receive Material action without the required module.", "Attempted to constuct CswNbtActReceiving without the required module." );
            }
            _MaterialId = MaterialNodeId;
            _ContainerOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
        }

        #endregion Constructor

        #region Public methods and props

        ///<summary>
        /// Instance a new container according to Object Class rules. Note: this does not get the properties.
        /// </summary>
        public CswNbtObjClassContainer makeContainer( Action<CswNbtNode> After )
        {
            CswNbtObjClassContainer ret = null;

            CswNbtMetaDataNodeType ContainerNt = _ContainerOc.getLatestVersionNodeTypes().FirstOrDefault();
            if( null != ContainerNt )
            {
                Action<CswNbtNode> After2 = delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassContainer RetAsContainer = NewNode;
                        RetAsContainer.Material.RelatedNodeId = _MaterialId;
                        //cases 30647 and 31079
                        //RetAsContainer.Material.setHidden( value: true, SaveToDb: false );
                        //RetAsContainer.Size.setHidden( value: true, SaveToDb: false );
                        if( null != After )
                        {
                            After( NewNode );
                        }
                    };
                ret = _CswNbtSdTabsAndProps.getAddNodeAndPostChanges( ContainerNt, After2 );
                if( null == ret )
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Could not create a new container.", "Failed to create a new Container node." );
                }
            }
            return ret;
        } // makeContainer()

        public JObject receiveMaterial( CswNbtReceivingDefinition ReceiptDefinition )
        {
            JObject Ret = new JObject();

            CswNbtObjClassContainer InitialContainerNode = _CswNbtResources.Nodes[ReceiptDefinition.ContainerNodeId];
            if( null != InitialContainerNode && ReceiptDefinition.Quantities.Count > 0 )
            {
                ReceiptDefinition.Quantities[0].ContainerIds.Add( InitialContainerNode.NodeId.ToString() );
                int processed = 0;
                receiveContainers( ReceiptDefinition, ref processed, 1 ); //process only the first container (the initial one)

                commitSDSDocNode( ReceiptDefinition.MaterialNodeId, ReceiptDefinition.SDSNodeId, ReceiptDefinition.SDSProps );

                CswNbtNode ReceiptLot = _makeReceiptLot( ReceiptDefinition.MaterialNodeId, ReceiptDefinition, InitialContainerNode.ExpirationDate.DateTimeValue );
                if( null == ReceiptDefinition.ReceiptLotNodeId )
                {
                    ReceiptDefinition.ReceiptLotNodeId = ReceiptLot.NodeId;
                }

                //Generate the barcodes upfront and treat them as custom barcodes so all Containers in this Receipt Def have similar numbers
                CswNbtMetaDataNodeTypeProp BarcodeProp = (CswNbtMetaDataNodeTypeProp) InitialContainerNode.NodeType.getBarcodeProperty();
                CswNbtSequenceValue ContainerBarcodeSequence = new CswNbtSequenceValue( BarcodeProp.PropId, _CswNbtResources );
                for( int i = 0; i < ReceiptDefinition.Quantities.Count; i++ )
                {
                    CswNbtAmountsGridQuantity quantity = ReceiptDefinition.Quantities[i];
                    while( quantity.Barcodes.Count < quantity.NumContainers )
                    {
                        if( quantity.Barcodes.Count == 0 && i == 0 ) //special case: the Initial Container node already has a barcode, so insert that one if a user didn't specify a custom barcode
                        {
                            quantity.Barcodes.Add( InitialContainerNode.Barcode.Barcode );
                        }
                        else
                        {
                            quantity.Barcodes.Add( ContainerBarcodeSequence.getNext() );
                        }
                    }
                }

                int TotalContainersToMake = ReceiptDefinition.CountNumberContainersToMake();
                int Threshhold = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( "batchthreshold" ) );
                bool UseBatchOp = TotalContainersToMake > Threshhold;
                Collection<CswPrimaryKey> ContainerIds = null;
                if( UseBatchOp )
                {
                    //Containers will be created in a batch op
                    CswNbtBatchOpReceiving ReceivingBatchOp = new CswNbtBatchOpReceiving( _CswNbtResources );
                    ReceivingBatchOp.makeBatchOp( ReceiptDefinition );
                }
                else
                {
                    //Create the containers now
                    int nodesProcessed = 0;
                    CswNbtReceivingDefinition ReceivedContainers = receiveContainers( ReceiptDefinition, ref nodesProcessed, TotalContainersToMake + 1 );
                    ContainerIds = new Collection<CswPrimaryKey>();
                    foreach( CswNbtAmountsGridQuantity Quantity in ReceivedContainers.Quantities )
                    {
                        foreach( string ContainerId in Quantity.ContainerIds )
                        {
                            ContainerIds.Add(CswConvert.ToPrimaryKey(ContainerId));
                        }
                    }
                }
                
                CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( ReceiptDefinition.MaterialNodeId );
                Ret = getLandingPageData( _CswNbtResources, MaterialNode, UseBatchOp, ContainerIds );
            }

            return Ret;
        }

        public CswNbtReceivingDefinition receiveContainers( CswNbtReceivingDefinition ReceiptDef, ref int NodesProcessed, int MaxProcessed )
        {
            CswNbtMetaDataNodeType ContainerNt = _CswNbtResources.MetaData.getNodeType( ReceiptDef.ContainerNodeTypeId );

            foreach( CswNbtAmountsGridQuantity QuantityDef in ReceiptDef.Quantities )
            {
                CswNbtObjClassSize AsSize = _CswNbtResources.Nodes.GetNode( QuantityDef.SizeNodeId );
                string Barcode = string.Empty;
                Action<CswNbtNode> After = delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassContainer thisContainer = NewNode;
                    if( QuantityDef.Barcodes.Count <= QuantityDef.NumContainers && false == string.IsNullOrEmpty( Barcode ) )
                    {
                        thisContainer.Barcode.setBarcodeValueOverride( Barcode, false );
                    }
                    thisContainer.Size.RelatedNodeId = QuantityDef.SizeNodeId;
                    thisContainer.Material.RelatedNodeId = ReceiptDef.MaterialNodeId;
                    if( AsSize.QuantityEditable.Checked != CswEnumTristate.True )
                    {
                        QuantityDef.Quantity = AsSize.InitialQuantity.Quantity;
                        QuantityDef.UnitNodeId = AsSize.InitialQuantity.UnitId;
                    }
                    if( null == thisContainer.Quantity.UnitId || Int32.MinValue == thisContainer.Quantity.UnitId.PrimaryKey )
                    {
                        thisContainer.Quantity.UnitId = QuantityDef.UnitNodeId;
                    }
                    thisContainer.DispenseIn( CswEnumNbtContainerDispenseType.Receive, QuantityDef.Quantity, QuantityDef.UnitNodeId, ReceiptDef.RequestItemtNodeId );
                    thisContainer.Disposed.Checked = CswEnumTristate.False;
                    thisContainer.ReceiptLot.RelatedNodeId = ReceiptDef.ReceiptLotNodeId;
                };

                for( Int32 C = 0; C < QuantityDef.NumContainers; C += 1 )
                {
                    if( NodesProcessed < MaxProcessed )
                    {
                        //we promote the first container before the batch op starts, so there should always be at least one container id in the first set of quantities
                        CswPrimaryKey ContainerId = null;
                        if( C < QuantityDef.ContainerIds.Count )
                        {
                            ContainerId = CswConvert.ToPrimaryKey( QuantityDef.ContainerIds[C] );
                        }
                        Barcode = ( QuantityDef.Barcodes.Count > C ? QuantityDef.Barcodes[C] : string.Empty );

                        if( null == ContainerId ) //only create a container if we haven't already
                        {
                            CswNbtNodeKey ContainerNodeKey;
                            CswNbtObjClassContainer Container = _CswNbtSdTabsAndProps.addNode( ContainerNt, ReceiptDef.ContainerProps, out ContainerNodeKey, After );
                            QuantityDef.ContainerIds.Add( Container.NodeId.ToString() );
                            NodesProcessed++;
                        }
                        else
                        {
                            CswNbtNode InitialContainerNode = _CswNbtResources.Nodes.GetNode( ContainerId );
                            if( null != InitialContainerNode && InitialContainerNode.IsTemp )
                            {
                                _CswNbtSdTabsAndProps.saveNodeProps( InitialContainerNode, ReceiptDef.ContainerProps );
                                After( InitialContainerNode );
                                InitialContainerNode.PromoteTempToReal();
                                NodesProcessed++;
                            }
                        }
                    }
                } //for( Int32 C = 0; C < NoContainers; C += 1 )
            }

            return ReceiptDef;
        }

        /// <summary>
        /// Get a landing page for a Material
        /// </summary>
        public static JObject getLandingPageData( CswNbtResources NbtResources, CswNbtNode MaterialNode, bool UseBatchOp, Collection<CswPrimaryKey> ContainerIds, CswNbtView MaterialNodeView = null )
        {
            JObject Ret = new JObject();
            if( null != MaterialNode )
            {
                MaterialNodeView = MaterialNodeView ?? CswNbtPropertySetMaterial.getMaterialNodeView( NbtResources, MaterialNode, "Received: " );
                MaterialNodeView.SaveToCache( IncludeInQuickLaunch : false );

                Ret["ActionId"] = NbtResources.Actions[CswEnumNbtActionName.Receiving].ActionId.ToString();

                //Used for Tab and Button items
                Ret["NodeId"] = MaterialNode.NodeId.ToString();
                Ret["NodeViewId"] = MaterialNodeView.SessionViewId.ToString();

                //Used for node-specific Add items
                Ret["RelatedNodeId"] = MaterialNode.NodeId.ToString();
                Ret["RelatedNodeName"] = MaterialNode.NodeName;

                //If (and when) action landing pages are slated to be roleId-specific, remove this line
                Ret["isConfigurable"] = NbtResources.CurrentNbtUser.IsAdministrator();

                //Used for viewing new material
                Ret["ActionLinks"] = new JObject();
                string ActionLinkName = MaterialNode.NodeId.ToString();
                Ret["ActionLinks"][ActionLinkName] = new JObject();
                Ret["ActionLinks"][ActionLinkName]["Text"] = MaterialNode.NodeName;
                Ret["ActionLinks"][ActionLinkName]["ViewId"] = MaterialNodeView.SessionViewId.ToString();

                if( UseBatchOp )
                {
                    Ret["Title"] = "The containers for this material have been scheduled for creation, but may not be available immediately. Click 'View Batch Operations' to check their progress.";
                    Ret["ActionLinks"]["BatchOps"] = new JObject();
                    Ret["ActionLinks"]["BatchOps"]["ActionName"] = "These containers will be created in a batch operation and may not be immediately available. You can check the progress of their creation below:";
                    Ret["ActionLinks"]["BatchOps"]["Text"] = "View Batch Operations";
                    Ret["ActionLinks"]["BatchOps"]["ViewId"] = NbtResources.ViewSelect.getViewIdByName( "My Batch Operations", CswEnumNbtViewVisibility.Global, null, null ).ToString();
                }
                else
                {
                    CswNbtMetaDataNodeType ContainerNT = NbtResources.MetaData.getNodeType( "Container" );
                    if( null != ContainerNT )
                    {
                        CswNbtView NewContainersView = new CswNbtView( NbtResources );
                        NewContainersView.ViewName = "New Containers";
                        CswNbtViewRelationship ContainerVr = NewContainersView.AddViewRelationship( ContainerNT, true );
                        ContainerVr.NodeIdsToFilterIn = ContainerIds;
                        NewContainersView.SaveToCache( false );

                        Ret["ActionLinks"]["NewContainers"] = new JObject();
                        Ret["ActionLinks"]["NewContainers"]["Text"] = "View Received Containers";
                        Ret["ActionLinks"]["NewContainers"]["ViewId"] = NewContainersView.SessionViewId.ToString(); ;
                    }
                }
            }
            return Ret;
        }

        /// <summary>
        /// Persist the SDS Document
        /// </summary>
        public void commitSDSDocNode( CswPrimaryKey MaterialId, CswPrimaryKey SDSNodeId, JObject SDSProps )
        {
            CswNbtObjClassSDSDocument SDSDoc = _CswNbtResources.Nodes.GetNode( SDSNodeId );
            if( null != SDSDoc )
            {
                _CswNbtSdTabsAndProps.saveProps( SDSDoc.NodeId, Int32.MinValue, SDSProps, SDSDoc.NodeTypeId, null, IsIdentityTab : false, setIsTempToFalse : false );
                if( ( SDSDoc.FileType.Value == CswNbtPropertySetDocument.CswEnumDocumentFileTypes.File && false == string.IsNullOrEmpty( SDSDoc.File.FileName ) ) ||
                    ( SDSDoc.FileType.Value == CswNbtPropertySetDocument.CswEnumDocumentFileTypes.Link && false == string.IsNullOrEmpty( SDSDoc.Link.Href ) ) )
                {
                    SDSDoc.Owner.RelatedNodeId = MaterialId;
                    SDSDoc.PromoteTempToReal();
                }
            }
        }

        /// <summary>
        /// Updates the default Expiration Date on containers to receive based on Receipt Lot's Manufactured Date
        /// </summary>
        public ContainerData.ReceivingData updateExpirationDate( ContainerData.ReceiptLotRequest Request )
        {
            ContainerData.ReceivingData ReceiveData = new ContainerData.ReceivingData();
            JObject ReceiptLotPropsObj = CswConvert.ToJObject( Request.ReceiptLotProps );
            if( ReceiptLotPropsObj.HasValues )
            {
                CswPrimaryKey ReceiptLotId = CswConvert.ToPrimaryKey( Request.ReceiptLotId );
                if( CswTools.IsPrimaryKey( ReceiptLotId ) )
                {
                    CswNbtObjClassReceiptLot ReceiptLot = _CswNbtResources.Nodes.GetNode( ReceiptLotId );
                    _CswNbtSdTabsAndProps.saveNodeProps( ReceiptLot.Node, ReceiptLotPropsObj );
                    CswPrimaryKey ContainerId = CswConvert.ToPrimaryKey( Request.ContainerId );
                    if( CswTools.IsPrimaryKey( ContainerId ) &&
                        ReceiptLot.ManufacturedDate.DateTimeValue != DateTime.MinValue )
                    {
                        CswNbtObjClassContainer Container = _CswNbtResources.Nodes.GetNode( ContainerId );
                        CswNbtPropertySetMaterial Material = _CswNbtResources.Nodes.GetNode( Container.Material.RelatedNodeId );
                        Container.ExpirationDate.DateTimeValue = Material.getDefaultExpirationDate( ReceiptLot.ManufacturedDate.DateTimeValue );
                        Container.postChanges( false );
                        JObject ContainerProps = getContainerAddProps( Container );
                        ReceiveData.ContainerProps = ContainerProps.ToString();
                    }
                }
            }
            return ReceiveData;
        }

        /// <summary>
        /// Get the Add Layout properties for a container
        /// </summary>
        public JObject getContainerAddProps( CswNbtObjClassContainer Container )
        {
            JObject Ret = new JObject();
            if( null != Container )
            {
                Ret = _CswNbtSdTabsAndProps.getProps( Container.Node, "", null, CswEnumNbtLayoutType.Add );
            }
            return Ret;
        }

        #endregion Public methods and props

        #region Private Helper Functions

        private CswNbtNode _makeReceiptLot( CswPrimaryKey MaterialId, CswNbtReceivingDefinition ReceiptDef, DateTime ExpirationDate )
        {
            Action<CswNbtNode> AfterReceiptLot = delegate( CswNbtNode NewNode )
            {
                CswNbtObjClassReceiptLot thisReceiptLot = NewNode;
                thisReceiptLot.Material.RelatedNodeId = MaterialId;
                thisReceiptLot.RequestItem.RelatedNodeId = ReceiptDef.RequestItemtNodeId;
                thisReceiptLot.ExpirationDate.DateTimeValue = ExpirationDate;
            };

            CswNbtObjClassReceiptLot ReceiptLot = _CswNbtResources.Nodes.GetNode( ReceiptDef.ReceiptLotNodeId );
            if( null != ReceiptDef.ReceiptLotNodeId && null != ReceiptLot )
            {
                _CswNbtSdTabsAndProps.saveProps( ReceiptLot.NodeId, Int32.MinValue, ReceiptDef.ReceiptLotProps, ReceiptLot.NodeTypeId, null, IsIdentityTab : false );
                AfterReceiptLot( ReceiptLot.Node );
                ReceiptLot.postChanges( false );
            }
            else
            {
                CswNbtMetaDataObjectClass ReceiptLotClass = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
                ReceiptLot = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ReceiptLotClass.FirstNodeType.NodeTypeId, AfterReceiptLot );
            }
            _attachCofA( ReceiptLot.NodeId, ReceiptDef );
            return ReceiptLot.Node;
        }

        private void _attachCofA( CswPrimaryKey ReceiptLotId, CswNbtReceivingDefinition ReceiptDef )
        {
            CswNbtObjClassCofADocument CofADoc = _CswNbtResources.Nodes.GetNode( ReceiptDef.CofADocNodeId );
            if( null != CofADoc )
            {
                _CswNbtSdTabsAndProps.saveProps( CofADoc.NodeId, Int32.MinValue, ReceiptDef.CofAPropsJSON, CofADoc.NodeTypeId, null, IsIdentityTab : false );
                if( ( CofADoc.FileType.Value == CswNbtPropertySetDocument.CswEnumDocumentFileTypes.File && false == string.IsNullOrEmpty( CofADoc.File.FileName ) ) ||
                    ( CofADoc.FileType.Value == CswNbtPropertySetDocument.CswEnumDocumentFileTypes.Link && false == string.IsNullOrEmpty( CofADoc.Link.Href ) ) )
                {
                    CofADoc.Owner.RelatedNodeId = ReceiptLotId;
                    CofADoc.PromoteTempToReal();
                }
            }
        }


        #endregion Private Helper Functions
    }
}