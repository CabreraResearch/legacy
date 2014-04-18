using ChemSW.Audit;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_Case52297B : CswUpdateSchemaTo
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
            return "B";
        }

        public override string Title
        {
            get { return "MLM2: Create Grid Props on OC CertDef Spec"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass CertDefSpecOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefSpecClass );

            CswNbtMetaDataObjectClass CertDefOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertificateDefinitionClass);
            CswNbtMetaDataObjectClass CertDefCharacteristicOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefCharacteristicLimitClass);
            CswNbtMetaDataObjectClass CertDefConditionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefConditionClass);

            ICswNbtMetaDataProp CertDefConditionCertDefOCP = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( CertDefConditionOC.ObjectClassId,
                                                                                                                  CswNbtObjClassCertDefCondition.PropertyName.CertDefSpec );

            ICswNbtMetaDataProp CertDefCharacteristicCertDefOCP = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( CertDefCharacteristicOC.ObjectClassId,
                                                                                                                  CswNbtObjClassCertDefCharacteristicLimit.PropertyName.CertDefSpec );

            CswNbtView ConditionsView = _CswNbtSchemaModTrnsctn.makeSafeView( "CertDefConditionsGridOnCertDefSpec",
                                                                              CswEnumNbtViewVisibility.Property );
            CswNbtViewRelationship ParentRelationship = ConditionsView.AddViewRelationship( CertDefSpecOC, true );
            CswNbtViewRelationship ConditionsRelationship = ConditionsView.AddViewRelationship( ParentRelationship,
                                                CswEnumNbtViewPropOwnerType.Second,
                                                CertDefConditionCertDefOCP,
                                                true);

            ConditionsView.AddViewProperty( ConditionsRelationship, CertDefConditionOC.getObjectClassProp( CswNbtObjClassCertDefCondition.PropertyName.Value), 1 );
            ConditionsView.AddViewProperty( ConditionsRelationship, CertDefConditionOC.getObjectClassProp( CswNbtObjClassCertDefCondition.PropertyName.MethodCondition), 2 );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassCertDefSpec.PropertyName.Conditions,
                FieldType = CswEnumNbtFieldType.Grid,
                ViewXml = ConditionsView.ToString()
            } );

            CswNbtView CharacteristicsView = _CswNbtSchemaModTrnsctn.makeSafeView( "CertDefCharacteristicsGridOnCertDefSpec",
                                                                              CswEnumNbtViewVisibility.Property );
            ParentRelationship = CharacteristicsView.AddViewRelationship( CertDefSpecOC, true );
            CswNbtViewRelationship CharacteristicsRelationship = CharacteristicsView.AddViewRelationship( ParentRelationship,
                                                CswEnumNbtViewPropOwnerType.Second,
                                                CertDefCharacteristicCertDefOCP,
                                                true);

            CharacteristicsView.AddViewProperty( CharacteristicsRelationship, CertDefCharacteristicOC.getObjectClassProp( CswNbtObjClassCertDefCharacteristicLimit.PropertyName.MethodCharacteristic), 1 );
            CharacteristicsView.AddViewProperty( CharacteristicsRelationship, CertDefCharacteristicOC.getObjectClassProp( CswNbtObjClassCertDefCharacteristicLimit.PropertyName.ResultType), 2 );
            CharacteristicsView.AddViewProperty( CharacteristicsRelationship, CertDefCharacteristicOC.getObjectClassProp( CswNbtObjClassCertDefCharacteristicLimit.PropertyName.Limits), 3 );
            CharacteristicsView.AddViewProperty( CharacteristicsRelationship, CertDefCharacteristicOC.getObjectClassProp( CswNbtObjClassCertDefCharacteristicLimit.PropertyName.PassOptions), 4 );
            CharacteristicsView.AddViewProperty( CharacteristicsRelationship, CertDefCharacteristicOC.getObjectClassProp( CswNbtObjClassCertDefCharacteristicLimit.PropertyName.PassValue), 5 );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassCertDefSpec.PropertyName.Characteristics,
                FieldType = CswEnumNbtFieldType.Grid,
                ViewXml = CharacteristicsView.ToString()
            } );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema