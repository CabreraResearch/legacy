using ChemSW.Audit;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_Case52303A : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE; }
        }

        public override int CaseNo
        {
            get { return 52303; }
        }

        public override string Title
        {
            get { return "MLM2: Create OC CertDef Condition"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass CertDefConditionOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.CertDefConditionClass, "check.png", CswEnumAuditLevel.PlainAudit );

            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.MLM, CertDefConditionOC.ObjectClassId );

            CswNbtMetaDataObjectClass MethodConditionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodConditionClass);

            CswNbtMetaDataObjectClass CertDefSpecOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefSpecClass);

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefConditionOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassCertDefCondition.PropertyName.MethodCondition,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsRequired = true,
                ReadOnly = true,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = MethodConditionOC.ObjectClassId
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefConditionOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassCertDefCondition.PropertyName.CertDefSpec,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsRequired = true,
                ReadOnly = true,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = CertDefSpecOC.ObjectClassId
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefConditionOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassCertDefCondition.PropertyName.Value,
                FieldType = CswEnumNbtFieldType.Text
            } );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema