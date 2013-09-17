using System;
using ChemSW.Encryption;
using Microsoft.Win32;

namespace NbtPrintLib
{
    [Serializable()]
    public class NbtPrintClientConfig
    {
        public PrinterSetupDataCollection printers = null;
        public string accessid;
        public string logon;
        public string password;
        public string url;
        public bool serviceMode;
        public string logMessages;

        public NbtPrintClientConfig()
        {

            printers = new PrinterSetupDataCollection();
            logMessages = string.Empty;
        }


        public void SaveToReg( RegistryKey rootKey )
        {
            CswEncryption _CswEncryption = new CswEncryption( string.Empty );
            _CswEncryption.Method = EncryptionMethod.TypeZero;

            RegistryKey akey = rootKey.OpenSubKey( "printers", true );
            if( akey == null )
            {
                akey = rootKey.CreateSubKey( "printers" );
            }
            printers.SaveToReg( printers, akey );

            if( false == string.IsNullOrEmpty( accessid ) )
                rootKey.SetValue( "accessid", accessid );
            if( false == string.IsNullOrEmpty( logon ) )
                rootKey.SetValue( "logon", logon );
            if( false == string.IsNullOrEmpty( serviceMode.ToString() ) )
                rootKey.SetValue( "serviceMode", serviceMode.ToString() );

            String pwd = password;
            if( pwd.Length > 0 )
            {
                pwd = _CswEncryption.encrypt( pwd );
                rootKey.SetValue( "password", pwd, Microsoft.Win32.RegistryValueKind.String );
            }
            if( string.IsNullOrEmpty( url ) )
            {
                url = "https://imcslive.chemswlive.com/Services/"; //the default server
            }
            rootKey.SetValue( "serverurl", url );

        }

        public void LoadFromReg( RegistryKey rootKey )
        {
            CswEncryption _CswEncryption = new CswEncryption( string.Empty );
            _CswEncryption.Method = EncryptionMethod.TypeZero;

            try
            {

                accessid = rootKey.GetValue( "accessid" ).ToString();
                logon = rootKey.GetValue( "logon" ).ToString();
                String pwd = rootKey.GetValue( "password" ).ToString();
                pwd = pwd.Replace( "\0", string.Empty );
                if( pwd.Length > 4 )
                {
                    password = _CswEncryption.decrypt( pwd );
                }
                url = rootKey.GetValue( "serverurl" ).ToString();
                if( url == string.Empty )
                {
                    url = "https://imcslive.chemswlive.com/Services/"; //the default server
                }

                //Log( "Loaded settings." );
                serviceMode = ( rootKey.GetValue( "serviceMode" ).ToString().ToLower() == "true" );
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
            }

        }

    }

}
