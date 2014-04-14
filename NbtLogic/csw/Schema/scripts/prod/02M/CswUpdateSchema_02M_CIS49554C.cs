using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateSchema_02M_CIS49554C : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE; }
        }

        public override int CaseNo
        {
            get { return 49554; }
        }

        public override string Title
        {
            get { return "delete unused config vars brand_pagetitle and brand_pageicon" + CaseNo; }
        }

        public override string AppendToScriptName()
        {
            return "C";
        }

        public override void update()
        {
            //constants
            const string brand_pagetitle = "brand_pagetitle";
            const string brand_pageicon = "brand_pageicon";

            _CswNbtSchemaModTrnsctn.deleteConfigurationVariable( brand_pagetitle );
            _CswNbtSchemaModTrnsctn.deleteConfigurationVariable( brand_pageicon );

        }
    }
}