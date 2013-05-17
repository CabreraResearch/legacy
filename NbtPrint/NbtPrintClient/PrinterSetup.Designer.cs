namespace NbtPrintClient
{
    partial class PrinterSetup
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
            this.tbDescript = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.btnClearReg = new System.Windows.Forms.Button();
            this.tbLPCname = new System.Windows.Forms.TextBox();
            this.btnSelPrn = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPrinter = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.cbEnabled = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // tbDescript
            // 
            this.tbDescript.Location = new System.Drawing.Point(134, 63);
            this.tbDescript.Multiline = true;
            this.tbDescript.Name = "tbDescript";
            this.tbDescript.Size = new System.Drawing.Size(219, 54);
            this.tbDescript.TabIndex = 65;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(38, 66);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(93, 13);
            this.label13.TabIndex = 64;
            this.label13.Text = "Printer Description";
            // 
            // btnClearReg
            // 
            this.btnClearReg.Location = new System.Drawing.Point(18, 148);
            this.btnClearReg.Name = "btnClearReg";
            this.btnClearReg.Size = new System.Drawing.Size(113, 23);
            this.btnClearReg.TabIndex = 63;
            this.btnClearReg.Text = "Delete This Printer";
            this.btnClearReg.UseVisualStyleBackColor = true;
            this.btnClearReg.Click += new System.EventHandler(this.btnClearReg_Click);
            // 
            // tbLPCname
            // 
            this.tbLPCname.Location = new System.Drawing.Point(134, 36);
            this.tbLPCname.Name = "tbLPCname";
            this.tbLPCname.Size = new System.Drawing.Size(152, 20);
            this.tbLPCname.TabIndex = 60;
            this.tbLPCname.Text = "My Label Printer";
            // 
            // btnSelPrn
            // 
            this.btnSelPrn.Location = new System.Drawing.Point(293, 8);
            this.btnSelPrn.Name = "btnSelPrn";
            this.btnSelPrn.Size = new System.Drawing.Size(60, 23);
            this.btnSelPrn.TabIndex = 59;
            this.btnSelPrn.Text = "Choose...";
            this.btnSelPrn.UseVisualStyleBackColor = true;
            this.btnSelPrn.Click += new System.EventHandler(this.btnSelPrn_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 11);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(115, 13);
            this.label10.TabIndex = 58;
            this.label10.Text = "*Selected Label Printer";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 57;
            this.label1.Text = "*ChemSW Printer Name";
            // 
            // tbPrinter
            // 
            this.tbPrinter.Location = new System.Drawing.Point(134, 8);
            this.tbPrinter.Name = "tbPrinter";
            this.tbPrinter.ReadOnly = true;
            this.tbPrinter.Size = new System.Drawing.Size(152, 20);
            this.tbPrinter.TabIndex = 56;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(214, 150);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 23);
            this.button1.TabIndex = 66;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(320, 150);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(94, 23);
            this.button2.TabIndex = 67;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // printDialog1
            // 
            this.printDialog1.AllowPrintToFile = false;
            this.printDialog1.UseEXDialog = true;
            // 
            // cbEnabled
            // 
            this.cbEnabled.AutoSize = true;
            this.cbEnabled.Location = new System.Drawing.Point(134, 124);
            this.cbEnabled.Name = "cbEnabled";
            this.cbEnabled.Size = new System.Drawing.Size(177, 17);
            this.cbEnabled.TabIndex = 68;
            this.cbEnabled.Text = "Enable Print Jobs for this printer.";
            this.cbEnabled.UseVisualStyleBackColor = true;
            // 
            // PrinterSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 181);
            this.Controls.Add(this.cbEnabled);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbDescript);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.btnClearReg);
            this.Controls.Add(this.tbLPCname);
            this.Controls.Add(this.btnSelPrn);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbPrinter);
            this.MinimizeBox = false;
            this.Name = "PrinterSetup";
            this.Text = "Printer Setup";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbDescript;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btnClearReg;
        private System.Windows.Forms.TextBox tbLPCname;
        private System.Windows.Forms.Button btnSelPrn;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbPrinter;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.CheckBox cbEnabled;
    }
}