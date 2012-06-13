
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
            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                ContainerOc, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
                    PropName = CswNbtObjClassContainer.DispensePropertyName
                } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                ContainerOc, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
                    PropName = CswNbtObjClassContainer.DisposePropertyName
                } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                ContainerOc, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
                    PropName = CswNbtObjClassContainer.MovePropertyName
                } );
            foreach( CswNbtMetaDataNodeType ContainerNt in ContainerOc.getNodeTypes() )
            {
                Int32 FirsTabId = Int32.MinValue;
                Int32 PropCount = Int32.MinValue;
                foreach( CswNbtMetaDataNodeTypeTab Tab in from _Tab in ContainerNt.getNodeTypeTabs() orderby _Tab.TabOrder, _Tab.TabId select _Tab )
                {
                    FirsTabId = Tab.TabId;
                    //PropCount =  
                    PropCount += ( from _Prop in Tab.getNodeTypeProps() orderby _Prop.AddLayout.DisplayRow descending where null != _Prop.AddLayout select _Prop ).Count();
                    CswNbtMetaDataNodeTypeProp MaxRowProp = ( from _Prop in Tab.getNodeTypeProps()
                                                              orderby _Prop.AddLayout.DisplayRow descending
                                                              where null != _Prop.AddLayout
                                                              select _Prop ).FirstOrDefault();
                    Int32 MaxRow = ( null != MaxRowProp ) ? MaxRowProp.AddLayout.DisplayRow : 0;
                    if( MaxRow > PropCount )
                    {
                        PropCount = MaxRow;
                    }
                    break;
                }

                CswNbtMetaDataNodeTypeProp DispenseNtp = ContainerNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.DispensePropertyName );
                DispenseNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, FirsTabId, PropCount, 1 );
                DispenseNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, FirsTabId, PropCount, 1 );

                CswNbtMetaDataNodeTypeProp MoveNtp = ContainerNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.MovePropertyName );
                MoveNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, FirsTabId, PropCount, 2 );
                MoveNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, FirsTabId, PropCount, 2 );

                CswNbtMetaDataNodeTypeProp DisposeNtp = ContainerNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.DisposePropertyName );
                DisposeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, FirsTabId, PropCount, 3 );
                DisposeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, FirsTabId, PropCount, 3 );
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
                RequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, FirsTabId, 99, 1 );
                RequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, FirsTabId, 99, 1 );
            }

        }//Update()

    }//class CswUpdateSchemaCase24514RequestButton

}//namespace ChemSW.Nbt.Schema