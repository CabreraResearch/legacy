using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02F_Case30228: CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Case 30228";

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30228; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", "hidden" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodetype_props", "hidden", "Is this NodeTypeProp hidden", false, true );
            }

        }


    }//class RunBeforeEveryExecutionOfUpdater_02F_Case27883
}//namespace ChemSW.Nbt.Schema


