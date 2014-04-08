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
            CswNbtMetaDataObjectClass CertDefSpecLevelOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.CertDefSpecLevel, "barchart.png", false );

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

            //TODO - uncomment when CIS-52297 (slated for Mag.2) is done
            //CswNbtMetaDataObjectClass CertDefOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefSpec );
            //_CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecLevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( CertDefSpecLevelOC )
            //{
            //    PropName = CswNbtObjClassCertDefSpecLevel.PropertyName.Level,
            //    FieldType = CswEnumNbtFieldType.Relationship,
            //    IsRequired = true,
            //    IsCompoundUnique = true,
            //    IsFk = true,
            //    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
            //    FkValue = CertDefOC.ObjectClassId
            //} );

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
                ListOptions = "" //TODO: get list opts from David/Steve
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecLevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( CertDefSpecLevelOC )
            {
                PropName = CswNbtObjClassCertDefSpecLevel.PropertyName.RetestSampleRegime,
                FieldType = CswEnumNbtFieldType.List,
                ListOptions = "" //TODO: get list opts from David/Steve
            } );

            //TODO: get more info on this prop, it's unclear on the case
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecLevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( CertDefSpecLevelOC )
            {
                PropName = CswNbtObjClassCertDefSpecLevel.PropertyName.SampleSize,
                FieldType = CswEnumNbtFieldType.Quantity
            } );
            
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecLevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( CertDefSpecLevelOC )
            {
                PropName = CswNbtObjClassCertDefSpecLevel.PropertyName.Frequency,
                FieldType = CswEnumNbtFieldType.List,
                ListOptions = "All Lots,First Lot Only,All Except First Lot"
            } );

            //TODO: figure out how to set this
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecLevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( CertDefSpecLevelOC )
            {
                PropName = CswNbtObjClassCertDefSpecLevel.PropertyName.ApprovalPeriod,
                FieldType = CswEnumNbtFieldType.Quantity
            } );

            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.MLM, CertDefSpecLevelOC.ObjectClassId );

        }
    }
}