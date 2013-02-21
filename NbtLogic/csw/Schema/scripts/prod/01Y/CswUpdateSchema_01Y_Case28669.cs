using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28669
    /// </summary>
    public class CswUpdateSchema_01Y_Case28669 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28669; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType ContainerNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container" );
            if( null != ContainerNT )
            {
                CswNbtMetaDataNodeTypeTab FireCodeTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ContainerNT, "Fire Code" );
                CswNbtMetaDataNodeTypeProp StoragePressureNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.StoragePressure );
                CswNbtMetaDataNodeTypeProp StorageTemperatureNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.StorageTemperature );
                CswNbtMetaDataNodeTypeProp UseTypeNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.UseType );
                StoragePressureNTP.removeFromAllLayouts();
                StorageTemperatureNTP.removeFromAllLayouts();
                UseTypeNTP.removeFromAllLayouts();
                StoragePressureNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, FireCodeTab.TabId, 1, 1 );
                StorageTemperatureNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, FireCodeTab.TabId, 2, 1 );
                UseTypeNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, FireCodeTab.TabId, 3, 1 );

                foreach( CswNbtMetaDataNodeTypeTab Tab in ContainerNT.getNodeTypeTabs() )
                {
                    if( Tab.TabOrder >= 4 )
                        Tab.TabOrder += 1;
                }
                CswNbtMetaDataNodeTypeTab DispensesTab = ContainerNT.getNodeTypeTab( "Dispenses" );
                if( null == DispensesTab )
                {
                    DispensesTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ContainerNT, "Dispenses", 4 );
                }
                CswNbtMetaDataNodeTypeProp DispenseNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Dispense );
                DispenseNTP.removeFromAllLayouts();
                DispenseNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, DispensesTab.TabId, 1, 1 );
            }

        } //Update()

    }//class CswUpdateSchema_01Y_Case28669

}//namespace ChemSW.Nbt.Schema