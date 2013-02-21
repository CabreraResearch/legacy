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
                    _Recurrance = _Recurrance ?? new CswCommaDelimitedString();
                    _Recurrance.Add( Recurrence.Always );
                    _Recurrance.Add( Recurrence.Never );
                    _Recurrance.Add( Recurrence.Daily );
                    _Recurrance.Add( Recurrence.DayOfMonth );
                    _Recurrance.Add( Recurrence.DayOfWeek );
                    _Recurrance.Add( Recurrence.DayOfYear );
                    _Recurrance.Add( Recurrence.Hourly );
                    _Recurrance.Add( Recurrence.NSeconds );
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

                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswScheduleLogicDetail.ColumnNames.RuleName ).ToString(), CswScheduleLogicDetail.ColumnNames.RuleName );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswScheduleLogicDetail.ColumnNames.Recurrance ).ToString(), CswScheduleLogicDetail.ColumnNames.Recurrance );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswScheduleLogicDetail.ColumnNames.Interval ).ToString(), CswScheduleLogicDetail.ColumnNames.Interval );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswScheduleLogicDetail.ColumnNames.ReprobateThreshold ).ToString(), CswScheduleLogicDetail.ColumnNames.ReprobateThreshold );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswScheduleLogicDetail.ColumnNames.MaxRunTimeMs ).ToString(), CswScheduleLogicDetail.ColumnNames.MaxRunTimeMs );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswScheduleLogicDetail.ColumnNames.Reprobate ).ToString(), CswScheduleLogicDetail.ColumnNames.Reprobate );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswScheduleLogicDetail.ColumnNames.RunStartTime ).ToString(), CswScheduleLogicDetail.ColumnNames.RunStartTime );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswScheduleLogicDetail.ColumnNames.RunEndTime ).ToString(), CswScheduleLogicDetail.ColumnNames.RunEndTime );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswScheduleLogicDetail.ColumnNames.TotalRogueCount ).ToString(), CswScheduleLogicDetail.ColumnNames.TotalRogueCount );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswScheduleLogicDetail.ColumnNames.FailedCount ).ToString(), CswScheduleLogicDetail.ColumnNames.FailedCount );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswScheduleLogicDetail.ColumnNames.ThreadId ).ToString(), CswScheduleLogicDetail.ColumnNames.ThreadId );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswScheduleLogicDetail.ColumnNames.StatusMessage ).ToString(), CswScheduleLogicDetail.ColumnNames.StatusMessage );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, CswScheduleLogicDetail.ColumnNames.HasChanged ).ToString(), CswScheduleLogicDetail.ColumnNames.HasChanged );

                    return Ret;
                }
                set { var disposable = value; }
            }
        }

    }//CswNbtScheduledRulesReturn




} // namespace ChemSW.Nbt.WebServices
