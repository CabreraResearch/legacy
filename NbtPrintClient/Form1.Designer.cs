namespace CswPrintClient1
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
            this.tbDescript = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.btnClearReg = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.cbEnabled = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblRegisterStatus = new System.Windows.Forms.Label();
            this.tbURL = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnRegister = new System.Windows.Forms.Button();
            this.tbLPCname = new System.Windows.Forms.TextBox();
            this.btnSelPrn = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbAccessId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPrinter = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.btnTestPrintSvc = new System.Windows.Forms.Button();
            this.tbPrintLabelId = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbTargetId = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnPrintEPL = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
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
            this.tabControl1.Size = new System.Drawing.Size(432, 403);
            this.tabControl1.TabIndex = 18;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tbLog);
            this.tabPage1.Controls.Add(this.lblStatus);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(424, 377);
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
            this.tbLog.Size = new System.Drawing.Size(418, 315);
            this.tbLog.TabIndex = 20;
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(3, 3);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(418, 56);
            this.lblStatus.TabIndex = 19;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tbDescript);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.btnClearReg);
            this.tabPage2.Controls.Add(this.btnSave);
            this.tabPage2.Controls.Add(this.cbEnabled);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.lblRegisterStatus);
            this.tabPage2.Controls.Add(this.tbURL);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.btnRegister);
            this.tabPage2.Controls.Add(this.tbLPCname);
            this.tabPage2.Controls.Add(this.btnSelPrn);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.tbPassword);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.tbUsername);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.tbAccessId);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.tbPrinter);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(424, 377);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Setup";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tbDescript
            // 
            this.tbDescript.Location = new System.Drawing.Point(130, 102);
            this.tbDescript.Multiline = true;
            this.tbDescript.Name = "tbDescript";
            this.tbDescript.Size = new System.Drawing.Size(219, 54);
            this.tbDescript.TabIndex = 55;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(34, 105);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(93, 13);
            this.label13.TabIndex = 54;
            this.label13.Text = "Printer Description";
            // 
            // btnClearReg
            // 
            this.btnClearReg.Location = new System.Drawing.Point(354, 75);
            this.btnClearReg.Name = "btnClearReg";
            this.btnClearReg.Size = new System.Drawing.Size(60, 23);
            this.btnClearReg.TabIndex = 53;
            this.btnClearReg.Text = "Clear";
            this.btnClearReg.UseVisualStyleBackColor = true;
            this.btnClearReg.Click += new System.EventHandler(this.btnClearReg_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(287, 344);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(115, 23);
            this.btnSave.TabIndex = 52;
            this.btnSave.Text = "Save Settings";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cbEnabled
            // 
            this.cbEnabled.AutoSize = true;
            this.cbEnabled.Location = new System.Drawing.Point(101, 325);
            this.cbEnabled.Name = "cbEnabled";
            this.cbEnabled.Size = new System.Drawing.Size(108, 17);
            this.cbEnabled.TabIndex = 51;
            this.cbEnabled.Text = "Enable Print Jobs";
            this.cbEnabled.UseVisualStyleBackColor = true;
            this.cbEnabled.Click += new System.EventHandler(this.cbEnabled_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(18, 186);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(81, 13);
            this.label12.TabIndex = 50;
            this.label12.Text = "Server Setup";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(14, 24);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(81, 13);
            this.label11.TabIndex = 49;
            this.label11.Text = "Printer Setup";
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
            this.tbURL.Location = new System.Drawing.Point(98, 213);
            this.tbURL.Name = "tbURL";
            this.tbURL.Size = new System.Drawing.Size(304, 20);
            this.tbURL.TabIndex = 47;
            this.tbURL.Text = "https://nbtlive.chemswlive.com/nbtwebapp/Services/";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(29, 216);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(63, 13);
            this.label9.TabIndex = 46;
            this.label9.Text = "Server URL";
            // 
            // btnRegister
            // 
            this.btnRegister.Location = new System.Drawing.Point(289, 75);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(60, 23);
            this.btnRegister.TabIndex = 45;
            this.btnRegister.Text = "Register";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // tbLPCname
            // 
            this.tbLPCname.Location = new System.Drawing.Point(130, 75);
            this.tbLPCname.Name = "tbLPCname";
            this.tbLPCname.Size = new System.Drawing.Size(152, 20);
            this.tbLPCname.TabIndex = 44;
            this.tbLPCname.Text = "My Label Printer";
            // 
            // btnSelPrn
            // 
            this.btnSelPrn.Location = new System.Drawing.Point(289, 47);
            this.btnSelPrn.Name = "btnSelPrn";
            this.btnSelPrn.Size = new System.Drawing.Size(60, 23);
            this.btnSelPrn.TabIndex = 43;
            this.btnSelPrn.Text = "Choose...";
            this.btnSelPrn.UseVisualStyleBackColor = true;
            this.btnSelPrn.Click += new System.EventHandler(this.btnSelPrn_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(11, 50);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(115, 13);
            this.label10.TabIndex = 42;
            this.label10.Text = "*Selected Label Printer";
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(101, 295);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(124, 20);
            this.tbPassword.TabIndex = 35;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 298);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "Password";
            // 
            // tbUsername
            // 
            this.tbUsername.Location = new System.Drawing.Point(101, 269);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.Size = new System.Drawing.Size(124, 20);
            this.tbUsername.TabIndex = 33;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 272);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "Username";
            // 
            // tbAccessId
            // 
            this.tbAccessId.Location = new System.Drawing.Point(101, 243);
            this.tbAccessId.Name = "tbAccessId";
            this.tbAccessId.Size = new System.Drawing.Size(124, 20);
            this.tbAccessId.TabIndex = 31;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 246);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "AccessId";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "*ChemSW Printer Name";
            // 
            // tbPrinter
            // 
            this.tbPrinter.Location = new System.Drawing.Point(130, 47);
            this.tbPrinter.Name = "tbPrinter";
            this.tbPrinter.ReadOnly = true;
            this.tbPrinter.Size = new System.Drawing.Size(152, 20);
            this.tbPrinter.TabIndex = 28;
            // 
            // tabPage3
            // 
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
            this.tabPage3.Size = new System.Drawing.Size(424, 377);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Testing";
            this.tabPage3.UseVisualStyleBackColor = true;
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
            this.btnTestPrintSvc.Size = new System.Drawing.Size(75, 23);
            this.btnTestPrintSvc.TabIndex = 44;
            this.btnTestPrintSvc.Text = "Print Service";
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
            this.btnPrintEPL.Size = new System.Drawing.Size(75, 23);
            this.btnPrintEPL.TabIndex = 6;
            this.btnPrintEPL.Text = "Print EPL";
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
            // printDialog1
            // 
            this.printDialog1.AllowPrintToFile = false;
            this.printDialog1.UseEXDialog = true;
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
            this.ClientSize = new System.Drawing.Size(432, 403);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "ChemSW Label Printer Client v1.2";
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
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbUsername;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbAccessId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbPrinter;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnPrintEPL;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.TextBox tbURL;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.TextBox tbLPCname;
        private System.Windows.Forms.Button btnSelPrn;
        private System.Windows.Forms.TextBox tbPrintLabelId;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbTargetId;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnTestPrintSvc;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblRegisterStatus;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox cbEnabled;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClearReg;
        private System.Windows.Forms.TextBox tbDescript;
        private System.Windows.Forms.Label label13;

    }
}

