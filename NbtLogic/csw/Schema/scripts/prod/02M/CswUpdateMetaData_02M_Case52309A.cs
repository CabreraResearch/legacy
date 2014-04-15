using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_Case52309A : CswUpdateSchemaTo
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

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass TestingLabMethodAssignmentOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.TestingLabMethodAssignmentClass, "check.png", true );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.MLM, TestingLabMethodAssignmentOC.ObjectClassId );


            CswNbtMetaDataObjectClass MethodOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodClass);
            CswNbtMetaDataObjectClass TestingLabOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.TestingLabClass);


            _CswNbtSchemaModTrnsctn.createObjectClassProp( TestingLabMethodAssignmentOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTestingLabMethodAssignment.PropertyName.TestingLab,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsCompoundUnique = true,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = TestingLabOC.ObjectClassId
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( TestingLabMethodAssignmentOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTestingLabMethodAssignment.PropertyName.Method,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsFk = true,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = MethodOC.ObjectClassId,
                IsCompoundUnique = true
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( TestingLabMethodAssignmentOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTestingLabMethodAssignment.PropertyName.Cost,
                FieldType = CswEnumNbtFieldType.Text,
                IsCompoundUnique = false
            } );
        } // update()

    }

}//namespace ChemSW.Nbt.Schema