﻿using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27578
    /// </summary>
    public class CswUpdateSchema_01S_Case27578 : CswUpdateSchemaTo
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

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27578; }
        }

        //Update()

    }

}//namespace ChemSW.Nbt.Schema