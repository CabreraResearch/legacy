﻿using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt.ChemCatCentral;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
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
        [WebInvoke( Method = "POST", UriTemplate = "SaveFile?propid={propid}&blobdataid={blobdataid}&caption={caption}" )]
        [Description( "Save a file" )]
        [FaultContract( typeof( FaultException ) )]
        public BlobDataReturn SaveFile( string propid, int blobdataid, string caption )
        {
            BlobDataReturn ret = new BlobDataReturn();

            if( _Context.Request.Files.Count > 0 )
            {
                BlobDataParams blobDataParams = new BlobDataParams();
                blobDataParams.postedFile = _Context.Request.Files[0];
                blobDataParams.propid = propid;

                blobDataParams.Blob.BlobDataId = blobdataid;
                blobDataParams.Blob.Caption = caption;

                var SvcDriver = new CswWebSvcDriver<BlobDataReturn, BlobDataParams>(
                    CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj: ret,
                    WebSvcMethodPtr: CswNbtWebServiceBinaryData.saveFile,
                    ParamObj: blobDataParams
                    );

                SvcDriver.run();
            }

            return ret;
        }

        [OperationContract]
        [WebInvoke( Method = "GET", UriTemplate = "getBlob?jctnodepropid={jctnodepropid}&nodeid={nodeid}&blobdataid={blobdataid}&usenodetypeasplaceholder={usenodetypeasplaceholder}&date={date}&uid={uid}" )]
        [Description( "Fetch a file" )]
        [FaultContract( typeof( FaultException ) )]
        public Stream getBlob( string jctnodepropid, string nodeid, int blobdataid, string usenodetypeasplaceholder, string uid, string date )
        {
            BlobDataReturn ret = new BlobDataReturn();

            BlobDataParams blobDataParams = new BlobDataParams();
            blobDataParams.appPath = _Context.Request.PhysicalApplicationPath;
            blobDataParams.propid = jctnodepropid;
            blobDataParams.nodeid = nodeid;
            blobDataParams.Blob.BlobDataId = blobdataid;
            blobDataParams.usenodetypeasplaceholder = usenodetypeasplaceholder.ToString();
            blobDataParams.date = date;

            var SvcDriver = new CswWebSvcDriver<BlobDataReturn, BlobDataParams>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: ret,
                WebSvcMethodPtr: CswNbtWebServiceBinaryData.getBlob,
                ParamObj: blobDataParams
                );

            SvcDriver.run();


            MemoryStream mem = new MemoryStream();
            BinaryWriter BWriter = new BinaryWriter( mem );
            BWriter.Write( ret.Data.data );
            mem.Position = 0;

            WebOperationContext.Current.OutgoingResponse.Headers.Add( "Content-Disposition", "attachment;filename=\"" + ret.Data.Blob.FileName + "\";" );
            WebOperationContext.Current.OutgoingResponse.Headers.Add( HttpResponseHeader.ContentType, ret.Data.Blob.ContentType );

            return mem;
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "clearBlob" )]
        [Description( "Clear all blobs associated with a property" )]
        [FaultContract( typeof( FaultException ) )]
        public BlobDataReturn clearBlob( BlobDataParams Request )
        {
            BlobDataReturn ret = new BlobDataReturn();

            var SvcDriver = new CswWebSvcDriver<BlobDataReturn, BlobDataParams>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: ret,
                WebSvcMethodPtr: CswNbtWebServiceBinaryData.clearBlob,
                ParamObj: Request
                );

            SvcDriver.run();

            return ret;
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "clearImage" )]
        [Description( "Clear a single image" )]
        [FaultContract( typeof( FaultException ) )]
        public NodePropImageReturn clearImage( BlobDataParams Request )
        {
            NodePropImageReturn ret = new NodePropImageReturn();

            var SvcDriver = new CswWebSvcDriver<NodePropImageReturn, BlobDataParams>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: ret,
                WebSvcMethodPtr: CswNbtWebServiceBinaryData.clearImage,
                ParamObj: Request
                );

            SvcDriver.run();

            return ret;
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "saveCaption" )]
        [Description( "Save a caption for an image" )]
        [FaultContract( typeof( FaultException ) )]
        public NodePropImageReturn saveCaption( BlobDataParams Request )
        {
            NodePropImageReturn ret = new NodePropImageReturn();

            var SvcDriver = new CswWebSvcDriver<NodePropImageReturn, BlobDataParams>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: ret,
                WebSvcMethodPtr: CswNbtWebServiceBinaryData.saveCaption,
                ParamObj: Request
                );

            SvcDriver.run();

            return ret;
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "getText" )]
        [Description( "Get the contents of a file" )]
        [FaultContract( typeof( FaultException ) )]
        public BlobDataReturn getText()
        {
            BlobDataReturn ret = new BlobDataReturn();

            if( _Context.Request.Files.Count > 0 )
            {
                BlobDataParams req = new BlobDataParams()
                    {
                        postedFile = _Context.Request.Files[0]
                    };

                var SvcDriver = new CswWebSvcDriver<BlobDataReturn, BlobDataParams>(
                    CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj: ret,
                    WebSvcMethodPtr: CswNbtWebServiceBinaryData.getText,
                    ParamObj: req
                    );

                SvcDriver.run();
            }

            return ret;
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "getImageProp" )]
        [Description( "Get all the images for an image node property" )]
        [FaultContract( typeof( FaultException ) )]
        public NodePropImageReturn getImageProp( BlobDataParams req )
        {
            NodePropImageReturn ret = new NodePropImageReturn();
            req.date = req.date ?? string.Empty;

            var SvcDriver = new CswWebSvcDriver<NodePropImageReturn, BlobDataParams>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: ret,
                WebSvcMethodPtr: CswNbtWebServiceBinaryData.getImageProp,
                ParamObj: req
                );

            SvcDriver.run();

            return ret;
        }

        [OperationContract]
        [WebInvoke( Method = "GET", UriTemplate = "getExternalImage?cdbregno={cdbregno}&productid={productid}&uid={uid}" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public Stream getExternalImage( string cdbregno, string productid, string uid )
        {
            ExternalImageRet ret = new ExternalImageRet();

            ACDSearchParams Params = new ACDSearchParams();
            Params.Cdbregno = CswConvert.ToInt32( cdbregno );
            Params.ProductId = CswConvert.ToInt32( productid );

            var SvcDriver = new CswWebSvcDriver<ExternalImageRet, ACDSearchParams>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: ret,
                WebSvcMethodPtr: CswNbtWebServiceC3Search.getExternalImage,
                ParamObj: Params
                );

            SvcDriver.run();


            MemoryStream mem = new MemoryStream();
            BinaryWriter BWriter = new BinaryWriter( mem );
            BWriter.Write( ret.Data );
            mem.Position = 0;

            string Filename = "molimage_" + productid;
            WebOperationContext.Current.OutgoingResponse.Headers.Add( "Content-Disposition", "inline;" );
            WebOperationContext.Current.OutgoingResponse.Headers.Add( HttpResponseHeader.ContentType, "image/png" );

            return mem;
        }
    }

    [DataContract]
    public class ExternalImageRet : CswWebSvcReturn
    {
        public ExternalImageRet()
        {
            Data = new byte[0];
        }
        [DataMember]
        public byte[] Data;
    }

    [DataContract]
    public class BlobDataParams
    {
        public HttpPostedFile postedFile;
        private string _nodeid = string.Empty;
        public string nodeid
        {
            get { return _nodeid; }
            set
            {
                _NodeId = CswConvert.ToPrimaryKey( value );
                _nodeid = value;
            }
        }

        private CswPrimaryKey _NodeId = null;
        public CswPrimaryKey NodeId
        {
            get { return _NodeId; }
        }
        public byte[] data = new byte[0];
        public string appPath = string.Empty;
        public string usenodetypeasplaceholder = string.Empty;

        [DataMember]
        public bool success = false;

        [DataMember]
        public string propid = string.Empty;

        [DataMember]
        public string filetext = string.Empty;

        [DataMember]
        public string caption = string.Empty;

        [DataMember]
        public string date = string.Empty;

        [DataMember]
        public CswNbtSdBlobData.CswNbtBlob Blob = new CswNbtSdBlobData.CswNbtBlob();
    }

    [DataContract]
    public class BlobDataReturn : CswWebSvcReturn
    {
        public BlobDataReturn()
        {
            Data = new BlobDataParams();
        }
        [DataMember]
        public BlobDataParams Data;
    }

    [DataContract]
    public class NodePropImageReturn : CswWebSvcReturn
    {
        public NodePropImageReturn()
        {
            Data = new CswNbtNodePropImage();
        }

        [DataMember]
        public CswNbtNodePropImage Data;
    }
}
