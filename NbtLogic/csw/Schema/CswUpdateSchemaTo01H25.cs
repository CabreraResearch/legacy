using System;
using System.Data;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using System.IO;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-24
    /// </summary>
    public class CswUpdateSchemaTo01H25 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 25 ); } }
        public CswUpdateSchemaTo01H25( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {
            string TableName = "scheduledrules";
            _CswNbtSchemaModTrnsctn.addTable( TableName, "scheduledruleid" );
            _CswNbtSchemaModTrnsctn.addStringColumn( TableName, "rulename", "corresponds to specific rule class", false, true, 50 );
            _CswNbtSchemaModTrnsctn.addLongColumn( TableName, "maxruntimems", "maximum number of milliseconds the rule is allowed to run before it gets halted by the schedule service", false, true );
            _CswNbtSchemaModTrnsctn.addLongColumn( TableName, "threadid", "updated by system only: threadid of most recent thread in which the rule is allowed to run", false, false );
            _CswNbtSchemaModTrnsctn.addLongColumn( TableName, "reprobatethreshold", "Number of times the rule is allowed to go rogue before it is marked reprobate", false, true );
            _CswNbtSchemaModTrnsctn.addLongColumn( TableName, "totalroguecount", "updated by system only: number of times the rule exceeded its maxruntime ms -- this value is reset when reprobate is set to true by the user", false, false );
            _CswNbtSchemaModTrnsctn.addLongColumn( TableName, "failedcount", "Total number of times that the rule has failed", false, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( TableName, "reprobate", "marked true if the number of times the rule exceeded maxruntimems was greater than the repropbate threshold; user can reset this value to true to cause the rule to be run again", false, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( TableName, "disabled", "", false, false );
            _CswNbtSchemaModTrnsctn.addStringColumn( TableName, "statusmessage", "Indicates how the rule terminated the last time it was run", false, false, 512 );
            _CswNbtSchemaModTrnsctn.addStringColumn( TableName, "recurrence", "The case-insensitive values are: Never, Always, NSeconds, Hourly, Daily, DayOfWeek, DayOfMonth, DayOfYear", false, true, 512 );
            _CswNbtSchemaModTrnsctn.addLongColumn( TableName, "interval", "Frequency interpreted in terms of recurrence", false, true );
            _CswNbtSchemaModTrnsctn.addDateColumn( TableName, "runstarttime", "the time at which the rule started running in the most recent run cycle", false, false );
            _CswNbtSchemaModTrnsctn.addDateColumn( TableName, "runendtime", "the time at which the rule stopped running in the most recent run cycle", false, false );
            _CswNbtSchemaModTrnsctn.addDateColumn( TableName, "lastrun", "the date-time the rule was run -- does not imply a succesfull run", false, false );


            _CswNbtSchemaModTrnsctn.dropTable( "schedule_items" ); //did not know this existed when above table was created; whatever

        } // update()

    }//class CswUpdateSchemaTo01H25

}//namespace ChemSW.Nbt.Schema

