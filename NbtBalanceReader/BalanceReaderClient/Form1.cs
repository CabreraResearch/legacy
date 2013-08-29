using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BalanceReaderClient.NbtPublic;

namespace BalanceReaderClient
{
    public partial class Form1: Form
    {


        private Dictionary<string, Balance> _balanceList;
        private Dictionary<string, BalanceConfiguration> _configurationList; 
        private Timer _balancePollTimer;
        private Timer _announceBalanceTimer;
        private NbtAuth _authenticationClient;
        private static string ConfigPath = Path.GetDirectoryName( Application.ExecutablePath ) + "/BalanceReaderClient.cfg";

        public Form1()
        {
            InitializeComponent();
            

            _balanceList = new Dictionary<string, Balance>();
            _configurationList = new Dictionary<string, BalanceConfiguration>();

            HardwareGrid.CurrentCellDirtyStateChanged += handleCheckboxUpdate;
            HardwareGrid.CellValueChanged += handleGridUpdate;
            //allow comboboxes to be opened with only one click instead of two
               HardwareGrid.EditMode = DataGridViewEditMode.EditOnEnter;

            ConfigurationsGrid.CellClick += handleConfigurationCellSelected;

            _authenticationClient = new NbtAuth();


            
            constructPollTimer(500); //set a default timer for updating balances

            _announceBalanceTimer = new Timer();
            _announceBalanceTimer.Interval = 600000;
            _announceBalanceTimer.Tick += announceBalances;
            _announceBalanceTimer.Start();

         //event handlers for keeping the _authenticationClient object up to date with user-entered values 
            AccessIdField.TextChanged += ( Sender, Args ) => { _authenticationClient.AccessId = AccessIdField.Text;  };
            UsernameField.TextChanged += ( Sender, Args ) => { _authenticationClient.UserId = UsernameField.Text; };
            PasswordField.TextChanged += ( Sender, Args ) => { _authenticationClient.Password = PasswordField.Text; };
            AddressField.TextChanged  += ( Sender, Args ) => { _authenticationClient.baseURL = AddressField.Text; };
            

       //populate the dropdown menus
            Dictionary<string, string> ExampleResponseDictionary = new Dictionary<string, string>()
                {
                    {"S N 00100.000 g", "S (?<stable>[SMDN])\\s+(?<weight>[\\d.]+)\\s+(?<unit>\\w+)"}, //  Mettler: All
                    {"00100.00 grams?", "(?<weight>[\\d.]+)\\s+(?<unit>[\\w ]{5})(?<stable>[? ])"},    //    Ohaus: TP/TS series
                    {"00100.00 grm ? G", "(?<weight>[\\d.]+)\\s+(?<unit>\\w+)\\s??(?<stable>[? ])"},   //    Ohaus: CW-11, various others
                    {"+   100.00 grm","(?<weight>[\\d.]+)\\s+(?<unit>\\w+)"},                          //Sartorius: All

                    {"Custom", "Custom"}
                };
            templateResponseBox.DataSource = new BindingSource( ExampleResponseDictionary, null );
            templateResponseBox.DisplayMember = "Key";
            templateResponseBox.ValueMember = "Value";
            templateResponseBox.SelectedIndexChanged += exampleResponseSelected;

            Dictionary<string, Enum> StopBitsDictionary = new Dictionary<string, Enum>
                {
                    {"1", System.IO.Ports.StopBits.One}, 
                    {"1.5", System.IO.Ports.StopBits.OnePointFive}, 
                    {"2", System.IO.Ports.StopBits.Two}
                };
            templateStopBitsBox.DataSource = new BindingSource( StopBitsDictionary, null );
            templateStopBitsBox.DisplayMember = "Key";
            templateStopBitsBox.ValueMember = "Value";

            
            Dictionary<string, Enum> ParityBitsDictionary = new Dictionary<string, Enum>
                {
                    {"None", Parity.None}, 
                    {"Even", Parity.Even}, 
                    { "Odd", Parity.Odd },
                    {"Mark", Parity.Mark}
                };
            templateParityBitBox.DataSource = new BindingSource( ParityBitsDictionary, null );
            templateParityBitBox.DisplayMember = "Key";
            templateParityBitBox.ValueMember = "Value";

            Dictionary<string, Enum> HandshakeDictionary = new Dictionary<string, Enum>
                {
                    {"None", System.IO.Ports.Handshake.None},
                    {"RTS", System.IO.Ports.Handshake.RequestToSend},
                    {"XonXoff", System.IO.Ports.Handshake.XOnXOff},
                    {"RTS/XonXoff", System.IO.Ports.Handshake.RequestToSendXOnXOff}
                };
            templateHandshakeBox.DataSource = new BindingSource( HandshakeDictionary, null );
            templateHandshakeBox.DisplayMember = "Key";
            templateHandshakeBox.ValueMember = "Value";


            templateDataBitsBox.Items.AddRange( new object[] {5, 6, 7, 8, 9 });
            templateBaudRateBox.Items.AddRange( new object[] {110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000 });


            HardwareGrid.DataError += ( Sender, Args ) => { };//a DataError handler must be assigned when working with comboBoxes in DataGridViews


        //save settings when the form is closed
            this.FormClosing += saveUserSettings;

            loadUserSettings();

        //perform loading functions that require a windowHandler
            this.Load += formLoaded;

        }//constructor()

