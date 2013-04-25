using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp.WebSvc.Logic.Scheduler
{
    [DataContract]
    public class CswNbtSchedServiceTimeLineRequest
    {
        [DataMember]
        public string FilterSchemaTo = string.Empty;

        [DataMember]
        public string FilterOpTo = string.Empty;

        [DataMember]
        public string FilterStartTimeTo = string.Empty;

        [DataMember]
        public string FilterEndTimeTo = string.Empty;

        [DataMember]
        public string SelectedLogFile = string.Empty;
    }

    [DataContract]
    public class CswNbtSchedServiceTimeLineReturn: CswWebSvcReturn
    {
        [DataMember]
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

            [DataMember]
            public FilterData FilterData = new FilterData();
        }
    }

    [DataContract]
    public class Series
    {
        [DataMember]
        public string SchemaName = string.Empty;

        [DataMember]
        public string OpName = string.Empty;

        [DataMember]
        public string ErrorMsg = string.Empty;

        public int SeriesNo = -1;

        [DataMember]
        public Collection<DataPoint> DataPoints = new Collection<DataPoint>();

        [DataMember]
        public Collection<Collection<double>> data = new Collection<Collection<double>>(); //for Flot

        [DataMember]
        public string label = string.Empty; //for Flot

        [DataMember]
        public string color = ""; //for Flot (hex color as string)
    }

    [DataContract]
    public class DataPoint
    {
        [DataMember]
        public double Start = double.MinValue;

        [DataMember]
        public double End = double.MinValue;

        [DataMember]
        public string StartDate = string.Empty;

        [DataMember]
        public double ExecutionTime = double.MinValue;
    }

    [DataContract]
    public class FilterData
    {
        [DataMember]
        public Collection<FilterOption> Schema = new Collection<FilterOption>();

        [DataMember]
        public Collection<FilterOption> Operations = new Collection<FilterOption>();

        [DataMember]
        public Collection<string> LogFiles = new Collection<string>();

        [DataMember]
        public string DefaultStartTime = string.Empty;

        [DataMember]
        public string DefaultEndTime = string.Empty;

        [DataMember]
        public string DefaultStartDay = string.Empty;

        [DataMember]
        public string DefaultEndDay = string.Empty;

        [DataContract]
        public class FilterOption
        {
            [DataMember]
            public string text = string.Empty;
            [DataMember]
            public string value = string.Empty;
        }
    }

}