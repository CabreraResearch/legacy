using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
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
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass ); }
        }


        public const string SupplierPropertyName = "Supplier";
        public const string ApprovalStatusPropertyName = "Approval Status";
        public const string PartNumberPropertyName = "Part Number";
        public const string SpecificGravityPropertyName = "Specific Gravity";
        public const string PhysicalStatePropertyName = "Physical State";
        public const string CasNoPropertyName = "CAS No";
        public const string RegulatoryListsPropertyName = "Regulatory Lists";
        public const string TradenamePropertyName = "Tradename";
        public const string StorageCompatibilityPropertyName = "Storage Compatibility";
        public const string ExpirationIntervalPropertyName = "Expiration Interval";
        public const string RequestPropertyName = "Request";
        public const string ReceivePropertyName = "Receive";

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMaterial
        /// </summary>
        public static implicit operator CswNbtObjClassMaterial( CswNbtNode Node )
        {
            CswNbtObjClassMaterial ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass ) )
            {
                ret = (CswNbtObjClassMaterial) Node.ObjClass;
            }
            return ret;
        }
        
        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( ApprovalStatus.WasModified )
            {
                Receive.Hidden = ApprovalStatus.Checked != Tristate.True;
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
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            CswNbtMetaDataObjectClassProp OCP = NodeTypeProp.getObjectClassProp();
            if( null != NodeTypeProp && null != OCP )
            {
                JObject ActionDataObj = new JObject();
                switch( OCP.PropName )
                {
                    case RequestPropertyName:
                        CswNbtActSubmitRequest RequestAct = new CswNbtActSubmitRequest( _CswNbtResources, CswNbtActSystemViews.SystemViewName.CISProRequestCart );

                        CswNbtObjClassRequestItem NodeAsRequestItem = RequestAct.makeRequestItem( new CswNbtActSubmitRequest.RequestItem( CswNbtActSubmitRequest.RequestItem.Material ), NodeId, OCP );
                        ActionDataObj["requestaction"] = OCP.PropName;
                        ActionDataObj["titleText"] = "Request for " + TradeName.Text;
                        ActionDataObj["requestItemProps"] = RequestAct.getRequestItemAddProps( NodeAsRequestItem );
                        ActionDataObj["requestItemNodeTypeId"] = RequestAct.RequestItemNt.NodeTypeId;
                        ButtonAction = NbtButtonAction.request;
                        break;
                    case ReceivePropertyName:
                        ActionDataObj["materialId"] = NodeId.ToString();
                        ActionDataObj["tradeName"] = TradeName.Text;
                        CswNbtView SizeView = new CswNbtView( _CswNbtResources );
                        SizeView.Visibility = NbtViewVisibility.Property;
                        SizeView.ViewMode = NbtViewRenderingMode.Grid;

                        CswNbtViewRelationship MaterialRel = SizeView.AddViewRelationship( ObjectClass, true );
                        CswNbtMetaDataObjectClass SizeOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
                        CswNbtMetaDataObjectClassProp CapacityOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.CapacityPropertyName );
                        CswNbtMetaDataObjectClassProp MaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.MaterialPropertyName );
                        CswNbtMetaDataObjectClassProp CatalogNoOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.CatalogNoPropertyName );
                        CswNbtMetaDataObjectClassProp DispensableOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.DispensablePropertyName );

                        CswNbtViewRelationship SizeRel = SizeView.AddViewRelationship( MaterialRel, NbtViewPropOwnerType.Second, MaterialOcp, true );
                        SizeView.AddViewProperty( SizeRel, CapacityOcp );
                        CswNbtViewProperty DispensableVp = SizeView.AddViewProperty( SizeRel, DispensableOcp );
                        DispensableVp.ShowInGrid = false;
                        SizeView.AddViewPropertyFilter( DispensableVp, DispensableOcp.getFieldTypeRule().SubFields.Default.Name, Value: "true" );
                        SizeView.AddViewProperty( SizeRel, CatalogNoOcp );
                        SizeView.SaveToCache( false );
                        ActionDataObj["sizesViewId"] = SizeView.SessionViewId.ToString();
                        ButtonAction = NbtButtonAction.receive;
                        break;
                }
                ActionData = ActionDataObj.ToString();
            }

            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Supplier { get { return ( _CswNbtNode.Properties[SupplierPropertyName] ); } }
        public CswNbtNodePropLogical ApprovalStatus { get { return ( _CswNbtNode.Properties[ApprovalStatusPropertyName] ); } }
        public CswNbtNodePropText PartNumber { get { return ( _CswNbtNode.Properties[PartNumberPropertyName] ); } }
        public CswNbtNodePropScientific SpecificGravity { get { return ( _CswNbtNode.Properties[SpecificGravityPropertyName] ); } }
        public CswNbtNodePropList PhysicalState { get { return ( _CswNbtNode.Properties[PhysicalStatePropertyName] ); } }
        public CswNbtNodePropText CasNo { get { return ( _CswNbtNode.Properties[CasNoPropertyName] ); } }
        public CswNbtNodePropStatic RegulatoryLists { get { return ( _CswNbtNode.Properties[RegulatoryListsPropertyName] ); } }
        public CswNbtNodePropText TradeName { get { return ( _CswNbtNode.Properties[TradenamePropertyName] ); } }
        public CswNbtNodePropImageList StorageCompatibility { get { return ( _CswNbtNode.Properties[StorageCompatibilityPropertyName] ); } }
        public CswNbtNodePropQuantity ExpirationInterval { get { return ( _CswNbtNode.Properties[ExpirationIntervalPropertyName] ); } }
        public CswNbtNodePropButton Request { get { return ( _CswNbtNode.Properties[RequestPropertyName] ); } }
        public CswNbtNodePropButton Receive { get { return ( _CswNbtNode.Properties[ReceivePropertyName] ); } }
 
        #endregion

    }//CswNbtObjClassMaterial

}//namespace ChemSW.Nbt.ObjClasses