        /// <summary>
        /// Attached to form's Load event.
        /// Performs loading operations which require the constructor to be completed (usually because they use the form's WindowHandle)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="E"></param>
        private void formLoaded( object sender, EventArgs E )
        {
            refreshConfigurationList();
            refreshHardwareList();
        }


        /// <summary>
        /// Stores user-entered values to BalanceReaderClient.cfg 
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="E"></param>
        public void saveUserSettings( object Sender, EventArgs E )
        {

            string[] ConfigurationFileLines = new string[5 + _balanceList.Count];
            ConfigurationFileLines[0] = _authenticationClient.AccessId;
            ConfigurationFileLines[1] = _authenticationClient.UserId;
            ConfigurationFileLines[2] = _authenticationClient.Password;
            ConfigurationFileLines[3] = _authenticationClient.baseURL;
            ConfigurationFileLines[4] = pollingFrequencyField.Value.ToString();

            int lineNumber = 5;
            foreach( Balance Balance in _balanceList.Values )
            {
                ConfigurationFileLines[lineNumber] = Balance.ToString();
                lineNumber = lineNumber + 1;
            }

            
            File.WriteAllLines( ConfigPath, ConfigurationFileLines );

        }//saveUserSettings()




        /// <summary>
        /// Reads BalanceReaderClient.cfg, and sets state accordingly
        /// </summary>
        public void loadUserSettings()
        {
            if( File.Exists( ConfigPath ) )
            {
                string[] ConfigurationFileLines = File.ReadAllLines( ConfigPath );

                AccessIdField.Text = ConfigurationFileLines[0];
                UsernameField.Text = ConfigurationFileLines[1];
                PasswordField.Text = ConfigurationFileLines[2];
                AddressField.Text = ConfigurationFileLines[3];
                pollingFrequencyField.Value = Decimal.Parse( ConfigurationFileLines[4] );
                _authenticationClient.AccessId = ConfigurationFileLines[0];
                _authenticationClient.UserId = ConfigurationFileLines[1];
                _authenticationClient.Password = ConfigurationFileLines[2];
                _authenticationClient.baseURL = ConfigurationFileLines[3];
                constructPollTimer( int.Parse( ConfigurationFileLines[4] ) );


                //all the remaining lines in the configuration file contain balance data, which is handled by the Balances
                for( int line = 5; line < ConfigurationFileLines.Length; line++ )
                {
                    string[] balanceDetails = ConfigurationFileLines[line].Split( '|' );
                    string COM = balanceDetails[0];
                    
                    if( _balanceList.ContainsKey( COM ) )
                    {
                      //set a current cell to avoid a crash from the handleGridUpdate event, then apply data to Balance
                         HardwareGrid.BeginInvoke( (Action) ( () => { HardwareGrid.CurrentCell = _balanceList[COM].InterfaceRow.Cells["FriendlyName"]; } ) );

                        bool fileOutdated = _balanceList[COM].FromString( balanceDetails, _configurationList );

                        if( fileOutdated )
                        {
                            File.Delete( ConfigPath );
                        }

                     //unset current cell to avoid having a highlighted region that the user did not click
                        HardwareGrid.BeginInvoke( (Action) ( () => { HardwareGrid.CurrentCell = null; } ) );

                    } //if ( File.Exists( "BalanceReaderClient.cfg" ) )
                } //for ( int line = 5; line < ConfigurationFileLines.Length; line++ )

            }//if File.Exists
        }//loadUserSettings()



