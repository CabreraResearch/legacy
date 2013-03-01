using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            private CswNbtObjClassMaterial _ExistingNode;
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
                if( Node.ObjClass.ObjectClass.ObjectClass != NbtObjectClass.MaterialClass )
                {
                    throw new CswDniException( ErrorType.Warning, "Cannot create a new Material object without a valid Material Type.", "Could not resolve NodeType for NodeTypeId: " + NodeTypeId + "." );
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
                        throw new CswDniException( ErrorType.Warning, "Cannot create a new Material object without a valid Material Type.", "Could not resolve NodeType for NodeTypeId: " + NodeTypeId + "." );
                    }
                    if( PotentialNt.getObjectClass().ObjectClass != NbtObjectClass.MaterialClass )
                    {
                        throw new CswDniException( ErrorType.Warning, "Cannot create a new Material object without a valid Material Type.", "Cannot make a Material for Object Class: " + PotentialNt.getObjectClass().ObjectClass + "." );
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
                        throw new CswDniException( ErrorType.Warning, "A Tradename is required to create a new Material.", "Provided Tradename was null or empty." );
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
                    if( null == PotentialSupplier || PotentialSupplier.ObjectClass.ObjectClass != NbtObjectClass.VendorClass )
                    {
                        throw new CswDniException( ErrorType.Warning, "Cannot create a new Material object without a valid Supplier.", "Provided SupplierId was invalid." );
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
                CswNbtObjClassMaterial ExistingMaterial = existingMaterial( ForceRecalc );
                return ( ExistingMaterial != null && false == ExistingMaterial.IsTemp );
            }

            public CswNbtObjClassMaterial existingMaterial( bool ForceRecalc = false )
            {
                if( ForceRecalc || null == _ExistingNode )
                {
                    // If a preexisting material was provided, Node will not be null
                    // because it was set in the constructor
                    //_ExistingNode = Node;
                    if( null == _ExistingNode )
                    {
                        _ExistingNode = CswNbtObjClassMaterial.getExistingMaterial( _NbtResources, NodeTypeId, SupplierId, TradeName, PartNo );
                    }
                }
                return _ExistingNode;
            }

            public CswNbtObjClassMaterial commit( bool RemoveTempStatus = false )
            {
                CswNbtObjClassMaterial Ret;
                if( null == Node ) //Don't commit twice
                {
                    if( existsInDb() )
                    {
                        throw new CswDniException( ErrorType.Warning, "A material with the same Type, Tradename, Supplier and PartNo already exists.", "A material with this configuration already exists. Name: " + _ExistingNode.NodeName + " , ID: " + _ExistingNode.NodeId + "." );
                    }
                    if( false == existsInDb() && Int32.MinValue != NodeTypeId )
                    {
                        Ret = _NbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp );
                        Node = Ret.Node;

                        bool InAddLayout = false;
                        foreach( CswNbtMetaDataNodeTypeProp ntp in _NbtResources.MetaData.NodeTypeLayout.getPropsInLayout( NodeTypeId, Int32.MinValue, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add ) )
                        {
                            if( Ret.PhysicalState.NodeTypePropId == ntp.PropId )
                            {
                                InAddLayout = true;
                            }
                        }
                        Ret.PhysicalState.Value = InAddLayout ? CswNbtObjClassMaterial.PhysicalStates.Solid : CswNbtObjClassMaterial.PhysicalStates.NA;
                    }
                    else
                    {
                        throw new CswDniException( ErrorType.Warning, "Cannot create a new Material object without a valid Supplier.", "Provided SupplierId was invalid." );
                    }
                }
                else
                {
                    Ret = Node;
                }

                Ret.TradeName.Text = TradeName;
                Ret.PartNumber.Text = PartNo;
                Ret.Supplier.RelatedNodeId = SupplierId;

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

            if( false == _CswNbtResources.Permit.can( CswNbtActionName.Create_Material, _CswNbtResources.CurrentNbtUser ) )
            {
                throw new CswDniException( ErrorType.Error, "You do not have permission to use the Create Material wizard.", "Attempted to access the Create Material wizard with role of " + _CswNbtResources.CurrentNbtUser.Rolename );
            }
        }

        #endregion ctor

        private JObject _tryCreateMaterial( Int32 MaterialNodeTypeId, CswPrimaryKey SupplierId, string TradeName, string PartNo, string NodeId )
        {
            JObject Ret = new JObject();

            NewMaterial PotentialMaterial = new NewMaterial( _CswNbtResources, MaterialNodeTypeId, TradeName, SupplierId, PartNo, NodeId );

            Ret["materialexists"] = PotentialMaterial.existsInDb();
            if( false == PotentialMaterial.existsInDb() )
            {
                CswNbtObjClassMaterial NodeAsMaterial = PotentialMaterial.Node;
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
                    Ret["nodetypeid"] = NodeAsMaterial.NodeTypeId;
                    _CswNbtResources.EditMode = NodeEditMode.Temp;
                    CswNbtSdTabsAndProps SdProps = new CswNbtSdTabsAndProps( _CswNbtResources );
                    Ret["properties"] = SdProps.getProps( NodeAsMaterial.Node, string.Empty, null, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                    Int32 DocumentNodeTypeId = CswNbtActReceiving.getSDSDocumentNodeTypeId( _CswNbtResources, NodeAsMaterial );
                    if( Int32.MinValue != DocumentNodeTypeId )
                    {
                        Ret["documenttypeid"] = DocumentNodeTypeId;
                    }
                    Ret["noderef"] = NodeAsMaterial.Node.NodeLink; //for the link
                }
            }
            else
            {
                CswNbtObjClassMaterial ExisitingMaterial = PotentialMaterial.existingMaterial();
                Ret["noderef"] = ExisitingMaterial.Node.NodeLink;
            }

            return Ret;
        }

        #region Public

        public JObject saveMaterial( Int32 NodeTypeId, string SupplierId, string Tradename, string PartNo, string NodeId )
        {
            JObject Ret = new JObject();

            CswPrimaryKey CurrentTempNodePk = CswConvert.ToPrimaryKey( NodeId );
            if( CswTools.IsPrimaryKey( CurrentTempNodePk ) )
            {
                CswNbtObjClassMaterial CurrentTempNode = _CswNbtResources.Nodes.GetNode( CurrentTempNodePk );
                Int32 CurrentNodeTypeId = CurrentTempNode.NodeTypeId;
                if( NodeTypeId != CurrentNodeTypeId )
                {
                    // Then we want to just forget about the first temp node created and create a new one with the new nodetype
                    Ret = _tryCreateMaterial( NodeTypeId, CswConvert.ToPrimaryKey( SupplierId ), Tradename, PartNo, null );
                }
                else
                {
                    // If the nodetype isn't different then we want to get the props and check if it exsits
                    if( string.IsNullOrEmpty( CurrentTempNode.PhysicalState.Value ) )
                    {
                        CurrentTempNode.PhysicalState.Value = CswNbtObjClassMaterial.PhysicalStates.Solid;
                    }
                    Ret = _tryCreateMaterial( NodeTypeId, CswConvert.ToPrimaryKey( SupplierId ), Tradename, PartNo, CurrentTempNodePk.ToString() );
                }
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
                // Default to the Chemical NodeType
                CswNbtMetaDataNodeType ChemicalNT = _CswNbtResources.MetaData.getNodeTypeFirstVersion( "Chemical" );
                if( null != ChemicalNT )
                {
                    CswNbtObjClassMaterial NewMaterialTempNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ChemicalNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp );
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

        /// <summary>
        /// Creates a new material, if one does not already exist, and returns the material nodeid
        /// </summary>
        public JObject createMaterial( Int32 NodeTypeId, string SupplierId, string Tradename, string PartNo, string NodeId )
        {
            return _tryCreateMaterial( NodeTypeId, CswConvert.ToPrimaryKey( SupplierId ), Tradename, PartNo, NodeId );
        }

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

            SizeNode = CswNbtResources.Nodes.makeNodeFromNodeTypeId( SizeNodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, true );
            CswPrimaryKey UnitIdPK = CswConvert.ToPrimaryKey( SizeObj["unitid"].ToString() );
            if( null != UnitIdPK )
            {
                CswNbtObjClassSize NodeAsSize = (CswNbtObjClassSize) SizeNode;
                NodeAsSize.InitialQuantity.Quantity = CswConvert.ToDouble( SizeObj["quantity"] );
                NodeAsSize.InitialQuantity.UnitId = UnitIdPK;
                NodeAsSize.CatalogNo.Text = SizeObj["catalogNo"].ToString();
                NodeAsSize.QuantityEditable.Checked = CswConvert.ToTristate( SizeObj["quantEditableChecked"] );
                NodeAsSize.Dispensable.Checked = CswConvert.ToTristate( SizeObj["dispensibleChecked"] );
                NodeAsSize.UnitCount.Value = CswConvert.ToDouble( SizeObj["unitCount"] );

                if( Tristate.False == NodeAsSize.QuantityEditable.Checked && false == CswTools.IsDouble( NodeAsSize.InitialQuantity.Quantity ) )
                {
                    SizeNode = null; //Case 27665 - instead of throwing a serverside warning, just throw out the size
                }
                else if( WriteNode )
                {
                    SizeNode.postChanges( true );
                }
            }
            else
            {
                SizeNode = null;
            }

            return Ret;
        }

        public ICswNbtTree getMaterialSizes( CswNbtResources CswNbtResources, CswPrimaryKey MaterialId )
        {
            ICswNbtTree SizesTree = null;
            if( null != MaterialId )
            {
                CswPrimaryKey pk = MaterialId;
                if( CswTools.IsPrimaryKey( pk ) )
                {
                    CswNbtMetaDataObjectClass sizeOC = CswNbtResources.MetaData.getObjectClass( NbtObjectClass.SizeClass );
                    CswNbtMetaDataObjectClassProp materialOCP = sizeOC.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );

                    CswNbtView sizesView = new CswNbtView( CswNbtResources );
                    CswNbtViewRelationship parent = sizesView.AddViewRelationship( sizeOC, true );
                    sizesView.AddViewPropertyAndFilter( parent,
                                                       MetaDataProp: materialOCP,
                                                       Value: pk.PrimaryKey.ToString(),
                                                       SubFieldName: CswNbtSubField.SubFieldName.NodeID,
                                                       FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

                    SizesTree = CswNbtResources.Trees.getTreeFromView( sizesView, true, false, false );

                }
            }

            return SizesTree;
        }

        private CswNbtNode _commitMaterialNode( JObject MaterialObj )
        {
            CswNbtNode Ret = null;

            JObject MaterialProperties = (JObject) MaterialObj["properties"];
            CswNbtSdTabsAndProps SdTabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );

            Int32 MaterialNodeTypeId = CswConvert.ToInt32( MaterialObj["materialnodetypeid"] );
            if( Int32.MinValue != MaterialNodeTypeId )
            {
                CswNbtMetaDataNodeType MaterialNt = _CswNbtResources.MetaData.getNodeType( MaterialNodeTypeId );
                if( null != MaterialNt )
                {
                    _CswNbtResources.EditMode = NodeEditMode.Edit;
                    Ret = _CswNbtResources.Nodes[CswConvert.ToString( MaterialObj["materialId"] )];
                    if( null != Ret )
                    {
                        Ret.IsTemp = false;
                        SdTabsAndProps.saveProps( Ret.NodeId, Int32.MinValue, MaterialProperties, Ret.NodeTypeId, null, IsIdentityTab: false );

                        NewMaterial FinalMaterial = new NewMaterial( _CswNbtResources, Ret );
                        FinalMaterial.TradeName = CswConvert.ToString( MaterialObj["tradename"] );
                        FinalMaterial.SupplierId = CswConvert.ToPrimaryKey( CswConvert.ToString( MaterialObj["supplierid"] ) );
                        FinalMaterial.PartNo = CswConvert.ToString( MaterialObj["partno"] );

                        CswNbtObjClassMaterial NodeAsMaterial = FinalMaterial.commit( RemoveTempStatus: true );

                        JObject RequestObj = CswConvert.ToJObject( MaterialObj["request"] );
                        if( RequestObj.HasValues )
                        {
                            CswNbtObjClassRequestMaterialCreate RequestCreate = _CswNbtResources.Nodes[CswConvert.ToString( RequestObj["requestitemid"] )];
                            if( null != RequestCreate )
                            {
                                RequestCreate.Material.RelatedNodeId = FinalMaterial.Node.NodeId;
                                RequestCreate.Status.Value = CswNbtObjClassRequestMaterialCreate.Statuses.Created;
                                RequestCreate.Fulfill.State = CswNbtObjClassRequestMaterialCreate.FulfillMenu.Complete;
                                RequestCreate.Fulfill.MenuOptions = CswNbtObjClassRequestMaterialCreate.FulfillMenu.Complete;
                                RequestCreate.postChanges( ForceUpdate: false );
                            }
                        }
                        CswNbtActReceiving.commitDocumentNode( _CswNbtResources, NodeAsMaterial, MaterialObj );
                    }
                }

                if( null == Ret )
                {
                    throw new CswDniException( ErrorType.Error,
                                               "Failed to create new material.",
                                               "Attempted to call _commitMaterialNode failed." );
                }
            }
            return Ret;
        }

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
                        /* 1. Validate the new material and get its properties and sizes */
                        MaterialNode = _commitMaterialNode( MaterialObj );
                        RetObj["createdmaterial"] = true;

                        /* 2. Add the sizes */
                        SizesArray = _removeDuplicateSizes( SizesArray );
                        _addMaterialSizes( SizesArray, MaterialNode );
                        RetObj["sizescount"] = SizesArray.Count;

                        /* 3. Add landingpage data */
                        RetObj["landingpagedata"] = _getLandingPageData( MaterialNode );
                    }
                }
            }
            return RetObj;
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
                        if( UniqueSizeObj["unitid"].ToString() == SizeObj["unitid"].ToString() &&
                            UniqueSizeObj["quantity"].ToString() == SizeObj["quantity"].ToString() &&
                            UniqueSizeObj["catalogNo"].ToString() == SizeObj["catalogNo"].ToString() )
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
                    Int32 SizeNtId = CswConvert.ToInt32( SizeObj["nodetypeid"] );
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
                MaterialNodeView = MaterialNodeView ?? CswNbtObjClassMaterial.getMaterialNodeView( NbtResources, MaterialNode );
                MaterialNodeView.SaveToCache( IncludeInQuickLaunch: false );

                Ret["ActionId"] = NbtResources.Actions[CswNbtActionName.Create_Material].ActionId.ToString();
                //Used for Tab and Button items
                Ret["NodeId"] = MaterialNode.NodeId.ToString();
                Ret["NodeViewId"] = MaterialNodeView.SessionViewId.ToString();
                //Used for node-specific Add items
                Ret["RelatedNodeId"] = MaterialNode.NodeId.ToString();
                Ret["RelatedNodeName"] = MaterialNode.NodeName;
                Ret["RelatedNodeTypeId"] = MaterialNode.NodeTypeId.ToString();
                Ret["RelatedObjectClassId"] = MaterialNode.getObjectClassId().ToString();
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

        public static JObject getMaterialUnitsOfMeasure( string PhysicalStateValue, CswNbtResources CswNbtResources )
        {
            JObject ret = new JObject();
            string PhysicalState = string.Empty;
            foreach( string CurrentPhysicalState in CswNbtObjClassMaterial.PhysicalStates.Options )
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

        /// <summary>
        /// Get the view to drive the Supplier picklist in the Create Material wizard
        /// </summary>
        public CswNbtView getMaterialSuppliersView()
        {
            CswNbtView Ret = new CswNbtView( _CswNbtResources );
            CswNbtMetaDataObjectClass VendorOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.VendorClass );
            CswNbtViewRelationship SupplierVr = Ret.AddViewRelationship( VendorOc, IncludeDefaultFilters: true );

            //This matches the MLM module event logic, but it may need adjustment down the line
            if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.MLM ) )
            {
                CswNbtMetaDataObjectClassProp CoorporateOcp = VendorOc.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorTypeName );
                Ret.AddViewPropertyAndFilter( SupplierVr,
                                              MetaDataProp: CoorporateOcp,
                                              Value: CswNbtObjClassVendor.VendorTypes.Corporate,
                                              FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
            }

            Ret.ViewName = "Create Material Supplier";
            Ret.SaveToCache( IncludeInQuickLaunch: false );
            return Ret;
        }

        #endregion Public


    }


}
