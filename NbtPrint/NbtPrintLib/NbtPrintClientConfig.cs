using System;
using ChemSW.Encryption;
using Microsoft.Win32;

namespace NbtPrintLib
{

    public class NbtPrintClientConfig
    {
        public PrinterSetupDataCollection printers = null;
        public string accessid;
        public string logon;
        public string password;
        public string url;
        public bool enabled;
        public string logMessages;

        public NbtPrintClientConfig()
        {

            printers = new PrinterSetupDataCollection();
            logMessages = string.Empty;
        }


        public void SaveSettings( RegistryKey rootKey )
        {
            CswEncryption _CswEncryption = new CswEncryption( string.Empty );
            _CswEncryption.Method = EncryptionMethod.TypeZero;

            RegistryKey akey = rootKey.OpenSubKey( "printers", true );
            if( akey == null )
            {
                akey = rootKey.CreateSubKey( "printers" );
            }
            printers.SaveToReg( printers, akey );

            rootKey.SetValue( "accessid", accessid );
            rootKey.SetValue( "logon", logon );
            rootKey.SetValue( "enabled", enabled.ToString() );
            String pwd = password;
            if( pwd.Length > 0 )
            {
                pwd = _CswEncryption.encrypt( pwd );
            }
            rootKey.SetValue( "password", pwd, Microsoft.Win32.RegistryValueKind.String );
            if( url == string.Empty )
            {
                url = "https://imcslive.chemswlive.com/Services/"; //the default server
            }
            rootKey.SetValue( "serverurl", url );
        }

        public void LoadSettings( RegistryKey rootKey )
        {
            CswEncryption _CswEncryption = new CswEncryption( string.Empty );
            _CswEncryption.Method = EncryptionMethod.TypeZero;

            try
            {

                accessid = rootKey.GetValue( "accessid" ).ToString();
                logon = rootKey.GetValue( "logon" ).ToString();
                String pwd = rootKey.GetValue( "password" ).ToString();
                pwd = pwd.Replace( "\0", string.Empty );
                try
                {
                    password = _CswEncryption.decrypt( pwd );
                }
                catch( Exception e )
                {
                    password = "";
                }
                url = rootKey.GetValue( "serverurl" ).ToString();
                if( url == string.Empty )
                {
                    url = "https://imcslive.chemswlive.com/Services/"; //the default server
                }

                //Log( "Loaded settings." );
                enabled = ( rootKey.GetValue( "Enabled" ).ToString().ToLower() == "true" );
                /*
                            if( true != enabled )
                            {
                                logMessages = "Print jobs are not enabled, see Setup tab.";
                            }
                            else
                            {
                                timer1.Enabled = true;
                                lblStatus.Text = "Waiting...";
                            } */
                try
                {
                    RegistryKey akey = rootKey.OpenSubKey( "printers", true );
                    if( akey == null )
                    {
                        akey = rootKey.CreateSubKey( "printers" );
                    }
                    printers.LoadFromReg( printers, akey );
                }
                catch( Exception e )
                {
                    logMessages = "Missing or invalid printer configuration(s). " + e.Message;
                }
            }
            catch( Exception )
            {
                logMessages = "No configuration data found.";
                enabled = false;
            }

        }

    }

}
