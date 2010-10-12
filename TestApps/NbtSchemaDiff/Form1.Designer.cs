namespace ChemSW.NbtSchemaDiff
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
            this.LeftSchemaSelectBox = new System.Windows.Forms.ComboBox();
            this.RightSchemaSelectBox = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.CompareButton = new System.Windows.Forms.Button();
            this.ShowUnmatchedCheckBox = new System.Windows.Forms.CheckBox();
            this.ShowDiffsOnlyCheckBox = new System.Windows.Forms.CheckBox();
            this.KeyDataGridView = new System.Windows.Forms.DataGridView();
            this.KeyLabel = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.ShowAllOrphans = new System.Windows.Forms.CheckBox();
            this.ToleranceBox = new ChemSW.CswFormsControls.CswLabeledTextBox();
            ( (System.ComponentModel.ISupportInitialize) ( this.dataGridView1 ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize) ( this.KeyDataGridView ) ).BeginInit();
            this.SuspendLayout();
            // 
            // LeftSchemaSelectBox
            // 
            this.LeftSchemaSelectBox.FormattingEnabled = true;
            this.LeftSchemaSelectBox.Location = new System.Drawing.Point( 13, 12 );
            this.LeftSchemaSelectBox.Name = "LeftSchemaSelectBox";
            this.LeftSchemaSelectBox.Size = new System.Drawing.Size( 185, 21 );
            this.LeftSchemaSelectBox.TabIndex = 0;
            // 
            // RightSchemaSelectBox
            // 
            this.RightSchemaSelectBox.FormattingEnabled = true;
            this.RightSchemaSelectBox.Location = new System.Drawing.Point( 204, 12 );
            this.RightSchemaSelectBox.Name = "RightSchemaSelectBox";
            this.RightSchemaSelectBox.Size = new System.Drawing.Size( 185, 21 );
            this.RightSchemaSelectBox.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point( 13, 85 );
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size( 1031, 504 );
            this.dataGridView1.TabIndex = 2;
            // 
            // CompareButton
            // 
            this.CompareButton.Location = new System.Drawing.Point( 153, 56 );
            this.CompareButton.Name = "CompareButton";
            this.CompareButton.Size = new System.Drawing.Size( 107, 23 );
            this.CompareButton.TabIndex = 3;
            this.CompareButton.Text = "Compare";
            this.CompareButton.UseVisualStyleBackColor = true;
            this.CompareButton.Click += new System.EventHandler( this.CompareButton_Click );
            // 
            // ShowUnmatchedCheckBox
            // 
            this.ShowUnmatchedCheckBox.AutoSize = true;
            this.ShowUnmatchedCheckBox.Checked = true;
            this.ShowUnmatchedCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowUnmatchedCheckBox.Location = new System.Drawing.Point( 269, 38 );
            this.ShowUnmatchedCheckBox.Name = "ShowUnmatchedCheckBox";
            this.ShowUnmatchedCheckBox.Size = new System.Drawing.Size( 145, 17 );
            this.ShowUnmatchedCheckBox.TabIndex = 5;
            this.ShowUnmatchedCheckBox.Text = "Show Unmatched Nodes";
            this.ShowUnmatchedCheckBox.UseVisualStyleBackColor = true;
            // 
            // ShowDiffsOnlyCheckBox
            // 
            this.ShowDiffsOnlyCheckBox.AutoSize = true;
            this.ShowDiffsOnlyCheckBox.Location = new System.Drawing.Point( 13, 38 );
            this.ShowDiffsOnlyCheckBox.Name = "ShowDiffsOnlyCheckBox";
            this.ShowDiffsOnlyCheckBox.Size = new System.Drawing.Size( 134, 17 );
            this.ShowDiffsOnlyCheckBox.TabIndex = 6;
            this.ShowDiffsOnlyCheckBox.Text = "Show Differences Only";
            this.ShowDiffsOnlyCheckBox.UseVisualStyleBackColor = true;
            // 
            // KeyDataGridView
            // 
            this.KeyDataGridView.AllowUserToAddRows = false;
            this.KeyDataGridView.AllowUserToDeleteRows = false;
            this.KeyDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.KeyDataGridView.Location = new System.Drawing.Point( 456, 12 );
            this.KeyDataGridView.Name = "KeyDataGridView";
            this.KeyDataGridView.ReadOnly = true;
            this.KeyDataGridView.Size = new System.Drawing.Size( 588, 67 );
            this.KeyDataGridView.TabIndex = 7;
            // 
            // KeyLabel
            // 
            this.KeyLabel.AutoSize = true;
            this.KeyLabel.Location = new System.Drawing.Point( 422, 12 );
            this.KeyLabel.Name = "KeyLabel";
            this.KeyLabel.Size = new System.Drawing.Size( 28, 13 );
            this.KeyLabel.TabIndex = 8;
            this.KeyLabel.Text = "Key:";
            // 
            // ShowAllOrphans
            // 
            this.ShowAllOrphans.AutoSize = true;
            this.ShowAllOrphans.Location = new System.Drawing.Point( 153, 38 );
            this.ShowAllOrphans.Name = "ShowAllOrphans";
            this.ShowAllOrphans.Size = new System.Drawing.Size( 110, 17 );
            this.ShowAllOrphans.TabIndex = 9;
            this.ShowAllOrphans.Text = "Show All Orphans";
            this.ShowAllOrphans.UseVisualStyleBackColor = true;
            // 
            // ToleranceBox
            // 
            this.ToleranceBox.LabelText = "Match Tolerance (%):";
            this.ToleranceBox.LabelWidth = 120;
            this.ToleranceBox.Location = new System.Drawing.Point( 269, 56 );
            this.ToleranceBox.Name = "ToleranceBox";
            this.ToleranceBox.Size = new System.Drawing.Size( 181, 23 );
            this.ToleranceBox.TabIndex = 10;
            this.ToleranceBox.Text = "20";
            this.ToleranceBox.TextBoxWidth = 50;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 1056, 601 );
            this.Controls.Add( this.ToleranceBox );
            this.Controls.Add( this.ShowAllOrphans );
            this.Controls.Add( this.KeyLabel );
            this.Controls.Add( this.KeyDataGridView );
            this.Controls.Add( this.ShowDiffsOnlyCheckBox );
            this.Controls.Add( this.ShowUnmatchedCheckBox );
            this.Controls.Add( this.CompareButton );
            this.Controls.Add( this.dataGridView1 );
            this.Controls.Add( this.RightSchemaSelectBox );
            this.Controls.Add( this.LeftSchemaSelectBox );
            this.Name = "Form1";
            this.Text = "NBT Schema Diff";
            ( (System.ComponentModel.ISupportInitialize) ( this.dataGridView1 ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize) ( this.KeyDataGridView ) ).EndInit();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox LeftSchemaSelectBox;
        private System.Windows.Forms.ComboBox RightSchemaSelectBox;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button CompareButton;
        private System.Windows.Forms.CheckBox ShowUnmatchedCheckBox;
        private System.Windows.Forms.CheckBox ShowDiffsOnlyCheckBox;
        private System.Windows.Forms.DataGridView KeyDataGridView;
        private System.Windows.Forms.Label KeyLabel;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.CheckBox ShowAllOrphans;
        private ChemSW.CswFormsControls.CswLabeledTextBox ToleranceBox;
    }
}

