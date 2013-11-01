using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30794 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 30794; }
        }

        public override string ScriptName
        {
            get { return "02G_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Lowercase gestaltsearch"; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update jct_nodes_props set gestaltsearch = lower(gestaltsearch)" );
        } // update()

    } // CswUpdateSchema_02G_Case30794

}//namespace ChemSW.Nbt.Schema