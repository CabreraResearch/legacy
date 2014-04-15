using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52299E : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 52299; }
        }

        public override string Title
        {
            get { return "Configure CertDef Characteristic Limit nodetype"; }
        }

        public override string AppendToScriptName()
        {
            return "E";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MethodCharacteristicOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodCharacteristicClass );
            CswNbtMetaDataNodeType MethodCharacteristicNT = MethodCharacteristicOC.FirstNodeType;
            CswNbtMetaDataNodeTypeProp MethodCharResultTypeNTP = MethodCharacteristicNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMethodCharacteristic.PropertyName.ResultType );

            // CertDef Characteristic Limit NodeType
            CswNbtMetaDataObjectClass CertDefCharLimitOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefCharacteristicLimitClass );
            CswNbtMetaDataNodeType CertDefCharLimitNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( CertDefCharLimitOC )
                {
                    NodeTypeName = "CertDef Characteristic Limit",
                    NameTemplate = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassCertDefCharacteristicLimit.PropertyName.CertDefSpec ) + ": " +
                                   CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassCertDefCharacteristicLimit.PropertyName.MethodCharacteristic ),
                    Category = "MLM"
                } );

            CswNbtMetaDataNodeTypeProp MethodCharacteristicNTP = CertDefCharLimitNT.getNodeTypePropByObjectClassProp( CswNbtObjClassCertDefCharacteristicLimit.PropertyName.MethodCharacteristic );
            CswNbtMetaDataNodeTypeProp CertDefSpecNTP = CertDefCharLimitNT.getNodeTypePropByObjectClassProp( CswNbtObjClassCertDefCharacteristicLimit.PropertyName.CertDefSpec );
            CswNbtMetaDataNodeTypeProp ResultTypeNTP = CertDefCharLimitNT.getNodeTypePropByObjectClassProp( CswNbtObjClassCertDefCharacteristicLimit.PropertyName.ResultType );
            CswNbtMetaDataNodeTypeProp LimitsNTP = CertDefCharLimitNT.getNodeTypePropByObjectClassProp( CswNbtObjClassCertDefCharacteristicLimit.PropertyName.Limits );
            CswNbtMetaDataNodeTypeProp PassOptionsNTP = CertDefCharLimitNT.getNodeTypePropByObjectClassProp( CswNbtObjClassCertDefCharacteristicLimit.PropertyName.PassOptions );
            CswNbtMetaDataNodeTypeProp PassValueNTP = CertDefCharLimitNT.getNodeTypePropByObjectClassProp( CswNbtObjClassCertDefCharacteristicLimit.PropertyName.PassValue );

            // Layout
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, CertDefCharLimitNT.NodeTypeId, MethodCharacteristicNTP, true, CertDefCharLimitNT.getFirstNodeTypeTab().TabId, 1, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, CertDefCharLimitNT.NodeTypeId, CertDefSpecNTP, true, CertDefCharLimitNT.getFirstNodeTypeTab().TabId, 2, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, CertDefCharLimitNT.NodeTypeId, ResultTypeNTP, true, CertDefCharLimitNT.getFirstNodeTypeTab().TabId, 3, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, CertDefCharLimitNT.NodeTypeId, LimitsNTP, true, CertDefCharLimitNT.getFirstNodeTypeTab().TabId, 4, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, CertDefCharLimitNT.NodeTypeId, PassOptionsNTP, true, CertDefCharLimitNT.getFirstNodeTypeTab().TabId, 5, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, CertDefCharLimitNT.NodeTypeId, PassValueNTP, true, CertDefCharLimitNT.getFirstNodeTypeTab().TabId, 6, 1 );

            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, CertDefCharLimitNT.NodeTypeId, MethodCharacteristicNTP, true, 1, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, CertDefCharLimitNT.NodeTypeId, CertDefSpecNTP, true, 2, 1 );

            // Set up 'Result Type' property reference
            ResultTypeNTP.DesignNode.AttributeProperty[CswNbtFieldTypeRulePropertyReference.AttributeName.FKType].AsText.Text = CswEnumNbtViewPropType.NodeTypePropId.ToString();
            ResultTypeNTP.DesignNode.AttributeProperty[CswNbtFieldTypeRulePropertyReference.AttributeName.Relationship].AsList.Value = MethodCharacteristicNTP.PropId.ToString();
            ResultTypeNTP.DesignNode.AttributeProperty[CswNbtFieldTypeRulePropertyReference.AttributeName.RelatedPropType].AsText.Text = CswEnumNbtViewPropType.NodeTypePropId.ToString();
            ResultTypeNTP.DesignNode.AttributeProperty[CswNbtFieldTypeRulePropertyReference.AttributeName.RelatedProperty].AsList.Value = MethodCharResultTypeNTP.PropId.ToString();

            // Set display conditions
            LimitsNTP.DesignNode.DisplayConditionProperty.RelatedNodeId = ResultTypeNTP.DesignNode.NodeId;
            LimitsNTP.DesignNode.DisplayConditionSubfield.Value = CswNbtFieldTypeRulePropertyReference.SubFieldName.Value.ToString();
            LimitsNTP.DesignNode.DisplayConditionFilterMode.Value = CswEnumNbtFilterMode.Equals.ToString();
            LimitsNTP.DesignNode.DisplayConditionValue.Text = CswNbtObjClassMethodCharacteristic.ResultTypeOption.Quantitative;

            PassOptionsNTP.DesignNode.DisplayConditionProperty.RelatedNodeId = ResultTypeNTP.DesignNode.NodeId;
            PassOptionsNTP.DesignNode.DisplayConditionSubfield.Value = CswNbtFieldTypeRulePropertyReference.SubFieldName.Value.ToString();
            PassOptionsNTP.DesignNode.DisplayConditionFilterMode.Value = CswEnumNbtFilterMode.Equals.ToString();
            PassOptionsNTP.DesignNode.DisplayConditionValue.Text = CswNbtObjClassMethodCharacteristic.ResultTypeOption.Qualitative;

            PassValueNTP.DesignNode.DisplayConditionProperty.RelatedNodeId = ResultTypeNTP.DesignNode.NodeId;
            PassValueNTP.DesignNode.DisplayConditionSubfield.Value = CswNbtFieldTypeRulePropertyReference.SubFieldName.Value.ToString();
            PassValueNTP.DesignNode.DisplayConditionFilterMode.Value = CswEnumNbtFilterMode.Equals.ToString();
            PassValueNTP.DesignNode.DisplayConditionValue.Text = CswNbtObjClassMethodCharacteristic.ResultTypeOption.Match;
        }
    }
}