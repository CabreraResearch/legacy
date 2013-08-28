
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_03 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Pre-Script: 03: Audit Columns"; } }
        public override void update()
        {
            // this should always be here, and always be last, and always in its own script
            // see case 21989 and 26011
            _CswNbtSchemaModTrnsctn.makeMissingAuditTablesAndColumns();
        } 

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 21989; }
        }

        //Update()

    }//class RunBeforeEveryExecutionOfUpdater_03

}//namespace ChemSW.Nbt.Schema


