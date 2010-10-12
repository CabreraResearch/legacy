using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ChemSW.Nbt.Sched;

namespace SchedulerGuiTest
{
    public partial class MainForm : Form
    {
        CswNbtSchdItemRunner _CswNbtSchdItemRunner = new CswNbtSchdItemRunner();
        public MainForm()
        {
            InitializeComponent();
        }

        private void btn_Test_Click( object sender, EventArgs e )
        {
            _CswNbtSchdItemRunner.start();

        }

        private void btn_Stop_Click( object sender, EventArgs e )
        {
            _CswNbtSchdItemRunner.stop();

        }
    }
}