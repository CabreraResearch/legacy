using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_Case52297C : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE; }
        }

        public override int CaseNo
        {
            get { return 52297; }
        }

        public override string Title
        {
            get { return "MLM2: Add default NT for CertDef Spec, and place conditions and characteristics in their own tabs"; }
        }

        public override string AppendToScriptName()
        {
            return "C";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass CertDefSpecOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefSpecClass );

            CswNbtMetaDataNodeType CertDefSpecDefaultNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( CertDefSpecOC )
                {
                    Category = "MLM",
                    ObjectClass = CertDefSpecOC,
                    ObjectClassId = CertDefSpecOC.ObjectClassId,
                    NodeTypeName = "CertDef Spec",
                    Searchable = true,
                    NameTemplate = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassCertDefSpec.PropertyName.NameForTestingConditions)
                } );

            CswNbtMetaDataNodeTypeTab ConditionsTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( CertDefSpecDefaultNT,
                                                                                                   "Conditions",
                                                                                                   2 );

            CswNbtMetaDataNodeTypeTab CharacteristicsTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( CertDefSpecDefaultNT,
                                                                                                        "Characteristics",
                                                                                                        3 );

            CswNbtMetaDataNodeTypeProp ConditionsNTP = CertDefSpecDefaultNT.getNodeTypePropByObjectClassProp( CswNbtObjClassCertDefSpec.PropertyName.Conditions);
            ConditionsNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, ConditionsTab.TabId, 1, 1 );

            CswNbtMetaDataNodeTypeProp CharacteristicsNTP = CertDefSpecDefaultNT.getNodeTypePropByObjectClassProp( CswNbtObjClassCertDefSpec.PropertyName.Characteristics);
            CharacteristicsNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, CharacteristicsTab.TabId, 1, 1 );

        }

    }
}

//namespace ChemSW.Nbt.Schema