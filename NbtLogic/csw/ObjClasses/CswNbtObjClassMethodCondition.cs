using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    class CswNbtObjClassMethodCondition : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string Method = "Method";
            public const string Name = "Name";
            public const string Units = "Units";
        }

        public CswNbtObjClassMethodCondition( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodConditionClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMethodCondition
        /// </summary>
        public static implicit operator CswNbtObjClassMethodCondition( CswNbtNode Node )
        {
            CswNbtObjClassMethodCondition ret = null;
            if( null !=Node && _Validate(Node, CswEnumNbtObjectClass.MethodConditionClass ) )
            {
                ret = (CswNbtObjClassMethodCondition)Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events
        
        protected override void  afterPopulateProps()
        {
           base.afterPopulateProps();
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Method
        {
            get { return _CswNbtNode.Properties[PropertyName.Method]; }
        }
        public CswNbtNodePropList Name
        {
            get { return _CswNbtNode.Properties[PropertyName.Name]; }
        }
        public CswNbtNodePropList Units
        {
            get { return _CswNbtNode.Properties[PropertyName.Units]; }
        }

        #endregion

    }//CswNbtObjClassMethod

}//namespace ChemSW.Nbt.ObjClasses