        /// <summary>
        /// Query OS for available balances, then construct a Balance object and interface row for each 
        /// </summary>
        public void refreshHardwareList()
        {

            //Deconstruct old balances and clear the interface list
               foreach( string BalanceName in _balanceList.Keys )
               {
                   HardwareGrid.Rows.Remove( _balanceList[BalanceName].InterfaceRow );
                   _balanceList[BalanceName].Dispose();
               }

               _balanceList.Clear();

            ManagementObjectSearcher DeviceScanner = new ManagementObjectSearcher("\\root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ConfigManagerErrorCode = 0");

            foreach( ManagementObject Device in DeviceScanner.Get() )
            {
                string Manufacturer = (string) Device["Manufacturer"];
                string Details = (string) Device["Caption"];
                Regex ComMatcher = new Regex( @"\((COM\d+)\)" ); //TODO: maybe a stronger filter that removes other non-balance COM devices?

                
                if ( ComMatcher.IsMatch( Details ) )
                {
                    string ComPort = ComMatcher.Match( Details ).Groups[1].Value; //extract just COM# from the Device["Caption"] string

                    _balanceList.Add( ComPort, new Balance( ComPort, Manufacturer, Details ) );
                    
                    int RowNum = HardwareGrid.Rows.Add( ComPort, Details, "", null, false, false, "0", "---" );//create a row in the UI table for the device

                 //give the Balance a reference to its own UI row and the authentication data needed for announcing the balance
                    _balanceList[ComPort].InterfaceRow = HardwareGrid.Rows[RowNum];
                    _balanceList[ComPort].AuthenticationClient = _authenticationClient; 

                }//if ( ComMatcher.IsMatch( Details ) )

            }//foreach( ManagementObject Device in DeviceScanner.Get() )

            //load any of the data that was saved and survived the transfer
               loadUserSettings();

        }//refreshHardwareList()



        /// <summary>
        /// Query NBT for a list of balance configurations already defined on the server
        /// </summary>
        public void refreshConfigurationList()
        {


            //define how to update UI when the authentication attempt resolves
            NbtAuth.SessionCompleteEvent fetchConfigurations = delegate( NbtPublicClient Client, string StatusText )
            {
                if( "Authenticated" == StatusText )
                {
                    Dictionary<string, BalanceConfiguration> NewConfigurationList = new Dictionary<string, BalanceConfiguration>();

                    CswNbtBalanceReturn ReceivedConfigurations = Client.ListBalanceConfigurations();
                    foreach( BalanceConfiguration Item in ReceivedConfigurations.Data.ConfigurationList )
                    {
                        NewConfigurationList.Add( Item.Name, Item );
                    }

                    if( false == NewConfigurationList.SequenceEqual( _configurationList ) )
                    {//only refresh the grid and pick lists if the configuration choices have changed
                        _configurationList = NewConfigurationList;
                        ConfigurationsGrid.BeginInvoke( (Action) ( _refreshConfigurationOptions ) );
                    }

                } //if( "Authenticated" == StatusText )
                else
                {
                     ConnectionResultsOutput.Invoke( (Action) ( () => { ConnectionResultsOutput.Text += "Connection Error: " + StatusText + "\r\n"; } ) );
                     tabControl1.BeginInvoke( (Action) ( () => { tabControl1.SelectedTab = NBTTab; } ) );
                }
            };//SessionCompleteEvent fetchConfigurations

            //Perform a test connection asynchronously, using the managed thread pool
            BackgroundWorker GetConfigurations = new BackgroundWorker();
            GetConfigurations.DoWork += _authenticationClient.PerformActionAsync;
            GetConfigurations.RunWorkerAsync( fetchConfigurations );


        }//refreshConfigurationList()



