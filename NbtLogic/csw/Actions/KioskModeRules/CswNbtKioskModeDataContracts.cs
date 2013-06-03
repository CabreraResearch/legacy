using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ChemSW.Nbt.Actions.KioskMode
{

    [DataContract]
    public class KioskModeData
    {
        [DataMember]
        public Collection<Mode> AvailableModes = new Collection<Mode>();
        [DataMember]
        public OperationData OperationData;
    }

    [DataContract]
    public class Mode
    {
        [DataMember]
        public string name = string.Empty;
        [DataMember]
        public string imgUrl = string.Empty;
        [DataMember]
        public string applies_to_types = string.Empty;
    }

    [DataContract]
    public class ActiveMode
    {
        [DataMember]
        public string field1Name = string.Empty;
        [DataMember]
        public string field2Name = string.Empty;
    }

    [DataContract]
    public class OperationData
    {
        [DataMember]
        public string Mode = string.Empty;
        [DataMember]
        public string ModeStatusMsg = string.Empty;
        [DataMember]
        public bool ModeServerValidated = false;
        [DataMember]
        public Collection<string> Log = new Collection<string>();
        [DataMember]
        public Field Field1;
        [DataMember]
        public Field Field2;
        [DataMember]
        public string LastItemScanned;
        [DataMember]
        public string ScanTextLabel;
    }

    [DataContract]
    public class Field
    {
        [DataMember]
        public string Name = string.Empty;
        [DataMember]
        public string Value = string.Empty;
        [DataMember]
        public string StatusMsg = string.Empty;
        [DataMember]
        public bool ServerValidated = false;
        [DataMember]
        public string SecondValue = string.Empty;
        [DataMember]
        public string FoundObjClass;
        [DataMember]
        public bool Active = false;
    }
}
