using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52316: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 52316; }
        }

        public override string Title
        {
            get { return "Script for " + CaseNo; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            if( _CswNbtSchemaModTrnsctn.isMaster() )
            {
                CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
                foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeTab firstTab = UserNT.getFirstNodeTypeTab();
                    CswNbtMetaDataNodeTypeProp CostCodeNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.CostCode );

                    CostCodeNTP.removeFromAllLayouts();
                    CostCodeNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, firstTab.TabId );
                }
            }
        }
    }
}