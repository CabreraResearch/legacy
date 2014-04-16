using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_Case52308B : CswUpdateSchemaTo
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

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass TestingLabUserAssignmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.TestingLabUserAssignmentClass);

            _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( TestingLabUserAssignmentOC )
            {
                Category = "MLM",
                ObjectClass = TestingLabUserAssignmentOC,
                ObjectClassId = TestingLabUserAssignmentOC.ObjectClassId,
                NodeTypeName = "Testing Lab User Assignment",
                Searchable = true,
                NameTemplate = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassTestingLabUserAssignment.PropertyName.TestingLab ) + ": " +
                               CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassTestingLabUserAssignment.PropertyName.User)
            } );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema