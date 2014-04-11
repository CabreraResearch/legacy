using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.UnitsOfMeasure;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassSize : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string Material = "Material";
            public const string InitialQuantity = "Initial Quantity";
            public const string QuantityEditable = "Quantity Editable";
            public const string Dispensable = "Dispensable";
            public const string CatalogNo = "Catalog No";
            public const string UnitCount = "Unit Count";
            public const string ContainerType = "Container Type";
            public const string Supplier = "Supplier";
            public const string Description = "Description";
            public const string UPC = "UPC";
        }

        public CswNbtObjClassSize( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassSize
        /// </summary>
        public static implicit operator CswNbtObjClassSize( CswNbtNode Node )
        {
            CswNbtObjClassSize ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.SizeClass ) )
            {
                ret = (CswNbtObjClassSize) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        protected override void beforeWriteNodeLogic( bool Creating, bool OverrideUniqueValidation )
        {
            if( CswEnumTristate.False == this.QuantityEditable.Checked && false == CswTools.IsDouble( this.InitialQuantity.Quantity ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Cannot have a null Initial Quantity if Quantity Editable is unchecked.", "Cannot have a null Initial Quantity if Quantity Editable is unchecked." );
            }
        }//beforeWriteNode()     

        protected override void afterPopulateProps()
        {
            InitialQuantity.SetOnBeforeRender( Prop => _setUnits() );
        }//afterPopulateProps()

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Material { get { return _CswNbtNode.Properties[PropertyName.Material]; } }
        public CswNbtNodePropQuantity InitialQuantity { get { return _CswNbtNode.Properties[PropertyName.InitialQuantity]; } }
        public CswNbtNodePropLogical QuantityEditable { get { return _CswNbtNode.Properties[PropertyName.QuantityEditable]; } }
        public CswNbtNodePropLogical Dispensable { get { return _CswNbtNode.Properties[PropertyName.Dispensable]; } }
        public CswNbtNodePropText CatalogNo { get { return _CswNbtNode.Properties[PropertyName.CatalogNo]; } }
        public CswNbtNodePropNumber UnitCount { get { return _CswNbtNode.Properties[PropertyName.UnitCount]; } }
        public CswNbtNodePropList ContainerType { get { return _CswNbtNode.Properties[PropertyName.ContainerType]; } }
        public CswNbtNodePropPropertyReference Supplier { get { return _CswNbtNode.Properties[PropertyName.Supplier]; } }
        public CswNbtNodePropText Description { get { return _CswNbtNode.Properties[PropertyName.Description]; } }
        public CswNbtNodePropText UPC { get { return _CswNbtNode.Properties[PropertyName.UPC]; } }

        #endregion

        #region Custom logic

        //set available quantity units when rendering the property on the add/edit layout
        private void _setUnits()
        {
            CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( Material.RelatedNodeId );
            if( MaterialNode != null )
            {
                Material.setReadOnly( value: true, SaveToDb: true );
                CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
                Vb.setQuantityUnitOfMeasureView( MaterialNode, InitialQuantity );
            }
        }

        #endregion

    }//CswNbtObjClassSize

}//namespace ChemSW.Nbt.ObjClasses
