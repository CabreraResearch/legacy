using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CswPrintClient1
{
    public partial class ConfirmDialog : Form
    {
        public ConfirmDialog()
        {
            InitializeComponent();
        }

        public delegate void ConfirmEvent();

        public ConfirmEvent onOk = null;
        public ConfirmEvent onCancel = null;

        public string Text
        {
            set { lblText.Text = value; }
        }

        private void btnOk_Click( object sender, EventArgs e )
        {
            this.Hide();
            if( null != onOk )
            {
                onOk();
            }
        }

        private void btnCancel_Click( object sender, EventArgs e )
        {
            this.Hide();
            if( null != onCancel )
            {
                onCancel();
            }
        }
    }
}
