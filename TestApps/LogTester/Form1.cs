using System;
using System.Windows.Forms;

namespace ChemSW.LogTester
{
    public partial class Form1 : Form
    {
        private Int32 _NumberOfThreads = 1;
        private Int32 _MessagesPerThread = 10;

        public Form1()
        {
            InitializeComponent();
        }

        public delegate void ReportHandler( string Message );
        public void Report( string Message )
        {
            textBox1.Text = Message + "\r\n" + textBox1.Text;
        }

        private void button1_Click( object sender, EventArgs e )
        {
            for( Int32 i = 1; i <= _NumberOfThreads; i++ )
            {
                Complainer c = new Complainer( i.ToString(), _MessagesPerThread );
                c.OnStatusUpdate = new Complainer.StatusUpdateHandler( StatusUpdate );

                MethodInvoker m = new MethodInvoker( c.Complain );
                m.BeginInvoke( null, null );
            }
        } // button1_Click()

        public void StatusUpdate( string Message )
        {
            textBox1.BeginInvoke( new ReportHandler( Report ), new object[] { Message } );
        }
    }
}
