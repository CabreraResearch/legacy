using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02D_Case30194_Pre : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Case 30194";

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30194; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass WorkUnitOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.WorkUnitClass );
            _CswNbtSchemaModTrnsctn.deleteModuleObjectClassJunction( CswEnumNbtModuleName.Containers, WorkUnitOC.ObjectClassId );
        }

    }//class RunBeforeEveryExecutionOfUpdater_02D_Case30194
}//namespace ChemSW.Nbt.Schema


