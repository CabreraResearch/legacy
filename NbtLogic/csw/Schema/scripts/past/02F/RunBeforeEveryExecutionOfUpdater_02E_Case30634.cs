using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02E_Case30634 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Case 30634";

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30634; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.changeColumnDataType( "TIER2", "CASNO", CswEnumDataDictionaryPortableDataType.String, 255 );
        }

    }//class RunBeforeEveryExecutionOfUpdater_02E_Case30634
}//namespace ChemSW.Nbt.Schema


