using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassCertDefCharacteristicLimit : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string CertDefSpec = "CertDef Spec";
            public const string Limits = "Limits";
            public const string MethodCharacteristic = "Method Characteristic";
            public const string PassOptions = "Pass Options";
            public const string PassValue = "Pass Value";
            public const string ResultType = "Result Type";
        }

        public CswNbtObjClassCertDefCharacteristicLimit( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefCharacteristicLimitClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassManufacturer
        /// </summary>
        public static implicit operator CswNbtObjClassCertDefCharacteristicLimit( CswNbtNode Node )
        {
            CswNbtObjClassCertDefCharacteristicLimit ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.CertDefCharacteristicLimitClass ) )
            {
                ret = (CswNbtObjClassCertDefCharacteristicLimit) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        protected override void afterPopulateProps()
        {
            // CIS-52299: "Limits" are required if certdef's level's labuseonly is false, hidden otherwise
            CswNbtObjClassCertDefSpec CertDefSpecNode = _CswNbtResources.Nodes[CertDefSpec.RelatedNodeId];
            if( null != CertDefSpecNode )
            {
                CswNbtObjClassLevel LevelNode = _CswNbtResources.Nodes[CertDefSpec.Level.RelatedNodeId];
                if( null != LevelNode )
                {
                    if( LevelNode.LabUseOnly.Checked == CswEnumTristate.True )
                    {
                        Limits.TemporarilyRequired = true;
                    }
                    else
                    {
                        Limits.setHidden( true, true );
                    }
                }
            }

            CswNbtObjClassMethodCharacteristic MethodCharNode = _CswNbtResources.Nodes[MethodCharacteristic.RelatedNodeId];
            if( null != MethodCharNode )
            {
                // CIS-52299: "Pass Options" list options come from Method Characteristic's "Result Options" Memo property
                PassOptions.InitOptions = delegate
                    {
                        Dictionary<string, string> ret = new Dictionary<string, string>();
                        CswDelimitedString opts = new CswDelimitedString( '\n' );
                        opts.FromString( MethodCharNode.ResultOptions.Text.Replace( ',', '\n' ), true, true );
                        foreach( string opt in opts )
                        {
                            ret.Add( opt, opt );

                        }
                        return ret;
                    };

                // CIS-52299: Round limits to precision defined on method characteristic
                Limits.Precision = CswConvert.ToInt32( MethodCharNode.Precision.Value );
            }
            base.afterPopulateProps();
        } // afterPopulateProps()

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship CertDefSpec { get { return _CswNbtNode.Properties[PropertyName.CertDefSpec]; } }
        public CswNbtNodePropNumericRange Limits { get { return _CswNbtNode.Properties[PropertyName.Limits]; } }
        public CswNbtNodePropRelationship MethodCharacteristic { get { return _CswNbtNode.Properties[PropertyName.MethodCharacteristic]; } }
        public CswNbtNodePropMultiList PassOptions { get { return _CswNbtNode.Properties[PropertyName.PassOptions]; } }
        public CswNbtNodePropText PassValue { get { return _CswNbtNode.Properties[PropertyName.PassValue]; } }
        public CswNbtNodePropPropertyReference ResultType { get { return _CswNbtNode.Properties[PropertyName.ResultType]; } }

        #endregion

    }//class CswNbtObjClassCertDefCharacteristicLimit

}//namespace ChemSW.Nbt.ObjClasses
