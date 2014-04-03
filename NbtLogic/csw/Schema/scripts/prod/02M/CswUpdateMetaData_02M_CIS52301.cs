using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS52301 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 52301; }
        }

        public override string Title
        {
            get { return "Method Characteristic OC"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass UoMOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
            CswNbtMetaDataObjectClass MethodOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodClass );
            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );

            CswNbtMetaDataObjectClass MethodCharacteristicOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.MethodCharacteristicClass, "barchart.png", true );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.MLM, MethodCharacteristicOC.ObjectClassId );

            CswNbtMetaDataObjectClassProp MethodOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( MethodCharacteristicOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassMethodCharacteristic.PropertyName.Method,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsCompoundUnique = true,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = MethodOC.ObjectClassId,
                IsRequired = true
            } );
            CswNbtMetaDataObjectClassProp CharacteristicOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( MethodCharacteristicOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassMethodCharacteristic.PropertyName.Characteristic,
                FieldType = CswEnumNbtFieldType.List,
                IsCompoundUnique = true,
                IsRequired = true
            } );
            CswNbtMetaDataObjectClassProp ConstituentMaterialOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( MethodCharacteristicOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassMethodCharacteristic.PropertyName.ConstituentMaterial,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsCompoundUnique = true,
                FkType = CswEnumNbtViewRelatedIdType.PropertySetId.ToString(),
                FkValue = MaterialPS.PropertySetId
            } );
            CswNbtMetaDataObjectClassProp ResultTypeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( MethodCharacteristicOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassMethodCharacteristic.PropertyName.ResultType,
                FieldType = CswEnumNbtFieldType.List,
                ListOptions = CswNbtObjClassMethodCharacteristic.ResultTypeOption.Quantitative + "," +
                              CswNbtObjClassMethodCharacteristic.ResultTypeOption.Qualitative + "," +
                              CswNbtObjClassMethodCharacteristic.ResultTypeOption.Match,
                IsRequired = true
            } );
            CswNbtMetaDataObjectClassProp ResultUnitsOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( MethodCharacteristicOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassMethodCharacteristic.PropertyName.ResultUnits,
                FieldType = CswEnumNbtFieldType.Relationship,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = UoMOC.ObjectClassId,
                FilterPropId = ResultTypeOCP.ObjectClassPropId,
                FilterMode = CswEnumNbtFilterMode.Equals,
                FilterSubfield = CswNbtFieldTypeRuleList.SubFieldName.Value,
                FilterValue = CswNbtObjClassMethodCharacteristic.ResultTypeOption.Quantitative
            } );
            CswNbtMetaDataObjectClassProp ResultOptionsOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( MethodCharacteristicOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassMethodCharacteristic.PropertyName.ResultOptions,
                FieldType = CswEnumNbtFieldType.Memo,
                FilterPropId = ResultTypeOCP.ObjectClassPropId,
                FilterMode = CswEnumNbtFilterMode.Equals,
                FilterSubfield = CswNbtFieldTypeRuleList.SubFieldName.Value,
                FilterValue = CswNbtObjClassMethodCharacteristic.ResultTypeOption.Qualitative
            } );
            CswNbtMetaDataObjectClassProp ObsoleteOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( MethodCharacteristicOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassMethodCharacteristic.PropertyName.Obsolete,
                FieldType = CswEnumNbtFieldType.Logical,
                IsRequired = true
            } );
            CswNbtMetaDataObjectClassProp PrecisionOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( MethodCharacteristicOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassMethodCharacteristic.PropertyName.Precision,
                FieldType = CswEnumNbtFieldType.Number,
                NumberMinValue = 0,
                NumberPrecision = 0,
                FilterPropId = ResultTypeOCP.ObjectClassPropId,
                FilterMode = CswEnumNbtFilterMode.Equals,
                FilterSubfield = CswNbtFieldTypeRuleList.SubFieldName.Value,
                FilterValue = CswNbtObjClassMethodCharacteristic.ResultTypeOption.Quantitative
            } );


        } // update()
    } // class CswUpdateMetaData_02M_CIS52301
}