using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28758 
    /// </summary>
    public class CswUpdateSchema_02D_Case28758: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28758; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( CswNbtMetaDataNodeType ContainerNT in ContainerOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp ViewSDS_NTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.ViewSDS );
                CswNbtMetaDataNodeTypeTab ContainerIdentityTab = ContainerNT.getIdentityTab();
                ViewSDS_NTP.updateLayout( CswEnumNbtLayoutType.Edit, true, ContainerIdentityTab.TabId, 1, 2 );
            }

        } // update()

    }//class CswUpdateSchema_02C_Case28758 

}//namespace ChemSW.Nbt.Schema