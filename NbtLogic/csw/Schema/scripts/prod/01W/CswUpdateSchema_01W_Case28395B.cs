using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28395
    /// </summary>
    public class CswUpdateSchema_01W_Case28395B : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28395; }
        }

        public override void update()
        {
            //Remove Container fire reporting props from Add Layout
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            foreach( CswNbtMetaDataNodeType ContainerNT  in ContainerOC.getNodeTypes())
            {
                CswNbtMetaDataNodeTypeProp StorageTemperatureNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.StorageTemperature );
                StorageTemperatureNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                CswNbtMetaDataNodeTypeProp StoragePressureNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.StoragePressure );
                StoragePressureNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                CswNbtMetaDataNodeTypeProp UseTypeNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.UseType );
                UseTypeNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            }
        }//Update()

    }//class CswUpdateSchemaCase_01W_28395

}//namespace ChemSW.Nbt.Schema