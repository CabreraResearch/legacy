using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.ChemCatCentral;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.UnitsOfMeasure;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassChemical : CswNbtPropertySetMaterial
    {
        #region Base

        /// <summary>
        /// Ctor
        /// </summary>
        public CswNbtObjClassChemical( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        /// <summary>
        /// Implicit cast of Node to Object Class
        /// </summary>
        public static implicit operator CswNbtObjClassChemical( CswNbtNode Node )
        {
            CswNbtObjClassChemical ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.ChemicalClass ) )
            {
                ret = (CswNbtObjClassChemical) Node.ObjClass;
            }
            return ret;
        }

        /// <summary>
        /// Object Class
        /// </summary>
        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass ); }
        }

        /// <summary>
        /// Cast a Request Item PropertySet back to an Object Class
        /// </summary>
        public static CswNbtObjClassChemical fromPropertySet( CswNbtPropertySetMaterial PropertySet )
        {
            return PropertySet.Node;
        }

        /// <summary>
        /// Cast a the Object Class as a PropertySet
        /// </summary>
        public static CswNbtPropertySetMaterial toPropertySet( CswNbtObjClassChemical ObjClass )
        {
            return ObjClass;
        }

        #endregion Base

        #region Enums

        public new sealed class PropertyName : CswNbtPropertySetMaterial.PropertyName
        {
            public const string PhysicalState = "Physical State";
            public const string SpecificGravity = "Specific Gravity";
            public const string StorageCompatibility = "Storage Compatibility";
            public const string ExpirationInterval = "Expiration Interval";
            public const string CasNo = "CAS No";
            public const string RegulatoryLists = "Regulatory Lists";
            public const string UNCode = "UN Code";
            public const string IsTierII = "Is Tier II";
            public const string ViewSDS = "View SDS";
            public const string HazardClasses = "Hazard Classes";
            public const string NFPA = "NFPA";
            public const string PPE = "PPE";
            public const string Hazardous = "Hazardous";
            public const string Formula = "Formula";
            public const string Structure = "Structure";
            public const string PhysicalDescription = "Physical Description";
            public const string MolecularWeight = "Molecular Weight";
            public const string pH = "pH";
            public const string BoilingPoint = "Boiling Point";
            public const string MeltingPoint = "Melting Point";
            public const string AqueousSolubility = "Aqueous Solubility";
            public const string FlashPoint = "Flash Point";
            public const string VaporPressure = "Vapor Pressure";
            public const string VaporDensity = "Vapor Density";
            public const string StorageAndHandling = "Storage and Handling";
            public const string Isotope = "Isotope";
            public const string MaterialType = "Material Type";
            public const string SpecialFlags = "Special Flags";
            public const string HazardCategories = "Hazard Categories";
            public const string Jurisdiction = "Jurisdiction";
            public const string LegacyId = "Legacy Id";
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

        public override void afterPropertySetWriteNode() { }

        public override void beforePropertySetDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtResources.StructureSearchManager.DeleteFingerprintRecord( this.NodeId.PrimaryKey );
        }

        public override void afterPropertySetDeleteNode() { }

        public override void afterPropertySetPopulateProps()
        {
            PhysicalState.SetOnPropChange( _onPhysicalStatePropChange );
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

        /// <summary>
        /// Abstract override to be called on onButtonClick
        /// </summary>
        public override void onReceiveButtonClick( NbtButtonData ButtonData )
        {
            Int32 SDSNodeTypeId = CswNbtActReceiving.getSDSDocumentNodeTypeId( _CswNbtResources );
            bool canAddSDS = _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.SDS ) &&
                             SDSNodeTypeId != Int32.MinValue;
            ButtonData.Data["state"]["canAddSDS"] = canAddSDS;
            if( canAddSDS )
            {
                ButtonData.Data["state"]["documentTypeId"] = SDSNodeTypeId;
                CswNbtMetaDataNodeTypeProp AssignedSDSProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypeId, "Assigned SDS" );
                CswNbtMetaDataNodeTypeProp RevisionDateProp = _CswNbtResources.MetaData.getNodeTypeProp( SDSNodeTypeId, "Revision Date" );
                if( null != AssignedSDSProp )
                {
                    CswNbtView AssignedSDSView = _CswNbtResources.ViewSelect.restoreView( AssignedSDSProp.ViewId );
                    ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( AssignedSDSView, false, false, false );
                    for( Int32 i = 0; i < Tree.getChildNodeCount(); i++ )
                    {
                        Tree.goToNthChild( i );
                        if( Tree.getNodeIdForCurrentPosition() == NodeId )
                        {
                            break;
                        }
                        Tree.goToParentNode();
                    }
                    Int32 NodeCount = Tree.getChildNodeCount();
                    JArray SDSDocs = new JArray();
                    if( NodeCount > 0 )
                    {
                        if( NodeCount > 0 )
                        {
                            for( Int32 i = 0; i < NodeCount; i++ )
                            {
                                Tree.goToNthChild( i );
                                JObject Doc = new JObject();

                                CswNbtObjClassDocument SDSDoc = Tree.getNodeForCurrentPosition();
                                if( null != RevisionDateProp )
                                {
                                    DateTime RevisionDate = SDSDoc.Node.Properties[RevisionDateProp].AsDateTime.DateTimeValue;
                                    Doc["revisiondate"] = RevisionDate == DateTime.MinValue ? "" : RevisionDate.ToShortDateString();
                                }
                                else
                                {
                                    Doc["revisiondate"] = "";
                                }
                                if( SDSDoc.FileType.Value.Equals( CswNbtObjClassDocument.FileTypes.File ) )
                                {
                                    Doc["displaytext"] = SDSDoc.File.FileName;
                                    Doc["linktext"] = SDSDoc.File.Href;
                                }
                                else
                                {
                                    Doc["displaytext"] = String.IsNullOrEmpty( SDSDoc.Link.Text ) ? SDSDoc.Link.GetFullURL() : SDSDoc.Link.Text;
                                    Doc["linktext"] = SDSDoc.Link.GetFullURL();
                                }
                                SDSDocs.Add( Doc );
                                Tree.goToParentNode();
                            }
                        }
                    }
                    ButtonData.Data["state"]["sdsDocs"] = SDSDocs;
                }
            }
        }

        public override void onPropertySetAddDefaultViewFilters( CswNbtViewRelationship ParentRelationship ) { }

        #endregion Inherited Events

        #region Custom Logic

        /// <summary>
        /// Calculates the expiration date from today based on the Material's Expiration Interval
        /// </summary>
        public override DateTime getDefaultExpirationDate()
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
                                url = CswNbtNodePropBlob.getLink( jctnodepropid, matchedNodeId );
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
        }//syncFireDbData()

        public void syncPCIDData()
        {
            //if the module is enabled
            CswC3SearchParams CswC3SearchParams = new CswC3SearchParams();
            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3SearchParams );
            ChemCatCentral.SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();

            // Set PCID specific properties
            CswC3SearchParams.Purpose = "PCID";
            CswC3SearchParams.SyncType = "CasNo";
            CswC3SearchParams.SyncKey = this.CasNo.Text;

            CswRetObjSearchResults SearchResults = C3SearchClient.getExtChemData( CswC3SearchParams );
            if( null != SearchResults.ExtChemDataResults )
            {
                if( SearchResults.ExtChemDataResults.Length > 0 )
                {
                    //todo: Set NFPA

                    //todo: Set PPE

                    //todo: Set Storage Compatibility

                    //todo: Set any additional properties ONLY IF they have an empty value. For now the following- structure, formula, density, mp, bp, physical description, tier II
                }
            }

            // Set the C3SyncDate property
            this.C3SyncDate.DateTimeValue = DateTime.Now;
        }

        #endregion Custom Logic

        #region Object class specific properties

        public CswNbtNodePropList PhysicalState { get { return _CswNbtNode.Properties[PropertyName.PhysicalState]; } }
        private void _onPhysicalStatePropChange( CswNbtNodeProp prop )
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
        public CswNbtNodePropNumber SpecificGravity { get { return _CswNbtNode.Properties[PropertyName.SpecificGravity]; } }
        public CswNbtNodePropImageList StorageCompatibility { get { return ( _CswNbtNode.Properties[PropertyName.StorageCompatibility] ); } }
        public CswNbtNodePropQuantity ExpirationInterval { get { return ( _CswNbtNode.Properties[PropertyName.ExpirationInterval] ); } }
        public CswNbtNodePropCASNo CasNo { get { return ( _CswNbtNode.Properties[PropertyName.CasNo] ); } }
        private void _onCasNoPropChange( CswNbtNodeProp Prop )
        {
            if( CasNo.GetOriginalPropRowValue() != CasNo.Text )
            {
                CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources );
                bool C3ServiceStatus = CswNbtC3ClientManager.checkC3ServiceReferenceStatus();
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
        public CswNbtNodePropMultiList HazardClasses { get { return ( _CswNbtNode.Properties[PropertyName.HazardClasses] ); } }
        public CswNbtNodePropNFPA NFPA { get { return ( _CswNbtNode.Properties[PropertyName.NFPA] ); } }
        public CswNbtNodePropMultiList PPE { get { return ( _CswNbtNode.Properties[PropertyName.PPE] ); } }
        public CswNbtNodePropLogical Hazardous { get { return ( _CswNbtNode.Properties[PropertyName.Hazardous] ); } }
        public CswNbtNodePropText Formula { get { return ( _CswNbtNode.Properties[PropertyName.Formula] ); } }
        public CswNbtNodePropMol Structure { get { return ( _CswNbtNode.Properties[PropertyName.Structure] ); } }
        public CswNbtNodePropMemo PhysicalDescription { get { return ( _CswNbtNode.Properties[PropertyName.PhysicalDescription] ); } }
        public CswNbtNodePropText MolecularWeight { get { return ( _CswNbtNode.Properties[PropertyName.MolecularWeight] ); } }
        public CswNbtNodePropText pH { get { return ( _CswNbtNode.Properties[PropertyName.pH] ); } }
        public CswNbtNodePropText BoilingPoint { get { return ( _CswNbtNode.Properties[PropertyName.BoilingPoint] ); } }
        public CswNbtNodePropText MeltingPoint { get { return ( _CswNbtNode.Properties[PropertyName.MeltingPoint] ); } }
        public CswNbtNodePropText AqueousSolubility { get { return ( _CswNbtNode.Properties[PropertyName.AqueousSolubility] ); } }
        public CswNbtNodePropText FlashPoint { get { return ( _CswNbtNode.Properties[PropertyName.FlashPoint] ); } }
        public CswNbtNodePropText VaporPressure { get { return ( _CswNbtNode.Properties[PropertyName.VaporPressure] ); } }
        public CswNbtNodePropText VaporDensity { get { return ( _CswNbtNode.Properties[PropertyName.VaporDensity] ); } }
        public CswNbtNodePropMemo StorageAndHandling { get { return ( _CswNbtNode.Properties[PropertyName.StorageAndHandling] ); } }
        public CswNbtNodePropText Isotope { get { return ( _CswNbtNode.Properties[PropertyName.Isotope] ); } }
        public CswNbtNodePropList MaterialType { get { return ( _CswNbtNode.Properties[PropertyName.MaterialType] ); } }
        public CswNbtNodePropMultiList SpecialFlags { get { return ( _CswNbtNode.Properties[PropertyName.SpecialFlags] ); } }
        public CswNbtNodePropMultiList HazardCategories { get { return ( _CswNbtNode.Properties[PropertyName.HazardCategories] ); } }
        public CswNbtNodePropChildContents Jurisdiction { get { return ( _CswNbtNode.Properties[PropertyName.Jurisdiction] ); } }

        #endregion Object class specific properties
    }//CswNbtObjClassChemical

}//namespace ChemSW.Nbt.ObjClasses