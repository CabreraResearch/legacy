using System.Runtime.Serialization;

namespace ChemSW.Session
{
    /// <summary>
    /// 
    /// </summary>
    public class CswNbtSessionAuthenticateData
    {
        [DataContract]
        public class Authentication
        {
            [DataContract]
            public class Request
            {
                [DataMember]
                public string UserName = "";
                [DataMember]
                public string Password = "";
                [DataMember]
                public string CustomerId = "";
                [DataMember]
                public bool IsMobile = false;

                public string IpAddress = "";
                public string CurrentViewId = "";
                public string CurrentActionName = "";
                public string SessionId = "";
            }

        }
    } // CswNbtSessionDataItem
} // namespace ChemSW.Nbt
