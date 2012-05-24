using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


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


        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMaterial
        /// </summary>
        public static explicit operator CswNbtObjClassMaterial( CswNbtNode Node )
        {
            CswNbtObjClassMaterial ret = null;
            if( _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass ) )
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

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

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
            if( null != NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties


        public CswNbtNodePropRelationship Supplier
        {
            get
            {
                return ( _CswNbtNode.Properties[SupplierPropertyName].AsRelationship );
            }
        }

        public CswNbtNodePropLogical ApprovalStatus
        {
            get
            {
                return ( _CswNbtNode.Properties[ApprovalStatusPropertyName].AsLogical );
            }
        }

        public CswNbtNodePropText PartNumber
        {
            get
            {
                return ( _CswNbtNode.Properties[PartNumberPropertyName].AsText );
            }
        }

        public CswNbtNodePropScientific SpecificGravity
        {
            get
            {
                return ( _CswNbtNode.Properties[SpecificGravityPropertyName].AsScientific );
            }
        }

        public CswNbtNodePropList PhysicalState
        {
            get
            {
                return ( _CswNbtNode.Properties[PhysicalStatePropertyName].AsList );
            }
        }

        public CswNbtNodePropText CasNo
        {
            get
            {
                return ( _CswNbtNode.Properties[CasNoPropertyName].AsText );
            }
        }

        public CswNbtNodePropStatic RegulatoryLists
        {
            get
            {
                return ( _CswNbtNode.Properties[RegulatoryListsPropName].AsStatic );
            }
        }

        public CswNbtNodePropText TradeName
        {
            get
            {
                return ( _CswNbtNode.Properties[TradenamePropName].AsText );
            }
        }

        public CswNbtNodePropImageList StorageCompatibility
        {
            get
            {
                return ( _CswNbtNode.Properties[StorageCompatibilityPropName].AsImageList );
            }
        }
        public CswNbtNodePropQuantity ExpirationInterval
        {
            get
            {
                return ( _CswNbtNode.Properties[ExpirationIntervalPropName].AsQuantity );
            }
        }

        #endregion

    }//CswNbtObjClassMaterial

}//namespace ChemSW.Nbt.ObjClasses
