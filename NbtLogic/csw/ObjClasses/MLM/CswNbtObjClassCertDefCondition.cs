using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    class CswNbtObjClassCertDefCondition : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string MethodCondition = "Method Condition";
            public const string CertDefSpec = "CertDef Spec";
            public const string Value = "Value";
        }

        public CswNbtObjClassCertDefCondition( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefConditionClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassCertDefCondition
        /// </summary>
        public static implicit operator CswNbtObjClassCertDefCondition( CswNbtNode Node )
        {
            CswNbtObjClassCertDefCondition ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.CertDefConditionClass ) )
            {
                ret = (CswNbtObjClassCertDefCondition) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship MethodCondition
        {
            get { return _CswNbtNode.Properties[PropertyName.MethodCondition]; }
        }
        public CswNbtNodePropRelationship CertDefSpec
        {
            get { return _CswNbtNode.Properties[PropertyName.CertDefSpec]; }
        }
        public CswNbtNodePropText Value
        {
            get { return _CswNbtNode.Properties[PropertyName.Value]; }
        }

        #endregion

    }//CswNbtObjClassMethod

}//namespace ChemSW.Nbt.ObjClasses