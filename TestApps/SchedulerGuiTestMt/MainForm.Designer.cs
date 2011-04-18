namespace SchedulerGuiTestMt
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
            this.components = new System.ComponentModel.Container();
            this.btn_Start = new System.Windows.Forms.Button();
            this.btn_Stop = new System.Windows.Forms.Button();
            this.lbl_Running = new System.Windows.Forms.Label();
            this.tbx_Secs = new System.Windows.Forms.TextBox();
            this.tmr_Seconds = new System.Windows.Forms.Timer( this.components );
            this.lbl_Status = new System.Windows.Forms.Label();
            this.tbx_Status = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btn_Start
            // 
            this.btn_Start.Location = new System.Drawing.Point( 12, 3 );
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size( 75, 23 );
            this.btn_Start.TabIndex = 0;
            this.btn_Start.Text = "Start";
            this.btn_Start.UseVisualStyleBackColor = true;
            this.btn_Start.Click += new System.EventHandler( this.btn_Start_Click );
            // 
            // btn_Stop
            // 
            this.btn_Stop.Location = new System.Drawing.Point( 12, 43 );
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Size = new System.Drawing.Size( 75, 23 );
            this.btn_Stop.TabIndex = 1;
            this.btn_Stop.Text = "Stop";
            this.btn_Stop.UseVisualStyleBackColor = true;
            this.btn_Stop.Click += new System.EventHandler( this.btn_Stop_Click );
            // 
            // lbl_Running
            // 
            this.lbl_Running.AutoSize = true;
            this.lbl_Running.Location = new System.Drawing.Point( 117, 5 );
            this.lbl_Running.Name = "lbl_Running";
            this.lbl_Running.Size = new System.Drawing.Size( 77, 13 );
            this.lbl_Running.TabIndex = 2;
            this.lbl_Running.Text = "Secs Running:";
            // 
            // tbx_Secs
            // 
            this.tbx_Secs.Enabled = false;
            this.tbx_Secs.Location = new System.Drawing.Point( 196, 3 );
            this.tbx_Secs.Name = "tbx_Secs";
            this.tbx_Secs.Size = new System.Drawing.Size( 78, 20 );
            this.tbx_Secs.TabIndex = 3;
            // 
            // tmr_Seconds
            // 
            this.tmr_Seconds.Tick += new System.EventHandler( this.TimerTick );
            // 
            // lbl_Status
            // 
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Location = new System.Drawing.Point( 120, 46 );
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size( 40, 13 );
            this.lbl_Status.TabIndex = 4;
            this.lbl_Status.Text = "Status:";
            // 
            // tbx_Status
            // 
            this.tbx_Status.Enabled = false;
            this.tbx_Status.Location = new System.Drawing.Point( 196, 43 );
            this.tbx_Status.Name = "tbx_Status";
            this.tbx_Status.Size = new System.Drawing.Size( 78, 20 );
            this.tbx_Status.TabIndex = 5;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 320, 120 );
            this.Controls.Add( this.tbx_Status );
            this.Controls.Add( this.lbl_Status );
            this.Controls.Add( this.tbx_Secs );
            this.Controls.Add( this.lbl_Running );
            this.Controls.Add( this.btn_Stop );
            this.Controls.Add( this.btn_Start );
            this.Name = "MainForm";
            this.Text = "Multi Threaded Schedule Test";
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Start;
        private System.Windows.Forms.Button btn_Stop;
        private System.Windows.Forms.Label lbl_Running;
        private System.Windows.Forms.TextBox tbx_Secs;
        private System.Windows.Forms.Timer tmr_Seconds;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.TextBox tbx_Status;
    }
}

