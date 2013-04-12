using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Grid.ExtJs;
using ChemSW.MtSched.Core;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    [DataContract]
    public class CswNbtDemoDataReturn : CswWebSvcReturn
    {
        /// <summary> ctor </summary>
        public CswNbtDemoDataReturn()
        {
            Data = new DemoData();
        }//ctor


        [DataContract]
        public sealed class ColumnNames
        {
            public const string Name = "Name";
            public const string Type = "Type";
            public const string IsUsedBy = "Is Used By";
            public const string IsRequiredBy = "Is Requird By";
            public const string Delete = "Delete";
            public const string ConvertToDemo = "Convert To Demo";

        }//ColumnNames

        [DataContract]
        public class DemoData
        {
            private CswCommaDelimitedString _Recurrance = null;
            public DemoData()
            {
                Grid = new CswExtJsGrid( GridPrefix );
            }

            [DataMember]
            public const string GridPrefix = "DemoData";

            [DataMember]
            public CswExtJsGrid Grid;

            //[DataMember]
            //public Collection<string> RecurrenceOptions
            //{
            //    get
            //    {
            //        _Recurrance = _Recurrance ?? new CswCommaDelimitedString();
            //        _Recurrance.Add( Recurrence.Always );
            //        _Recurrance.Add( Recurrence.Never );
            //        _Recurrance.Add( Recurrence.Daily );
            //        _Recurrance.Add( Recurrence.DayOfMonth );
            //        _Recurrance.Add( Recurrence.DayOfWeek );
            //        _Recurrance.Add( Recurrence.DayOfYear );
            //        _Recurrance.Add( Recurrence.Hourly );
            //        _Recurrance.Add( Recurrence.NSeconds );
            //        return _Recurrance.ToStringCollection();
            //    }
            //    set { var disposable = value; }
            //}

            [DataMember]
            public CswDictionary ColumnIds
            {
                get
                {
                    CswDictionary Ret = new CswDictionary();

                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, ColumnNames.Name ).ToString(), ColumnNames.Name );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, ColumnNames.Type ).ToString(), ColumnNames.Type );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, ColumnNames.IsUsedBy ).ToString(), ColumnNames.IsUsedBy );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, ColumnNames.IsRequiredBy ).ToString(), ColumnNames.IsRequiredBy );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, ColumnNames.Delete ).ToString(), ColumnNames.Delete );
                    Ret.Add( new CswExtJsGridDataIndex( GridPrefix, ColumnNames.ConvertToDemo ).ToString(), ColumnNames.ConvertToDemo );

                    return Ret;
                }
                set { var disposable = value; }
            }
        }//


        [DataMember]
        public DemoData Data;

    }//CswNbtDemoDataReturn




} // namespace ChemSW.Nbt.WebServices
