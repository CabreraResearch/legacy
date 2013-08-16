using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02F_Case30252 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Case 30252";

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.DH; }
        }

        public override int CaseNo
        {
            get { return 30252; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.addStringColumn( "object_class", "oraviewname", "stable oracle dbview name for this object class", false, false, 30 );
            _CswNbtSchemaModTrnsctn.addStringColumn( "object_class_props", "oraviewcolname", "stable oracle dbview column name for this object class property", false, false, 30 );
            _CswNbtSchemaModTrnsctn.addStringColumn( "nodetypes", "oraviewname", "stable oracle dbview name for this nodetype", false, false, 30 );
            _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_props", "oraviewcolname", "stable oracle dbview column name for this nodetype property", false, false, 30 );

        }

    }//class RunBeforeEveryExecutionOfUpdater_02F_Case30252
}//namespace ChemSW.Nbt.Schema


