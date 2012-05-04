
using ChemSW.Exceptions;

namespace NbtWebAppServices.Session
{
    public class CswSessionRequest
    {
        public string Password { get; set; }
        public string CustomerId { get; set; }
        public string UserName { get; set; }
        public bool IsMobile { get; set; }
    }

    public class CswSessionAuthentication
    {
        private string _AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown.ToString();
        public string AuthenticationStatus { get { return _AuthenticationStatus; } set { _AuthenticationStatus = value; } }

        private string _TimeOut = "0";
        public string TimeOut { get { return _TimeOut; } set { _TimeOut = value; } }

        private CswPerfTimer _CswPerfTimer = new CswPerfTimer();
        public CswPerfTimer CswPerfTimer { get { return _CswPerfTimer; } set { _CswPerfTimer = value; } }
    }

    public class CswPerfTimer
    {
        public double ServerInit { get; set; }
        public double DbInit { get; set; }
        public double DbQuery { get; set; }
        public double DbCommit { get; set; }
        public double DbDeinit { get; set; }
        public double TreeLoaderSql { get; set; }
        public double ServerTotal { get; set; }
    }

    public class CswError
    {
        public readonly bool Success = false;
        public string Message;
        public string Detail;
        public ErrorType Type;
        public bool Display;
    }

}