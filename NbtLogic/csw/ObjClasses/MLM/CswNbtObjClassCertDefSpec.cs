using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    class CswNbtObjClassCertDefSpec : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string NameForTestingConditions = "Name For Testing Conditions";
            public const string Method = "Method";
            public const string CertDef = "Cert Def";
            public const string Conditions = "Conditions";
            public const string Characteristics = "Characteristics";
        }

        public CswNbtObjClassCertDefSpec( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefSpecClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassCertDefSpec
        /// </summary>
        public static implicit operator CswNbtObjClassCertDefSpec( CswNbtNode Node )
        {
            CswNbtObjClassCertDefSpec ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.CertDefSpecClass ) )
            {
                ret = (CswNbtObjClassCertDefSpec) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText NameForTestingConditions
        {
            get { return _CswNbtNode.Properties[PropertyName.NameForTestingConditions]; }
        }
        public CswNbtNodePropRelationship Method
        {
            get { return _CswNbtNode.Properties[PropertyName.Method]; }
        }
        public CswNbtNodePropRelationship CertDef
        {
            get { return _CswNbtNode.Properties[PropertyName.CertDef]; }
        }
        public CswNbtNodePropGrid Conditions
        {
            get { return _CswNbtNode.Properties[PropertyName.Conditions]; }
        }
        public CswNbtNodePropGrid Characteristics
        {
            get { return _CswNbtNode.Properties[PropertyName.Characteristics]; }
        }

        #endregion

    }//CswNbtObjClassMethod

}//namespace ChemSW.Nbt.ObjClasses