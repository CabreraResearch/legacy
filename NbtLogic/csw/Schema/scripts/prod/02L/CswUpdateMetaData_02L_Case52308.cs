using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02L_Case52308 : CswUpdateSchemaTo
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
            CswNbtMetaDataObjectClass TestingLabUserAssignmentOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.TestingUserLabAssignmentClass, "check.png", true );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.MLM, TestingLabUserAssignmentOC.ObjectClassId );

            _CswNbtSchemaModTrnsctn.commitTransaction();

            _CswNbtSchemaModTrnsctn.createObjectClassProp( TestingLabUserAssignmentOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTestingUserLabAssignment.PropertyName.User,
                FieldType = CswEnumNbtFieldType.Relationship,
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( TestingLabUserAssignmentOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTestingUserLabAssignment.PropertyName.TestingLab,
                FieldType = CswEnumNbtFieldType.Relationship,
            } );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema