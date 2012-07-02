using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassSize : CswNbtObjClass
    {
        public const string MaterialPropertyName = "Material";
        public const string CapacityPropertyName = "Capacity";
        public const string QuantityEditablePropertyName = "Quantity Editable";
        public const string DispensablePropertyName = "Dispensable";
        public const string CatalogNoPropertyName = "Catalog No";

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassSize( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassSize
        /// </summary>
        public static implicit operator CswNbtObjClassSize( CswNbtNode Node )
        {
            CswNbtObjClassSize ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass ) )
            {
                ret = (CswNbtObjClassSize) Node.ObjClass;
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
            //case 25759 - set capacity unittype view based on related material physical state
            CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( Material.RelatedNodeId );
            if( MaterialNode != null )
            {
                CswNbtObjClassMaterial MaterialNodeAsMaterial = MaterialNode;
                if( false == string.IsNullOrEmpty( MaterialNodeAsMaterial.PhysicalState.Value ) )
                {
                    Capacity.View = _getQuantityUnitOfMeasureView( MaterialNodeAsMaterial.PhysicalState.Value.ToLower() );
                }
            }

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

        #region Private Helper Functions

        private CswNbtView _getQuantityUnitOfMeasureView( string MaterialPhysicalState )
        {
            CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass );

            CswNbtView StateSpecificUnitTypeView = new CswNbtView( _CswNbtResources );
            StateSpecificUnitTypeView.ViewName = "CswNbtNodeTypePropQuantity_" + this.NodeId.ToString();

            foreach( CswNbtMetaDataNodeType UnitOfMeasureNodeType in UnitOfMeasureOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UnitTypeProp = UnitOfMeasureNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassUnitOfMeasure.UnitTypePropertyName );
                CswNbtObjClassUnitOfMeasure.UnitTypes UnitType = (CswNbtObjClassUnitOfMeasure.UnitTypes) UnitTypeProp.DefaultValue.AsList.Value;
                if( _physicalStateMatchesUnitType( MaterialPhysicalState, UnitType ) )
                {
                    StateSpecificUnitTypeView.AddViewRelationship( UnitOfMeasureNodeType, true );
                }
            }

            return StateSpecificUnitTypeView;
        }

        private bool _physicalStateMatchesUnitType( string PhysicalState, CswNbtObjClassUnitOfMeasure.UnitTypes UnitType )
        {
            bool matchFound = false;

            switch( PhysicalState )
            {
                case "n/a":
                    matchFound = UnitType == CswNbtObjClassUnitOfMeasure.UnitTypes.Each;
                    break;
                case "solid":
                    matchFound = UnitType == CswNbtObjClassUnitOfMeasure.UnitTypes.Weight;
                    break;
                case "liquid":
                case "gas":
                    matchFound = UnitType == CswNbtObjClassUnitOfMeasure.UnitTypes.Weight ||
                                    UnitType == CswNbtObjClassUnitOfMeasure.UnitTypes.Volume;
                    break;
                default:
                    matchFound = false;
                    break;
            }

            return matchFound;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Material { get { return _CswNbtNode.Properties[MaterialPropertyName]; } }
        public CswNbtNodePropQuantity Capacity { get { return _CswNbtNode.Properties[CapacityPropertyName]; } }
        public CswNbtNodePropLogical QuantityEditable { get { return _CswNbtNode.Properties[QuantityEditablePropertyName]; } }
        public CswNbtNodePropLogical Dispensable { get { return _CswNbtNode.Properties[DispensablePropertyName]; } }
        public CswNbtNodePropText CatalogNo { get { return _CswNbtNode.Properties[DispensablePropertyName]; } }

        #endregion


    }//CswNbtObjClassSize

}//namespace ChemSW.Nbt.ObjClasses
