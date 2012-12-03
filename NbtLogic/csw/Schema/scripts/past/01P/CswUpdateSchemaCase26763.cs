using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26763
    /// </summary>
    public class CswUpdateSchemaCase26763 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );

            //This block is needed in the event of a full SchemaUpdate reset to ensure that the RequestDispense button only exists on the Requests tab
            foreach( CswNbtMetaDataNodeType ContainerNt in ContainerOc.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp RequestNtp = ContainerNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.RequestPropertyName );
                foreach( CswNbtMetaDataNodeTypeTab ContainerNodeTypeTab in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeTabs( ContainerNt.NodeTypeId ) )
                {
                    RequestNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, TabId: ContainerNodeTypeTab.TabId );
                }

                CswNbtMetaDataNodeTypeTab RequestsTab = ContainerNt.getNodeTypeTab( "Requests" );
                if( null == RequestsTab )
                {
                    RequestsTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ContainerNt, "Requests", ContainerNt.getNodeTypeTabIds().Count );
                }
                RequestNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, ContainerNt.getFirstNodeTypeTab().TabId );
                RequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, RequestsTab.TabId, 1, 1 );
                RequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, RequestsTab.TabId, 1, 1 );
            }
        }//Update()

    }//class CswUpdateSchemaCase26763

}//namespace ChemSW.Nbt.Schema