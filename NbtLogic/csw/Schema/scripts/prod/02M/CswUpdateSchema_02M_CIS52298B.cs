using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52298B: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 52298; }
        }

        public override string Title
        {
            get { return "Create MLM Level NodeType"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass LevelOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.Level );
            CswNbtMetaDataNodeType LevelNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( LevelOC )
                {
                    Category = "MLM",
                    IconFileName = "barchart.png",
                    NodeTypeName = "Level",
                    NameTemplate = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassLevel.PropertyName.LevelNumber )
                } );
            CswNbtMetaDataNodeTypeTab FirstTab = LevelNT.getFirstNodeTypeTab();

            foreach( CswNbtMetaDataObjectClassProp ocp in LevelOC.getObjectClassProps() )
            {
                CswNbtMetaDataNodeTypeProp ntp = LevelNT.getNodeTypePropByObjectClassProp( ocp );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, LevelNT.NodeTypeId, ntp, true, FirstTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, LevelNT.NodeTypeId, ntp, true);
            }
        }
    }
}