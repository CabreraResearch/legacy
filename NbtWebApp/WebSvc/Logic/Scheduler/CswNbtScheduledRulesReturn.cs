using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Grid.ExtJs;
using ChemSW.MtSched.Core;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    [DataContract]
    public class CswNbtScheduledRulesReturn: CswWebSvcReturn
    {
        /// <summary> ctor </summary>
        public CswNbtScheduledRulesReturn()
        {
            Data = new Ret();
        }//ctor

        [DataMember]
        public Ret Data;

        [DataContract]
        public class Ret
        {
            private CswCommaDelimitedString _Recurrance = null;
            public Ret()
            {
                Grid = new CswExtJsGrid( GridPrefix );
            }

            [DataMember]
            public string CustomerId = "";

            [DataMember]
            public const string GridPrefix = "ScheduledRules";

            [DataMember]
            public CswExtJsGrid Grid;

            [DataMember]
            public Collection<string> RecurrenceOptions
            {
                get
                {
                    //We don't want Always or NSeconds to be viable options for Scheduled Rules
                    _Recurrance = _Recurrance ?? new CswCommaDelimitedString();
                    //_Recurrance.Add( CswEnumRecurrence.Always );
                    _Recurrance.Add( CswEnumRecurrence.Never );
                    _Recurrance.Add( CswEnumRecurrence.Daily );
                    _Recurrance.Add( CswEnumRecurrence.DayOfMonth );
                    _Recurrance.Add( CswEnumRecurrence.DayOfWeek );
                    _Recurrance.Add( CswEnumRecurrence.DayOfYear );
                    _Recurrance.Add( CswEnumRecurrence.Hourly );
                    _Recurrance.Add( CswEnumRecurrence.NMinutes );
                    //_Recurrance.Add( CswEnumRecurrence.NSeconds );
                    return _Recurrance.ToStringCollection();
                }
                set { var disposable = value; }
            }

            [DataMember]
            public CswDictionary ColumnIds
            {
                get
                {
                    CswDictionary Ret = new CswDictionary();

                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.RuleName ).ToString(), CswEnumScheduleLogicDetailColumnNames.RuleName );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.Recurrance ).ToString(), CswEnumScheduleLogicDetailColumnNames.Recurrance );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.Interval ).ToString(), CswEnumScheduleLogicDetailColumnNames.Interval );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.ReprobateThreshold ).ToString(), CswEnumScheduleLogicDetailColumnNames.ReprobateThreshold );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.MaxRunTimeMs ).ToString(), CswEnumScheduleLogicDetailColumnNames.MaxRunTimeMs );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.Reprobate ).ToString(), CswEnumScheduleLogicDetailColumnNames.Reprobate );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.RunStartTime ).ToString(), CswEnumScheduleLogicDetailColumnNames.RunStartTime );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.RunEndTime ).ToString(), CswEnumScheduleLogicDetailColumnNames.RunEndTime );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.TotalRogueCount ).ToString(), CswEnumScheduleLogicDetailColumnNames.TotalRogueCount );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.FailedCount ).ToString(), CswEnumScheduleLogicDetailColumnNames.FailedCount );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.ThreadId ).ToString(), CswEnumScheduleLogicDetailColumnNames.ThreadId );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.StatusMessage ).ToString(), CswEnumScheduleLogicDetailColumnNames.StatusMessage );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.Priority ).ToString(), CswEnumScheduleLogicDetailColumnNames.Priority );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.LoadCount ).ToString(), CswEnumScheduleLogicDetailColumnNames.LoadCount );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.Disabled ).ToString(), CswEnumScheduleLogicDetailColumnNames.Disabled );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.HasChanged ).ToString(), CswEnumScheduleLogicDetailColumnNames.HasChanged );

                    return Ret;
                }
                set { var disposable = value; }
            }
        }

    }//CswNbtScheduledRulesReturn




} // namespace ChemSW.Nbt.WebServices
