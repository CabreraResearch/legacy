using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.ChemCatCentral;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMaterial : CswNbtPropertySetMaterial
    {
        #region Base

        /// <summary>
        /// Ctor
        /// </summary>
        public CswNbtObjClassMaterial( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        /// <summary>
        /// Implicit cast of Node to Object Class
        /// </summary>
        public static implicit operator CswNbtObjClassMaterial( CswNbtNode Node )
        {
            CswNbtObjClassMaterial ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.MaterialClass ) )
            {
                ret = (CswNbtObjClassMaterial) Node.ObjClass;
            }
            return ret;
        }

        /// <summary>
        /// Object Class
        /// </summary>
        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialClass ); }
        }

        /// <summary>
        /// Cast a Request Item PropertySet back to an Object Class
        /// </summary>
        public static CswNbtObjClassMaterial fromPropertySet( CswNbtPropertySetMaterial PropertySet )
        {
            return PropertySet.Node;
        }

        /// <summary>
        /// Cast a the Object Class as a PropertySet
        /// </summary>
        public static CswNbtPropertySetMaterial toPropertySet( CswNbtObjClassMaterial ObjClass )
        {
            return ObjClass;
        }

        #endregion Base

        #region Enums

        public new sealed class PropertyName : CswNbtPropertySetMaterial.PropertyName
        {
            public const string CasNo = "CAS No";
            public const string RegulatoryLists = "Regulatory Lists";
            public const string UNCode = "UN Code";
            public const string IsTierII = "Is Tier II";
            public const string ViewSDS = "View SDS";
            public const string C3ProductId = "C3ProductId";
            public const string C3SyncDate = "C3SyncDate";
            public const string HazardClasses = "Hazard Classes";
        }

        #endregion Enums

        #region Inherited Events

        public override void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            ViewSDS.State = PropertyName.ViewSDS;
            ViewSDS.MenuOptions = PropertyName.ViewSDS + ",View Other";

            if( CasNo.WasModified && _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.RegulatoryLists ) && false == IsCopy )
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
        }

        public override void afterPropertySetWriteNode() {}

        public override void beforePropertySetDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtResources.StructureSearchManager.DeleteFingerprintRecord( this.NodeId.PrimaryKey );
        }

        public override void afterPropertySetDeleteNode() {}

        public override void afterPropertySetPopulateProps()
        {
            CasNo.SetOnPropChange( _onCasNoPropChange );
        }

        /// <summary>
        /// Abstract override to be called on onButtonClick
        /// </summary>
        public override bool onPropertySetButtonClick( NbtButtonData ButtonData )
        {
            bool HasPermission = true;
            if( null != ButtonData.NodeTypeProp )
            {
                HasPermission = false;
                string OCPPropName = ButtonData.NodeTypeProp.getObjectClassPropName();
                switch( OCPPropName )
                {
                    case PropertyName.ViewSDS:
                        HasPermission = true;
                        GetMatchingSDSForCurrentUser( ButtonData );
                        break;
                }
            }
            return HasPermission;
        }

        public override void onPropertySetAddDefaultViewFilters( CswNbtViewRelationship ParentRelationship ) {}

        #endregion Inherited Events

        #region Custom Logic

        private void _updateRegulatoryLists()
        {
            RegulatoryLists.StaticText = "";

            if( false == String.IsNullOrEmpty( CasNo.Text ) ) //if the CASNo is empty we don't both matching
            {
                CswNbtMetaDataObjectClass regListOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
                CswNbtMetaDataObjectClassProp casNosOCP = regListOC.getObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.CASNumbers );

                CswNbtView matchingRegLists = new CswNbtView( _CswNbtResources );
                CswNbtViewRelationship parent = matchingRegLists.AddViewRelationship( regListOC, true );
                matchingRegLists.AddViewPropertyAndFilter( parent, casNosOCP,
                    Value: CasNo.Text,
                    FilterMode: CswEnumNbtFilterMode.Contains );

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
            CswNbtMetaDataObjectClass materialComponentOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialComponentClass );
            CswNbtMetaDataObjectClassProp constituentOCP = materialComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Constituent );
            CswNbtMetaDataObjectClassProp mixtureOCP = materialComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Mixture );

            CswNbtView componentsView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = componentsView.AddViewRelationship( materialComponentOC, false );
            componentsView.AddViewPropertyAndFilter( parent, constituentOCP,
                Value: NodeId.PrimaryKey.ToString(),
                FilterMode: CswEnumNbtFilterMode.Equals,
                SubFieldName: CswEnumNbtSubFieldName.NodeID );
            componentsView.AddViewRelationship( parent, CswEnumNbtViewPropOwnerType.First, mixtureOCP, false );

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
            Ret.AddViewPropertyAndFilter( MaterialRel, PartNoNtp, PartNo );

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
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.SDS ) &&
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
                                                 SubFieldName: CswEnumNbtSubFieldName.Checked,
                                                 Value: false.ToString(),
                                                 FilterMode: CswEnumNbtFilterMode.Equals );

                docView.AddViewPropertyAndFilter( parent,
                                                 MetaDataProp: ownerNTP,
                                                 SubFieldName: CswEnumNbtSubFieldName.NodeID,
                                                 Value: NodeId.PrimaryKey.ToString(),
                                                 FilterMode: CswEnumNbtFilterMode.Equals );

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
                        ButtonData.Action = CswEnumNbtButtonAction.popup;
                    }
                    else
                    {
                        ButtonData.Message = "There are no active SDS assigned to this " + NodeType.NodeTypeName;
                        ButtonData.Action = CswEnumNbtButtonAction.nothing;
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
                        ButtonData.Action = CswEnumNbtButtonAction.griddialog;
                    }
                    else
                    {
                        ButtonData.Message = "Could not find the Assigned SDS prop";
                        ButtonData.Action = CswEnumNbtButtonAction.nothing;
                    }
                }
            }
        }

        public void syncFireDbData()
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.FireDbSync ) )
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
        public CswNbtNodePropText UNCode { get { return ( _CswNbtNode.Properties[PropertyName.UNCode] ); } }
        public CswNbtNodePropLogical IsTierII { get { return ( _CswNbtNode.Properties[PropertyName.IsTierII] ); } }
        public CswNbtNodePropButton ViewSDS { get { return ( _CswNbtNode.Properties[PropertyName.ViewSDS] ); } }
        public CswNbtNodePropText C3ProductId { get { return ( _CswNbtNode.Properties[PropertyName.C3ProductId] ); } }
        public CswNbtNodePropDateTime C3SyncDate { get { return ( _CswNbtNode.Properties[PropertyName.C3SyncDate] ); } }
        public CswNbtNodePropMultiList HazardClasses { get { return ( _CswNbtNode.Properties[PropertyName.HazardClasses] ); } }

        #endregion
    }//CswNbtObjClassMaterial

}//namespace ChemSW.Nbt.ObjClasses