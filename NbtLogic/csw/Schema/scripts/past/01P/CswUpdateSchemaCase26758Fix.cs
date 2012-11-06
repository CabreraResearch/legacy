using System;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26758Fixed
    /// </summary>
    public class CswUpdateSchemaCase26758Fixed : CswUpdateSchemaTo
    {
        public override void update()
        {
            //CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );

            //foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getNodeTypes() )
            //{
            //    CswNbtMetaDataNodeTypeProp RequestBtnNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.RequestPropertyName );
            //    CswNbtMetaDataNodeTypeProp ReceiveBtnNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.ReceivePropertyName );
            //    RequestBtnNtp.updateLayout(CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, ReceiveBtnNtp, true );
            //    RequestBtnNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, ReceiveBtnNtp, true );
            //}
        }//Update()

    }//class CswUpdateSchemaCase26758Fixed

}//namespace ChemSW.Nbt.Schema