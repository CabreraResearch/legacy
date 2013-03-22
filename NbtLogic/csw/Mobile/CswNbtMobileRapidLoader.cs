using System;
using System.IO;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Mail;

namespace ChemSW.Nbt.csw.Mobile
{
    #region Data Contract

    [DataContract]
    public class RapidLoaderData
    {
        [DataContract]
        public class RapidLoaderDataRequest
        {
            [DataMember]
            public String EmailAddress = String.Empty;
            [DataMember]
            public String CSVData = String.Empty;
        }
    }

    #endregion Data Contract

    public class CswNbtMobileRapidLoader
    {        
        #region Properties and ctor

        private CswNbtResources _CswNbtResources;
        private String _EmailBodyTemplate = "Hello {0},<br/><br/>Your ChemSW Rapid Loader Mobile import can be downloaded from:<br/><br/>{1}<br/><br/>This download link will remain available for one week.<br/><br/>-ChemSW Support";

        public CswNbtMobileRapidLoader( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        #endregion Properties and ctor

        #region Public Methods

        public void saveRapidLoaderData( RapidLoaderData.RapidLoaderDataRequest Request )
        {
            if( false == String.IsNullOrEmpty( Request.EmailAddress ) )
            {
                String FullPathName = _getFileNameAndPath();
                FileStream fs = new FileStream( FullPathName, FileMode.CreateNew );
                StreamWriter sw = new StreamWriter( fs, System.Text.Encoding.Default );
                sw.Write( Request.CSVData );
                sw.Flush();

                String EmailMessageSubject = "Your ChemSW Rapid Loader import is available for download";
                String EmailMessageBody = String.Format( 
                    _EmailBodyTemplate, 
                    _CswNbtResources.CurrentNbtUser.Username, 
                    _makeLink( FullPathName, FullPathName ) 
                    );
                _sendEmail( _CswNbtResources.CurrentNbtUser.Username, Request.EmailAddress, EmailMessageSubject, EmailMessageBody );
                _sendEmail( _CswNbtResources.CurrentNbtUser.Username, "enterprisesupport@chemsw.com", EmailMessageSubject, EmailMessageBody );
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Email Missing", "null email address" );
            }
        }

        #endregion Public Methods

        #region Private Helper Functions

        private String _getFileNameAndPath()
        {
            CswTempFile TempFileTools = new CswTempFile( _CswNbtResources );
            String TempFileName = "RL_" + _CswNbtResources.AccessId + "_" + _CswNbtResources.CurrentUser.Username + "_" + DateTime.Now.ToString( "yyyyMMdd_HHmmss" ) + ".csv";
            String TempPath = TempFileTools.TempPath;
            return TempPath + "\\" + TempFileName;
        }

        private String _makeLink( String Href, String Text )
        {
            String ret = "<a href=\"";
            if( !ret.EndsWith( "/" ) )
            {
                ret += "/";
            }
            ret += Href + "\">" + Text + "</a>";
            return ret;
        }

        private void _sendEmail( String UserName, String EmailAddress, String Subject, String MessageBody )
        {
            CswMail _CswMail = _CswNbtResources.CswMail;
            CswMailMessage MailMessage = new CswMailMessage
                {
                    Recipient = EmailAddress, 
                    RecipientDisplayName = UserName, 
                    Subject = Subject, 
                    Content = MessageBody, 
                    Format = Quiksoft.EasyMail.SMTP.BodyPartFormat.HTML
                };

            if( _CswMail.send( MailMessage ) )
            {
                _CswNbtResources.logMessage( "Rapid Loader email to " + UserName + " at " + EmailAddress + " (succeeded);" );
            }
            else
            {
                _CswNbtResources.logMessage( "Rapid Loader email to " + UserName + " at " + EmailAddress + " (failed: " + _CswMail.Status + ");" );
            }
        }

        #endregion Private Helper Functions
    }
}