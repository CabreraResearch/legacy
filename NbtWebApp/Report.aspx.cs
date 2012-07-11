using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.WebServices;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Web;

namespace ChemSW.Nbt.WebPages
{

    public partial class Report : Page
    {
        protected CrystalReportViewer _CrystalReportViewer;
        //protected CswLiteralText _LoadingLiteral;
        protected override void OnInit( EventArgs e )
        {
            try
            {
                //LoadReportButton.Style.Add( HtmlTextWriterStyle.Display, "none" );
                //LoadReportButton.Click += new EventHandler( LoadReportButton_Click );

                //_LoadingLiteral = new CswLiteralText( "Loading..." );
                //ph.Controls.Add( _LoadingLiteral );

                _CrystalReportViewer = new CrystalReportViewer();
                _CrystalReportViewer.ID = "CrystalReportViewer";
                ph.Controls.Add( _CrystalReportViewer );

                //if( Page.IsPostBack )
                //{
                // this allows paging on the report control to work
                LoadReport();
                //}
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnInit( e );
        }

        protected override void OnLoad( EventArgs e )
        {
            try
            {
                //Master.AjaxManager.AjaxSettings.AddAjaxSetting( LoadReportButton, ph );
                //Master.AjaxManager.AjaxSettings.AddAjaxSetting( _CrystalReportViewer, _CrystalReportViewer );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.OnLoad( e );
        }

        protected override void OnPreRender( EventArgs e )
        {
            //            string JS = @"  <script>
            //                            function LoadReport()
            //                            {
            //                                document.getElementById('" + LoadReportButton.ClientID + @"').click();
            //                            }
            //                            </script>";

            //            System.Web.UI.ScriptManager.RegisterClientScriptBlock( this, this.GetType(), this.UniqueID + "_JS", JS, false );

            base.OnPreRender( e );
        }

        //void LoadReportButton_Click( object sender, EventArgs e )
        //{
        //    LoadReport();
        //}

        private void LoadReport()
        {
            //_LoadingLiteral.Visible = false;
            if( Request.QueryString["reportid"] != string.Empty )
            {
                CswPrimaryKey ReportId = new CswPrimaryKey();
                ReportId.FromString( Request.QueryString["reportid"] );
                CswNbtNode Node = Master.CswNbtResources.Nodes.GetNode( ReportId );
                CswNbtObjClassReport ReportNode = (CswNbtObjClassReport) Node;

                // Get the Report Data
                //CswNbtView View = Master.CswNbtResources.ViewSelect.restoreView( ReportNode.View.ViewId );
                //if( View == null )
                //    throw new CswDniException( ErrorType.Warning, "Report has invalid View", "Report received a null view" );

                //ICswNbtTree Tree = Master.CswNbtResources.Trees.getTreeFromView( View, true, true, false, false );
                //Tree.goToRoot();
                ////Tree.goToNthChild( 0 );
                //if( Tree.getChildNodeCount() > 0 )
                //{
                // BROKEN BY case 24709
                //Tree.XmlTreeDestinationFormat = XmlTreeDestinationFormat.ReportingDataSet;
                //string TransformedXml = Tree.getTreeAsXml();
                //StringReader XmlReader = new StringReader( TransformedXml );
                //DataSet ReportData = new DataSet();
                //ReportData.ReadXml( XmlReader );

                string ReportSql = ReportNode.getUserContextSql( Master.CswNbtResources.CurrentNbtUser.Username );
                CswArbitrarySelect ReportSelect = Master.CswNbtResources.makeCswArbitrarySelect( "Report_" + ReportNode.NodeId.ToString() + "_Select", ReportSql );
                DataTable ReportTable = ReportSelect.getTable();
                if( ReportTable.Rows.Count > 0 )
                {
                    // Get the Report Layout File
                    Int32 JctNodePropId = ReportNode.RPTFile.JctNodePropId;
                    if( JctNodePropId > 0 )
                    {
                        wsTools Tools = new wsTools( Master.CswNbtResources );
                        string ReportTempFileName = Tools.getFullFilePath( JctNodePropId.ToString() + ".rpt" );
                        if( !File.Exists( ReportTempFileName ) )
                        {
                            CswTableSelect JctSelect = Master.CswNbtResources.makeCswTableSelect( "getReportLayoutBlob_select", "jct_nodes_props" );
                            JctSelect.AllowBlobColumns = true;
                            CswCommaDelimitedString SelectColumns = new CswCommaDelimitedString();
                            SelectColumns.Add( "blobdata" );
                            SelectColumns.Add( "field2" );
                            DataTable JctTable = JctSelect.getTable( SelectColumns, "jctnodepropid", JctNodePropId, "", true );

                            if( JctTable.Rows.Count > 0 )
                            {
                                if( !JctTable.Rows[0].IsNull( "blobdata" ) )
                                {
                                    string ContentType = JctTable.Rows[0]["field2"].ToString();
                                    byte[] BlobData = JctTable.Rows[0]["blobdata"] as byte[];
                                    FileStream fs = new FileStream( ReportTempFileName, FileMode.CreateNew );
                                    BinaryWriter BWriter = new BinaryWriter( fs, System.Text.Encoding.Default );
                                    BWriter.Write( BlobData );
                                }
                                else
                                    throw new CswDniException( ErrorType.Warning, "Report is missing RPT file", "Report's RPTFile blobdata is null" );
                            }
                        }

                        if( File.Exists( ReportTempFileName ) )
                        {
                            // Render the Report
                            ReportDocument Report = new ReportDocument();
                            Report.FileName = ReportTempFileName;
                            Report.SetDataSource( ReportTable );
                            _CrystalReportViewer.ReportSource = Report;
                        }
                    } // if( JctNodePropId > 0 )
                } // if(ReportTable.Rows.Count > 0) 
                else
                {
                    Label NoResultsLabel = new Label();
                    NoResultsLabel.ID = "NoResultsLabel";
                    NoResultsLabel.Text = "<br><br>There are no rows to display.<br><br><br>";
                    ph.Controls.Add( NoResultsLabel );
                }
            }
        }
    }
}
