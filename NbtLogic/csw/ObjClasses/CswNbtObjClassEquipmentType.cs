using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassEquipmentType : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Parts = "Parts";
            public const string TypeName = "Type Name";
        }

        public CswNbtObjClassEquipmentType( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentTypeClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassEquipmentType
        /// </summary>
        public static implicit operator CswNbtObjClassEquipmentType( CswNbtNode Node )
        {
            CswNbtObjClassEquipmentType ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.EquipmentTypeClass ) )
            {
                ret = (CswNbtObjClassEquipmentType) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        //Extend CswNbtObjClass events here

        #endregion

        #region Object class specific properties

        public CswNbtNodePropMemo Parts { get { return ( _CswNbtNode.Properties[PropertyName.Parts] ); } }
        public CswNbtNodePropText TypeName { get { return ( _CswNbtNode.Properties[PropertyName.TypeName] ); } }

        #endregion

    }//CswNbtObjClassEquipmentType

}//namespace ChemSW.Nbt.ObjClasses
