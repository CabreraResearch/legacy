using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.ChemCatCentral;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.UnitsOfMeasure;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMaterial : CswNbtObjClass
    {
        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassMaterial( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialClass ); }
        }

        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string Supplier = "Supplier";
            public const string PartNumber = "Part Number";
            public const string SpecificGravity = "Specific Gravity";
            public const string PhysicalState = "Physical State";
            public const string CasNo = "CAS No";
            public const string RegulatoryLists = "Regulatory Lists";
            public const string Tradename = "Tradename";
            public const string StorageCompatibility = "Storage Compatibility";
            public const string ExpirationInterval = "Expiration Interval";
            public const string Request = "Request";
            public const string Receive = "Receive";
            public const string MaterialId = "Material Id";
            public const string ApprovedForReceiving = "Approved for Receiving";
            public const string UNCode = "UN Code";
            public const string IsTierII = "Is Tier II";
            public const string ViewSDS = "View SDS";
            public const string C3ProductId = "C3ProductId";
            public const string C3SyncDate = "C3SyncDate";
            public const string HazardClasses = "Hazard Classes";
        }

        public sealed class PhysicalStates
        {
            public const string NA = "n/a";
            public const string Liquid = "liquid";
            public const string Solid = "solid";
            public const string Gas = "gas";
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Solid, Liquid, Gas, NA };
        }

        public sealed class Requests
        {
            public const string Bulk = "Request By Bulk";
            public const string Size = "Request By Size";

            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Bulk, Size };
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMaterial
        /// </summary>
        public static implicit operator CswNbtObjClassMaterial( CswNbtNode Node )
        {
            CswNbtObjClassMaterial ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.MaterialClass ) )
            {
                ret = (CswNbtObjClassMaterial) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        private bool _canReceive()
        {
            return ApprovedForReceiving.Checked == Tristate.True && _CswNbtResources.Permit.can( CswNbtActionName.Receiving );
        }

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            Request.MenuOptions = Requests.Options.ToString();
            Request.State = Requests.Size;

            ViewSDS.State = PropertyName.ViewSDS;
            ViewSDS.MenuOptions = PropertyName.ViewSDS + ",View Other";

            if( ApprovedForReceiving.WasModified )
            {
                Receive.setHidden( value: ApprovedForReceiving.Checked != Tristate.True, SaveToDb: true );
            }

            if( CasNo.WasModified && _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.RegulatoryLists ) && false == IsCopy )
            {
                CswCommaDelimitedString ParentMaterials = new CswCommaDelimitedString();
                getParentMaterials( ref ParentMaterials );
                if( ParentMaterials.Count > 0 ) //this material is used by others as a component...we have no idea how deep the rabbit hole is...make a batch op to update 
                {
                    ParentMaterials.Add( NodeId.ToString() ); //we need to update this material too, so add it to the list
                    CswNbtBatchOpUpdateRegulatoryListsForMaterials BatchOp = new CswNbtBatchOpUpdateRegulatoryListsForMaterials( _CswNbtResources );
                    BatchOp.makeBatchOp( ParentMaterials );
                }
                else //this material isn't used as a component anywhere, so just update it by its self
                {
                    _updateRegulatoryLists();
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
            _CswNbtResources.StructureSearchManager.DeleteFingerprintRecord( this.NodeId.PrimaryKey );

            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            ApprovedForReceiving.setReadOnly( false == _CswNbtResources.Permit.can( CswNbtActionName.Material_Approval ), SaveToDb: false );
            _toggleButtonVisibility();
            PhysicalState.SetOnPropChange( _physicalStatePropChangeHandler );
            CasNo.SetOnPropChange( _onCasNoPropChange );
            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        private void _toggleButtonVisibility()
        {
            Receive.setHidden( value: false == _canReceive(), SaveToDb: false );
            Request.setHidden( value: false == _CswNbtResources.Permit.can( CswNbtActionName.Submit_Request ), SaveToDb: false );
        }

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
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
                    case PropertyName.Request:
                        if( _CswNbtResources.Permit.can( CswNbtActionName.Submit_Request ) )
                        {
                            HasPermission = true;
                            CswNbtActRequesting RequestAct = new CswNbtActRequesting( _CswNbtResources );

                            CswNbtPropertySetRequestItem NodeAsPropSet = RequestAct.makeMaterialRequestItem( new CswNbtActRequesting.RequestItem( CswNbtActRequesting.RequestItem.Material ), NodeId, ButtonData );
                            NodeAsPropSet.postChanges( false );

                            ButtonData.Data["requestaction"] = OCPPropName;
                            ButtonData.Data["titleText"] = ButtonData.SelectedText + " for " + TradeName.Text;
                            ButtonData.Data["requestItemProps"] = RequestAct.getRequestItemAddProps( NodeAsPropSet );
                            ButtonData.Data["requestItemNodeTypeId"] = NodeAsPropSet.NodeTypeId;
                            ButtonData.Action = NbtButtonAction.request;
                        }
                        break;
                    case PropertyName.Receive:
                        if( _CswNbtResources.Permit.can( CswNbtActionName.Receiving ) )
                        {
                            HasPermission = true;
                            ButtonData.Data["state"] = new JObject();
                            ButtonData.Data["state"]["materialId"] = NodeId.ToString();
                            ButtonData.Data["state"]["materialNodeTypeId"] = NodeTypeId;
                            ButtonData.Data["state"]["tradeName"] = TradeName.Text;
                            CswNbtActReceiving Act = new CswNbtActReceiving( _CswNbtResources, ObjectClass, NodeId );
                            //ButtonData.Data["sizesViewId"] = Act.SizesView.SessionViewId.ToString();
                            Int32 ContainerLimit = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.container_receipt_limit.ToString() ) );
                            ButtonData.Data["state"]["containerlimit"] = ContainerLimit;
                            CswNbtObjClassContainer Container = Act.makeContainer();
                            Container.Location.SelectedNodeId = _CswNbtResources.CurrentNbtUser.DefaultLocationId;
                            Container.Owner.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
                            ButtonData.Data["state"]["containerNodeId"] = Container.NodeId.ToString();
                            ButtonData.Data["state"]["containerNodeTypeId"] = Container.NodeTypeId;
                            ButtonData.Data["state"]["containerAddLayout"] = Act.getContainerAddProps( Container );
                            bool customBarcodes = CswConvert.ToBoolean( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.custom_barcodes.ToString() ) );
                            ButtonData.Data["state"]["customBarcodes"] = customBarcodes;
                            ButtonData.Data["state"]["nodetypename"] = this.NodeType.NodeTypeName;

                            CswDateTime CswDate = new CswDateTime( _CswNbtResources, getDefaultExpirationDate() );
                            if( false == CswDate.IsNull )
                            {
                                foreach( JProperty child in ButtonData.Data["state"]["containerAddLayout"].Children() )
                                {
                                    JToken name = child.First.SelectToken( "name" );
                                    if( null != name )
                                    {
                                        if( name.ToString() == "Expiration Date" )
                                        {
                                            ButtonData.Data["state"]["containerAddLayout"][child.Name]["values"]["value"] = CswDate.ToClientAsDateTimeJObject();
                                        }
                                    }
                                }
                            }

                            bool canAddSDS = NodeType.NodeTypeName == "Chemical" &&
                                _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.SDS ) &&
                                ( CswNbtActReceiving.getSDSDocumentNodeTypeId( _CswNbtResources ) != Int32.MinValue );
                            ButtonData.Data["state"]["canAddSDS"] = canAddSDS;
                            if( canAddSDS )
                            {
                                CswNbtMetaDataNodeTypeProp AssignedSDSProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypeId, "Assigned SDS" );
                                if( null != AssignedSDSProp )
                                {
                                    ButtonData.Data["state"]["sdsViewId"] = AssignedSDSProp.ViewId.ToString();
                                }
                            }

                            ButtonData.Action = NbtButtonAction.receive;

                            Container.postChanges( false );

                        }
                        break;
                    case PropertyName.ViewSDS:
                        HasPermission = true;
                        GetMatchingSDSForCurrentUser( ButtonData );
                        break;
                    case CswNbtObjClass.PropertyName.Save:
                        HasPermission = true;
                        break;
                }
                if( false == HasPermission )
                {
                    throw new CswDniException( ErrorType.Warning, "You do not have permission to the " + OCPPropName + " action.", "You do not have permission to the " + OCPPropName + " action." );
                }
            }

            return true;
        }
        #endregion

        #region Custom Logic

        /// <summary>
        /// Calculates the expiration date from today based on the Material's Expiration Interval
        /// </summary>
        public DateTime getDefaultExpirationDate()
        {
            DateTime DefaultExpDate = DateTime.MinValue;
            
            //No point trying to get default if both values are invalid
            if( CswTools.IsPrimaryKey( ExpirationInterval.UnitId ) && ExpirationInterval.Quantity > 0 )
            {
                DefaultExpDate = DateTime.Now;
                switch( this.ExpirationInterval.CachedUnitName.ToLower() )
                {
                    case "seconds":
                        DefaultExpDate = DefaultExpDate.AddSeconds( this.ExpirationInterval.Quantity );
                        break;
                    case "minutes":
                        DefaultExpDate = DefaultExpDate.AddMinutes( this.ExpirationInterval.Quantity );
                        break;
                    case "hours":
                        DefaultExpDate = DefaultExpDate.AddHours( this.ExpirationInterval.Quantity );
                        break;
                    case "days":
                        DefaultExpDate = DefaultExpDate.AddDays( this.ExpirationInterval.Quantity );
                        break;
                    case "weeks":
                        DefaultExpDate = DefaultExpDate.AddDays( this.ExpirationInterval.Quantity * 7 );
                        break;
                    case "months":
                        DefaultExpDate = DefaultExpDate.AddMonths( CswConvert.ToInt32( this.ExpirationInterval.Quantity ) );
                        break;
                    case "years":
                        DefaultExpDate = DefaultExpDate.AddYears( CswConvert.ToInt32( this.ExpirationInterval.Quantity ) );
                        break;
                    default:
                        DefaultExpDate = DateTime.MinValue;
                        break;
                }
            }
            return DefaultExpDate;
        }

        private void _updateRegulatoryLists()
        {
            RegulatoryLists.StaticText = "";

            if( false == String.IsNullOrEmpty( CasNo.Text ) ) //if the CASNo is empty we don't both matching
            {
                CswNbtMetaDataObjectClass regListOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RegulatoryListClass );
                CswNbtMetaDataObjectClassProp casNosOCP = regListOC.getObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.CASNumbers );

                CswNbtView matchingRegLists = new CswNbtView( _CswNbtResources );
                CswNbtViewRelationship parent = matchingRegLists.AddViewRelationship( regListOC, true );
                matchingRegLists.AddViewPropertyAndFilter( parent, casNosOCP,
                    Value: CasNo.Text,
                    FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Contains );

                ICswNbtTree tree = _CswNbtResources.Trees.getTreeFromView( matchingRegLists, true, false, false );
                int childCount = tree.getChildNodeCount();

                CswCommaDelimitedString regLists = new CswCommaDelimitedString();
                for( int i = 0; i < childCount; i++ )
                {
                    tree.goToNthChild( i );
                    regLists.Add( tree.getNodeNameForCurrentPosition() );
                    tree.goToParentNode();
                }
                RegulatoryLists.StaticText = regLists.ToString();
            }
        }

        /// <summary>
        /// Gets all the node ids of materials that use this material as a component
        /// </summary>
        /// <returns></returns>
        public void getParentMaterials( ref CswCommaDelimitedString MachingMaterialIDs )
        {
            CswNbtMetaDataObjectClass materialComponentOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialComponentClass );
            CswNbtMetaDataObjectClassProp constituentOCP = materialComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Constituent );
            CswNbtMetaDataObjectClassProp mixtureOCP = materialComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Mixture );

            CswNbtView componentsView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = componentsView.AddViewRelationship( materialComponentOC, false );
            componentsView.AddViewPropertyAndFilter( parent, constituentOCP,
                Value: NodeId.PrimaryKey.ToString(),
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals,
                SubFieldName: CswNbtSubField.SubFieldName.NodeID );
            componentsView.AddViewRelationship( parent, NbtViewPropOwnerType.First, mixtureOCP, false );

            ICswNbtTree componentsTree = _CswNbtResources.Trees.getTreeFromView( componentsView, false, false, false );
            int nodesCount = componentsTree.getChildNodeCount();
            for( int i = 0; i < nodesCount; i++ )
            {
                componentsTree.goToNthChild( i );
                int childNodesCount = componentsTree.getChildNodeCount();
                for( int c = 0; c < childNodesCount; c++ )
                {
                    componentsTree.goToNthChild( c );
                    MachingMaterialIDs.Add( componentsTree.getNodeIdForCurrentPosition().ToString() ); //the mixture node id
                    componentsTree.goToParentNode();
                }
                componentsTree.goToParentNode();
            }
        }

        public static CswNbtView getMaterialNodeView( CswNbtResources NbtResources, CswNbtNode MaterialNode )
        {
            CswNbtView Ret = null;
            if( MaterialNode != null )
            {
                Ret = MaterialNode.getViewOfNode();
                if( NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Containers ) )
                {
                    CswNbtMetaDataObjectClass SizeOc = NbtResources.MetaData.getObjectClass( NbtObjectClass.SizeClass );
                    CswNbtMetaDataObjectClassProp MaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
                    Ret.AddViewRelationship( Ret.Root.ChildRelationships[0], NbtViewPropOwnerType.Second, MaterialOcp, false );
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
                throw new CswDniException( ErrorType.Error,
                                           "Cannot get a material without a type, a supplier and a tradename.",
                                           "Attempted to call _getMaterialNodeView with invalid or empty parameters. Type: " + NodeTypeId + ", Tradename: " + Tradename + ", SupplierId: " + SupplierId );
            }

            CswNbtView Ret = new CswNbtView( NbtResources );
            Ret.ViewMode = NbtViewRenderingMode.Tree;
            Ret.Visibility = NbtViewVisibility.User;
            Ret.VisibilityUserId = NbtResources.CurrentNbtUser.UserId;
            CswNbtMetaDataNodeType MaterialNt = NbtResources.MetaData.getNodeType( NodeTypeId );
            CswNbtViewRelationship MaterialRel = Ret.AddViewRelationship( MaterialNt, false );
            CswNbtMetaDataNodeTypeProp TradeNameNtp = MaterialNt.getNodeTypePropByObjectClassProp( PropertyName.Tradename );
            CswNbtMetaDataNodeTypeProp SupplierNtp = MaterialNt.getNodeTypePropByObjectClassProp( PropertyName.Supplier );
            CswNbtMetaDataNodeTypeProp PartNoNtp = MaterialNt.getNodeTypePropByObjectClassProp( PropertyName.PartNumber );

            Ret.AddViewPropertyAndFilter( MaterialRel, TradeNameNtp, Tradename );
            Ret.AddViewPropertyAndFilter( MaterialRel, SupplierNtp, SupplierId.PrimaryKey.ToString(), CswNbtSubField.SubFieldName.NodeID );
            Ret.AddViewPropertyAndFilter( MaterialRel, PartNoNtp, PartNo );

            if( NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Containers ) )
            {
                CswNbtMetaDataObjectClass SizeOc = NbtResources.MetaData.getObjectClass( NbtObjectClass.SizeClass );
                CswNbtMetaDataObjectClassProp MaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
                Ret.AddViewRelationship( MaterialRel, NbtViewPropOwnerType.Second, MaterialOcp, false );
            }

            Ret.ViewName = "New Material: " + Tradename;

            return Ret;
        }

        /// <summary>
        /// Fetch a Material node by NodeTypeId, TradeName, Supplier and PartNo (Optional). This method will throw if required parameters are null or empty.
        /// </summary>
        public static CswNbtObjClassMaterial getExistingMaterial( CswNbtResources NbtResources, Int32 MaterialNodeTypeId, CswPrimaryKey SupplierId, string TradeName, string PartNo )
        {
            CswNbtObjClassMaterial Ret = null;

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

        public void GetMatchingSDSForCurrentUser( NbtButtonData ButtonData )
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.SDS ) &&
                ( CswNbtActReceiving.getSDSDocumentNodeTypeId( _CswNbtResources ) != Int32.MinValue ) )
            {
                Int32 SDSDocumentNTId = CswNbtActReceiving.getSDSDocumentNodeTypeId( _CswNbtResources );
                CswNbtMetaDataNodeType SDSDocumentNT = _CswNbtResources.MetaData.getNodeType( SDSDocumentNTId );
                CswNbtMetaDataNodeTypeProp archivedNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Archived );
                CswNbtMetaDataNodeTypeProp formatNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Format );
                CswNbtMetaDataNodeTypeProp languageNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Language );
                CswNbtMetaDataNodeTypeProp fileTypeNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.FileType );
                CswNbtMetaDataNodeTypeProp fileNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.File );
                CswNbtMetaDataNodeTypeProp linkNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Link );
                CswNbtMetaDataNodeTypeProp ownerNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Owner );

                CswNbtView docView = new CswNbtView( _CswNbtResources );
                CswNbtViewRelationship parent = docView.AddViewRelationship( SDSDocumentNT, true );

                docView.AddViewPropertyAndFilter( parent,
                                                 MetaDataProp: archivedNTP,
                                                 SubFieldName: CswNbtSubField.SubFieldName.Checked,
                                                 Value: false.ToString(),
                                                 FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

                docView.AddViewPropertyAndFilter( parent,
                                                 MetaDataProp: ownerNTP,
                                                 SubFieldName: CswNbtSubField.SubFieldName.NodeID,
                                                 Value: NodeId.PrimaryKey.ToString(),
                                                 FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

                docView.AddViewProperty( parent, formatNTP );
                docView.AddViewProperty( parent, languageNTP );
                docView.AddViewProperty( parent, fileNTP );
                docView.AddViewProperty( parent, linkNTP );
                docView.AddViewProperty( parent, fileTypeNTP );

                CswNbtObjClassUser currentUserNode = _CswNbtResources.Nodes[_CswNbtResources.CurrentNbtUser.UserId];
                CswNbtObjClassJurisdiction userJurisdictionNode = _CswNbtResources.Nodes[currentUserNode.JurisdictionProperty.RelatedNodeId];

                if( ButtonData.SelectedText.Equals( PropertyName.ViewSDS ) )
                {

                    ICswNbtTree docsTree = _CswNbtResources.Trees.getTreeFromView( docView, false, false, false );
                    int childCount = docsTree.getChildNodeCount();
                    int lvlMatched = Int32.MinValue;
                    string matchedFileType = "";
                    CswNbtTreeNodeProp matchedFileProp = null;
                    CswNbtTreeNodeProp matchedLinkProp = null;
                    CswPrimaryKey matchedNodeId = null;

                    if( childCount > 0 )
                    {
                        for( int i = 0; i < childCount; i++ )
                        {
                            docsTree.goToNthChild( i );

                            string format = "";
                            string language = "";
                            string fileType = "";
                            CswNbtTreeNodeProp fileProp = null;
                            CswNbtTreeNodeProp linkProp = null;
                            CswPrimaryKey nodeId = docsTree.getNodeIdForCurrentPosition();

                            foreach( CswNbtTreeNodeProp prop in docsTree.getChildNodePropsOfNode() )
                            {
                                CswNbtMetaDataNodeTypeProp docNTP = _CswNbtResources.MetaData.getNodeTypeProp( prop.NodeTypePropId );
                                switch( docNTP.getObjectClassPropName() )
                                {
                                    case CswNbtObjClassDocument.PropertyName.Format:
                                        format = prop.Field1;
                                        break;
                                    case CswNbtObjClassDocument.PropertyName.Language:
                                        language = prop.Field1;
                                        break;
                                    case CswNbtObjClassDocument.PropertyName.FileType:
                                        fileType = prop.Field1;
                                        break;
                                    case CswNbtObjClassDocument.PropertyName.File:
                                        fileProp = prop;
                                        break;
                                    case CswNbtObjClassDocument.PropertyName.Link:
                                        linkProp = prop;
                                        break;
                                }
                            }

                            if( lvlMatched < 0 )
                            {
                                matchedFileType = fileType;
                                matchedFileProp = fileProp;
                                matchedLinkProp = linkProp;
                                matchedNodeId = nodeId;
                                lvlMatched = 0;
                            }
                            if( null != userJurisdictionNode )
                            {
                                if( lvlMatched < 1 && format.Equals( userJurisdictionNode.Format.Value ) )
                                {
                                    matchedFileType = fileType;
                                    matchedFileProp = fileProp;
                                    matchedLinkProp = linkProp;
                                    matchedNodeId = nodeId;
                                    lvlMatched = 1;
                                }
                                if( lvlMatched < 2 && language.Equals( currentUserNode.Language ) )
                                {
                                    matchedFileType = fileType;
                                    matchedFileProp = fileProp;
                                    matchedLinkProp = linkProp;
                                    matchedNodeId = nodeId;
                                    lvlMatched = 2;
                                }
                                if( lvlMatched < 3 && format.Equals( userJurisdictionNode.Format.Value ) && language.Equals( currentUserNode.Language ) )
                                {
                                    matchedFileType = fileType;
                                    matchedFileProp = fileProp;
                                    matchedLinkProp = linkProp;
                                    matchedNodeId = nodeId;
                                    lvlMatched = 3;
                                }
                            }
                            docsTree.goToParentNode();
                        }

                        string url = "";
                        switch( matchedFileType )
                        {
                            case CswNbtObjClassDocument.FileTypes.File:
                                int jctnodepropid = CswConvert.ToInt32( matchedFileProp.JctNodePropId );
                                int nodetypepropid = CswConvert.ToInt32( matchedFileProp.NodeTypePropId );
                                url = CswNbtNodePropBlob.getLink( jctnodepropid, matchedNodeId, nodetypepropid );
                                break;
                            case CswNbtObjClassDocument.FileTypes.Link:
                                //CswNbtMetaDataNodeTypeProp linkNTP = _CswNbtResources.MetaData.getNodeTypeProp( matchedLinkProp.NodeTypePropId );
                                url = CswNbtNodePropLink.GetFullURL( linkNTP.Attribute1, matchedLinkProp.Field2, linkNTP.Attribute2 );
                                break;
                        }
                        ButtonData.Data["url"] = url;
                        ButtonData.Action = NbtButtonAction.popup;
                    }
                    else
                    {
                        ButtonData.Message = "There are no active SDS assigned to this " + NodeType.NodeTypeName;
                        ButtonData.Action = NbtButtonAction.nothing;
                    }
                }
                else //load Assigned SDS grid dialog
                {
                    CswNbtMetaDataNodeTypeProp assignedSDSNTP = NodeType.getNodeTypeProp( "Assigned SDS" );
                    if( null != assignedSDSNTP )
                    {
                        ButtonData.Data["viewid"] = assignedSDSNTP.ViewId.ToString();
                        ButtonData.Data["title"] = assignedSDSNTP.PropName;
                        ButtonData.Data["nodeid"] = NodeId.ToString();
                        ButtonData.Data["nodetypeid"] = NodeTypeId.ToString();
                        ButtonData.Action = NbtButtonAction.griddialog;
                    }
                    else
                    {
                        ButtonData.Message = "Could not find the Assigned SDS prop";
                        ButtonData.Action = NbtButtonAction.nothing;
                    }
                }
            }
        }

        public void syncFireDbData()
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.FireDbSync ) )
            {
                CswC3SearchParams CswC3SearchParams = new CswC3SearchParams();
                CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3SearchParams );
                ChemCatCentral.SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();

                // Set FireDb specific properties
                CswC3SearchParams.Purpose = "FireDb";
                CswC3SearchParams.SyncType = "CasNo";
                CswC3SearchParams.SyncKey = this.CasNo.Text;

                CswRetObjSearchResults SearchResults = C3SearchClient.getExtChemData( CswC3SearchParams );
                if( null != SearchResults.ExtChemDataResults )
                {
                    if( SearchResults.ExtChemDataResults.Length > 0 )
                    {
                        CswCommaDelimitedString CurrentHazardClasses = new CswCommaDelimitedString();
                        CurrentHazardClasses = this.HazardClasses.Value;

                        CswCommaDelimitedString UpdatedHazardClasses = new CswCommaDelimitedString();

                        ChemCatCentral.CswC3ExtChemData C3ExtChemData = SearchResults.ExtChemDataResults[0];
                        foreach( CswC3ExtChemData.UfcHazardClass UfcHazardClass in C3ExtChemData.ExtensionData1.UfcHazardClasses )
                        {
                            if( false == CurrentHazardClasses.Contains( UfcHazardClass.HazardClass ) )
                            {
                                UpdatedHazardClasses.Add( UfcHazardClass.HazardClass );
                            }
                        }

                        // Add the original hazard classes to the new list
                        foreach( string HazardClass in CurrentHazardClasses )
                        {
                            UpdatedHazardClasses.Add( HazardClass );
                        }

                        // Set the value of the property to the new list
                        this.HazardClasses.Value = UpdatedHazardClasses;
                    }
                }

                // Set the C3SyncDate property
                this.C3SyncDate.DateTimeValue = DateTime.Now;
            }
        }

        #endregion Custom Logic

        #region Object class specific properties

        public CswNbtNodePropRelationship Supplier { get { return ( _CswNbtNode.Properties[PropertyName.Supplier] ); } }
        public CswNbtNodePropText PartNumber { get { return ( _CswNbtNode.Properties[PropertyName.PartNumber] ); } }
        public CswNbtNodePropNumber SpecificGravity { get { return ( _CswNbtNode.Properties[PropertyName.SpecificGravity] ); } }
        public CswNbtNodePropList PhysicalState { get { return ( _CswNbtNode.Properties[PropertyName.PhysicalState] ); } }
        public CswNbtNodePropCASNo CasNo { get { return ( _CswNbtNode.Properties[PropertyName.CasNo] ); } }
        private void _onCasNoPropChange( CswNbtNodeProp Prop )
        {
            if( CasNo.GetOriginalPropRowValue() != CasNo.Text )
            {
                CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources );
                bool C3ServiceStatus = CswNbtC3ClientManager.checkC3ServiceReferenceStatus( _CswNbtResources );
                if( C3ServiceStatus )
                {
                    syncFireDbData();
                }
            }
        }

        public CswNbtNodePropStatic RegulatoryLists { get { return ( _CswNbtNode.Properties[PropertyName.RegulatoryLists] ); } }
        public CswNbtNodePropText TradeName { get { return ( _CswNbtNode.Properties[PropertyName.Tradename] ); } }
        public CswNbtNodePropImageList StorageCompatibility { get { return ( _CswNbtNode.Properties[PropertyName.StorageCompatibility] ); } }
        public CswNbtNodePropQuantity ExpirationInterval { get { return ( _CswNbtNode.Properties[PropertyName.ExpirationInterval] ); } }
        public CswNbtNodePropButton Request { get { return ( _CswNbtNode.Properties[PropertyName.Request] ); } }
        private void _physicalStatePropChangeHandler( CswNbtNodeProp prop )
        {
            if( false == String.IsNullOrEmpty( PhysicalState.Value ) )
            {
                CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
                CswNbtView unitsOfMeasureView = Vb.getQuantityUnitOfMeasureView( _CswNbtNode.NodeId );
                if( null != unitsOfMeasureView )
                {
                    unitsOfMeasureView.save();
                }
            }
        }
        public CswNbtNodePropButton Receive { get { return ( _CswNbtNode.Properties[PropertyName.Receive] ); } }
        public CswNbtNodePropSequence MaterialId { get { return ( _CswNbtNode.Properties[PropertyName.MaterialId] ); } }
        public CswNbtNodePropLogical ApprovedForReceiving { get { return ( _CswNbtNode.Properties[PropertyName.ApprovedForReceiving] ); } }
        public CswNbtNodePropText UNCode { get { return ( _CswNbtNode.Properties[PropertyName.UNCode] ); } }
        public CswNbtNodePropLogical IsTierII { get { return ( _CswNbtNode.Properties[PropertyName.IsTierII] ); } }
        public CswNbtNodePropButton ViewSDS { get { return ( _CswNbtNode.Properties[PropertyName.ViewSDS] ); } }
        public CswNbtNodePropText C3ProductId { get { return ( _CswNbtNode.Properties[PropertyName.C3ProductId] ); } }
        public CswNbtNodePropDateTime C3SyncDate { get { return ( _CswNbtNode.Properties[PropertyName.C3SyncDate] ); } }
        public CswNbtNodePropMultiList HazardClasses { get { return ( _CswNbtNode.Properties[PropertyName.HazardClasses] ); } }

        #endregion
    }//CswNbtObjClassMaterial

}//namespace ChemSW.Nbt.ObjClasses
