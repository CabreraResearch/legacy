using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27330
    /// </summary>
    public class CswUpdateSchemaCase27330 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab IdentityTab = MaterialNt.getNodeTypeTab( "Identity" );
                if( null == IdentityTab )
                {
                    IdentityTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( MaterialNt, "Identity", MaterialNt.getNodeTypeTabIds().Count );
                }
                CswNbtMetaDataNodeTypeProp RequestBtnNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.RequestPropertyName );
                RequestBtnNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, IdentityTab.TabId );
                CswNbtMetaDataNodeTypeProp ReceiveBtnNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.ReceivePropertyName );
                ReceiveBtnNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, IdentityTab.TabId );
            }
        }//Update()

    }//class CswUpdateSchemaCaseXXXXX

}//namespace ChemSW.Nbt.Schema