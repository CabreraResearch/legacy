using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Returns;


namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for handling files (blob data)
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class BlobData
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "SaveFile" )]
        [Description( "Save a file" )]
        [FaultContract( typeof( FaultException ) )]
        public BlobDataReturn SaveFile()
        {
            BlobDataReturn ret = new BlobDataReturn();

            if( _Context.Request.Files.Count > 0 )
            {
                BlobDataParams blobDataParams = new BlobDataParams();
                blobDataParams.postedFile = _Context.Request.Files[0];
                blobDataParams.propid = _Context.Request.QueryString["propid"];

                var SvcDriver = new CswWebSvcDriver<BlobDataReturn, BlobDataParams>(
                    CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj : ret,
                    WebSvcMethodPtr : CswNbtWebServiceBinaryData.saveFile,
                    ParamObj : blobDataParams
                    );

                SvcDriver.run();
            }

            return ret;
        }

        [OperationContract]
        [WebInvoke( Method = "GET", UriTemplate = "getBlob" )]
        [Description( "Fetch a file" )]
        [FaultContract( typeof( FaultException ) )]
        public Stream getBlob()
        {
            BlobDataReturn ret = new BlobDataReturn();

            BlobDataParams blobDataParams = new BlobDataParams();
            blobDataParams.propid = _Context.Request.QueryString["jctnodepropid"];
            blobDataParams.appPath = _Context.Request.PhysicalApplicationPath;

            var SvcDriver = new CswWebSvcDriver<BlobDataReturn, BlobDataParams>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : ret,
                WebSvcMethodPtr : CswNbtWebServiceBinaryData.getBlob,
                ParamObj : blobDataParams
                );

            SvcDriver.run();

            MemoryStream mem = new MemoryStream();
            BinaryWriter BWriter = new BinaryWriter( mem, System.Text.Encoding.Default );
            BWriter.Write( ret.Data.data );
            mem.Position = 0;

            _Context.Response.ContentType = ret.Data.contenttype;
            _Context.Response.AddHeader( "Content-Disposition", "attachment;filename=" + ret.Data.filename + ";" );

            return mem;
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "clearBlob" )]
        [Description( "Fetch a file" )]
        [FaultContract( typeof( FaultException ) )]
        public BlobDataReturn clearBlob( BlobDataParams Request )
        {
            BlobDataReturn ret = new BlobDataReturn();

            var SvcDriver = new CswWebSvcDriver<BlobDataReturn, BlobDataParams>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : ret,
                WebSvcMethodPtr : CswNbtWebServiceBinaryData.clearBlob,
                ParamObj : Request
                );

            SvcDriver.run();

            return ret;
        }
    }

    [DataContract]
    public class BlobDataParams
    {
        public HttpPostedFile postedFile;
        public string nodeid = string.Empty;
        public byte[] data = new byte[0];
        public string appPath = string.Empty;

        [DataMember]
        public bool success = false;

        [DataMember]
        public string propid = string.Empty;

        [DataMember]
        public string contenttype = string.Empty;

        [DataMember]
        public string href = string.Empty;

        [DataMember]
        public string filename = string.Empty;
    }

    [DataContract]
    public class BlobDataReturn: CswWebSvcReturn
    {
        public BlobDataReturn()
        {
            Data = new BlobDataParams();
        }
        [DataMember]
        public BlobDataParams Data;
    }
}
