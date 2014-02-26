using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    public class CswUpdateMetaData_02K_Case31672: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 31672; }
        }

        public override string Title
        {
            get { return "Fix OraViewColName for anything created in new design mode"; }
        }

        public override void update()
        {
            CswTableUpdate NTPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "update_nt_oraview_names", "nodetype_props" );
            DataTable NTPTable = NTPUpdate.getTable("where oraviewcolname is null");
            foreach( DataRow Prop in NTPTable.Rows )
            {
                Prop["oraviewcolname"] = CswFormat.MakeOracleCompliantIdentifier( Prop["propname"].ToString() );
            }
            NTPUpdate.update( NTPTable );
        } // update()

    }//class CswUpdateMetaData_02K_Case31672

}//namespace ChemSW.Nbt.Schema