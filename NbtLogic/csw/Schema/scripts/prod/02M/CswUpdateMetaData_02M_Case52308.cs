using ChemSW.Audit;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_Case52308 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE; }
        }

        public override int CaseNo
        {
            get { return 52308; }
        }

        public override string Title
        {
            get { return "MLM2: Create new OC Testing Lab User Assignment"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass TestingLabUserAssignmentOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.TestingLabUserAssignmentClass, "check.png", CswEnumAuditLevel.PlainAudit );
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            CswNbtMetaDataObjectClass TestingLabOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.TestingLabClass);

            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.MLM, TestingLabUserAssignmentOC.ObjectClassId );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( TestingLabUserAssignmentOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTestingLabUserAssignment.PropertyName.User,
                FieldType = CswEnumNbtFieldType.Relationship,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = UserOC.ObjectClassId

            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( TestingLabUserAssignmentOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTestingLabUserAssignment.PropertyName.TestingLab,
                FieldType = CswEnumNbtFieldType.Relationship,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = TestingLabOC.ObjectClassId

            } );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema