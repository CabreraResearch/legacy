using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS52298A: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 52298; }
        }

        public override string Title
        {
            get { return "Create MLM Level ObjectClass"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass LevelOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.Level, "barchart.png", false );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( LevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( LevelOC )
                {
                    PropName = CswNbtObjClassLevel.PropertyName.LevelNumber,
                    FieldType = CswEnumNbtFieldType.Text,
                    IsUnique = true,
                    IsRequired = true
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( LevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( LevelOC )
            {
                PropName = CswNbtObjClassLevel.PropertyName.LevelName,
                FieldType = CswEnumNbtFieldType.Text,
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( LevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( LevelOC )
            {
                PropName = CswNbtObjClassLevel.PropertyName.LevelSuffix,
                FieldType = CswEnumNbtFieldType.Text,
            } );

            CswNbtMetaDataObjectClassProp OnlyQualifiedCofAOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( LevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( LevelOC )
            {
                PropName = CswNbtObjClassLevel.PropertyName.OnlyQualifiedCofA,
                FieldType = CswEnumNbtFieldType.Logical,
                IsRequired = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( OnlyQualifiedCofAOCP, true );

            CswNbtMetaDataObjectClassProp LabUseOnlyOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( LevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( LevelOC )
            {
                PropName = CswNbtObjClassLevel.PropertyName.LabUseOnly,
                FieldType = CswEnumNbtFieldType.Logical,
                IsRequired = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( LabUseOnlyOCP, false );

            CswNbtMetaDataObjectClassProp Enterprise = _CswNbtSchemaModTrnsctn.createObjectClassProp( LevelOC, new CswNbtWcfMetaDataModel.ObjectClassProp( LevelOC )
            {
                PropName = CswNbtObjClassLevel.PropertyName.Enterprise,
                FieldType = CswEnumNbtFieldType.Logical,
                IsRequired = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( Enterprise, false );


            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction(CswEnumNbtModuleName.MLM, LevelOC.ObjectClassId);

        }
    }
}