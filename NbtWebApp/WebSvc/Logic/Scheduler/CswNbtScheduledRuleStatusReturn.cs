using System.Runtime.Serialization;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    [DataContract]
    public class CswNbtScheduledRuleStatusReturn: CswWebSvcReturn
    {
        public CswNbtScheduledRuleStatusReturn()
        {
            Data = new ScheduledRuleStatus();
        }

        [DataMember]
        public ScheduledRuleStatus Data;

        [DataContract]
        public class ScheduledRuleStatus
        {
            [DataMember]
            public string RuleStatus = "";
        }
    }
}
