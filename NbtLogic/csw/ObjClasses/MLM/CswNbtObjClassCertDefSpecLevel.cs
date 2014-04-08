using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassCertDefSpecLevel: CswNbtObjClass
    {
        /// <summary>
        /// Object Class property names
        /// </summary>
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Level = "Level";
            //public const string CertDefSpec = "Cert Def Spec"; //TODO - uncomment when CIS-52297 (slated for Mag.2) is done
            public const string AllowedDataSources = "Allowed Data Sources";
            public const string RequiredForApproval = "Required For Approval";
            public const string InitialSampleRegime = "Initial Sample Regime";
            public const string RetestSampleRegime = "Retest Sample Regime";
            public const string SampleSize = "Sample Size";
            public const string Frequency = "Frequency";
            public const string ApprovalPeriod = "Approval Period";
        }

        public CswNbtObjClassCertDefSpecLevel( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefSpecLevel ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassLevel
        /// </summary>
        public static implicit operator CswNbtObjClassCertDefSpecLevel( CswNbtNode Node )
        {
            CswNbtObjClassCertDefSpecLevel ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.CertDefSpecLevel ) )
            {
                ret = (CswNbtObjClassCertDefSpecLevel) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        protected override void afterPopulateProps()
        {
            RetestSampleRegime.SetOnBeforeRender( delegate( CswNbtNodeProp Prop )
                {
                    CswNbtObjClassLevel LevelNode = _CswNbtResources.Nodes.GetNode( Level.RelatedNodeId );
                    if( LevelNode.LabUseOnly.Checked == CswEnumTristate.True )
                    {
                        Prop.setHidden( true, false );
                    }
                } );
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Level { get { return _CswNbtNode.Properties[PropertyName.Level]; } }
        //public CswNbtNodePropRelationship CertDefSpec { get { return _CswNbtNode.Properties[PropertyName.CertDefSpec]; } }  //TODO - uncomment when CIS-52297 (slated for Mag.2) is done
        public CswNbtNodePropList AllowedDataSources { get { return _CswNbtNode.Properties[PropertyName.AllowedDataSources]; } }
        public CswNbtNodePropLogical RequiredForApproval { get { return _CswNbtNode.Properties[PropertyName.RequiredForApproval]; } }
        public CswNbtNodePropList InitialSampleRegime { get { return _CswNbtNode.Properties[PropertyName.InitialSampleRegime]; } }
        public CswNbtNodePropList RetestSampleRegime { get { return _CswNbtNode.Properties[PropertyName.RetestSampleRegime]; } }
        public CswNbtNodePropQuantity SampleSize { get { return _CswNbtNode.Properties[PropertyName.SampleSize]; } }
        public CswNbtNodePropList Frequency { get { return _CswNbtNode.Properties[PropertyName.Frequency]; } }
        public CswNbtNodePropQuantity ApprovalPeriod { get { return _CswNbtNode.Properties[PropertyName.ApprovalPeriod]; } }

        #endregion


    }//CswNbtObjClassCertDefSpecLevel

}//namespace ChemSW.Nbt.ObjClasses
