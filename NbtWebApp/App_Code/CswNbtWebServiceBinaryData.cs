using System;
using System.Data;
using System.IO;
using System.Web;
using ChemSW.Core;
using ChemSW.DB;

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
                    DataTable JctTable = JctSelect.getTable( SelectColumns, "jctnodepropid", JctNodePropId, "", true );

                    byte[] BlobData;
                    string ContentType;
                    string FileName;
                    if( JctTable.Rows.Count > 0 && false == JctTable.Rows[0].IsNull( "blobdata" ) )
                    {
                        FileName = JctTable.Rows[0]["field1"].ToString();
                        ContentType = JctTable.Rows[0]["field2"].ToString();
                        BlobData = JctTable.Rows[0]["blobdata"] as byte[];
                    }
                    else
                    {
                        FileName = "empty";
                        ContentType = "image/gif";
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
