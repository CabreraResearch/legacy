using System.Runtime.Serialization;

namespace NbtWebApp.WebSvc.Returns
{
    /// <summary>
    /// Base DataContracts for all web service returns
    /// </summary>
    public class CswWebSvcReturnError
    {
        [DataContract]
        public enum FaultCode
        {
            [EnumMember]
            ERROR,
            [EnumMember]
            INCORRECT_PARAMETER
        }

        [DataContract]
        public class FaultException
        {
            [DataMember]
            public FaultCode ErrorCode;
            [DataMember]
            public string Message;
            [DataMember]
            public string Details;
        }
    }
}