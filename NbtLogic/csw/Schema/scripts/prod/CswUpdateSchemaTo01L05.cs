using ChemSW.MtSched.Core;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-05
    /// </summary>
    public class CswUpdateSchemaTo01L05 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 05 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 24415

            _CswNbtSchemaModTrnsctn.createScheduledRule( NbtScheduleRuleNames.DisableChemSwAdmin, Recurrence.Daily, 1 );

            #endregion Case 24415

        }//Update()

    }//class CswUpdateSchemaTo01L05

}//namespace ChemSW.Nbt.Schema


