using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System.Collections;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26833
    /// </summary>
    public class CswUpdateSchemaCase26833 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswNbtMetaDataNodeType supplyNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Supply" );

            if( null != supplyNT )
            {
                CswNbtMetaDataNodeTypeProp pictureNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( supplyNT.NodeTypeId, "Picture" );
                if( null != pictureNTP )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( pictureNTP );
                    CswNbtMetaDataNodeTypeTab tabToAddImagePropTo = _getTab( supplyNT, "Picture", 1 );
                    CswNbtMetaDataNodeTypeProp imageNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( supplyNT, CswNbtMetaDataFieldType.NbtFieldType.Image, "Picture", tabToAddImagePropTo.TabId );
                }
            }

        }//Update()

        private CswNbtMetaDataNodeTypeTab _getTab( CswNbtMetaDataNodeType MaterialNT, string TabName, Int32 Order )
        {
            CswNbtMetaDataNodeTypeTab tab = MaterialNT.getNodeTypeTab( TabName );
            if( null == tab )
            {
                tab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( MaterialNT, TabName, Order );
            }
            else
            {
                tab.TabOrder = Order;
            }
            return tab;
        } // _getTab()

    }//class CswUpdateSchemaCase26833

}//namespace ChemSW.Nbt.Schema