using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMaterial : CswNbtObjClass, ICswNbtPropertySetRequest
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


        public static string SupplierPropertyName { get { return "Supplier"; } }
        public static string ApprovalStatusPropertyName { get { return "Approval Status"; } }
        public static string PartNumberPropertyName { get { return "Part Number"; } }
        public static string SpecificGravityPropertyName { get { return "Specific Gravity"; } }
        public static string PhysicalStatePropertyName { get { return "Physical State"; } }
        public static string CasNoPropertyName { get { return "CAS No"; } }
        public static string RegulatoryListsPropName { get { return "Regulatory Lists"; } }
        public static string TradenamePropName { get { return "Tradename"; } }
        public static string StorageCompatibilityPropName { get { return "Storage Compatibility"; } }
        public static string ExpirationIntervalPropName { get { return "Expiration Interval"; } }
        public static string RequestPropertyName { get { return "Request"; } }

        public string RequestButtonPropertyName { get { return RequestPropertyName; } }

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
        public CswNbtNodePropStatic RegulatoryLists { get { return ( _CswNbtNode.Properties[RegulatoryListsPropName] ); } }
        public CswNbtNodePropText TradeName { get { return ( _CswNbtNode.Properties[TradenamePropName] ); } }
        public CswNbtNodePropImageList StorageCompatibility { get { return ( _CswNbtNode.Properties[StorageCompatibilityPropName] ); } }
        public CswNbtNodePropQuantity ExpirationInterval { get { return ( _CswNbtNode.Properties[ExpirationIntervalPropName] ); } }
        public CswNbtNodePropButton Request { get { return ( _CswNbtNode.Properties[RequestPropertyName] ); } }

        #endregion

    }//CswNbtObjClassMaterial

}//namespace ChemSW.Nbt.ObjClasses
