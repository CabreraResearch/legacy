using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 30194
    /// </summary>
    public class CswUpdateSchema_02D_Case30194 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30194; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp WorkUnitNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.WorkUnit );
                CswNbtMetaDataNodeTypeTab UserTab = UserNT.getFirstNodeTypeTab();
                WorkUnitNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, UserTab.TabId );
            }

        } // update()

    }//class CswUpdateSchema_02C_Case30194

}//namespace ChemSW.Nbt.Schema