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
        protected override void OnInit( EventArgs e )
        {
            try
            {
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
                _CrystalReportViewer = new CrystalReportViewer();
                _CrystalReportViewer.ID = "CrystalReportViewer";
                ph.Controls.Add( _CrystalReportViewer );

                LoadReport();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.OnLoad( e );
        }

        private void LoadReport()
        {
            //_LoadingLiteral.Visible = false;
            CswPrimaryKey ReportPk = new CswPrimaryKey();
            if( false == string.IsNullOrEmpty( Request.Form["reportid"] ) )
            {
                ReportPk.FromString( Request.Form["reportid"] );
            }
            else if( false == string.IsNullOrEmpty( reportid.Value ) )
            {
                ReportPk.FromString( reportid.Value );
            }

            if( CswTools.IsPrimaryKey( ReportPk ) )
            {
                reportid.Value = ReportPk.ToString();

                CswNbtObjClassReport ReportNode = Master.CswNbtResources.Nodes.GetNode( ReportPk );
                if( null != ReportNode )
                {
                    CswNbtWebServiceReport.ReportData reportData = new CswNbtWebServiceReport.ReportData();
                    reportData.reportParams = CswNbtWebServiceReport.FormReportParamsToCollection( Request.Form );

                    string ReportSql = CswNbtObjClassReport.ReplaceReportParams( ReportNode.SQL.Text, reportData.ReportParamDictionary );
                    CswArbitrarySelect ReportSelect = Master.CswNbtResources.makeCswArbitrarySelect( "Report_" + ReportNode.NodeId.ToString() + "_Select", ReportSql );
                    DataTable ReportTable = ReportSelect.getTable();
                    if( ReportTable.Rows.Count > 0 )
                    {
                        // Get the Report Layout File
                        Int32 JctNodePropId = ReportNode.RPTFile.JctNodePropId;
                        if( JctNodePropId > 0 )
                        {
                            CswFilePath FilePathTools = new CswFilePath( Master.CswNbtResources );
                            string ReportTempFileName = FilePathTools.getFullReportFilePath( JctNodePropId.ToString() );
                            if( !File.Exists( ReportTempFileName ) )
                            {
                                ( new FileInfo( ReportTempFileName ) ).Directory.Create(); //creates the /rpt directory if it doesn't exist

                                DataTable JctTable = _getReportSQLFromDB( JctNodePropId );

                                if( JctTable.Rows.Count > 0 )
                                {
                                    if( !JctTable.Rows[0].IsNull( "blobdata" ) )
                                    {
                                        _createReportFileFromNodePropData( ReportTempFileName, JctTable.Rows[0] );
                                    }
                                    else
                                        throw new CswDniException( CswEnumErrorType.Warning, "Report is missing RPT file", "Report's RPTFile blobdata is null" );
                                }
                            }
                            if( File.Exists( ReportTempFileName ) )
                            {
                                _renderReport( ReportTempFileName, ReportTable );
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
                } // if(null != ReportNode)
            } // if( CswTools.IsPrimaryKey( ReportId ) )
        } // LoadReport()

        private DataTable _getReportSQLFromDB( Int32 JctNodePropId )
        {
            DataTable JctTable;
            CswTableSelect JctSelect = Master.CswNbtResources.makeCswTableSelect( "getReportLayoutBlob_select", "blob_data" );
            JctSelect.AllowBlobColumns = true;
            CswCommaDelimitedString SelectColumns = new CswCommaDelimitedString();
            SelectColumns.Add( "blobdata" );
            JctTable = JctSelect.getTable( SelectColumns, "jctnodepropid", JctNodePropId, "", true );
            return JctTable;
        }

        private void _createReportFileFromNodePropData( string ReportTempFileName, DataRow reportData )
        {
            byte[] BlobData = reportData["blobdata"] as byte[];
            FileStream fs = new FileStream( ReportTempFileName, FileMode.CreateNew );
            BinaryWriter BWriter = new BinaryWriter( fs, System.Text.Encoding.Default );
            BWriter.Write( BlobData );
        }

        private void _renderReport( string ReportTempFileName, DataTable ReportTable )
        {
            ReportDocument Report = new ReportDocument();
            Report.FileName = ReportTempFileName;
            Report.SetDataSource( ReportTable );
            _CrystalReportViewer.ReportSource = Report;
        }
    }
}
