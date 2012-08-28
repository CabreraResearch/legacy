using System;
using System.Data;
using System.IO;
using System.Web;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceBinaryData
    {
        private readonly CswNbtResources _CswNbtResources;
        public CswNbtWebServiceBinaryData( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
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
