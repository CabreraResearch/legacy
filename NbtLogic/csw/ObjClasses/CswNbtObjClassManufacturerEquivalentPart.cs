using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassManufacturerEquivalentPart : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Material = "Material";
            public const string EnterprisePart = "Enterprise Part";
            public const string Manufacturer = "Manufacturer";
        }

        public CswNbtObjClassManufacturerEquivalentPart( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ManufacturerEquivalentPartClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassManufacturerEquivalentPart
        /// </summary>
        public static implicit operator CswNbtObjClassManufacturerEquivalentPart( CswNbtNode Node )
        {
            CswNbtObjClassManufacturerEquivalentPart ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.ManufacturerEquivalentPartClass ) )
            {
                ret = (CswNbtObjClassManufacturerEquivalentPart) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        //Extend CswNbtObjClass events here

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Material { get { return _CswNbtNode.Properties[PropertyName.Material]; } }
        public CswNbtNodePropRelationship EnterprisePart { get { return _CswNbtNode.Properties[PropertyName.EnterprisePart]; } }
        public CswNbtNodePropRelationship Manufacturer { get { return _CswNbtNode.Properties[PropertyName.Manufacturer]; } }

        #endregion

    }//CswNbtObjClassManufacturerEquivalentPart

}//namespace ChemSW.Nbt.ObjClasses
