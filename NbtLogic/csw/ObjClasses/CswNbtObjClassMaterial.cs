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
                if( RequestPropertyName == OCP.PropName )
                {
                    CswNbtActSubmitRequest RequestAct = new CswNbtActSubmitRequest( _CswNbtResources, CswNbtActSystemViews.SystemViewName.CISProRequestCart );

                    CswNbtObjClassRequestItem NodeAsRequestItem = RequestAct.makeRequestItem( new CswNbtActSubmitRequest.RequestItem( CswNbtActSubmitRequest.RequestItem.Material ), NodeId, OCP );
                    JObject ActionDataObj = new JObject();
                    ActionDataObj["requestaction"] = OCP.PropName;
                    ActionDataObj["titleText"] = "Request for " + TradeName.Text;
                    ActionDataObj["requestItemProps"] = RequestAct.getRequestItemAddProps( NodeAsRequestItem );
                    ActionDataObj["requestItemNodeTypeId"] = RequestAct.RequestItemNt.NodeTypeId;
                    ActionData = ActionDataObj.ToString();

                    ButtonAction = NbtButtonAction.request;
                }
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

        #endregion

    }//CswNbtObjClassMaterial

}//namespace ChemSW.Nbt.ObjClasses
