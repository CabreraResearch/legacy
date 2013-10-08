using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.UnitsOfMeasure;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Actions
{
    /// <summary>
    /// Utility class for Create Material logic
    /// </summary>
    public class CswNbtActCreateMaterial
    {
        #region NewMaterial Class
        /// <summary>
        /// Helper class for creating a New Material node with an eye toward strict validation
        /// </summary>
        public class NewMaterial
        {
            private CswNbtResources _NbtResources;
            private Int32 _NodeTypeId;
            private string _TradeName;
            private CswPrimaryKey _SupplierId;
            private string _PartNo;
            private CswNbtPropertySetMaterial _ExistingNode;
            private CswNbtMetaDataNodeType _MaterialNt;
            private CswNbtObjClassVendor _Supplier;
            private string _NodeTypeName;
            private CswPrimaryKey _MaterialId;

            /// <summary>
            /// Standard constructor for validating required properties
            /// </summary>
            public NewMaterial( CswNbtResources CswNbtResources, Int32 NodeTypeId, string TradeName, CswPrimaryKey SupplierId, string PartNo = "", string NodeId = "" )
            {
                _NbtResources = CswNbtResources;
                this.NodeTypeId = NodeTypeId;
                this.TradeName = TradeName;
                this.SupplierId = SupplierId;
                this.PartNo = PartNo;
                //If we are providing an existing material
                this._MaterialId = CswConvert.ToPrimaryKey( NodeId );
                if( CswTools.IsPrimaryKey( _MaterialId ) )
                {
                    Node = _NbtResources.Nodes[_MaterialId];
                }
            }

            /// <summary>
            /// Secondary constructor for continuing work on a new Material node
            /// </summary>
            public NewMaterial( CswNbtResources CswNbtResources, CswNbtNode Node )
            {
                _NbtResources = CswNbtResources;
                CswNbtMetaDataPropertySet MaterialSet = Node.ObjClass.ObjectClass.getPropertySet();
                if( null == MaterialSet || MaterialSet.Name != CswEnumNbtPropertySetName.MaterialSet )
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "Cannot create a new Material object without a valid Material Type.", "Could not resolve NodeType for NodeTypeId: " + NodeTypeId + "." );
                }
                this.Node = Node;
            }

            public Int32 NodeTypeId
            {
                get { return _NodeTypeId; }
                set
                {
                    Int32 Id = value;
                    CswNbtMetaDataNodeType PotentialNt = _NbtResources.MetaData.getNodeType( Id );
                    if( null == PotentialNt )
                    {
                        throw new CswDniException( CswEnumErrorType.Warning, "Cannot create a new Material object without a valid Material Type.", "Could not resolve NodeType for NodeTypeId: " + NodeTypeId + "." );
                    }
                    CswNbtMetaDataPropertySet MaterialSet = PotentialNt.getObjectClass().getPropertySet();
                    if( null == MaterialSet || MaterialSet.Name != CswEnumNbtPropertySetName.MaterialSet )
                    {
                        throw new CswDniException( CswEnumErrorType.Warning, "Cannot create a new Material object without a valid Material Type.", "Cannot make a Material for Object Class: " + PotentialNt.getObjectClass().ObjectClass + "." );
                    }
                    _NodeTypeName = PotentialNt.NodeTypeName;
                    _MaterialNt = PotentialNt;
                    _NodeTypeId = Id;
                }
            }

            public string NodeTypeName
            {
                get { return _NodeTypeName; }
            }

            public string TradeName
            {
                get { return _TradeName; }
                set
                {
                    string PotentialName = value;
                    if( String.IsNullOrEmpty( PotentialName ) )
                    {
                        throw new CswDniException( CswEnumErrorType.Warning, "A Tradename is required to create a new Material.", "Provided Tradename was null or empty." );
                    }

                    _TradeName = PotentialName;
                }
            }

            public CswPrimaryKey SupplierId
            {
                get { return _SupplierId; }
                set
                {
                    CswNbtObjClassVendor PotentialSupplier = _NbtResources.Nodes[value];
                    if( null == PotentialSupplier || PotentialSupplier.ObjectClass.ObjectClass != CswEnumNbtObjectClass.VendorClass )
                    {
                        throw new CswDniException( CswEnumErrorType.Warning, "Cannot create a new Material object without a valid Supplier.", "Provided SupplierId was invalid." );
                    }

                    _Supplier = PotentialSupplier;
                    _SupplierId = PotentialSupplier.NodeId;
                }
            }

            public string SupplierName
            {
                get { return _Supplier.VendorName.Text; }
            }

            public string PartNo
            {
                get { return _PartNo; }
                set { _PartNo = value; }
            }

            public bool existsInDb( bool ForceRecalc = false )
            {
                CswNbtPropertySetMaterial ExistingMaterial = existingMaterial( ForceRecalc );
                return ( ExistingMaterial != null && false == ExistingMaterial.IsTemp );
            }

            public CswNbtPropertySetMaterial existingMaterial( bool ForceRecalc = false )
            {
                if( ForceRecalc || null == _ExistingNode )
                {
                    // If a preexisting material was provided, Node will not be null
                    // because it was set in the constructor
                    //_ExistingNode = Node;
                    if( null == _ExistingNode )
                    {
                        _ExistingNode = CswNbtPropertySetMaterial.getExistingMaterial( _NbtResources, NodeTypeId, SupplierId, TradeName, PartNo );
                    }
                }
                return _ExistingNode;
            }

            public CswNbtPropertySetMaterial commit( bool RemoveTempStatus = false )
            {
                CswNbtPropertySetMaterial Ret;
                if( null == Node ) //Don't commit twice
                {
                    if( existsInDb() )
                    {
                        throw new CswDniException( CswEnumErrorType.Warning, "A material with the same Type, Tradename, Supplier and PartNo already exists.", "A material with this configuration already exists. Name: " + _ExistingNode.NodeName + " , ID: " + _ExistingNode.NodeId + "." );
                    }
                    if( false == existsInDb() && Int32.MinValue != NodeTypeId )
                    {
                        Ret = _NbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, IsTemp: true );
                        Node = Ret.Node;
                    }
                    else
                    {
                        throw new CswDniException( CswEnumErrorType.Warning, "Cannot create a new Material object without a valid Supplier.", "Provided SupplierId was invalid." );
                    }
                }
                else
                {
                    Ret = Node;
                }

                Ret.TradeName.Text = TradeName;
                Ret.PartNumber.Text = PartNo;
                Ret.Supplier.RelatedNodeId = SupplierId;
                Ret.ApprovedForReceiving.Checked = CswConvert.ToTristate( _NbtResources.Permit.can( CswEnumNbtActionName.Material_Approval ) );

                Ret.IsTemp = ( false == RemoveTempStatus );
                Ret.postChanges( ForceUpdate: false );

                return Ret;
            }


            public CswNbtNode Node { get; private set; }
        }

        #endregion NewMaterial Class

        #region ctor

        private CswNbtResources _CswNbtResources;

        /// <summary>
        /// Base Constructor
        /// </summary>
        public CswNbtActCreateMaterial( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

            if( false == _CswNbtResources.Permit.can( CswEnumNbtActionName.Create_Material, _CswNbtResources.CurrentNbtUser ) )
            {
                throw new CswDniException( CswEnumErrorType.Error, "You do not have permission to use the Create Material wizard.", "Attempted to access the Create Material wizard with role of " + _CswNbtResources.CurrentNbtUser.Rolename );
            }
        }

        #endregion ctor

        #region Wizard Setup

        /// <summary>
        /// Get the view to drive the Supplier picklist in the Create Material wizard
        /// </summary>
        public CswNbtView getMaterialSuppliersView()
        {
            CswNbtView Ret = new CswNbtView( _CswNbtResources );
            CswNbtMetaDataObjectClass VendorOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
            CswNbtViewRelationship SupplierVr = Ret.AddViewRelationship( VendorOc, IncludeDefaultFilters: true );

            //This matches the MLM module event logic, but it may need adjustment down the line
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.MLM ) )
            {
                CswNbtMetaDataObjectClassProp CoorporateOcp = VendorOc.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorTypeName );
                Ret.AddViewPropertyAndFilter( SupplierVr,
                                              MetaDataProp: CoorporateOcp,
                                              Value: CswNbtObjClassVendor.VendorTypes.Corporate,
                                              FilterMode: CswEnumNbtFilterMode.Equals );
            }

            Ret.ViewName = "Create Material Supplier";
            //Case 30335 - This is required for MLM to filter to Corporate Vendors
            Ret.SaveToCache( IncludeInQuickLaunch: false );
            return Ret;
        }

        #endregion Wizard Setup

        #region Temp Material Logic

        public JObject initNewTempMaterialNode( Int32 NodeTypeId, string SupplierId, string Suppliername, string Tradename, string PartNo, string NodeId )
        {
            JObject Ret = new JObject();

            //Check if the vendor needs to be created
            if( false == CswTools.IsPrimaryKey( CswConvert.ToPrimaryKey( SupplierId ) ) )
            {
                CswNbtMetaDataObjectClass VendorOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
                if( null != VendorOC )
                {
                    CswNbtMetaDataNodeType VendorNT = VendorOC.FirstNodeType;
                    if( null != VendorNT )
                    {
                        CswNbtObjClassVendor NewVendorNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( VendorNT.NodeTypeId, IsTemp: true, OnAfterMakeNode: delegate( CswNbtNode NewNode )
                            {
                                ( (CswNbtObjClassVendor) NewNode ).VendorName.Text = Suppliername;
                                ( (CswNbtObjClassVendor) NewNode ).VendorName.SyncGestalt();
                                //NewVendorNode.postChanges( true );
                            } );
                        //Set the supplierId to the new vendor node
                        SupplierId = NewVendorNode.NodeId.ToString();
                    }
                }
            }

            CswPrimaryKey CurrentTempNodePk = CswConvert.ToPrimaryKey( NodeId );
            if( CswTools.IsPrimaryKey( CurrentTempNodePk ) )
            {
                CswNbtPropertySetMaterial CurrentTempNode = _CswNbtResources.Nodes.GetNode( CurrentTempNodePk );
                if( null != CurrentTempNode )
                {
                    Int32 CurrentNodeTypeId = CurrentTempNode.NodeTypeId;
                    if( NodeTypeId != CurrentNodeTypeId )
                    {
                        // Then we want to just forget about the first temp node created and create a new one with the new nodetype
                        Ret = _tryCreateTempMaterial( NodeTypeId, CswConvert.ToPrimaryKey( SupplierId ), Tradename, PartNo, null );
                    }
                    else
                    {
                        // If the nodetype isn't different then we want to get the props and check if it exsits
                        Ret = _tryCreateTempMaterial( NodeTypeId, CswConvert.ToPrimaryKey( SupplierId ), Tradename, PartNo, CurrentTempNodePk.ToString() );
                    }
                }
            }
            return Ret;
        }

        private JObject _tryCreateTempMaterial( Int32 MaterialNodeTypeId, CswPrimaryKey SupplierId, string TradeName, string PartNo, string NodeId )
        {
            JObject Ret = new JObject();

            NewMaterial PotentialMaterial = new NewMaterial( _CswNbtResources, MaterialNodeTypeId, TradeName, SupplierId, PartNo, NodeId );

            Ret["materialexists"] = PotentialMaterial.existsInDb();
            if( false == PotentialMaterial.existsInDb() )
            {
                CswNbtPropertySetMaterial NodeAsMaterial = PotentialMaterial.Node;
                if( null == NodeAsMaterial )
                {
                    NodeAsMaterial = PotentialMaterial.commit();
                }
                if( null != NodeAsMaterial )
                {
                    Ret["materialid"] = NodeAsMaterial.NodeId.ToString();
                    Ret["tradename"] = NodeAsMaterial.TradeName.Text;
                    Ret["partno"] = NodeAsMaterial.PartNumber.Text;
                    Ret["supplier"] = NodeAsMaterial.Supplier.CachedNodeName;
                    Ret["supplierid"] = SupplierId.ToString();
                    Ret["nodetypeid"] = NodeAsMaterial.NodeTypeId;
                    NodeAsMaterial.Save.setHidden( value: true, SaveToDb: true );
                    CswNbtSdTabsAndProps SdProps = new CswNbtSdTabsAndProps( _CswNbtResources );
                    Ret["properties"] = SdProps.getProps( NodeAsMaterial.Node, string.Empty, null, CswEnumNbtLayoutType.Add );
                    if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.SDS ) )
                    {
                        CswNbtMetaDataObjectClass SDSDocOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SDSDocumentClass );
                        CswNbtMetaDataNodeType SDSNodeType = SDSDocOC.FirstNodeType;
                        if( null != SDSNodeType )
                        {
                            Ret["documenttypeid"] = SDSNodeType.NodeTypeId;
                        }
                    }
                    Ret["noderef"] = NodeAsMaterial.Node.NodeLink; //for the link
                }
            }
            else
            {
                CswNbtPropertySetMaterial ExisitingMaterial = PotentialMaterial.existingMaterial();
                Ret["noderef"] = ExisitingMaterial.Node.NodeLink;
            }

            return Ret;
        }

        /// <summary>
        /// Makes a temporary node of the Chemical nodetype. The reason we can't use createMaterial()
        /// is because we don't have the any properties to provide to the method and tradename,
        /// material type, and supplier are required.
        /// </summary>
        /// <param name="NodeId"></param>
        /// <returns></returns>
        public CswPrimaryKey makeTemp( string NodeId )
        {
            CswPrimaryKey Ret = new CswPrimaryKey();

            CswPrimaryKey NodePk = CswConvert.ToPrimaryKey( NodeId );

            if( false == CswTools.IsPrimaryKey( NodePk ) ) //node doesn't exist
            {
                CswNbtMetaDataObjectClass ChemicalOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
                CswNbtMetaDataNodeType ChemicalNT = ChemicalOC.FirstNodeType;
                if( null != ChemicalNT )
                {
                    CswNbtPropertySetMaterial NewMaterialTempNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ChemicalNT.NodeTypeId, IsTemp: true );
                    if( null != NewMaterialTempNode )
                    {
                        Ret = NewMaterialTempNode.Node.NodeId;
                    }
                }
            }
            else //node exists
            {
                Ret = NodePk;
            }

            return Ret;
        }

        #endregion Temp Material Logic

        #region Commit Material Logic

        /// <summary>
        /// Finalize the new Material
        /// </summary>
        public JObject commitMaterial( string MaterialDefinition )
        {
            JObject RetObj = new JObject();

            JObject MaterialObj = CswConvert.ToJObject( MaterialDefinition );
            if( MaterialObj.HasValues )
            {
                JArray SizesArray = CswConvert.ToJArray( MaterialObj["sizeNodes"] );
                CswPrimaryKey MaterialId = new CswPrimaryKey();
                MaterialId.FromString( CswConvert.ToString( MaterialObj["materialId"] ) );
                if( CswTools.IsPrimaryKey( MaterialId ) )
                {
                    CswNbtNode MaterialNode = _CswNbtResources.Nodes[MaterialId];
                    if( null != MaterialNode )
                    {
                        /* 1. Validate the new material and get its properties */
                        MaterialNode = _commitMaterialNode( MaterialObj );
                        RetObj["createdmaterial"] = true;

                        /* 2. Add the sizes */
                        if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
                        {
                            SizesArray = _removeDuplicateSizes( SizesArray );
                            _addMaterialSizes( SizesArray, MaterialNode );
                            RetObj["sizescount"] = SizesArray.Count;
                        }

                        /* 3. Add landingpage data */
                        RetObj["landingpagedata"] = _getLandingPageData( MaterialNode );
                    }
                }
            }
            return RetObj;
        }

        private CswNbtNode _commitMaterialNode( JObject MaterialObj )
        {
            CswNbtNode Ret = null;

            Int32 MaterialNodeTypeId = CswConvert.ToInt32( MaterialObj["materialnodetypeid"] );
            if( Int32.MinValue != MaterialNodeTypeId )
            {
                CswNbtMetaDataNodeType MaterialNt = _CswNbtResources.MetaData.getNodeType( MaterialNodeTypeId );
                if( null != MaterialNt )
                {
                    Ret = _CswNbtResources.Nodes[CswConvert.ToString( MaterialObj["materialId"] )];
                    if( null != Ret )
                    {
                        // Set the Vendor node property isTemp = false if necessary
                        CswPrimaryKey VendorNodePk = CswConvert.ToPrimaryKey( CswConvert.ToString( MaterialObj["supplierid"] ) );
                        if( CswTools.IsPrimaryKey( VendorNodePk ) )
                        {
                            CswNbtObjClassVendor VendorNode = _CswNbtResources.Nodes.GetNode( VendorNodePk );
                            if( null != VendorNode && VendorNode.IsTemp )
                            {
                                VendorNode.IsTemp = false;
                                VendorNode.postChanges( false );
                            }
                        }

                        Ret.IsTemp = false;
                        JObject MaterialProperties = (JObject) MaterialObj["properties"];
                        CswNbtSdTabsAndProps SdTabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );
                        SdTabsAndProps.saveProps( Ret.NodeId, Int32.MinValue, MaterialProperties, Ret.NodeTypeId, null, IsIdentityTab: false );

                        NewMaterial FinalMaterial = new NewMaterial( _CswNbtResources, Ret );
                        FinalMaterial.TradeName = CswConvert.ToString( MaterialObj["tradename"] );
                        FinalMaterial.SupplierId = CswConvert.ToPrimaryKey( CswConvert.ToString( MaterialObj["supplierid"] ) );
                        FinalMaterial.PartNo = CswConvert.ToString( MaterialObj["partno"] );

                        CswNbtPropertySetMaterial NodeAsMaterial = FinalMaterial.commit( RemoveTempStatus: true );
                        NodeAsMaterial.Save.setHidden( value: false, SaveToDb: true );

                        JObject RequestObj = CswConvert.ToJObject( MaterialObj["request"] );
                        if( RequestObj.HasValues )
                        {
                            _processRequest( CswConvert.ToString( RequestObj["requestitemid"] ), FinalMaterial.Node.NodeId );
                        }
                        CswNbtActReceiving Receiving = new CswNbtActReceiving( _CswNbtResources );
                        Receiving.commitSDSDocNode( NodeAsMaterial.NodeId, MaterialObj );
                    }
                }

                if( null == Ret )
                {
                    throw new CswDniException( CswEnumErrorType.Error,
                                               "Failed to create new material.",
                                               "Attempted to call _commitMaterialNode failed." );
                }
            }
            return Ret;
        }

        private void _processRequest( String RequestItemId, CswPrimaryKey MaterialId )
        {
            CswNbtObjClassRequestMaterialCreate RequestCreate = _CswNbtResources.Nodes[RequestItemId];
            if( null != RequestCreate )
            {
                RequestCreate.Material.RelatedNodeId = MaterialId;
                RequestCreate.Status.Value = CswNbtObjClassRequestMaterialCreate.Statuses.Created;
                RequestCreate.Fulfill.State = CswNbtObjClassRequestMaterialCreate.FulfillMenu.Complete;
                RequestCreate.Fulfill.MenuOptions = CswNbtObjClassRequestMaterialCreate.FulfillMenu.Complete;
                RequestCreate.postChanges( ForceUpdate: false );

                CswNbtMetaDataObjectClass RequestMaterialDispenseOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestMaterialDispenseClass );
                CswNbtMetaDataNodeType RequestMaterialDispenseNT = RequestMaterialDispenseOC.FirstNodeType;
                if( null != RequestMaterialDispenseNT )
                {
                    _CswNbtResources.Nodes.makeNodeFromNodeTypeId( RequestMaterialDispenseNT.NodeTypeId, delegate( CswNbtNode NewNode )
                        {
                            CswNbtObjClassRequestMaterialDispense RequestDispense = NewNode;
                            RequestDispense.Request.RelatedNodeId = RequestCreate.Request.RelatedNodeId;
                            RequestDispense.Location.SelectedNodeId = RequestCreate.Location.SelectedNodeId;
                            RequestDispense.Location.CachedNodeName = RequestCreate.Location.CachedNodeName;
                            RequestDispense.Location.CachedPath = RequestCreate.Location.CachedPath;
                            RequestDispense.Quantity.Quantity = RequestCreate.Quantity.Quantity;
                            RequestDispense.Quantity.UnitId = RequestCreate.Quantity.UnitId;
                            RequestDispense.Material.RelatedNodeId = MaterialId;
                            RequestDispense.InventoryGroup.RelatedNodeId = RequestCreate.InventoryGroup.RelatedNodeId;
                            RequestDispense.ExternalOrderNumber.Text = RequestCreate.ExternalOrderNumber.Text;
                            RequestDispense.Requestor.RelatedNodeId = RequestCreate.Requestor.RelatedNodeId;
                            RequestDispense.RequestedFor.RelatedNodeId = RequestCreate.RequestedFor.RelatedNodeId;
                            RequestDispense.Comments.CommentsJson = RequestCreate.Comments.CommentsJson;
                            RequestDispense.NeededBy.DateTimeValue = RequestCreate.NeededBy.DateTimeValue;
                            RequestDispense.Priority.Value = RequestCreate.Priority.Value;
                            RequestDispense.Status.Value = CswNbtObjClassRequestMaterialDispense.Statuses.Submitted;
                            RequestDispense.Type.Value = CswNbtObjClassRequestMaterialDispense.Types.Bulk;
                            RequestDispense.setRequestDescription();
                            //RequestDispense.postChanges( false );
                        } );
                }
            }
        }

        public JObject saveMaterialProps( CswPrimaryKey NodePk, JObject PropsObj, Int32 NodeTypeId )
        {
            JObject Ret = new JObject();

            if( CswTools.IsPrimaryKey( NodePk ) )
            {
                CswNbtPropertySetMaterial MaterialNode = _CswNbtResources.Nodes.GetNode( NodePk );

                CswNbtSdTabsAndProps SdTabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );
                SdTabsAndProps.saveNodeProps( MaterialNode.Node, PropsObj );

                switch( MaterialNode.ObjectClass.ObjectClass )
                {
                    case CswEnumNbtObjectClass.ChemicalClass:
                        CswNbtObjClassChemical ChemicalNode = MaterialNode.Node;
                        Ret["PhysicalState"] = ChemicalNode.PhysicalState.Value;

                        // Add more properties here if you want.

                        break;
                    case CswEnumNbtObjectClass.NonChemicalClass:
                        Ret["PhysicalState"] = CswNbtPropertySetMaterial.CswEnumPhysicalState.NA;

                        // Add properties here!

                        break;
                }
            }

            return Ret;
        }

        #endregion Commit Material Logic

        #region Size Logic

        public static JObject getSizeNodeProps( CswNbtResources CswNbtResources, Int32 SizeNodeTypeId, string SizeDefinition, bool WriteNode )
        {
            JObject SizeObj = CswConvert.ToJObject( SizeDefinition, true, "size" );
            CswNbtNode SizeNode;
            return getSizeNodeProps( CswNbtResources, SizeNodeTypeId, SizeObj, WriteNode, out SizeNode );
        }

        public static JObject getSizeNodeProps( CswNbtResources CswNbtResources, Int32 SizeNodeTypeId, string SizeDefinition, bool WriteNode, out CswNbtNode SizeNode )
        {
            JObject SizeObj = CswConvert.ToJObject( SizeDefinition, true, "size" );
            return getSizeNodeProps( CswNbtResources, SizeNodeTypeId, SizeObj, WriteNode, out SizeNode );
        }

        public static JObject getSizeNodeProps( CswNbtResources CswNbtResources, Int32 SizeNodeTypeId, JObject SizeObj, bool WriteNode, out CswNbtNode SizeNode )
        {
            JObject Ret = new JObject();

            CswPrimaryKey UnitIdPK = CswConvert.ToPrimaryKey( SizeObj["uom"]["id"].ToString() );
            if( null != UnitIdPK )
            {
                SizeNode = CswNbtResources.Nodes.makeNodeFromNodeTypeId( SizeNodeTypeId, OverrideUniqueValidation: false, OnAfterMakeNode: delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassSize NodeAsSize = (CswNbtObjClassSize) NewNode;
                        NodeAsSize.InitialQuantity.Quantity = CswConvert.ToDouble( SizeObj["quantity"]["value"] );
                        NodeAsSize.InitialQuantity.UnitId = UnitIdPK;
                        NodeAsSize.CatalogNo.Text = SizeObj["catalogNo"]["value"].ToString();
                        NodeAsSize.QuantityEditable.Checked = CswConvert.ToTristate( SizeObj["quantityEditable"]["value"] );
                        NodeAsSize.Dispensable.Checked = CswConvert.ToTristate( SizeObj["dispensible"]["value"] );
                        NodeAsSize.UnitCount.Value = CswConvert.ToDouble( SizeObj["unitCount"]["value"] );
                    } );
            }
            else
            {
                SizeNode = null;
            }

            return Ret;
        }

        private JArray _removeDuplicateSizes( JArray SizesArray )
        {
            JArray UniqueSizesArray = new JArray();
            foreach( JObject SizeObj in SizesArray )
            {
                bool addSizeToCompare = true;
                if( SizeObj.HasValues )
                {
                    foreach( JObject UniqueSizeObj in UniqueSizesArray )
                    {
                        if( UniqueSizeObj["uom"]["id"].ToString() == SizeObj["uom"]["id"].ToString() &&
                            UniqueSizeObj["quantity"]["value"].ToString() == SizeObj["quantity"]["value"].ToString() &&
                            UniqueSizeObj["catalogNo"]["value"].ToString() == SizeObj["catalogNo"]["value"].ToString() )
                        {
                            addSizeToCompare = false;
                        }
                    }
                    if( addSizeToCompare )
                    {
                        UniqueSizesArray.Add( SizeObj );
                    }
                }
            }
            return UniqueSizesArray;
        }

        /// <summary>
        /// Make nodes for defined sizes, else remove undefinable sizes from the JArray
        /// </summary>
        private void _addMaterialSizes( JArray SizesArray, CswNbtNode MaterialNode )
        {
            JArray ArrayToIterate = (JArray) SizesArray.DeepClone();
            foreach( JObject SizeObj in ArrayToIterate )
            {
                if( SizeObj.HasValues )
                {
                    CswNbtNode SizeNode;
                    Int32 SizeNtId = CswConvert.ToInt32( SizeObj["nodeTypeId"]["value"] );
                    if( Int32.MinValue != SizeNtId )
                    {
                        getSizeNodeProps( _CswNbtResources, SizeNtId, SizeObj, false, out SizeNode );
                        if( null != SizeNode )
                        {
                            CswNbtObjClassSize NodeAsSize = SizeNode;
                            NodeAsSize.Material.RelatedNodeId = MaterialNode.NodeId;
                            SizeNode.postChanges( true );
                        }
                        else
                        {
                            SizesArray.Remove( SizeObj );
                        }
                    }
                    else
                    {
                        SizesArray.Remove( SizeObj );
                    }
                }
                else
                {
                    SizesArray.Remove( SizeObj );
                }
            }
        }

        public JObject getSizeLogicalsVisibility( int SizeNodeTypeId )
        {
            JObject ret = new JObject();
            ret["showQuantityEditable"] = "false";
            ret["showDispensable"] = "false";
            CswNbtMetaDataNodeType SizeNt = _CswNbtResources.MetaData.getNodeType( SizeNodeTypeId );
            if( null != SizeNt )
            {
                CswNbtMetaDataNodeTypeProp QuantityEditable = SizeNt.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.QuantityEditable );
                if( null != QuantityEditable.AddLayout )
                {
                    ret["showQuantityEditable"] = "true";
                }
                CswNbtMetaDataNodeTypeProp Dispensable = SizeNt.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.Dispensable );
                if( null != Dispensable.AddLayout )
                {
                    ret["showDispensable"] = "true";
                }
            }
            return ret;
        }

        public static JObject getMaterialUnitsOfMeasure( string PhysicalStateValue, CswNbtResources CswNbtResources )
        {
            JObject ret = new JObject();
            string PhysicalState = "n/a";
            foreach( string CurrentPhysicalState in CswNbtPropertySetMaterial.CswEnumPhysicalState.Options )
            {
                if( PhysicalStateValue.Equals( CurrentPhysicalState ) )
                {
                    PhysicalState = CurrentPhysicalState;
                }
            }

            if( false == string.IsNullOrEmpty( PhysicalState ) )
            {
                CswNbtUnitViewBuilder unitViewBuilder = new CswNbtUnitViewBuilder( CswNbtResources );

                CswNbtView unitsView = unitViewBuilder.getQuantityUnitOfMeasureView( PhysicalState );

                Collection<CswNbtNode> _UnitNodes = new Collection<CswNbtNode>();
                ICswNbtTree UnitsTree = CswNbtResources.Trees.getTreeFromView( CswNbtResources.CurrentNbtUser, unitsView, true, false, false );
                UnitsTree.goToRoot();
                for( int i = 0; i < UnitsTree.getChildNodeCount(); i++ )
                {
                    UnitsTree.goToNthChild( i );
                    _UnitNodes.Add( UnitsTree.getNodeForCurrentPosition() );
                    UnitsTree.goToParentNode();
                }

                foreach( CswNbtNode unitNode in _UnitNodes )
                {
                    CswNbtObjClassUnitOfMeasure nodeAsUnitOfMeasure = (CswNbtObjClassUnitOfMeasure) unitNode;
                    ret[nodeAsUnitOfMeasure.NodeId.ToString()] = nodeAsUnitOfMeasure.Name.Gestalt;
                }
            }

            return ret;
        }

        #endregion Size Logic

        #region LandingPage Logic

        private JObject _getLandingPageData( CswNbtNode MaterialNode )
        {
            return getLandingPageData( _CswNbtResources, MaterialNode );
        }

        /// <summary>
        /// Get a landing page for a Material
        /// </summary>
        public static JObject getLandingPageData( CswNbtResources NbtResources, CswNbtNode MaterialNode, CswNbtView MaterialNodeView = null )
        {
            JObject Ret = new JObject();
            if( null != MaterialNode )
            {
                MaterialNodeView = MaterialNodeView ?? CswNbtPropertySetMaterial.getMaterialNodeView( NbtResources, MaterialNode );
                MaterialNodeView.SaveToCache( IncludeInQuickLaunch: false );

                Ret["ActionId"] = NbtResources.Actions[CswEnumNbtActionName.Create_Material].ActionId.ToString();
                //Used for Tab and Button items
                Ret["NodeId"] = MaterialNode.NodeId.ToString();
                Ret["NodeViewId"] = MaterialNodeView.SessionViewId.ToString();
                //Used for node-specific Add items
                Ret["RelatedNodeId"] = MaterialNode.NodeId.ToString();
                Ret["RelatedNodeName"] = MaterialNode.NodeName;
                //If (and when) action landing pages are slated to be roleId-specific, remove this line
                Ret["isConfigurable"] = NbtResources.CurrentNbtUser.IsAdministrator();
                //Used for viewing new material
                Ret["ActionLinks"] = new JObject();
                string ActionLinkName = MaterialNode.NodeId.ToString();
                Ret["ActionLinks"][ActionLinkName] = new JObject();
                Ret["ActionLinks"][ActionLinkName]["Text"] = MaterialNode.NodeName;
                Ret["ActionLinks"][ActionLinkName]["ViewId"] = MaterialNodeView.SessionViewId.ToString();
            }
            return Ret;
        }

        #endregion LandingPage Logic

    }
}