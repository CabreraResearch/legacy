using System.Collections.ObjectModel;
using System.Runtime.Serialization;
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
            Data = new Collection<CswScheduleLogicDetail>();
        }//ctor

        [DataMember]
        public Collection<CswScheduleLogicDetail> Data;
        /// <summary> data </summary>

    }//CswNbtScheduledRulesReturn




} // namespace ChemSW.Nbt.WebServices
