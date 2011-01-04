using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.NbtWebControls;
using ChemSW.NbtWebControls.FieldTypes;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;
using ChemSW.DB;

namespace ChemSW.Nbt.WebPages
{
    public partial class Popup_PutBlob : System.Web.UI.Page
    {
        private string _Mode = "";
        private CswNbtResources _CswNbtResources = null;
        private CswBlobber _CswBlobber = null;

        protected override void OnInit(EventArgs e)
        {
            _CswNbtResources = Master.CswNbtResources;

            if (Request.QueryString["mode"] != null && Request.QueryString["nodeid"] != null && Request.QueryString["propid"] != null)
            {
                CswPrimaryKey NodeId = new CswPrimaryKey();
                NodeId.FromString( Request.QueryString["nodeid"] );
                Int32 PropId = CswConvert.ToInt32(Request.QueryString["propid"]);
                _Mode = Request.QueryString["mode"];
                if( NodeId != null && PropId > 0 )
                {
                    CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId );

                    // Fetch the JctNodePropID
                    Int32 JctNodePropId = Int32.MinValue;
                    CswTableSelect JctSelect = _CswNbtResources.makeCswTableSelect( "PutBlob_OnInit_select", "jct_nodes_props" );
                    DataTable JctTable = JctSelect.getTable( "where nodeid = " + NodeId.PrimaryKey.ToString() + " and nodetypepropid = " + PropId.ToString() );
                    if( JctTable.Rows.Count > 0 )
                    {
                        if( JctTable.Rows.Count > 1 )
                            throw new CswDniException( "Invalid Data Condition", "Popup_PutBlob.aspx.cs found more than 1 record for nodeid (" + NodeId.ToString() + ") and propid: " + PropId.ToString() );
                        JctNodePropId = CswConvert.ToInt32( JctTable.Rows[0]["jctnodepropid"].ToString() );
                    }

                    _CswBlobber = new CswBlobber( _CswNbtResources );
                    if( _Mode == "image" )
                    {
                        _CswBlobber.Type = CswBlobber.BlobType.Image;
                        _CswBlobber.ID = "ImageUpload";
                    }
                    else if( _Mode == "doc" )
                    {
                        _CswBlobber.Type = CswBlobber.BlobType.Document;
                        _CswBlobber.ID = "DocUpload";
                    }
                    _CswBlobber.NodeId = NodeId;
                    _CswBlobber.PropId = PropId;
                    _CswBlobber.JctNodePropId = JctNodePropId;
                    _CswBlobber.Length = MetaDataProp.Length;
                    ContentHolder.Controls.Add( _CswBlobber );
                }
            }
            else
            {
                throw new CswDniException("Invalid parameters for Popup_PutBlob.aspx");
            }
            base.OnInit(e);
        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            _CswBlobber.Save();

            String JS = @"  <script language=""Javascript""> 
                                Popup_OK_Clicked();
                            </script> ";

            System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), this.UniqueID + "_JS", JS, false);


        }
    }

}