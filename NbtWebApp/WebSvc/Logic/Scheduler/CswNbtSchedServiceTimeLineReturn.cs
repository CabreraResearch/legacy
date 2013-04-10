using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp.WebSvc.Logic.Scheduler
{
    [DataContract]
    public class CswNbtSchedServiceTimeLineReturn: CswWebSvcReturn
    {
        public TimelineData Data;
        public CswNbtSchedServiceTimeLineReturn()
        {
            Data = new TimelineData();
        }

        [DataContract]
        public class TimelineData
        {
            [DataMember]
            public Collection<Series> Series = new Collection<Series>();
        }

        [DataContract]
        public class Series
        {
            [DataMember]
            public string SchemaName = string.Empty;

            [DataMember]
            public string OpName = string.Empty;

            [DataMember]
            public Collection<DataPoint> DataPoints = new Collection<DataPoint>();
        }

        [DataContract]
        public class DataPoint
        {
            [DataMember]
            public string Start = string.Empty;

            [DataMember]
            public string End = string.Empty;

            [DataMember]
            public string StartDate = string.Empty;

            [DataMember]
            public string ExecutionTime = string.Empty;

            [DataMember]
            public bool IsNull = false;
        }

    }
}