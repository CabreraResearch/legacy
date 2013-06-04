namespace NbtPrintClient
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lbPrinterList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.cbServiceMode = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.lblRegisterStatus = new System.Windows.Forms.Label();
            this.tbURL = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbAccessId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.lblTestStatus = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnTestPrintSvc = new System.Windows.Forms.Button();
            this.tbPrintLabelId = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbTargetId = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnPrintEPL = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(422, 426);
            this.tabControl1.TabIndex = 18;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tbLog);
            this.tabPage1.Controls.Add(this.lblStatus);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(414, 400);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Status";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tbLog
            // 
            this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLog.Location = new System.Drawing.Point(3, 59);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(408, 338);
            this.tbLog.TabIndex = 20;
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(3, 3);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(408, 56);
            this.lblStatus.TabIndex = 19;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lbPrinterList);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.btnSave);
            this.tabPage2.Controls.Add(this.cbServiceMode);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.lblRegisterStatus);
            this.tabPage2.Controls.Add(this.tbURL);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.tbPassword);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.tbUsername);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.tbAccessId);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(414, 400);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Setup";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lbPrinterList
            // 
            this.lbPrinterList.FormattingEnabled = true;
            this.lbPrinterList.Location = new System.Drawing.Point(21, 206);
            this.lbPrinterList.Name = "lbPrinterList";
            this.lbPrinterList.Size = new System.Drawing.Size(381, 121);
            this.lbPrinterList.TabIndex = 56;
            this.lbPrinterList.DoubleClick += new System.EventHandler(this.lbPrinterList_DoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(18, 189);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 55;
            this.label1.Text = "Printers";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(21, 333);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(99, 23);
            this.button1.TabIndex = 54;
            this.button1.Text = "Add Printer...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(278, 359);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(124, 29);
            this.btnSave.TabIndex = 52;
            this.btnSave.Text = "Save Settings";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cbServiceMode
            // 
            this.cbServiceMode.AutoSize = true;
            this.cbServiceMode.Location = new System.Drawing.Point(101, 159);
            this.cbServiceMode.Name = "cbServiceMode";
            this.cbServiceMode.Size = new System.Drawing.Size(208, 17);
            this.cbServiceMode.TabIndex = 51;
            this.cbServiceMode.Text = "Use Windows Service (requires admin)";
            this.cbServiceMode.UseVisualStyleBackColor = true;
            this.cbServiceMode.Click += new System.EventHandler(this.cbEnabled_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(18, 20);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(81, 13);
            this.label12.TabIndex = 50;
            this.label12.Text = "Server Setup";
            // 
            // lblRegisterStatus
            // 
            this.lblRegisterStatus.Location = new System.Drawing.Point(127, 3);
            this.lblRegisterStatus.Name = "lblRegisterStatus";
            this.lblRegisterStatus.Size = new System.Drawing.Size(268, 41);
            this.lblRegisterStatus.TabIndex = 48;
            // 
            // tbURL
            // 
            this.tbURL.Location = new System.Drawing.Point(98, 47);
            this.tbURL.Name = "tbURL";
            this.tbURL.Size = new System.Drawing.Size(304, 20);
            this.tbURL.TabIndex = 47;
            this.tbURL.Text = "https://nbtlive.chemswlive.com/nbtwebapp/Services/";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(29, 50);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(63, 13);
            this.label9.TabIndex = 46;
            this.label9.Text = "Server URL";
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(101, 129);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(124, 20);
            this.tbPassword.TabIndex = 35;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 132);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "Password";
            // 
            // tbUsername
            // 
            this.tbUsername.Location = new System.Drawing.Point(101, 103);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.Size = new System.Drawing.Size(124, 20);
            this.tbUsername.TabIndex = 33;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "Username";
            // 
            // tbAccessId
            // 
            this.tbAccessId.Location = new System.Drawing.Point(101, 77);
            this.tbAccessId.Name = "tbAccessId";
            this.tbAccessId.Size = new System.Drawing.Size(124, 20);
            this.tbAccessId.TabIndex = 31;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "AccessId";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.lblTestStatus);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.btnTestPrintSvc);
            this.tabPage3.Controls.Add(this.tbPrintLabelId);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.tbTargetId);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Controls.Add(this.btnPrintEPL);
            this.tabPage3.Controls.Add(this.textBox1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(414, 400);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Testing";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // lblTestStatus
            // 
            this.lblTestStatus.Location = new System.Drawing.Point(17, 338);
            this.lblTestStatus.Name = "lblTestStatus";
            this.lblTestStatus.Size = new System.Drawing.Size(389, 57);
            this.lblTestStatus.TabIndex = 46;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(20, 236);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(166, 13);
            this.label8.TabIndex = 45;
            this.label8.Text = "getLabel() Service Test Info";
            // 
            // btnTestPrintSvc
            // 
            this.btnTestPrintSvc.Location = new System.Drawing.Point(20, 308);
            this.btnTestPrintSvc.Name = "btnTestPrintSvc";
            this.btnTestPrintSvc.Size = new System.Drawing.Size(166, 23);
            this.btnTestPrintSvc.TabIndex = 44;
            this.btnTestPrintSvc.Text = "Print Service For Selected Printer";
            this.btnTestPrintSvc.UseVisualStyleBackColor = true;
            this.btnTestPrintSvc.Click += new System.EventHandler(this.btnTestPrintService_Click);
            // 
            // tbPrintLabelId
            // 
            this.tbPrintLabelId.Location = new System.Drawing.Point(95, 284);
            this.tbPrintLabelId.Name = "tbPrintLabelId";
            this.tbPrintLabelId.Size = new System.Drawing.Size(100, 20);
            this.tbPrintLabelId.TabIndex = 43;
            this.tbPrintLabelId.Text = "nodes_17021";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 287);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 42;
            this.label7.Text = "PrintLabelId";
            // 
            // tbTargetId
            // 
            this.tbTargetId.Location = new System.Drawing.Point(95, 258);
            this.tbTargetId.Name = "tbTargetId";
            this.tbTargetId.Size = new System.Drawing.Size(100, 20);
            this.tbTargetId.TabIndex = 41;
            this.tbTargetId.Text = "nodes_23182";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 261);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 40;
            this.label6.Text = "TargetId";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(17, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Printer Data Test Info";
            // 
            // btnPrintEPL
            // 
            this.btnPrintEPL.Location = new System.Drawing.Point(17, 196);
            this.btnPrintEPL.Name = "btnPrintEPL";
            this.btnPrintEPL.Size = new System.Drawing.Size(169, 23);
            this.btnPrintEPL.TabIndex = 6;
            this.btnPrintEPL.Text = "Print EPL to Selected Printer";
            this.btnPrintEPL.UseVisualStyleBackColor = true;
            this.btnPrintEPL.Click += new System.EventHandler(this.btnPrintEPL_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(17, 28);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(358, 162);
            this.textBox1.TabIndex = 5;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // timer1
            // 
            this.timer1.Interval = 10000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 426);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "ChemSW Label Printer Client v1.5";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbUsername;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbAccessId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnPrintEPL;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox tbURL;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbPrintLabelId;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbTargetId;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnTestPrintSvc;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblRegisterStatus;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox cbServiceMode;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox lbPrinterList;
        private System.Windows.Forms.Label lblTestStatus;

    }
}

