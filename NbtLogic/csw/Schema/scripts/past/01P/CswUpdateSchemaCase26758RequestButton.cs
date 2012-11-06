
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26758RequestButton
    /// </summary>
    public class CswUpdateSchemaCase26758RequestButton : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );

            foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp RequestsGridNtp = MaterialNt.getNodeTypeProp( "Submitted Requests" );
                if( null != RequestsGridNtp )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( RequestsGridNtp );
                }
                CswNbtMetaDataNodeTypeTab RequestsTab = MaterialNt.getNodeTypeTab( "Requests" );
                if( null != RequestsTab )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeTab( RequestsTab );
                }
                //CswNbtMetaDataNodeTypeProp RequestBtnNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.RequestPropertyName );
                //bool Moved = false;
                //foreach( CswNbtMetaDataNodeTypeTab Tab in MaterialNt.getNodeTypeTabs() )
                //{
                //    if( Tab.TabName == MaterialNt.NodeTypeName )
                //    {
                //        RequestBtnNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, Tab.TabId );
                //        RequestBtnNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, Tab.TabId );
                //        Moved = true;
                //    }
                //}
                //if( false == Moved )
                //{
                //    CswNbtMetaDataNodeTypeTab Tab = MaterialNt.getNodeTypeTabs().FirstOrDefault();
                //    if( null != Tab )
                //    {
                //        RequestBtnNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, Tab.TabId );
                //        RequestBtnNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, Tab.TabId );
                //    }
                //}
            }

        }//Update()


    }//class CswUpdateSchemaCase26758RequestButton

}//namespace ChemSW.Nbt.Schema