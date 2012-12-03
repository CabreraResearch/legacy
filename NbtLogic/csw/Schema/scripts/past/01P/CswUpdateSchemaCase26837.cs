using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26837
    /// </summary>
    public class CswUpdateSchemaCase26837 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswNbtMetaDataNodeType biologicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Biological" );
            if( null != biologicalNT )
            {
                CswNbtMetaDataNodeTypeTab tab = _getTab( biologicalNT, "Biosafety", 2 );
                CswNbtMetaDataNodeTypeProp storageConditionsNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( biologicalNT, CswNbtMetaDataFieldType.NbtFieldType.List, "Storage Conditions", tab.TabId );
                if( null != storageConditionsNTP )
                {
                    storageConditionsNTP.ListOptions = "37C,25C,5C,-20C,-80C";
                    storageConditionsNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
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

    }//class CswUpdateSchemaCase26837

}//namespace ChemSW.Nbt.Schema