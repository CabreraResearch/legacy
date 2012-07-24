using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27263
    /// </summary>
    public class CswUpdateSchemaCase27263 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass RequestItemOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
            foreach( CswNbtMetaDataNodeType RequestItemNt in RequestItemOc.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp MaterialNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Material );
                MaterialNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );

                CswNbtMetaDataNodeTypeProp ContainerNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Container );
                ContainerNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, MaterialNtp, true );

                CswNbtMetaDataNodeTypeProp TypeNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Type );
                TypeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, ContainerNtp, true );

                CswNbtMetaDataNodeTypeProp LocationNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Location );
                LocationNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, TypeNtp, true );

                CswNbtMetaDataNodeTypeProp RequestNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request );
                RequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, LocationNtp, true );

            }
            

        }//Update()

    }//class CswUpdateSchemaCase27263

}//namespace ChemSW.Nbt.Schema