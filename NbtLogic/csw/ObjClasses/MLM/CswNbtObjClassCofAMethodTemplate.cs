using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassCofAMethodTemplate : CswNbtObjClass
    {
        /// <summary>
        /// Object Class property names
        /// </summary>
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Material = "Material";
            public const string CertMethodId = "C of A Method ID";
            public const string Description = "Description";
            public const string MethodNo = "Method No";
            public const string Conditions = "Conditions";
            public const string Lower = "Lower";
            public const string Upper = "Upper";
            public const string Units = "Units";
            public const string Qualified = "Qualified";
            public const string CertDefConditionSet = "CertDef Condition Set";
            public const string Obsolete = "Obsolete";
        }

        public CswNbtObjClassCofAMethodTemplate( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.CofAMethodTemplateClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassGeneric
        /// </summary>
        public static implicit operator CswNbtObjClassCofAMethodTemplate( CswNbtNode Node )
        {
            CswNbtObjClassCofAMethodTemplate ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.CofAMethodTemplateClass ) )
            {
                ret = (CswNbtObjClassCofAMethodTemplate) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        //Extend CswNbtObjClass events here
    
        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Material { get { return ( _CswNbtNode.Properties[PropertyName.Material] ); } }
        //TODO: Implement this property when the PropRefSequence FieldType is implemented
        //public CswNbtNodePropPropRefSequence Material { get { return ( _CswNbtNode.Properties[PropertyName.Material] ); } }
        public CswNbtNodePropText Description { get { return ( _CswNbtNode.Properties[PropertyName.Description] ); } }
        public CswNbtNodePropText MethodNo { get { return ( _CswNbtNode.Properties[PropertyName.MethodNo] ); } }
        public CswNbtNodePropText Conditions { get { return ( _CswNbtNode.Properties[PropertyName.Conditions] ); } }
        public CswNbtNodePropNumber Lower { get { return ( _CswNbtNode.Properties[PropertyName.Lower] ); } }
        public CswNbtNodePropNumber Upper { get { return ( _CswNbtNode.Properties[PropertyName.Upper] ); } }
        public CswNbtNodePropText Units { get { return ( _CswNbtNode.Properties[PropertyName.Units] ); } }
        public CswNbtNodePropLogical Qualified { get { return ( _CswNbtNode.Properties[PropertyName.Qualified] ); } }
        public CswNbtNodePropRelationship CertDefContionSet { get { return ( _CswNbtNode.Properties[PropertyName.CertDefConditionSet] ); } }
        public CswNbtNodePropLogical Obsolete { get { return ( _CswNbtNode.Properties[PropertyName.Obsolete] ); } }

        #endregion


    }//CswNbtObjClassGeneric

}//namespace ChemSW.Nbt.ObjClasses
