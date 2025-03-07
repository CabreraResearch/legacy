﻿using ChemSW.Audit;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_Case52302A : CswUpdateSchemaTo
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
            CswNbtMetaDataObjectClass MethodConditionOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.MethodConditionClass, "check.png", CswEnumAuditLevel.PlainAudit );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.MLM, MethodConditionOC.ObjectClassId );

            CswNbtMetaDataObjectClass UoMOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );

            CswNbtMetaDataObjectClass MethodOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodClass );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( MethodConditionOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassMethodCondition.PropertyName.Method,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsCompoundUnique = true,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = MethodOC.ObjectClassId
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( MethodConditionOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassMethodCondition.PropertyName.Name,
                FieldType = CswEnumNbtFieldType.List,
                IsCompoundUnique = true
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( MethodConditionOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassMethodCondition.PropertyName.Units,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsFk = true,
                IsCompoundUnique = false,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = UoMOC.ObjectClassId
            } );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema