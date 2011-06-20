using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ChemSW.Log;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Config;
using ChemSW.Nbt.Config;

namespace ChemSW.LogTester
{
	public partial class Form1 : Form
	{
		private Int32 _NumberOfThreads = 2;
		private Int32 _MessagesPerThread = 1000;
		
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