        /// <summary>
        /// Refresh the DataSource for the configurations grid and drop down to use the most recent dictionary
        /// </summary>
        private void _refreshConfigurationOptions()
        {
            ConfigurationsGrid.AutoGenerateColumns = false;
            if( _configurationList.Count > 0 )
            {//todo: ideally, a less computationally heavy method of refreshing should be found for updates after the first
                ConfigurationsGrid.DataSource = _configurationList.Values.ToList();
                ConfigurationsGrid.Columns["configName"].DataPropertyName = "Name";
                ConfigurationsGrid.Columns["Request"].DataPropertyName = "RequestFormat";
                ConfigurationsGrid.Columns["Response"].DataPropertyName = "ResponseFormat";
                ConfigurationsGrid.Columns["BaudRate"].DataPropertyName = "BaudRate";
                ConfigurationsGrid.Columns["ParityBit"].DataPropertyName = "ParityBit";
                ConfigurationsGrid.Columns["DataBits"].DataPropertyName = "DataBits";
                ConfigurationsGrid.Columns["StopBits"].DataPropertyName = "StopBits";
                ConfigurationsGrid.Columns["Handshake"].DataPropertyName = "Handshake";
                ConfigurationsGrid.Refresh();

                Configuration.DataSource = new BindingSource( _configurationList, null );
                Configuration.DisplayMember = "Key";
                Configuration.ValueType = typeof( BalanceConfiguration );
                Configuration.ValueMember = "Value";

                loadUserSettings();
            }//if _configurationList.Count > 0
        }//_refreshConfigurationOptions()




        /// <summary>
        /// Construct a new Timer to manage the regular polling of balances.
        /// </summary>
        /// <param name="Milliseconds">length of time between balance polls in milliseconds</param>
        private void constructPollTimer(int Milliseconds)
        {
          //cleanup any old timer
            if( null != _balancePollTimer )
            {
                _balancePollTimer.Stop();
                _balancePollTimer.Dispose();
            }


            _balancePollTimer = new Timer();
            _balancePollTimer.Interval = Milliseconds;
            _balancePollTimer.Tick += pollBalances;
            _balancePollTimer.Start();

        }//constructPollTimer()



        /// <summary>
        /// Attached to pollBalancesTimer. 
        /// Iterate through all balances in dictionary and call performBalanceCheck() for each.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="E"></param>
        private void pollBalances(object Sender, EventArgs E )
        {
            foreach( Balance Balance in _balanceList.Values )
            {
                Balance.BalanceAsynchronousTask CheckBalance = Balance.performBalanceCheck;
                CheckBalance.BeginInvoke(null, null);
            }

        }//pollBalances()





        /// <summary>
        /// Attached to announceBalanceTimer. 
        /// Iterate through all balances in dictionary and call announceBalance() for each.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="E"></param>
        private void announceBalances( object Sender, EventArgs E )
        {
            foreach( Balance Balance in _balanceList.Values )
            {
                Balance.BalanceAsynchronousTask AnnounceBalance = Balance.announceBalance;
                AnnounceBalance.BeginInvoke( null, null );
            }

        }//announceBalances()



        /// <summary>
        /// Attached to HardwareGrid's CellValueChanged event.
        /// Reads what the updated value of the cell is, then passes the information to the appropriate Balance object.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="E"></param>
        private void handleGridUpdate( object Sender, EventArgs E )
        {
            if( null != HardwareGrid.CurrentRow )
            {
                string Device = (string) HardwareGrid.CurrentRow.Cells["COM"].Value;
                string Field = HardwareGrid.CurrentCell.OwningColumn.Name;
                object Value = HardwareGrid.CurrentCell.Value;

                _balanceList[Device].updateField( Field, Value );
            }
        }//handleGridUpdate()



        /// <summary>
        /// Attached to HardwareGrid's CurrentCellDirtyStateChanged event.
        /// Needed to make checkboxes send data immediately.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="E"></param>
        private void handleCheckboxUpdate( object Sender, EventArgs E )
        {
            HardwareGrid.CommitEdit( DataGridViewDataErrorContexts.Commit );
        }




