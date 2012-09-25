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
                [DataMember( IsRequired = true )]
                public string UserName = "";
                [DataMember( IsRequired = true )]
                public string Password = "";
                [DataMember( IsRequired = true )]
                public string CustomerId = "";
                [DataMember( EmitDefaultValue = false, IsRequired = false )]
                public bool IsMobile = false;

                public string IpAddress = "";
                public string CurrentViewId = "";
                public string CurrentActionName = "";
                public string SessionId = "";
                public bool IsValid()
                {
                    return false == string.IsNullOrEmpty( Password ) &&
                        false == string.IsNullOrEmpty( CustomerId ) &&
                        false == string.IsNullOrEmpty( UserName );
                }
            }


            [DataContract]
            public class Response
            {
                private string _AuthenticationStatus = Security.AuthenticationStatus.Unknown;

                [DataMember]
                public string AuthenticationStatus
                {
                    get { return _AuthenticationStatus; }
                    set { _AuthenticationStatus = value; }
                }

                private string _TimeOut = "0";

                [DataMember]
                public string TimeOut
                {
                    get { return _TimeOut; }
                    set { _TimeOut = value; }
                }

                [DataMember]
                public Expired ExpirationReset = null;

                [DataContract]
                public class Expired
                {
                    [DataMember]
                    public string UserId = "";
                    [DataMember]
                    public string UserKey = "";
                    [DataMember]
                    public string PasswordId = "";
                    [DataMember]
                    public string NewPassword = "";
                }
            }
        }
    } // CswNbtSessionDataItem
} // namespace ChemSW.Nbt
