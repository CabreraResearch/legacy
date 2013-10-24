using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Conversion;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.UnitsOfMeasure;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassContainer : CswNbtObjClass, ICswNbtPermissionTarget
    {
        #region Properties

        public new sealed class PropertyName : CswNbtObjClass.PropertyName
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
            public const string ContainerFamily = "Container Family";
            public const string ReceiptLot = "Receipt Lot";
            public const string LotControlled = "Lot Controlled";
            public const string Requisitionable = "Requisitionable";
            public const string ContainerGroup = "Container Group";
            public const string LabelFormat = "Label Format";
            public const string ReservedFor = "Reserved For";
            public const string DateCreated = "Date Created";
            public const string StoragePressure = "Storage Pressure";
            public const string StorageTemperature = "Storage Temperature";
            public const string UseType = "Use Type";
            public const string ViewSDS = "View SDS";
            public const string ViewCofA = "View C of A";
            public const string ContainerDispenseTransactions = "Container Dispense Transactions";
            public const string Documents = "Documents";
            public const string SubmittedRequests = "Submitted Requests";
            public const string HomeLocation = "Home Location";
            public const string Notes = "Notes";
            public const string Project = "Project";
            public const string SpecificActivity = "Specific Activity";
            public const string TareQuantity = "Tare Quantity";
            public const string Concentration = "Concentration";
            public const string OpenedDate = "Opened Date";
        }

        #endregion Properties

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
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassContainer
        /// </summary>
        public static implicit operator CswNbtObjClassContainer( CswNbtNode Node )
        {
            CswNbtObjClassContainer ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.ContainerClass ) )
            {
                ret = (CswNbtObjClassContainer) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( IsCopy, OverrideUniqueValidation );
        }

        public override void afterCreateNode()
        {
            // originally case 27330, moved here by case 30647
            Size.setReadOnly( value: true, SaveToDb: true );

            _CswNbtObjClassDefault.afterCreateNode();
        }

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            ViewSDS.State = PropertyName.ViewSDS;
            ViewSDS.MenuOptions = PropertyName.ViewSDS + ",View All";
            ViewCofA.State = PropertyName.ViewCofA;
            ViewCofA.MenuOptions = PropertyName.ViewCofA + ",View All";

            // update Request Menu
            CswCommaDelimitedString MenuOpts = new CswCommaDelimitedString();
            if( CswEnumTristate.True != Disposed.Checked )
            {
                MenuOpts.Add( CswEnumNbtContainerRequestMenu.Move );
                if( CswEnumTristate.True != Missing.Checked && Quantity.Quantity > 0 )
                {
                    MenuOpts.Add( CswEnumNbtContainerRequestMenu.Dispense );
                }
                MenuOpts.Add( CswEnumNbtContainerRequestMenu.Dispose );
            }
            Request.State = CswEnumNbtContainerRequestMenu.Move;
            Request.MenuOptions = MenuOpts.ToString();
            if( DateTime.MinValue == DateCreated.DateTimeValue )
            {
                DateCreated.DateTimeValue = DateTime.Now;
            }

            // Case 28206: Setting Location of Container based on Container Group
            // Note: If the Location and Container Group are both set on a Container,
            // then the Container Group location overrides the user set location.
            if( this.ContainerGroup.wasAnySubFieldModified() &&
                this.ContainerGroup.RelatedNodeId != null &&
                ( this.ContainerGroup.GetOriginalPropRowValue( CswEnumNbtSubFieldName.NodeID ) != this.ContainerGroup.RelatedNodeId.PrimaryKey.ToString() ) )
            {
                CswNbtObjClassContainerGroup NewGroupNode = _CswNbtResources.Nodes[this.ContainerGroup.RelatedNodeId];

                if( null != NewGroupNode )
                {
                    if( CswEnumTristate.True == NewGroupNode.SyncLocation.Checked )
                    {
                        this.Location.SelectedNodeId = NewGroupNode.Location.SelectedNodeId;
                        this.Location.RefreshNodeName();
                    }
                }
            }
            else if( this.Location.wasAnySubFieldModified() &&
                     this.Location.SelectedNodeId != null &&
                     ( this.Location.GetOriginalPropRowValue( CswEnumNbtSubFieldName.NodeID ) != this.Location.SelectedNodeId.PrimaryKey.ToString() ) )
            {
                CswNbtObjClassContainerGroup NewGroupNode = _CswNbtResources.Nodes[this.ContainerGroup.RelatedNodeId];

                if( null != NewGroupNode )
                {
                    if( CswEnumTristate.True == NewGroupNode.SyncLocation.Checked && ( this.Location.SelectedNodeId != NewGroupNode.Location.SelectedNodeId ) )
                    {
                        this.ContainerGroup.RelatedNodeId = null;
                    }
                }
            }

            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );

        }//beforeWriteNode()

        public override void afterWriteNode( bool Creating )
        {
            _CswNbtObjClassDefault.afterWriteNode( Creating );
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            CswNbtSdInventoryLevelMgr Mgr = new CswNbtSdInventoryLevelMgr( _CswNbtResources );
            string Reason = "Container " + Barcode.Barcode + " with quantity " + Quantity.Quantity + " has been deleted.";
            _InventoryLevelModified = Mgr.addToCurrentQuantity( -Quantity.Quantity, Quantity.UnitId, Reason, Material.RelatedNodeId, Location.SelectedNodeId );
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            Dispose.SetOnPropChange( OnDisposePropChange );
            Quantity.SetOnPropChange( OnQuantityPropChange );
            Location.SetOnPropChange( OnLocationPropChange );
            LotControlled.SetOnPropChange( OnLotControlledPropChange );
            Owner.SetOnPropChange( OnOwnerPropChange ); //case 28514

            _toggleAllPropertyStates();

            if( ( _CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Add || _CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Temp || IsTemp ) &&
                false == _CswNbtResources.Permit.can( CswEnumNbtActionName.Receiving ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "You do not have the necessary Action permission to Receive containers.", "You do not have the necessary Action permission to Receive containers." );
            }

            // Find inventory groups for which the user has 'Edit' permission
            Dictionary<CswPrimaryKey, CswPrimaryKey> UserPermissions = _CswNbtResources.CurrentNbtUser.getUserPermissions( CswEnumNbtObjectClass.InventoryGroupPermissionClass, true );
            if( ( _CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Add || _CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Temp || IsTemp ) &&
                UserPermissions.Keys.Count == 0 )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "You do not have the necessary Inventory Group permissions to Receive containers.", "You do not have the necessary Inventory Group permissions to Receive containers." );
            }
            Location.SetOnBeforeRender( delegate( CswNbtNodeProp Prop )
                {
                    Location.View = CswNbtNodePropLocation.LocationPropertyView( _CswNbtResources, Location.NodeTypeProp, null, UserPermissions.Keys ); //Location.SelectedNodeId, UserPermissions.Keys );
                } );

            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            // Disposed == false
            CswNbtMetaDataObjectClassProp DisposedOCP = ObjectClass.getObjectClassProp( PropertyName.Disposed );
            CswNbtViewProperty viewProp = ParentRelationship.View.AddViewProperty( ParentRelationship, DisposedOCP );
            viewProp.ShowInGrid = false;
            ParentRelationship.View.AddViewPropertyFilter( viewProp, FilterMode: CswEnumNbtFilterMode.Equals, Value: CswEnumTristate.False.ToString(), ShowAtRuntime: true );

            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData.NodeTypeProp )
            {
                bool HasPermission = false;
                string OCPPropName = ButtonData.NodeTypeProp.getObjectClassPropName();
                switch( OCPPropName )
                {
                    case PropertyName.Dispose:
                        ButtonData.MultiClick = true;
                        if( canContainer( _CswNbtResources, _CswNbtResources.Actions[CswEnumNbtActionName.DisposeContainer], getPermissionGroupId() ) )
                        {
                            HasPermission = true;
                            DisposeContainer(); //case 26665
                            postChanges( true );
                            ButtonData.Action = CswEnumNbtButtonAction.refresh;
                        }
                        break;
                    case PropertyName.Undispose:
                        ButtonData.MultiClick = true;
                        if( canContainer( _CswNbtResources, _CswNbtResources.Actions[CswEnumNbtActionName.UndisposeContainer], getPermissionGroupId() ) )
                        {
                            HasPermission = true;
                            UndisposeContainer();
                            postChanges( true );
                            ButtonData.Action = CswEnumNbtButtonAction.refresh;
                        }
                        break;
                    case PropertyName.Dispense:
                        if( canContainer( _CswNbtResources, _CswNbtResources.Actions[CswEnumNbtActionName.DispenseContainer], getPermissionGroupId() ) )
                        {
                            HasPermission = true;
                            //ActionData = this.NodeId.ToString();
                            ButtonData.Data = _getDispenseActionData();
                            ButtonData.Action = CswEnumNbtButtonAction.dispense;
                        }
                        break;
                    case PropertyName.Request:
                        if( canContainer( _CswNbtResources, _CswNbtResources.Actions[CswEnumNbtActionName.Submit_Request], getPermissionGroupId() ) )
                        {
                            CswNbtActRequesting RequestAct = new CswNbtActRequesting( _CswNbtResources );
                            HasPermission = true;
                            if( false == CswEnumNbtContainerRequestMenu.Options.Contains( ButtonData.SelectedText, CaseSensitive: false ) )
                            {
                                //Case 30718: Default Option Text "Dispense" != "Request Dispense"
                                if( ButtonData.SelectedText == "Dispense" )
                                {
                                    ButtonData.SelectedText = CswEnumNbtContainerRequestMenu.Dispense;
                                }
                                else
                                {
                                    throw new CswDniException( "Could not find matching Container Button Action for " + ButtonData.SelectedText );
                                }
                            }

                            CswNbtPropertySetRequestItem NodeAsPropSet = RequestAct.makeContainerRequestItem( this, ButtonData );

                            ButtonData.Data["titleText"] = "Add to Cart: " + NodeAsPropSet.Type.Value + " " + Barcode.Barcode;
                            ButtonData.Data["requestaction"] = ButtonData.SelectedText;
                            ButtonData.Data["requestItemProps"] = RequestAct.getRequestItemAddProps( NodeAsPropSet );
                            ButtonData.Data["requestItemNodeTypeId"] = NodeAsPropSet.NodeTypeId;
                        }
                        break;
                    case PropertyName.ContainerFamily:
                        HasPermission = true;
                        CswNbtView containerFamilyView = GetFamilyView();
                        containerFamilyView.SaveToCache( false );

                        ButtonData.Action = CswEnumNbtButtonAction.loadView;
                        ButtonData.Data["viewid"] = containerFamilyView.SessionViewId.ToString();
                        ButtonData.Data["viewmode"] = containerFamilyView.ViewMode.ToString();
                        ButtonData.Data["type"] = "view";
                        break;
                    case PropertyName.ViewSDS:
                        HasPermission = true;

                        CswNbtPropertySetMaterial material = _CswNbtResources.Nodes[Material.RelatedNodeId];
                        if( null != material && material.ObjectClass.ObjectClass == CswEnumNbtObjectClass.ChemicalClass )
                        {
                            CswNbtObjClassChemical chemical = material.Node;
                            chemical.GetMatchingSDSForCurrentUser( ButtonData );
                        }
                        break;
                    case PropertyName.ViewCofA:
                        HasPermission = true;
                        CswNbtObjClassReceiptLot ReceiptLotNode = _CswNbtResources.Nodes[ReceiptLot.RelatedNodeId];
                        if( null != ReceiptLotNode )
                        {
                            ReceiptLotNode.getCofA( ButtonData );
                        }
                        break;
                    case CswNbtObjClass.PropertyName.Save:
                        HasPermission = true;
                        break;

                }
                if( false == HasPermission )
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "You do not have permission to the " + OCPPropName + " action.", "You do not have permission to the " + OCPPropName + " action." );
                }
            }
            return true;
        }
        #endregion Inherited Events

        #region Custom Logic

        public CswPrimaryKey getPermissionGroupId()
        {
            CswPrimaryKey ret = null;
            CswNbtObjClassLocation LocationNode = _CswNbtResources.Nodes[Location.SelectedNodeId];
            if( null != LocationNode )
            {
                ret = LocationNode.InventoryGroup.RelatedNodeId;
            }
            return ret;
        }

        /// <summary>
        /// Check container Action permissions based on InventoryGroup.
        /// </summary>
        public static bool canContainer( CswNbtResources CswNbtResources, CswNbtAction Action, CswPrimaryKey InventoryGroupId, ICswNbtUser User = null )
        {
            if( null == Action )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "You do not have appropriate permissions", "canContainer called with null Action" );
            }
            return _canContainer( CswNbtResources, Action, InventoryGroupId, User );
        }

        private static bool _canContainer( CswNbtResources CswNbtResources, CswNbtAction Action, CswPrimaryKey InventoryGroupId, ICswNbtUser User )
        {
            bool ret = true;

            if( null == User )
            {
                User = CswNbtResources.CurrentNbtUser;
            }
            if( false == ( User is CswNbtSystemUser ) )
            {
                ret = false;

                if( CswTools.IsPrimaryKey( InventoryGroupId ) )
                {
                    CswNbtObjClassInventoryGroupPermission PermNode = (CswNbtObjClassInventoryGroupPermission) User.getPermissionForGroup( InventoryGroupId );
                    if( null != PermNode )
                    {
                        ret = PermNode.canAction( Action );
                    }
                }
                else
                {
                    ret = true;
                }
            }
            return ret;
        }

        /// <summary>
        /// Checks permission and disposes a container (does not post changes!)
        /// </summary>
        public void DisposeContainer( bool OverridePermissions = false )
        {
            if( OverridePermissions || canContainer( _CswNbtResources, _CswNbtResources.Actions[CswEnumNbtActionName.DisposeContainer], getPermissionGroupId() ) )
            {
                _createContainerTransactionNode( CswEnumNbtContainerDispenseType.Dispose, -this.Quantity.Quantity, this.Quantity.UnitId, SrcContainer: this );
                this.Quantity.Quantity = 0;
                this.Disposed.Checked = CswEnumTristate.True;
                CreateContainerLocationNode( CswEnumNbtContainerLocationTypeOptions.Dispose );
                _CswNbtNode.IconFileNameOverride = "x.png";
                _CswNbtNode.Searchable = false;
            }
        }

        /// <summary>
        /// Checks permission and undisposes a container
        /// </summary>
        public void UndisposeContainer( bool OverridePermissions = false, bool CreateContainerLocation = true )
        {

            if( OverridePermissions || canContainer( _CswNbtResources, _CswNbtResources.Actions[CswEnumNbtActionName.UndisposeContainer], getPermissionGroupId() ) )
            {
                CswNbtMetaDataNodeType ContDispTransNT = _CswNbtResources.MetaData.getNodeType( "Container Dispense Transaction" );
                CswNbtObjClassContainerDispenseTransaction ContDispTransNode = _getMostRecentDisposeTransaction( ContDispTransNT );

                if( ContDispTransNode != null )
                {
                    this.Quantity.Quantity = -ContDispTransNode.QuantityDispensed.Quantity;
                    this.Quantity.UnitId = ContDispTransNode.QuantityDispensed.UnitId;
                    ContDispTransNode.Node.delete( OverridePermissions: true );
                }
                this.Disposed.Checked = CswEnumTristate.False;

                if( CreateContainerLocation )
                {
                    CreateContainerLocationNode( CswEnumNbtContainerLocationTypeOptions.Undispose );
                }
                _CswNbtNode.IconFileNameOverride = "";
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
        public void DispenseOut( CswEnumNbtContainerDispenseType DispenseType, double QuantityToDeduct, CswPrimaryKey UnitId,
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
            CreateContainerLocationNode( CswEnumNbtContainerLocationTypeOptions.Dispense );
        } // DispenseOut()

        /// <summary>
        /// Dispense into this container.  
        /// </summary>
        /// <param name="DispenseType"></param>
        /// <param name="QuantityToAdd">Positive quantity to add</param>
        /// <param name="UnitId"></param>
        /// <param name="RequestItemId"></param>
        /// <param name="SourceContainer"></param>
        public void DispenseIn( CswEnumNbtContainerDispenseType DispenseType, double QuantityToAdd, CswPrimaryKey UnitId,
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
            CswEnumNbtContainerLocationTypeOptions ContainerLocationType =
                SourceContainer == null ? CswEnumNbtContainerLocationTypeOptions.Receipt
                                        : CswEnumNbtContainerLocationTypeOptions.Dispense;
            CreateContainerLocationNode( ContainerLocationType );
        } // DispenseIn()

        /// <summary>
        /// Gets a tree view of this containers family
        /// </summary>
        /// <returns></returns>
        public CswNbtView GetFamilyView()
        {
            CswNbtMetaDataObjectClass containerOC = _CswNbtResources.MetaData.getObjectClass( this.ObjectClass.ObjectClassId );
            CswNbtMetaDataObjectClassProp barcodeOCP = containerOC.getObjectClassProp( PropertyName.Barcode );
            CswNbtMetaDataObjectClassProp sourceContainerOCP = containerOC.getObjectClassProp( PropertyName.SourceContainer );
            int maxGenerations = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.container_max_depth ) );

            CswNbtObjClassContainer eldestContainer = FindEldestContainer();

            CswNbtView familyView = new CswNbtView( _CswNbtResources );
            familyView.ViewName = "Container Family for " + Barcode.Barcode;
            CswNbtViewRelationship parent = familyView.AddViewRelationship( containerOC, false ); //only this container should be at the top
            parent.NodeIdsToFilterIn.Add( eldestContainer.NodeId );

            _getFamilyView( ref familyView, parent, 1, maxGenerations, sourceContainerOCP, barcodeOCP );

            return familyView;
        }

        /// <summary>
        /// Gets the forerunner container from which all family members of this container derive from
        /// </summary>
        public CswNbtObjClassContainer FindEldestContainer()
        {
            CswNbtObjClassContainer eldestContainer = this;
            while( null != eldestContainer.SourceContainer.RelatedNodeId )
            {
                if( null != eldestContainer.SourceContainer.RelatedNodeId )
                {
                    eldestContainer = _CswNbtResources.Nodes.GetNode( eldestContainer.SourceContainer.RelatedNodeId );
                }
            }
            return eldestContainer;
        }

        /// <summary>
        /// Create a new ContainerLocation node. 
        /// *Note: The NewLocationBarcode and ContainerBarcode parameters are necessary only if the Type = ReconcileScans. They
        /// are currently only used for the reconcile data from the CISPro/NBT CORE mobile app.
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="NewLocationBarcode"></param>
        /// <param name="ContainerBarcode"></param>
        public void CreateContainerLocationNode( CswEnumNbtContainerLocationTypeOptions Type, string NewLocationBarcode = "", string ContainerBarcode = "" )
        {
            CswNbtMetaDataNodeType ContLocNT = _CswNbtResources.MetaData.getNodeType( "Container Location" );
            if( ContLocNT != null )
            {
                _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ContLocNT.NodeTypeId, delegate( CswNbtNode NewNode )
                   {
                       CswNbtObjClassContainerLocation ContLocNode = NewNode;
                       ContLocNode.Type.Value = Type.ToString();
                       ContLocNode.Container.RelatedNodeId = NodeId;
                       if( null != Location )
                       {
                           if( Type != CswEnumNbtContainerLocationTypeOptions.ReconcileScans )
                           {
                               ContLocNode.Location.SelectedNodeId = Location.SelectedNodeId;
                               ContLocNode.Location.CachedNodeName = Location.CachedNodeName;
                               ContLocNode.Location.CachedPath = Location.CachedPath;
                           }
                           else
                           {
                               ContLocNode.LocationScan.Text = NewLocationBarcode;
                               ContLocNode.ContainerScan.Text = ContainerBarcode;
                           }
                       }
                       ContLocNode.ActionApplied.Checked = CswEnumTristate.False;
                       ContLocNode.ScanDate.DateTimeValue = DateTime.Now;
                       ContLocNode.User.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
                       //ContLocNode.postChanges( false );
                   } );
                LocationVerified.DateTimeValue = DateTime.Now;
                if( Missing.Checked == CswEnumTristate.True )
                {
                    Missing.Checked = CswEnumTristate.False;
                }
            }
        }

        #region CISPro/NBT CORE Mobile App Operations

        /// <summary>
        /// Update the location of a container.
        /// </summary>
        public void MoveContainer( CswPrimaryKey newLocation )
        {
            this.Location.SelectedNodeId = newLocation;
            this.Location.RefreshNodeName();
        }

        /// <summary>
        /// Update the owner of a container.
        /// </summary>
        public void UpdateOwner( CswPrimaryKey newOwner )
        {
            this.Owner.RelatedNodeId = newOwner;
            this.Owner.RefreshNodeName();
        }

        public void UpdateOwner( CswNbtObjClassUser userNode )
        {
            UpdateOwner( userNode.NodeId );
        }

        /// <summary>
        /// Update the owner and location of a container.
        /// </summary>
        public void TransferContainer( CswPrimaryKey newOwner )
        {
            if( CswTools.IsPrimaryKey( newOwner ) )
            {
                CswNbtObjClassUser userNode = _CswNbtResources.Nodes[newOwner];
                if( null != userNode )
                {
                    TransferContainer( userNode );
                }
            }
        }

        public void TransferContainer( CswNbtObjClassUser newOwner )
        {
            Owner.RelatedNodeId = newOwner.NodeId;
            Owner.RefreshNodeName();

            Location.SelectedNodeId = newOwner.DefaultLocationId;
            Location.SyncGestalt();
            Location.RefreshNodeName();
        }

        #endregion  CISPro/NBT CORE Mobile App Operations

        #endregion Custom Logic

        #region Private Helper Methods

        private double _getDispenseAmountInProperUnits( double Amount, CswPrimaryKey OldUnitId, CswPrimaryKey NewUnitId )
        {
            double convertedValue = Amount;
            if( OldUnitId != NewUnitId )
            {
                CswNbtUnitConversion ConversionObj = new CswNbtUnitConversion( _CswNbtResources, OldUnitId, NewUnitId, Material.RelatedNodeId );
                convertedValue = ConversionObj.convertUnit( Amount );
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
            ActionDataObj["materialid"] = Material.RelatedNodeId.ToString();
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
            ActionDataObj["initialQuantity"] = InitialQuantityObj;
            bool customBarcodes = CswConvert.ToBoolean( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.custom_barcodes.ToString() ) );
            ActionDataObj["customBarcodes"] = customBarcodes;
            bool netQuantityEnforced = CswConvert.ToBoolean( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.netquantity_enforced.ToString() ) );
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
                CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( Material.RelatedNodeId );
                CswNbtUnitViewBuilder UnitBuilder = new CswNbtUnitViewBuilder( _CswNbtResources );
                UnitBuilder.setQuantityUnitOfMeasureView( MaterialNode, InitialQuantity, true );
                InitialQuantity.ToJSON( InitialQuantityObj );
                CswNbtObjClassUnitOfMeasure UnitNode = _CswNbtResources.Nodes.GetNode( sizeNode.InitialQuantity.UnitId );
                if( null != UnitNode &&
                    ( UnitNode.UnitType.Value == CswEnumNbtUnitTypes.Each.ToString() ||
                    false == CswTools.IsDouble( UnitNode.ConversionFactor.Base ) ) )
                {
                    InitialQuantityObj["isUnitReadOnly"] = "true";
                }
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Cannot dispense container: Container's size is undefined.", "Dispense fail - null Size relationship." );
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
                    CswEnumNbtSubFieldName.NodeID,
                    false,
                    CswEnumNbtFilterMode.Equals
                    );

                DisposedContainerTransactionsView.AddViewPropertyAndFilter(
                    ParentRelationship,
                    ContDispTransNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.Type ),
                    CswEnumNbtContainerDispenseType.Dispose.ToString(),
                    CswEnumNbtSubFieldName.Value,
                    false,
                    CswEnumNbtFilterMode.Equals
                    );

                ICswNbtTree DispenseTransactionTree = _CswNbtResources.Trees.getTreeFromView( DisposedContainerTransactionsView, false, true, false );
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
        private void _createContainerTransactionNode( CswEnumNbtContainerDispenseType DispenseType, double Amount, CswPrimaryKey UnitId, CswPrimaryKey RequestItemId = null,
                                                      CswNbtObjClassContainer SrcContainer = null, CswNbtObjClassContainer DestinationContainer = null )
        {
            CswNbtMetaDataNodeType ContDispTransNT = _CswNbtResources.MetaData.getNodeType( "Container Dispense Transaction" );
            if( ContDispTransNT != null )
            {
                _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ContDispTransNT.NodeTypeId, delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassContainerDispenseTransaction ContDispTransNode = NewNode;
                        if( SrcContainer != null )
                        {
                            ContDispTransNode.SourceContainer.RelatedNodeId = SrcContainer.NodeId;
                            ContDispTransNode.RemainingSourceContainerQuantity.Quantity = SrcContainer.Quantity.Quantity;
                            if( DispenseType == CswEnumNbtContainerDispenseType.Dispose )
                            {
                                ContDispTransNode.RemainingSourceContainerQuantity.Quantity = 0;
                            }
                            ContDispTransNode.RemainingSourceContainerQuantity.UnitId = SrcContainer.Quantity.UnitId;
                        }
                        if( DestinationContainer != null )
                        {
                            ContDispTransNode.DestinationContainer.RelatedNodeId = DestinationContainer.NodeId;
                        }
                        ContDispTransNode.Dispenser.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
                        ContDispTransNode.QuantityDispensed.Quantity = Amount;
                        ContDispTransNode.QuantityDispensed.UnitId = UnitId;
                        ContDispTransNode.Type.Value = DispenseType.ToString();
                        ContDispTransNode.DispensedDate.DateTimeValue = DateTime.Now;
                        if( null != RequestItemId && Int32.MinValue != RequestItemId.PrimaryKey )
                        {
                            ContDispTransNode.RequestItem.RelatedNodeId = RequestItemId;
                        }
                        //ContDispTransNode.postChanges( false );
                    } );
            } // if( ContDispTransNT != null )
        } // _createContainerTransactionNode

        /// <summary>
        /// Sets up onBeforeRender events in order to:
        /// Toggle the ReadOnly state of all non-button properties according to the Disposed state of the Container.
        /// Toggle the Hidden state of all button properties according to Disposed state, permissions and business logic
        /// Toggle the ReadOnly state of Expiration Date according to the Material's ContainerExpiraionLock value.
        /// </summary>
        /// <remarks>
        /// This is the ONLY place that setReadonly or setHidden should be used!
        /// </remarks>
        private void _toggleAllPropertyStates()
        {
            //Old comment: //SaveToDb true is necessary to override what's in the db even if it isn't actually saved as part of this request
            //Handle the raw data condition first.

            foreach( CswNbtNodePropWrapper prop in _CswNbtNode.Properties )
            {
                prop.SetOnBeforeRender( delegate( CswNbtNodeProp p )
                    {
                        bool IsDisposed = ( Disposed.Checked == CswEnumTristate.True );
                        CswPrimaryKey InventoryGroupId = getPermissionGroupId();

                        if( p.getFieldType().FieldType == CswEnumNbtFieldType.Button )
                        {
                            // BUTTONS
                            bool isHidden = false;
                            switch( p.ObjectClassPropName )
                            {
                                case PropertyName.Undispose:
                                    isHidden = ( false == canContainer( _CswNbtResources, _CswNbtResources.Actions[CswEnumNbtActionName.UndisposeContainer], InventoryGroupId ) )
                                               || false == IsDisposed; //Hide the Undispose button when the Container is not disposed
                                    p.setHidden( value: isHidden, SaveToDb: false );
                                    break;
                                case PropertyName.Dispose:
                                    isHidden = ( false == canContainer( _CswNbtResources, _CswNbtResources.Actions[CswEnumNbtActionName.DisposeContainer], InventoryGroupId ) )
                                               || IsDisposed;
                                    p.setHidden( value: isHidden, SaveToDb: false );
                                    break;
                                case PropertyName.Request:
                                    isHidden = ( false == canContainer( _CswNbtResources, _CswNbtResources.Actions[CswEnumNbtActionName.Submit_Request], InventoryGroupId ) )
                                               || IsDisposed
                                               || ( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.MLM )
                                                    && Requisitionable.Checked == CswEnumTristate.False );
                                    p.setHidden( value: isHidden, SaveToDb: false );
                                    break;
                                case PropertyName.Dispense:
                                    isHidden = ( false == canContainer( _CswNbtResources, _CswNbtResources.Actions[CswEnumNbtActionName.DispenseContainer], InventoryGroupId ) )
                                               || IsDisposed;
                                    p.setHidden( value: isHidden, SaveToDb: false );
                                    break;
                                case PropertyName.ViewSDS:
                                    CswNbtPropertySetMaterial material = _CswNbtResources.Nodes[Material.RelatedNodeId];
                                    isHidden = ( null == material
                                                 || material.ObjectClass.ObjectClass != CswEnumNbtObjectClass.ChemicalClass
                                                 || false == CswNbtObjClassSDSDocument.materialHasActiveSDS( _CswNbtResources, Material.RelatedNodeId )
                                                 || _CswNbtResources.MetaData.NodeTypeLayout.getPropsNotInLayout( ( (CswNbtObjClassChemical) material.Node ).NodeType,
                                                                                                                  Int32.MinValue,
                                                                                                                  CswEnumNbtLayoutType.Edit )
                                                                    .Contains( ( (CswNbtObjClassChemical) material.Node ).ViewSDS.NodeTypeProp )
                                        //       || IsDisposed   actually, this one we can show.
                                               );
                                    p.setHidden( value: isHidden, SaveToDb: false );
                                    break;
                                case PropertyName.ViewCofA:
                                    isHidden = ( false == CswNbtObjClassCofADocument.receiptLotHasActiveCofA( _CswNbtResources, ReceiptLot.RelatedNodeId ) );
                                    p.setHidden( value: isHidden, SaveToDb: false );
                                    break;
                                default:
                                    p.setHidden( value: IsDisposed, SaveToDb: false );
                                    break;
                            } // switch
                        } // if( prop.getFieldType().FieldType == CswEnumNbtFieldType.Button )
                        else
                        {
                            // NOT BUTTONS
                            switch( p.ObjectClassPropName )
                            {
                                case PropertyName.Barcode:
                                    p.setReadOnly( value: false == string.IsNullOrEmpty( Barcode.Barcode ), SaveToDb: true );
                                    break;
                                case PropertyName.ExpirationDate:
                                    if( CswTools.IsPrimaryKey( Material.RelatedNodeId ) )
                                    {
                                        CswNbtPropertySetMaterial MaterialNode = _CswNbtResources.Nodes[Material.RelatedNodeId];
                                        p.setReadOnly( MaterialNode.ContainerExpirationLocked.Checked == CswEnumTristate.True, SaveToDb: false );
                                    }
                                    break;
                                case PropertyName.SourceContainer:
                                    bool isHidden = ( false == CswTools.IsPrimaryKey( SourceContainer.RelatedNodeId ) );
                                    p.setHidden( value: isHidden, SaveToDb: true );

                                    if( CswTools.IsPrimaryKey( Material.RelatedNodeId ) )
                                    {
                                        SourceContainer.setReadOnly( value: true, SaveToDb: true );
                                    }
                                    break;
                            } // switch

                            // This overrides all of the above!
                            p.setReadOnly( IsDisposed, SaveToDb: true );

                        } // if-else( prop.getFieldType().FieldType == CswEnumNbtFieldType.Button )
                    } );
            } // foreach
        } //_toggleAllPropertyStates()


        private bool _checkStorageCompatibility()
        {
            bool Ret = true;

            CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( Material.RelatedNodeId );
            if( MaterialNode != null && MaterialNode.ObjClass.ObjectClass.ObjectClass == CswEnumNbtObjectClass.ChemicalClass )
            {
                // case 24488 - When Location is modified, verify that:
                //  the Material's Storage Compatibility is null,
                //  or the Material's Storage Compatibility is one the selected values in the new Location.
                CswNbtNodePropImageList materialStorageCompatibilty = MaterialNode.Properties[CswNbtObjClassChemical.PropertyName.StorageCompatibility];
                CswNbtNode locationNode = _CswNbtResources.Nodes.GetNode( Location.SelectedNodeId );
                if( null != locationNode ) //what if the user didn't specify a location?
                {
                    CswNbtNodePropImageList locationStorageCompatibility = locationNode.Properties[CswNbtObjClassLocation.PropertyName.StorageCompatibility];
                    if( false == _isStorageCompatible( materialStorageCompatibilty.Value, locationStorageCompatibility.Value ) )
                    {
                        Ret = false;
                    }
                }
            }

            return Ret;
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

        /// <summary>
        /// private worker method to get younger generations of containers in the family.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="parent"></param>
        /// <param name="children"></param>
        /// <param name="generation"></param>
        /// <param name="maxGenerations"></param>
        /// <param name="sourceContainerOCP"></param>
        /// <param name="barcodeOCP"></param>
        private void _getFamilyView( ref CswNbtView view, CswNbtViewRelationship parent, int generation, int maxGenerations,
            CswNbtMetaDataObjectClassProp sourceContainerOCP, CswNbtMetaDataObjectClassProp barcodeOCP )
        {
            if( generation <= maxGenerations )
            {
                CswNbtViewRelationship generationXParent = view.AddViewRelationship( parent, CswEnumNbtViewPropOwnerType.Second, sourceContainerOCP, false );
                view.AddViewProperty( generationXParent, sourceContainerOCP );
                _getFamilyView( ref view, generationXParent, generation + 1, maxGenerations, sourceContainerOCP, barcodeOCP );
            }
        }

        /// <summary>
        /// True if the Inventory Group of this Location is in the collection of Inventory Groups to which the current user has Edit permission
        /// </summary>
        /// <returns></returns>
        public bool isLocationInAccessibleInventoryGroup( CswPrimaryKey LocationId )
        {
            bool ret = false;
            CswNbtObjClassLocation ThisLocation = _CswNbtResources.Nodes[LocationId];
            if( null != ThisLocation )
            {
                CswNbtPropertySetPermission PermGrp = _CswNbtResources.CurrentNbtUser.getPermissionForGroup( ThisLocation.InventoryGroup.RelatedNodeId );
                if( null != PermGrp )
                {
                    ret = ( PermGrp.Edit.Checked == CswEnumTristate.True );
                }
            }
            return ret;
        }

        #endregion

        #region Object class specific properties

        private void _updateRequestItems( string RequestItemType )
        {
            if( RequestItemType == CswNbtObjClassRequestContainerUpdate.Types.Move ||
             RequestItemType == CswNbtObjClassRequestContainerUpdate.Types.Dispose )
            {
                CswNbtView RequestItemView = new CswNbtView( _CswNbtResources );
                CswNbtMetaDataObjectClass RequestItemOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestContainerUpdateClass );
                CswNbtViewRelationship RiRelationship = RequestItemView.AddViewRelationship( RequestItemOc, false );
                CswNbtMetaDataObjectClassProp StatusOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Status );
                CswNbtMetaDataObjectClassProp ContainerOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Container );
                CswNbtMetaDataObjectClassProp TypeOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Type );

                RequestItemView.AddViewPropertyAndFilter( RiRelationship, StatusOcp, CswNbtObjClassRequestContainerUpdate.Statuses.Submitted );
                RequestItemView.AddViewPropertyAndFilter( RiRelationship, ContainerOcp, SubFieldName: CswEnumNbtSubFieldName.NodeID, Value: NodeId.PrimaryKey.ToString() );
                RequestItemView.AddViewPropertyAndFilter( RiRelationship, TypeOcp, RequestItemType );

                if( RequestItemType == CswNbtObjClassRequestContainerUpdate.Types.Move )
                {
                    CswNbtMetaDataObjectClassProp LocationOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Location );
                    RequestItemView.AddViewPropertyAndFilter( RiRelationship, LocationOcp, SubFieldName: CswEnumNbtSubFieldName.NodeID, Value: Location.SelectedNodeId.PrimaryKey.ToString() );
                }

                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( RequestItemView, IncludeSystemNodes: false, RequireViewPermissions: false, IncludeHiddenNodes: false );
                if( Tree.getChildNodeCount() > 0 )
                {
                    for( Int32 N = 0; N < Tree.getChildNodeCount(); N += 1 )
                    {
                        Tree.goToNthChild( N );
                        CswNbtObjClassRequestContainerUpdate NodeAsRequestItem = Tree.getNodeForCurrentPosition();
                        if( null != NodeAsRequestItem )
                        {
                            switch( RequestItemType )
                            {
                                case CswNbtObjClassRequestContainerUpdate.Types.Move:
                                    NodeAsRequestItem.Status.Value = CswNbtObjClassRequestContainerUpdate.Statuses.Moved;
                                    break;
                                case CswNbtObjClassRequestContainerUpdate.Types.Dispose:
                                    NodeAsRequestItem.Status.Value = CswNbtObjClassRequestContainerUpdate.Statuses.Disposed;
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
        public CswNbtNodePropLocation Location { get { return ( _CswNbtNode.Properties[PropertyName.Location] ); } }
        private void OnLocationPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            // This method is being called multiple times so this was added
            if( CswTools.IsPrimaryKey( Location.SelectedNodeId ) && ( Location.GetOriginalPropRowValue() != Location.SelectedNodeId.ToString() ) )
            {
                if( false == _checkStorageCompatibility() )
                {
                    throw new CswDniException( CswEnumErrorType.Warning,
                                              "Storage compatibilities do not match, cannot move this container to specified location. Please choose another location.",
                                              "Storage compatibilities do not match, cannot move this container to specified location. Please choose another location." );
                }

                if( CswTools.IsPrimaryKey( Location.SelectedNodeId ) &&
                    false == string.IsNullOrEmpty( Location.CachedNodeName ) &&
                    Location.CachedNodeName != CswNbtNodePropLocation.GetTopLevelName( _CswNbtResources ) )
                {
                    if( false == _InventoryLevelModified &&
                        CswConvert.ToInt32( Quantity.Quantity ) != 0 )
                    {
                        CswNbtSdInventoryLevelMgr Mgr = new CswNbtSdInventoryLevelMgr( _CswNbtResources );
                        CswNbtSubField NodeId = ( (CswNbtFieldTypeRuleLocation) _CswNbtResources.MetaData.getFieldTypeRule( Location.getFieldTypeValue() ) ).NodeIdSubField;
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
                    if( false == Creating )
                    {
                        _updateRequestItems( CswNbtObjClassRequestContainerUpdate.Types.Move );
                    }
                }
                if( null != Location.SelectedNodeId )
                {
                    if( String.IsNullOrEmpty( Location.GetOriginalPropRowValue() ) )
                    {
                        CswNbtObjClassLocation LocNode = _CswNbtResources.Nodes.GetNode( Location.SelectedNodeId );
                        CswNbtObjClassInventoryGroup InvGroupNode = _CswNbtResources.Nodes.GetNode( LocNode.InventoryGroup.RelatedNodeId );
                        if( null != InvGroupNode )
                        {
                            LotControlled.Checked = InvGroupNode.Central.Checked == CswEnumTristate.True ? CswEnumTristate.True : CswEnumTristate.False;
                        }
                    }
                    else if( Location.GetOriginalPropRowValue() != Location.CachedNodeName && Location.CreateContainerLocation )
                    {
                        CreateContainerLocationNode( CswEnumNbtContainerLocationTypeOptions.Move );
                    }
                }
            }

        }
        public CswNbtNodePropDateTime LocationVerified { get { return ( _CswNbtNode.Properties[PropertyName.LocationVerified] ); } }
        public CswNbtNodePropRelationship Material { get { return ( _CswNbtNode.Properties[PropertyName.Material] ); } }
        public CswNbtNodePropList Status { get { return ( _CswNbtNode.Properties[PropertyName.Status] ); } }
        public CswNbtNodePropLogical Missing { get { return ( _CswNbtNode.Properties[PropertyName.Missing] ); } }
        public CswNbtNodePropLogical Disposed { get { return ( _CswNbtNode.Properties[PropertyName.Disposed] ); } }
        private void OnDisposePropChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( CswConvert.ToTristate( Disposed.GetOriginalPropRowValue() ) != Disposed.Checked &&
                Disposed.Checked == CswEnumTristate.True )
            {
                _updateRequestItems( CswNbtObjClassRequestContainerUpdate.Types.Dispose );
            }
        }
        public CswNbtNodePropRelationship SourceContainer { get { return ( _CswNbtNode.Properties[PropertyName.SourceContainer] ); } }
        public CswNbtNodePropQuantity Quantity { get { return ( _CswNbtNode.Properties[PropertyName.Quantity] ); } }
        private void OnQuantityPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( false == _InventoryLevelModified )
            {
                CswNbtSdInventoryLevelMgr Mgr = new CswNbtSdInventoryLevelMgr( _CswNbtResources );
                double PrevQuantity = CswConvert.ToDouble( Node.Properties[PropertyName.Quantity].GetOriginalPropRowValue( ( (CswNbtFieldTypeRuleQuantity) _CswNbtResources.MetaData.getFieldTypeRule( Quantity.getFieldTypeValue() ) ).QuantitySubField.Column ) );
                if( false == CswTools.IsDouble( PrevQuantity ) )
                {
                    PrevQuantity = 0;
                }
                double Diff = Quantity.Quantity - PrevQuantity;
                if( CswConvert.ToInt32( Diff ) != 0 )
                {
                    string Reason = "Container " + Barcode.Barcode + " quantity changed by: " + Diff + " " + Quantity.CachedUnitName;
                    if( Disposed.Checked == CswEnumTristate.True )
                    {
                        Reason += " on disposal.";
                    }
                    _InventoryLevelModified = Mgr.addToCurrentQuantity( Diff, Quantity.UnitId, Reason, Material.RelatedNodeId, Location.SelectedNodeId );
                }
            }
        }

        public CswNbtNodePropDateTime ExpirationDate { get { return ( _CswNbtNode.Properties[PropertyName.ExpirationDate] ); } }

        public CswNbtNodePropRelationship Size { get { return ( _CswNbtNode.Properties[PropertyName.Size] ); } }
        public CswNbtNodePropButton Request { get { return ( _CswNbtNode.Properties[PropertyName.Request] ); } }
        public CswNbtNodePropButton Dispense { get { return ( _CswNbtNode.Properties[PropertyName.Dispense] ); } }
        public CswNbtNodePropButton Dispose { get { return ( _CswNbtNode.Properties[PropertyName.Dispose] ); } }
        public CswNbtNodePropButton Undispose { get { return ( _CswNbtNode.Properties[PropertyName.Undispose] ); } }
        public CswNbtNodePropRelationship Owner { get { return ( _CswNbtNode.Properties[PropertyName.Owner] ); } }
        private void OnOwnerPropChange( CswNbtNodeProp Prop, bool Creating ) //case 28514
        {
            // Case 28800 - Fixes received container's location always defaulting to current user
            if( CswTools.IsPrimaryKey( Owner.RelatedNodeId ) && ( Owner.GetOriginalPropRowValue() != Owner.Gestalt ) )
            {
                // Because we don't want this logic to fire all the time; only in 
                // the Receive Material Wizard where the Container Node is Temp.
                // -- This isn't the best fix but it works for now
                if( this.IsTemp )
                {
                    if( CswTools.IsPrimaryKey( Owner.RelatedNodeId ) )
                    {
                        CswNbtObjClassUser CurrentOwnerNode = _CswNbtResources.Nodes.GetNode( Owner.RelatedNodeId );
                        if( null != CurrentOwnerNode )
                        {
                            if( isLocationInAccessibleInventoryGroup( CurrentOwnerNode.DefaultLocationId ) )
                            {
                                Location.SelectedNodeId = CurrentOwnerNode.DefaultLocationId;
                                Location.RefreshNodeName();
                            }
                        }
                    }
                }
            }

        }
        public CswNbtNodePropButton ContainerFamily { get { return ( _CswNbtNode.Properties[PropertyName.ContainerFamily] ); } }
        public CswNbtNodePropRelationship ReceiptLot { get { return ( _CswNbtNode.Properties[PropertyName.ReceiptLot] ); } }
        public CswNbtNodePropLogical LotControlled { get { return ( _CswNbtNode.Properties[PropertyName.LotControlled] ); } }
        private void OnLotControlledPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( LotControlled.Checked == CswEnumTristate.True )
            {
                //DispenseForCertificate.RelatedNodeId = null;//TODO - uncomment when DispenseForCertificate is created
                Status.Value = String.Empty;
            }
            else if( LotControlled.Checked == CswEnumTristate.False )
            {
                //TODO - uncomment this if-else condition when Certificates have been implemented
                //if( null != Certificate.RelatedNodeId )
                //{ 
                //    CswNbtObjClassCertificate CertificateNode = _CswNbtResources.Nodes.GetNode( Certificate.RelatedNodeId );
                //    if( null != CertificateNode )
                //    {
                //        Status.Value = CertificateNode.Status.Value;
                //        ExpirationDate.DateTimeValue = CertificateNode.ExpirationDate.DateTimeValue;
                //    }
                //}
                //else
                //{
                Status.Value = CswEnumNbtContainerStatuses.LabUseOnly;
                CswNbtObjClassReceiptLot ReceiptLotNode = _CswNbtResources.Nodes.GetNode( ReceiptLot.RelatedNodeId );
                if( null != ReceiptLotNode && ReceiptLotNode.ExpirationDate.DateTimeValue != DateTime.MinValue )
                {
                    ExpirationDate.DateTimeValue = ReceiptLotNode.ExpirationDate.DateTimeValue;
                }
                //}
            }
        }
        public CswNbtNodePropRelationship ContainerGroup { get { return ( _CswNbtNode.Properties[PropertyName.ContainerGroup] ); } }
        public CswNbtNodePropLogical Requisitionable { get { return ( _CswNbtNode.Properties[PropertyName.Requisitionable] ); } }
        public CswNbtNodePropRelationship LabelFormat { get { return ( _CswNbtNode.Properties[PropertyName.LabelFormat] ); } }
        public CswNbtNodePropRelationship ReservedFor { get { return ( _CswNbtNode.Properties[PropertyName.ReservedFor] ); } }
        public CswNbtNodePropDateTime DateCreated { get { return ( _CswNbtNode.Properties[PropertyName.DateCreated] ); } }
        public CswNbtNodePropList StoragePressure { get { return ( _CswNbtNode.Properties[PropertyName.StoragePressure] ); } }
        public CswNbtNodePropList StorageTemperature { get { return ( _CswNbtNode.Properties[PropertyName.StorageTemperature] ); } }
        public CswNbtNodePropList UseType { get { return ( _CswNbtNode.Properties[PropertyName.UseType] ); } }
        public CswNbtNodePropButton ViewSDS { get { return ( _CswNbtNode.Properties[PropertyName.ViewSDS] ); } }
        public CswNbtNodePropButton ViewCofA { get { return ( _CswNbtNode.Properties[PropertyName.ViewCofA] ); } }
        public CswNbtNodePropGrid ContainerDispenseTransactions { get { return ( _CswNbtNode.Properties[PropertyName.ContainerDispenseTransactions] ); } }
        public CswNbtNodePropGrid Documents { get { return ( _CswNbtNode.Properties[PropertyName.Documents] ); } }
        public CswNbtNodePropGrid SubmittedRequests { get { return ( _CswNbtNode.Properties[PropertyName.SubmittedRequests] ); } }
        public CswNbtNodePropRelationship HomeLocation { get { return ( _CswNbtNode.Properties[PropertyName.HomeLocation] ); } }
        public CswNbtNodePropComments Notes { get { return ( _CswNbtNode.Properties[PropertyName.Notes] ); } }
        public CswNbtNodePropText Project { get { return ( _CswNbtNode.Properties[PropertyName.Project] ); } }
        public CswNbtNodePropText SpecificActivity { get { return ( _CswNbtNode.Properties[PropertyName.SpecificActivity] ); } }
        public CswNbtNodePropQuantity TareQuantity { get { return ( _CswNbtNode.Properties[PropertyName.TareQuantity] ); } }
        public CswNbtNodePropText Concentration { get { return ( _CswNbtNode.Properties[PropertyName.Concentration] ); } }
        public CswNbtNodePropDateTime OpenedDate { get { return ( _CswNbtNode.Properties[PropertyName.OpenedDate] ); } }

        #endregion


    }//CswNbtObjClassContainer

}//namespace ChemSW.Nbt.ObjClasses
