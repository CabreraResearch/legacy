using System.Runtime.Serialization;
using NbtWebApp.Actions.Receiving;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtWebServiceReceiving
    {
        [DataContract]
        public class CswNbtReceivingDefinitonReturn: CswWebSvcReturn
        {
            public CswNbtReceivingDefinitonReturn()
            {
                Data = new CswNbtReceivingDefiniton();
            }
            [DataMember]
            public CswNbtReceivingDefiniton Data;
        }

        public static void ReceiveMaterial( ICswResources CswResources, CswNbtReceivingDefinitonReturn Response, CswNbtReceivingDefiniton ReceivingDefiniton )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswNbtActReceiving ActReceiving = new CswNbtActReceiving( NbtResources, ReceivingDefiniton.MaterialNodeId );
            JObject ActionData=  ActReceiving.receiveMaterial( ReceivingDefiniton );
            Response.Data.ActionData = ActionData.ToString();
        }

    }
}
