using System;
using System.IO;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Web.Services;
using System.Web.Script.Services;
using System.Collections.Generic; // supports ScriptService attribute
using ChemSW.Core;
using ChemSW.Config;
using ChemSW.Nbt.Security;
using ChemSW.Security;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Statistics;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Formatting = Newtonsoft.Json.Formatting;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Data.OracleClient;

namespace ChemSW.Nbt.WebServices
{
	/// <summary>
	/// NBT Web service interface
	/// </summary>
	/// 
	[ScriptService]
	[WebService( Namespace = "http://localhost/NbtWebApp" )]
	[WebServiceBinding( ConformsTo = WsiProfiles.BasicProfile1_1 )]
	public class wsTEST : System.Web.Services.WebService
	{
		private const string _ConnectionString = "user id=nbt_master;data source=dracula;password=nbt;Pooling=yes";

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement testStatic()
		{
			return new XElement( "Test" );
		}

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement testRead()
		{
			OracleConnection OraConnection = new OracleConnection( _ConnectionString );
			string Sql = "select * from sessionlist where rownum = 1";
			OracleDataAdapter OraAdapter = new OracleDataAdapter( Sql, OraConnection );
			DataSet DS = new DataSet();
			OraAdapter.Fill( DS );
			DataTable SessionsTable = DS.Tables[0];
			OraConnection.Close();

            //LinkedList<Guid> OldGuidList = (LinkedList<Guid>) Session["GuidList"];
		    SortedSet<Guid> NewGuidList = new SortedSet<Guid>();
            for( Int32 i = 0; i < 10000; i++ )
            {
                Guid newGuid = Guid.NewGuid();
                NewGuidList.Add( newGuid );
            }

		    Guid MaxGuid = NewGuidList.Max();
            Guid MinGuid = NewGuidList.Min();

            StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter( sb );
			SessionsTable.WriteXml( sw );
			return XElement.Parse( sb.ToString() );
		}

        [WebMethod( EnableSession = true )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement testReadSession()
        {
            OracleConnection OraConnection = new OracleConnection( _ConnectionString );
            string Sql = "select * from sessionlist where rownum = 1";
            OracleDataAdapter OraAdapter = new OracleDataAdapter( Sql, OraConnection );
            DataSet DS = new DataSet();
            OraAdapter.Fill( DS );
            DataTable SessionsTable = DS.Tables[0];
            OraConnection.Close();

            SortedSet<Guid> OldGuidList = (SortedSet<Guid>) Session["GuidList"];
            SortedSet<Guid> NewGuidList = new SortedSet<Guid>();
            for( Int32 i = 0; i < 10000; i++ )
            {
                if( null != Session["GuidList"] )
                { Int32 GuidCount = ( (SortedSet<Guid>) Session["GuidList"] ).Count; }
                Guid newGuid = Guid.NewGuid();
                NewGuidList.Add( newGuid );
                Session["GuidList"] = NewGuidList;
            }

            Guid MaxGuid = ( (SortedSet<Guid>) Session["GuidList"] ).Max();
            Guid MinGuid = ( (SortedSet<Guid>) Session["GuidList"] ).Min();

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter( sb );
            SessionsTable.WriteXml( sw );
            return XElement.Parse( sb.ToString() );
        }

		[WebMethod( EnableSession = false )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement testWrite()
		{
			OracleConnection OraConnection = new OracleConnection( _ConnectionString );
			OraConnection.Open();
			string Sql = "insert into sessionlist (SESSIONLISTID,SESSIONID,ACCESSID,USERID,USERNAME,IPADDRESS,CSWPRIMEKEY,ROLETIMEOUTMINUTES,TIMEOUTDATE,LOGINDATE,ISMOBILE,ISDEMO)";
			Sql += "values (seq_sessionlistid.nextval, '" + Guid.NewGuid().ToString() + "', '1', '1', 'admin', '1.2.3.4', '', '30', '', '', '0', '0')";
			OracleCommand OraCommand = new OracleCommand();
			OraCommand.CommandText = Sql;
			OraCommand.Connection = OraConnection;
			Int32 rows = OraCommand.ExecuteNonQuery();
			OraConnection.Close();
			return new XElement( "result", new XAttribute( "rows", rows.ToString() ) );
		}


	}//wsTEST

} // namespace ChemSW.WebServices
