
using Microsoft.Win32;

namespace NbtPrintLib
{
    public class PrinterSetupData
    {
        public string LPCname;
        public string PrinterName;
        public bool Enabled;
        public string PrinterKey;
        public string Description;
        public string Message;
        public bool Succeeded;
        public bool working;

        public PrinterSetupData()
        {
            Enabled = false;
            working = false;
            PrinterKey = string.Empty;
            LPCname = string.Empty;
            Description = string.Empty;
        }

        public bool isRegistered()
        {
            return ( PrinterKey != string.Empty );
        }


        public void LoadFromReg( RegistryKey akey )
        {
            LPCname = akey.GetValue( "LPCname" ).ToString();
            Enabled = ( akey.GetValue( "Enabled" ).ToString().ToLower() == "true" );
            PrinterName = akey.GetValue( "printer" ).ToString();
            PrinterKey = akey.GetValue( "printerkey" ).ToString();
            Description = akey.GetValue( "description" ).ToString();
        }

        public void SaveToReg( RegistryKey akey )
        {
            akey.SetValue( "LPCname", LPCname );
            akey.SetValue( "Enabled", Enabled.ToString() );
            akey.SetValue( "printer", PrinterName );
            akey.SetValue( "printerkey", PrinterKey );
            akey.SetValue( "description", Description );
        }



    }
}
