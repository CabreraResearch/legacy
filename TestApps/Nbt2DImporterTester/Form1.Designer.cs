namespace Nbt2DImporterTester
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnLoadData = new System.Windows.Forms.Button();
            this.btnRunImport = new System.Windows.Forms.Button();
            this.txtDataFilePath = new System.Windows.Forms.TextBox();
            this.txtBindingsFilePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnReadBindings = new System.Windows.Forms.Button();
            this.txtRows = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbxImportDataTableName = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblPending = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cbVerbose = new System.Windows.Forms.CheckBox();
            this.cbContinuous = new System.Windows.Forms.CheckBox();
            this.btnRefreshCounts = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnLoadData
            // 
            this.btnLoadData.Enabled = false;
            this.btnLoadData.Location = new System.Drawing.Point(15, 139);
            this.btnLoadData.Name = "btnLoadData";
            this.btnLoadData.Size = new System.Drawing.Size(90, 27);
            this.btnLoadData.TabIndex = 0;
            this.btnLoadData.Text = "Load Data";
            this.btnLoadData.UseVisualStyleBackColor = true;
            this.btnLoadData.Click += new System.EventHandler( this.btnLoadData_Click );
            // 
            // btnRunImport
            // 
            this.btnRunImport.Enabled = false;
            this.btnRunImport.Location = new System.Drawing.Point(15, 238);
            this.btnRunImport.Name = "btnRunImport";
            this.btnRunImport.Size = new System.Drawing.Size(90, 27);
            this.btnRunImport.TabIndex = 1;
            this.btnRunImport.Text = "Run Import";
            this.btnRunImport.UseVisualStyleBackColor = true;
            this.btnRunImport.Click += new System.EventHandler( this.btnRunImport_Click );
            // 
            // txtDataFilePath
            // 
            this.txtDataFilePath.Location = new System.Drawing.Point(12, 113);
            this.txtDataFilePath.Name = "txtDataFilePath";
            this.txtDataFilePath.Size = new System.Drawing.Size(384, 20);
            this.txtDataFilePath.TabIndex = 3;
            this.txtDataFilePath.Text = "Z:\\D\\temp\\imcs_export.xlsx";
            // 
            // txtBindingsFilePath
            // 
            this.txtBindingsFilePath.Location = new System.Drawing.Point(12, 25);
            this.txtBindingsFilePath.Name = "txtBindingsFilePath";
            this.txtBindingsFilePath.Size = new System.Drawing.Size(384, 20);
            this.txtBindingsFilePath.TabIndex = 4;
            this.txtBindingsFilePath.Text = "Z:\\D\\temp\\imcs_export_bindings.xlsx";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Data File";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Bindings File";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 197);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Import Data Table Name";
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(402, 12);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(482, 300);
            this.txtLog.TabIndex = 8;
            // 
            // btnReadBindings
            // 
            this.btnReadBindings.Location = new System.Drawing.Point(15, 50);
            this.btnReadBindings.Name = "btnReadBindings";
            this.btnReadBindings.Size = new System.Drawing.Size(90, 27);
            this.btnReadBindings.TabIndex = 9;
            this.btnReadBindings.Text = "Read Bindings";
            this.btnReadBindings.UseVisualStyleBackColor = true;
            this.btnReadBindings.Click += new System.EventHandler( this.btnReadBindings_Click );
            // 
            // txtRows
            // 
            this.txtRows.Location = new System.Drawing.Point(125, 242);
            this.txtRows.Name = "txtRows";
            this.txtRows.Size = new System.Drawing.Size(30, 20);
            this.txtRows.TabIndex = 10;
            this.txtRows.Text = "10";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(161, 245);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Rows to Run";
            // 
            // cbxImportDataTableName
            // 
            this.cbxImportDataTableName.FormattingEnabled = true;
            this.cbxImportDataTableName.Location = new System.Drawing.Point(12, 213);
            this.cbxImportDataTableName.Name = "cbxImportDataTableName";
            this.cbxImportDataTableName.Size = new System.Drawing.Size(384, 21);
            this.cbxImportDataTableName.TabIndex = 12;
            this.cbxImportDataTableName.TextChanged += new System.EventHandler(this.cbxImportDataTableName_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(122, 279);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Pending Rows:";
            // 
            // lblPending
            // 
            this.lblPending.AutoSize = true;
            this.lblPending.Location = new System.Drawing.Point(207, 279);
            this.lblPending.Name = "lblPending";
            this.lblPending.Size = new System.Drawing.Size(13, 13);
            this.lblPending.TabIndex = 14;
            this.lblPending.Text = "?";
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(207, 301);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(13, 13);
            this.lblError.TabIndex = 16;
            this.lblError.Text = "?";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(139, 301);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Error Rows:";
            // 
            // cbVerbose
            // 
            this.cbVerbose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbVerbose.AutoSize = true;
            this.cbVerbose.Checked = true;
            this.cbVerbose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbVerbose.Location = new System.Drawing.Point(819, 318);
            this.cbVerbose.Name = "cbVerbose";
            this.cbVerbose.Size = new System.Drawing.Size(65, 17);
            this.cbVerbose.TabIndex = 17;
            this.cbVerbose.Text = "Verbose";
            this.cbVerbose.UseVisualStyleBackColor = true;
            // 
            // cbContinuous
            // 
            this.cbContinuous.AutoSize = true;
            this.cbContinuous.Location = new System.Drawing.Point(248, 244);
            this.cbContinuous.Name = "cbContinuous";
            this.cbContinuous.Size = new System.Drawing.Size(79, 17);
            this.cbContinuous.TabIndex = 18;
            this.cbContinuous.Text = "Continuous";
            this.cbContinuous.UseVisualStyleBackColor = true;
            // 
            // btnRefreshCounts
            // 
            this.btnRefreshCounts.Enabled = false;
            this.btnRefreshCounts.Image = ((System.Drawing.Image)(resources.GetObject("btnRefreshCounts.Image")));
            this.btnRefreshCounts.Location = new System.Drawing.Point(248, 279);
            this.btnRefreshCounts.Name = "btnRefreshCounts";
            this.btnRefreshCounts.Size = new System.Drawing.Size(41, 35);
            this.btnRefreshCounts.TabIndex = 19;
            this.btnRefreshCounts.UseVisualStyleBackColor = true;
            this.btnRefreshCounts.Click += new System.EventHandler( this.btnRefreshCounts_Click );
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(896, 340);
            this.Controls.Add(this.btnRefreshCounts);
            this.Controls.Add(this.cbContinuous);
            this.Controls.Add(this.cbVerbose);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lblPending);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cbxImportDataTableName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtRows);
            this.Controls.Add(this.btnReadBindings);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBindingsFilePath);
            this.Controls.Add(this.txtDataFilePath);
            this.Controls.Add(this.btnRunImport);
            this.Controls.Add(this.btnLoadData);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoadData;
        private System.Windows.Forms.Button btnRunImport;
        private System.Windows.Forms.TextBox txtDataFilePath;
        private System.Windows.Forms.TextBox txtBindingsFilePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnReadBindings;
        private System.Windows.Forms.TextBox txtRows;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbxImportDataTableName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblPending;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox cbVerbose;
        private System.Windows.Forms.CheckBox cbContinuous;
        private System.Windows.Forms.Button btnRefreshCounts;
    }
}

