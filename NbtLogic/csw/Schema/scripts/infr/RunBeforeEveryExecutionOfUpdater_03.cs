
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_03 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Audit Columns";

        public override void update()
        {
            // this should always be here, and always be last, and always in its own script
            // see case 21989 and 26011
            _CswNbtSchemaModTrnsctn.makeMissingAuditTablesAndColumns();

            string QueryIdUniqueConstraint = _CswNbtSchemaModTrnsctn.getUniqueConstraintName( "static_sql_selects", "queryid" );
            if( string.IsNullOrEmpty( QueryIdUniqueConstraint ) )
            {
                _CswNbtSchemaModTrnsctn.makeUniqueConstraint( "static_sql_selects", "queryid" );
            }
        } 

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        //Update()

    }//class RunBeforeEveryExecutionOfUpdater_03

}//namespace ChemSW.Nbt.Schema


