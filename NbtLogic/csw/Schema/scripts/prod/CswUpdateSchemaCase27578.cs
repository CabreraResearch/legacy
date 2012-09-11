using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Properties;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27578
    /// </summary>
    public class CswUpdateSchemaCase27578 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {

            CswNbtMetaDataNodeType sizeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Size" );
            if( null != sizeNT )
            {
                CswNbtMetaDataNodeTypeTab sizeNTT = sizeNT.getFirstNodeTypeTab();
                if( null != sizeNTT )
                {
                    CswNbtMetaDataNodeTypeProp containerTypeNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( sizeNT, CswNbtMetaDataFieldType.NbtFieldType.List, "Container Type", sizeNTT.TabId );
                    containerTypeNTP.ListOptions = "Aboveground Tank [A]," +
                                                   "Bag [I]," +
                                                   "Belowground Tank [B]," +
                                                   "Box [J]," +
                                                   "Can [F]," +
                                                   "Carboy [G]," +
                                                   "Cylinder [K]," +
                                                   "Fiberdrum [H]," +
                                                   "Glass Bottle or Jug [L]," +
                                                   "Plastic [M]," +
                                                   "Plastic or Non-Metal Drum [E]," +
                                                   "Steel Drum [D]," +
                                                   "Tank Inside Building [C]," +
                                                   "Tank Wagon [O]," +
                                                   "Tote Bin [N]";

                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, sizeNT.NodeTypeId, containerTypeNTP.PropId, true, sizeNTT.TabId );
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, sizeNT.NodeTypeId, containerTypeNTP.PropId, true, sizeNTT.TabId );
                }
            }

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema