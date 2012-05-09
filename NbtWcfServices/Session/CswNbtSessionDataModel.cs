
using System.Collections.ObjectModel;
using ChemSW.Exceptions;

namespace NbtWebAppServices.Session
{
    public class CswNbtSessionAuthenticationStatus
    {
        private string _AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown.ToString();
        public string AuthenticationStatus { get { return _AuthenticationStatus; } set { _AuthenticationStatus = value; } }

        private string _TimeOut = "0";
        public string TimeOut { get { return _TimeOut; } set { _TimeOut = value; } }
    }

    public class CswNbtWebServicePerformance
    {
        public double ServerInit { get; set; }
        public double DbInit { get; set; }
        public double DbQuery { get; set; }
        public double DbCommit { get; set; }
        public double DbDeinit { get; set; }
        public double TreeLoaderSql { get; set; }
        public double ServerTotal { get; set; }
    }

    public class CswNbtWebServiceErrorMessage
    {
        public string ErrorMessage = default( string );
        public string ErrorDetail = default( string );
        public ErrorType ErrorType = ErrorType.None;
        public bool DisplayError = false;
    }

    public class CswNbtWebServiceStatus
    {
        public CswNbtWebServiceStatus()
        {
            Errors = new Collection<CswNbtWebServiceErrorMessage>();
        }
        public bool Success = true;
        public Collection<CswNbtWebServiceErrorMessage> Errors { get; set; }
        public bool DisplayErrors = false;
    }

}