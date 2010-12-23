using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.DB;
using System.Web.UI.HtmlControls;
using System.IO;
using ChemSW.Core;

namespace ChemSW.NbtWebControls
{
    public class CswBlobber : CompositeControl, INamingContainer
    {
        private CswNbtResources _CswNbtResources = null;
        private FileUpload _FileUpload = null;

        public CswBlobber(CswNbtResources Objs)
        {
            _CswNbtResources = Objs;
        }

        public enum BlobType { Image, Document };

        private BlobType _Type;
        public BlobType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private CswPrimaryKey _NodeId;
        public CswPrimaryKey NodeId
        {
            get { return _NodeId; }
            set { _NodeId = value; }
        }
       
        private Int32 _PropId;
        public Int32 PropId
        {
            get { return _PropId; }
            set { _PropId = value; }
        }
        
        private Int32 _JctNodePropId;
        public Int32 JctNodePropId
        {
            get { return _JctNodePropId; }
            set { _JctNodePropId = value; }
        }

        private Int32 _Length;
        public Int32 Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        public void Save()
        {
            EnsureChildControls();
            if (_FileUpload.HasFile)
            {
                Byte[] Data = new Byte[_FileUpload.FileContent.Length];
                _FileUpload.FileContent.Read(Data, 0, (int)_FileUpload.FileContent.Length);

                CswTableUpdate JctUpdate = _CswNbtResources.makeCswTableUpdate( "Blobber_save_update", "jct_nodes_props" );
                JctUpdate.AllowBlobColumns = true;
                if( JctNodePropId > 0 )
                {
                    DataTable JctTable = JctUpdate.getTable( "jctnodepropid", JctNodePropId );
                    JctTable.Rows[0]["blobdata"] = Data;
                    JctTable.Rows[0]["field1"] = _FileUpload.FileName;
                    JctTable.Rows[0]["field2"] = _FileUpload.PostedFile.ContentType;
                    JctUpdate.update( JctTable );
                }
                else
                {
                    DataTable JctTable = JctUpdate.getEmptyTable();
                    DataRow JRow = JctTable.NewRow();
                    JRow["nodetypepropid"] = PropId;
                    JRow["nodeid"] = CswConvert.ToDbVal( NodeId.PrimaryKey );
                    JRow["nodeidtablename"] = NodeId.TableName;
                    JRow["blobdata"] = Data;
                    JRow["field1"] = _FileUpload.FileName;
                    JRow["field2"] = _FileUpload.PostedFile.ContentType;
                    JctTable.Rows.Add( JRow );
                    JctNodePropId = CswConvert.ToInt32( JRow["jctnodepropid"].ToString() );
                    JctUpdate.update( JctTable );
                }

                // BZ 4828 - Clear the temp/ directory of matching reports
                string FilePath = Page.Server.MapPath( "" ) + @"\temp\" + JctNodePropId.ToString() + ".rpt";
                if( File.Exists( FilePath ) )
                    File.Delete( FilePath );
            }
        }

        protected override void CreateChildControls()
        {
            if( NodeId.TableName == "nodes" )
            {
                CswTableSelect NodeSelect = _CswNbtResources.makeCswTableSelect( "Blobber_node_select", "nodes" );
                DataTable NodeTable = NodeSelect.getTable( "nodeid", NodeId.PrimaryKey );
                string NodeName = NodeTable.Rows[0]["nodename"].ToString();

                string PropName = _CswNbtResources.MetaData.getNodeTypeProp( PropId ).PropName;

                Label TitleLabel = new Label();
                TitleLabel.ID = "TitleLabel";
                if( Type == BlobType.Image )
                    TitleLabel.Text = "Upload Image for " + PropName + " property of " + NodeName;
                else if( Type == BlobType.Document )
                    TitleLabel.Text = "Upload Document for " + PropName + " property of " + NodeName;
                this.Controls.Add( TitleLabel );

                this.Controls.Add( new HtmlGenericControl( "br" ) );

                _FileUpload = new FileUpload();
                _FileUpload.ID = "fileupload";
                _FileUpload.CssClass = "fileupload";
                _FileUpload.Attributes.Add( "size", Length.ToString() );
                this.Controls.Add( _FileUpload );
            }
            base.CreateChildControls();
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }
    }
}
