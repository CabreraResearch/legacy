using System.Runtime.Serialization;
using ChemSW.Grid.ExtJs;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    [DataContract]
    public class CswNbtScheduledRulesReturn: CswWebSvcReturn
    {
        /// <summary> ctor </summary>
        public CswNbtScheduledRulesReturn()
        {
            Data = new CswExtJsGrid( "ScheduledRules" );
        }//ctor

        [DataMember]
        public CswExtJsGrid Data;
        /// <summary> data </summary>

    }//CswNbtScheduledRulesReturn




} // namespace ChemSW.Nbt.WebServices
