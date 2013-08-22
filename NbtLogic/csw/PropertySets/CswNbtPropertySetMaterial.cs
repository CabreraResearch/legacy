using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Material Property Set
    /// </summary>
    public abstract class CswNbtPropertySetMaterial : CswNbtObjClass
    {
        #region Enums

        /// <summary>
        /// Object Class property names
        /// </summary>
        public new class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string MaterialId = "Material Id";
            public const string TradeName = "Tradename";
            public const string Supplier = "Supplier";
            public const string PartNumber = "Part Number";
            public const string ApprovedForReceiving = "Approved for Receiving";
            public const string Request = "Request";
            public const string Receive = "Receive";
            public const string C3ProductId = "C3ProductId";
            public const string IsConstituent = "Is Constituent";
            public const string ContainerExpirationLocked = "Container Expiration Locked";
            public const string Documents = "Documents";
            public const string Synonyms = "Synonyms";
        }

        public sealed class CswEnumPhysicalState
        {
            public const string NA = "n/a";
            public const string Liquid = "liquid";
            public const string Solid = "solid";
            public const string Gas = "gas";
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Solid, Liquid, Gas, NA };
        }

        public sealed class CswEnumRequestOption
        {
            public const string Bulk = "Request By Bulk";
            public const string Size = "Request By Size";
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Bulk, Size };
        }

        #endregion Enums

        #region Base

        /// <summary>
        /// Default Object Class for consumption by derived classes
        /// </summary>
        public CswNbtObjClassDefault CswNbtObjClassDefault = null;

        /// <summary>
        /// Property Set ctor
        /// </summary>
        public CswNbtPropertySetMaterial( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GenericClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtPropertySetMaterial
        /// </summary>
        public static implicit operator CswNbtPropertySetMaterial( CswNbtNode Node )
        {
            CswNbtPropertySetMaterial ret = null;
            if( null != Node && Members().Contains( Node.ObjClass.ObjectClass.ObjectClass ) )
            {
                ret = (CswNbtPropertySetMaterial) Node.ObjClass;
            }
            return ret;
        }

        public static Collection<CswEnumNbtObjectClass> Members()
        {
            Collection<CswEnumNbtObjectClass> Ret = new Collection<CswEnumNbtObjectClass>
            {
                CswEnumNbtObjectClass.ChemicalClass,
                CswEnumNbtObjectClass.NonChemicalClass
            };
            return Ret;
        }

        public override CswNbtNode CopyNode()
        {
            CswNbtNode CopiedNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswEnumNbtMakeNodeOperation.MakeTemp );
            CopiedNode.copyPropertyValues( Node );
            CopiedNode.postChanges( true, true );
            return CopiedNode;
        }

        #endregion Base

        #region Abstract Methods

        /// <summary>
        /// Before write node event for derived classes to implement
        /// </summary>
        public abstract void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation );

        /// <summary>
        /// After write node event for derived classes to implement
        /// </summary>
        public abstract void afterPropertySetWriteNode();

        /// <summary>
        /// Before delete node event for derived classes to implement
        /// </summary>
        public abstract void beforePropertySetDeleteNode( bool DeleteAllRequiredRelatedNodes = false );

        /// <summary>
        /// After delete node event for derived classes to implement
        /// </summary>
        public abstract void afterPropertySetDeleteNode();

        /// <summary>
        /// Populate props event for derived classes to implement
        /// </summary>
        public abstract void afterPropertySetPopulateProps();

        /// <summary>
        /// Button click event for derived classes to implement
        /// </summary>
        public abstract bool onPropertySetButtonClick( NbtButtonData ButtonData );

        /// <summary>
        /// ObjectClass-specific data for Receive button click
        /// </summary>
        public abstract void onReceiveButtonClick( NbtButtonData ButtonData );

        /// <summary>
        /// Mechanism to add default filters in derived classes
        /// </summary>
        public abstract void onPropertySetAddDefaultViewFilters( CswNbtViewRelationship ParentRelationship );

        public abstract DateTime getDefaultExpirationDate( DateTime InitialDate );

        public abstract void onUpdatePropertyValue();

        #endregion Abstract Methods

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            beforePropertySetWriteNode( IsCopy, OverrideUniqueValidation );

            Request.MenuOptions = CswEnumRequestOption.Options.ToString();
            Request.State = CswEnumRequestOption.Size;

            if( ApprovedForReceiving.WasModified )
            {
                Receive.setHidden( value: ApprovedForReceiving.Checked != CswEnumTristate.True, SaveToDb: true );
            }

            CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }

        public override void afterWriteNode()
        {
            afterPropertySetWriteNode();
            CswNbtObjClassDefault.afterWriteNode();
        }

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            beforePropertySetDeleteNode( DeleteAllRequiredRelatedNodes );
            CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        }

        public override void afterDeleteNode()
        {
            afterPropertySetDeleteNode();
            CswNbtObjClassDefault.afterDeleteNode();
        }

        protected override void afterPopulateProps()
        {
            afterPropertySetPopulateProps();
            ApprovedForReceiving.setReadOnly( false == _CswNbtResources.Permit.can( CswEnumNbtActionName.Material_Approval ), SaveToDb: false );
            ContainerExpirationLocked.setReadOnly( false == _CswNbtResources.Permit.can( CswEnumNbtActionName.Container_Expiration_Lock ), SaveToDb: false );
            _toggleButtonVisibility();
            _toggleConstituentProps();
            CswNbtObjClassDefault.triggerAfterPopulateProps();
        }

        /// <summary>
        /// Hide properties not appropriate for constituents.
        /// See also CswNbtObjClassChemical._toggleConstituentProps()
        /// </summary>
        private void _toggleConstituentProps()
        {
            if( CswEnumTristate.True == IsConstituent.Checked )
            {
                ApprovedForReceiving.setHidden( true, true );
                Supplier.setHidden( true, true );
                PartNumber.setHidden( true, true );
                Receive.setHidden( true, true );
                Request.setHidden( true, true );

            }
        } // _toggleConstituentProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            // Not IsConstituent
            ICswNbtMetaDataProp IsConstituentProp = null;
            // case 30136 - don't add this filter if the ownertype is nodetype
            //if( ParentRelationship.getOwnerType() == CswEnumNbtViewRelatedIdType.NodeTypeId )
            //{
            //    CswNbtMetaDataNodeType NodeType = ParentRelationship.getNodeTypeOwner();
            //    IsConstituentProp = NodeType.getNodeTypePropByObjectClassProp( PropertyName.IsConstituent );
            //}
            //else 
            if( ParentRelationship.getOwnerType() == CswEnumNbtViewRelatedIdType.ObjectClassId )
            {
                CswNbtMetaDataObjectClass ObjClass = ParentRelationship.getObjClassOwner();
                IsConstituentProp = ObjClass.getObjectClassProp( PropertyName.IsConstituent );
            }
            else if( ParentRelationship.getOwnerType() == CswEnumNbtViewRelatedIdType.PropertySetId )
            {
                //Bug - We're adding the Chemical IsConstituent property here, but for NTPs treeloader works by PropName. Not ideal, but works unless the propname changes
                CswNbtMetaDataPropertySet MaterialPS = _CswNbtResources.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
                CswNbtMetaDataObjectClass FirstMaterialOC = MaterialPS.getObjectClasses().FirstOrDefault();
                IsConstituentProp = FirstMaterialOC.getObjectClassProp( PropertyName.IsConstituent ).getNodeTypeProps().FirstOrDefault();
            }

            if( null != IsConstituentProp )
            {
                CswNbtViewProperty viewProp = ParentRelationship.View.AddViewProperty( ParentRelationship, IsConstituentProp );
                viewProp.ShowInGrid = false;
                ParentRelationship.View.AddViewPropertyFilter( viewProp,
                                                               FilterMode: CswEnumNbtFilterMode.NotEquals,
                                                               Value: CswEnumTristate.True.ToString() );
            }

            onPropertySetAddDefaultViewFilters( ParentRelationship );
            CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData.NodeTypeProp )
            {
                bool HasPermission = false;
                string OCPPropName = ButtonData.NodeTypeProp.getObjectClassPropName();
                switch( OCPPropName )
                {
                    case PropertyName.Request:
                        if( _CswNbtResources.Permit.can( CswEnumNbtActionName.Submit_Request ) )
                        {
                            HasPermission = true;
                            CswNbtActRequesting RequestAct = new CswNbtActRequesting( _CswNbtResources );

                            CswNbtPropertySetRequestItem NodeAsPropSet = RequestAct.makeMaterialRequestItem( new CswEnumNbtRequestItemType( CswEnumNbtRequestItemType.Material ), NodeId, ButtonData );

                            ButtonData.Data["requestaction"] = OCPPropName;
                            ButtonData.Data["titleText"] = ButtonData.SelectedText + " for " + TradeName.Text;
                            ButtonData.Data["requestItemProps"] = RequestAct.getRequestItemAddProps( NodeAsPropSet );
                            ButtonData.Data["requestItemNodeTypeId"] = NodeAsPropSet.NodeTypeId;
                            ButtonData.Data["relatednodeid"] = NodeId.ToString();
                            ButtonData.Action = CswEnumNbtButtonAction.request;
                        }
                        break;
                    case PropertyName.Receive:
                        if( _CswNbtResources.Permit.can( CswEnumNbtActionName.Receiving ) )
                        {
                            HasPermission = true;
                            CswNbtActReceiving Act = new CswNbtActReceiving( _CswNbtResources, ObjectClass, NodeId );
                            _CswNbtResources.setAuditActionContext( CswEnumNbtActionName.Receiving );
                            CswNbtObjClassContainer Container = Act.makeContainer();

                            //Case 29436
                            if( Container.isLocationInAccessibleInventoryGroup( _CswNbtResources.CurrentNbtUser.DefaultLocationId ) )
                            {
                                Container.Location.SelectedNodeId = _CswNbtResources.CurrentNbtUser.DefaultLocationId;
                            }
                            Container.Owner.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
                            DateTime ExpirationDate = getDefaultExpirationDate( DateTime.Now );
                            if( DateTime.MinValue != ExpirationDate )
                            {
                                Container.ExpirationDate.DateTimeValue = ExpirationDate;
                            }
                            Container.postChanges( false );

                            ButtonData.Data["state"] = new JObject();
                            ButtonData.Data["state"]["materialId"] = NodeId.ToString();
                            ButtonData.Data["state"]["materialNodeTypeId"] = NodeTypeId;
                            ButtonData.Data["state"]["tradeName"] = TradeName.Text;

                            Int32 ContainerLimit = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.container_receipt_limit.ToString() ) );
                            ButtonData.Data["state"]["containerlimit"] = ContainerLimit;
                            ButtonData.Data["state"]["containerNodeId"] = Container.NodeId.ToString();
                            ButtonData.Data["state"]["containerNodeTypeId"] = Container.NodeTypeId;
                            bool customBarcodes = CswConvert.ToBoolean( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.custom_barcodes.ToString() ) );
                            ButtonData.Data["state"]["customBarcodes"] = customBarcodes;
                            ButtonData.Data["state"]["nodetypename"] = this.NodeType.NodeTypeName;
                            ButtonData.Data["state"]["containerAddLayout"] = Act.getContainerAddProps( Container );

                            onReceiveButtonClick( ButtonData );
                            _setCofAData( ButtonData );
                            ButtonData.Action = CswEnumNbtButtonAction.receive;
                        }
                        break;
                    case CswNbtObjClass.PropertyName.Save:
                        HasPermission = true;
                        break;
                }
                HasPermission = HasPermission || onPropertySetButtonClick( ButtonData );
                if( false == HasPermission )
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "You do not have permission to the " + OCPPropName + " action.", "You do not have permission to the " + OCPPropName + " action." );
                }
            }

            return true;
        }

        #endregion Inherited Events

        #region Custom Logic

        public static CswNbtView getMaterialNodeView( CswNbtResources NbtResources, CswNbtNode MaterialNode )
        {
            CswNbtView Ret = null;
            if( MaterialNode != null )
            {
                Ret = MaterialNode.getViewOfNode( false );
                if( NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
                {
                    CswNbtMetaDataObjectClass SizeOc = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );
                    CswNbtMetaDataObjectClassProp MaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
                    Ret.AddViewRelationship( Ret.Root.ChildRelationships[0], CswEnumNbtViewPropOwnerType.Second, MaterialOcp, false );
                }
                Ret.ViewName = "New Material: " + MaterialNode.NodeName;
            }
            return Ret;
        }

        public static CswNbtView getMaterialNodeView( CswNbtResources NbtResources, Int32 NodeTypeId, string Tradename, CswPrimaryKey SupplierId, string PartNo = "" )
        {
            if( Int32.MinValue == NodeTypeId ||
                false == CswTools.IsPrimaryKey( SupplierId ) ||
                String.IsNullOrEmpty( Tradename ) )
            {
                throw new CswDniException( CswEnumErrorType.Error,
                                           "Cannot get a material without a type, a supplier and a tradename.",
                                           "Attempted to call _getMaterialNodeView with invalid or empty parameters. Type: " + NodeTypeId + ", Tradename: " + Tradename + ", SupplierId: " + SupplierId );
            }

            CswNbtView Ret = new CswNbtView( NbtResources );
            Ret.ViewMode = CswEnumNbtViewRenderingMode.Tree;
            Ret.Visibility = CswEnumNbtViewVisibility.User;
            Ret.VisibilityUserId = NbtResources.CurrentNbtUser.UserId;
            CswNbtMetaDataNodeType MaterialNt = NbtResources.MetaData.getNodeType( NodeTypeId );
            CswNbtViewRelationship MaterialRel = Ret.AddViewRelationship( MaterialNt, false );
            CswNbtMetaDataNodeTypeProp TradeNameNtp = MaterialNt.getNodeTypePropByObjectClassProp( PropertyName.TradeName );
            CswNbtMetaDataNodeTypeProp SupplierNtp = MaterialNt.getNodeTypePropByObjectClassProp( PropertyName.Supplier );
            CswNbtMetaDataNodeTypeProp PartNoNtp = MaterialNt.getNodeTypePropByObjectClassProp( PropertyName.PartNumber );

            Ret.AddViewPropertyAndFilter( MaterialRel, TradeNameNtp, Tradename );
            Ret.AddViewPropertyAndFilter( MaterialRel, SupplierNtp, SupplierId.PrimaryKey.ToString(), CswEnumNbtSubFieldName.NodeID );
            CswEnumNbtFilterMode PartNoFilterMode = CswEnumNbtFilterMode.Equals;
            if( string.IsNullOrEmpty( PartNo ) )
            {
                PartNoFilterMode = CswEnumNbtFilterMode.Null;
            }
            Ret.AddViewPropertyAndFilter( ParentViewRelationship: MaterialRel,
                                            MetaDataProp: PartNoNtp,
                                            Value: PartNo,
                                            FilterMode: PartNoFilterMode );

            if( NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                CswNbtMetaDataObjectClass SizeOc = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );
                CswNbtMetaDataObjectClassProp MaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
                Ret.AddViewRelationship( MaterialRel, CswEnumNbtViewPropOwnerType.Second, MaterialOcp, false );
            }

            Ret.ViewName = "New Material: " + Tradename;

            return Ret;
        }

        /// <summary>
        /// Fetch a Material node by NodeTypeId, TradeName, Supplier and PartNo (Optional). This method will throw if required parameters are null or empty.
        /// </summary>
        public static CswNbtPropertySetMaterial getExistingMaterial( CswNbtResources NbtResources, Int32 MaterialNodeTypeId, CswPrimaryKey SupplierId, string TradeName, string PartNo )
        {
            CswNbtPropertySetMaterial Ret = null;

            CswNbtView MaterialNodeView = getMaterialNodeView( NbtResources, MaterialNodeTypeId, TradeName, SupplierId, PartNo );
            ICswNbtTree Tree = NbtResources.Trees.getTreeFromView( MaterialNodeView, false, false, false );
            bool MaterialExists = Tree.getChildNodeCount() > 0;

            if( MaterialExists )
            {
                Tree.goToNthChild( 0 );
                Ret = Tree.getNodeForCurrentPosition();
            }
            return Ret;
        }

        private bool _canReceive()
        {
            return ( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) &&
                     ApprovedForReceiving.Checked == CswEnumTristate.True &&
                     _CswNbtResources.Permit.can( CswEnumNbtActionName.Receiving ) &&
                     CswNbtObjClassInventoryGroupPermission.getInventoryGroupIdsForCurrentUser( _CswNbtResources ).Count > 0 );
        }

        private void _toggleButtonVisibility()
        {
            Receive.setHidden( value: false == _canReceive(), SaveToDb: false );
            Request.setHidden( value: false == _CswNbtResources.Permit.can( CswEnumNbtActionName.Submit_Request ), SaveToDb: false );
        }

        private void _setCofAData( NbtButtonData ButtonData )
        {
            bool canAddCofA = false;
            CswNbtMetaDataObjectClass CofADocOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.CofADocumentClass );
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.ManufacturerLotInfo ) )
            {
                CswNbtMetaDataNodeType CofANT = CofADocOC.FirstNodeType;
                canAddCofA = _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, CofANT );
                if( canAddCofA )
                {
                    CswNbtMetaDataObjectClass ReceiptLotOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
                    ButtonData.Data["state"]["cofaDocTypeId"] = CofANT.NodeTypeId;
                    ButtonData.Data["state"]["receiptLotTypeId"] = ReceiptLotOC.FirstNodeType.NodeTypeId;
                }
            }
            ButtonData.Data["state"]["canAddCofA"] = canAddCofA;
        }

        #endregion Custom Logic

        #region Property Set specific properties

        public CswNbtNodePropSequence MaterialId { get { return ( _CswNbtNode.Properties[PropertyName.MaterialId] ); } }
        public CswNbtNodePropText TradeName { get { return _CswNbtNode.Properties[PropertyName.TradeName]; } }
        public CswNbtNodePropRelationship Supplier { get { return _CswNbtNode.Properties[PropertyName.Supplier]; } }
        public CswNbtNodePropText PartNumber { get { return _CswNbtNode.Properties[PropertyName.PartNumber]; } }
        public CswNbtNodePropLogical ApprovedForReceiving { get { return ( _CswNbtNode.Properties[PropertyName.ApprovedForReceiving] ); } }
        public CswNbtNodePropButton Receive { get { return _CswNbtNode.Properties[PropertyName.Receive]; } }
        public CswNbtNodePropButton Request { get { return _CswNbtNode.Properties[PropertyName.Request]; } }
        public CswNbtNodePropText C3ProductId { get { return ( _CswNbtNode.Properties[PropertyName.C3ProductId] ); } }
        public CswNbtNodePropLogical IsConstituent { get { return ( _CswNbtNode.Properties[PropertyName.IsConstituent] ); } }
        public CswNbtNodePropLogical ContainerExpirationLocked { get { return ( _CswNbtNode.Properties[PropertyName.ContainerExpirationLocked] ); } }
        public CswNbtNodePropGrid Documents { get { return ( _CswNbtNode.Properties[PropertyName.Documents] ); } }
        public CswNbtNodePropGrid Synonyms { get { return ( _CswNbtNode.Properties[PropertyName.Synonyms] ); } }

        #endregion

    }//CswNbtPropertySetMaterial

}//namespace ChemSW.Nbt.ObjClasses