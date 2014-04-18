using ChemSW.Audit;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_Case52297A : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE; }
        }

        public override int CaseNo
        {
            get { return 52297; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override string Title
        {
            get { return "MLM2: Create OC CertDef Spec"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass CertDefSpecOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.CertDefSpecClass, "doc.png", CswEnumAuditLevel.PlainAudit );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.MLM, CertDefSpecOC.ObjectClassId );

            CswNbtMetaDataObjectClass MethodOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodClass );
            CswNbtMetaDataObjectClass CertDefOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertificateDefinitionClass );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassCertDefSpec.PropertyName.NameForTestingConditions,
                FieldType = CswEnumNbtFieldType.Text
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassCertDefSpec.PropertyName.Method,
                FieldType = CswEnumNbtFieldType.Relationship,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                IsRequired = true,
                ReadOnly = true,
                FkValue = MethodOC.ObjectClassId
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassCertDefSpec.PropertyName.CertDef,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsRequired = true,
                ReadOnly = true,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = CertDefOC.ObjectClassId
            } );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema