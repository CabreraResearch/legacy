using System;
using ChemSW.Core;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.ChemCatCentral;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
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
            ViewSDS.MenuOptions = PropertyName.ViewSDS + ",View All";

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
            if( false == CswNbtObjClassSDSDocument.materialHasActiveSDS( _CswNbtResources, NodeId ) )
            {
                ViewSDS.setHidden( true, false );
            }
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
            bool canAddSDS = false;
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

                                    CswNbtObjClassSDSDocument SDSDoc = Tree.getNodeForCurrentPosition();
                                    if( null != RevisionDateProp )
                                    {
                                        DateTime RevisionDate = SDSDoc.Node.Properties[RevisionDateProp].AsDateTime.DateTimeValue;
                                        Doc["revisiondate"] = RevisionDate == DateTime.MinValue ? "" : RevisionDate.ToShortDateString();
                                    }
                                    else
                                    {
                                        Doc["revisiondate"] = "";
                                    }
                                    if( SDSDoc.FileType.Value.Equals( CswNbtPropertySetDocument.CswEnumDocumentFileTypes.File ) )
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
            ButtonData.Data["state"]["canAddSDS"] = canAddSDS;
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
                        ButtonData.Data["viewid"] = assignedSDSDocsView.SessionViewId.ToString();
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

                        // Set the C3SyncDate property
                        this.C3SyncDate.DateTimeValue = DateTime.Now;
                    }
                }
            }
        }//syncFireDbData()

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
                CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources,
                                                                                        CswC3SearchParams );
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
                        ChemCatCentral.CswC3ExtChemData C3ExtChemData = SearchResults.ExtChemDataResults[0];

                        // NFPA
                        if( string.IsNullOrEmpty( this.NFPA.Red ) )
                        {
                            NFPA.Red = CswConvert.ToString( C3ExtChemData.ExtensionData1.PcidData.NFPA.NfpaFire );
                        }
                        if( string.IsNullOrEmpty( this.NFPA.Yellow ) )
                        {
                            NFPA.Yellow = CswConvert.ToString( C3ExtChemData.ExtensionData1.PcidData.NFPA.NfpaReact );
                        }
                        if( string.IsNullOrEmpty( this.NFPA.Blue ) )
                        {
                            NFPA.Blue = CswConvert.ToString( C3ExtChemData.ExtensionData1.PcidData.NFPA.NfpaHealth );
                        }
                        if( string.IsNullOrEmpty( this.NFPA.White ) )
                        {
                            string NFPASpecificRating = _NFPASpecificRatingNumberToText( C3ExtChemData.ExtensionData1.PcidData.NFPA.NfpaSpecific );
                            NFPA.White = NFPASpecificRating;
                        }

                        // PPE
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

                        // Storage Compatibility
                        if( StorageCompatibility.Value.IsEmpty )
                        {
                            string StorageCompatImagePath = _getStorageCompatImagePath( C3ExtChemData.ExtensionData1.PcidData.StorageCompatibility );
                            CswDelimitedString StorageCompatValue = new CswDelimitedString( '\n' );
                            StorageCompatValue.Add( StorageCompatImagePath );
                            StorageCompatibility.Value = StorageCompatValue;
                        }

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
                                        CswNbtSdBlobData SdBlobData = new CswNbtSdBlobData( _CswNbtResources );
                                        SdBlobData.saveMol( molData, propAttr, out Href, out FormattedMolString, false );
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

                        }//foreach( CswC3ExtChemData.PCID.AdditionalProperty Property in C3ExtChemData.ExtensionData1.PcidData.AdditionalProperties )

                        // Set the C3SyncDate property
                        this.C3SyncDate.DateTimeValue = DateTime.Now;

                    }//if( SearchResults.ExtChemDataResults.Length > 0 )

                }//if( null != SearchResults.ExtChemDataResults )

            }//if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.PCIDSync ) )
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
                    syncPCIDData();
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