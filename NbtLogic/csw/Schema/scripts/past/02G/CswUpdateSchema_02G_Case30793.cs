using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30793 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {                
            get { return 30793; }
        }

        public override string ScriptName
        {
            get { return "02G_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Update JCT10 index"; }
        }

        public override void update()
        {

            _CswNbtSchemaModTrnsctn.updateIndex( "jct_nodes_props", "nodeid, nodetypepropid, field1, field2, field3, field1_numeric, field1_date, hidden","JCT10" );

        } // update()

    } // class CswUpdateSchema_02G_Case30793

}//namespace ChemSW.Nbt.Schema