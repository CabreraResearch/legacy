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
            if ( disposing && ( components != null ) )
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
            this.tbxMessages = new System.Windows.Forms.TextBox();
            this.btn_Test = new System.Windows.Forms.Button();
            this.cbxl_TestToRun = new System.Windows.Forms.CheckedListBox();
            this.btn_CheckAll = new System.Windows.Forms.Button();
            this.btn_UncheckAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbxMessages
            // 
            this.tbxMessages.Location = new System.Drawing.Point( 324, 17 );
            this.tbxMessages.Multiline = true;
            this.tbxMessages.Name = "tbxMessages";
            this.tbxMessages.Size = new System.Drawing.Size( 886, 424 );
            this.tbxMessages.TabIndex = 0;
            // 
            // btn_Test
            // 
            this.btn_Test.Location = new System.Drawing.Point( 6, 455 );
            this.btn_Test.Name = "btn_Test";
            this.btn_Test.Size = new System.Drawing.Size( 75, 23 );
            this.btn_Test.TabIndex = 1;
            this.btn_Test.Text = "Run Tests";
            this.btn_Test.UseVisualStyleBackColor = true;
            this.btn_Test.Click += new System.EventHandler( this.btn_Test_Click );
            // 
            // cbxl_TestToRun
            // 
            this.cbxl_TestToRun.FormattingEnabled = true;
            this.cbxl_TestToRun.Location = new System.Drawing.Point( 6, 17 );
            this.cbxl_TestToRun.Name = "cbxl_TestToRun";
            this.cbxl_TestToRun.Size = new System.Drawing.Size( 291, 424 );
            this.cbxl_TestToRun.TabIndex = 3;
            // 
            // btn_CheckAll
            // 
            this.btn_CheckAll.Location = new System.Drawing.Point( 142, 455 );
            this.btn_CheckAll.Name = "btn_CheckAll";
            this.btn_CheckAll.Size = new System.Drawing.Size( 75, 23 );
            this.btn_CheckAll.TabIndex = 4;
            this.btn_CheckAll.Text = "Check All";
            this.btn_CheckAll.UseVisualStyleBackColor = true;
            this.btn_CheckAll.Click += new System.EventHandler( this.btn_CheckAll_Click );
            // 
            // btn_UncheckAll
            // 
            this.btn_UncheckAll.Location = new System.Drawing.Point( 224, 455 );
            this.btn_UncheckAll.Name = "btn_UncheckAll";
            this.btn_UncheckAll.Size = new System.Drawing.Size( 74, 23 );
            this.btn_UncheckAll.TabIndex = 5;
            this.btn_UncheckAll.Text = "Uncheck All";
            this.btn_UncheckAll.UseVisualStyleBackColor = true;
            this.btn_UncheckAll.Click += new System.EventHandler( this.btn_UncheckAll_Click );
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 1211, 481 );
            this.Controls.Add( this.btn_UncheckAll );
            this.Controls.Add( this.btn_CheckAll );
            this.Controls.Add( this.cbxl_TestToRun );
            this.Controls.Add( this.btn_Test );
            this.Controls.Add( this.tbxMessages );
            this.Name = "MainForm";
            this.Text = "Schema Update Auto Test";
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbxMessages;
        private System.Windows.Forms.Button btn_Test;
        private System.Windows.Forms.CheckedListBox cbxl_TestToRun;
        private System.Windows.Forms.Button btn_CheckAll;
        private System.Windows.Forms.Button btn_UncheckAll;
    }
}

