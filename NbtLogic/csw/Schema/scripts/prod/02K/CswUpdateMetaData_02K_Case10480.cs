using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    public class CswUpdateMetaData_02K_Case10480: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 10480; }
        }

        public override string Title
        {
            get { return "Winter is coming."; }
        }

        public override void update()
        {
            CswTableUpdate NTUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "update_ntnames", "nodetypes" );
            DataTable NTTable = NTUpdate.getTable();
            foreach( DataRow NTRow in NTTable.Rows )
            {
                NTRow["nodetypename"] = CswFormat.MakeIntoValidName( NTRow["nodetypename"].ToString() );
            }
            NTUpdate.update( NTTable );
        } // update()

    }//class CswUpdateMetaData_02K_Case10480

}//namespace ChemSW.Nbt.Schema