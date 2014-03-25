using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02L_Case52302: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE; }
        }

        public override int CaseNo
        {
            get { return 52302; }
        }

        public override string Title
        {
            get { return "MLM2: Create new OC Method Condition"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MethodConditionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodConditionClass);

            if( null == MethodConditionOC)
            {
                MethodConditionOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.MethodConditionClass, "check.png", true );
                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.MLM, MethodConditionOC.ObjectClassId );
                _CswNbtSchemaModTrnsctn.commitTransaction();
            } //if MethodMDOC == null

            _CswNbtSchemaModTrnsctn.createObjectClassProp( MethodConditionOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassMethodCondition.PropertyName.Method,
                FieldType = CswEnumNbtFieldType.Relationship,
                ServerManaged = false,
                IsUnique = true
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( MethodConditionOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassMethodCondition.PropertyName.Name,
                FieldType = CswEnumNbtFieldType.List,
                ServerManaged = false,
                IsCompoundUnique = true
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( MethodConditionOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassMethodCondition.PropertyName.Units,
                FieldType = CswEnumNbtFieldType.List,
                ServerManaged = false
            } );

            _CswNbtSchemaModTrnsctn.commitTransaction();

        } // update()

    }

}//namespace ChemSW.Nbt.Schema