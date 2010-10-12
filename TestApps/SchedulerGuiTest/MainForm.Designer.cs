namespace SchedulerGuiTest
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
            this.btn_Test = new System.Windows.Forms.Button();
            this.btn_Stop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_Test
            // 
            this.btn_Test.Location = new System.Drawing.Point( 29, 29 );
            this.btn_Test.Name = "btn_Test";
            this.btn_Test.Size = new System.Drawing.Size( 49, 23 );
            this.btn_Test.TabIndex = 0;
            this.btn_Test.Text = "Start";
            this.btn_Test.UseVisualStyleBackColor = true;
            this.btn_Test.Click += new System.EventHandler( this.btn_Test_Click );
            // 
            // btn_Stop
            // 
            this.btn_Stop.Location = new System.Drawing.Point( 29, 71 );
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Size = new System.Drawing.Size( 49, 23 );
            this.btn_Stop.TabIndex = 1;
            this.btn_Stop.Text = "Stop";
            this.btn_Stop.UseVisualStyleBackColor = true;
            this.btn_Stop.Click += new System.EventHandler( this.btn_Stop_Click );
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 654, 266 );
            this.Controls.Add( this.btn_Stop );
            this.Controls.Add( this.btn_Test );
            this.Name = "MainForm";
            this.Text = "NBT Scheduler Test";
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.Button btn_Test;
        private System.Windows.Forms.Button btn_Stop;
    }
}

