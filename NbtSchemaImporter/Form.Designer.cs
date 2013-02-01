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
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.ErrorLabel = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.FileTypeSelectBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.DataFileLink = new System.Windows.Forms.LinkLabel();
            this.ModeLabel = new System.Windows.Forms.Label();
            this.ModeComboBox = new System.Windows.Forms.ComboBox();
            this.DataFileLabel = new System.Windows.Forms.Label();
            this.ImportButton = new System.Windows.Forms.Button();
            this.InitSchemaSelectBox = new System.Windows.Forms.ComboBox();
            this.InitSchemaLabel = new System.Windows.Forms.Label();
            this.gbxMessages = new System.Windows.Forms.GroupBox();
            this.ResultsTextBox = new System.Windows.Forms.TextBox();
            this.PhaseTextBox = new System.Windows.Forms.TextBox();
            this.btn_Types = new System.Windows.Forms.Button();
            this.gbxMessages.SuspendLayout();
            this.SuspendLayout();
            // 
            // ErrorLabel
            // 
            this.ErrorLabel.AutoSize = true;
            this.ErrorLabel.ForeColor = System.Drawing.Color.Red;
            this.ErrorLabel.Location = new System.Drawing.Point(6, 371);
            this.ErrorLabel.Name = "ErrorLabel";
            this.ErrorLabel.Size = new System.Drawing.Size(0, 13);
            this.ErrorLabel.TabIndex = 11;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // FileTypeSelectBox
            // 
            this.FileTypeSelectBox.FormattingEnabled = true;
            this.FileTypeSelectBox.Items.AddRange(new object[] {
            "Post-Ripper style XML",
            "Rapid-Loader Style XLS"});
            this.FileTypeSelectBox.Location = new System.Drawing.Point(96, 25);
            this.FileTypeSelectBox.Name = "FileTypeSelectBox";
            this.FileTypeSelectBox.Size = new System.Drawing.Size(183, 21);
            this.FileTypeSelectBox.TabIndex = 21;
            this.FileTypeSelectBox.Text = "Post-Ripper style XML";
            this.FileTypeSelectBox.SelectedIndexChanged += new System.EventHandler(this.FileTypeSelectBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "File Type:";
            // 
            // DataFileLink
            // 
            this.DataFileLink.AutoSize = true;
            this.DataFileLink.Location = new System.Drawing.Point(96, 8);
            this.DataFileLink.Name = "DataFileLink";
            this.DataFileLink.Size = new System.Drawing.Size(52, 13);
            this.DataFileLink.TabIndex = 19;
            this.DataFileLink.TabStop = true;
            this.DataFileLink.Text = "Choose...";
            this.DataFileLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DataFileLink_LinkClicked);
            // 
            // ModeLabel
            // 
            this.ModeLabel.AutoSize = true;
            this.ModeLabel.Location = new System.Drawing.Point(45, 82);
            this.ModeLabel.Name = "ModeLabel";
            this.ModeLabel.Size = new System.Drawing.Size(37, 13);
            this.ModeLabel.TabIndex = 17;
            this.ModeLabel.Text = "Mode:";
            // 
            // ModeComboBox
            // 
            this.ModeComboBox.FormattingEnabled = true;
            this.ModeComboBox.Items.AddRange(new object[] {
            "Update",
            "Duplicate",
            "Overwrite"});
            this.ModeComboBox.Location = new System.Drawing.Point(96, 79);
            this.ModeComboBox.Name = "ModeComboBox";
            this.ModeComboBox.Size = new System.Drawing.Size(119, 21);
            this.ModeComboBox.TabIndex = 16;
            this.ModeComboBox.Text = "Duplicate";
            // 
            // DataFileLabel
            // 
            this.DataFileLabel.AutoSize = true;
            this.DataFileLabel.Location = new System.Drawing.Point(37, 8);
            this.DataFileLabel.Name = "DataFileLabel";
            this.DataFileLabel.Size = new System.Drawing.Size(52, 13);
            this.DataFileLabel.TabIndex = 13;
            this.DataFileLabel.Text = "Data File:";
            // 
            // ImportButton
            // 
            this.ImportButton.Location = new System.Drawing.Point(373, 28);
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.Size = new System.Drawing.Size(80, 23);
            this.ImportButton.TabIndex = 7;
            this.ImportButton.Text = "Start";
            this.ImportButton.UseVisualStyleBackColor = true;
            this.ImportButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // InitSchemaSelectBox
            // 
            this.InitSchemaSelectBox.FormattingEnabled = true;
            this.InitSchemaSelectBox.Location = new System.Drawing.Point(96, 52);
            this.InitSchemaSelectBox.Name = "InitSchemaSelectBox";
            this.InitSchemaSelectBox.Size = new System.Drawing.Size(183, 21);
            this.InitSchemaSelectBox.TabIndex = 3;
            // 
            // InitSchemaLabel
            // 
            this.InitSchemaLabel.AutoSize = true;
            this.InitSchemaLabel.Location = new System.Drawing.Point(7, 55);
            this.InitSchemaLabel.Name = "InitSchemaLabel";
            this.InitSchemaLabel.Size = new System.Drawing.Size(83, 13);
            this.InitSchemaLabel.TabIndex = 2;
            this.InitSchemaLabel.Text = "Target Schema:";
            // 
            // gbxMessages
            // 
            this.gbxMessages.Controls.Add(this.ResultsTextBox);
            this.gbxMessages.Location = new System.Drawing.Point(15, 196);
            this.gbxMessages.Name = "gbxMessages";
            this.gbxMessages.Size = new System.Drawing.Size(438, 327);
            this.gbxMessages.TabIndex = 22;
            this.gbxMessages.TabStop = false;
            this.gbxMessages.Text = "Messages";
            // 
            // ResultsTextBox
            // 
            this.ResultsTextBox.Location = new System.Drawing.Point(6, 19);
            this.ResultsTextBox.Multiline = true;
            this.ResultsTextBox.Name = "ResultsTextBox";
            this.ResultsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ResultsTextBox.Size = new System.Drawing.Size(423, 292);
            this.ResultsTextBox.TabIndex = 18;
            // 
            // PhaseTextBox
            // 
            this.PhaseTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PhaseTextBox.Enabled = false;
            this.PhaseTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PhaseTextBox.Location = new System.Drawing.Point(24, 117);
            this.PhaseTextBox.Multiline = true;
            this.PhaseTextBox.Name = "PhaseTextBox";
            this.PhaseTextBox.Size = new System.Drawing.Size(423, 73);
            this.PhaseTextBox.TabIndex = 23;
            // 
            // btn_Types
            // 
            this.btn_Types.Location = new System.Drawing.Point(377, 88);
            this.btn_Types.Name = "btn_Types";
            this.btn_Types.Size = new System.Drawing.Size(75, 23);
            this.btn_Types.TabIndex = 24;
            this.btn_Types.Text = "Types";
            this.btn_Types.UseVisualStyleBackColor = true;
            this.btn_Types.Click += new System.EventHandler(this.btn_Types_Click);
            // 
            // ImporterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 535);
            this.Controls.Add(this.btn_Types);
            this.Controls.Add(this.PhaseTextBox);
            this.Controls.Add(this.gbxMessages);
            this.Controls.Add(this.FileTypeSelectBox);
            this.Controls.Add(this.ErrorLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DataFileLink);
            this.Controls.Add(this.DataFileLabel);
            this.Controls.Add(this.ModeLabel);
            this.Controls.Add(this.InitSchemaLabel);
            this.Controls.Add(this.InitSchemaSelectBox);
            this.Controls.Add(this.ModeComboBox);
            this.Controls.Add(this.ImportButton);
            this.Name = "ImporterForm";
            this.Text = "NBT Schema Importer";
            this.gbxMessages.ResumeLayout(false);
            this.gbxMessages.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label ErrorLabel;
        //private System.Windows.Forms.CheckBox ClearExistingCheckBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ComboBox FileTypeSelectBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel DataFileLink;
        private System.Windows.Forms.Label ModeLabel;
        private System.Windows.Forms.ComboBox ModeComboBox;
        private System.Windows.Forms.Label DataFileLabel;
        private System.Windows.Forms.Button ImportButton;
        private System.Windows.Forms.ComboBox InitSchemaSelectBox;
        private System.Windows.Forms.Label InitSchemaLabel;
        private System.Windows.Forms.GroupBox gbxMessages;
        private System.Windows.Forms.TextBox ResultsTextBox;
        private System.Windows.Forms.TextBox PhaseTextBox;
        private System.Windows.Forms.Button btn_Types;
    }
}

