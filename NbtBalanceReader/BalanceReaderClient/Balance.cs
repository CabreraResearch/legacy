using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BalanceReaderClient.NbtPublic;

namespace BalanceReaderClient
{

    class Balance
    {
        private SerialPort _balanceConnection;
        private SerialBalance _balanceData;
        private BalanceConfiguration _serialPortConfiguration;
        private string _readBuffer;

        private bool _isEnabled;
        private bool _updateInProgress;
        private bool _stableReadsOnly;
        private double _driftThreshold;
        private int _pendingReadRequests;

        public DataGridViewRow InterfaceRow;
        public NbtAuth AuthenticationClient;


        public Balance(string COM, string Manufacturer, string Description )
        {
            _isEnabled = false;
            _updateInProgress = false;
            _stableReadsOnly = false;

            _balanceData = new SerialBalance();
            _balanceData.NbtName = "";
            _balanceData.CurrentWeight = 0;
            _balanceData.UnitOfMeasurement = "";
            _balanceData.Manufacturer = Manufacturer;
            _balanceData.DeviceDescription = Description;
            _balanceData.Operational = false;

            _driftThreshold = 0;
            _pendingReadRequests = 0;

            _serialPortConfiguration = new BalanceConfiguration();

            _balanceConnection = new SerialPort();
            _balanceConnection.PortName = COM;

            _balanceConnection.DataReceived += parseWeightResult;
            
        }//constructor


        public void Dispose()
        {
            _balanceConnection.Close();
            _balanceConnection.Dispose();
        }



        /// <summary>
        /// The BalanceAsynchronousTask is a delegate wrapper for long-running balance tasks.
        /// Instances of this delegate should be called with BeginInvoke.
        /// </summary>
        public delegate void BalanceAsynchronousTask();


        /// <summary>
        /// Sends a read request to the Balance
        /// </summary>
        public void performBalanceCheck()
        {
            if( _isEnabled && false == String.IsNullOrEmpty( _serialPortConfiguration.RequestFormat ) && false == _updateInProgress )
            {
                if( false == _balanceConnection.IsOpen )
                {
                    _balanceConnection.Open();
                }
                _balanceConnection.Write( _serialPortConfiguration.RequestFormat );
                _pendingReadRequests = _pendingReadRequests + 1;
            }
            
        }//performBalanceCheck()



        /// <summary>
        /// Announces Balance to NBT
        /// </summary>
        public void announceBalance( )
        {
            if( _isEnabled )
            {
                NbtAuth.SessionCompleteEvent SendBalanceToServer = delegate( NbtPublicClient Client, string StatusText )
                    {
                        if( StatusText == "Authenticated" )
                        {
                            Client.UpdateBalanceData( _balanceData );
                        }
                    };

                AuthenticationClient.PerformAction( SendBalanceToServer );

            }

        }//announceBalance()




        /// <summary>
        /// Updates the Balance's weight properties. If changed, sends updates to NBT
        /// </summary>
        private void parseWeightResult( object Sender, SerialDataReceivedEventArgs Data )
        {

         //since this is being handled asynchronously, we want to ensure that only one balance message is being sent at a time
            if( false == _updateInProgress )
            {
                _updateInProgress = true;

                Regex ResultParser = new Regex( _serialPortConfiguration.ResponseFormat );

                //pull the new data from the serial port into the internal buffer, then split the internal buffer on newlines
                  _readBuffer += _balanceConnection.ReadExisting();
                  string[] NewLine = {"\r\n"};
                  string[] ReadLines = _readBuffer.Split( NewLine, StringSplitOptions.None );

                //put all the trailing data back into the buffer
                  _readBuffer = ReadLines[ReadLines.Length - 1];

                  if( ReadLines.Length > 1 )
                  {//if there is a complete response
                      //get the last valid response on the array
                      string Result = ReadLines[ReadLines.Length - 2];

                      if( ResultParser.IsMatch( Result ) )
                      {
                          _pendingReadRequests = 0;
                          _balanceData.Operational = true;

                          Match ParsedResult = ResultParser.Match( Result );

                          double NewWeight;
                          double.TryParse( ParsedResult.Groups["weight"].Value, out NewWeight );
                          string NewUnits = ParsedResult.Groups["unit"].Value;
                          string StableReadCharacter = ParsedResult.Groups["stable"].Value;

                          //if one of the weight values changed
                          if( Math.Abs( NewWeight - _balanceData.CurrentWeight ) >= _driftThreshold
                              || ParsedResult.Groups["unit"].Value != _balanceData.UnitOfMeasurement )
                          {
                              //if the read is stable or we're accepting unstable reads
                              if( false == _stableReadsOnly || "S" == StableReadCharacter || " " == StableReadCharacter )
                              {
                                  _balanceData.CurrentWeight = NewWeight;
                                  _balanceData.UnitOfMeasurement = NewUnits;
                                  announceBalance();
                                  InterfaceRow.Cells["CurrentWeight"].Value = NewWeight + NewUnits;
                              }
                          } //if( Math.Abs( NewWeight - _balanceData.CurrentWeight ) > _driftThreshold
                      } //if ( ResultParser.IsMatch( Result )
                      else if( _balanceData.Operational && _pendingReadRequests >= 10 )
                      {
                          _balanceData.Operational = false;
                          announceBalance();
                      }

                  } //if ( ReadLines.Length > 1 ) 
                  else if( _balanceData.Operational && _pendingReadRequests >= 10 )
                  {
                      _balanceData.Operational = false;
                      announceBalance();
                  }
            }//if( false == _updateInProgress )

            _updateInProgress = false;


        }//parseWeightResult()