        /// <summary>
        /// Attached to configurationsGrid's CellClick event.
        /// Update all the form fields with the values from the row clicked.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="E"></param>
        private void handleConfigurationCellSelected( object Sender, EventArgs E )
        {
            if( null != ConfigurationsGrid.CurrentCell )
            {
                templateNameBox.Text = (string) ConfigurationsGrid.CurrentRow.Cells["configName"].Value;
                templateRequestBox.Text = (string) ConfigurationsGrid.CurrentRow.Cells["Request"].Value;

                //get the dictionary of already defined example responses from the combobox
                   Dictionary<string, string> PreconfiguredRegex = (Dictionary<string, string>) ( (BindingSource) templateResponseBox.DataSource ).DataSource;

                if( PreconfiguredRegex.ContainsValue( (string) ConfigurationsGrid.CurrentRow.Cells["Response"].Value ) )
                {
                    templateResponseBox.SelectedValue = ConfigurationsGrid.CurrentRow.Cells["Response"].Value;
                    templateExpressionBox.BackColor = Color.LightGray;
                    templateExpressionBox.ReadOnly = true;
                }
                else
                {
                    templateResponseBox.SelectedValue = "Custom";
                    templateExpressionBox.BackColor = ConfigurationsGrid.DefaultCellStyle.BackColor;
                    templateExpressionBox.ReadOnly = false;
                }

                templateExpressionBox.Text = (string) ConfigurationsGrid.CurrentRow.Cells["Response"].Value;
                templateBaudRateBox.SelectedItem = ConfigurationsGrid.CurrentRow.Cells["BaudRate"].Value;
                templateDataBitsBox.SelectedItem = ConfigurationsGrid.CurrentRow.Cells["DataBits"].Value;
                templateStopBitsBox.SelectedValue = Enum.Parse( typeof( StopBits ), (string) ConfigurationsGrid.CurrentRow.Cells["StopBits"].Value );
                templateParityBitBox.SelectedValue = Enum.Parse( typeof( Parity ), (string) ConfigurationsGrid.CurrentRow.Cells["ParityBit"].Value );
                templateHandshakeBox.SelectedValue = Enum.Parse( typeof( Handshake ), (string) ConfigurationsGrid.CurrentRow.Cells["Handshake"].Value );

            }//if ( null != ConfigurationsGrid.CurrentRow )
        }//handleConfigurationCellSelected()



        /// <summary>
        /// Attached to Update button for polling frequency.
        /// Construct a new timer to poll balances with a frequency of the current value on the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updatePollingButton_Click( object sender, EventArgs e )
        {
            constructPollTimer( int.Parse(pollingFrequencyField.Text) );
        }




        /// <summary>
        /// Attached to Refresh button under the hardware grid.
        /// Re-scan the registry for an updated list of available hardware.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshHardwareButton_Click( object sender, EventArgs e )
        {
         //save any user-entered data for the existing balances
            saveUserSettings( this, new EventArgs() );
            refreshHardwareList();
        }




        /// <summary>
        /// Attached to Test Connection button on the NBT tab.
        /// Try to connect to NBT with the values entered, and display result to output box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void testConnectionButton_Click( object sender, EventArgs e )
        {

          //define how to update UI when the authentication attempt resolves
            NbtAuth.SessionCompleteEvent PrintStatusMessage = delegate(NbtPublicClient Client, string StatusText )
                {
                    switch( StatusText )
                    {
                        case "Failed":
                            ConnectionResultsOutput.Invoke( (Action) ( () => { ConnectionResultsOutput.Text += "You have supplied an incorrect username or password for this account.\r\n"; } ) );
                            break;

                        case "NonExistentAccessId":
                            ConnectionResultsOutput.Invoke( (Action) ( () => { ConnectionResultsOutput.Text += "The Customer Id you have supplied does not exist in the database.\r\n"; } ) );
                            break;

                        case "Authenticated":
                            ConnectionResultsOutput.Invoke( (Action) ( () => { ConnectionResultsOutput.Text += "Connection successful.\r\n"; } ) );
                            break;

                        case "Object reference not set to an instance of an object.":
                            ConnectionResultsOutput.Invoke( (Action) ( () => { ConnectionResultsOutput.Text += "Please enter the connection details you use to connect to NBT.\r\n"; } ) );
                            break;

                        case "Invalid URI: The format of the URI could not be determined.":
                            ConnectionResultsOutput.Invoke( (Action) ( () => { ConnectionResultsOutput.Text += "Please check the host address for your connection.\r\n"; } ) );
                            break;

                        default:
                            ConnectionResultsOutput.Invoke( (Action) ( () => { ConnectionResultsOutput.Text += "Connection Error: " + StatusText + "\r\n"; } ) );
                            break;

                    }//switch ( StatusText )

                };//SessionCompleteEvent PrintStatusMessage


            //Perform a test connection asynchronously, using the managed thread pool
               ConnectionResultsOutput.Text += "Attempting to connect...\r\n";
               BackgroundWorker NbtTask = new BackgroundWorker();
               NbtTask.DoWork += _authenticationClient.PerformActionAsync;
               NbtTask.RunWorkerAsync(PrintStatusMessage);


        }//testConnectionButton_Click( object sender, EventArgs e )




