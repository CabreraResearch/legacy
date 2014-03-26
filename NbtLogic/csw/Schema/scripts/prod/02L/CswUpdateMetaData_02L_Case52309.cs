﻿using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02L_Case52309 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE; }
        }

        public override int CaseNo
        {
            get { return 52309; }
        }

        public override string Title
        {
            get { return "MLM2: Create new OC Testing Lab Method Assignment"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass TestingLabMethodAssignmentOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.TestingLabMethodAssignmentClass, "check.png", true );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.MLM, TestingLabMethodAssignmentOC.ObjectClassId );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( TestingLabMethodAssignmentOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTestingLabMethodAssignment.PropertyName.TestingLab,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsCompoundUnique = true
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( TestingLabMethodAssignmentOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTestingLabMethodAssignment.PropertyName.Method,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsCompoundUnique = true
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( TestingLabMethodAssignmentOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTestingLabMethodAssignment.PropertyName.Cost,
                FieldType = CswEnumNbtFieldType.Text
            } );
        } // update()

    }

}//namespace ChemSW.Nbt.Schema