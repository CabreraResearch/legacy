using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Grid.ExtJs;
using ChemSW.MtSched.Core;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    [DataContract]
    public class CswNbtDemoDataRequests 
    {
        [DataContract]
        public class CswDemoNodesGridRequest
        {
            [DataMember] public List<string> NodeIds;

        }//class: DemoNodesGridRequest

    }//CswNbtDemoDataRequests




} // namespace ChemSW.Nbt.WebServices
