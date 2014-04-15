using ChemSW.Audit;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS52299D : CswUpdateSchemaTo
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
            get { return "CertDef Characteristic Limit"; }
        }

        public override string AppendToScriptName()
        {
            return "D";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass CertDefSpecOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefSpecClass );
            CswNbtMetaDataObjectClass MethodCharacteristicOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodCharacteristicClass );

            CswNbtMetaDataObjectClass CertDefCharLimitOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.CertDefCharacteristicLimitClass, "doc.png", CswEnumAuditLevel.PlainAudit );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.MLM, CertDefCharLimitOC.ObjectClassId );

            CswNbtMetaDataObjectClassProp CertDefSpecOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefCharLimitOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertDefCharacteristicLimit.PropertyName.CertDefSpec,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    ReadOnly = true,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = CertDefSpecOC.ObjectClassId,
                    IsRequired = true
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefCharLimitOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertDefCharacteristicLimit.PropertyName.Limits,
                    FieldType = CswEnumNbtFieldType.NumericRange
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefCharLimitOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertDefCharacteristicLimit.PropertyName.MethodCharacteristic,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    ReadOnly = true,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = MethodCharacteristicOC.ObjectClassId,
                    IsRequired = true
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefCharLimitOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertDefCharacteristicLimit.PropertyName.PassOptions,
                    FieldType = CswEnumNbtFieldType.MultiList
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefCharLimitOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertDefCharacteristicLimit.PropertyName.PassValue,
                    FieldType = CswEnumNbtFieldType.Text
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefCharLimitOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertDefCharacteristicLimit.PropertyName.ResultType,
                    FieldType = CswEnumNbtFieldType.PropertyReference
                } );

            CertDefCharLimitOC._DataRow["searchdeferpropid"] = CertDefSpecOCP.ObjectClassPropId;

        } // update()
    } // class
} // namespace