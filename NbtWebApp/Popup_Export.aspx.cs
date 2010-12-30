using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using ChemSW.NbtWebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.CswWebControls;
using ChemSW.Nbt.ImportExport;

namespace ChemSW.Nbt.WebPages
{
    public partial class Popup_Export : System.Web.UI.Page
    {

        public CswNbtView View
        {
            get { return Master.CswNbtView; }
        }

        private CswMainMenu.ExportOutputFormat Format;
        private RadioButton PropsInViewOnlyRadio;
        private RadioButton AllPropsRadio;
        private Button ExportButton;

        protected override void OnInit( EventArgs e )
        {
            try
            {
                string strFormat;
                if( Request.QueryString["format"] == null || Request.QueryString["format"] == string.Empty )
                    strFormat = CswMainMenu.ExportOutputFormat.Excel.ToString();
                else
                    strFormat = Request.QueryString["format"].ToString();
                Format = (CswMainMenu.ExportOutputFormat) Enum.Parse( typeof( CswMainMenu.ExportOutputFormat ), strFormat, true );

                EnsureChildControls();

                if( Format != CswMainMenu.ExportOutputFormat.MobileXML || Request.QueryString["sessionviewid"] == null )
                {
                    _doExport();
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnInit( e );
        }

        protected override void CreateChildControls()
        {
            if( Format == CswMainMenu.ExportOutputFormat.MobileXML )
            {
                CswAutoTable Table = new CswAutoTable();
                Table.ID = "exporttable";
                ph.Controls.Add( Table );

                PropsInViewOnlyRadio = new RadioButton();
                PropsInViewOnlyRadio.ID = "PropsInViewOnlyCheckBox";
                PropsInViewOnlyRadio.Text = "Export Only Properties Found In View";
                PropsInViewOnlyRadio.Checked = true;
                PropsInViewOnlyRadio.GroupName = "exportview";
                Table.addControl( 0, 0, PropsInViewOnlyRadio );

                AllPropsRadio = new RadioButton();
                AllPropsRadio.ID = "AllPropsRadio";
                AllPropsRadio.Text = "Export All Properties";
                AllPropsRadio.Checked = false;
                AllPropsRadio.GroupName = "exportview";
                Table.addControl( 1, 0, AllPropsRadio );

                ExportButton = new Button();
                ExportButton.ID = "ExportButton";
                ExportButton.CssClass = "button";
                ExportButton.Text = "Export";
                ExportButton.Click += new EventHandler( ExportButton_Click );
                Table.addControl( 2, 0, ExportButton );
            }

            base.CreateChildControls();
        }
        
        protected void ExportButton_Click( object sender, EventArgs e )
        {
            try
            {
                _doExport();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        private void _doExport()
        {
            if( Format == CswMainMenu.ExportOutputFormat.ReportXML || Format == CswMainMenu.ExportOutputFormat.MobileXML )
            {
                // XML for Crystal Reports or Import/Export/Mobile
                CswNbtView CswNbtView = null;
                CswNbtMetaDataNodeType NodeType = null;
                if( Request.QueryString["nodetypeid"] != null )
                {
                    NodeType = Master.CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( Request.QueryString["nodetypeid"] ) );
                }
                else if( Request.QueryString["sessionviewid"] != null )
                {
                    CswNbtView = (CswNbtView) Master.CswNbtResources.ViewCache.getView( CswConvert.ToInt32( Request.QueryString["sessionviewid"] ) );
                }
                else
                {
                    CswNbtView = View;
                }

                string ExportXmlString = string.Empty;
                if( Format == CswMainMenu.ExportOutputFormat.ReportXML )
                {
                    ICswNbtTree CswNbtTree = Master.CswNbtResources.Trees.getTreeFromView( CswNbtView, true, false, false, true );
                    CswNbtTree.XmlTreeDestinationFormat = XmlTreeDestinationFormat.ReportingDataSet;
                    ExportXmlString = CswNbtTree.getTreeAsXml();
                }
                else if( Format == CswMainMenu.ExportOutputFormat.MobileXML )
                {
                    bool PropsInViewOnly = ( PropsInViewOnlyRadio.Checked );
                    CswNbtImportExport Exporter = new CswNbtImportExport( Master.CswNbtResources );
                    XmlDocument ExportXmlDoc = null;
                    if( CswNbtView != null )
                        ExportXmlDoc = Exporter.ExportView( CswNbtView, true, PropsInViewOnly );
                    else
                        ExportXmlDoc = Exporter.ExportNodeType( NodeType );
                    ExportXmlString = ExportXmlDoc.InnerXml;
                }

                //Response.ClearContent();
                //Response.ContentType = "text/xml";
                //Response.Write( AllXml );
                //Response.End();

                string Timestamp = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                string FileName = string.Empty;
                if( CswNbtView != null )
                    FileName = CswTools.SafeFileName( CswNbtView.ViewName ) + "_" + Timestamp + ".xml";
                else
                    FileName = CswTools.SafeFileName( NodeType.NodeTypeName ) + "_" + Timestamp + ".xml";

                string TempFilePath = Server.MapPath( "" ) + @"\temp\";
                string TempFileFqpn = TempFilePath + FileName;

                ASCIIEncoding ASCIIEncoding = new ASCIIEncoding();
                Stream Stream = new FileStream( TempFileFqpn,
                                                FileMode.Create,
                                                System.Security.AccessControl.FileSystemRights.CreateFiles |
                                                System.Security.AccessControl.FileSystemRights.Write |
                                                System.Security.AccessControl.FileSystemRights.Write,
                                                FileShare.ReadWrite,
                                                8,
                                                FileOptions.None );
                Stream.Write( ASCIIEncoding.GetBytes( ExportXmlString ), 0, ASCIIEncoding.GetByteCount( ExportXmlString ) );
                Stream.Flush();
                Stream.Close();

                FileInfo FileInfo = new FileInfo( TempFileFqpn );
                Response.Clear();
                Response.AddHeader( "content-disposition", "attachment;filename=" + FileName );
                Response.ContentType = "text/xml";
                Response.WriteFile( TempFileFqpn, 0, FileInfo.Length );
                Response.Flush();
                Response.End();
            }
            else
            {
                // Grid Export

                CswNodesGrid _NodesGrid = new CswNodesGrid( Master.CswNbtResources );
                _NodesGrid.OnError += new CswErrorHandler( Master.HandleError );
                this.Controls.Add( _NodesGrid );

                NbtViewRenderingMode NbtViewRenderingMode = NbtViewRenderingMode.Grid;
                if( null != Request.QueryString["renderingmode"] )
                    NbtViewRenderingMode = (NbtViewRenderingMode) Enum.Parse( typeof( NbtViewRenderingMode ), Request.QueryString["renderingmode"].ToString() );

                if( NbtViewRenderingMode.Grid == NbtViewRenderingMode )
                {
                    if( Request.QueryString["nodeid"] != null &&
                        Request.QueryString["propid"] != null &&
                        //CswTools.IsInteger( Request.QueryString["nodeid"].ToString() ) &&
                        CswTools.IsInteger( Request.QueryString["propid"].ToString() ) )
                    {
                        CswNbtMetaDataNodeTypeProp MetaDataProp = Master.CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( Request.QueryString["propid"].ToString() ) );
                        CswPrimaryKey NodeId = new CswPrimaryKey();
                        NodeId.FromString( Request.QueryString["nodeid"] );
                        CswNbtNode Node = Master.CswNbtResources.Nodes[NodeId];
                        CswNbtNodePropWrapper Prop = Node.Properties[MetaDataProp];
                        CswNbtView GridView = Prop.AsGrid.View;
                        _NodesGrid.View = GridView;
                    }
                    else if( Request.QueryString["sessionviewid"] != null )
                    {
                        //CswNbtView AView = (CswNbtView) CswNbtViewFactory.restoreView( Master.CswNbtResources, CswConvert.ToInt32( Request.QueryString["viewid"].ToString() ) );
                        CswNbtView AView = (CswNbtView) Master.CswNbtResources.ViewCache.getView( CswConvert.ToInt32( Request.QueryString["sessionviewid"] ) );
                        _NodesGrid.View = AView;
                    }
                    else
                    {
                        _NodesGrid.View = View;
                    }

                    _NodesGrid.DataBind();
                    _NodesGrid.Grid.ExportSettings.IgnorePaging = true;

                    switch( Format )
                    {
                        case CswMainMenu.ExportOutputFormat.CSV:
                            _NodesGrid.Grid.MasterTableView.ExportToCSV();
                            break;
                        case CswMainMenu.ExportOutputFormat.Excel:
                            //_NodesGrid.Grid.MasterTableView.ExportToExcel();
                            _NodesGrid.Grid.MasterTableView.ExportToCSV();
                            break;
                        case CswMainMenu.ExportOutputFormat.PDF:
                            _NodesGrid.Grid.MasterTableView.ExportToPdf();
                            break;
                        case CswMainMenu.ExportOutputFormat.Word:
                            _NodesGrid.Grid.MasterTableView.ExportToWord();
                            break;
                        default:
                            throw new CswDniException( "Unknown Export Format", "The export format " + Request.QueryString["format"].ToString() + " is not supported." );
                    }

                } // if( NbtViewRenderingMode.Grid == NbtViewRenderingMode )
            } // if-else( Format == CswMainMenu.ExportOutputFormat.Mobile || Format == CswMainMenu.ExportOutputFormat.XML )
        } // _doExport()

    } // class Popup_Export
}//namespace ChemSW.Nbt.WebPages
