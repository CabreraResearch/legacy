using System;
using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28053
    /// </summary>
    public class CswUpdateSchema_01W_Case28053 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 28053; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType MaterialDocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Document" );
            if( null != MaterialDocumentNT )
            {
                CswNbtMetaDataNodeTypeProp OwnerNTP = MaterialDocumentNT.getNodeTypeProp( "Owner" );
                if( null != OwnerNTP )
                {
                    // Change prop name
                    OwnerNTP.PropName = "Tradename";

                    // Change view name
                    Int32 NodeViewId = OwnerNTP.ViewId.get();
                    CswTableUpdate NodeViewsTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updateViewNameForMaterialDocNT", "node_views" );
                    DataTable NodeViewsDt = NodeViewsTU.getTable( "where nodeviewid = " + NodeViewId );
                    foreach( DataRow CurrentRow in NodeViewsDt.Rows )
                    {
                        CurrentRow["viewname"] = "Tradename";
                    }

                    NodeViewsTU.update( NodeViewsDt );

                }
            }

        } //Update()

    }//class CswUpdateSchema_01V_Case28053

}//namespace ChemSW.Nbt.Schema