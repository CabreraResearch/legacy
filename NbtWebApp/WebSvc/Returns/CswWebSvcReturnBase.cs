using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW;
using ChemSW.Exceptions;
using Newtonsoft.Json.Linq;

namespace NbtWebApp.WebSvc.Returns
{
    /// <summary>
    /// Base DataContracts for all web service returns
    /// </summary>
    public class CswWebSvcReturnBase
    {
        /// <summary>
        /// Contract for Authentication status
        /// </summary>
        [DataContract]
        public class Authentication
        {
            private string _AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown.ToString();

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
        }

        /// <summary>
        /// Contract for communicating performance data surrounding this request
        /// </summary>
        [DataContract]
        public class Performance
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

            public JObject ToJObject()
            {
                return new JObject
                {
                    new JProperty( "serverinit", ServerInit ),
                    new JProperty( "dbinit", DbInit ),
                    new JProperty( "dbquery", DbQuery ),
                    new JProperty( "dbcommit", DbCommit ),
                    new JProperty( "dbdeinit", DbDeinit ),
                    new JProperty( "treeloadersql", TreeLoaderSql ),
                    new JProperty( "servertotal", ServerTotal )
                };
            }

        }

        /// <summary>
        /// Contract for specifying Log options
        /// </summary>
        [DataContract]
        public class Logging
        {
            [DataMember]
            public string LogLevel = "";
            [DataMember]
            public string CustomerId = "";
            [DataMember]
            public string Server = "";
            [DataMember]
            public string LogglyInput = CswResources.CswLogglyVenue;

            public JObject ToJObject()
            {
                return new JObject
                {
                    new JProperty( "LogLevel", LogLevel ),
                    new JProperty( "CustomerId", CustomerId ),
                    new JProperty( "Server", Server ),
                };
            }
        }

        /// <summary>
        /// Contract for constructing an error message to send to the client
        /// </summary>
        [DataContract]
        public class ErrorMessage
        {
            [DataMember]
            public string Message = default( string );
            [DataMember]
            public string Detail = default( string );
            [DataMember]
            public ErrorType Type = ErrorType.None;
            [DataMember]
            public bool ShowError = false;

            public JObject ToJObject()
            {
                return new JObject
                {
                    new JProperty( "Message", Message ),
                    new JProperty( "Detail", Detail ),
                    new JProperty( "Type", Type ),
                    new JProperty( "Display", ShowError )
                };
            }
        }

        /// <summary>
        /// Contract for defining status of the request and communicating error(s) to the client
        /// </summary>
        [DataContract]
        public class Status
        {
            /// <summary>
            ///  
            /// </summary>
            public Status()
            {
                Errors = new Collection<ErrorMessage>();
            }

            [DataMember]
            public bool Success = true;

            [DataMember]
            public Collection<ErrorMessage> Errors { get; set; }

            [DataMember]
            public bool DisplayErrors = false;
        }

    }
}