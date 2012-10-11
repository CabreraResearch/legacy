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
        public sealed class PropertyName
        {
            public const string Barcode = "Barcode";
            public const string Material = "Material";
            public const string Location = "Location";
            public const string LocationVerified = "Location Verified";
            public const string Status = "Status";
            public const string Missing = "Missing";
            public const string Disposed = "Disposed";
            public const string SourceContainer = "Source Container";
            public const string Quantity = "Quantity";
            public const string ExpirationDate = "Expiration Date";
            public const string Size = "Size";
            public const string Request = "Request";
            public const string Dispense = "Dispense this Container";
            public const string Dispose = "Dispose this Container";
            public const string Undispose = "Undispose";
            public const string Owner = "Owner";
        }

        private bool _IsDisposed
        {
            get { return Disposed.Checked == Tristate.True; }
        }
        public sealed class RequestMenu
        {
            public const string Move = "Request Move";
            public const string Dispose = "Request Dispose";
            public const string Dispense = "Request Dispense";

            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    Move,
                    Dispose,
                    Dispense
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
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassContainer
        /// </summary>
        public static implicit operator CswNbtObjClassContainer( CswNbtNode Node )
        {
            CswNbtObjClassContainer ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.ContainerClass ) )
            {
                ret = (CswNbtObjClassContainer) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public void updateRequestMenu()
        {
            bool IsDisposed = Disposed.Checked == Tristate.True;
            CswCommaDelimitedString MenuOpts = new CswCommaDelimitedString();
            if( false == IsDisposed )
            {
                MenuOpts.Add( RequestMenu.Move );
                if( Missing.Checked != Tristate.True && Quantity.Quantity > 0 )
                {
                    MenuOpts.Add( RequestMenu.Dispense );
                }
                MenuOpts.Add( RequestMenu.Dispose );
            }
            Request.State = RequestMenu.Move;
            Request.MenuOptions = MenuOpts.ToString();
        }

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            updateRequestMenu();

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
            CswNbtMetaDataObjectClassProp DisposedOCP = ObjectClass.getObjectClassProp( PropertyName.Disposed );

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
                    case PropertyName.Dispose:
                        if( _CswNbtResources.Permit.canContainer( NodeId, CswNbtPermit.NodeTypePermission.Create, _CswNbtResources.Actions[CswNbtActionName.DisposeContainer] ) )
                        {
                            HasPermission = true;
                            DisposeContainer(); //case 26665
                            postChanges( true );
                            ButtonData.Action = NbtButtonAction.refresh;
                        }
                        break;
                    case PropertyName.Undispose:
                        if( _CswNbtResources.Permit.canContainer( NodeId, CswNbtPermit.NodeTypePermission.Create, _CswNbtResources.Actions[CswNbtActionName.UndisposeContainer] ) )
                        {
                            HasPermission = true;
                            UndisposeContainer();
                            postChanges( true );
                            ButtonData.Action = NbtButtonAction.refresh;
                        }
                        break;
                    case PropertyName.Dispense:
                        if( _CswNbtResources.Permit.canContainer( NodeId, CswNbtPermit.NodeTypePermission.Create, _CswNbtResources.Actions[CswNbtActionName.DispenseContainer] ) )
                        {
                            HasPermission = true;
                            //ActionData = this.NodeId.ToString();
                            ButtonData.Data = _getDispenseActionData();
                            ButtonData.Action = NbtButtonAction.dispense;
                        }
                        break;
                    case PropertyName.Request:
                        if( _CswNbtResources.Permit.canContainer( NodeId, CswNbtPermit.NodeTypePermission.Create, _CswNbtResources.Actions[CswNbtActionName.Submit_Request] ) )
                        {
                            CswNbtActSubmitRequest RequestAct = new CswNbtActSubmitRequest( _CswNbtResources, CreateDefaultRequestNode: true );
                            HasPermission = true;

                            CswNbtObjClassRequestItem NodeAsRequestItem = RequestAct.makeContainerRequestItem( this, ButtonData );

                            ButtonData.Data["titleText"] = OCP.PropName + " " + Barcode.Barcode;
                            ButtonData.Data["requestaction"] = ButtonData.SelectedText;
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
            if( OldUnitId != NewUnitId )
            {
                CswNbtUnitConversion ConversionObj = new CswNbtUnitConversion( _CswNbtResources, OldUnitId, NewUnitId, Material.RelatedNodeId );
                convertedValue = ConversionObj.convertUnit( Quantity );
            }
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
                ActionDataObj["precision"] = Quantity.Precision.ToString();
            }
            JObject InitialQuantityObj = _getInitialQuantityJSON();
            ActionDataObj["initialQuantity"] = InitialQuantityObj.ToString();
            bool customBarcodes = CswConvert.ToBoolean( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.custom_barcodes.ToString() ) );
            ActionDataObj["customBarcodes"] = customBarcodes;
            bool netQuantityEnforced = CswConvert.ToBoolean( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.netquantity_enforced.ToString() ) );
            ActionDataObj["netQuantityEnforced"] = netQuantityEnforced;
            return ActionDataObj;
        }

        private JObject _getInitialQuantityJSON()
        {
            JObject InitialQuantityObj = new JObject();
            CswNbtObjClassSize sizeNode = _CswNbtResources.Nodes.GetNode( Size.RelatedNodeId );
            if( null != sizeNode )
            {
                CswNbtNodePropQuantity InitialQuantity = sizeNode.InitialQuantity;
                InitialQuantity.ToJSON( InitialQuantityObj );
                CswNbtObjClassUnitOfMeasure UnitNode = _CswNbtResources.Nodes.GetNode( sizeNode.InitialQuantity.UnitId );
                if( null != UnitNode && 
                    ( UnitNode.UnitType.Value == CswNbtObjClassUnitOfMeasure.UnitTypes.Each.ToString() || 
                    false == CswTools.IsDouble( UnitNode.ConversionFactor.Base ) ) )
                {
                    InitialQuantityObj["unitReadonly"] = "true";
                }
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Cannot dispense container: Container's size is undefined.", "Dispense fail - null Size relationship." );
            }
            return InitialQuantityObj;
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
                    ContDispTransNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.SourceContainer ),
                    NodeId.PrimaryKey.ToString(),
                    CswNbtSubField.SubFieldName.NodeID,
                    false,
                    CswNbtPropFilterSql.PropertyFilterMode.Equals
                    );

                DisposedContainerTransactionsView.AddViewPropertyAndFilter(
                    ParentRelationship,
                    ContDispTransNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.Type ),
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
                CswNbtMetaDataObjectClass RequestItemOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestItemClass );
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

        public CswNbtNodePropBarcode Barcode { get { return ( _CswNbtNode.Properties[PropertyName.Barcode] ); } }
        private void OnBarcodePropChange( CswNbtNodeProp Prop )
        {
            Barcode.setReadOnly( value: false == string.IsNullOrEmpty( Barcode.Barcode ), SaveToDb: true );
        }

        public CswNbtNodePropLocation Location { get { return ( _CswNbtNode.Properties[PropertyName.Location] ); } }
        private void OnLocationPropChange( CswNbtNodeProp Prop )
        {
            CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( Material.RelatedNodeId );
            if( MaterialNode != null )
            {
                // case 24488 - When Location is modified, verify that:
                //  the Material's Storage Compatibility is null,
                //  or the Material's Storage Compatibility is one the selected values in the new Location.
                CswNbtNodePropImageList materialStorageCompatibilty = MaterialNode.Properties[CswNbtObjClassMaterial.PropertyName.StorageCompatibility];
                CswNbtNode locationNode = _CswNbtResources.Nodes.GetNode( Location.SelectedNodeId );
                if( null != locationNode ) //what if the user didn't specify a location?
                {
                    CswNbtNodePropImageList locationStorageCompatibility = locationNode.Properties[CswNbtObjClassLocation.PropertyName.StorageCompatability];
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
                    Int32 PrevLocationId = CswConvert.ToInt32( Node.Properties[PropertyName.Location].GetOriginalPropRowValue( NodeId.Column ) );
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
        public CswNbtNodePropDateTime LocationVerified { get { return ( _CswNbtNode.Properties[PropertyName.LocationVerified] ); } }
        public CswNbtNodePropRelationship Material { get { return ( _CswNbtNode.Properties[PropertyName.Material] ); } }
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
        public CswNbtNodePropList Status { get { return ( _CswNbtNode.Properties[PropertyName.Status] ); } }
        public CswNbtNodePropLogical Missing { get { return ( _CswNbtNode.Properties[PropertyName.Missing] ); } }
        public CswNbtNodePropLogical Disposed { get { return ( _CswNbtNode.Properties[PropertyName.Disposed] ); } }
        private void OnDisposedPropChange( CswNbtNodeProp Prop )
        {
            Disposed.setHidden( value: true, SaveToDb: true );
            if( CswConvert.ToTristate( Disposed.GetOriginalPropRowValue() ) != Disposed.Checked &&
                Disposed.Checked == Tristate.True )
            {
                _updateRequestItems( CswNbtObjClassRequestItem.Types.Dispose );
            }
            updateRequestMenu();
        }
        public CswNbtNodePropRelationship SourceContainer { get { return ( _CswNbtNode.Properties[PropertyName.SourceContainer] ); } }
        private void OnSourceContainerChange( CswNbtNodeProp Prop )
        {
            if( null != SourceContainer.RelatedNodeId && Int32.MinValue != SourceContainer.RelatedNodeId.PrimaryKey )
            {
                SourceContainer.setHidden( value: false, SaveToDb: true );
            }
            else
            {
                SourceContainer.setHidden( value: true, SaveToDb: true );
            }
        }
        public CswNbtNodePropQuantity Quantity { get { return ( _CswNbtNode.Properties[PropertyName.Quantity] ); } }
        private void OnQuantityPropChange( CswNbtNodeProp Prop )
        {
            updateRequestMenu();
            if( false == _InventoryLevelModified )
            {
                CswNbtSdInventoryLevelMgr Mgr = new CswNbtSdInventoryLevelMgr( _CswNbtResources );
                double PrevQuantity = CswConvert.ToDouble( Node.Properties[PropertyName.Quantity].GetOriginalPropRowValue( ( (CswNbtFieldTypeRuleQuantity) _CswNbtResources.MetaData.getFieldTypeRule( Quantity.getFieldType().FieldType ) ).QuantitySubField.Column ) );
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

        public CswNbtNodePropDateTime ExpirationDate { get { return ( _CswNbtNode.Properties[PropertyName.ExpirationDate] ); } }
        public CswNbtNodePropRelationship Size { get { return ( _CswNbtNode.Properties[PropertyName.Size] ); } }
        private void OnSizePropChange( CswNbtNodeProp Prop )
        {
            if( CswTools.IsPrimaryKey( Size.RelatedNodeId ) )
            {
                Size.setReadOnly( value: true, SaveToDb: true );
                Size.setHidden( value: true, SaveToDb: true );
            }
        }

        public CswNbtNodePropButton Request { get { return ( _CswNbtNode.Properties[PropertyName.Request] ); } }
        public CswNbtNodePropButton Dispense { get { return ( _CswNbtNode.Properties[PropertyName.Dispense] ); } }
        public CswNbtNodePropButton Dispose { get { return ( _CswNbtNode.Properties[PropertyName.Dispose] ); } }
        public CswNbtNodePropButton Undispose { get { return ( _CswNbtNode.Properties[PropertyName.Undispose] ); } }
        public CswNbtNodePropRelationship Owner { get { return ( _CswNbtNode.Properties[PropertyName.Owner] ); } }
        #endregion


    }//CswNbtObjClassContainer

}//namespace ChemSW.Nbt.ObjClasses
