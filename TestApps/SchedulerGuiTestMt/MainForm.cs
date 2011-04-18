using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ChemSW.MtSched.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.Sched;

namespace SchedulerGuiTestMt
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            tmr_Seconds.Interval = 1000; 
        }

        private CswScheduleService _CswScheduleService = null;
        private void btn_Start_Click( object sender, EventArgs e )
        {
            tbx_Secs.Text = string.Empty;
            tmr_Seconds.Start();
            tbx_Status.Text = "Starting";
            _CswScheduleService = new CswScheduleService( new CswScheduleLogicFactoryNbt(), new CswScheduleResourceFactoryNbt(), new CswScheduleLogicDetailPersistenceFactoryNbt() );
            _CswScheduleService.start();
            tbx_Status.Text = "Running";

        }

        private void btn_Stop_Click( object sender, EventArgs e )
        {
            tbx_Status.Text = "Stopping";
            _CswScheduleService.stop();
            tmr_Seconds.Stop();
            tbx_Status.Text = "Idle";

        }

        private void TimerTick( object sender, EventArgs e )
        {
            Int32 TotalSecs = Int32.MinValue;
            if( string.Empty != tbx_Secs.Text )
            {
                TotalSecs = Convert.ToInt32(tbx_Secs.Text);
            }
            else
            {
                TotalSecs = 0; 
            }

            TotalSecs++;

            tbx_Secs.Text = TotalSecs.ToString();
        }//TimerTick() 
    }
}
