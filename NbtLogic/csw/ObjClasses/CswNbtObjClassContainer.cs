using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Conversion;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassContainer : CswNbtObjClass
    {
        public const string BarcodePropertyName = "Barcode";
        public const string MaterialPropertyName = "Material";
        public const string LocationPropertyName = "Location";
        public const string LocationVerifiedPropertyName = "Location Verified";
        public const string StatusPropertyName = "Status";
        public const string MissingPropertyName = "Missing";
        public const string DisposedPropertyName = "Disposed";
        public const string SourceContainerPropertyName = "Source Container";
        public const string QuantityPropertyName = "Quantity";
        public const string ExpirationDatePropertyName = "Expiration Date";
        public const string SizePropertyName = "Size";
        public const string RequestPropertyName = "Request";
        public const string DispensePropertyName = "Dispense";
        public const string DisposePropertyName = "Dispose";
        public const string UndisposePropertyName = "Undispose";
        public const string OwnerPropertyName = "Owner";
        private bool _IsDisposed
        {
            get { return Disposed.Checked == Tristate.True; }
        }
        public sealed class RequestMenu
        {
            public const string Dispense = "Dispense";
            public const string Dispose = "Dispose";
            public const string Move = "Move";

            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    Dispense,
                    Dispose,
                    Move
                };

        }

        /// <summary>
        /// Has the corresponding Inventory Level been modified in a change event on this instance?
        /// </summary>
        private bool _InventoryLevelModified = false;

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassContainer( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassContainer
        /// </summary>
        public static implicit operator CswNbtObjClassContainer( CswNbtNode Node )
        {
            CswNbtObjClassContainer ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass ) )
            {
                ret = (CswNbtObjClassContainer) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
            Disposed.setHidden( value: true, SaveToDb: true );
            SourceContainer.setHidden( value: true, SaveToDb: true );
        } // afterCreateNode()

        private void _updateRequestMenu()
        {
            bool IsDisposed = Disposed.Checked == Tristate.True;
            CswCommaDelimitedString MenuOpts = new CswCommaDelimitedString();
            string SelectedOpt = RequestMenu.Dispose;
            if( false == IsDisposed )
            {
                if( Missing.Checked != Tristate.True && Quantity.Quantity > 0 )
                {
                    MenuOpts.Add( RequestMenu.Dispense );
                    SelectedOpt = RequestMenu.Dispense;
                }
                else
                {
                    SelectedOpt = RequestMenu.Move;
                }
                MenuOpts.Add( RequestMenu.Move );
                MenuOpts.Add( RequestMenu.Dispose );
            }
            Request.State = SelectedOpt;
            Request.MenuOptions = MenuOpts.ToString();
        }

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _updateRequestMenu();

            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            Material.SetOnPropChange( OnMaterialPropChange );
            Dispose.SetOnPropChange( OnDisposedPropChange );
            OnDisposedPropChange( Dispose );
            Quantity.SetOnPropChange( OnQuantityPropChange );
            Location.SetOnPropChange( OnLocationPropChange );
            Size.SetOnPropChange( OnSizePropChange );
            SourceContainer.SetOnPropChange( OnSourceContainerChange );
            Barcode.SetOnPropChange( OnBarcodePropChange );
            _toggleButtonVisibility();
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        private void _toggleButtonVisibility( bool SaveToDb = false )
        {
            Dispense.setHidden( value: ( _IsDisposed || false == _CswNbtResources.Permit.canContainer( NodeId, CswNbtPermit.NodeTypePermission.Create, _CswNbtResources.Actions[CswNbtActionName.DispenseContainer] ) ), SaveToDb: SaveToDb );
            Dispose.setHidden( value: ( _IsDisposed || false == _CswNbtResources.Permit.canContainer( NodeId, CswNbtPermit.NodeTypePermission.Edit, _CswNbtResources.Actions[CswNbtActionName.DisposeContainer] ) ), SaveToDb: SaveToDb );
            Undispose.setHidden( value: ( false == _IsDisposed || false == _CswNbtResources.Permit.canContainer( NodeId, CswNbtPermit.NodeTypePermission.Edit, _CswNbtResources.Actions[CswNbtActionName.UndisposeContainer] ) ), SaveToDb: SaveToDb );
            Request.setHidden( value: ( _IsDisposed || false == _CswNbtResources.Permit.canContainer( NodeId, CswNbtPermit.NodeTypePermission.View, _CswNbtResources.Actions[CswNbtActionName.Submit_Request] ) ), SaveToDb: SaveToDb );
        }

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            // Disposed == false
            CswNbtMetaDataObjectClassProp DisposedOCP = ObjectClass.getObjectClassProp( DisposedPropertyName );

            //ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, DisposedOCP, Tristate.False.ToString() );

            CswNbtViewProperty viewProp = ParentRelationship.View.AddViewProperty( ParentRelationship, DisposedOCP );
            viewProp.ShowInGrid = false;
            ParentRelationship.View.AddViewPropertyFilter( viewProp, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals, Value: Tristate.False.ToString() );

            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            CswNbtMetaDataObjectClassProp OCP = ButtonData.NodeTypeProp.getObjectClassProp();
            if( null != ButtonData.NodeTypeProp && null != OCP )
            {
                bool HasPermission = false;
                switch( OCP.PropName )
                {
                    case DisposePropertyName:
                        if( _CswNbtResources.Permit.canContainer( NodeId, CswNbtPermit.NodeTypePermission.Create, _CswNbtResources.Actions[CswNbtActionName.DisposeContainer] ) )
                        {
                            HasPermission = true;
                            DisposeContainer(); //case 26665
                            postChanges( true );
                            ButtonData.Action = NbtButtonAction.refresh;
                        }
                        break;
                    case UndisposePropertyName:
                        if( _CswNbtResources.Permit.canContainer( NodeId, CswNbtPermit.NodeTypePermission.Create, _CswNbtResources.Actions[CswNbtActionName.UndisposeContainer] ) )
                        {
                            HasPermission = true;
                            UndisposeContainer();
                            postChanges( true );
                            ButtonData.Action = NbtButtonAction.refresh;
                        }
                        break;
                    case DispensePropertyName:
                        if( _CswNbtResources.Permit.canContainer( NodeId, CswNbtPermit.NodeTypePermission.Create, _CswNbtResources.Actions[CswNbtActionName.DispenseContainer] ) )
                        {
                            HasPermission = true;
                            //ActionData = this.NodeId.ToString();
                            ButtonData.Data = _getDispenseActionData();
                            ButtonData.Action = NbtButtonAction.dispense;
                        }
                        break;
                    case RequestPropertyName:
                        if( _CswNbtResources.Permit.canContainer( NodeId, CswNbtPermit.NodeTypePermission.Create, _CswNbtResources.Actions[CswNbtActionName.Submit_Request] ) )
                        {
                            CswNbtActSubmitRequest RequestAct = new CswNbtActSubmitRequest( _CswNbtResources, CreateDefaultRequestNode: true );
                            HasPermission = true;

                            CswNbtObjClassRequestItem NodeAsRequestItem = RequestAct.makeContainerRequestItem( new CswNbtActSubmitRequest.RequestItem(), NodeId, ButtonData.SelectedText );
                            NodeAsRequestItem.Material.RelatedNodeId = Material.RelatedNodeId;
                            NodeAsRequestItem.Material.setReadOnly( value: true, SaveToDb: ButtonData.SelectedText != RequestMenu.Dispense );

                            CswPrimaryKey SelectedLocationId = new CswPrimaryKey();
                            if( CswTools.IsPrimaryKey( _CswNbtResources.CurrentNbtUser.DefaultLocationId ) )
                            {
                                SelectedLocationId = _CswNbtResources.CurrentNbtUser.DefaultLocationId;
                            }
                            else
                            {
                                SelectedLocationId = Location.SelectedNodeId;
                            }
                            NodeAsRequestItem.Location.SelectedNodeId = SelectedLocationId;
                            NodeAsRequestItem.Location.RefreshNodeName();
                            NodeAsRequestItem.Material.setHidden( value: true, SaveToDb: ButtonData.SelectedText != RequestMenu.Dispense );

                            switch( ButtonData.SelectedText )
                            {
                                case RequestMenu.Dispense:
                                    NodeAsRequestItem.Quantity.UnitId = Quantity.UnitId;
                                    ButtonData.Action = NbtButtonAction.request;
                                    break;
                                case RequestMenu.Dispose:
                                    NodeAsRequestItem.IsTemp = false; /* This is the only condition in which we want to commit the node upfront. */
                                    ButtonData.Action = NbtButtonAction.nothing;
                                    break;
                                case RequestMenu.Move:
                                    ButtonData.Action = NbtButtonAction.request;
                                    break;
                            }
                            NodeAsRequestItem.postChanges( ForceUpdate: true );
                            ButtonData.Data["titleText"] = OCP.PropName + " " + Barcode.Barcode;
                            ButtonData.Data["requestaction"] = OCP.PropName;
                            ButtonData.Data["requestItemProps"] = RequestAct.getRequestItemAddProps( NodeAsRequestItem );
                            ButtonData.Data["requestItemNodeTypeId"] = RequestAct.RequestItemNt.NodeTypeId;
                        }
                        break;
                }
                if( false == HasPermission )
                {
                    throw new CswDniException( ErrorType.Warning, "You do not have permission to the " + OCP.PropName + " action.", "You do not have permission to the " + OCP.PropName + " action." );
                }
            }
            return true;
        }
        #endregion Inherited Events

        #region Custom Logic

        /// <summary>
        /// Checks permission and disposes a container
        /// </summary>
        public void DisposeContainer()
        {

            if( _CswNbtResources.Permit.canContainer( NodeId, CswNbtPermit.NodeTypePermission.Create, _CswNbtResources.Actions[CswNbtActionName.DisposeContainer] ) )
            {
                _createContainerTransactionNode( CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispose, -this.Quantity.Quantity, this.Quantity.UnitId, SourceContainer: this );
                this.Quantity.Quantity = 0;
                this.Disposed.Checked = Tristate.True;
                _setDisposedReadOnly( true );
            }
        }

        /// <summary>
        /// Checks permission and undisposes a container
        /// </summary>
        public void UndisposeContainer()
        {

            if( _CswNbtResources.Permit.canContainer( NodeId, CswNbtPermit.NodeTypePermission.Create, _CswNbtResources.Actions[CswNbtActionName.UndisposeContainer] ) )
            {
                CswNbtMetaDataNodeType ContDispTransNT = _CswNbtResources.MetaData.getNodeType( "Container Dispense Transaction" );
                CswNbtObjClassContainerDispenseTransaction ContDispTransNode = _getMostRecentDisposeTransaction( ContDispTransNT );

                if( ContDispTransNode != null )
                {
                    this.Quantity.Quantity = -ContDispTransNode.QuantityDispensed.Quantity;
                    this.Quantity.UnitId = ContDispTransNode.QuantityDispensed.UnitId;
                    ContDispTransNode.Node.delete( OverridePermissions: true );
                }
                this.Disposed.Checked = Tristate.False;
                _setDisposedReadOnly( false );
            }
        }

        /// <summary>
        /// Dispense out of this container.
        /// </summary>
        /// <param name="DispenseType"></param>
        /// <param name="QuantityToDeduct">Positive quantity to subtract</param>
        /// <param name="UnitId"></param>
        /// <param name="RequestItemId"></param>
        /// <param name="DestinationContainer"></param>
        public void DispenseOut( CswNbtObjClassContainerDispenseTransaction.DispenseType DispenseType, double QuantityToDeduct, CswPrimaryKey UnitId,
                                 CswPrimaryKey RequestItemId = null, CswNbtObjClassContainer DestinationContainer = null, bool RecordTransaction = true )
        {
            double RealQuantityToDeduct = _getDispenseAmountInProperUnits( QuantityToDeduct, UnitId, Quantity.UnitId );
            double CurrentQuantity = 0;
            if( CswTools.IsDouble( Quantity.Quantity ) )
            {
                CurrentQuantity = Quantity.Quantity;
            }
            Quantity.Quantity = CurrentQuantity - RealQuantityToDeduct;

            if( DestinationContainer != null )
            {
                DestinationContainer.DispenseIn( DispenseType, QuantityToDeduct, UnitId, RequestItemId, this, false );  // false, because we do not want another duplicate transaction record
            }
            if( RecordTransaction )
            {
                _createContainerTransactionNode( DispenseType, -RealQuantityToDeduct, this.Quantity.UnitId, RequestItemId, this, DestinationContainer );
            }
        } // DispenseOut()

        /// <summary>
        /// Dispense into this container.  
        /// </summary>
        /// <param name="DispenseType"></param>
        /// <param name="QuantityToAdd">Positive quantity to add</param>
        /// <param name="UnitId"></param>
        /// <param name="RequestItemId"></param>
        /// <param name="SourceContainer"></param>
        public void DispenseIn( CswNbtObjClassContainerDispenseTransaction.DispenseType DispenseType, double QuantityToAdd, CswPrimaryKey UnitId,
                                CswPrimaryKey RequestItemId = null, CswNbtObjClassContainer SourceContainer = null, bool RecordTransaction = true )
        {
            double RealQuantityToAdd = _getDispenseAmountInProperUnits( QuantityToAdd, UnitId, Quantity.UnitId );
            double CurrentQuantity = 0;
            if( CswTools.IsDouble( Quantity.Quantity ) )
            {
                CurrentQuantity = Quantity.Quantity;
            }
            Quantity.Quantity = CurrentQuantity + RealQuantityToAdd;
            if( RecordTransaction )
            {
                _createContainerTransactionNode( DispenseType, RealQuantityToAdd, Quantity.UnitId, RequestItemId, SourceContainer, this );
            }
        } // DispenseIn()

        #endregion Custom Logic

        #region Private Helper Methods

        private double _getDispenseAmountInProperUnits( double Quantity, CswPrimaryKey OldUnitId, CswPrimaryKey NewUnitId )
        {
            double convertedValue = Quantity;
            CswNbtUnitConversion ConversionObj = new CswNbtUnitConversion( _CswNbtResources, OldUnitId, NewUnitId, Material.RelatedNodeId );
            convertedValue = ConversionObj.convertUnit( Quantity );
            return convertedValue;
        }

        private JObject _getDispenseActionData()
        {
            JObject ActionDataObj = new JObject();
            ActionDataObj["sourceContainerNodeId"] = NodeId.ToString();
            ActionDataObj["containerobjectclassid"] = ObjectClass.ObjectClassId;
            ActionDataObj["containernodetypeid"] = NodeTypeId;
            ActionDataObj["barcode"] = Barcode.Barcode;
            ActionDataObj["materialname"] = Material.CachedNodeName;
            ActionDataObj["location"] = Location.CachedFullPath;
            ActionDataObj["sizeid"] = Size.RelatedNodeId.ToString();

            CswNbtObjClassUnitOfMeasure unitNode = _CswNbtResources.Nodes.GetNode( Quantity.UnitId );
            if( null != unitNode )
            {
                ActionDataObj["currentQuantity"] = Quantity.Quantity;
                ActionDataObj["currentUnitName"] = unitNode.Name.Text;
            }
            JObject CapacityObj = _getCapacityJSON();
            ActionDataObj["capacity"] = CapacityObj.ToString();
            bool customBarcodes = CswConvert.ToBoolean( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.custom_barcodes.ToString() ) );
            ActionDataObj["customBarcodes"] = customBarcodes;
            return ActionDataObj;
        }

        private JObject _getCapacityJSON()
        {
            JObject CapacityObj = new JObject();
            CswNbtObjClassSize sizeNode = _CswNbtResources.Nodes.GetNode( Size.RelatedNodeId );
            if( null != sizeNode )
            {
                CswNbtNodePropQuantity Capacity = sizeNode.InitialQuantity;
                Capacity.ToJSON( CapacityObj );
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Cannot dispense container: Container's size is undefined.", "Dispense fail - null Size relationship." );
            }
            return CapacityObj;
        }


        private CswNbtObjClassContainerDispenseTransaction _getMostRecentDisposeTransaction( CswNbtMetaDataNodeType ContDispTransNT )
        {
            CswNbtObjClassContainerDispenseTransaction ContDispTransNode = null;
            if( ContDispTransNT != null )
            {
                CswNbtView DisposedContainerTransactionsView = new CswNbtView( _CswNbtResources );
                DisposedContainerTransactionsView.ViewName = "ContDispTransDisposed";
                CswNbtViewRelationship ParentRelationship = DisposedContainerTransactionsView.AddViewRelationship( ContDispTransNT, false );

                DisposedContainerTransactionsView.AddViewPropertyAndFilter(
                    ParentRelationship,
                    ContDispTransNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.SourceContainerPropertyName ),
                    NodeId.PrimaryKey.ToString(),
                    CswNbtSubField.SubFieldName.NodeID,
                    false,
                    CswNbtPropFilterSql.PropertyFilterMode.Equals
                    );

                DisposedContainerTransactionsView.AddViewPropertyAndFilter(
                    ParentRelationship,
                    ContDispTransNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.TypePropertyName ),
                    CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispose.ToString(),
                    CswNbtSubField.SubFieldName.Value,
                    false,
                    CswNbtPropFilterSql.PropertyFilterMode.Equals
                    );

                ICswNbtTree DispenseTransactionTree = _CswNbtResources.Trees.getTreeFromView( DisposedContainerTransactionsView, false, true );
                int NumOfTransactions = DispenseTransactionTree.getChildNodeCount();
                if( NumOfTransactions > 0 )
                {
                    DispenseTransactionTree.goToNthChild( 0 );
                    ContDispTransNode = DispenseTransactionTree.getNodeForCurrentPosition();
                }
            }
            return ContDispTransNode;
        }

        /// <summary>
        /// Record a container dispense transaction
        /// </summary>
        /// <param name="DispenseType"></param>
        /// <param name="Quantity">Quantity adjustment (negative for dispenses, disposes, and wastes, positive for receiving and add)</param>
        /// <param name="UnitId"></param>
        /// <param name="RequestItemId"></param>
        /// <param name="SourceContainer"></param>
        /// <param name="DestinationContainer"></param>
        private void _createContainerTransactionNode( CswNbtObjClassContainerDispenseTransaction.DispenseType DispenseType, double Quantity, CswPrimaryKey UnitId, CswPrimaryKey RequestItemId = null,
                                                      CswNbtObjClassContainer SourceContainer = null, CswNbtObjClassContainer DestinationContainer = null )
        {
            CswNbtMetaDataNodeType ContDispTransNT = _CswNbtResources.MetaData.getNodeType( "Container Dispense Transaction" );
            if( ContDispTransNT != null )
            {
                CswNbtObjClassContainerDispenseTransaction ContDispTransNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ContDispTransNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );

                if( SourceContainer != null )
                {
                    ContDispTransNode.SourceContainer.RelatedNodeId = SourceContainer.NodeId;
                    ContDispTransNode.RemainingSourceContainerQuantity.Quantity = SourceContainer.Quantity.Quantity;
                    ContDispTransNode.RemainingSourceContainerQuantity.UnitId = SourceContainer.Quantity.UnitId;
                }
                if( DestinationContainer != null )
                {
                    ContDispTransNode.DestinationContainer.RelatedNodeId = DestinationContainer.NodeId;
                }
                ContDispTransNode.QuantityDispensed.Quantity = Quantity;
                ContDispTransNode.QuantityDispensed.UnitId = UnitId;
                ContDispTransNode.Type.Value = DispenseType.ToString();
                ContDispTransNode.DispensedDate.DateTimeValue = DateTime.Now;
                if( null != RequestItemId && Int32.MinValue != RequestItemId.PrimaryKey )
                {
                    ContDispTransNode.RequestItem.RelatedNodeId = RequestItemId;
                }
                ContDispTransNode.postChanges( false );
            } // if( ContDispTransNT != null )
        } // _createContainerTransactionNode


        private void _setDisposedReadOnly( bool isReadOnly )//case 25814
        {
            Barcode.setReadOnly( value: isReadOnly, SaveToDb: true );
            Material.setReadOnly( value: isReadOnly, SaveToDb: true );
            Location.setReadOnly( value: isReadOnly, SaveToDb: true );
            Status.setReadOnly( value: isReadOnly, SaveToDb: true );
            Missing.setReadOnly( value: isReadOnly, SaveToDb: true );
            SourceContainer.setReadOnly( value: isReadOnly, SaveToDb: true );
            ExpirationDate.setReadOnly( value: isReadOnly, SaveToDb: true );
            Size.setReadOnly( value: isReadOnly, SaveToDb: true );
            Owner.setReadOnly( value: isReadOnly, SaveToDb: true );
            _toggleButtonVisibility( true );
        }

        private bool _isStorageCompatible( CswDelimitedString materialStorageCompatibility, CswDelimitedString locationStorageCompatibilities )
        {
            //if storage compatibility on the material is null, it can go anywhere
            //OR if SC on the location is null, it can store anything
            bool ret = materialStorageCompatibility.Count == 0 || locationStorageCompatibilities.Count == 0;
            foreach( string matComp in materialStorageCompatibility ) //loop through the materials storage compatibilities
            {
                if( matComp.Contains( "0w.gif" ) ) //if it has '0-none' selected, it can go anywhere
                {
                    ret = true;
                }
            }
            foreach( string comp in locationStorageCompatibilities )
            {
                if( materialStorageCompatibility.Contains( comp ) || comp.Contains( "0w.gif" ) ) //if the locations storage compatibility matches OR it has '0-none', it can house the material
                {
                    ret = true;
                }
            }
            return ret;
        }

        #endregion

        #region Object class specific properties

        private void _updateRequestItems( string RequestItemType )
        {
            if( RequestItemType == CswNbtObjClassRequestItem.Types.Move ||
             RequestItemType == CswNbtObjClassRequestItem.Types.Dispose )
            {
                CswNbtView RequestItemView = new CswNbtView( _CswNbtResources );
                CswNbtMetaDataObjectClass RequestItemOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
                CswNbtViewRelationship RiRelationship = RequestItemView.AddViewRelationship( RequestItemOc, false );
                CswNbtMetaDataObjectClassProp StatusOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Status );
                CswNbtMetaDataObjectClassProp ContainerOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Container );
                CswNbtMetaDataObjectClassProp TypeOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Type );

                RequestItemView.AddViewPropertyAndFilter( RiRelationship, StatusOcp, CswNbtObjClassRequestItem.Statuses.Submitted );
                RequestItemView.AddViewPropertyAndFilter( RiRelationship, ContainerOcp, SubFieldName: CswNbtSubField.SubFieldName.NodeID, Value: NodeId.PrimaryKey.ToString() );
                RequestItemView.AddViewPropertyAndFilter( RiRelationship, TypeOcp, RequestItemType );

                if( RequestItemType == CswNbtObjClassRequestItem.Types.Move )
                {
                    CswNbtMetaDataObjectClassProp LocationOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Location );
                    RequestItemView.AddViewPropertyAndFilter( RiRelationship, LocationOcp, SubFieldName: CswNbtSubField.SubFieldName.NodeID, Value: Location.SelectedNodeId.PrimaryKey.ToString() );
                }

                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( RequestItemView, IncludeSystemNodes: false, RequireViewPermissions: false );
                if( Tree.getChildNodeCount() > 0 )
                {
                    for( Int32 N = 0; N < Tree.getChildNodeCount(); N += 1 )
                    {
                        Tree.goToNthChild( N );
                        CswNbtObjClassRequestItem NodeAsRequestItem = Tree.getNodeForCurrentPosition();
                        if( null != NodeAsRequestItem )
                        {
                            switch( RequestItemType )
                            {
                                case CswNbtObjClassRequestItem.Types.Move:
                                    NodeAsRequestItem.Status.Value = CswNbtObjClassRequestItem.Statuses.Moved;
                                    break;
                                case CswNbtObjClassRequestItem.Types.Dispose:
                                    NodeAsRequestItem.Status.Value = CswNbtObjClassRequestItem.Statuses.Disposed;
                                    break;
                            }
                            NodeAsRequestItem.postChanges( false );
                        }
                        Tree.goToParentNode();
                    }
                }
            }
        }

        public CswNbtNodePropBarcode Barcode { get { return ( _CswNbtNode.Properties[BarcodePropertyName] ); } }
        private void OnBarcodePropChange( CswNbtNodeProp Prop )
        {
            Barcode.setReadOnly( value: false == string.IsNullOrEmpty( Barcode.Barcode ), SaveToDb: true );
        }

        public CswNbtNodePropLocation Location { get { return ( _CswNbtNode.Properties[LocationPropertyName] ); } }
        private void OnLocationPropChange( CswNbtNodeProp Prop )
        {
            CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( Material.RelatedNodeId );
            if( MaterialNode != null )
            {
                // case 24488 - When Location is modified, verify that:
                //  the Material's Storage Compatibility is null,
                //  or the Material's Storage Compatibility is one the selected values in the new Location.
                CswNbtNodePropImageList materialStorageCompatibilty = MaterialNode.Properties[CswNbtObjClassMaterial.StorageCompatibilityPropertyName];
                CswNbtNode locationNode = _CswNbtResources.Nodes.GetNode( Location.SelectedNodeId );
                if( null != locationNode ) //what if the user didn't specify a location?
                {
                    CswNbtNodePropImageList locationStorageCompatibility = locationNode.Properties[CswNbtObjClassLocation.StorageCompatabilityPropertyName];
                    if( false == _isStorageCompatible( materialStorageCompatibilty.Value, locationStorageCompatibility.Value ) )
                    {
                        throw new CswDniException( ErrorType.Warning,
                                                  "Storage compatibilities do not match, cannot move this container to specified location",
                                                  "Storage compatibilities do not match, cannot move this container to specified location" );
                    }
                }
            }
            if( CswTools.IsPrimaryKey( Location.SelectedNodeId ) &&
                false == string.IsNullOrEmpty( Location.CachedNodeName ) &&
                Location.CachedNodeName != CswNbtNodePropLocation.TopLevelName )
            {
                if( false == _InventoryLevelModified &&
                    CswConvert.ToInt32( Quantity.Quantity ) != 0 )
                {
                    CswNbtSdInventoryLevelMgr Mgr = new CswNbtSdInventoryLevelMgr( _CswNbtResources );
                    CswNbtSubField NodeId = ( (CswNbtFieldTypeRuleLocation) _CswNbtResources.MetaData.getFieldTypeRule( Location.getFieldType().FieldType ) ).NodeIdSubField;
                    Int32 PrevLocationId = CswConvert.ToInt32( Node.Properties[LocationPropertyName].GetOriginalPropRowValue( NodeId.Column ) );
                    string Reason = "Container " + Barcode.Barcode + " moved to new location: " + Location.CachedNodeName;
                    if( Int32.MinValue != PrevLocationId )
                    {
                        CswPrimaryKey PrevLocationPk = new CswPrimaryKey( "nodes", PrevLocationId );
                        if( PrevLocationPk != Location.SelectedNodeId )
                        {
                            _InventoryLevelModified = Mgr.changeLocationOfQuantity( Quantity.Quantity, Quantity.UnitId, Reason, Material.RelatedNodeId, PrevLocationPk, Location.SelectedNodeId );
                        }
                    }
                    else
                    {
                        _InventoryLevelModified = Mgr.addToCurrentQuantity( Quantity.Quantity, Quantity.UnitId, Reason, Material.RelatedNodeId, Location.SelectedNodeId );
                    }
                }
                _updateRequestItems( CswNbtObjClassRequestItem.Types.Move );
            }
        }
        public CswNbtNodePropDateTime LocationVerified { get { return ( _CswNbtNode.Properties[LocationVerifiedPropertyName] ); } }
        public CswNbtNodePropRelationship Material { get { return ( _CswNbtNode.Properties[MaterialPropertyName] ); } }
        private void OnMaterialPropChange( CswNbtNodeProp Prop )
        {
            if( Material.RelatedNodeId != null )
            {
                CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( Material.RelatedNodeId );
                if( MaterialNode != null )
                {
                    CswNbtObjClassMaterial MaterialNodeAsMaterial = MaterialNode;
                    // case 24488 - Expiration Date default is Today + Expiration Interval of the Material
                    // I'd like to do this on beforeCreateNode(), but the Material isn't set yet.
                    if( ExpirationDate.DateTimeValue == DateTime.MinValue )
                    {
                        ExpirationDate.DateTimeValue = MaterialNodeAsMaterial.getDefaultExpirationDate();
                    }
                }
                SourceContainer.setReadOnly( value: true, SaveToDb: true );
            }
        }
        public CswNbtNodePropList Status { get { return ( _CswNbtNode.Properties[StatusPropertyName] ); } }
        public CswNbtNodePropLogical Missing { get { return ( _CswNbtNode.Properties[MissingPropertyName] ); } }
        public CswNbtNodePropLogical Disposed { get { return ( _CswNbtNode.Properties[DisposedPropertyName] ); } }
        private void OnDisposedPropChange( CswNbtNodeProp Prop )
        {
            if( Disposed.Checked == Tristate.True )
            {
                _updateRequestItems( CswNbtObjClassRequestItem.Types.Dispose );
            }
            _updateRequestMenu();
        }
        public CswNbtNodePropRelationship SourceContainer { get { return ( _CswNbtNode.Properties[SourceContainerPropertyName] ); } }
        private void OnSourceContainerChange( CswNbtNodeProp Prop )
        {
            if( null != SourceContainer.RelatedNodeId && Int32.MinValue != SourceContainer.RelatedNodeId.PrimaryKey )
            {
                SourceContainer.setHidden( value: false, SaveToDb: true );
            }
        }
        public CswNbtNodePropQuantity Quantity { get { return ( _CswNbtNode.Properties[QuantityPropertyName] ); } }
        private void OnQuantityPropChange( CswNbtNodeProp Prop )
        {
            _updateRequestMenu();
            if( false == _InventoryLevelModified )
            {
                CswNbtSdInventoryLevelMgr Mgr = new CswNbtSdInventoryLevelMgr( _CswNbtResources );
                double PrevQuantity = CswConvert.ToDouble( Node.Properties[QuantityPropertyName].GetOriginalPropRowValue( ( (CswNbtFieldTypeRuleQuantity) _CswNbtResources.MetaData.getFieldTypeRule( Quantity.getFieldType().FieldType ) ).QuantitySubField.Column ) );
                if( false == CswTools.IsDouble( PrevQuantity ) )
                {
                    PrevQuantity = 0;
                }
                double Diff = Quantity.Quantity - PrevQuantity;
                if( CswConvert.ToInt32( Diff ) != 0 )
                {
                    string Reason = "Container " + Barcode.Barcode + " quantity changed by: " + Diff + " " + Quantity.CachedUnitName;
                    if( Disposed.Checked == Tristate.True )
                    {
                        Reason += " on disposal.";
                    }
                    _InventoryLevelModified = Mgr.addToCurrentQuantity( Diff, Quantity.UnitId, Reason, Material.RelatedNodeId, Location.SelectedNodeId );
                }
            }
        }

        public CswNbtNodePropDateTime ExpirationDate { get { return ( _CswNbtNode.Properties[ExpirationDatePropertyName] ); } }
        public CswNbtNodePropRelationship Size { get { return ( _CswNbtNode.Properties[SizePropertyName] ); } }
        private void OnSizePropChange( CswNbtNodeProp Prop )
        {
            if( CswTools.IsPrimaryKey( Size.RelatedNodeId ) )
            {
                Size.setReadOnly( value: true, SaveToDb: true );
                Size.setHidden( value: true, SaveToDb: true );
            }
        }

        public CswNbtNodePropButton Request { get { return ( _CswNbtNode.Properties[RequestPropertyName] ); } }
        public CswNbtNodePropButton Dispense { get { return ( _CswNbtNode.Properties[DispensePropertyName] ); } }
        public CswNbtNodePropButton Dispose { get { return ( _CswNbtNode.Properties[DisposePropertyName] ); } }
        public CswNbtNodePropButton Undispose { get { return ( _CswNbtNode.Properties[UndisposePropertyName] ); } }
        public CswNbtNodePropRelationship Owner { get { return ( _CswNbtNode.Properties[OwnerPropertyName] ); } }
        #endregion


    }//CswNbtObjClassContainer

}//namespace ChemSW.Nbt.ObjClasses
