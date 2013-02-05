using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassFireClassExemptAmount : CswNbtObjClass
    {
        #region Properties and ctor

        public sealed class PropertyName
        {
            public const string SetName = "Set Name";
            public const string HazardCategory = "Hazard Category";
            public const string Class = "Class";
            public const string HazardClass = "Hazard Class";
            public const string HazardType = "Hazard Type";
            public const string CategoryFootnotes = "Category Footnotes";
            public const string SortOrder = "Sort Order";
            public const string StorageSolidExemptAmount = "Storage Solid Exempt Amount";
            public const string StorageSolidExemptFootnotes = "Storage Solid Exempt Footnotes";
            public const string StorageLiquidExemptAmount = "Storage Liquid Exempt Amount";
            public const string StorageLiquidExemptFootnotes = "Storage Liquid Exempt Footnotes";
            public const string StorageGasExemptAmount = "Storage Gas Exempt Amount";
            public const string StorageGasExemptFootnotes = "Storage Gas Exempt Footnotes";
            public const string ClosedSolidExemptAmount = "Closed Solid Exempt Amount";
            public const string ClosedSolidExemptFootnotes = "Closed Solid Exempt Footnotes";
            public const string ClosedLiquidExemptAmount = "Closed Liquid Exempt Amount";
            public const string ClosedLiquidExemptFootnotes = "Closed Liquid Exempt Footnotes";
            public const string ClosedGasExemptAmount = "Closed Gas Exempt Amount";
            public const string ClosedGasExemptFootnotes = "Closed Gas Exempt Footnotes";
            public const string OpenSolidExemptAmount = "Open Solid Exempt Amount";
            public const string OpenSolidExemptFootnotes = "Open Solid Exempt Footnotes";
            public const string OpenLiquidExemptAmount = "Open Liquid Exempt Amount";
            public const string OpenLiquidExemptFootnotes = "Open Liquid Exempt Footnotes";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault;

        public CswNbtObjClassFireClassExemptAmount( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.FireClassExemptAmountClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassFireClassExemptAmount
        /// </summary>
        public static implicit operator CswNbtObjClassFireClassExemptAmount( CswNbtNode Node )
        {
            CswNbtObjClassFireClassExemptAmount ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.FireClassExemptAmountClass ) )
            {
                ret = (CswNbtObjClassFireClassExemptAmount) Node.ObjClass;
            }
            return ret;
        }

        #endregion Properties and ctor

        #region Inherited Events

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

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship SetName { get { return _CswNbtNode.Properties[PropertyName.SetName]; } }
        public CswNbtNodePropText HazardCategory { get { return _CswNbtNode.Properties[PropertyName.HazardCategory]; } }
        public CswNbtNodePropText Class { get { return _CswNbtNode.Properties[PropertyName.Class]; } }
        public CswNbtNodePropList HazardClass { get { return _CswNbtNode.Properties[PropertyName.HazardClass]; } }
        public CswNbtNodePropList HazardType { get { return _CswNbtNode.Properties[PropertyName.HazardType]; } }
        public CswNbtNodePropText CategoryFootnotes { get { return _CswNbtNode.Properties[PropertyName.CategoryFootnotes]; } }
        public CswNbtNodePropNumber SortOrder { get { return _CswNbtNode.Properties[PropertyName.SortOrder]; } }
        public CswNbtNodePropText StorageSolidExemptAmount { get { return _CswNbtNode.Properties[PropertyName.StorageSolidExemptAmount]; } }
        public CswNbtNodePropText StorageSolidExemptFootnotes { get { return _CswNbtNode.Properties[PropertyName.StorageSolidExemptFootnotes]; } }
        public CswNbtNodePropText StorageLiquidExemptAmount { get { return _CswNbtNode.Properties[PropertyName.StorageLiquidExemptAmount]; } }
        public CswNbtNodePropText StorageLiquidExemptFootnotes { get { return _CswNbtNode.Properties[PropertyName.StorageLiquidExemptFootnotes]; } }
        public CswNbtNodePropText StorageGasExemptAmount { get { return _CswNbtNode.Properties[PropertyName.StorageGasExemptAmount]; } }
        public CswNbtNodePropText StorageGasExemptFootnotes { get { return _CswNbtNode.Properties[PropertyName.StorageGasExemptFootnotes]; } }
        public CswNbtNodePropText ClosedSolidExemptAmount { get { return _CswNbtNode.Properties[PropertyName.ClosedSolidExemptAmount]; } }
        public CswNbtNodePropText ClosedSolidExemptFootnotes { get { return _CswNbtNode.Properties[PropertyName.ClosedSolidExemptFootnotes]; } }
        public CswNbtNodePropText ClosedLiquidExemptAmount { get { return _CswNbtNode.Properties[PropertyName.ClosedLiquidExemptAmount]; } }
        public CswNbtNodePropText ClosedLiquidExemptFootnotes { get { return _CswNbtNode.Properties[PropertyName.ClosedLiquidExemptFootnotes]; } }
        public CswNbtNodePropText ClosedGasExemptAmount { get { return _CswNbtNode.Properties[PropertyName.ClosedGasExemptAmount]; } }
        public CswNbtNodePropText ClosedGasExemptFootnotes { get { return _CswNbtNode.Properties[PropertyName.ClosedGasExemptFootnotes]; } }
        public CswNbtNodePropText OpenSolidExemptAmount { get { return _CswNbtNode.Properties[PropertyName.OpenSolidExemptAmount]; } }
        public CswNbtNodePropText OpenSolidExemptFootnotes { get { return _CswNbtNode.Properties[PropertyName.OpenSolidExemptFootnotes]; } }
        public CswNbtNodePropText OpenLiquidExemptAmount { get { return _CswNbtNode.Properties[PropertyName.OpenLiquidExemptAmount]; } }
        public CswNbtNodePropText OpenLiquidExemptFootnotes { get { return _CswNbtNode.Properties[PropertyName.OpenLiquidExemptFootnotes]; } }

        #endregion

    }//CswNbtObjClassFireClassExemptAmount

}//namespace ChemSW.Nbt.ObjClasses
