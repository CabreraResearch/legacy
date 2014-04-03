using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    class CswNbtObjClassMethodCharacteristic : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string Characteristic = "Characteristic";
            public const string ConstituentMaterial = "Constituent Material";
            public const string Method = "Method";
            public const string Obsolete = "Obsolete";
            public const string Precision = "Precision";
            public const string ResultOptions = "Result Options";
            public const string ResultType = "Result Type";
            public const string ResultUnits = "Result Units";
        }

        public sealed class ResultTypeOption
        {
            public const string Quantitative = "Quantitative";
            public const string Qualitative = "Qualitative";
            public const string Match = "Match";
        }

        public CswNbtObjClassMethodCharacteristic( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodCharacteristicClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMethodCharacteristic
        /// </summary>
        public static implicit operator CswNbtObjClassMethodCharacteristic( CswNbtNode Node )
        {
            CswNbtObjClassMethodCharacteristic ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.MethodCharacteristicClass ) )
            {
                ret = (CswNbtObjClassMethodCharacteristic) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        #endregion

        #region Object class specific properties

        public CswNbtNodePropList Characteristic { get { return _CswNbtNode.Properties[PropertyName.Characteristic]; } }
        public CswNbtNodePropRelationship ConstituentMaterial { get { return _CswNbtNode.Properties[PropertyName.ConstituentMaterial]; } }
        public CswNbtNodePropRelationship Method { get { return _CswNbtNode.Properties[PropertyName.Method]; } }
        public CswNbtNodePropLogical Obsolete { get { return _CswNbtNode.Properties[PropertyName.Obsolete]; } }
        public CswNbtNodePropNumber Precision { get { return _CswNbtNode.Properties[PropertyName.Precision]; } }
        public CswNbtNodePropMemo ResultOptions { get { return _CswNbtNode.Properties[PropertyName.ResultOptions]; } }
        public CswNbtNodePropList ResultType { get { return _CswNbtNode.Properties[PropertyName.ResultType]; } }
        public CswNbtNodePropRelationship ResultUnits { get { return _CswNbtNode.Properties[PropertyName.ResultUnits]; } }

        #endregion

    }//CswNbtObjClassMethod

}//namespace ChemSW.Nbt.ObjClasses