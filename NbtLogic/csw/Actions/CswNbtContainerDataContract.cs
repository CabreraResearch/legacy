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
            UnscannedLocations = new Collection<UnscannedLocation>();
        }

        #region Reconciliation

        [DataMember] 
        public Int32 OutstandingActionsCount = 0;
        [DataMember]
        public Collection<ReconciliationStatistics> ContainerStatistics;
        [DataMember]
        public Collection<ReconciliationStatuses> ContainerStatuses;
        [DataMember]
        public Collection<UnscannedLocation> UnscannedLocations;

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
            public Double PercentScanned = 0.0;
        }

        [DataContract]
        public class UnscannedLocation
        {
            [DataMember]
            public String LocationId = String.Empty;
            [DataMember]
            public String Name = String.Empty;
            [DataMember]
            public String Path = String.Empty;
            [DataMember]
            public String Link = String.Empty;
            [DataMember]
            public bool AllowInventory = true;
        }

        [DataContract]
        public class ReconciliationStatuses
        {
            public ReconciliationStatuses()
            {
                ActionOptions = new Collection<String>();
            }

            [DataMember]
            public String ContainerId = String.Empty;
            [DataMember]
            public String ContainerBarcode = String.Empty;
            [DataMember]
            public String PriorLocation = String.Empty;
            [DataMember]
            public String ScannedLocation = String.Empty;
            [DataMember]
            public String LocationId = String.Empty;
            [DataMember]
            public String ContainerLocationId = String.Empty;
            [DataMember]
            public String ContainerStatus = String.Empty;
            [DataMember]
            public String ScanDate = String.Empty;
            [DataMember]
            public String Action = String.Empty;
            [DataMember]
            public String Completed = String.Empty;
            [DataMember]
            public Collection<String> ActionOptions;
        }

        [DataContract]
        public class ReconciliationRequest
        {
            public ReconciliationRequest()
            {
                ContainerActions = new Collection<ReconciliationActions>();
                ContainerLocationTypes = new Collection<ReconciliationTypes>();
            }

            [DataMember]
            public String LocationId = String.Empty;
            [DataMember]
            public bool IncludeChildLocations = false;
            [DataMember]
            public String StartDate = String.Empty;
            [DataMember]
            public String EndDate = String.Empty;
            [DataMember]
            public Collection<ReconciliationActions> ContainerActions;
            [DataMember]
            public Collection<ReconciliationTypes> ContainerLocationTypes;
        }

        [DataContract]
        public class ReconciliationActions
        {
            [DataMember]
            public String ContainerId = String.Empty;
            [DataMember]
            public String ContainerLocationId = String.Empty;
            [DataMember]
            public String LocationId = String.Empty;
            [DataMember]
            public String Action = String.Empty;
        }

        [DataContract]
        public class ReconciliationTypes
        {
            [DataMember]
            public String Type = String.Empty;
            [DataMember]
            public bool Enabled = true;
        }

        #endregion Reconciliation

        #region Receive Material

        [DataContract]
        public class ReceivingData
        {
            [DataMember]
            public String ContainerProps = String.Empty;
        }

        [DataContract]
        public class ReceiptLotRequest
        {
            [DataMember]
            public String ReceiptLotId = String.Empty;
            [DataMember]
            public String ReceiptLotProps = String.Empty;
            [DataMember]
            public String ContainerId = String.Empty;
        }

        #endregion Receive Material

    } // ContainerData
}
