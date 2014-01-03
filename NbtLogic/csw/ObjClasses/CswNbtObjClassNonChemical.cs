using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassNonChemical : CswNbtPropertySetMaterial
    {
        #region Base

        /// <summary>
        /// Ctor
        /// </summary>
        public CswNbtObjClassNonChemical( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        /// <summary>
        /// Implicit cast of Node to Object Class
        /// </summary>
        public static implicit operator CswNbtObjClassNonChemical( CswNbtNode Node )
        {
            CswNbtObjClassNonChemical ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.NonChemicalClass ) )
            {
                ret = (CswNbtObjClassNonChemical) Node.ObjClass;
            }
            return ret;
        }

        /// <summary>
        /// Object Class
        /// </summary>
        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.NonChemicalClass ); }
        }

        public override void beforePromoteNode()
        {
        }

        public override void afterPromoteNode()
        {
        }

        /// <summary>
        /// Cast a Request Item PropertySet back to an Object Class
        /// </summary>
        public static CswNbtObjClassNonChemical fromPropertySet( CswNbtPropertySetMaterial PropertySet )
        {
            return PropertySet.Node;
        }

        /// <summary>
        /// Cast a the Object Class as a PropertySet
        /// </summary>
        public static CswNbtPropertySetMaterial toPropertySet( CswNbtObjClassNonChemical ObjClass )
        {
            return ObjClass;
        }

        #endregion Base

        #region Enums

        public new sealed class PropertyName : CswNbtPropertySetMaterial.PropertyName
        {
            public const string Picture = "Picture";
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

        #endregion ObjectClass-specific properties

    }//CswNbtObjClassNonChemical

}//namespace ChemSW.Nbt.ObjClasses