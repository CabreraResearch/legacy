using System;
using System.Data;
using System.IO;
using System.Web;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
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

            string Href = string.Empty;
            CswNbtSdBlobData SdBlobData = new CswNbtSdBlobData( NbtResources );
            int BlobDataId = CswConvert.ToInt32( Request.Blob.BlobDataId );

            //IE9 sends the entire file url - we only want the file name
            string fileName = Path.GetFileName( Request.postedFile.FileName );

            BlobDataId = SdBlobData.saveFile( Request.propid, FileData, Request.postedFile.ContentType, fileName, out Href, BlobDataId );

            Request.Blob.BlobDataId = BlobDataId;
            Request.Blob.ContentType = Request.postedFile.ContentType;
            Request.Blob.FileName = fileName;
            Request.Blob.BlobUrl = Href;

            Request.success = true;
            Return.Data = Request;
        }

        public static void getBlob( ICswResources CswResources, BlobDataReturn Return, BlobDataParams Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            DataTable blobDataTbl;
            int BlobDataId = CswConvert.ToInt32( Request.Blob.BlobDataId );
            int PropId = _getNodeTypePropIdFromJctNodePropId( NbtResources, CswConvert.ToInt32( Request.propid ) );
            CswNbtMetaDataNodeTypeProp MetaDataProp = NbtResources.MetaData.getNodeTypeProp( PropId );

            if( NbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, MetaDataProp.getNodeType(), NbtResources.CurrentNbtUser ) )
            {

                //Get the file from blob_data
                if( String.IsNullOrEmpty( Request.date ) )
                {
                    CswTableSelect blobDataSelect = NbtResources.makeCswTableSelect( "getBlob", "blob_data" );
                    string whereClause = "where jctnodepropid = " + Request.propid;
                    if( Int32.MinValue != BlobDataId )
                    {
                        whereClause += " and blobdataid = " + Request.Blob.BlobDataId;
                    }
                    blobDataTbl = blobDataSelect.getTable( whereClause );
                }
                else //get the audited record
                {
                    int jctnodepropid = CswConvert.ToInt32( Request.propid );
                    CswArbitrarySelect blobDataAuditSelect = CswNbtSdBlobData.GetBlobAuditSelect( NbtResources, Request.date, jctnodepropid, BlobDataId );
                    blobDataTbl = blobDataAuditSelect.getTable();
                }

                foreach( DataRow row in blobDataTbl.Rows )
                {
                    Request.data = row["blobdata"] as byte[];
                    Request.Blob.FileName = row["filename"].ToString();
                    Request.Blob.ContentType = row["contenttype"].ToString();
                }

                if( null == Request.data || Request.data.Length == 0 )
                {
                    bool UseNTPlaceHolder = CswConvert.ToBoolean( Request.usenodetypeasplaceholder );
                    if( UseNTPlaceHolder )
                    {
                        CswPrimaryKey NodeId = CswConvert.ToPrimaryKey( Request.nodeid );
                        CswNbtNode Node = NbtResources.Nodes[NodeId];
                        if( null != Node )
                        {
                            CswNbtMetaDataNodeType NodeType = Node.getNodeType();
                            if( null != NodeType && NodeType.IconFileName != string.Empty )
                            {
                                Request.Blob.FileName = NodeType.IconFileName;
                                Request.Blob.ContentType = "image/png";
                                Request.data = File.ReadAllBytes( Request.appPath + CswNbtMetaDataObjectClass.IconPrefix100 + NodeType.IconFileName );
                            }
                        }
                    }
                    else
                    {
                        Request.data = File.ReadAllBytes( Request.appPath + "/Images/icons/300/_placeholder.gif" );
                        Request.Blob.ContentType = "image/gif";
                        Request.Blob.FileName = "empty.gif";
                    }
                }

                Return.Data = Request;
            } // NbtResources.Permit.canNodeType()
            else
            {
                throw new CswDniException( CswEnumErrorType.Warning, "You do no have sufficient priveledges to fetch this file.", "User " + NbtResources.CurrentNbtUser.UserId + " tried to fetch a file " + PropId + " they do not have permission on." );
            }
        }

        public static void clearImage( ICswResources CswResources, NodePropImageReturn Return, BlobDataParams Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswPropIdAttr PropId = new CswPropIdAttr( Request.propid );
            CswNbtNode Node = NbtResources.Nodes[PropId.NodeId];
            if( null != Node )
            {
                CswNbtMetaDataNodeTypeProp MetaDataProp = NbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
                if( null != MetaDataProp )
                {
                    CswNbtNodePropWrapper FileProp = Node.Properties[MetaDataProp];
                    if( NbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Edit, MetaDataProp.getNodeType(), NbtResources.CurrentNbtUser ) &&
                        NbtResources.Permit.isPropWritable( CswEnumNbtNodeTypePermission.Edit, MetaDataProp, null, FileProp ) )
                    {

                        int BlobDataId = Request.Blob.BlobDataId;

                        CswTableUpdate blobDataTS = NbtResources.makeCswTableUpdate( "clearImage", "blob_data" );
                        DataTable blobDataTbl = blobDataTS.getTable( "where blobdataid = " + BlobDataId );
                        foreach( DataRow row in blobDataTbl.Rows )
                        {
                            row.Delete();
                        }
                        blobDataTS.update( blobDataTbl );

                        Request.Blob = new CswNbtSdBlobData.CswNbtBlob();
                        Request.success = true;

                        getImageProp( CswResources, Return, Request );
                    }
                }
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Warning, "You do not have sufficient priveledges to clear this Image property", "User " + NbtResources.CurrentNbtUser.UserId + " attempted to call clearImg without sufficient priviledges." );
            }
        }

        public static void saveCaption( ICswResources CswResources, NodePropImageReturn Return, BlobDataParams Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            int BlobDataId = Request.Blob.BlobDataId;

            CswTableUpdate blobDataTS = NbtResources.makeCswTableUpdate( "clearImage", "blob_data" );
            DataTable blobDataTbl = blobDataTS.getTable( "where blobdataid = " + BlobDataId );
            foreach( DataRow row in blobDataTbl.Rows )
            {
                row["caption"] = Request.Blob.Caption;
            }
            blobDataTS.update( blobDataTbl );
        }

        public static void clearBlob( ICswResources CswResources, BlobDataReturn Return, BlobDataParams Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswPropIdAttr PropId = new CswPropIdAttr( Request.propid );
            CswNbtNode Node = NbtResources.Nodes[PropId.NodeId];
            if( null != Node )
            {
                CswNbtMetaDataNodeTypeProp MetaDataProp = NbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
                if( null != MetaDataProp )
                {
                    CswNbtNodePropWrapper FileProp = Node.Properties[MetaDataProp];
                    if( NbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Edit, MetaDataProp.getNodeType(), NbtResources.CurrentNbtUser ) &&
                        NbtResources.Permit.isPropWritable( CswEnumNbtNodeTypePermission.Edit, MetaDataProp, null, FileProp ) )
                    {
                        CswNbtSdTabsAndProps tabsandprops = new CswNbtSdTabsAndProps( NbtResources );
                        tabsandprops.ClearPropValue( Request.propid, true );

                        Request.Blob = new CswNbtSdBlobData.CswNbtBlob();
                        Request.success = true;

                        Return.Data = Request;
                    }
                    else
                    {
                        throw new CswDniException( CswEnumErrorType.Warning, "You do not have sufficient priveledges to clear this File property", "User " + NbtResources.CurrentNbtUser.UserId + " attempted to call clearBlob without sufficient priviledges." );
                    }
                }
            }
        }

        public static void getText( ICswResources CswResources, BlobDataReturn Return, BlobDataParams Request )
        {
            Stream stream = Request.postedFile.InputStream;
            using( StreamReader reader = new StreamReader( stream ) )
            {
                Request.filetext = reader.ReadToEnd();
            }
            Request.Blob.ContentType = Request.postedFile.ContentType;

            //IE9 sends the whole file url, we only want the filename
            string fileName = Path.GetFileName( Request.postedFile.FileName );

            Request.Blob.FileName = fileName;

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

        public static void getImageProp( ICswResources CswResources, NodePropImageReturn Return, BlobDataParams Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            CswPropIdAttr PropIdAttr = new CswPropIdAttr( Request.propid );
            CswNbtNode node = NbtResources.Nodes[PropIdAttr.NodeId];

            if( null != node )
            {
                CswNbtNodePropWrapper prop = node.Properties[PropIdAttr.NodeTypePropId];

                if( NbtResources.Permit.isPropWritable( CswEnumNbtNodeTypePermission.View, prop.NodeTypeProp, null, prop ) )
                {
                    if( Int32.MinValue == prop.JctNodePropId )
                    {
                        prop.makePropRow(); //if we don't have a jct_node_prop row for this prop, we do now
                        node.postChanges( true );
                    }
                    prop.AsImage.SetImages( Request.date );
                    if( null != prop )
                    {
                        Return.Data = prop;
                    }
                }
            }
        }

        private static int _getNodeTypePropIdFromJctNodePropId( CswNbtResources NbtResources, int JctNodePropId )
        {
            int ret = Int32.MinValue;
            string sql = "select nodetypepropid from jct_nodes_props where jctnodepropid = :jctnodepropid";
            CswArbitrarySelect arbSelect = NbtResources.makeCswArbitrarySelect( "BinaryData.getNodeTypePropIdFromJctNodePropId", sql );
            arbSelect.addParameter( "jctnodepropid", JctNodePropId.ToString() );
            DataTable tbl = arbSelect.getTable();
            if( tbl.Rows.Count > 0 )
            {
                ret = CswConvert.ToInt32( tbl.Rows[0]["nodetypepropid"] );
            }
            return ret;
        }

    } // class CswNbtWebServiceBinaryData

} // namespace ChemSW.Nbt.WebServices
