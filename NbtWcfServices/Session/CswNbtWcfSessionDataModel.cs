
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Exceptions;

namespace NbtWebAppServices.Session
{
    [DataContract]
    public class CswNbtSessionAuthenticationStatus
    {
        private string _AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown.ToString();
        [DataMember]
        public string AuthenticationStatus { get { return _AuthenticationStatus; } set { _AuthenticationStatus = value; } }

        private string _TimeOut = "0";
        [DataMember]
        public string TimeOut { get { return _TimeOut; } set { _TimeOut = value; } }
    }

    [DataContract]
    public class CswNbtWebServicePerformance
    {
        [DataMember]
        public double ServerInit { get; set; }
        [DataMember]
        public double DbInit { get; set; }
        [DataMember]
        public double DbQuery { get; set; }
        [DataMember]
        public double DbCommit { get; set; }
        [DataMember]
        public double DbDeinit { get; set; }
        [DataMember]
        public double TreeLoaderSql { get; set; }
        [DataMember]
        public double ServerTotal { get; set; }
    }

    [DataContract]
    public class CswNbtWebServiceErrorMessage
    {
        [DataMember]
        public string ErrorMessage = default( string );
        [DataMember]
        public string ErrorDetail = default( string );
        [DataMember]
        public ErrorType ErrorType = ErrorType.None;
        [DataMember]
        public bool DisplayError = false;
    }

    [DataContract]
    public class CswNbtWebServiceStatus
    {
        public CswNbtWebServiceStatus()
        {
            Errors = new Collection<CswNbtWebServiceErrorMessage>();
        }
        [DataMember]
        public bool Success = true;
        [DataMember]
        public Collection<CswNbtWebServiceErrorMessage> Errors { get; set; }
        [DataMember]
        public bool DisplayErrors = false;
    }

}