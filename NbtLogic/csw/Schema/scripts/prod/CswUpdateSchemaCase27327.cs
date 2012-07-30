using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27327
    /// </summary>
    public class CswUpdateSchemaCase27327 : CswUpdateSchemaTo
    {
        public override void update()
        {

            /* updating the propery order on size */
            CswNbtMetaDataNodeType sizeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Size" );
            if( null != sizeNT )
            {
                CswNbtMetaDataNodeTypeProp initQuantNTP = sizeNT.getNodeTypeProp( CswNbtObjClassSize.InitialQuantityPropertyName );
                CswNbtMetaDataNodeTypeProp catalogNoNTP = sizeNT.getNodeTypeProp( CswNbtObjClassSize.CatalogNoPropertyName );
                CswNbtMetaDataNodeTypeProp quanEditNTP = sizeNT.getNodeTypeProp( CswNbtObjClassSize.QuantityEditablePropertyName );
                CswNbtMetaDataNodeTypeProp dispensableNTP = sizeNT.getNodeTypeProp( CswNbtObjClassSize.DispensablePropertyName );

                if( null != initQuantNTP )
                {
                    initQuantNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, Int32.MinValue, 1, 1 );
                }

                if( null != catalogNoNTP )
                {
                    catalogNoNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, Int32.MinValue, 2, 1 );
                }

                if( null != quanEditNTP )
                {
                    quanEditNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, Int32.MinValue, 3, 1 );
                }

                if( null != dispensableNTP )
                {
                    dispensableNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, Int32.MinValue, 4, 1 );
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase27327

}//namespace ChemSW.Nbt.Schema