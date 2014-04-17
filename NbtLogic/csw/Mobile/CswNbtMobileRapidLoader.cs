using System;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.DB;
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
            String TempFileName = "RL_" + _CswNbtResources.AccessId + "_" + _CswNbtResources.CurrentUser.Username + "_" + DateTime.Now.ToString( "yyyyMMdd_HHmmss" ) + ".csv";
            String FullPathName = _getFileNameAndPath( TempFileName );
            FileStream fs = new FileStream( FullPathName, FileMode.CreateNew );
            StreamWriter sw = new StreamWriter( fs, System.Text.Encoding.Default );
            sw.Write( Request.CSVData );
            sw.Flush();
            sw.Close();

            // CIS-53216
            // We need to hack the data coming from the client into the default 'CISPro' import format
            DataTable CsvData = CswTools.CsvToDataTable( FullPathName, true );

            // Add missing columns:
            CsvData.Columns.Add( "Department" );
            CsvData.Columns.Add( "Physical Description" );
            CsvData.Columns.Add( "expirationdate" );
            CsvData.Columns.Add( "manufacturerlotno" );
            CsvData.Columns.Add( "inventorygroupname" );
            CsvData.Columns.Add( "Unit_Weight" );
            CsvData.Columns.Add( "Unit_Volume" );
            CsvData.Columns.Add( "Unit_Each" );
            CsvData.Columns.Add( "ConvFactBase" );
            CsvData.Columns.Add( "ConvFactExp" );
            CsvData.Columns.Add( "Rolename" );

            // Rename existing columns:
            CsvData.Columns["Chemical"].ColumnName = "MaterialName";
            CsvData.Columns["Supplier"].ColumnName = "vendorname";
            CsvData.Columns["CAS No"].ColumnName = "CasNo";
            CsvData.Columns["Quantity"].ColumnName = "netquantity";
            CsvData.Columns["Units"].ColumnName = "UnitOfMeasureName";
            CsvData.Columns["Owner"].ColumnName = "responsible";
            CsvData.Columns["Barcode"].ColumnName = "barcodeid";
            CsvData.Columns["CatalogNo"].ColumnName = "catalogno";

            // Massage data:
            foreach( DataRow csvRow in CsvData.Rows )
            {
                csvRow["inventorygroupname"] = "Default Inventory Group";
                switch( csvRow["UnitType"].ToString() )
                {
                    case "WEIGHT":
                        csvRow["Unit_Weight"] = csvRow["UnitOfMeasureName"];
                        break;
                    case "VOLUME":
                        csvRow["Unit_Volume"] = csvRow["UnitOfMeasureName"];
                        break;
                    case "EACH":
                        csvRow["Unit_Each"] = csvRow["UnitOfMeasureName"];
                        break;
                }
                csvRow["ConvFactBase"] = 1;
                csvRow["ConvFactExp"] = 0;
                csvRow["Rolename"] = "cispro_general";
            }
            
            // Overwrite the existing CSV
            FileStream fs2 = new FileStream( FullPathName, FileMode.Truncate );
            StreamWriter sw2 = new StreamWriter( fs2, System.Text.Encoding.Default );
            sw2.Write( CswDataTable.ToCsv( CsvData ) );
            sw2.Flush();
            sw2.Close();

            String EmailMessageSubject = "Your ChemSW Rapid Loader import is available for download";
            String EmailMessageBody = String.Format(
                _EmailBodyTemplate,
                _CswNbtResources.CurrentNbtUser.Username,
                _makeLink( TempFileName )
                );
            if( false == String.IsNullOrEmpty( Request.EmailAddress ) )
            {
                _sendEmail( _CswNbtResources.CurrentNbtUser.Username, Request.EmailAddress, EmailMessageSubject, EmailMessageBody );
            }
            _sendEmail( _CswNbtResources.CurrentNbtUser.Username, _CswNbtResources.SetupVbls[CswEnumSetupVariableNames.SupportEmail], EmailMessageSubject, EmailMessageBody );
        }

        #endregion Public Methods

        #region Private Helper Functions

        private String _getFileNameAndPath( String TempFileName )
        {
            CswTempFile TempFileTools = new CswTempFile( _CswNbtResources );
            String TempPath = TempFileTools.TempPath;
            return TempPath + "\\" + TempFileName;
        }

        private String _makeLink( String FileName )
        {
            String Href = _CswNbtResources.SetupVbls[CswEnumSetupVariableNames.MailReportUrlStem] + "temp/" + FileName;
            String ret = "<a href=\"" + Href + "\">" + Href + "</a>";
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
                    Format = CswEnumMailMessageBodyFormat.HTML
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