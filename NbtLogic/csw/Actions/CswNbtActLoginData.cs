using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Security;
using ChemSW.WebSvc;

namespace ChemSW.Nbt.Actions
{
    #region DataContract

    [DataContract]
    public class LoginData
    {
        public LoginData()
        {
            Logins = new Collection<Login>();
        }

        [DataMember]
        public Collection<Login> Logins;

        [DataContract]
        public class Login
        {
            /// <summary>
            /// Default ctr for WCF
            /// </summary>
            public Login()
            {
                
            }

            /// <summary>
            /// Init Login Data with the current Authentication Request
            /// </summary>
            public Login( CswWebSvcSessionAuthenticateData.Authentication.Request Request, CswNbtObjClassUser User = null )
            {
                AuthenticationRequest = Request;
                Username = AuthenticationRequest.UserName;
                IPAddress = AuthenticationRequest.IpAddress;
                setStatus( Request.AuthenticationStatus, User );
            }

            [DataMember]
            public String Username = String.Empty;
            [DataMember]
            public String IPAddress = String.Empty;
            [DataMember]
            public String LoginDate = String.Empty;
            [DataMember]
            public String LoginStatus = String.Empty;
            [DataMember]
            public String FailureReason = String.Empty;
            [DataMember]
            public Int32 FailedLoginCount = 0;  

            public CswWebSvcSessionAuthenticateData.Authentication.Request AuthenticationRequest;

            private void setStatus( CswEnumAuthenticationStatus Status, CswNbtObjClassUser User = null )
            {
                LoginStatus = Status == CswEnumAuthenticationStatus.Authenticated ? "Success" : "Failed";
                FailureReason = CswEnumAuthenticationStatus.EuphamizedText[Status];
                if( Status == CswEnumAuthenticationStatus.Failed )
                {
                    FailureReason = null == User ? "Unknown Username" : "Bad Password";
                }
            }
        }

        [DataContract]
        public class LoginDataRequest
        {
            [DataMember]
            public String StartDate = String.Empty;
            [DataMember]
            public String EndDate = String.Empty;
        }
    }

    #endregion DataContract

    public class CswNbtActLoginData
    {
        #region Properties and ctor

        private CswNbtResources _CswNbtResources;
        private LoginData Data;

        public CswNbtActLoginData( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            Data = new LoginData();
        }

        #endregion Properties and ctor

        #region Public Methods

        public LoginData getLoginData( LoginData.LoginDataRequest Request )
        {
            DataTable LoginTable = _getLoginRecords( Request );
            foreach( DataRow LoginDataRow in LoginTable.Rows )
            {
                LoginData.Login LoginRecord = new LoginData.Login
                {
                    Username = LoginDataRow["username"].ToString(),
                    IPAddress = LoginDataRow["ipaddress"].ToString(),
                    LoginDate = LoginDataRow["logindate"].ToString(),
                    LoginStatus = LoginDataRow["loginstatus"].ToString(),
                    FailureReason = LoginDataRow["failurereason"].ToString(),
                    FailedLoginCount = CswConvert.ToInt32( LoginDataRow["failedlogincount"] )
                };
                Data.Logins.Add( LoginRecord );
            }
                                        
            return Data;
        }

        public void postLoginData( LoginData.Login LoginRecord )
        {
            if( null == LoginRecord.AuthenticationRequest.Parameters ||
                false != LoginRecord.AuthenticationRequest.Parameters.IsIncludedInLoginData )
            {
                CswTableUpdate LoginData = _CswNbtResources.makeCswTableUpdate( "Login Data Insert", "login_data" );
                DataTable LoginDataTable = LoginData.getTable();
                DataRow LoginRow = LoginDataTable.NewRow();
                LoginRow[ "username" ] = LoginRecord.Username;
                LoginRow[ "ipaddress" ] = LoginRecord.IPAddress;
                LoginRow[ "logindate" ] = LoginRecord.LoginDate;
                LoginRow[ "loginstatus" ] = LoginRecord.LoginStatus;
                LoginRow[ "failurereason" ] = LoginRecord.FailureReason;
                LoginRow[ "failedlogincount" ] = LoginRecord.FailedLoginCount;
                LoginDataTable.Rows.Add( LoginRow );
                LoginData.update( LoginDataTable );
            }
        }

        #endregion Public Methods

        #region Private Methods

        private DataTable _getLoginRecords( LoginData.LoginDataRequest Request )
        {
            Int32 RowLimit = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.treeview_resultlimit.ToString() ) );
            RowLimit = ( RowLimit > 0 ) ? RowLimit : 250;

            String WhereClauseTemplate = @"where logindate >= {0} and logindate < {1} + 1";
            String WhereClause = String.Format( WhereClauseTemplate,
                _CswNbtResources.getDbNativeDate( DateTime.Parse( Request.StartDate ) ),
                _CswNbtResources.getDbNativeDate( DateTime.Parse( Request.EndDate ) )
            );
            
            CswTableSelect LoginDataSelect = _CswNbtResources.makeCswTableSelect( "Login_Data Select", "login_data" );
            DataTable TargetTable = LoginDataSelect.getTable( WhereClause: WhereClause, PageLowerBoundExclusive: 0, PageUpperBoundInclusive: RowLimit, 
                //Yuck.
                RequireOneRow: false, OrderByColumns: null, FilterColumn: null, FilterValue: Int32.MinValue, SelectColumns: null  );
            return TargetTable;
        }

        #endregion Private Methods
    }
}
