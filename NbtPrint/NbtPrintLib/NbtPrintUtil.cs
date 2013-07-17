
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using ChemSW.Core;

namespace NbtPrintLib
{
    public class NbtPrintUtil
    {
        static public bool PrintLabel( string aPrinterName, string LabelData, ref string statusInfo, ref string errMsg )
        {
            bool Ret = true;
            errMsg = string.Empty;

            if( LabelData != string.Empty )
            {
                string HexStarter = "<HEX>";
                string HexEnder = "</HEX>";
                if( LabelData.Contains( HexStarter ) )
                {
                    // We have to print it as byte[], not string

                    // Convert to a set of byte[]'s
                    Collection<byte[]> PartsOfLabel = new Collection<byte[]>();
                    string currentLabelData = LabelData;

                    while( currentLabelData.Contains( HexStarter ) )
                    {
                        Int32 hexstart = currentLabelData.IndexOf( HexStarter );
                        Int32 hexend = currentLabelData.IndexOf( HexEnder );
                        string prestr = currentLabelData.Substring( 0, hexstart );
                        string hexstr = currentLabelData.Substring( hexstart + HexStarter.Length, hexend - hexstart - HexEnder.Length + 1 );
                        PartsOfLabel.Add( CswTools.StringToByteArray( prestr ) );
                        PartsOfLabel.Add( Convert.FromBase64String( hexstr ) );

                        currentLabelData = currentLabelData.Substring( hexend + HexEnder.Length + 1 );
                    }
                    PartsOfLabel.Add( CswTools.StringToByteArray( currentLabelData ) );

                    // Concatenate all parts into a single byte[]
                    Int32 newLen = 0;
                    foreach( byte[] part in PartsOfLabel )
                    {
                        newLen += part.Length;
                    }
                    byte[] entireLabel = new byte[newLen];
                    Int32 currentOffset = 0;
                    foreach( byte[] part in PartsOfLabel )
                    {
                        System.Buffer.BlockCopy( part, 0, entireLabel, currentOffset, part.Length );
                        currentOffset += part.Length;
                    }

                    //unmanaged code pointer required for the function call
                    IntPtr unmanagedPointer = Marshal.AllocHGlobal( entireLabel.Length );
                    try
                    {
                        Marshal.Copy( entireLabel, 0, unmanagedPointer, entireLabel.Length );
                        // Call unmanaged code
                        if( RawPrinterHelper.SendBytesToPrinter( aPrinterName, unmanagedPointer, entireLabel.Length ) )
                        {
                            statusInfo = "Printed " + statusInfo;
                        }
                        else
                        {
                            Ret = false;
                            errMsg = "Label printing error on client.";
                            statusInfo = "Error printing " + statusInfo;
                        }
                    }
                    finally
                    {
                        //unmanaged pointer must be explicitly released to prevent memory leak
                        Marshal.FreeHGlobal( unmanagedPointer );
                    }
                }
                else
                {
                    if( RawPrinterHelper.SendStringToPrinter( aPrinterName, LabelData ) )
                    {
                        statusInfo = "Printed " + statusInfo;
                    }
                    else
                    {
                        Ret = false;
                        errMsg = "Label printing error on client.";
                        statusInfo = "Error printing " + statusInfo;
                    }

                }
            } // if( LabelData != string.Empty )
            else
            {
                statusInfo = "No label content.";
            }
            return Ret;
        } // _printLabel()


    }
}
