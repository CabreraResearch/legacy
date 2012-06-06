
using System;
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24514
    /// </summary>
    public class CswUpdateSchemaCase24514RequestButton : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp ContainerRequestOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp(
                ContainerOc, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
                    PropName = CswNbtObjClassContainer.RequestPropertyName
                } );
            foreach( CswNbtMetaDataNodeType ContainerNt in ContainerOc.getNodeTypes() )
            {
                Int32 FirsTabId = Int32.MinValue;
                foreach( CswNbtMetaDataNodeTypeTab Tab in from _Tab in ContainerNt.getNodeTypeTabs() orderby _Tab.TabOrder, _Tab.TabId select _Tab )
                {
                    FirsTabId = Tab.TabId;
                    break;
                }
                CswNbtMetaDataNodeTypeProp RequestNtp = ContainerNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.RequestPropertyName );
                RequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, FirsTabId );
                RequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, FirsTabId );
            }

            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp MaterialRequestOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp(
                MaterialOc, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
                    PropName = CswNbtObjClassMaterial.RequestPropertyName
                } );
            foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getNodeTypes() )
            {
                Int32 FirsTabId = Int32.MinValue;
                foreach( CswNbtMetaDataNodeTypeTab Tab in from _Tab in MaterialNt.getNodeTypeTabs() orderby _Tab.TabOrder, _Tab.TabId select _Tab )
                {
                    FirsTabId = Tab.TabId;
                    break;
                }
                CswNbtMetaDataNodeTypeProp RequestNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.RequestPropertyName );
                RequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, FirsTabId );
                RequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, FirsTabId );
            }

        }//Update()

    }//class CswUpdateSchemaCase24514RequestButton

}//namespace ChemSW.Nbt.Schema