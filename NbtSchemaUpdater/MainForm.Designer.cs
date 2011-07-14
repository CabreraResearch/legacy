namespace ChemSW.Nbt.Schema
{
    partial class MainForm
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
            this.schemalistboxlabel = new System.Windows.Forms.Label();
            this.currentschemaversionlabel = new System.Windows.Forms.Label();
            this.currentschemaversion = new System.Windows.Forms.Label();
            this.updatetoschemaversionlabel = new System.Windows.Forms.Label();
            this.updatetoschemaversion = new System.Windows.Forms.Label();
            this.updatebutton = new System.Windows.Forms.Button();
            this.schemacombobox = new System.Windows.Forms.ComboBox();
            this.minimumversionlabel = new System.Windows.Forms.Label();
            this.minimumversion = new System.Windows.Forms.Label();
            this.historygrid = new System.Windows.Forms.DataGridView();
            this.FetchButton = new System.Windows.Forms.Button();
            this.StatusTextBox = new System.Windows.Forms.TextBox();
            this.MyCancelButton = new System.Windows.Forms.Button();
            this.ClipBoardLink = new System.Windows.Forms.LinkLabel();
            ( (System.ComponentModel.ISupportInitialize) ( this.historygrid ) ).BeginInit();
            this.SuspendLayout();
            // 
            // schemalistboxlabel
            // 
            this.schemalistboxlabel.AutoSize = true;
            this.schemalistboxlabel.Location = new System.Drawing.Point( 9, 13 );
            this.schemalistboxlabel.Name = "schemalistboxlabel";
            this.schemalistboxlabel.Size = new System.Drawing.Size( 141, 13 );
            this.schemalistboxlabel.TabIndex = 0;
            this.schemalistboxlabel.Text = "Select a Schema to Update:";
            // 
            // currentschemaversionlabel
            // 
            this.currentschemaversionlabel.AutoSize = true;
            this.currentschemaversionlabel.Location = new System.Drawing.Point( 26, 67 );
            this.currentschemaversionlabel.Name = "currentschemaversionlabel";
            this.currentschemaversionlabel.Size = new System.Drawing.Size( 124, 13 );
            this.currentschemaversionlabel.TabIndex = 2;
            this.currentschemaversionlabel.Text = "Current Schema Version:";
            // 
            // currentschemaversion
            // 
            this.currentschemaversion.AutoSize = true;
            this.currentschemaversion.Location = new System.Drawing.Point( 152, 67 );
            this.currentschemaversion.Name = "currentschemaversion";
            this.currentschemaversion.Size = new System.Drawing.Size( 13, 13 );
            this.currentschemaversion.TabIndex = 3;
            this.currentschemaversion.Text = "0";
            // 
            // updatetoschemaversionlabel
            // 
            this.updatetoschemaversionlabel.AutoSize = true;
            this.updatetoschemaversionlabel.Location = new System.Drawing.Point( 13, 90 );
            this.updatetoschemaversionlabel.Name = "updatetoschemaversionlabel";
            this.updatetoschemaversionlabel.Size = new System.Drawing.Size( 137, 13 );
            this.updatetoschemaversionlabel.TabIndex = 4;
            this.updatetoschemaversionlabel.Text = "Update to Schema Version:";
            // 
            // updatetoschemaversion
            // 
            this.updatetoschemaversion.AutoSize = true;
            this.updatetoschemaversion.Location = new System.Drawing.Point( 152, 90 );
            this.updatetoschemaversion.Name = "updatetoschemaversion";
            this.updatetoschemaversion.Size = new System.Drawing.Size( 13, 13 );
            this.updatetoschemaversion.TabIndex = 5;
            this.updatetoschemaversion.Text = "0";
            // 
            // updatebutton
            // 
            this.updatebutton.Location = new System.Drawing.Point( 273, 44 );
            this.updatebutton.Name = "updatebutton";
            this.updatebutton.Size = new System.Drawing.Size( 75, 23 );
            this.updatebutton.TabIndex = 6;
            this.updatebutton.Text = "Update";
            this.updatebutton.UseVisualStyleBackColor = true;
            this.updatebutton.Click += new System.EventHandler( this.updatebutton_Click );
            // 
            // schemacombobox
            // 
            this.schemacombobox.FormattingEnabled = true;
            this.schemacombobox.Location = new System.Drawing.Point( 155, 10 );
            this.schemacombobox.Name = "schemacombobox";
            this.schemacombobox.Size = new System.Drawing.Size( 133, 21 );
            this.schemacombobox.TabIndex = 8;
            // 
            // minimumversionlabel
            // 
            this.minimumversionlabel.AutoSize = true;
            this.minimumversionlabel.Location = new System.Drawing.Point( 19, 44 );
            this.minimumversionlabel.Name = "minimumversionlabel";
            this.minimumversionlabel.Size = new System.Drawing.Size( 131, 13 );
            this.minimumversionlabel.TabIndex = 2;
            this.minimumversionlabel.Text = "Minimum Schema Version:";
            // 
            // minimumversion
            // 
            this.minimumversion.AutoSize = true;
            this.minimumversion.Location = new System.Drawing.Point( 152, 44 );
            this.minimumversion.Name = "minimumversion";
            this.minimumversion.Size = new System.Drawing.Size( 13, 13 );
            this.minimumversion.TabIndex = 3;
            this.minimumversion.Text = "0";
            // 
            // historygrid
            // 
            this.historygrid.AllowUserToAddRows = false;
            this.historygrid.AllowUserToDeleteRows = false;
            this.historygrid.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.historygrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.historygrid.Location = new System.Drawing.Point( 12, 229 );
            this.historygrid.Name = "historygrid";
            this.historygrid.ReadOnly = true;
            this.historygrid.Size = new System.Drawing.Size( 336, 143 );
            this.historygrid.TabIndex = 9;
            // 
            // FetchButton
            // 
            this.FetchButton.Location = new System.Drawing.Point( 295, 8 );
            this.FetchButton.Name = "FetchButton";
            this.FetchButton.Size = new System.Drawing.Size( 53, 23 );
            this.FetchButton.TabIndex = 10;
            this.FetchButton.Text = "Fetch";
            this.FetchButton.UseVisualStyleBackColor = true;
            this.FetchButton.Click += new System.EventHandler( this.FetchButton_Click );
            // 
            // StatusTextBox
            // 
            this.StatusTextBox.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.StatusTextBox.Location = new System.Drawing.Point( 13, 118 );
            this.StatusTextBox.Multiline = true;
            this.StatusTextBox.Name = "StatusTextBox";
            this.StatusTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.StatusTextBox.Size = new System.Drawing.Size( 335, 92 );
            this.StatusTextBox.TabIndex = 11;
            // 
            // CancelButton
            // 
            this.MyCancelButton.Location = new System.Drawing.Point( 273, 80 );
            this.MyCancelButton.Name = "CancelButton";
            this.MyCancelButton.Size = new System.Drawing.Size( 75, 23 );
            this.MyCancelButton.TabIndex = 12;
            this.MyCancelButton.Text = "Stop";
            this.MyCancelButton.UseVisualStyleBackColor = true;
            this.MyCancelButton.Click += new System.EventHandler( this.CancelButton_Click );
            // 
            // ClipBoardLink
            // 
            this.ClipBoardLink.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.ClipBoardLink.AutoSize = true;
            this.ClipBoardLink.Location = new System.Drawing.Point( 233, 213 );
            this.ClipBoardLink.Name = "ClipBoardLink";
            this.ClipBoardLink.Size = new System.Drawing.Size( 115, 13 );
            this.ClipBoardLink.TabIndex = 13;
            this.ClipBoardLink.TabStop = true;
            this.ClipBoardLink.Text = "Copy Error to Clipboard";
            this.ClipBoardLink.Visible = false;
            this.ClipBoardLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler( this.ClipBoardLink_LinkClicked );
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 360, 384 );
            this.Controls.Add( this.ClipBoardLink );
            this.Controls.Add( this.MyCancelButton );
            this.Controls.Add( this.StatusTextBox );
            this.Controls.Add( this.FetchButton );
            this.Controls.Add( this.historygrid );
            this.Controls.Add( this.schemacombobox );
            this.Controls.Add( this.updatebutton );
            this.Controls.Add( this.updatetoschemaversion );
            this.Controls.Add( this.updatetoschemaversionlabel );
            this.Controls.Add( this.minimumversion );
            this.Controls.Add( this.currentschemaversion );
            this.Controls.Add( this.minimumversionlabel );
            this.Controls.Add( this.currentschemaversionlabel );
            this.Controls.Add( this.schemalistboxlabel );
            this.Name = "MainForm";
            this.Text = "NBT Schema Updater";
            ( (System.ComponentModel.ISupportInitialize) ( this.historygrid ) ).EndInit();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label schemalistboxlabel;
        private System.Windows.Forms.Label currentschemaversionlabel;
        private System.Windows.Forms.Label currentschemaversion;
        private System.Windows.Forms.Label updatetoschemaversionlabel;
        private System.Windows.Forms.Label updatetoschemaversion;
        private System.Windows.Forms.Button updatebutton;
        private System.Windows.Forms.ComboBox schemacombobox;
        private System.Windows.Forms.Label minimumversionlabel;
        private System.Windows.Forms.Label minimumversion;
        private System.Windows.Forms.DataGridView historygrid;
        private System.Windows.Forms.Button FetchButton;
        private System.Windows.Forms.TextBox StatusTextBox;
        private System.Windows.Forms.Button MyCancelButton;
        private System.Windows.Forms.LinkLabel ClipBoardLink;
    }
}

