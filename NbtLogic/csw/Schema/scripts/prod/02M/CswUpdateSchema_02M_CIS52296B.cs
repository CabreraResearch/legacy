using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52296B: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 52296; }
        }

        public override string Title
        {
            get { return "Create MLM CertDefSpecLevel NodeType"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass CertDefSpecLevelOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefSpecLevel );
            CswNbtMetaDataNodeType CertDefSpecLevelNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( CertDefSpecLevelOC )
                {
                    Category = "MLM",
                    IconFileName = "barchart.png",
                    NodeTypeName = "Cert Def Spec Level",                                                                //TODO: uncomment below when CIS-52297 is done
                    NameTemplate = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassCertDefSpecLevel.PropertyName.Level ) //+ " " + CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassCertDefSpecLevel.PropertyName.CertDefSpec )
                } );
            CswNbtMetaDataNodeTypeTab FirstTab = CertDefSpecLevelNT.getFirstNodeTypeTab();

            foreach( CswNbtMetaDataObjectClassProp ocp in CertDefSpecLevelOC.getObjectClassProps() )
            {
                CswNbtMetaDataNodeTypeProp ntp = CertDefSpecLevelNT.getNodeTypePropByObjectClassProp( ocp );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, CertDefSpecLevelNT.NodeTypeId, ntp, true, FirstTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, CertDefSpecLevelNT.NodeTypeId, ntp, true );
            }
        }
    }
}