        /// <summary>
        /// Update Balance properties from data changed in grid
        /// </summary>
        /// <param name="Field">The column name of the parameter changed</param>
        /// <param name="Value">The value of the column's cell</param>
        public void updateField( string Field, object Value )
        {
            switch( Field )
            {
                case "NBTName":
                    _balanceData.NbtName = (string) Value;
                    break;

                case "StableOnly":
                    _stableReadsOnly = (bool) Value;
                    break;

                case "Enabled":
                    if( true == (bool) Value )
                    {
                        BalanceAsynchronousTask EnableBalance = delegate()
                            {
                                if( false == _balanceConnection.IsOpen )
                                {
                                    if( string.IsNullOrEmpty( _balanceData.NbtName ) )
                                    {

                                        InterfaceRow.Cells["Enabled"].Value = false;
                                        InterfaceRow.Cells["NBTName"].Selected = true;
                                    }
                                    else if( string.IsNullOrEmpty( _balanceData.Configuration ) )
                                    {
                                        InterfaceRow.Cells["Enabled"].Value = false;
                                        InterfaceRow.Cells["Configuration"].Selected = true;
                                    }
                                    else
                                    {
                                        _isEnabled = true;

                                        InterfaceRow.Cells["NBTName"].ReadOnly = true;
                                        InterfaceRow.Cells["NBTName"].Style.BackColor = Color.LightGray;
                                        InterfaceRow.Cells["Configuration"].ReadOnly = true;
                                        InterfaceRow.Cells["Configuration"].Style.BackColor = Color.LightGray;

                                        _balanceConnection.Open();
                                    }
                                }
                                announceBalance();
                            };
                        
                        EnableBalance.BeginInvoke( null, null );
                    }//if true == Value
                    else
                    {
                        BalanceAsynchronousTask DisableBalance = delegate()
                            {
                                _isEnabled = false;

                                InterfaceRow.Cells["NBTName"].ReadOnly = false;
                                InterfaceRow.Cells["NBTName"].Style.BackColor = InterfaceRow.DefaultCellStyle.BackColor;
                                InterfaceRow.Cells["Configuration"].ReadOnly = false;
                                InterfaceRow.Cells["Configuration"].Style.BackColor = InterfaceRow.DefaultCellStyle.BackColor;

                                _balanceConnection.Close();

                            };
                        DisableBalance.BeginInvoke( null, null );

                    }//else -- if true == Value
                    break;

                case "Configuration":
                    _serialPortConfiguration = (BalanceConfiguration) Value;

                    _balanceConnection.BaudRate = _serialPortConfiguration.BaudRate;
                    _balanceConnection.DataBits = _serialPortConfiguration.DataBits;
                    _balanceConnection.Handshake = (Handshake) Enum.Parse( typeof( Handshake ), _serialPortConfiguration.Handshake );
                    _balanceConnection.Parity = (Parity) Enum.Parse( typeof( Parity ), _serialPortConfiguration.ParityBit );
                    _balanceConnection.StopBits = (StopBits) Enum.Parse( typeof( StopBits ), _serialPortConfiguration.StopBits );

                    _balanceData.Configuration = _serialPortConfiguration.Name;

                    break;

                case "DriftThreshold":

                    if ( false == double.TryParse( (string) Value, out _driftThreshold ) )
                    {//when the user entered an invalid threshold, clear the field
                        InterfaceRow.Cells["DriftThreshold"].Value = "";
                    }
                    break;

            }//switch( Field )
        }//updateField()



        /// <summary>
        /// Serialize the balance as a string. Used in saving Balance data to file.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _balanceConnection.PortName + "|" + _balanceData.DeviceDescription + "|" + _balanceData.NbtName + "|" + _stableReadsOnly.ToString() + "|" + _driftThreshold + "|" + _serialPortConfiguration.Name;

        }



        /// <summary>
        /// If the string data has the same hardware description as the current Balance, updates the Balance's values to the stored values
        /// </summary>
        /// <param name="SerializedElements">An array of data from a tokenized string formatted according to ToString()</param>
        /// <param name="Configurations">A dictionary of configurations which could possibly be applied to the balance</param>
        /// <returns type="bool">returns true when file is outdated, otherwise false</returns>
        public bool FromString(string[] SerializedElements, Dictionary<string,BalanceConfiguration> Configurations )
        {
            bool IsOutdated = false;

            if( 6 != SerializedElements.Length )
            {
                IsOutdated = true;
            }
            else if( _balanceData.DeviceDescription == SerializedElements[1] )
            {//this data is for the same hardware, perform update

                    InterfaceRow.Cells["NBTName"].Value = SerializedElements[2];
                    updateField( "NBTName", SerializedElements[2] );
                    InterfaceRow.Cells["StableOnly"].Value = bool.Parse( SerializedElements[3] );
                    updateField( "StableOnly", bool.Parse( SerializedElements[3] ) );
                    InterfaceRow.Cells["DriftThreshold"].Value = SerializedElements[4];
                    updateField( "DriftThreshold", SerializedElements[4] );
                if ( Configurations.ContainsKey( SerializedElements[5] ) )
                {
                    InterfaceRow.Cells["Configuration"].Value = Configurations[ SerializedElements[5] ];
                    updateField( "Configuration", Configurations[ SerializedElements[5] ] );
                }

            }//else if ( _balanceData.DeviceDescription == SerializedElements[1] )

            return IsOutdated;

        }//FromString()



    }//class Balance
}//namespce BalanceReaderClient
