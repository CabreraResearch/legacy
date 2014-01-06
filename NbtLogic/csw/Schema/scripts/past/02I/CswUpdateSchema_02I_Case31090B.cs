using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31090B: CswUpdateSchemaTo
    {
        public override string Title { get { return "Remove List Code RegList prop from all layouts"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31090; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass RegListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            foreach( CswNbtMetaDataNodeType RegListNT in RegListOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp listCodeNTP = RegListNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.ListCode );
                listCodeNTP.removeFromAllLayouts();
            }
        }

    }

}//namespace ChemSW.Nbt.Schema