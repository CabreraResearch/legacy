using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_Case52302B : CswUpdateSchemaTo
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
            get { return "MLM2: Add default OC for Method Condition"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MethodConditionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodConditionClass );

            _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( MethodConditionOC )
                {
                    Category = "MLM",
                    ObjectClass = MethodConditionOC,
                    ObjectClassId = MethodConditionOC.ObjectClassId,
                    NodeTypeName = "Method Condition",
                    Searchable = true
                } );
        }

    }
}

//namespace ChemSW.Nbt.Schema