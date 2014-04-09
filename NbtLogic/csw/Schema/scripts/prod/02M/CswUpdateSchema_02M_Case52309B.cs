using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_Case52309B : CswUpdateSchemaTo
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
            get { return "MLM2: Create default NT for Testing Lab Method Assignment"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass TestingLabMethodAssignmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.TestingLabMethodAssignmentClass );

            _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( TestingLabMethodAssignmentOC )
            {
                Category = "MLM",
                ObjectClass = TestingLabMethodAssignmentOC,
                ObjectClassId = TestingLabMethodAssignmentOC.ObjectClassId,
                NodeTypeName = "Testing Lab Method Assignment",
                Searchable = true
            } );
        }

        // update()
    }
}

//namespace ChemSW.Nbt.Schema