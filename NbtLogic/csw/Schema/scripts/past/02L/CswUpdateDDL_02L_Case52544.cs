using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateDDL_02L_Case52544 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 52544; }
        }

        public override string Title
        {
            get { return "Remove filter column"; }
        }

        public override void update()
        {
            if( _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", "filter" ) )
            {
                _CswNbtSchemaModTrnsctn.dropColumn( "nodetype_props", "filter" );
            }
        }

    }//class CswUpdateDDL_02K_Case52544
}//namespace ChemSW.Nbt.Schema