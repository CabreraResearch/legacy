using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_Case52303B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE; }
        }

        public override int CaseNo
        {
            get { return 52303; }
        }

        public override string Title
        {
            get { return "MLM2: Add default OC for CertDef Condition"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass CertDefConditionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefConditionClass );

            _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( CertDefConditionOC )
                {
                    Category = "MLM",
                    ObjectClass = CertDefConditionOC,
                    ObjectClassId = CertDefConditionOC.ObjectClassId,
                    NodeTypeName = "CertDef Condition",
                    Searchable = true,
                    NameTemplate = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassCertDefCondition.PropertyName.CertDefSpec) + ": " +
                                   CswNbtMetaData.MakeTemplateEntry(  CswNbtObjClassCertDefCondition.PropertyName.MethodCondition)
                } );

        }

    }
}

//namespace ChemSW.Nbt.Schema