using System;
using System.Data;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Sched;
using System.IO;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-26
    /// </summary>
    public class CswUpdateSchemaTo01H27 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 27 ); } }
        public CswUpdateSchemaTo01H27( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {

            string TableNameRules = "scheduledrules";
            _CswNbtSchemaModTrnsctn.addTable( TableNameRules, NbtScheduledRuleColumns.ScheduledRuleId.ToString() );
            _CswNbtSchemaModTrnsctn.addStringColumn( TableNameRules, NbtScheduledRuleColumns.RuleName.ToString(), "corresponds to specific rule class", false, true, 50 );
            _CswNbtSchemaModTrnsctn.addLongColumn( TableNameRules, NbtScheduledRuleColumns.MaxRunTimeMs.ToString(), "maximum number of milliseconds the rule is allowed to run before it gets halted by the schedule service", false, true );
            _CswNbtSchemaModTrnsctn.addLongColumn( TableNameRules, NbtScheduledRuleColumns.ThreadId.ToString(), "updated by system only: threadid of most recent thread in which the rule is allowed to run", false, false );
            _CswNbtSchemaModTrnsctn.addLongColumn( TableNameRules, NbtScheduledRuleColumns.ReprobateThreshold.ToString(), "Number of times the rule is allowed to go rogue before it is marked reprobate", false, true );
            _CswNbtSchemaModTrnsctn.addLongColumn( TableNameRules, NbtScheduledRuleColumns.TotalRogueCount.ToString(), "updated by system only: number of times the rule exceeded its maxruntime ms -- this value is reset when reprobate is set to true by the user", false, false );
            _CswNbtSchemaModTrnsctn.addLongColumn( TableNameRules, NbtScheduledRuleColumns.FailedCount.ToString(), "Total number of times that the rule has failed", false, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( TableNameRules, NbtScheduledRuleColumns.Reprobate.ToString(), "marked true if the number of times the rule exceeded maxruntimems was greater than the repropbate threshold; user can reset this value to true to cause the rule to be run again", false, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( TableNameRules, NbtScheduledRuleColumns.Disabled.ToString(), "", false, false );
            _CswNbtSchemaModTrnsctn.addStringColumn( TableNameRules, NbtScheduledRuleColumns.StatusMessage.ToString(), "Indicates how the rule terminated the last time it was run", false, false, 512 );
            _CswNbtSchemaModTrnsctn.addStringColumn( TableNameRules, NbtScheduledRuleColumns.Recurrence.ToString(), "The case-insensitive values are: Never, Always, NSeconds, Hourly, Daily, DayOfWeek, DayOfMonth, DayOfYear", false, true, 512 );
            _CswNbtSchemaModTrnsctn.addLongColumn( TableNameRules, NbtScheduledRuleColumns.Interval.ToString(), "Frequency interpreted in terms of recurrence", false, true );
            _CswNbtSchemaModTrnsctn.addDateColumn( TableNameRules, NbtScheduledRuleColumns.RunStartTime.ToString(), "the time at which the rule started running in the most recent run cycle", false, false );
            _CswNbtSchemaModTrnsctn.addDateColumn( TableNameRules, NbtScheduledRuleColumns.RunEndTime.ToString(), "the time at which the rule stopped running in the most recent run cycle", false, false );
            _CswNbtSchemaModTrnsctn.addDateColumn( TableNameRules, NbtScheduledRuleColumns.LastRun.ToString(), "the date-time the rule was run -- does not imply a succesfull run", false, false );

            string TableNameScheduleRuleParams = "scheduledruleparams";
            _CswNbtSchemaModTrnsctn.addTable( TableNameScheduleRuleParams, NbtScheduledRuleParamsColumns.ScheduledRuleParamId.ToString() );
            _CswNbtSchemaModTrnsctn.addForeignKeyColumn( TableNameScheduleRuleParams, NbtScheduledRuleColumns.ScheduledRuleId.ToString(), "foreign key", false, false, TableNameRules, NbtScheduledRuleColumns.ScheduledRuleId.ToString() );
            _CswNbtSchemaModTrnsctn.addStringColumn( TableNameScheduleRuleParams, NbtScheduledRuleParamsColumns.ParamName.ToString(), "name of parameter", false, false, 50 );
            _CswNbtSchemaModTrnsctn.addStringColumn( TableNameScheduleRuleParams, NbtScheduledRuleParamsColumns.ParamVal.ToString(), "value of parameter", false, false, 250 );


            _CswNbtSchemaModTrnsctn.dropTable( "schedule_items" ); //did not know this existed when above table was created; whatever

        } // update()

    }//class CswUpdateSchemaTo01H27

}//namespace ChemSW.Nbt.Schema

