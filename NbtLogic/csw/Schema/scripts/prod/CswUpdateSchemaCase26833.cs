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

            //Update the Supply nodetype
            CswNbtMetaDataNodeType supplyNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Supply" );

            if( null != supplyNT )
            {
                CswNbtMetaDataNodeTypeProp pictureNTPSupply = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( supplyNT.NodeTypeId, "Picture" );
                if( null != pictureNTPSupply )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( pictureNTPSupply );
                }
                CswNbtMetaDataNodeTypeTab tabSupplyPic = _getTab( supplyNT, "Picture", 1 );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( supplyNT, CswNbtMetaDataFieldType.NbtFieldType.Image, "Picture", tabSupplyPic.TabId );
            }

            //Update the Biological nodetype
            CswNbtMetaDataNodeType biologicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Biological" );

            if( null != biologicalNT )
            {
                CswNbtMetaDataNodeTypeProp pictureNTPBio = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( biologicalNT.NodeTypeId, "Picture" );
                if( null != pictureNTPBio )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( pictureNTPBio );
                }
                CswNbtMetaDataNodeTypeTab tabBioPic = _getTab( biologicalNT, "Picture", 3 );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( biologicalNT, CswNbtMetaDataFieldType.NbtFieldType.Image, "Picture", tabBioPic.TabId );
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