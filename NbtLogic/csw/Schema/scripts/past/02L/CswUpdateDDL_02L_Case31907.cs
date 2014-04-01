using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateDDL_02L_Case31907 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31907; }
        }

        public override string Title
        {
            get { return "Tier II Range Codes"; }
        }

        public override void update()
        {
            const string T2RangeCodesTableName = "tier2_rangecodes";
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( T2RangeCodesTableName ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( T2RangeCodesTableName, "tier2_rangecode_id" );

                _CswNbtSchemaModTrnsctn.addStringColumn( T2RangeCodesTableName, "range_code", "The range code to be entered for Tier II reporting", true, 100 );
                _CswNbtSchemaModTrnsctn.addNumberColumn( T2RangeCodesTableName, "lower_bound", "The lower bound quantity for the range code", true );
                _CswNbtSchemaModTrnsctn.addNumberColumn( T2RangeCodesTableName, "upper_bound", "The upper bound quantity for the range code", true );
            }
        }
    }
}


