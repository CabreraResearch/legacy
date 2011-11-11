namespace ChemSW.Nbt.Schema
{
    partial class ImporterForm
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
            this.InitSchemaLabel = new System.Windows.Forms.Label();
            this.InitSchemaSelectBox = new System.Windows.Forms.ComboBox();
            this.ImportButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.ExportTab = new System.Windows.Forms.TabPage();
            this.UnCheckAllButton = new System.Windows.Forms.Button();
            this.CheckAllButton = new System.Windows.Forms.Button();
            this.NodeTypeCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.ExportCompletedLabel = new System.Windows.Forms.Label();
            this.InProgressLabel = new System.Windows.Forms.Label();
            this.ExportIncludeLabel = new System.Windows.Forms.Label();
            this.ExportViews = new System.Windows.Forms.CheckBox();
            this.ExportNodes = new System.Windows.Forms.CheckBox();
            this.ExportNodetypes = new System.Windows.Forms.CheckBox();
            this.ExportButton = new System.Windows.Forms.Button();
            this.ExportSchemaSelectBox = new System.Windows.Forms.ComboBox();
            this.ExportSchemaLabel = new System.Windows.Forms.Label();
            this.ImportPage = new System.Windows.Forms.TabPage();
            this.FileTypeSelectBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.DataFileLink = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.ModeComboBox = new System.Windows.Forms.ComboBox();
            this.DataFileLabel = new System.Windows.Forms.Label();
            this.ResultsTextBox = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.ErrorLabel = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.PhaseTextBox = new System.Windows.Forms.TextBox();
            this.gbx_PhaseStatus = new System.Windows.Forms.GroupBox();
            this.btn_ResetSchema = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.ExportTab.SuspendLayout();
            this.ImportPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // InitSchemaLabel
            // 
            this.InitSchemaLabel.AutoSize = true;
            this.InitSchemaLabel.Location = new System.Drawing.Point( 3, 64 );
            this.InitSchemaLabel.Name = "InitSchemaLabel";
            this.InitSchemaLabel.Size = new System.Drawing.Size( 83, 13 );
            this.InitSchemaLabel.TabIndex = 2;
            this.InitSchemaLabel.Text = "Target Schema:";
            // 
            // InitSchemaSelectBox
            // 
            this.InitSchemaSelectBox.FormattingEnabled = true;
            this.InitSchemaSelectBox.Location = new System.Drawing.Point( 92, 61 );
            this.InitSchemaSelectBox.Name = "InitSchemaSelectBox";
            this.InitSchemaSelectBox.Size = new System.Drawing.Size( 183, 21 );
            this.InitSchemaSelectBox.TabIndex = 3;
            // 
            // ImportButton
            // 
            this.ImportButton.Location = new System.Drawing.Point( 333, 86 );
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.Size = new System.Drawing.Size( 80, 23 );
            this.ImportButton.TabIndex = 7;
            this.ImportButton.Text = "Start";
            this.ImportButton.UseVisualStyleBackColor = true;
            this.ImportButton.Click += new System.EventHandler( this.ImportButton_Click );
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.tabControl1.Controls.Add( this.ExportTab );
            this.tabControl1.Controls.Add( this.ImportPage );
            this.tabControl1.Location = new System.Drawing.Point( 3, 5 );
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size( 427, 374 );
            this.tabControl1.TabIndex = 10;
            // 
            // ExportTab
            // 
            this.ExportTab.Controls.Add( this.UnCheckAllButton );
            this.ExportTab.Controls.Add( this.CheckAllButton );
            this.ExportTab.Controls.Add( this.NodeTypeCheckedListBox );
            this.ExportTab.Controls.Add( this.ExportCompletedLabel );
            this.ExportTab.Controls.Add( this.InProgressLabel );
            this.ExportTab.Controls.Add( this.ExportIncludeLabel );
            this.ExportTab.Controls.Add( this.ExportViews );
            this.ExportTab.Controls.Add( this.ExportNodes );
            this.ExportTab.Controls.Add( this.ExportNodetypes );
            this.ExportTab.Controls.Add( this.ExportButton );
            this.ExportTab.Controls.Add( this.ExportSchemaSelectBox );
            this.ExportTab.Controls.Add( this.ExportSchemaLabel );
            this.ExportTab.Location = new System.Drawing.Point( 4, 22 );
            this.ExportTab.Name = "ExportTab";
            this.ExportTab.Padding = new System.Windows.Forms.Padding( 3 );
            this.ExportTab.Size = new System.Drawing.Size( 350, 348 );
            this.ExportTab.TabIndex = 1;
            this.ExportTab.Text = "Export";
            this.ExportTab.UseVisualStyleBackColor = true;
            // 
            // UnCheckAllButton
            // 
            this.UnCheckAllButton.Location = new System.Drawing.Point( 253, 135 );
            this.UnCheckAllButton.Name = "UnCheckAllButton";
            this.UnCheckAllButton.Size = new System.Drawing.Size( 75, 23 );
            this.UnCheckAllButton.TabIndex = 11;
            this.UnCheckAllButton.Text = "Uncheck All";
            this.UnCheckAllButton.UseVisualStyleBackColor = true;
            this.UnCheckAllButton.Click += new System.EventHandler( this.UnCheckAllButton_Click );
            // 
            // CheckAllButton
            // 
            this.CheckAllButton.Location = new System.Drawing.Point( 252, 105 );
            this.CheckAllButton.Name = "CheckAllButton";
            this.CheckAllButton.Size = new System.Drawing.Size( 75, 23 );
            this.CheckAllButton.TabIndex = 10;
            this.CheckAllButton.Text = "Check All";
            this.CheckAllButton.UseVisualStyleBackColor = true;
            this.CheckAllButton.Click += new System.EventHandler( this.CheckAllButton_Click );
            // 
            // NodeTypeCheckedListBox
            // 
            this.NodeTypeCheckedListBox.CheckOnClick = true;
            this.NodeTypeCheckedListBox.FormattingEnabled = true;
            this.NodeTypeCheckedListBox.Location = new System.Drawing.Point( 6, 105 );
            this.NodeTypeCheckedListBox.Name = "NodeTypeCheckedListBox";
            this.NodeTypeCheckedListBox.Size = new System.Drawing.Size( 240, 154 );
            this.NodeTypeCheckedListBox.TabIndex = 9;
            // 
            // ExportCompletedLabel
            // 
            this.ExportCompletedLabel.AutoSize = true;
            this.ExportCompletedLabel.Location = new System.Drawing.Point( 119, 309 );
            this.ExportCompletedLabel.Name = "ExportCompletedLabel";
            this.ExportCompletedLabel.Size = new System.Drawing.Size( 90, 13 );
            this.ExportCompletedLabel.TabIndex = 8;
            this.ExportCompletedLabel.Text = "Export Completed";
            this.ExportCompletedLabel.Visible = false;
            // 
            // InProgressLabel
            // 
            this.InProgressLabel.AutoSize = true;
            this.InProgressLabel.ForeColor = System.Drawing.Color.Red;
            this.InProgressLabel.Location = new System.Drawing.Point( 116, 309 );
            this.InProgressLabel.Name = "InProgressLabel";
            this.InProgressLabel.Size = new System.Drawing.Size( 102, 13 );
            this.InProgressLabel.TabIndex = 7;
            this.InProgressLabel.Text = "Export In Progress...";
            this.InProgressLabel.Visible = false;
            // 
            // ExportIncludeLabel
            // 
            this.ExportIncludeLabel.AutoSize = true;
            this.ExportIncludeLabel.Location = new System.Drawing.Point( 59, 37 );
            this.ExportIncludeLabel.Name = "ExportIncludeLabel";
            this.ExportIncludeLabel.Size = new System.Drawing.Size( 95, 13 );
            this.ExportIncludeLabel.TabIndex = 6;
            this.ExportIncludeLabel.Text = "Include in Export...";
            // 
            // ExportViews
            // 
            this.ExportViews.AutoSize = true;
            this.ExportViews.Checked = true;
            this.ExportViews.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ExportViews.Location = new System.Drawing.Point( 160, 82 );
            this.ExportViews.Name = "ExportViews";
            this.ExportViews.Size = new System.Drawing.Size( 54, 17 );
            this.ExportViews.TabIndex = 5;
            this.ExportViews.Text = "Views";
            this.ExportViews.UseVisualStyleBackColor = true;
            // 
            // ExportNodes
            // 
            this.ExportNodes.AutoSize = true;
            this.ExportNodes.Checked = true;
            this.ExportNodes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ExportNodes.Location = new System.Drawing.Point( 160, 59 );
            this.ExportNodes.Name = "ExportNodes";
            this.ExportNodes.Size = new System.Drawing.Size( 57, 17 );
            this.ExportNodes.TabIndex = 4;
            this.ExportNodes.Text = "Nodes";
            this.ExportNodes.UseVisualStyleBackColor = true;
            // 
            // ExportNodetypes
            // 
            this.ExportNodetypes.AutoSize = true;
            this.ExportNodetypes.Checked = true;
            this.ExportNodetypes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ExportNodetypes.Location = new System.Drawing.Point( 160, 36 );
            this.ExportNodetypes.Name = "ExportNodetypes";
            this.ExportNodetypes.Size = new System.Drawing.Size( 124, 17 );
            this.ExportNodetypes.TabIndex = 3;
            this.ExportNodetypes.Text = "Nodetype Definitions";
            this.ExportNodetypes.UseVisualStyleBackColor = true;
            // 
            // ExportButton
            // 
            this.ExportButton.Location = new System.Drawing.Point( 126, 281 );
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size( 75, 23 );
            this.ExportButton.TabIndex = 2;
            this.ExportButton.Text = "Export";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler( this.ExportButton_Click );
            // 
            // ExportSchemaSelectBox
            // 
            this.ExportSchemaSelectBox.FormattingEnabled = true;
            this.ExportSchemaSelectBox.Location = new System.Drawing.Point( 160, 9 );
            this.ExportSchemaSelectBox.Name = "ExportSchemaSelectBox";
            this.ExportSchemaSelectBox.Size = new System.Drawing.Size( 179, 21 );
            this.ExportSchemaSelectBox.TabIndex = 1;
            // 
            // ExportSchemaLabel
            // 
            this.ExportSchemaLabel.AutoSize = true;
            this.ExportSchemaLabel.Location = new System.Drawing.Point( 23, 12 );
            this.ExportSchemaLabel.Name = "ExportSchemaLabel";
            this.ExportSchemaLabel.Size = new System.Drawing.Size( 131, 13 );
            this.ExportSchemaLabel.TabIndex = 0;
            this.ExportSchemaLabel.Text = "Select a Schema to Save:";
            // 
            // ImportPage
            // 
            this.ImportPage.Controls.Add( this.btn_ResetSchema );
            this.ImportPage.Controls.Add( this.PhaseTextBox );
            this.ImportPage.Controls.Add( this.FileTypeSelectBox );
            this.ImportPage.Controls.Add( this.label2 );
            this.ImportPage.Controls.Add( this.DataFileLink );
            this.ImportPage.Controls.Add( this.label1 );
            this.ImportPage.Controls.Add( this.gbx_PhaseStatus );
            this.ImportPage.Controls.Add( this.ModeComboBox );
            this.ImportPage.Controls.Add( this.DataFileLabel );
            this.ImportPage.Controls.Add( this.ImportButton );
            this.ImportPage.Controls.Add( this.InitSchemaSelectBox );
            this.ImportPage.Controls.Add( this.InitSchemaLabel );
            this.ImportPage.Location = new System.Drawing.Point( 4, 22 );
            this.ImportPage.Name = "ImportPage";
            this.ImportPage.Padding = new System.Windows.Forms.Padding( 3 );
            this.ImportPage.Size = new System.Drawing.Size( 419, 348 );
            this.ImportPage.TabIndex = 0;
            this.ImportPage.Text = "Import";
            this.ImportPage.UseVisualStyleBackColor = true;
            // 
            // FileTypeSelectBox
            // 
            this.FileTypeSelectBox.FormattingEnabled = true;
            this.FileTypeSelectBox.Items.AddRange( new object[] {
            "IMCS Desktop Export XML File",
            "NBT Export XML File"} );
            this.FileTypeSelectBox.Location = new System.Drawing.Point( 92, 34 );
            this.FileTypeSelectBox.Name = "FileTypeSelectBox";
            this.FileTypeSelectBox.Size = new System.Drawing.Size( 183, 21 );
            this.FileTypeSelectBox.TabIndex = 21;
            this.FileTypeSelectBox.Text = "IMCS Desktop Export XML File";
            this.FileTypeSelectBox.SelectedIndexChanged += new System.EventHandler( this.FileTypeSelectBox_OnChange );
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 33, 37 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 53, 13 );
            this.label2.TabIndex = 20;
            this.label2.Text = "File Type:";
            // 
            // DataFileLink
            // 
            this.DataFileLink.AutoSize = true;
            this.DataFileLink.Location = new System.Drawing.Point( 92, 9 );
            this.DataFileLink.Name = "DataFileLink";
            this.DataFileLink.Size = new System.Drawing.Size( 52, 13 );
            this.DataFileLink.TabIndex = 19;
            this.DataFileLink.TabStop = true;
            this.DataFileLink.Text = "Choose...";
            this.DataFileLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler( this.DataFileLink_LinkClicked );
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 49, 91 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 37, 13 );
            this.label1.TabIndex = 17;
            this.label1.Text = "Mode:";
            // 
            // ModeComboBox
            // 
            this.ModeComboBox.FormattingEnabled = true;
            this.ModeComboBox.Items.AddRange( new object[] {
            "Update",
            "Duplicate",
            "Overwrite"} );
            this.ModeComboBox.Location = new System.Drawing.Point( 92, 88 );
            this.ModeComboBox.Name = "ModeComboBox";
            this.ModeComboBox.Size = new System.Drawing.Size( 119, 21 );
            this.ModeComboBox.TabIndex = 16;
            this.ModeComboBox.Text = "Duplicate";
            // 
            // DataFileLabel
            // 
            this.DataFileLabel.AutoSize = true;
            this.DataFileLabel.Location = new System.Drawing.Point( 34, 9 );
            this.DataFileLabel.Name = "DataFileLabel";
            this.DataFileLabel.Size = new System.Drawing.Size( 52, 13 );
            this.DataFileLabel.TabIndex = 13;
            this.DataFileLabel.Text = "Data File:";
            // 
            // ResultsTextBox
            // 
            this.ResultsTextBox.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.ResultsTextBox.Location = new System.Drawing.Point( 436, 27 );
            this.ResultsTextBox.Multiline = true;
            this.ResultsTextBox.Name = "ResultsTextBox";
            this.ResultsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ResultsTextBox.Size = new System.Drawing.Size( 349, 352 );
            this.ResultsTextBox.TabIndex = 18;
            // 
            // ErrorLabel
            // 
            this.ErrorLabel.AutoSize = true;
            this.ErrorLabel.ForeColor = System.Drawing.Color.Red;
            this.ErrorLabel.Location = new System.Drawing.Point( 6, 371 );
            this.ErrorLabel.Name = "ErrorLabel";
            this.ErrorLabel.Size = new System.Drawing.Size( 0, 13 );
            this.ErrorLabel.TabIndex = 11;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // PhaseTextBox
            // 
            this.PhaseTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PhaseTextBox.Enabled = false;
            this.PhaseTextBox.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte) ( 0 ) ) );
            this.PhaseTextBox.Location = new System.Drawing.Point( 14, 148 );
            this.PhaseTextBox.Multiline = true;
            this.PhaseTextBox.Name = "PhaseTextBox";
            this.PhaseTextBox.Size = new System.Drawing.Size( 397, 180 );
            this.PhaseTextBox.TabIndex = 19;
            // 
            // gbx_PhaseStatus
            // 
            this.gbx_PhaseStatus.Location = new System.Drawing.Point( 6, 129 );
            this.gbx_PhaseStatus.Name = "gbx_PhaseStatus";
            this.gbx_PhaseStatus.Size = new System.Drawing.Size( 413, 213 );
            this.gbx_PhaseStatus.TabIndex = 20;
            this.gbx_PhaseStatus.TabStop = false;
            this.gbx_PhaseStatus.Text = "Process Status";
            // 
            // btn_ResetSchema
            // 
            this.btn_ResetSchema.Location = new System.Drawing.Point( 281, 32 );
            this.btn_ResetSchema.Name = "btn_ResetSchema";
            this.btn_ResetSchema.Size = new System.Drawing.Size( 48, 23 );
            this.btn_ResetSchema.TabIndex = 22;
            this.btn_ResetSchema.Text = "Reset";
            this.btn_ResetSchema.UseVisualStyleBackColor = true;
            this.btn_ResetSchema.Click += new System.EventHandler( this.btn_ResetSchema_Click );
            // 
            // ImporterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 791, 405 );
            this.Controls.Add( this.ErrorLabel );
            this.Controls.Add( this.tabControl1 );
            this.Controls.Add( this.ResultsTextBox );
            this.Name = "ImporterForm";
            this.Text = "NBT Schema Importer";
            this.tabControl1.ResumeLayout( false );
            this.ExportTab.ResumeLayout( false );
            this.ExportTab.PerformLayout();
            this.ImportPage.ResumeLayout( false );
            this.ImportPage.PerformLayout();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label InitSchemaLabel;
        private System.Windows.Forms.ComboBox InitSchemaSelectBox;
        private System.Windows.Forms.Button ImportButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage ImportPage;
        private System.Windows.Forms.TabPage ExportTab;
        private System.Windows.Forms.ComboBox ExportSchemaSelectBox;
        private System.Windows.Forms.Label ExportSchemaLabel;
        private System.Windows.Forms.Label ExportIncludeLabel;
        private System.Windows.Forms.CheckBox ExportViews;
        private System.Windows.Forms.CheckBox ExportNodes;
        private System.Windows.Forms.CheckBox ExportNodetypes;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label InProgressLabel;
        private System.Windows.Forms.Label ErrorLabel;
        private System.Windows.Forms.Label ExportCompletedLabel;
        private System.Windows.Forms.Label DataFileLabel;
        private System.Windows.Forms.CheckedListBox NodeTypeCheckedListBox;
        private System.Windows.Forms.Button CheckAllButton;
        private System.Windows.Forms.Button UnCheckAllButton;
        //private System.Windows.Forms.CheckBox ClearExistingCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ModeComboBox;
        private System.Windows.Forms.TextBox ResultsTextBox;
        private System.Windows.Forms.LinkLabel DataFileLink;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ComboBox FileTypeSelectBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PhaseTextBox;
        private System.Windows.Forms.GroupBox gbx_PhaseStatus;
        private System.Windows.Forms.Button btn_ResetSchema;
    }
}

