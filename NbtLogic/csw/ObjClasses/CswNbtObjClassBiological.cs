using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassBiological: CswNbtPropertySetMaterial
    {
        #region Base

        /// <summary>
        /// Ctor
        /// </summary>
        public CswNbtObjClassBiological( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        /// <summary>
        /// Implicit cast of Node to Object Class
        /// </summary>
        public static implicit operator CswNbtObjClassBiological( CswNbtNode Node )
        {
            CswNbtObjClassBiological ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.BiologicalClass ) )
            {
                ret = (CswNbtObjClassBiological) Node.ObjClass;
            }
            return ret;
        }

        /// <summary>
        /// Object Class
        /// </summary>
        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.BiologicalClass ); }
        }

        /// <summary>
        /// Cast a Request Item PropertySet back to an Object Class
        /// </summary>
        public static CswNbtObjClassBiological fromPropertySet( CswNbtPropertySetMaterial PropertySet )
        {
            return PropertySet.Node;
        }

        /// <summary>
        /// Cast a the Object Class as a PropertySet
        /// </summary>
        public static CswNbtPropertySetMaterial toPropertySet( CswNbtObjClassBiological ObjClass )
        {
            return ObjClass;
        }

        #endregion Base

        #region Enums

        public new sealed class PropertyName: CswNbtPropertySetMaterial.PropertyName
        {
            public const string Picture = "Picture";
            public const string ReferenceType = "Reference Type";
            public const string ReferenceNumber = "Reference Number";
            public const string Type = "Type";
            public const string SpeciesOrigin = "Species Origin";
            public const string BiosafetyLevel = "Biosafety Level";
            public const string Vectors = "Vectors";
            public const string StorageCondition = "Storage Conditions";
            public const string PhysicalState = "Physical State";
        }

        #endregion Enums

        #region Inherited Events

        public override void onReceiveButtonClick( NbtButtonData ButtonData )
        {
            ButtonData.Data["state"]["canAddSDS"] = false;
        }


        public override void onUpdatePropertyValue() { }
        #endregion Inherited Events

        #region ObjectClass-specific properties

        public CswNbtNodePropGrid Picture { get { return ( _CswNbtNode.Properties[PropertyName.Picture] ); } }
        public CswNbtNodePropText SpeciesOrigin { get { return _CswNbtNode.Properties[PropertyName.SpeciesOrigin]; } }
        public CswNbtNodePropText ReferenceNumber { get { return _CswNbtNode.Properties[PropertyName.ReferenceNumber]; } }
        public CswNbtNodePropList Type { get { return _CswNbtNode.Properties[PropertyName.Type]; } }
        public CswNbtNodePropList ReferenceType { get { return _CswNbtNode.Properties[PropertyName.ReferenceType]; } }
        public CswNbtNodePropList BiosafetyLevel { get { return _CswNbtNode.Properties[PropertyName.BiosafetyLevel]; } }
        public CswNbtNodePropList StorageConditions { get { return _CswNbtNode.Properties[PropertyName.StorageCondition];  } }
        public CswNbtNodePropMultiList Vectors { get { return _CswNbtNode.Properties[PropertyName.Vectors]; } }
        public CswNbtNodePropList PhysicalState { get { return _CswNbtNode.Properties[PropertyName.PhysicalState]; } }
        #endregion ObjectClass-specific properties

    }//CswNbtObjClassNonChemical

}//namespace ChemSW.Nbt.ObjClasses