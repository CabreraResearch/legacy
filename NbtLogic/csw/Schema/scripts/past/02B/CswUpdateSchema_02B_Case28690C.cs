using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28690
    /// </summary>
    public class CswUpdateSchema_02B_Case28690C : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28690; }
        }

        private CswNbtMetaDataObjectClass NonChemicalOC;

        public override void update()
        {
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            NonChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.NonChemicalClass );
            CswNbtMetaDataNodeType SupplyNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Supply" );
            CswNbtMetaDataNodeType BiologicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Biological" );
            Int32 SupplyId = 0, BiologicalId = 0;
            if( null != SupplyNT )
            {
                SupplyId = SupplyNT.NodeTypeId;
            }
            if( null != BiologicalNT )
            {
                BiologicalId = BiologicalNT.NodeTypeId;
            }
            //Change Supply and Biological's OC to NonChemical
            CswTableUpdate NTUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28690_nt_update", "nodetypes" );
            DataTable NTTable = NTUpdate.getTable( "where objectclassid = " + MaterialOC.ObjectClassId );
            foreach( DataRow NTRow in NTTable.Rows )
            {
                if( NTRow["nodetypeid"].ToString() == SupplyId.ToString() || NTRow["nodetypeid"].ToString() == BiologicalId.ToString() )
                {
                    NTRow["objectclassid"] = NonChemicalOC.ObjectClassId;
                }
            }
            NTUpdate.update( NTTable );
        } // update()

    }//class CswUpdateSchema_02B_Case28690C
}//namespace ChemSW.Nbt.Schema