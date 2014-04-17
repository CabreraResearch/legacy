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
            return "AF";
        }

        public override string Title
        {
            get { return "MLM2: Create Grid Props on OC CertDef Spec"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass CertDefSpecOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefSpecClass );

            CswNbtMetaDataObjectClass MethodOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodClass );
            CswNbtMetaDataObjectClass CertDefOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertificateDefinitionClass);
            CswNbtMetaDataObjectClass CertDefCharacteristicOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefCharacteristicLimitClass);
            CswNbtMetaDataObjectClass CertDefConditionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefConditionClass);

            ICswNbtMetaDataProp CertDefConditionCertDefOCP = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( CertDefConditionOC.ObjectClassId,
                                                                                                                  CswNbtObjClassCertDefCondition.PropertyName.CertDefSpec );

            CswNbtView ConditionsView = _CswNbtSchemaModTrnsctn.makeSafeView( "CertDefConditionsGridOnCertDefSpec",
                                                                              CswEnumNbtViewVisibility.Property );
            CswNbtViewRelationship ParentRelationship = ConditionsView.AddViewRelationship( CertDefSpecOC, true );
            CswNbtViewRelationship ConditionsRelationship = ConditionsView.AddViewRelationship( ParentRelationship,
                                                CswEnumNbtViewPropOwnerType.Second,
                                                CertDefConditionCertDefOCP,
                                                true);
            ConditionsView.AddViewProperty( ConditionsRelationship, CertDefConditionOC.getObjectClassProp( CswNbtObjClassCertDefCondition.PropertyName.Value), 1 );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassCertDefSpec.PropertyName.Conditions,
                FieldType = CswEnumNbtFieldType.Grid,
                ViewXml = ConditionsView.ToString()
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefSpecOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassCertDefSpec.PropertyName.Characteristics,
                FieldType = CswEnumNbtFieldType.Grid
            } );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema