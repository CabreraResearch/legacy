using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Actions
{
    [DataContract]
    public class ContainerData
    {
        public ContainerData()
        {
            ContainerStatistics = new Collection<ReconciliationStatistics>();
            ContainerStatuses = new Collection<ReconciliationStatuses>();
        }

        [DataMember]
        public Collection<ReconciliationStatistics> ContainerStatistics;
        [DataMember]
        public Collection<ReconciliationStatuses> ContainerStatuses;

        [DataContract]
        public class ReconciliationStatistics
        {
            [DataMember]
            public String Status = String.Empty;
            [DataMember]
            public Int32 ContainerCount = Int32.MinValue;
            [DataMember]
            public Int32 AmountScanned = Int32.MinValue;
            [DataMember]
            public Double PercentScanned = Double.NaN;
        }

        [DataContract]
        public class ReconciliationStatuses
        {
            [DataMember]
            public String ContainerId = String.Empty;
            [DataMember]
            public String ContainerBarcode = String.Empty;
            [DataMember]
            public String ContainerStatus = String.Empty;
            [DataMember]
            public String Action = String.Empty;
            [DataMember]
            public String ActionApplied = String.Empty;
        }

        [DataContract]
        public class ReconciliationRequest
        {
            [DataMember]
            public String LocationId = String.Empty;
            [DataMember]
            public bool IncludeChildLocations = false;
            [DataMember]
            public String StartDate = String.Empty;
            [DataMember]
            public String EndDate = String.Empty;
        }

    } // ContainerData
}
