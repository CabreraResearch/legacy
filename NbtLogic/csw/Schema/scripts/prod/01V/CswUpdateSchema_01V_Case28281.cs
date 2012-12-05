using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28281
    /// </summary>
    public class CswUpdateSchema_01V_Case28281 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28281; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType ContainerNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container" );
            if( null != ContainerNT )
            {
                CswNbtMetaDataNodeTypeTab ContainerTab = ContainerNT.getFirstNodeTypeTab();

                CswNbtMetaDataNodeTypeProp StoragePressureNTP = 
                    _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.StoragePressure );
                CswNbtMetaDataNodeTypeProp StorageTemperatureNTP =
                    _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.StorageTemperature );
                CswNbtMetaDataNodeTypeProp UseTypeNTP =
                    _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.UseType );

                StoragePressureNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                StorageTemperatureNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                UseTypeNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

                StoragePressureNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, ContainerTab.TabId, 1, 2, "Fire Reporting");
                StorageTemperatureNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, ContainerTab.TabId, 2, 2, "Fire Reporting" );
                UseTypeNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, ContainerTab.TabId, 3, 2, "Fire Reporting" );
            }
        }

        //Update()

    }//class CswUpdateSchemaCase_01V_28281

}//namespace ChemSW.Nbt.Schema