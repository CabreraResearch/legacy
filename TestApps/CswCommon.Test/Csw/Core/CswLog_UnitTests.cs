using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Log;
using NUnit.Framework;
using Newtonsoft.Json;

namespace CswCommon.Test.Csw.Core
{
    [TestFixture]
    public class CswLog_UnitTests
    {

        [Test]
        public void sendLogglyMessage()
        {

            //*************************************************************
            //Set up the test log entry values
            string TestVal_1 = DateTime.Now.ToString();
            const CswEnumLegalAttribute TestAttr_1 = CswEnumLegalAttribute.db_querytime;

            string TestVal_2 = System.Guid.NewGuid().ToString();
            const CswEnumLegalAttribute TestAttr_2 = CswEnumLegalAttribute.exoteric_message;


            //*************************************************************
            //Write the data to loggly with out log venue
            CswStatusMessage CswStatusMessage = new CswStatusMessage();
            CswStatusMessage.Attributes.Add( TestAttr_1, TestVal_1 );
            CswStatusMessage.Attributes.Add( TestAttr_2, TestVal_2 );
            
            CswLogVenueLoggly CswLogVenueLoggly = new CswLogVenueLoggly();
            CswLogVenueLoggly.send( CswStatusMessage );

            //*************************************************************
            //Verify that what we wrote to loggly was in fact written by GETing it from Logly

            //This works in linux: curl -u pglaser:n0ts3cure -H 'content-type:text/plain' 'https://chemswlive.loggly.com/api/search/?q=json.username:chemsw_admin' -D header.txt

            CswSetupVbls CswSetupVbls = new CswSetupVbls( CswEnumSetupMode.UnknownExe );
            string SearchValueUri = "https://chemswlive.loggly.com/api/search/?q=json." + TestAttr_1 + ":" + TestVal_1;


            HttpWebRequest HttpWebRequest = (HttpWebRequest) WebRequest.Create( SearchValueUri );
            HttpWebRequest.Method = "GET";
            HttpWebRequest.KeepAlive = false;
            HttpWebRequest.Timeout = 1000;
            HttpWebRequest.ReadWriteTimeout = 1000;
            HttpWebRequest.UserAgent = "pglaser:n0ts3cure";
            HttpWebRequest.ContentType = "text/plain";


            WebResponse WebResponse = HttpWebRequest.GetResponse();
            StringBuilder StringBuilder = new StringBuilder();
            System.IO.Stream Stream = WebResponse.GetResponseStream();
            int read_bytes = 0;
            do
            {
                byte[] buffer = new byte[2048];
                read_bytes = Stream.Read( buffer, 0, buffer.Length );
                StringBuilder.Append( Encoding.UTF8.GetString( buffer, 0, read_bytes ) );

            } while( read_bytes > 0 );

            Dictionary<CswEnumLegalAttribute, string> SearchResult = JsonConvert.DeserializeObject<Dictionary<CswEnumLegalAttribute, string>>( StringBuilder.ToString() );

            Assert.IsTrue( SearchResult.ContainsKey( TestAttr_1 ) );
            Assert.Equals( SearchResult[TestAttr_1], TestVal_1 );


        }//sendLogglyMessage()

    }//class CswLog_UnitTests

}//CswCommon.Test.Csw.Core
