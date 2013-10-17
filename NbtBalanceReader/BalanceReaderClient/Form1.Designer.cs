namespace BalanceReaderClient
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.HardwareTab = new System.Windows.Forms.TabPage();
            this.HardwareGrid = new System.Windows.Forms.DataGridView();
            this.refreshHardwareButton = new System.Windows.Forms.Button();
            this.updatePollingButton = new System.Windows.Forms.Button();
            this.pollingFrequencyField = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.ConfigurationsTab = new System.Windows.Forms.TabPage();
            this.saveTemplateButton = new System.Windows.Forms.Button();
            this.templateRefreshButton = new System.Windows.Forms.Button();
            this.templateHandshakeBox = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.templateStopBitsBox = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.templateDataBitsBox = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.templateParityBitBox = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.templateBaudRateBox = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.templateResponseBox = new System.Windows.Forms.ComboBox();
            this.templateExpressionBox = new System.Windows.Forms.TextBox();
            this.templateRequestBox = new System.Windows.Forms.TextBox();
            this.templateNameBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ConfigurationsGrid = new System.Windows.Forms.DataGridView();
            this.configName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Request = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Response = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BaudRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataBits = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StopBits = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParityBit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Handshake = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NBTTab = new System.Windows.Forms.TabPage();
            this.AddressField = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ConnectionResultsOutput = new System.Windows.Forms.TextBox();
            this.testConnectionButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.PasswordField = new System.Windows.Forms.TextBox();
            this.UsernameField = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.AccessIdField = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.form1BindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.COM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FriendlyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NBTName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Configuration = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Enabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.StableOnly = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.DriftThreshold = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrentWeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1.SuspendLayout();
            this.HardwareTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HardwareGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pollingFrequencyField)).BeginInit();
            this.ConfigurationsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConfigurationsGrid)).BeginInit();
            this.NBTTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.form1BindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.HardwareTab);
            this.tabControl1.Controls.Add(this.ConfigurationsTab);
            this.tabControl1.Controls.Add(this.NBTTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(774, 455);
            this.tabControl1.TabIndex = 0;
            // 
            // HardwareTab
            // 
            this.HardwareTab.Controls.Add(this.HardwareGrid);
            this.HardwareTab.Controls.Add(this.refreshHardwareButton);
            this.HardwareTab.Controls.Add(this.updatePollingButton);
            this.HardwareTab.Controls.Add(this.pollingFrequencyField);
            this.HardwareTab.Controls.Add(this.label4);
            this.HardwareTab.Location = new System.Drawing.Point(4, 22);
            this.HardwareTab.Name = "HardwareTab";
            this.HardwareTab.Padding = new System.Windows.Forms.Padding(3);
            this.HardwareTab.Size = new System.Drawing.Size(766, 429);
            this.HardwareTab.TabIndex = 1;
            this.HardwareTab.Text = "Hardware";
            this.HardwareTab.UseVisualStyleBackColor = true;
            // 
            // HardwareGrid
            // 
            this.HardwareGrid.AllowUserToAddRows = false;
            this.HardwareGrid.AllowUserToDeleteRows = false;
            this.HardwareGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HardwareGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.HardwareGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.HardwareGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.COM,
            this.FriendlyName,
            this.NBTName,
            this.Configuration,
            this.Enabled,
            this.StableOnly,
            this.DriftThreshold,
            this.CurrentWeight});
            this.HardwareGrid.Location = new System.Drawing.Point(13, 109);
            this.HardwareGrid.Name = "HardwareGrid";
            this.HardwareGrid.RowHeadersVisible = false;
            this.HardwareGrid.Size = new System.Drawing.Size(745, 214);
            this.HardwareGrid.TabIndex = 4;
            // 
            // refreshHardwareButton
            // 
            this.refreshHardwareButton.Location = new System.Drawing.Point(13, 365);
            this.refreshHardwareButton.Name = "refreshHardwareButton";
            this.refreshHardwareButton.Size = new System.Drawing.Size(158, 23);
            this.refreshHardwareButton.TabIndex = 3;
            this.refreshHardwareButton.Text = "Refresh Hardware List";
            this.refreshHardwareButton.UseVisualStyleBackColor = true;
            this.refreshHardwareButton.Click += new System.EventHandler(this.refreshHardwareButton_Click);
            // 
            // updatePollingButton
            // 
            this.updatePollingButton.Location = new System.Drawing.Point(239, 40);
            this.updatePollingButton.Name = "updatePollingButton";
            this.updatePollingButton.Size = new System.Drawing.Size(75, 23);
            this.updatePollingButton.TabIndex = 2;
            this.updatePollingButton.Text = "Update";
            this.updatePollingButton.UseVisualStyleBackColor = true;
            this.updatePollingButton.Click += new System.EventHandler(this.updatePollingButton_Click);
            // 
            // pollingFrequencyField
            // 
            this.pollingFrequencyField.Location = new System.Drawing.Point(148, 43);
            this.pollingFrequencyField.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.pollingFrequencyField.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.pollingFrequencyField.Name = "pollingFrequencyField";
            this.pollingFrequencyField.Size = new System.Drawing.Size(72, 20);
            this.pollingFrequencyField.TabIndex = 1;
            this.pollingFrequencyField.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(29, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Polling Frequency (ms)";
            // 
            // ConfigurationsTab
            // 
            this.ConfigurationsTab.Controls.Add(this.saveTemplateButton);
            this.ConfigurationsTab.Controls.Add(this.templateRefreshButton);
            this.ConfigurationsTab.Controls.Add(this.templateHandshakeBox);
            this.ConfigurationsTab.Controls.Add(this.label15);
            this.ConfigurationsTab.Controls.Add(this.templateStopBitsBox);
            this.ConfigurationsTab.Controls.Add(this.label14);
            this.ConfigurationsTab.Controls.Add(this.templateDataBitsBox);
            this.ConfigurationsTab.Controls.Add(this.label13);
            this.ConfigurationsTab.Controls.Add(this.templateParityBitBox);
            this.ConfigurationsTab.Controls.Add(this.label12);
            this.ConfigurationsTab.Controls.Add(this.templateBaudRateBox);
            this.ConfigurationsTab.Controls.Add(this.label11);
            this.ConfigurationsTab.Controls.Add(this.templateResponseBox);
            this.ConfigurationsTab.Controls.Add(this.templateExpressionBox);
            this.ConfigurationsTab.Controls.Add(this.templateRequestBox);
            this.ConfigurationsTab.Controls.Add(this.templateNameBox);
            this.ConfigurationsTab.Controls.Add(this.label10);
            this.ConfigurationsTab.Controls.Add(this.label9);
            this.ConfigurationsTab.Controls.Add(this.label8);
            this.ConfigurationsTab.Controls.Add(this.label7);
            this.ConfigurationsTab.Controls.Add(this.label6);
            this.ConfigurationsTab.Controls.Add(this.ConfigurationsGrid);
            this.ConfigurationsTab.Location = new System.Drawing.Point(4, 22);
            this.ConfigurationsTab.Name = "ConfigurationsTab";
            this.ConfigurationsTab.Padding = new System.Windows.Forms.Padding(3);
            this.ConfigurationsTab.Size = new System.Drawing.Size(766, 429);
            this.ConfigurationsTab.TabIndex = 2;
            this.ConfigurationsTab.Text = "Configurations";
            this.ConfigurationsTab.UseVisualStyleBackColor = true;
            // 
            // saveTemplateButton
            // 
            this.saveTemplateButton.Location = new System.Drawing.Point(603, 362);
            this.saveTemplateButton.Name = "saveTemplateButton";
            this.saveTemplateButton.Size = new System.Drawing.Size(88, 38);
            this.saveTemplateButton.TabIndex = 21;
            this.saveTemplateButton.Text = "Save Template";
            this.saveTemplateButton.UseVisualStyleBackColor = true;
            this.saveTemplateButton.Click += new System.EventHandler(this.saveTemplateButton_Click);
            // 
            // templateRefreshButton
            // 
            this.templateRefreshButton.Location = new System.Drawing.Point(603, 196);
            this.templateRefreshButton.Name = "templateRefreshButton";
            this.templateRefreshButton.Size = new System.Drawing.Size(88, 23);
            this.templateRefreshButton.TabIndex = 20;
            this.templateRefreshButton.Text = "Refresh";
            this.templateRefreshButton.UseVisualStyleBackColor = true;
            this.templateRefreshButton.Click += new System.EventHandler(this.templateRefreshButton_Click);
            // 
            // templateHandshakeBox
            // 
            this.templateHandshakeBox.FormattingEnabled = true;
            this.templateHandshakeBox.Location = new System.Drawing.Point(447, 379);
            this.templateHandshakeBox.Name = "templateHandshakeBox";
            this.templateHandshakeBox.Size = new System.Drawing.Size(81, 21);
            this.templateHandshakeBox.TabIndex = 19;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(363, 382);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(62, 13);
            this.label15.TabIndex = 18;
            this.label15.Text = "Handshake";
            // 
            // templateStopBitsBox
            // 
            this.templateStopBitsBox.FormattingEnabled = true;
            this.templateStopBitsBox.Location = new System.Drawing.Point(447, 343);
            this.templateStopBitsBox.Name = "templateStopBitsBox";
            this.templateStopBitsBox.Size = new System.Drawing.Size(81, 21);
            this.templateStopBitsBox.TabIndex = 17;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(376, 346);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(49, 13);
            this.label14.TabIndex = 16;
            this.label14.Text = "Stop Bits";
            // 
            // templateDataBitsBox
            // 
            this.templateDataBitsBox.FormattingEnabled = true;
            this.templateDataBitsBox.Location = new System.Drawing.Point(447, 310);
            this.templateDataBitsBox.Name = "templateDataBitsBox";
            this.templateDataBitsBox.Size = new System.Drawing.Size(81, 21);
            this.templateDataBitsBox.TabIndex = 15;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(375, 313);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(50, 13);
            this.label13.TabIndex = 14;
            this.label13.Text = "Data Bits";
            // 
            // templateParityBitBox
            // 
            this.templateParityBitBox.FormattingEnabled = true;
            this.templateParityBitBox.Location = new System.Drawing.Point(447, 277);
            this.templateParityBitBox.Name = "templateParityBitBox";
            this.templateParityBitBox.Size = new System.Drawing.Size(81, 21);
            this.templateParityBitBox.TabIndex = 13;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(377, 280);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 13);
            this.label12.TabIndex = 12;
            this.label12.Text = "Parity Bit";
            // 
            // templateBaudRateBox
            // 
            this.templateBaudRateBox.FormattingEnabled = true;
            this.templateBaudRateBox.Location = new System.Drawing.Point(447, 244);
            this.templateBaudRateBox.Name = "templateBaudRateBox";
            this.templateBaudRateBox.Size = new System.Drawing.Size(81, 21);
            this.templateBaudRateBox.TabIndex = 11;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(367, 247);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "Baud Rate";
            // 
            // templateResponseBox
            // 
            this.templateResponseBox.FormattingEnabled = true;
            this.templateResponseBox.Location = new System.Drawing.Point(174, 325);
            this.templateResponseBox.Name = "templateResponseBox";
            this.templateResponseBox.Size = new System.Drawing.Size(115, 21);
            this.templateResponseBox.TabIndex = 9;
            // 
            // templateExpressionBox
            // 
            this.templateExpressionBox.Location = new System.Drawing.Point(174, 359);
            this.templateExpressionBox.Name = "templateExpressionBox";
            this.templateExpressionBox.Size = new System.Drawing.Size(115, 20);
            this.templateExpressionBox.TabIndex = 8;
            // 
            // templateRequestBox
            // 
            this.templateRequestBox.Location = new System.Drawing.Point(174, 295);
            this.templateRequestBox.Name = "templateRequestBox";
            this.templateRequestBox.Size = new System.Drawing.Size(115, 20);
            this.templateRequestBox.TabIndex = 7;
            // 
            // templateNameBox
            // 
            this.templateNameBox.Location = new System.Drawing.Point(174, 261);
            this.templateNameBox.Name = "templateNameBox";
            this.templateNameBox.Size = new System.Drawing.Size(115, 20);
            this.templateNameBox.TabIndex = 6;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(96, 362);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(58, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Expression";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(99, 328);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Response";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(107, 298);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Request";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(119, 264);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Name";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(67, 222);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Configure Template:";
            // 
            // ConfigurationsGrid
            // 
            this.ConfigurationsGrid.AllowUserToAddRows = false;
            this.ConfigurationsGrid.AllowUserToDeleteRows = false;
            this.ConfigurationsGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfigurationsGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ConfigurationsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ConfigurationsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.configName,
            this.Request,
            this.Response,
            this.BaudRate,
            this.DataBits,
            this.StopBits,
            this.ParityBit,
            this.Handshake});
            this.ConfigurationsGrid.Location = new System.Drawing.Point(19, 22);
            this.ConfigurationsGrid.Name = "ConfigurationsGrid";
            this.ConfigurationsGrid.ReadOnly = true;
            this.ConfigurationsGrid.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ConfigurationsGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.ConfigurationsGrid.RowHeadersVisible = false;
            this.ConfigurationsGrid.Size = new System.Drawing.Size(727, 168);
            this.ConfigurationsGrid.TabIndex = 0;
            // 
            // configName
            // 
            this.configName.HeaderText = "Name";
            this.configName.Name = "configName";
            this.configName.ReadOnly = true;
            // 
            // Request
            // 
            this.Request.HeaderText = "Request";
            this.Request.Name = "Request";
            this.Request.ReadOnly = true;
            // 
            // Response
            // 
            this.Response.HeaderText = "Response";
            this.Response.Name = "Response";
            this.Response.ReadOnly = true;
            // 
            // BaudRate
            // 
            this.BaudRate.HeaderText = "Baud Rate";
            this.BaudRate.Name = "BaudRate";
            this.BaudRate.ReadOnly = true;
            // 
            // DataBits
            // 
            this.DataBits.HeaderText = "Data Bits";
            this.DataBits.Name = "DataBits";
            this.DataBits.ReadOnly = true;
            // 
            // StopBits
            // 
            this.StopBits.HeaderText = "Stop Bits";
            this.StopBits.Name = "StopBits";
            this.StopBits.ReadOnly = true;
            // 
            // ParityBit
            // 
            this.ParityBit.HeaderText = "Parity Bit";
            this.ParityBit.Name = "ParityBit";
            this.ParityBit.ReadOnly = true;
            // 
            // Handshake
            // 
            this.Handshake.HeaderText = "Handshake";
            this.Handshake.Name = "Handshake";
            this.Handshake.ReadOnly = true;
            // 
            // NBTTab
            // 
            this.NBTTab.Controls.Add(this.AddressField);
            this.NBTTab.Controls.Add(this.label5);
            this.NBTTab.Controls.Add(this.ConnectionResultsOutput);
            this.NBTTab.Controls.Add(this.testConnectionButton);
            this.NBTTab.Controls.Add(this.label3);
            this.NBTTab.Controls.Add(this.PasswordField);
            this.NBTTab.Controls.Add(this.UsernameField);
            this.NBTTab.Controls.Add(this.label2);
            this.NBTTab.Controls.Add(this.AccessIdField);
            this.NBTTab.Controls.Add(this.label1);
            this.NBTTab.Location = new System.Drawing.Point(4, 22);
            this.NBTTab.Name = "NBTTab";
            this.NBTTab.Padding = new System.Windows.Forms.Padding(3);
            this.NBTTab.Size = new System.Drawing.Size(766, 429);
            this.NBTTab.TabIndex = 0;
            this.NBTTab.Text = "NBT Setup";
            this.NBTTab.UseVisualStyleBackColor = true;
            // 
            // AddressField
            // 
            this.AddressField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AddressField.Location = new System.Drawing.Point(214, 95);
            this.AddressField.Name = "AddressField";
            this.AddressField.Size = new System.Drawing.Size(253, 20);
            this.AddressField.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(138, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Host Address";
            // 
            // ConnectionResultsOutput
            // 
            this.ConnectionResultsOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConnectionResultsOutput.Location = new System.Drawing.Point(12, 133);
            this.ConnectionResultsOutput.Multiline = true;
            this.ConnectionResultsOutput.Name = "ConnectionResultsOutput";
            this.ConnectionResultsOutput.ReadOnly = true;
            this.ConnectionResultsOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ConnectionResultsOutput.Size = new System.Drawing.Size(729, 271);
            this.ConnectionResultsOutput.TabIndex = 9;
            // 
            // testConnectionButton
            // 
            this.testConnectionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.testConnectionButton.Location = new System.Drawing.Point(540, 43);
            this.testConnectionButton.Name = "testConnectionButton";
            this.testConnectionButton.Size = new System.Drawing.Size(75, 46);
            this.testConnectionButton.TabIndex = 8;
            this.testConnectionButton.Text = "Test Connection";
            this.testConnectionButton.UseVisualStyleBackColor = true;
            this.testConnectionButton.Click += new System.EventHandler(this.testConnectionButton_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(155, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Password";
            // 
            // PasswordField
            // 
            this.PasswordField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PasswordField.Location = new System.Drawing.Point(214, 69);
            this.PasswordField.Name = "PasswordField";
            this.PasswordField.Size = new System.Drawing.Size(253, 20);
            this.PasswordField.TabIndex = 4;
            // 
            // UsernameField
            // 
            this.UsernameField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UsernameField.Location = new System.Drawing.Point(214, 43);
            this.UsernameField.Name = "UsernameField";
            this.UsernameField.Size = new System.Drawing.Size(253, 20);
            this.UsernameField.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(153, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Username";
            // 
            // AccessIdField
            // 
            this.AccessIdField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AccessIdField.Location = new System.Drawing.Point(214, 17);
            this.AccessIdField.Name = "AccessIdField";
            this.AccessIdField.Size = new System.Drawing.Size(253, 20);
            this.AccessIdField.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(145, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Customer Id";
            // 
            // form1BindingSource
            // 
            this.form1BindingSource.DataSource = typeof(BalanceReaderClient.Form1);
            // 
            // COM
            // 
            this.COM.HeaderText = "COM";
            this.COM.Name = "COM";
            this.COM.ReadOnly = true;
            this.COM.Visible = false;
            // 
            // FriendlyName
            // 
            this.FriendlyName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.FriendlyName.HeaderText = "Device";
            this.FriendlyName.Name = "FriendlyName";
            this.FriendlyName.ReadOnly = true;
            this.FriendlyName.Width = 66;
            // 
            // NBTName
            // 
            this.NBTName.FillWeight = 191.6933F;
            this.NBTName.HeaderText = "ChemSW Live Name *";
            this.NBTName.Name = "NBTName";
            // 
            // Configuration
            // 
            this.Configuration.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Configuration.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Configuration.HeaderText = "Configuration *";
            this.Configuration.Name = "Configuration";
            // 
            // Enabled
            // 
            this.Enabled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Enabled.FillWeight = 44F;
            this.Enabled.HeaderText = "Enabled";
            this.Enabled.Name = "Enabled";
            this.Enabled.Width = 52;
            // 
            // StableOnly
            // 
            this.StableOnly.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.StableOnly.FillWeight = 44F;
            this.StableOnly.HeaderText = "Stable Reads";
            this.StableOnly.Name = "StableOnly";
            this.StableOnly.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.StableOnly.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.StableOnly.ToolTipText = "Only send data to NBT on a stable measurement";
            this.StableOnly.Width = 96;
            // 
            // DriftThreshold
            // 
            this.DriftThreshold.FillWeight = 49.32624F;
            this.DriftThreshold.HeaderText = "Drift Threshold";
            this.DriftThreshold.Name = "DriftThreshold";
            // 
            // CurrentWeight
            // 
            this.CurrentWeight.FillWeight = 68.83578F;
            this.CurrentWeight.HeaderText = "Current Weight";
            this.CurrentWeight.Name = "CurrentWeight";
            this.CurrentWeight.ReadOnly = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 455);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Balance Reader Client v1.0.3";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.HardwareTab.ResumeLayout(false);
            this.HardwareTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HardwareGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pollingFrequencyField)).EndInit();
            this.ConfigurationsTab.ResumeLayout(false);
            this.ConfigurationsTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConfigurationsGrid)).EndInit();
            this.NBTTab.ResumeLayout(false);
            this.NBTTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.form1BindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage NBTTab;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox PasswordField;
        private System.Windows.Forms.TextBox UsernameField;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox AccessIdField;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage HardwareTab;
        private System.Windows.Forms.Button testConnectionButton;
        private System.Windows.Forms.TextBox ConnectionResultsOutput;
        private System.Windows.Forms.Button refreshHardwareButton;
        private System.Windows.Forms.Button updatePollingButton;
        private System.Windows.Forms.NumericUpDown pollingFrequencyField;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView HardwareGrid;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox AddressField;
        private System.Windows.Forms.TabPage ConfigurationsTab;
        private System.Windows.Forms.DataGridView ConfigurationsGrid;
        private System.Windows.Forms.TextBox templateNameBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox templateHandshakeBox;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox templateStopBitsBox;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox templateDataBitsBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox templateParityBitBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox templateBaudRateBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox templateResponseBox;
        private System.Windows.Forms.TextBox templateExpressionBox;
        private System.Windows.Forms.TextBox templateRequestBox;
        private System.Windows.Forms.Button saveTemplateButton;
        private System.Windows.Forms.Button templateRefreshButton;
        private System.Windows.Forms.BindingSource form1BindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn configName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Request;
        private System.Windows.Forms.DataGridViewTextBoxColumn Response;
        private System.Windows.Forms.DataGridViewTextBoxColumn BaudRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataBits;
        private System.Windows.Forms.DataGridViewTextBoxColumn StopBits;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParityBit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Handshake;
        private System.Windows.Forms.DataGridViewTextBoxColumn COM;
        private System.Windows.Forms.DataGridViewTextBoxColumn FriendlyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn NBTName;
        private System.Windows.Forms.DataGridViewComboBoxColumn Configuration;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Enabled;
        private System.Windows.Forms.DataGridViewCheckBoxColumn StableOnly;
        private System.Windows.Forms.DataGridViewTextBoxColumn DriftThreshold;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrentWeight;
    }
}

