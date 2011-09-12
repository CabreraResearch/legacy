using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChemSW.Log;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Config;
using ChemSW.Nbt.Config;

using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

namespace ChemSW.LogTester
{
	class Complainer
	{
		private CswNbtResources _CswNbtResources;
		private ICswLogger _CswLogger;
		private string _ThreadId;
		private Int32 _MessageCount;

		public Complainer( string ThreadId, Int32 MessageCount )
		{
			_ThreadId = ThreadId;
			_MessageCount = MessageCount;

			_CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.NbtExe, true, false );
			_CswLogger = _CswNbtResources.CswLogger;
		}

		public delegate void StatusUpdateHandler( string Message );
		public StatusUpdateHandler OnStatusUpdate = null;

		public void Complain()
		{
			for( Int32 i = 1; i <= _MessageCount; i++ )
			{
				if( OnStatusUpdate != null && i % 100 == 0 )
				{
					OnStatusUpdate( "Thread " + _ThreadId + " - Logging message #" + i.ToString() );
				}
				
				_CswLogger.reportAppState( "Thread " + _ThreadId + " - Logging message #" + i.ToString() );
				
				
				//CswStatusMessage Message = _makeStatusMessage();
				//Message.ContentType = ContentType.AppState;
				//Message.Attributes.Add( "app_state", "Thread " + _ThreadId + " - Logging message #" + i.ToString() );

				//_send( Message );
			}
		} // Complain()

		//// from CswAppStatusReporter
		//private CswStatusMessage _makeStatusMessage()
		//{
		//    CswStatusMessage ReturnVal = new CswStatusMessage();
		//    ReturnVal.AppType = AppType.Nbt;
		//    ReturnVal.Attributes.Add( "time_reported", DateTime.Now.ToString( "yyyy/MM/dd HH:mm:ss.fff" ) );
		//    ReturnVal.Attributes.Add( "access_id", "accessid-undefined" );
		//    ReturnVal.Attributes.Add( "session_id", "sessionid-undefined" );
		//    ReturnVal.Attributes.Add( "user_name", "username-undefined" );

		//    return ( ReturnVal );

		//}//_makeStatusMessage()


		//// from CswLogVenueUdpBackgroundThread.cs
		//private BinaryFormatter _BinaryFormatter = new BinaryFormatter();
		//private Socket _UDPSocket = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
		//private const int ANYPORT = 0;
		//public IPAddress UdpIpAddress = IPAddress.Parse( "127.0.0.1" );
		//public Int32 UdpPort = 5000;
		//public Int32 UdbBufferSize = 16256;
		//IPEndPoint _RemoteIpEndPoint = null;

		//private void _send( CswStatusMessage Message )
		//{
		//    if( null == _RemoteIpEndPoint )
		//    {
		//        _RemoteIpEndPoint = new IPEndPoint( UdpIpAddress, UdpPort );
		//    }//


		//    if( null == _UDPSocket )
		//    {
		//        _UDPSocket = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
		//    }

		//    Byte[] SendBuffer = new Byte[UdbBufferSize];
		//    MemoryStream MemoryStream = new MemoryStream( SendBuffer );
		//    _BinaryFormatter.Serialize( MemoryStream, Message );
		//    int nBytesSent = _UDPSocket.SendTo( SendBuffer, _RemoteIpEndPoint );

		//    //_LastTimeAMessageWasSent = DateTime.Now;

		//}//_send() 

	}
}
