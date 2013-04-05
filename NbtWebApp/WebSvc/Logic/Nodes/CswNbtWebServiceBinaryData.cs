using System;
using System.Data;
using System.IO;
using System.Web;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
using NbtWebApp;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceBinaryData
    {
        private readonly CswNbtResources _CswNbtResources;
        public CswNbtWebServiceBinaryData( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public static void saveFile( ICswResources CswResources, BlobDataReturn Return, BlobDataParams Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            BinaryReader br = new BinaryReader( Request.postedFile.InputStream );
            byte[] FileData = new byte[Request.postedFile.InputStream.Length];
            for( int i = 0; i < Request.postedFile.InputStream.Length; i++ )
            {
                FileData[i] = br.ReadByte();
            }

            CswNbtSdTabsAndProps tabsAndProps = new CswNbtSdTabsAndProps( NbtResources );

            CswPropIdAttr PropId = new CswPropIdAttr( Request.propid );

            CswNbtMetaDataNodeTypeProp MetaDataProp = NbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
            CswNbtNode Node = NbtResources.Nodes[PropId.NodeId];
            CswNbtNodePropWrapper FileProp = Node.Properties[MetaDataProp];

            //Save the attribute data to jct_nodes_props
            CswTableUpdate JctUpdate = NbtResources.makeCswTableUpdate( "Blobber_save_update", "jct_nodes_props" );
            DataTable JctTable = JctUpdate.getTable( "jctnodepropid", FileProp.JctNodePropId );
            JctTable.Rows[0]["field1"] = Request.postedFile.FileName;
            JctTable.Rows[0]["field2"] = Request.postedFile.ContentType;
            JctUpdate.update( JctTable );

            //Save the file to blob_data
            CswTableUpdate BlobUpdate = NbtResources.makeCswTableUpdate( "saveBlob", "blob_data" );
            DataTable BlobTbl = BlobUpdate.getTable( "where jctnodepropid = " + FileProp.JctNodePropId );
            if( BlobTbl.Rows.Count > 0 )
            {
                BlobTbl.Rows[0]["blobdata"] = FileData;
            }
            else
            {
                DataRow NewRow = BlobTbl.NewRow();
                NewRow["jctnodepropid"] = FileProp.JctNodePropId;
                NewRow["blobdata"] = FileData;
                BlobTbl.Rows.Add( NewRow );
            }
            BlobUpdate.update( BlobTbl );

            Node.postChanges( false );

            Request.contenttype = Request.postedFile.ContentType;
            Request.filename = Request.postedFile.FileName;
            Request.href = CswNbtNodePropBlob.getLink( FileProp.JctNodePropId, PropId.NodeId, FileProp.NodeTypePropId );
            Request.success = true;
            Return.Data = Request;
        }

        public static void getBlob( ICswResources CswResources, BlobDataReturn Return, BlobDataParams Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            //Get the file from blob_data
            CswTableSelect blobDataSelect = NbtResources.makeCswTableSelect( "getBlob", "blob_data" );
            DataTable blobDataTbl = blobDataSelect.getTable( "where jctnodepropid = " + Request.propid );
            foreach( DataRow row in blobDataTbl.Rows )
            {
                Request.data = row["blobdata"] as byte[];
            }

            //Get the file info from jct_nodes_props
            CswTableSelect jctnodepropsSelect = NbtResources.makeCswTableSelect( "getBlobAttr", "jct_nodes_props" );
            DataTable jnpTbl = jctnodepropsSelect.getTable( "where jctnodepropid = " + Request.propid );
            foreach( DataRow row in jnpTbl.Rows )
            {
                Request.filename = row["field1"].ToString();
                Request.contenttype = row["field2"].ToString();
            }

            if( null == Request.data )
            {
                Request.data = File.ReadAllBytes( Request.appPath + "/Images/icons/300/_placeholder.gif" );
                Request.contenttype = "image/gif";
                Request.filename = "empty";
            }

            Return.Data = Request;
        }

        public static void clearBlob( ICswResources CswResources, BlobDataReturn Return, BlobDataParams Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            CswPropIdAttr PropId = new CswPropIdAttr( Request.propid );

            CswNbtSdTabsAndProps tabsandprops = new CswNbtSdTabsAndProps( NbtResources );
            tabsandprops.ClearPropValue( Request.propid, true );

            //CswNbtMetaDataNodeTypeProp MetaDataProp = NbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
            //CswNbtNode Node = NbtResources.Nodes[PropId.NodeId];
            //CswNbtNodePropWrapper FileProp = Node.Properties[MetaDataProp];
            //FileProp.ClearBlob();

            Request.contenttype = "";
            Request.filename = "";
            Request.href = "";
            Request.success = true;

            Return.Data = Request;
        }

        public static void getText( ICswResources CswResources, BlobDataReturn Return, BlobDataParams Request )
        {
            Stream stream = Request.postedFile.InputStream;
            using( StreamReader reader = new StreamReader( stream ) )
            {
                Request.filetext = reader.ReadToEnd();
            }
            Request.contenttype = Request.postedFile.ContentType;
            Request.filename = Request.postedFile.FileName;

            Return.Data = Request;
        }

        public void displayBlobData( HttpContext Context )
        {
            bool UseNodeTypeAsPlaceHolder = true;
            if( null != Context.Request["usenodetypeasplaceholder"] )
            {
                UseNodeTypeAsPlaceHolder = CswConvert.ToBoolean( Context.Request["usenodetypeasplaceholder"] );
            }
            if( null != Context.Request["jctnodepropid"] )
            {
                Int32 JctNodePropId = Convert.ToInt32( Context.Request["jctnodepropid"] );
                if( Int32.MinValue != JctNodePropId )
                {
                    CswTableSelect JctSelect = _CswNbtResources.makeCswTableSelect( "displayBlobData_select", "jct_nodes_props" );
                    JctSelect.AllowBlobColumns = true;
                    CswCommaDelimitedString SelectColumns = new CswCommaDelimitedString();
                    SelectColumns.Add( "blobdata" );
                    SelectColumns.Add( "field2" );
                    SelectColumns.Add( "field1" );
                    SelectColumns.Add( "nodeid" );
                    DataTable JctTable = JctSelect.getTable( SelectColumns, "jctnodepropid", JctNodePropId, "", true );

                    byte[] BlobData = null;
                    string FileName = "empty";
                    string ContentType = "image/gif";
                    if( JctTable.Rows.Count > 0 )
                    {
                        Int32 NodeId = CswConvert.ToInt32( JctTable.Rows[0]["nodeid"] );
                        if( false == JctTable.Rows[0].IsNull( "blobdata" ) )
                        {
                            FileName = JctTable.Rows[0]["field1"].ToString();
                            ContentType = JctTable.Rows[0]["field2"].ToString();
                            BlobData = JctTable.Rows[0]["blobdata"] as byte[];
                        }
                        else if( UseNodeTypeAsPlaceHolder )
                        {
                            CswNbtNode Node = _CswNbtResources.Nodes[new CswPrimaryKey( "nodes", NodeId )];
                            if( null != Node )
                            {
                                CswNbtMetaDataNodeType NodeType = Node.getNodeType();
                                if( null != NodeType && NodeType.IconFileName != string.Empty )
                                {
                                    FileName = NodeType.IconFileName;
                                    ContentType = "image/png";
                                    BlobData = File.ReadAllBytes( Context.Request.PhysicalApplicationPath + CswNbtMetaDataObjectClass.IconPrefix100 + NodeType.IconFileName );
                                }
                            }
                        }
                    } // if( JctTable.Rows.Count > 0 )

                    if( FileName == "empty" )
                    {
                        BlobData = File.ReadAllBytes( Context.Request.PhysicalApplicationPath + "/Images/icons/300/_placeholder.gif" );
                    }

                    MemoryStream mem = new MemoryStream();
                    BinaryWriter BWriter = new BinaryWriter( mem, System.Text.Encoding.Default );
                    BWriter.Write( BlobData );

                    Context.Response.ClearContent();
                    Context.Response.ContentType = ContentType;
                    Context.Response.BinaryWrite( mem.ToArray() );
                    Context.Response.AddHeader( "Content-Disposition", "attachment;filename=" + FileName + ";" );
                    Context.Response.End();
                }
            }
        }//if we got any result
        //return RetPath;
    } // class CswNbtWebServiceBinaryData

} // namespace ChemSW.Nbt.WebServices
