using ChemSW.Audit;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS52296A: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 52296; }
        }

        public override string Title
        {
            get { return "Create MLM CertDefSpecLevel ObjectClass"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass CertDefSpecLevelOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.CertDefSpecLevelClass, "barchart.png", CswEnumAuditLevel.NoAudit );

            CswNbtMetaDataObjectClass LevelOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.Level );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecLevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( CertDefSpecLevelOC )
            {
                PropName = CswNbtObjClassCertDefSpecLevel.PropertyName.Level,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsRequired = true,
                IsCompoundUnique = true,
                IsFk = true,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = LevelOC.ObjectClassId
            } );

            CswNbtMetaDataObjectClass CertDefOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefSpecClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecLevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( CertDefSpecLevelOC )
            {
                PropName = CswNbtObjClassCertDefSpecLevel.PropertyName.CertDefSpec,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsRequired = true,
                IsCompoundUnique = true,
                IsFk = true,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = CertDefOC.ObjectClassId
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecLevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( CertDefSpecLevelOC )
            {
                PropName = CswNbtObjClassCertDefSpecLevel.PropertyName.AllowedDataSources,
                FieldType = CswEnumNbtFieldType.List,
                ListOptions = "Test Only,CofA Only,Test or CofA"
            } );

            CswNbtMetaDataObjectClassProp RequiredForApprovalOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecLevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( CertDefSpecLevelOC )
                {
                    PropName = CswNbtObjClassCertDefSpecLevel.PropertyName.RequiredForApproval,
                    FieldType = CswEnumNbtFieldType.Logical,
                    IsRequired = true
                } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( RequiredForApprovalOCP, false );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecLevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( CertDefSpecLevelOC )
            {
                PropName = CswNbtObjClassCertDefSpecLevel.PropertyName.InitialSampleRegime,
                FieldType = CswEnumNbtFieldType.List,
                ListOptions = "None,Fixed,SqrtN,Every"
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecLevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( CertDefSpecLevelOC )
            {
                PropName = CswNbtObjClassCertDefSpecLevel.PropertyName.RetestSampleRegime,
                FieldType = CswEnumNbtFieldType.List,
                ListOptions = "None,Fixed,SqrtN,Every"
            } );

            //This property's options are controlled by business logic
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecLevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( CertDefSpecLevelOC )
            {
                PropName = CswNbtObjClassCertDefSpecLevel.PropertyName.SampleSize,
                FieldType = CswEnumNbtFieldType.Quantity
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecLevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( CertDefSpecLevelOC )
            {
                PropName = CswNbtObjClassCertDefSpecLevel.PropertyName.SampleSizeNumber,
                FieldType = CswEnumNbtFieldType.Number
            } );
            
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecLevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( CertDefSpecLevelOC )
            {
                PropName = CswNbtObjClassCertDefSpecLevel.PropertyName.Frequency,
                FieldType = CswEnumNbtFieldType.List,
                ListOptions = "All Lots,First Lot Only,All Except First Lot"
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecLevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( CertDefSpecLevelOC )
            {
                PropName = CswNbtObjClassCertDefSpecLevel.PropertyName.ApprovalPeriod,
                FieldType = CswEnumNbtFieldType.Quantity
            } );

            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.MLM, CertDefSpecLevelOC.ObjectClassId );

        }
    }
}