using System;
using System.Linq;
using ChemSW.Config;
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
    public class CswNbtObjClassContainer: CswNbtObjClass
    {
        #region Properties

        public new sealed class PropertyName: CswNbtObjClass.PropertyName
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

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            ViewSDS.State = PropertyName.ViewSDS;
            ViewSDS.MenuOptions = PropertyName.ViewSDS + ",View Other";

            // update Request Menu
            CswCommaDelimitedString MenuOpts = new CswCommaDelimitedString();
            if( Tristate.True != Disposed.Checked )
            {
                MenuOpts.Add( CswEnumNbtContainerRequestMenu.Move );
                if( Tristate.True != Missing.Checked && Quantity.Quantity > 0 )
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
            if( this.ContainerGroup.WasModified &&
                this.ContainerGroup.RelatedNodeId != null &&
                ( this.ContainerGroup.GetOriginalPropRowValue( CswEnumNbtSubFieldName.NodeID ) != this.ContainerGroup.RelatedNodeId.PrimaryKey.ToString() ) )
            {
                CswNbtObjClassContainerGroup NewGroupNode = _CswNbtResources.Nodes[this.ContainerGroup.RelatedNodeId];

                if( null != NewGroupNode )
                {
                    if( Tristate.True == NewGroupNode.SyncLocation.Checked )
                    {
                        this.Location.SelectedNodeId = NewGroupNode.Location.SelectedNodeId;
                        this.Location.RefreshNodeName();
                    }
                }
            }
            else if( this.Location.WasModified &&
                     this.Location.SelectedNodeId != null &&
                     ( this.Location.GetOriginalPropRowValue( CswEnumNbtSubFieldName.NodeID ) != this.Location.SelectedNodeId.PrimaryKey.ToString() ) )
            {
                CswNbtObjClassContainerGroup NewGroupNode = _CswNbtResources.Nodes[this.ContainerGroup.RelatedNodeId];

                if( null != NewGroupNode )
                {
                    if( Tristate.True == NewGroupNode.SyncLocation.Checked && ( this.Location.SelectedNodeId != NewGroupNode.Location.SelectedNodeId ) )
                    {
                        this.ContainerGroup.RelatedNodeId = null;
                    }
                }
            }

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
            CswNbtSdInventoryLevelMgr Mgr = new CswNbtSdInventoryLevelMgr( _CswNbtResources );
            string Reason = "Container " + Barcode.Barcode + " with quantity " + Quantity.Quantity + " has been deleted.";
            _InventoryLevelModified = Mgr.addToCurrentQuantity( -Quantity.Quantity, Quantity.UnitId, Reason, Material.RelatedNodeId, Location.SelectedNodeId );
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            Material.SetOnPropChange( OnMaterialPropChange );
            Dispose.SetOnPropChange( OnDisposePropChange );
            Quantity.SetOnPropChange( OnQuantityPropChange );
            Location.SetOnPropChange( OnLocationPropChange );
            Size.SetOnPropChange( OnSizePropChange );
            SourceContainer.SetOnPropChange( OnSourceContainerChange );
            Barcode.SetOnPropChange( OnBarcodePropChange );
            LotControlled.SetOnPropChange( OnLotControlledPropChange );
            Owner.SetOnPropChange( OnOwnerPropChange ); //case 28514

            bool IsDisposed = ( Disposed.Checked == Tristate.True );
            //SaveToDb true is necessary to override what's in the db even if it isn't actually saved as part of this request
            Dispense.setHidden( value : ( IsDisposed || false == canContainer( _CswNbtResources.Actions[CswNbtActionName.DispenseContainer] ) ), SaveToDb : true );
            Dispose.setHidden( value : ( IsDisposed || false == canContainer( _CswNbtResources.Actions[CswNbtActionName.DisposeContainer] ) ), SaveToDb : true );
            Undispose.setHidden( value : ( false == IsDisposed || false == canContainer( _CswNbtResources.Actions[CswNbtActionName.UndisposeContainer] ) ), SaveToDb : true );
            bool CantRequest = ( ( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.MLM ) &&
                                  Requisitionable.Checked == Tristate.False ) ||
                                  IsDisposed ||
                                  false == canContainer( _CswNbtResources.Actions[CswNbtActionName.Submit_Request] ) );
            Request.setHidden( value : CantRequest, SaveToDb : true );

            CswNbtObjClassMaterial material = _CswNbtResources.Nodes[Material.RelatedNodeId];
            if( null != material )
            {
                CswNbtMetaDataNodeTypeTab materialIdentityTab = material.NodeType.getIdentityTab();
                bool isHidden = _CswNbtResources.MetaData.NodeTypeLayout.getPropsNotInLayout( material.NodeType, Int32.MinValue, CswEnumNbtLayoutType.Edit ).Contains( material.ViewSDS.NodeTypeProp );
                ViewSDS.setHidden( isHidden, false );
            }

            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            // Disposed == false
            CswNbtMetaDataObjectClassProp DisposedOCP = ObjectClass.getObjectClassProp( PropertyName.Disposed );
            CswNbtViewProperty viewProp = ParentRelationship.View.AddViewProperty( ParentRelationship, DisposedOCP );
            viewProp.ShowInGrid = false;
            ParentRelationship.View.AddViewPropertyFilter( viewProp, FilterMode : CswEnumNbtFilterMode.Equals, Value : Tristate.False.ToString() , ShowAtRuntime: true );

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
                        if( canContainer( _CswNbtResources.Actions[CswNbtActionName.DisposeContainer] ) )
                        {
                            HasPermission = true;
                            DisposeContainer(); //case 26665
                            postChanges( true );
                            ButtonData.Action = CswEnumNbtButtonAction.refresh;
                        }
                        break;
                    case PropertyName.Undispose:
                        if( canContainer( _CswNbtResources.Actions[CswNbtActionName.UndisposeContainer] ) )
                        {
                            HasPermission = true;
                            UndisposeContainer();
                            postChanges( true );
                            ButtonData.Action = CswEnumNbtButtonAction.refresh;
                        }
                        break;
                    case PropertyName.Dispense:
                        if( canContainer( _CswNbtResources.Actions[CswNbtActionName.DispenseContainer] ) )
                        {
                            HasPermission = true;
                            //ActionData = this.NodeId.ToString();
                            ButtonData.Data = _getDispenseActionData();
                            ButtonData.Action = CswEnumNbtButtonAction.dispense;
                        }
                        break;
                    case PropertyName.Request:
                        if( canContainer( _CswNbtResources.Actions[CswNbtActionName.Submit_Request] ) )
                        {
                            CswNbtActRequesting RequestAct = new CswNbtActRequesting( _CswNbtResources );
                            HasPermission = true;

                            CswNbtPropertySetRequestItem NodeAsPropSet = RequestAct.makeContainerRequestItem( this, ButtonData );

                            ButtonData.Data["titleText"] = "Add to Cart&#58 " + NodeAsPropSet.Type.Value + " " + Barcode.Barcode;
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

                        CswNbtObjClassMaterial material = _CswNbtResources.Nodes[Material.RelatedNodeId];
                        if( null != material )
                        {
                            material.GetMatchingSDSForCurrentUser( ButtonData );
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


        /// <summary>
        /// Check container permissions.  Provide one of Permission or Action.
        /// </summary>
        public bool canContainer( CswNbtPermit.NodeTypePermission Permission, ICswNbtUser User = null )
        {
            return _canContainer( Permission, null, User );
        }
        /// <summary>
        /// Check container permissions.  Provide one of Permission or Action.
        /// </summary>
        public bool canContainer( CswNbtAction Action, ICswNbtUser User = null )
        {
            if( null == Action )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "You do not have appropriate permissions", "canContainer called with null Action" );
            }
            return _canContainer( CswNbtPermit.NodeTypePermission.View, Action, User );
        }

        /// <summary>
        /// Check container permissions.  Provide one of Permission or Action.
        /// </summary>
        private bool _canContainer( CswNbtPermit.NodeTypePermission Permission, CswNbtAction Action, ICswNbtUser User )
        {
            bool ret = true;

            if( null == User )
            {
                User = _CswNbtResources.CurrentNbtUser;
            }
            if( false == ( User is CswNbtSystemUser ) )
            {
                // Special container permissions, based on Inventory Group                

                // We find the matching InventoryGroupPermission based on:
                //   the Container's Location's Inventory Group
                //   the User's WorkUnit
                //   the User's Role
                // We allow or deny permission to perform the action using the appropriate Logical

                ret = false;

                CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
                CswNbtObjClassLocation LocationNode = _CswNbtResources.Nodes[this.Location.SelectedNodeId];
                if( null != LocationNode )
                {
                    CswNbtObjClassInventoryGroup InventoryGroupNode = _CswNbtResources.Nodes[LocationNode.InventoryGroup.RelatedNodeId];
                    if( null != InventoryGroupNode )
                    {
                        //if the user has no workunit, but the location does belong to an inventory group, you don't get permission.
                        if( null != User.WorkUnitId )
                        {
                            CswNbtMetaDataObjectClass InvGrpPermOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupPermissionClass );

                            CswNbtMetaDataObjectClassProp PermInvGrpOCP = InvGrpPermOC.getObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.InventoryGroup );
                            CswNbtMetaDataObjectClassProp PermRoleOCP = InvGrpPermOC.getObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.Role );
                            CswNbtMetaDataObjectClassProp PermWorkUnitOCP = InvGrpPermOC.getObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.WorkUnit );

                            CswNbtView InvGrpPermView = new CswNbtView( _CswNbtResources );
                            InvGrpPermView.ViewName = "CswNbtPermit_InventoryGroupPermCheck";
                            CswNbtViewRelationship InvGrpPermVR = InvGrpPermView.AddViewRelationship( InvGrpPermOC, false );

                            // filter to inventory group, role, and workunit
                            InvGrpPermView.AddViewPropertyAndFilter( InvGrpPermVR, PermInvGrpOCP, InventoryGroupNode.NodeId.PrimaryKey.ToString(), CswEnumNbtSubFieldName.NodeID );
                            InvGrpPermView.AddViewPropertyAndFilter( InvGrpPermVR, PermRoleOCP, User.RoleId.PrimaryKey.ToString(), CswEnumNbtSubFieldName.NodeID );
                            InvGrpPermView.AddViewPropertyAndFilter( InvGrpPermVR, PermWorkUnitOCP, User.WorkUnitId.PrimaryKey.ToString(), CswEnumNbtSubFieldName.NodeID );

                            ICswNbtTree InvGrpPermTree = _CswNbtResources.Trees.getTreeFromView( InvGrpPermView, false, true, false );
                            if( InvGrpPermTree.getChildNodeCount() > 0 )
                            {
                                InvGrpPermTree.goToNthChild( 0 ); // inventory group permission
                                CswNbtObjClassInventoryGroupPermission PermNode = InvGrpPermTree.getNodeForCurrentPosition();
                                if( Action != null )
                                {
                                    if( ( Action.Name == CswNbtActionName.DispenseContainer && PermNode.Dispense.Checked == Tristate.True ) ||
                                        ( Action.Name == CswNbtActionName.DisposeContainer && PermNode.Dispose.Checked == Tristate.True ) ||
                                        ( Action.Name == CswNbtActionName.UndisposeContainer && PermNode.Undispose.Checked == Tristate.True ) ||
                                        ( Action.Name == CswNbtActionName.Submit_Request && PermNode.Request.Checked == Tristate.True ) )
                                    {
                                        ret = true;
                                    }
                                    else if( Action.Name == CswNbtActionName.Receiving )
                                    {
                                        foreach( CswNbtMetaDataNodeType ContainerNt in ContainerOC.getLatestVersionNodeTypes() )
                                        {
                                            ret = _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create, ContainerNt );
                                            if( ret )
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                { //there's only edit, so edit applies to all three
                                    if( ( Permission == CswNbtPermit.NodeTypePermission.View && PermNode.View.Checked == Tristate.True ) ||
                                        PermNode.Edit.Checked == Tristate.True )
                                    {
                                        ret = true;
                                    }

                                } //if-else action is not null
                            } // if( InvGrpPermTree.getChildNodeCount() > 0 )
                        }//if( null != User.WorkUnitId )
                    } // if( null != InventoryGroupNode )
                    else
                    {
                        // location has no inventory group, no permissions to enforce
                        ret = true;
                    }
                } // if(null != LocationNode )
                else
                {
                    // container has no location, no permissions to enforce
                    ret = true;
                }
            } // if( false == ( User is CswNbtSystemUser ) )
            return ret;
        } // canContainer()

        /// <summary>
        /// Checks permission and disposes a container (does not post changes!)
        /// </summary>
        public void DisposeContainer( bool OverridePermissions = false )
        {
            if( OverridePermissions || canContainer( _CswNbtResources.Actions[CswNbtActionName.DisposeContainer] ) )
            {
                _createContainerTransactionNode( CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispose, -this.Quantity.Quantity, this.Quantity.UnitId, SrcContainer : this );
                this.Quantity.Quantity = 0;
                this.Disposed.Checked = Tristate.True;
                this.Undispose.setHidden( false, true );
                _setDisposedReadOnly( true );
                CreateContainerLocationNode( CswNbtObjClassContainerLocation.TypeOptions.Dispose );
                _CswNbtNode.IconFileNameOverride = "x.png";
                _CswNbtNode.Searchable = false;
            }
        }

        /// <summary>
        /// Checks permission and undisposes a container
        /// </summary>
        public void UndisposeContainer( bool OverridePermissions = false, bool CreateContainerLocation = true )
        {

            if( OverridePermissions || canContainer( _CswNbtResources.Actions[CswNbtActionName.UndisposeContainer] ) )
            {
                CswNbtMetaDataNodeType ContDispTransNT = _CswNbtResources.MetaData.getNodeType( "Container Dispense Transaction" );
                CswNbtObjClassContainerDispenseTransaction ContDispTransNode = _getMostRecentDisposeTransaction( ContDispTransNT );

                if( ContDispTransNode != null )
                {
                    this.Quantity.Quantity = -ContDispTransNode.QuantityDispensed.Quantity;
                    this.Quantity.UnitId = ContDispTransNode.QuantityDispensed.UnitId;
                    ContDispTransNode.Node.delete( OverridePermissions : true );
                }
                this.Disposed.Checked = Tristate.False;
                _setDisposedReadOnly( false );
                if( CreateContainerLocation )
                {
                    CreateContainerLocationNode( CswNbtObjClassContainerLocation.TypeOptions.Undispose );
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
            CreateContainerLocationNode( CswNbtObjClassContainerLocation.TypeOptions.Dispense );
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
            CswNbtObjClassContainerLocation.TypeOptions ContainerLocationType =
                SourceContainer == null ? CswNbtObjClassContainerLocation.TypeOptions.Receipt
                                        : CswNbtObjClassContainerLocation.TypeOptions.Dispense;
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
            int maxGenerations = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.container_max_depth ) );

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
        /// *Note: The NewLocationBarcode and ContainerBarcode parameters are necessary only if the Type = Scan. They
        /// are currently only used for the reconcile data from the CISPro/NBT CORE mobile app.
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="NewLocationBarcode"></param>
        /// <param name="ContainerBarcode"></param>
        public void CreateContainerLocationNode( CswNbtObjClassContainerLocation.TypeOptions Type, string NewLocationBarcode = "", string ContainerBarcode = "" )
        {
            CswNbtMetaDataNodeType ContLocNT = _CswNbtResources.MetaData.getNodeType( "Container Location" );
            if( ContLocNT != null )
            {
                CswNbtObjClassContainerLocation ContLocNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ContLocNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                ContLocNode.Type.Value = Type.ToString();
                ContLocNode.Container.RelatedNodeId = NodeId;
                if( null != Location )
                {
                    if( Type != CswNbtObjClassContainerLocation.TypeOptions.Scan )
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
                ContLocNode.ActionApplied.Checked = Tristate.False;
                ContLocNode.ScanDate.DateTimeValue = DateTime.Now;
                ContLocNode.User.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
                ContLocNode.postChanges( false );
                LocationVerified.DateTimeValue = DateTime.Now;
                if( Missing.Checked == Tristate.True )
                {
                    Missing.Checked = Tristate.False;
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
                    CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispose.ToString(),
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
        private void _createContainerTransactionNode( CswNbtObjClassContainerDispenseTransaction.DispenseType DispenseType, double Amount, CswPrimaryKey UnitId, CswPrimaryKey RequestItemId = null,
                                                      CswNbtObjClassContainer SrcContainer = null, CswNbtObjClassContainer DestinationContainer = null )
        {
            CswNbtMetaDataNodeType ContDispTransNT = _CswNbtResources.MetaData.getNodeType( "Container Dispense Transaction" );
            if( ContDispTransNT != null )
            {
                CswNbtObjClassContainerDispenseTransaction ContDispTransNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ContDispTransNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );

                if( SrcContainer != null )
                {
                    ContDispTransNode.SourceContainer.RelatedNodeId = SrcContainer.NodeId;
                    ContDispTransNode.RemainingSourceContainerQuantity.Quantity = SrcContainer.Quantity.Quantity;
                    if( DispenseType == CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispose )
                    {
                        ContDispTransNode.RemainingSourceContainerQuantity.Quantity = 0;
                    }
                    ContDispTransNode.RemainingSourceContainerQuantity.UnitId = SrcContainer.Quantity.UnitId;
                }
                if( DestinationContainer != null )
                {
                    ContDispTransNode.DestinationContainer.RelatedNodeId = DestinationContainer.NodeId;
                }
                ContDispTransNode.QuantityDispensed.Quantity = Amount;
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

        private void _setDisposedReadOnly( bool isReadOnly ) //case 25814
        {
            foreach( CswNbtNodePropWrapper prop in _CswNbtNode.Properties )
            {
                if( prop.ObjectClassPropName != PropertyName.ContainerFamily &&
                    prop.ObjectClassPropName != PropertyName.Undispose &&
                    prop.ObjectClassPropName != PropertyName.Disposed )
                {
                    if( prop.getFieldType().FieldType == CswEnumNbtFieldType.Button )
                    {
                        prop.setHidden( isReadOnly, SaveToDb: true );
                    }
                    prop.setReadOnly( isReadOnly, SaveToDb: true );
                }
            }
        } // _setDisposedReadOnly()

        private bool _checkStorageCompatibility()
        {
            bool Ret = true;

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
                CswNbtViewRelationship generationXParent = view.AddViewRelationship( parent, NbtViewPropOwnerType.Second, sourceContainerOCP, false );
                view.AddViewProperty( generationXParent, sourceContainerOCP );
                _getFamilyView( ref view, generationXParent, generation + 1, maxGenerations, sourceContainerOCP, barcodeOCP );
            }
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
                RequestItemView.AddViewPropertyAndFilter( RiRelationship, ContainerOcp, SubFieldName : CswEnumNbtSubFieldName.NodeID, Value : NodeId.PrimaryKey.ToString() );
                RequestItemView.AddViewPropertyAndFilter( RiRelationship, TypeOcp, RequestItemType );

                if( RequestItemType == CswNbtObjClassRequestContainerUpdate.Types.Move )
                {
                    CswNbtMetaDataObjectClassProp LocationOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Location );
                    RequestItemView.AddViewPropertyAndFilter( RiRelationship, LocationOcp, SubFieldName : CswEnumNbtSubFieldName.NodeID, Value : Location.SelectedNodeId.PrimaryKey.ToString() );
                }

                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( RequestItemView, IncludeSystemNodes : false, RequireViewPermissions : false, IncludeHiddenNodes : false );
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
        private void OnBarcodePropChange( CswNbtNodeProp Prop )
        {
            Barcode.setReadOnly( value : false == string.IsNullOrEmpty( Barcode.Barcode ), SaveToDb : true );
        }

        public CswNbtNodePropLocation Location { get { return ( _CswNbtNode.Properties[PropertyName.Location] ); } }
        private void OnLocationPropChange( CswNbtNodeProp Prop )
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
                    _updateRequestItems( CswNbtObjClassRequestContainerUpdate.Types.Move );
                }
                if( null != Location.SelectedNodeId )
                {
                    if( String.IsNullOrEmpty( Location.GetOriginalPropRowValue() ) )
                    {
                        CswNbtObjClassLocation LocNode = _CswNbtResources.Nodes.GetNode( Location.SelectedNodeId );
                        CswNbtObjClassInventoryGroup InvGroupNode = _CswNbtResources.Nodes.GetNode( LocNode.InventoryGroup.RelatedNodeId );
                        if( null != InvGroupNode )
                        {
                            LotControlled.Checked = InvGroupNode.Central.Checked == Tristate.True ? Tristate.True : Tristate.False;
                        }
                    }
                    else if( Location.GetOriginalPropRowValue() != Location.CachedNodeName && Location.CreateContainerLocation )
                    {
                        CreateContainerLocationNode( CswNbtObjClassContainerLocation.TypeOptions.Move );
                    }
                }
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
                SourceContainer.setReadOnly( value : true, SaveToDb : true );
            }
        }
        public CswNbtNodePropList Status { get { return ( _CswNbtNode.Properties[PropertyName.Status] ); } }
        public CswNbtNodePropLogical Missing { get { return ( _CswNbtNode.Properties[PropertyName.Missing] ); } }
        public CswNbtNodePropLogical Disposed { get { return ( _CswNbtNode.Properties[PropertyName.Disposed] ); } }
        private void OnDisposePropChange( CswNbtNodeProp Prop )
        {
            Dispose.setHidden( value : true, SaveToDb : true );
            if( CswConvert.ToTristate( Disposed.GetOriginalPropRowValue() ) != Disposed.Checked &&
                Disposed.Checked == Tristate.True )
            {
                _updateRequestItems( CswNbtObjClassRequestContainerUpdate.Types.Dispose );
            }
        }
        public CswNbtNodePropRelationship SourceContainer { get { return ( _CswNbtNode.Properties[PropertyName.SourceContainer] ); } }
        private void OnSourceContainerChange( CswNbtNodeProp Prop )
        {
            if( null != SourceContainer.RelatedNodeId && Int32.MinValue != SourceContainer.RelatedNodeId.PrimaryKey )
            {
                SourceContainer.setHidden( value : false, SaveToDb : true );
            }
            else
            {
                SourceContainer.setHidden( value : true, SaveToDb : true );
            }
        }
        public CswNbtNodePropQuantity Quantity { get { return ( _CswNbtNode.Properties[PropertyName.Quantity] ); } }
        private void OnQuantityPropChange( CswNbtNodeProp Prop )
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
                Size.setReadOnly( value : true, SaveToDb : true );
                Size.setHidden( value : true, SaveToDb : true );
            }
        }

        public CswNbtNodePropButton Request { get { return ( _CswNbtNode.Properties[PropertyName.Request] ); } }
        public CswNbtNodePropButton Dispense { get { return ( _CswNbtNode.Properties[PropertyName.Dispense] ); } }
        public CswNbtNodePropButton Dispose { get { return ( _CswNbtNode.Properties[PropertyName.Dispose] ); } }
        public CswNbtNodePropButton Undispose { get { return ( _CswNbtNode.Properties[PropertyName.Undispose] ); } }
        public CswNbtNodePropRelationship Owner { get { return ( _CswNbtNode.Properties[PropertyName.Owner] ); } }
        private void OnOwnerPropChange( CswNbtNodeProp Prop ) //case 28514
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
                            Location.SelectedNodeId = CurrentOwnerNode.DefaultLocationId;
                            Location.RefreshNodeName();
                        }
                    }
                }
            }

        }
        public CswNbtNodePropButton ContainerFamily { get { return ( _CswNbtNode.Properties[PropertyName.ContainerFamily] ); } }
        public CswNbtNodePropRelationship ReceiptLot { get { return ( _CswNbtNode.Properties[PropertyName.ReceiptLot] ); } }
        public CswNbtNodePropLogical LotControlled { get { return ( _CswNbtNode.Properties[PropertyName.LotControlled] ); } }
        private void OnLotControlledPropChange( CswNbtNodeProp Prop )
        {
            if( LotControlled.Checked == Tristate.True )
            {
                //DispenseForCertificate.RelatedNodeId = null;//TODO - uncomment when DispenseForCertificate is created
                Status.Value = String.Empty;
            }
            else if( LotControlled.Checked == Tristate.False )
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
                if( null != ReceiptLotNode )
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
        #endregion


    }//CswNbtObjClassContainer

}//namespace ChemSW.Nbt.ObjClasses
