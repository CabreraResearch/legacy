using System;
using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case31907: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31907; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override string Title
        {
            get { return "Fill Tier 2 Range Codes Table"; }
        }

        public override void update()
        {
            CswTableUpdate T2RangeCodesUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "t2_range_codes_update", "tier2_rangecodes" );
            DataTable RangeCodesTable = T2RangeCodesUpdate.getEmptyTable();
            _addRangeCode( RangeCodesTable, "01", 0, 99 );
            _addRangeCode( RangeCodesTable, "02", 100, 499 );
            _addRangeCode( RangeCodesTable, "03", 500, 999 );
            _addRangeCode( RangeCodesTable, "04", 1000, 4999 );
            _addRangeCode( RangeCodesTable, "05", 5000, 9999 );
            _addRangeCode( RangeCodesTable, "06", 10000, 24999 );
            _addRangeCode( RangeCodesTable, "07", 25000, 49999 );
            _addRangeCode( RangeCodesTable, "08", 50000, 74999 );
            _addRangeCode( RangeCodesTable, "09", 75000, 99999 );
            _addRangeCode( RangeCodesTable, "10", 100000, 499999 );
            _addRangeCode( RangeCodesTable, "11", 500000, 999999 );
            _addRangeCode( RangeCodesTable, "12", 1000000, 9999999 );
            _addRangeCode( RangeCodesTable, "13", 10000000, Int32.MaxValue - 1 );
            T2RangeCodesUpdate.update( RangeCodesTable );
        } // update()


        private void _addRangeCode( DataTable RangeCodesTable, string RangeCode, int LowerBound, int UpperBound )
        {
            DataRow RangeCodesRow = RangeCodesTable.NewRow();
            RangeCodesRow["range_code"] = RangeCode;
            RangeCodesRow["lower_bound"] = LowerBound;
            RangeCodesRow["upper_bound"] = UpperBound;
            RangeCodesTable.Rows.Add( RangeCodesRow );
        }
    }

}//namespace ChemSW.Nbt.Schema