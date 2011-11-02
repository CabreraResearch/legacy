using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ChemSW.NbtWebControls;
using ChemSW.DB;
using ChemSW.Core;

namespace ChemSW.Nbt.WebPages
{
    public partial class GetBlob : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["jctnodepropid"] != null)
            {
                displayBlobData(Convert.ToInt32(Request.QueryString["jctnodepropid"]));
            }
        }
        
        private void displayBlobData(Int32 JctNodePropId)
        {
            CswTableSelect JctSelect = Master.CswNbtResources.makeCswTableSelect("displayBlobData_select", "jct_nodes_props" );
            JctSelect.AllowBlobColumns = true;
            CswCommaDelimitedString SelectColumns = new CswCommaDelimitedString();
            SelectColumns.Add("blobdata");
            SelectColumns.Add("field2");
            SelectColumns.Add( "field1" );
            DataTable JctTable = JctSelect.getTable( SelectColumns, "jctnodepropid", JctNodePropId, "", true );

            if( JctTable.Rows.Count > 0 )
            {

                if( !JctTable.Rows[ 0 ].IsNull( "blobdata" ) )
                {
                    string FileName = JctTable.Rows[0]["field1"].ToString();
                    string ContentType = JctTable.Rows[ 0 ][ "field2" ].ToString();
                    byte[] BlobData = JctTable.Rows[ 0 ][ "blobdata" ] as byte[];

                    MemoryStream mem = new MemoryStream();
                    BinaryWriter BWriter = new BinaryWriter( mem, System.Text.Encoding.Default );
                    BWriter.Write( BlobData );
                    
                    Response.ClearContent();
                    Response.ContentType = ContentType;
                    Response.BinaryWrite( mem.ToArray() );
                    Response.AddHeader( "Content-Disposition", "attachment;filename=" + FileName + ";" );
                    Response.End();

                }//if we actually have blob data

            }//if we got any result
        }
    }
}
