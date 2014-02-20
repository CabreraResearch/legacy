using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ChemCatCentral;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassChemical: CswNbtPropertySetMaterial
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

        #endregion Base

        #region Enums

        public new sealed class PropertyName: CswNbtPropertySetMaterial.PropertyName
        {
            public const string PhysicalState = "Physical State";
            public const string SpecificGravity = "Specific Gravity";
            public const string StorageCompatibility = "Storage Compatibility";
            public const string ExpirationInterval = "Expiration Interval";
            public const string CasNo = "CAS No";
            //public const string RegulatoryLists = "Regulatory Lists";
            public const string RegulatoryListsGrid = "Regulatory Lists";
            public const string UNCode = "UN Code";
            public const string IsTierII = "Is Tier II";
            public const string ViewSDS = "View SDS";
            public const string AssignedSDS = "Assigned SDS";
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
            public const string SuppressedRegulatoryLists = "Suppressed Regulatory Lists";
            public const string LQNo = "LQNo";
            public const string DOTCode = "DOT Code";
            public const string HazardInfo = "Hazard Info";
            public const string CompressedGas = "Compressed Gas";
            public const string SMILES = "SMILES";
            public const string DisposalInstructions = "Disposal Instructions";
            public const string OpenExpireInterval = "Open Expire Interval";
            public const string EINECS = "EINECS";
            public const string SubclassName = "Subclass Name";
            public const string Pictograms = "Pictograms";
            public const string LabelCodes = "Label Codes";
            public const string LabelCodesGrid = "Labels Codes Grid";
            public const string AddLabelCodes = "Add Label Codes";
            public const string LinkChemWatch = "Link ChemWatch";
            public const string ProductDescription = "Product Description";
            public const string LegacyMaterialId = "Legacy Material Id";
            public const string OpenExpirationInterval = "Open Expiration Interval";
        }

        #endregion Enums

        #region Inherited Events

        public override void beforePropertySetWriteNode()
        {
            ViewSDS.State = PropertyName.ViewSDS;
            ViewSDS.MenuOptions = PropertyName.ViewSDS + ",View All";

            //if( CasNo.getAnySubFieldModified() && _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.RegulatoryLists ) && false == IsCopy )
            //{
            //    CswCommaDelimitedString ParentMaterials = new CswCommaDelimitedString();
            //    getParentMaterials( ref ParentMaterials );
            //    if( ParentMaterials.Count > 0 ) //this material is used by others as a component...we have no idea how deep the rabbit hole is...make a batch op to update 
            //    {
            //        ParentMaterials.Add( NodeId.ToString() ); //we need to update this material too, so add it to the list
            //        CswNbtBatchOpUpdateRegulatoryListsForMaterials BatchOp = new CswNbtBatchOpUpdateRegulatoryListsForMaterials( _CswNbtResources );
            //        BatchOp.makeBatchOp( ParentMaterials );
            //    }
            //    else //this material isn't used as a component anywhere, so just update it by its self
            //    {
            //        _updateRegulatoryLists();
            //    }
            //}
        }

        public override void beforePropertySetDeleteNode()
        {
            _CswNbtResources.StructureSearchManager.DeleteFingerprintRecord( this.NodeId.PrimaryKey );
        }

        public override void afterPropertySetPopulateProps()
        {
            LabelCodes.InitOptions = _initDsdPhraseOptions;
            LabelCodes.SetOnPropChange( OnLabelCodesChange );
            AddLabelCodes.SetOnPropChange( OnAddLabelCodesPropChange );

            CswNbtView DsdView = setupDsdPhraseView();
            if( null != DsdView )
            {
                DsdView.SaveToCache( false, true );
            }

            ViewSDS.SetOnBeforeRender( delegate( CswNbtNodeProp prop )
                {
                    if( false == CswNbtObjClassSDSDocument.materialHasActiveSDS( _CswNbtResources, NodeId ) )
                    {
                        ViewSDS.setHidden( true, false );
                    }
                } );

            if( IsConstituent.Checked == CswEnumTristate.True )
            {
                AssignedSDS.setHidden( true, false );
            }
            Jurisdiction.SetSelected = delegate()
            {
                CswPrimaryKey SelectedNodeId = null;
                CswNbtObjClassUser User = _CswNbtResources.Nodes[_CswNbtResources.CurrentNbtUser.UserId];
                if( null != User && null != User.JurisdictionId )
                {
                    CswNbtMetaDataObjectClass GHSOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );
                    CswNbtMetaDataObjectClassProp JurisdictionOCP = GHSOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.Jurisdiction );
                    CswNbtMetaDataObjectClassProp MaterialOCP = GHSOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.Material );
                    CswNbtView JurisdictionsView = new CswNbtView( _CswNbtResources );
                    CswNbtViewRelationship RootVR = JurisdictionsView.AddViewRelationship( GHSOC, false );
                    JurisdictionsView.AddViewPropertyAndFilter( RootVR, JurisdictionOCP, CswEnumNbtFilterConjunction.And, User.JurisdictionId.PrimaryKey.ToString(), CswEnumNbtSubFieldName.NodeID, FilterMode : CswEnumNbtFilterMode.Equals );
                    JurisdictionsView.AddViewPropertyAndFilter( RootVR, MaterialOCP, CswEnumNbtFilterConjunction.And, NodeId.PrimaryKey.ToString(), CswEnumNbtSubFieldName.NodeID, FilterMode : CswEnumNbtFilterMode.Equals );
                    ICswNbtTree JurisdictionsTree = _CswNbtResources.Trees.getTreeFromView( JurisdictionsView, false, false, false );
                    if( JurisdictionsTree.getChildNodeCount() > 0 )
                    {
                        JurisdictionsTree.goToNthChild( 0 );
                        SelectedNodeId = JurisdictionsTree.getNodeIdForCurrentPosition();
                    }
                }
                return SelectedNodeId;
            };
            //PhysicalState.SetOnPropChange( _onPhysicalStatePropChange );
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
                    case PropertyName.LinkChemWatch:
                        if( _CswNbtResources.Permit.can( CswEnumNbtActionName.ChemWatch ) )
                        {
                            HasPermission = true;
                            ButtonData.Data["state"] = new JObject();
                            ButtonData.Data["state"]["materialId"] = NodeId.ToString();
                            ButtonData.Action = CswEnumNbtButtonAction.chemwatch;
                        }
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
            bool canAddSDS = false;
            bool addSDSPermission = false;
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.SDS ) )
            {
                CswNbtMetaDataObjectClass SDSDocOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SDSDocumentClass );
                CswNbtMetaDataNodeType SDSNodeType = SDSDocOC.FirstNodeType;
                canAddSDS = null != SDSNodeType;
                if( canAddSDS )
                {
                    ButtonData.Data["state"]["sdsDocTypeId"] = SDSNodeType.NodeTypeId;
                    CswNbtMetaDataNodeTypeProp AssignedSDSProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypeId, "Assigned SDS" );
                    CswNbtMetaDataNodeTypeProp RevisionDateProp = _CswNbtResources.MetaData.getNodeTypeProp( SDSNodeType.NodeTypeId, "Revision Date" );
                    if( null != AssignedSDSProp )
                    {
                        CswNbtView AssignedSDSView = _CswNbtResources.ViewSelect.restoreView( AssignedSDSProp.ViewId );
                        AssignedSDSView = AssignedSDSView.PrepGridView( NodeId );
                        ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( AssignedSDSView, false, false, false );
                        if( Tree.getChildNodeCount() > 0 )
                        {
                            Tree.goToNthChild( 0 );
                            JArray SDSDocs = new JArray();
                            for( Int32 i = 0; i < Tree.getChildNodeCount(); i++ )
                            {
                                Tree.goToNthChild( i );
                                JObject Doc = new JObject();

                                CswNbtObjClassSDSDocument SDSDoc = Tree.getNodeForCurrentPosition();
                                if( null != RevisionDateProp )
                                {
                                    DateTime RevisionDate =
                                        SDSDoc.Node.Properties[RevisionDateProp].AsDateTime.DateTimeValue;
                                    Doc["revisiondate"] = RevisionDate == DateTime.MinValue
                                                              ? ""
                                                              : RevisionDate.ToShortDateString();
                                }
                                else
                                {
                                    Doc["revisiondate"] = "";
                                }
                                if(
                                    SDSDoc.FileType.Value.Equals(
                                        CswNbtPropertySetDocument.CswEnumDocumentFileTypes.File ) )
                                {
                                    Doc["displaytext"] = SDSDoc.File.FileName;
                                    Doc["linktext"] = SDSDoc.File.Href;
                                }
                                else
                                {
                                    Doc["displaytext"] = String.IsNullOrEmpty( SDSDoc.Link.Text )
                                                             ? SDSDoc.Link.GetFullURL()
                                                             : SDSDoc.Link.Text;
                                    Doc["linktext"] = SDSDoc.Link.GetFullURL();
                                }
                                SDSDocs.Add( Doc );
                                Tree.goToParentNode();
                            }
                            ButtonData.Data["state"]["sdsDocs"] = SDSDocs;
                        }//if( Tree.getChildNodeCount() > 0 )
                    }

                    // Does the current User have permission to create SDS Documents?
                    if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, SDSNodeType ) )
                    {
                        addSDSPermission = true;
                    }
                }

            }//if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.SDS ) )

            ButtonData.Data["state"]["canAddSDS"] = canAddSDS;
            ButtonData.Data["state"]["addSDSPermission"] = addSDSPermission;
        }

        public override void onPropertySetAddDefaultViewFilters( CswNbtViewRelationship ParentRelationship ) { }

        /// <summary>
        /// This method is called when the UpdtPropVals Schedule Rule is run.
        /// </summary>
        public override void onUpdatePropertyValue()
        {
            RefreshRegulatoryListMembers();
            // We only want to trigger the syncing of FireDb/PCID
            // if we are running the ExtChemDataSyncRule.
            if( Node.PendingUpdate )
            {
                syncFireDbData();
                syncPCIDData();
            }
        }

        #endregion Inherited Events

        #region Custom Logic

        /// <summary>
        /// Calculates the expiration date from today based on the Material's Expiration Interval
        /// </summary>
        public override DateTime getDefaultExpirationDate( DateTime InitialDate )
        {
            DateTime DefaultExpDate = DateTime.MinValue;

            //No point trying to get default if both values are invalid
            if( CswTools.IsPrimaryKey( ExpirationInterval.UnitId ) && ExpirationInterval.Quantity > 0 )
            {
                DefaultExpDate = InitialDate == DateTime.MinValue ? DateTime.Now : InitialDate;
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
                Value : NodeId.PrimaryKey.ToString(),
                FilterMode : CswEnumNbtFilterMode.Equals,
                SubFieldName : CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID );
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
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.SDS ) )
            {
                if( ButtonData.SelectedText.Equals( PropertyName.ViewSDS ) )
                {
                    String url = CswNbtObjClassSDSDocument.getAssignedSDSDocumentUrl( _CswNbtResources, NodeId );
                    if( false == String.IsNullOrEmpty( url ) )
                    {
                        ButtonData.Data["url"] = url;
                        ButtonData.Action = CswEnumNbtButtonAction.popup;
                    }
                    else
                    {
                        ButtonData.Message = "There are no active SDS assigned to this " + NodeType.NodeTypeName;
                        ButtonData.Action = CswEnumNbtButtonAction.nothing;
                    }
                }
                else
                {
                    CswNbtView assignedSDSDocsView = CswNbtObjClassSDSDocument.getAssignedSDSDocumentsView( _CswNbtResources, NodeId );
                    if( null != assignedSDSDocsView )
                    {
                        ButtonData.Data["viewid"] = assignedSDSDocsView.ViewId.ToString();
                        ButtonData.Data["title"] = assignedSDSDocsView.ViewName;
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

        /// <summary>
        /// Syncs this Material's Hazard Classes with data stored in ChemCatCentral.
        /// </summary>
        public void syncFireDbData()
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.FireDbSync ) )
            {
                CswC3SearchParams CswC3SearchParams = new CswC3SearchParams();
                CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3SearchParams );
                SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client( CswEnumErrorType.Warning, "Unable to sync data. Please contact your administrator. " );
                if( null != C3SearchClient )
                {
                    // Set FireDb specific properties
                    CswC3SearchParams.Purpose = "FireDb";
                    CswC3SearchParams.SyncType = "CasNo";
                    CswC3SearchParams.SyncKey = this.CasNo.Text;

                    CswRetObjSearchResults SearchResults = C3SearchClient.getExtChemData( CswC3SearchParams );
                    if( null != SearchResults.ExtChemDataResults )
                    {
                        if( SearchResults.ExtChemDataResults.Length > 0 )
                        {
                            ChemCatCentral.CswC3ExtChemData C3ExtChemData = SearchResults.ExtChemDataResults[0];

                            #region Hazard Classes

                            foreach( CswC3ExtChemData.FireDB.UfcHazardClass UfcHazardClass in C3ExtChemData.ExtensionData1.FireDbData.UfcHazardClasses )
                            {
                                if( false == HazardClasses.CheckValue( UfcHazardClass.HazardClass ) )
                                {
                                    HazardClasses.AddValue( UfcHazardClass.HazardClass );
                                }
                            }

                            #endregion

                            #region Material Type

                            if( string.IsNullOrEmpty( this.MaterialType.Value ) )
                            {
                                if( false == string.IsNullOrEmpty( C3ExtChemData.ExtensionData1.FireDbData.MaterialType ) )
                                {
                                    this.MaterialType.Value = C3ExtChemData.ExtensionData1.FireDbData.MaterialType;
                                }
                            }

                            #endregion

                            #region Hazard Categories

                            foreach( CswC3ExtChemData.FireDB.HazardCategoryClass HazardCategoryClass in C3ExtChemData.ExtensionData1.FireDbData.HazardCategories )
                            {
                                // First convert c3 hazard category to nbt equivalent
                                string ConvertHazardCategory = _convertHazardCategory( HazardCategoryClass.HazardCategory );
                                if( false == HazardCategories.CheckValue( ConvertHazardCategory ) )
                                {
                                    HazardCategories.AddValue( ConvertHazardCategory );
                                }
                            }

                            #endregion

                            #region EHS List

                            if( false == string.IsNullOrEmpty( C3ExtChemData.ExtensionData1.FireDbData.EhsList ) )
                            {
                                bool EHSList = CswConvert.ToBoolean( C3ExtChemData.ExtensionData1.FireDbData.EhsList );
                                if( EHSList )
                                {
                                    this.SpecialFlags.AddValue( "EHS" );
                                }
                            }

                            #endregion
                        }
                    }
                }
            }//if (_CswNbtResources.Modules.IsModuleEnabled(CswEnumNbtModuleName.FireDbSync))
        }//syncFireDbData()

        private string _convertHazardCategory( string C3HazardCategory )
        {
            string NbtHazardCategory = string.Empty;

            switch( C3HazardCategory )
            {
                case "F = Fire":
                    NbtHazardCategory = C3HazardCategory;
                    break;
                case "R = Reactive":
                    NbtHazardCategory = C3HazardCategory;
                    break;
                case "P = Pressure Release":
                    NbtHazardCategory = "P = Pressure";
                    break;
                case "C = Chronic":
                    NbtHazardCategory = "C = Chronic (delayed)";
                    break;
                case "A = Acute":
                    NbtHazardCategory = "I = Immediate (acute)";
                    break;
            }

            return NbtHazardCategory;
        }

        /// <summary>
        /// Syncs various properties of this Material including:
        /// NFPA, PPE, Storage Compatibility, Structure, Formula, Density, MP, BP, Physical Description, TierII and Regulatory Lists
        /// with data in ChemCatCentral.
        /// </summary>
        public void syncPCIDData()
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.PCIDSync ) )
            {
                CswC3SearchParams CswC3SearchParams = new CswC3SearchParams();
                CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3SearchParams );
                SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client( CswEnumErrorType.Warning, "Unable to sync data. Please contact your administrator. " );
                if( null != C3SearchClient )
                {

                    // Set PCID specific properties
                    CswC3SearchParams.Purpose = "PCID";
                    CswC3SearchParams.SyncType = "CasNo";
                    CswC3SearchParams.SyncKey = this.CasNo.Text;

                    CswRetObjSearchResults SearchResults = C3SearchClient.getExtChemData( CswC3SearchParams );
                    if( null != SearchResults.ExtChemDataResults )
                    {
                        if( SearchResults.ExtChemDataResults.Length > 0 )
                        {
                            CswC3ExtChemData C3ExtChemData = SearchResults.ExtChemDataResults[0];

                            #region NFPA

                            if( string.IsNullOrEmpty( NFPA.Red ) )
                            {
                                NFPA.Red = CswConvert.ToString( C3ExtChemData.ExtensionData1.PcidData.NFPA.NfpaFire );
                            }
                            if( string.IsNullOrEmpty( NFPA.Yellow ) )
                            {
                                NFPA.Yellow = CswConvert.ToString( C3ExtChemData.ExtensionData1.PcidData.NFPA.NfpaReact );
                            }
                            if( string.IsNullOrEmpty( NFPA.Blue ) )
                            {
                                NFPA.Blue = CswConvert.ToString( C3ExtChemData.ExtensionData1.PcidData.NFPA.NfpaHealth );
                            }
                            if( string.IsNullOrEmpty( NFPA.White ) )
                            {
                                string NFPASpecificRating = _NFPASpecificRatingNumberToText( C3ExtChemData.ExtensionData1.PcidData.NFPA.NfpaSpecific );
                                NFPA.White = NFPASpecificRating;
                            }

                            #endregion

                            #region PPE

                            CswCommaDelimitedString CurrentPPEOptions = new CswCommaDelimitedString();
                            CurrentPPEOptions = this.PPE.Value;

                            CswCommaDelimitedString UpdatedPPEOptions = new CswCommaDelimitedString();

                            foreach( CswC3ExtChemData.PCID.PPEClass PPEClass in C3ExtChemData.ExtensionData1.PcidData.PPEOptions )
                            {
                                if( false == CurrentPPEOptions.Contains( PPEClass.PPE ) )
                                {
                                    UpdatedPPEOptions.Add( PPEClass.PPE );
                                }
                            }

                            // Add the original PPE options to the new list
                            foreach( string PPE in CurrentPPEOptions )
                            {
                                UpdatedPPEOptions.Add( PPE );
                            }

                            // Set the value of the property to the new list
                            this.PPE.Value = UpdatedPPEOptions;

                            #endregion

                            // Storage Compatibility
                            if( StorageCompatibility.Value.IsEmpty )
                            {
                                string StorageCompatImagePath = _getStorageCompatImagePath( C3ExtChemData.ExtensionData1.PcidData.StorageCompatibility );
                                CswDelimitedString StorageCompatValue = new CswDelimitedString( '\n' );
                                StorageCompatValue.Add( StorageCompatImagePath );
                                StorageCompatibility.Value = StorageCompatValue;
                            }

                            #region Additional Properties

                            // Additional properties ONLY IF they have an empty value. For now the following- structure, formula, density, mp, bp, physical description, tier II
                            foreach( CswC3ExtChemData.PCID.AdditionalProperty Property in C3ExtChemData.ExtensionData1.PcidData.AdditionalProperties )
                            {
                                // Structure
                                if( Property.Name.Equals( "STRUCT" ) )
                                {
                                    if( false == string.IsNullOrEmpty( Property.Value ) )
                                    {
                                        if( this.Structure.Empty )
                                        {
                                            string propAttr =
                                                new CswPropIdAttr( Node,
                                                                  NodeType.getNodeTypePropByObjectClassProp(
                                                                      PropertyName.Structure ) ).ToString();
                                            string molData = Property.Value;

                                            string Href;
                                            string FormattedMolString;
                                            string errorMsg;
                                            CswNbtSdBlobData SdBlobData = new CswNbtSdBlobData( _CswNbtResources );
                                            SdBlobData.saveMol( molData, propAttr, out Href, out FormattedMolString, out errorMsg, false, this.Node );
                                        }
                                    }
                                }

                                // Formula
                                if( Property.Name.Equals( "MFORMULA" ) )
                                {
                                    if( false == string.IsNullOrEmpty( Property.Value ) )
                                    {
                                        if( this.Formula.Empty )
                                        {
                                            this.Formula.Text = Property.Value;
                                        }
                                    }
                                }

                                // Density
                                if( Property.Name.Equals( "DENSITY_GMML" ) )
                                {
                                    if( false == string.IsNullOrEmpty( Property.Value ) )
                                    {
                                        if( this.VaporDensity.Empty )
                                        {
                                            this.VaporDensity.Text = Property.Value;
                                        }
                                    }
                                }

                                // MP
                                if( Property.Name.Equals( "MP_C" ) )
                                {
                                    if( false == string.IsNullOrEmpty( Property.Value ) )
                                    {
                                        if( this.MeltingPoint.Empty )
                                        {
                                            this.MeltingPoint.Text = Property.Value;
                                        }
                                    }
                                }

                                // BP
                                if( Property.Name.Equals( "BOILING_POINT_C" ) )
                                {
                                    if( false == string.IsNullOrEmpty( Property.Value ) )
                                    {
                                        if( this.BoilingPoint.Empty )
                                        {
                                            this.BoilingPoint.Text = Property.Value;
                                        }
                                    }
                                }

                                // Physical Description
                                if( Property.Name.Equals( "PHYSICAL_APPEARANCE" ) )
                                {
                                    if( false == string.IsNullOrEmpty( Property.Value ) )
                                    {
                                        if( this.PhysicalDescription.Empty )
                                        {
                                            this.PhysicalDescription.Text = Property.Value;
                                        }
                                    }
                                }

                                // Tier II
                                if( Property.Name.Equals( "TIER_II" ) )
                                {
                                    if( false == string.IsNullOrEmpty( Property.Value ) )
                                    {
                                        if( this.IsTierII.Empty )
                                        {
                                            this.IsTierII.Checked = CswConvert.ToTristate( Property.Value );
                                        }
                                    }
                                }

                            } //foreach( CswC3ExtChemData.PCID.AdditionalProperty Property in C3ExtChemData.ExtensionData1.PcidData.AdditionalProperties )

                            #endregion

                            #region GHS

                            CswNbtMetaDataObjectClass GHSCOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );

                            // Classifications
                            CswCommaDelimitedString GHSClassifications = _mapC3GHSClassificationsToNBT( C3ExtChemData.ExtensionData1.PcidData.GHS );
                            CswCommaDelimitedString GHSClassNodeIds = new CswCommaDelimitedString();

                            CswNbtMetaDataObjectClass GHSClassOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClassificationClass );
                            Dictionary<CswPrimaryKey, string> NamesAndIds = GHSClassOC.getNodeIdAndNames( false, false );
                            foreach( KeyValuePair<CswPrimaryKey, string> KeyValuePair in NamesAndIds )
                            {
                                foreach( string Name in GHSClassifications )
                                {
                                    if( KeyValuePair.Value.Equals( Name ) )
                                    {
                                        GHSClassNodeIds.Add( KeyValuePair.Key.ToString() );
                                    }
                                }
                            }

                            // Label Codes
                            List<string> GHSPhrases = C3ExtChemData.ExtensionData1.PcidData.GHS.CODES.ToList();
                            CswCommaDelimitedString GHSPhraseNodeIds = new CswCommaDelimitedString();

                            CswNbtMetaDataObjectClass GHSPhraseOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSPhraseClass );
                            Dictionary<CswPrimaryKey, string> GHSPhraseNodes = GHSPhraseOC.getNodeIdAndNames( false, false );
                            foreach( KeyValuePair<CswPrimaryKey, string> KeyValuePair in GHSPhraseNodes )
                            {
                                foreach( string GHSPhrase in GHSPhrases )
                                {
                                    if( KeyValuePair.Value.Equals( GHSPhrase ) )
                                    {
                                        GHSPhraseNodeIds.Add( KeyValuePair.Key.ToString() );
                                    }
                                }
                            }

                            CswNbtObjClassGHS GHSNode = _getDefaultJurisdictionGHSNode();
                            if( null != GHSNode )
                            {
                                CswCommaDelimitedString UpdatedClassifications = new CswCommaDelimitedString();
                                CswCommaDelimitedString CurrentClassifications = GHSNode.Classifications.Value;
                                UpdatedClassifications = CurrentClassifications;
                                foreach( string NodeId in GHSClassNodeIds )
                                {
                                    if( false == UpdatedClassifications.Contains( NodeId ) )
                                    {
                                        UpdatedClassifications.Add( NodeId );
                                    }
                                }
                                GHSNode.Classifications.Value = UpdatedClassifications;

                                foreach( string GHSPhraseNodeId in GHSPhraseNodeIds )
                                {
                                    if( false == GHSNode.LabelCodes.Value.Contains( GHSPhraseNodeId ) )
                                    {
                                        GHSNode.LabelCodes.AddValue( GHSPhraseNodeId );
                                    }
                                }

                                GHSNode.postChanges( true );
                            }
                            else
                            {
                                // Get nodeid for Default Jurisdiction node
                                CswNbtMetaDataObjectClass JurisdictionOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.JurisdictionClass );
                                Dictionary<CswPrimaryKey, string> JurisdictionNodes = JurisdictionOC.getNodeIdAndNames( false, false );
                                CswPrimaryKey DefaultJurisdictionNodeId = null;
                                foreach( KeyValuePair<CswPrimaryKey, string> JurisdictionNode in JurisdictionNodes.Where( JurisdictionNode => JurisdictionNode.Value == "Default Jurisdiction" ) )
                                {
                                    DefaultJurisdictionNodeId = JurisdictionNode.Key;
                                }

                                // Get nodeid for Warning node
                                CswNbtMetaDataObjectClass SignalWordOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSSignalWordClass );
                                Dictionary<CswPrimaryKey, string> SignalWordNodes = SignalWordOC.getNodeIdAndNames( false, false );
                                CswPrimaryKey WarningWordNodeId = null;
                                foreach( KeyValuePair<CswPrimaryKey, string> SignalWordNode in SignalWordNodes.Where( SignalWordNode => SignalWordNode.Value == "Warning" ) )
                                {
                                    WarningWordNodeId = SignalWordNode.Key;
                                }

                                // Create a new GHS node with Default Jurisidiction
                                CswNbtMetaDataNodeType GHSNT = GHSCOC.FirstNodeType;
                                _CswNbtResources.Nodes.makeNodeFromNodeTypeId( GHSNT.NodeTypeId, delegate( CswNbtNode NewNode )
                                    {
                                        CswNbtObjClassGHS NewGHSNode = NewNode;
                                        NewGHSNode.Material.RelatedNodeId = this.NodeId;
                                        NewGHSNode.Jurisdiction.RelatedNodeId = DefaultJurisdictionNodeId;
                                        NewGHSNode.Classifications.Value = GHSClassNodeIds;
                                        NewGHSNode.SignalWord.RelatedNodeId = WarningWordNodeId;
                                        NewGHSNode.LabelCodes.Value = GHSPhraseNodeIds;
                                    } );

                            }

                            #endregion GHS

                        } //if( SearchResults.ExtChemDataResults.Length > 0 )

                    } //if( null != SearchResults.ExtChemDataResults )

                }//if( null != C3SearchClient )

            }//if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.PCIDSync ) )
        }

        private CswCommaDelimitedString _mapC3GHSClassificationsToNBT( CswC3ExtChemData.PCID.GHSClass GHSData )
        {
            CswCommaDelimitedString Ret = new CswCommaDelimitedString();

            if( false == string.IsNullOrEmpty( GHSData.EXPLOSIVES ) )
            {
                Ret.Add( "Explosives (Division " + GHSData.EXPLOSIVES + ")" );
            }
            if( false == string.IsNullOrEmpty( GHSData.FLAMMABLEGASES ) )
            {
                Ret.Add( "Flammable Gas (Category " + GHSData.FLAMMABLEGASES + ")" );
            }
            if( false == string.IsNullOrEmpty( GHSData.FLAMMABLEAEROSOLS ) )
            {
                Ret.Add( "Flammable Aerosols (Category " + GHSData.FLAMMABLEAEROSOLS + ")" );
            }

            if( GHSData.OXIDIZINGGASES == 1 )
            {
                Ret.Add( "Oxidizing Gases" );
            }

            if( GHSData.PRESSUREGASESCOMPRESSEDGASES == 1 )
            {
                Ret.Add( "Gases Under Pressure (Compressed gas)" );
            }
            if( GHSData.PRESSUREGASESLIQUEFIEDGASES == 1 )
            {
                Ret.Add( "Gases Under Pressure (Liquefied gas)" );
            }
            if( GHSData.PRESSUREGASESREFRIGERATEDLIQUE == 1 )
            {
                Ret.Add( "Gases Under Pressure (Refrigerated liquefied gas)" );
            }
            if( GHSData.PRESSUREGASESDISSOLVEDGASES == 1 )
            {
                Ret.Add( "Gases Under Pressure (Dissolved gas)" );
            }

            if( false == string.IsNullOrEmpty( GHSData.FLAMMABLELIQUIDS ) )
            {
                Ret.Add( "Flammable Liquids (Category " + GHSData.FLAMMABLELIQUIDS + ")" );
            }
            if( false == string.IsNullOrEmpty( GHSData.FLAMMABLESOLIDS ) )
            {
                Ret.Add( "Flammable Solids (Category " + GHSData.FLAMMABLESOLIDS + ")" );
            }
            if( false == string.IsNullOrEmpty( GHSData.SELFREACTIVESUBSTANCES ) && false == GHSData.SELFHEATINGSUBSTANCES.Equals( "1" ) )
            {
                Ret.Add( "Self-Reactive Substances and Mixtures (" + GHSData.SELFREACTIVESUBSTANCES + ")" );
            }

            if( GHSData.PYROPHORICLIQUIDS == 1 )
            {
                Ret.Add( "Pyrophoric Liquids" );
            }
            if( GHSData.PYROPHORICSOLIDS == 1 )
            {
                Ret.Add( "Pyrophoric Solids" );
            }

            if( false == string.IsNullOrEmpty( GHSData.SELFHEATINGSUBSTANCES ) )
            {
                Ret.Add( "Self-Heating Substances and Mixtures (Category " + GHSData.SELFHEATINGSUBSTANCES + ")" );
            }
            if( false == string.IsNullOrEmpty( GHSData.WATERREACTIVEFLAMMABLEGAS ) )
            {
                Ret.Add( "Substances and Mixtures, which on Contact with Water, Emit Flammable Gases (Category " + GHSData.WATERREACTIVEFLAMMABLEGAS + ")" );
            }

            if( GHSData.OXIDIZINGLIQUIDS != 0 )
            {
                Ret.Add( "Oxidizing Liquids (Category " + GHSData.OXIDIZINGLIQUIDS + ")" );
            }
            if( GHSData.OXIDIZINGSOLIDS != 0 )
            {
                Ret.Add( "Oxidizing Solids (Category " + GHSData.OXIDIZINGLIQUIDS + ")" );
            }

            if( false == string.IsNullOrEmpty( GHSData.ORGANICPEROXIDES ) )
            {
                Ret.Add( "Organic Peroxides (" + GHSData.ORGANICPEROXIDES + ")" );
            }

            if( GHSData.CORROSIVETOMETALS == 1 )
            {
                Ret.Add( "Corrosive to Metal" );
            }

            if( false == string.IsNullOrEmpty( GHSData.ACUTETOXICITY ) )
            {
                Ret.Add( "Acute Toxicity (Category " + GHSData.ACUTETOXICITY + ")" );
            }

            if( GHSData.SKINCORROSION == 1 )
            {
                Ret.Add( "Skin Corrosion/Irritation (Category 1A)" );
            }
            if( GHSData.SKINIRRITATION == 1 )
            {
                Ret.Add( "Skin Corrosion/Irritation (Category 2)" );
            }
            if( GHSData.SERIOUSEYEDAMAGE == 1 )
            {
                Ret.Add( "Serious Eye Damage/Eye Irritation (Category 1)" );
            }
            if( GHSData.EYEIRRITATION == 1 )
            {
                Ret.Add( "Serious Eye Damage/Eye Irritation (Category 2A)" );
            }
            if( GHSData.RESPIRATORYSENSITIZER == 1 )
            {
                Ret.Add( "Respiratory Sensitization" );
            }
            if( GHSData.SKINSENSITIZER == 1 )
            {
                Ret.Add( "Skin Sensitization" );
            }

            if( false == string.IsNullOrEmpty( GHSData.GERMCELLMUTAGENICITY ) )
            {
                if( GHSData.GERMCELLMUTAGENICITY.Equals( "1" ) )
                {
                    Ret.Add( "Germ Cell Mutagenicity (Category 1A)" );
                }
                if( GHSData.GERMCELLMUTAGENICITY.Equals( "2" ) )
                {
                    Ret.Add( "Germ Cell Mutagenicity (Category 1B)" );
                }
            }

            if( false == string.IsNullOrEmpty( GHSData.CARCINOGENICITY ) )
            {
                if( GHSData.CARCINOGENICITY.Equals( "1" ) )
                {
                    Ret.Add( "Carcinogenicity (Category 1A)" );
                }
                if( GHSData.CARCINOGENICITY.Equals( "2" ) )
                {
                    Ret.Add( "Carcinogenicity (Category 1B)" );
                }
            }

            if( false == string.IsNullOrEmpty( GHSData.REPRODUCTIVETOXICITY ) )
            {
                if( GHSData.REPRODUCTIVETOXICITY.Equals( "1" ) )
                {
                    Ret.Add( "Reproductive Toxicity (Category 1A)" );
                }
                if( GHSData.REPRODUCTIVETOXICITY.Equals( "2" ) )
                {
                    Ret.Add( "Reproductive Toxicity (Category 1B)" );
                }
                if( GHSData.REPRODUCTIVETOXICITY.Equals( "3" ) )
                {
                    Ret.Add( "Reproductive Toxicity (Category 2)" );
                }
            }

            if( false == string.IsNullOrEmpty( GHSData.TOSTSINGLEEXPOSURE ) )
            {
                Ret.Add( "Specific Target Organ Toxicity (Single Exposure) (Category " + GHSData.TOSTSINGLEEXPOSURE + ")" );
            }

            if( false == string.IsNullOrEmpty( GHSData.TOSTREPEATEDEXPOSURE ) )
            {
                Ret.Add( "Specific Target Organ Toxicity (Repeated Exposure) (Category " + GHSData.TOSTREPEATEDEXPOSURE + ")" );
            }

            if( false == string.IsNullOrEmpty( GHSData.ASPIRATIONHAZARD ) )
            {
                Ret.Add( "Aspiration Toxicity (Category " + GHSData.ASPIRATIONHAZARD + ")" );
            }

            if( false == string.IsNullOrEmpty( GHSData.ACUTEAQUATICTOXICITY ) )
            {
                if( GHSData.ACUTEAQUATICTOXICITY.Equals( "1" ) || GHSData.ACUTEAQUATICTOXICITY.Equals( "I" ) )
                {
                    Ret.Add( "Aquatic Toxicity (Acute) (Category I)" );
                }
                else if( GHSData.ACUTEAQUATICTOXICITY.Equals( "2" ) || GHSData.ACUTEAQUATICTOXICITY.Equals( "II" ) )
                {
                    Ret.Add( "Aquatic Toxicity (Acute) (Category II)" );
                }
                else if( GHSData.ACUTEAQUATICTOXICITY.Equals( "3" ) || GHSData.ACUTEAQUATICTOXICITY.Equals( "III" ) )
                {
                    Ret.Add( "Aquatic Toxicity (Acute) (Category III)" );
                }
            }

            if( false == string.IsNullOrEmpty( GHSData.CHRONICAQUATICTOXICITY ) )
            {
                if( GHSData.CHRONICAQUATICTOXICITY.Equals( "1" ) || GHSData.CHRONICAQUATICTOXICITY.Equals( "I" ) )
                {
                    Ret.Add( "Aquatic Toxicity (Chronic) (Category I)" );
                }
                else if( GHSData.CHRONICAQUATICTOXICITY.Equals( "2" ) || GHSData.CHRONICAQUATICTOXICITY.Equals( "II" ) )
                {
                    Ret.Add( "Aquatic Toxicity (Chronic) (Category II)" );
                }
                else if( GHSData.CHRONICAQUATICTOXICITY.Equals( "3" ) || GHSData.CHRONICAQUATICTOXICITY.Equals( "III" ) )
                {
                    Ret.Add( "Aquatic Toxicity (Chronic) (Category III)" );
                }
                else if( GHSData.CHRONICAQUATICTOXICITY.Equals( "4" ) || GHSData.CHRONICAQUATICTOXICITY.Equals( "IV" ) )
                {
                    Ret.Add( "Aquatic Toxicity (Chronic) (Category IV)" );
                }
            }

            return Ret;
        }

        private CswNbtObjClassGHS _getDefaultJurisdictionGHSNode()
        {
            CswNbtObjClassGHS Ret = null;

            CswNbtMetaDataObjectClass JurisdictionOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.JurisdictionClass );
            CswNbtMetaDataObjectClassProp NameOCP = JurisdictionOC.getObjectClassProp( CswNbtObjClassJurisdiction.PropertyName.Name );
            CswNbtView View1 = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship View1Parent = View1.AddViewRelationship( JurisdictionOC, true );
            View1.AddViewPropertyAndFilter( View1Parent, NameOCP, "Default Jurisdiction", CswEnumNbtSubFieldName.Text );
            ICswNbtTree View1Tree = _CswNbtResources.Trees.getTreeFromView( View1, false, false, false );
            if( View1Tree.getChildNodeCount() > 0 )
            {
                View1Tree.goToNthChild( 0 );

                CswNbtMetaDataObjectClass GHSOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );
                CswNbtMetaDataObjectClassProp MaterialOCP = GHSOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.Material );
                CswNbtMetaDataObjectClassProp JurisdictionOCP = GHSOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.Jurisdiction );
                CswNbtView View2 = new CswNbtView( _CswNbtResources );
                CswNbtViewRelationship Parent = View2.AddViewRelationship( GHSOC, true );
                View2.AddViewPropertyAndFilter( Parent, MaterialOCP, NodeId.PrimaryKey.ToString(), CswEnumNbtSubFieldName.NodeID );
                View2.AddViewPropertyAndFilter( Parent, JurisdictionOCP, View1Tree.getNodeForCurrentPosition().NodeId.PrimaryKey.ToString(), CswEnumNbtSubFieldName.NodeID );
                ICswNbtTree View2Tree = _CswNbtResources.Trees.getTreeFromView( View2, false, false, false );
                if( View2Tree.getChildNodeCount() > 0 )
                {
                    View2Tree.goToNthChild( 0 );
                    Ret = View2Tree.getNodeForCurrentPosition();
                }
            }

            return Ret;
        }

        private string _getStorageCompatImagePath( Int32 StorageCompatibility )
        {
            string StorageCompatImagePath = string.Empty;

            // If Int32.MinValue, then there was no compatbility set
            if( StorageCompatibility == Int32.MinValue )
            {
                StorageCompatibility = 0;
            }

            switch( StorageCompatibility )
            {
                case 0:
                    StorageCompatImagePath = "Images/cispro/0w.gif";
                    break;
                case 1:
                    StorageCompatImagePath = "Images/cispro/1o.gif";
                    break;
                case 2:
                    StorageCompatImagePath = "Images/cispro/2y.gif";
                    break;
                case 3:
                    StorageCompatImagePath = "Images/cispro/3g.gif";
                    break;
                case 4:
                    StorageCompatImagePath = "Images/cispro/4b.gif";
                    break;
                case 5:
                    StorageCompatImagePath = "Images/cispro/5l.gif";
                    break;
                case 6:
                    StorageCompatImagePath = "Images/cispro/6p.gif";
                    break;
                case 7:
                    StorageCompatImagePath = "Images/cispro/7r.gif";
                    break;
                default:
                    break;
            }

            return StorageCompatImagePath;
        }

        private string _NFPASpecificRatingNumberToText( Int32 NFPASpecificRating )
        {
            string NFPASpecificRatingText = string.Empty;

            // If Int32.MinValue, then there was no rating set
            if( NFPASpecificRating == Int32.MinValue )
            {
                NFPASpecificRating = 0;
            }

            switch( NFPASpecificRating )
            {
                case 0:
                    // This means the rating is "No known specific hazards" so we leave the string empty.
                    break;
                case 1:
                    // Corrosive
                    NFPASpecificRatingText = "COR";
                    break;
                case 2:
                    // Radioactive
                    NFPASpecificRatingText = "RAD";
                    break;
                case 3:
                    // Oxidizer
                    NFPASpecificRatingText = "OX";
                    break;
                case 4:
                    // Use no water
                    NFPASpecificRatingText = "W";
                    break;
                case 5:
                    // Acid
                    NFPASpecificRatingText = "ACID";
                    break;
                case 6:
                    // Alkaline
                    NFPASpecificRatingText = "ALK";
                    break;
                default:
                    break;
            }

            return NFPASpecificRatingText;
        }

        /// <summary>
        /// Hide properties not appropriate for constituents.
        /// See also CswNbtPropertySetMaterial._toggleConstituentProps()
        /// </summary>
        private void _toggleConstituentProps()
        {
            if( CswEnumTristate.True == IsConstituent.Checked )
            {
                ViewSDS.setHidden( true, true );
                ExpirationInterval.setHidden( true, true );
                IsTierII.setHidden( true, true );
                //RegulatoryLists.setHidden( true, true );
            }
        } // _toggleConstituentProps()


        /// <summary>
        /// Returns all relevant CAS numbers for this Chemical and all its constituents
        /// </summary>
        public Collection<string> getCASNos()
        {
            // get list of CAS numbers (me and my constituents)
            Collection<string> myCasNos = new Collection<string>();
            if( false == string.IsNullOrEmpty( CasNo.Text ) )
            {
                myCasNos.Add( CasNo.Text );
            }

            CswNbtMetaDataObjectClass ComponentOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialComponentClass );
            CswNbtMetaDataObjectClass ChemicalOC = this.ObjectClass;
            CswNbtMetaDataObjectClassProp ChemicalCasNoOCP = ChemicalOC.getObjectClassProp( PropertyName.CasNo );
            CswNbtMetaDataObjectClassProp ComponentMixtureOCP = ComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Mixture );
            CswNbtMetaDataObjectClassProp ComponentConstituentOCP = ComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Constituent );

            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.ViewName = "getConstituentsView";
            //CswNbtViewRelationship ChemRel = View.AddViewRelationship( this.ObjectClass, false );
            //ChemRel.NodeIdsToFilterIn.Add( this.NodeId );
            //CswNbtViewRelationship CompRel = View.AddViewRelationship( ChemRel, CswEnumNbtViewPropOwnerType.Second, ComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Mixture ), false );
            CswNbtViewRelationship CompRel = View.AddViewRelationship( ComponentOC, false );
            View.AddViewPropertyAndFilter( CompRel, ComponentMixtureOCP,
                                                    SubFieldName : CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID,
                                                    FilterMode : CswEnumNbtFilterMode.Equals,
                                                    Value : this.NodeId.PrimaryKey.ToString() );
            CswNbtViewRelationship ConstRel = View.AddViewRelationship( CompRel, CswEnumNbtViewPropOwnerType.First, ComponentConstituentOCP, false );
            View.AddViewProperty( ConstRel, ChemicalCasNoOCP );

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, RequireViewPermissions : false, IncludeSystemNodes : true, IncludeHiddenNodes : true );
            //for( Int32 i = 0; i < Tree.getChildNodeCount(); i++ ) // Chemical
            //{
            //    Tree.goToNthChild( i );
            for( Int32 j = 0; j < Tree.getChildNodeCount(); j++ ) // Component
            {
                Tree.goToNthChild( j );
                for( Int32 k = 0; k < Tree.getChildNodeCount(); k++ ) // Constituent
                {
                    Tree.goToNthChild( k );

                    CswNbtTreeNodeProp casnoTreeProp = Tree.getChildNodePropsOfNode().FirstOrDefault( p => p.ObjectClassPropName == ChemicalCasNoOCP.PropName );
                    if( null != casnoTreeProp )
                    {
                        string thisCasNo = casnoTreeProp[( (CswNbtFieldTypeRuleCASNo) ChemicalCasNoOCP.getFieldTypeRule() ).TextSubField.Column];
                        if( false == string.IsNullOrEmpty( thisCasNo ) )
                        {
                            myCasNos.Add( thisCasNo );
                        }
                    }
                    Tree.goToParentNode();
                } // for( Int32 k = 0; k < Tree.getChildNodeCount(); k++ ) // Constituent
                Tree.goToParentNode();
            } // for( Int32 j = 0; j < Tree.getChildNodeCount(); j++ ) // Component
            //    Tree.goToParentNode();
            //} // for( Int32 i = 0; i < Tree.getChildNodeCount(); i++ ) // Chemical
            return myCasNos;
        } // getCASNos()


        /// <summary>
        /// Returns nodeids for all chemicals that use this chemical as a constituent
        /// </summary>
        public CswCommaDelimitedString getMixtureMaterials()
        {
            CswCommaDelimitedString ret = new CswCommaDelimitedString();

            CswNbtMetaDataObjectClass ComponentOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialComponentClass );
            CswNbtMetaDataObjectClassProp ComponentConstituentOCP = ComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Constituent );
            CswNbtMetaDataObjectClassProp ComponentMixtureOCP = ComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Mixture );

            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.ViewName = "getMixtureMaterials";
            //CswNbtViewRelationship ConstRel = View.AddViewRelationship( this.ObjectClass, false );
            //ConstRel.NodeIdsToFilterIn.Add( this.NodeId );
            //CswNbtViewRelationship CompRel = View.AddViewRelationship( ConstRel, CswEnumNbtViewPropOwnerType.Second, ComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Constituent ), false );
            CswNbtViewRelationship CompRel = View.AddViewRelationship( ComponentOC, false );
            View.AddViewPropertyAndFilter( CompRel, ComponentConstituentOCP,
                                                    SubFieldName : CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID,
                                                    FilterMode : CswEnumNbtFilterMode.Equals,
                                                    Value : this.NodeId.PrimaryKey.ToString() );
            CswNbtViewRelationship ChemRel = View.AddViewRelationship( CompRel, CswEnumNbtViewPropOwnerType.First, ComponentMixtureOCP, false );

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, RequireViewPermissions : false, IncludeSystemNodes : true, IncludeHiddenNodes : true );
            //for( Int32 i = 0; i < Tree.getChildNodeCount(); i++ ) // Constituent
            //{
            //    Tree.goToNthChild( i );
            for( Int32 j = 0; j < Tree.getChildNodeCount(); j++ ) // Component
            {
                Tree.goToNthChild( j );
                for( Int32 k = 0; k < Tree.getChildNodeCount(); k++ ) // Chemical
                {
                    Tree.goToNthChild( k );

                    ret.Add( Tree.getNodeIdForCurrentPosition().PrimaryKey.ToString() );

                    Tree.goToParentNode();
                } // for( Int32 k = 0; k < Tree.getChildNodeCount(); k++ ) // Constituent
                Tree.goToParentNode();
            } // for( Int32 j = 0; j < Tree.getChildNodeCount(); j++ ) // Component
            //    Tree.goToParentNode();
            //} // for( Int32 i = 0; i < Tree.getChildNodeCount(); i++ ) // Chemical
            return ret;
        } // getMixtureMaterials()

        public class RegListEntry
        {
            public string RegulatoryList;
            public CswPrimaryKey RegulatoryListId;
            public CswPrimaryKey MemberId;
            public CswPrimaryKey ByUserId;
        }

        /// <summary>
        /// Returns a collection of existing regulatory list information for this Chemical
        /// </summary>
        public Collection<RegListEntry> getRegulatoryLists()
        {
            Collection<RegListEntry> ret = new Collection<RegListEntry>();
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.RegulatoryLists ) )
            {
                CswNbtMetaDataObjectClass RegulatoryListOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
                CswNbtMetaDataObjectClass RegListMemberOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListMemberClass );
                if( null != RegulatoryListOC && null != RegListMemberOC )
                {
                    CswNbtMetaDataObjectClassProp RegListNameOCP = RegulatoryListOC.getObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.Name );
                    CswNbtMetaDataObjectClassProp MemberByUserOCP = RegListMemberOC.getObjectClassProp( CswNbtObjClassRegulatoryListMember.PropertyName.ByUser );
                    CswNbtMetaDataObjectClassProp MemberChemicalOCP = RegListMemberOC.getObjectClassProp( CswNbtObjClassRegulatoryListMember.PropertyName.Chemical );

                    CswNbtView MemberView = new CswNbtView( _CswNbtResources );
                    MemberView.ViewName = "getRegListMembersView";
                    //CswNbtViewRelationship ChemRel = MemberView.AddViewRelationship( this.ObjectClass, false );
                    //ChemRel.NodeIdsToFilterIn.Add( this.NodeId );
                    //CswNbtViewRelationship MemberRel = MemberView.AddViewRelationship( ChemRel, CswEnumNbtViewPropOwnerType.Second, RegListMemberOC.getObjectClassProp( CswNbtObjClassRegulatoryListMember.PropertyName.Chemical ), false );
                    CswNbtViewRelationship MemberRel = MemberView.AddViewRelationship( RegListMemberOC, false );
                    MemberView.AddViewProperty( MemberRel, MemberByUserOCP, 1 );
                    MemberView.AddViewPropertyAndFilter( MemberRel, MemberChemicalOCP,
                                                         SubFieldName : CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID,
                                                         FilterMode : CswEnumNbtFilterMode.Equals,
                                                         Value : this.NodeId.PrimaryKey.ToString(),
                                                         ShowInGrid : false );
                    CswNbtViewRelationship RegListRel = MemberView.AddViewRelationship( MemberRel, CswEnumNbtViewPropOwnerType.First, RegListMemberOC.getObjectClassProp( CswNbtObjClassRegulatoryListMember.PropertyName.RegulatoryList ), false );
                    MemberView.AddViewProperty( RegListRel, RegListNameOCP );


                    ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( MemberView, RequireViewPermissions : false, IncludeSystemNodes : true, IncludeHiddenNodes : true );
                    //for( Int32 i = 0; i < Tree.getChildNodeCount(); i++ ) // Chemical
                    //{
                    //    Tree.goToNthChild( i );
                    for( Int32 m = 0; m < Tree.getChildNodeCount(); m++ ) // Member
                    {
                        Tree.goToNthChild( m );
                        CswPrimaryKey thisMemberId = Tree.getNodeIdForCurrentPosition();

                        CswPrimaryKey thisByUserId = null;
                        if( Tree.getChildNodePropsOfNode().Count > 0 )
                        {
                            CswNbtTreeNodeProp byUserTreeProp = Tree.getChildNodePropsOfNode().FirstOrDefault( p => p.ObjectClassPropName == MemberByUserOCP.PropName );
                            if( null != byUserTreeProp )
                            {
                                Int32 byUserId = CswConvert.ToInt32( byUserTreeProp[( (CswNbtFieldTypeRuleRelationship) MemberByUserOCP.getFieldTypeRule() ).NodeIDSubField.Column] );
                                if( Int32.MinValue != byUserId )
                                {
                                    thisByUserId = new CswPrimaryKey( "nodes", byUserId );
                                }
                            }
                        }

                        for( Int32 r = 0; r < Tree.getChildNodeCount(); r++ ) // RegList
                        {
                            Tree.goToNthChild( r );
                            CswPrimaryKey thisRegListId = Tree.getNodeIdForCurrentPosition();

                            CswNbtTreeNodeProp nameTreeProp = Tree.getChildNodePropsOfNode().FirstOrDefault( p => p.ObjectClassPropName == RegListNameOCP.PropName );
                            if( null != nameTreeProp )
                            {
                                string thisRegList = nameTreeProp[( (CswNbtFieldTypeRuleText) RegListNameOCP.getFieldTypeRule() ).TextSubField.Column];
                                if( false == string.IsNullOrEmpty( thisRegList ) )
                                {
                                    ret.Add( new RegListEntry()
                                        {
                                            MemberId = thisMemberId,
                                            RegulatoryList = thisRegList,
                                            RegulatoryListId = thisRegListId,
                                            ByUserId = thisByUserId
                                        } );
                                }
                            }

                            Tree.goToParentNode();
                        } // for( Int32 k = 0; k < Tree.getChildNodeCount(); k++ ) // RegList
                        Tree.goToParentNode();
                    } // for( Int32 j = 0; j < Tree.getChildNodeCount(); j++ ) // Member
                    //    Tree.goToParentNode();
                    //} // for( Int32 i = 0; i < Tree.getChildNodeCount(); i++ ) // Chemical

                } // if( null != RegListMemberOC )
            } // if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.RegulatoryLists ) )
            return ret;
        } // getRegulatoryLists()


        /// <summary>
        /// Refresh regulatory list membership for this Chemical
        /// Does not alter any existing Member records with a non-null ByUser value.
        /// If LOLI Sync is enabled, LOLI Managed Regulatory Lists are also refreshed.
        /// </summary>
        public void RefreshRegulatoryListMembers()
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.RegulatoryLists ) )
            {
                // get list of CAS numbers (me and my constituents)
                Collection<string> myCasNos = getCASNos();

                CswNbtMetaDataObjectClass RegulatoryListOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
                CswNbtMetaDataObjectClass RegListMemberOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListMemberClass );
                if( null != RegulatoryListOC && null != RegListMemberOC )
                {
                    CswNbtMetaDataNodeType RegListMemberNT = RegListMemberOC.FirstNodeType;
                    // get existing RegListMembers
                    Collection<RegListEntry> myRegLists = getRegulatoryLists();

                    // get new matching regulatory lists
                    Collection<CswPrimaryKey> matchingRegLists = CswNbtObjClassRegulatoryList.findMatches( _CswNbtResources, myCasNos );

                    // If the reg list matches, but no current member entry exists, add one
                    foreach( CswPrimaryKey reglistid in matchingRegLists )
                    {
                        bool any = false;
                        foreach( RegListEntry entry in myRegLists )
                        {
                            if( entry.RegulatoryListId == reglistid )
                            {
                                any = true;
                                break;
                            }
                        }
                        if( false == any && false == isRegulatoryListSuppressed( reglistid ) )
                        {
                            // add new reg list member node
                            _CswNbtResources.Nodes.makeNodeFromNodeTypeId( RegListMemberNT.NodeTypeId, delegate( CswNbtNode NewNode )
                                {
                                    CswNbtObjClassRegulatoryListMember newMemberNode = NewNode;
                                    newMemberNode.SetByChemical = true; // since this node creation was automatically determined
                                    newMemberNode.Chemical.RelatedNodeId = this.NodeId;
                                    newMemberNode.RegulatoryList.RelatedNodeId = reglistid;
                                    //newMemberNode.Show.Checked = CswEnumTristate.True;
                                } );
                        }
                    }

                    // If a current member entry exists, but the reg list doesn't match, delete it (unless it is a user override)
                    foreach( RegListEntry reglistentry in myRegLists.Where( entry => false == matchingRegLists.Any( reglistid => entry.RegulatoryListId == reglistid ) &&
                                                                                     entry.ByUserId == null ) )
                    {
                        // delete existing reg list member node
                        CswNbtObjClassRegulatoryListMember doomedMemberNode = _CswNbtResources.Nodes[reglistentry.MemberId];
                        if( null != doomedMemberNode )
                        {
                            doomedMemberNode.SetByChemical = true; // since this node deletion was automatically determined
                            doomedMemberNode.Node.delete();
                        }
                    }
                } // if( null != RegulatoryListOC && null != RegListMemberOC )

                if( CswEnumTristate.True == IsConstituent.Checked )
                {
                    // Other chemicals that use this chemical as a constituent probably need to be updated as well
                    CswDelimitedString mixMats = getMixtureMaterials();

                    // We do this directly, not using a view, for performance
                    while( mixMats.Count > 0 )
                    {
                        CswTableUpdate NodesTableUpdate = _CswNbtResources.makeCswTableUpdate( "RefreshRegulatoryListMembers_pendingupdate", "nodes" );
                        CswDelimitedString mixMatsFirstThousand = mixMats.SubString( 0, 998 );
                        mixMats = mixMats.SubString( 999, mixMats.Count );
                        DataTable NodesTable = NodesTableUpdate.getTable( "where istemp = '0' and nodeid in (" + mixMatsFirstThousand.ToString() + ")" );
                        foreach( DataRow NodesRow in NodesTable.Rows )
                        {
                            NodesRow["pendingupdate"] = "1";
                        }
                        NodesTableUpdate.update( NodesTable );
                    }
                }

            } // if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.RegulatoryLists ) )
        } // RefreshRegulatoryListMembers()

        private Dictionary<string, string> _initDsdPhraseOptions()
        {
            Dictionary<string, string> Ret = new Dictionary<string, string>();
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.DSD ) )
            {
                CswNbtMetaDataObjectClass DsdPhraseOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DSDPhraseClass );
                Dictionary<CswPrimaryKey, string> Phrases = DsdPhraseOC.getNodeIdAndNames( false, false );
                Ret = Phrases.Keys.ToDictionary( pk => pk.ToString(), pk => Phrases[pk] );
            }
            return Ret;
        } // _initDsdPhraseOptions()

        public CswNbtView setupDsdPhraseView()
        {
            CswNbtView DsdView = null;
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.DSD ) )
            {
                CswNbtMetaDataObjectClass DsdPhraseOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DSDPhraseClass );
                CswNbtMetaDataObjectClassProp CodeOCP = DsdPhraseOC.getObjectClassProp( CswNbtObjClassDSDPhrase.PropertyName.Code );
                CswNbtMetaDataObjectClassProp EngOCP = DsdPhraseOC.getObjectClassProp( CswNbtObjClassDSDPhrase.PropertyName.English );

                DsdView = LabelCodesGrid.View;

                DsdView.Root.ChildRelationships.Clear();
                if( LabelCodes.Value.Count > 0 )
                {
                    CswNbtViewRelationship parent = DsdView.AddViewRelationship( DsdPhraseOC, false );
                    DsdView.AddViewProperty( parent, CodeOCP );
                    DsdView.AddViewProperty( parent, EngOCP );

                    foreach( string PhraseId in LabelCodes.Value )
                    {
                        CswPrimaryKey PhrasePk = new CswPrimaryKey();
                        PhrasePk.FromString( PhraseId );
                        parent.NodeIdsToFilterIn.Add( PhrasePk );
                    }
                }
            }
            return DsdView;
        } // setupDsdPhraseView()

        #endregion Custom Logic

        #region Object class specific properties

        public CswNbtNodePropList PhysicalState { get { return _CswNbtNode.Properties[PropertyName.PhysicalState]; } }
        //private void _onPhysicalStatePropChange( CswNbtNodeProp prop, bool Creating )
        //{
        //if( false == String.IsNullOrEmpty( PhysicalState.Value ) )
        //{
        //    CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
        //    CswNbtView unitsOfMeasureView = Vb.getQuantityUnitOfMeasureView( _CswNbtNode.NodeId );
        //    if( null != unitsOfMeasureView )
        //    {
        //        unitsOfMeasureView.save();
        //    }
        //}
        //}
        public CswNbtNodePropNumber SpecificGravity { get { return _CswNbtNode.Properties[PropertyName.SpecificGravity]; } }
        public CswNbtNodePropImageList StorageCompatibility { get { return ( _CswNbtNode.Properties[PropertyName.StorageCompatibility] ); } }
        public CswNbtNodePropQuantity ExpirationInterval { get { return ( _CswNbtNode.Properties[PropertyName.ExpirationInterval] ); } }
        public CswNbtNodePropCASNo CasNo { get { return ( _CswNbtNode.Properties[PropertyName.CasNo] ); } }

        private void _onCasNoPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( CasNo.GetOriginalPropRowValue() != CasNo.Text )
            {
                CswC3Params CswC3Params = new CswC3Params();
                CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3Params );
                SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client( CswEnumErrorType.Warning, "Unable to sync data. Please contact your administrator. " );
                if( null != C3SearchClient )
                {
                    syncFireDbData();
                    syncPCIDData();
                }

                if( CasNo.wasAnySubFieldModified() && _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.RegulatoryLists ) )
                {
                    RefreshRegulatoryListMembers();
                }
            }
        } // _onCasNoPropChange()

        //public CswNbtNodePropStatic RegulatoryLists { get { return ( _CswNbtNode.Properties[PropertyName.RegulatoryLists] ); } }
        public CswNbtNodePropGrid RegulatoryListsGrid { get { return ( _CswNbtNode.Properties[PropertyName.RegulatoryListsGrid] ); } }
        public CswNbtNodePropText UNCode { get { return ( _CswNbtNode.Properties[PropertyName.UNCode] ); } }
        public CswNbtNodePropLogical IsTierII { get { return ( _CswNbtNode.Properties[PropertyName.IsTierII] ); } }
        public CswNbtNodePropButton ViewSDS { get { return ( _CswNbtNode.Properties[PropertyName.ViewSDS] ); } }
        public CswNbtNodePropGrid AssignedSDS { get { return ( _CswNbtNode.Properties[PropertyName.AssignedSDS] ); } }
        public CswNbtNodePropMultiList HazardClasses { get { return ( _CswNbtNode.Properties[PropertyName.HazardClasses] ); } }
        public CswNbtNodePropNFPA NFPA { get { return ( _CswNbtNode.Properties[PropertyName.NFPA] ); } }
        public CswNbtNodePropMultiList PPE { get { return ( _CswNbtNode.Properties[PropertyName.PPE] ); } }
        public CswNbtNodePropLogical Hazardous { get { return ( _CswNbtNode.Properties[PropertyName.Hazardous] ); } }
        public CswNbtNodePropFormula Formula { get { return ( _CswNbtNode.Properties[PropertyName.Formula] ); } }
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
        public CswNbtNodePropMemo SuppressedRegulatoryLists { get { return ( _CswNbtNode.Properties[PropertyName.SuppressedRegulatoryLists] ); } }
        public CswNbtNodePropRelationship LQNo { get { return ( _CswNbtNode.Properties[PropertyName.LQNo] ); } }
        public CswNbtNodePropNumber DOTCode { get { return _CswNbtNode.Properties[PropertyName.DOTCode]; } }
        public CswNbtNodePropMemo HazardInfo { get { return _CswNbtNode.Properties[PropertyName.HazardInfo]; } }
        public CswNbtNodePropLogical CompressedGas { get { return _CswNbtNode.Properties[PropertyName.CompressedGas]; } }
        public CswNbtNodePropText SMILES { get { return _CswNbtNode.Properties[PropertyName.SMILES]; } }
        public CswNbtNodePropMemo DisposalInstructions { get { return _CswNbtNode.Properties[PropertyName.DisposalInstructions]; } }
        public CswNbtNodePropQuantity OpenExpireInterval { get { return _CswNbtNode.Properties[PropertyName.OpenExpireInterval]; } }
        public CswNbtNodePropText EINECS { get { return _CswNbtNode.Properties[PropertyName.EINECS]; } }
        public CswNbtNodePropText SubclassName { get { return _CswNbtNode.Properties[PropertyName.SubclassName]; } }
        public CswNbtNodePropImageList Pictograms { get { return _CswNbtNode.Properties[PropertyName.Pictograms]; } }
        public CswNbtNodePropMultiList LabelCodes { get { return _CswNbtNode.Properties[PropertyName.LabelCodes]; } }
        public void OnLabelCodesChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( Creating == false )
            {
                CswNbtView DsdView = setupDsdPhraseView();
                if( null != DsdView )
                {
                    DsdView.SaveToCache( false, true );
                }
            }
        }

        public CswNbtNodePropMemo AddLabelCodes { get { return ( _CswNbtNode.Properties[PropertyName.AddLabelCodes] ); } }
        private void OnAddLabelCodesPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            CswCommaDelimitedString NewGHSCodesDelimited = new CswCommaDelimitedString();
            NewGHSCodesDelimited.FromString( AddLabelCodes.Text, true );
            foreach( string LabelCode in NewGHSCodesDelimited )
            {
                if( LabelCodes.Options.ContainsValue( LabelCode.Trim().ToUpper() ) )
                {
                    LabelCodes.AddValue( LabelCodes.Options.FirstOrDefault( x => x.Value == LabelCode.Trim().ToUpper() ).Key );
                }
            }
            AddLabelCodes.Text = string.Empty;
        }

        public CswNbtNodePropGrid LabelCodesGrid { get { return _CswNbtNode.Properties[PropertyName.LabelCodesGrid]; } }
        public CswNbtNodePropGrid LinkChemWatch { get { return _CswNbtNode.Properties[PropertyName.LinkChemWatch]; } }
        public CswNbtNodePropMemo ProductDescription { get { return _CswNbtNode.Properties[PropertyName.ProductDescription]; } }
        public CswNbtNodePropText LegacyMaterialId { get { return _CswNbtNode.Properties[PropertyName.LegacyMaterialId]; } }
        public CswNbtNodePropTimeInterval OpenExpirationInterval { get { return _CswNbtNode.Properties[PropertyName.OpenExpirationInterval]; } }

        #endregion Object class specific properties

        #region Suppressed Regulatory Lists helpers

        public bool isRegulatoryListSuppressed( CswPrimaryKey regListId )
        {
            bool ret = false;
            if( CswTools.IsPrimaryKey( regListId ) )
            {
                CswCommaDelimitedString SuppressedList = new CswCommaDelimitedString();
                SuppressedList.FromString( SuppressedRegulatoryLists.Text );
                ret = SuppressedList.Contains( regListId.PrimaryKey.ToString() );
            }
            return ret;
        } // removeSuppressedRegulatoryList

        public void removeSuppressedRegulatoryList( CswPrimaryKey regListId )
        {
            if( CswTools.IsPrimaryKey( regListId ) )
            {
                CswCommaDelimitedString SuppressedList = new CswCommaDelimitedString();
                SuppressedList.FromString( SuppressedRegulatoryLists.Text );
                SuppressedList.Remove( regListId.PrimaryKey.ToString() );
                SuppressedRegulatoryLists.Text = SuppressedList.ToString();
            }
        } // removeSuppressedRegulatoryList

        public void addSuppressedRegulatoryList( CswPrimaryKey regListId )
        {
            if( CswTools.IsPrimaryKey( regListId ) )
            {
                CswCommaDelimitedString SuppressedList = new CswCommaDelimitedString();
                SuppressedList.FromString( SuppressedRegulatoryLists.Text );
                SuppressedList.Add( regListId.PrimaryKey.ToString() );
                SuppressedRegulatoryLists.Text = SuppressedList.ToString();
            }
        } // addSuppressedRegulatoryList

        #endregion Suppressed Regulatory Lists helpers

    }//CswNbtObjClassChemical

}//namespace ChemSW.Nbt.ObjClasses