        /// <summary>
        /// Attached to Refresh button under the configuration grid.
        /// Send a request to NBT for an updated list of available configurations.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void templateRefreshButton_Click( object sender, EventArgs e )
        {
            saveUserSettings( this, new EventArgs() );
            refreshConfigurationList();
        }





        /// <summary>
        /// Attached to Save button on Configuration tab.
        /// Attempt to send current form values to NBT as a new balance config. 
        /// On success: if the balance is new, append to the hardware grid
        /// On failure: switch to NBT tab and print error to output box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveTemplateButton_Click( object sender, EventArgs e )
        {

            NbtAuth.SessionCompleteEvent sendConfiguration = delegate( NbtPublicClient Client, string StatusText )
            {
                if( "Authenticated" == StatusText )
                {
                  BalanceConfiguration ConfigurationToSend = new BalanceConfiguration();
                    ConfigurationToSend.Name = templateNameBox.Text;
                    ConfigurationToSend.RequestFormat = templateRequestBox.Text;
                    ConfigurationToSend.ResponseFormat = templateExpressionBox.Text;
                    templateBaudRateBox.Invoke( (Action) ( () => { ConfigurationToSend.BaudRate = (int) templateBaudRateBox.SelectedItem;  } ) );
                    templateDataBitsBox.Invoke( (Action) ( () => { ConfigurationToSend.DataBits = (int) templateDataBitsBox.SelectedItem; } ) );
                    templateParityBitBox.Invoke( (Action) ( () => { ConfigurationToSend.ParityBit = templateParityBitBox.SelectedValue.ToString(); } ) );
                    templateStopBitsBox.Invoke( (Action) ( () => { ConfigurationToSend.StopBits = templateStopBitsBox.SelectedValue.ToString(); } ) );
                    templateHandshakeBox.Invoke( (Action) ( () => { ConfigurationToSend.Handshake = templateHandshakeBox.SelectedValue.ToString(); } ) );


                    BalanceConfiguration RegisteredBalance = Client.registerBalanceConfiguration( ConfigurationToSend ).Data.ConfigurationList[0];
                    if( null != RegisteredBalance )
                    {
                        saveUserSettings( this, new EventArgs() );
                        refreshConfigurationList();
                    }


                } //if( "Authenticated" == StatusText )
                else
                {
                    ConnectionResultsOutput.Invoke( (Action) ( () => { ConnectionResultsOutput.Text += "Connection Error: " + StatusText + "\r\n"; } ) );
                    tabControl1.BeginInvoke( (Action) ( () => { tabControl1.SelectedTab = NBTTab; } ) );
                }
            };//SessionCompleteEvent fetchConfigurations


            //Perform a test connection asynchronously, using the managed thread pool
            BackgroundWorker NbtTask = new BackgroundWorker();
            NbtTask.DoWork += _authenticationClient.PerformActionAsync;
            NbtTask.RunWorkerAsync( sendConfiguration );

        }//saveTemplateButton_Click()




        /// <summary>
        /// Attached to example Responses dropdown on configuration tab.
        /// If the user selects a non-custom value, auto-set the expression and make read only.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="E"></param>
        private void exampleResponseSelected(object Sender, EventArgs E) 
        {
            if( "Custom" != templateResponseBox.SelectedValue )
            {
                templateExpressionBox.ReadOnly = true;
                templateExpressionBox.BackColor = Color.LightGray;
                templateExpressionBox.Text = (string) templateResponseBox.SelectedValue;
            }
            else
            {
                templateExpressionBox.ReadOnly = false;
                templateExpressionBox.BackColor = Color.White;
            }
        }//exampleResponseSelected()




    }//class Form1
}//namespace BalanceReaderClient
