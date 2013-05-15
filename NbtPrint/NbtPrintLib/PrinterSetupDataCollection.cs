
using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace NbtPrintLib
{
    public class PrinterSetupDataCollection : Collection<PrinterSetupData>
    {
        public void SaveToReg( PrinterSetupDataCollection me, RegistryKey regKey )
        {
            int idx = 0;
            string[] keys = regKey.GetSubKeyNames();
            foreach( string akey in keys )
            {
                regKey.DeleteSubKey( akey );
            }

            foreach( PrinterSetupData aprinter in me )
            {
                RegistryKey akey = regKey.CreateSubKey( idx.ToString() );
                aprinter.SaveToReg( akey );
                ++idx;
            }

        }

        public void LoadFromReg( PrinterSetupDataCollection me, RegistryKey regKey )
        {
            me.Clear();
            foreach( string keyName in regKey.GetSubKeyNames() )
            {
                PrinterSetupData aprinter = new PrinterSetupData();
                RegistryKey akey = regKey.OpenSubKey( keyName );
                aprinter.LoadFromReg( akey );
                me.Add( aprinter );
            }
        }
    }
}