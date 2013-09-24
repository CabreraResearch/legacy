using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Web;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Grid;
using ChemSW.Nbt.ObjClasses;
using ChemSW.NbtWebControls.FieldTypes;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;
using ExportOptions = CrystalDecisions.Shared.ExportOptions;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceReport
    {
        #region WCF Data Objects

        /// <summary>
        /// Return Object for Reports, which inherits from CswWebSvcReturn
        /// </summary>
        [DataContract]
        public class ReportReturn : CswWebSvcReturn
        {
            public ReportReturn()
            {
                Data = new ReportData();
            }
            [DataMember]
            public ReportData Data;
        }

        [DataContract]
        public class ReportData
        {
            [DataMember]
            public string reportFormat = string.Empty;
            [DataMember]
            public string nodeId = string.Empty;
            [DataMember]
            public string gridJSON = string.Empty;
            [DataMember]
            public Stream stream = null;
            [DataMember]
            public Collection<ReportParam> reportParams = new Collection<ReportParam>();

            [DataMember]
            public Collection<string> controlledParams
            {
                get
                {
                    return new Collection<string> { CswNbtObjClassReport.ControlledParams.NodeId, CswNbtObjClassReport.ControlledParams.RoleId, CswNbtObjClassReport.ControlledParams.UserId };
                }
                private set
                {
                    Collection<string> noSetterAllowed = value;
                }
            }

            [DataMember]
            public bool doesSupportCrystal = false;

            [DataContract]
            public class ReportParam
            {
                [DataMember]
                public string name = string.Empty;
                [DataMember]
                public string value = string.Empty;
            }

            private Dictionary<string, string> _reportParamsDictionary = null;
            public Dictionary<string, string> ReportParamDictionary
            {
                get
                {
                    if( null == _reportParamsDictionary )
                    {
                        _reportParamsDictionary = new Dictionary<string, string>();
                        foreach( ReportParam param in reportParams )
                        {
                            _reportParamsDictionary.Add( param.name, param.value );
                        }
                    }
                    return _reportParamsDictionary;
                }
            }

        }

        /// <summary>
        /// Return Object for Reports, which inherits from CswWebSvcReturn
        /// </summary>
        [DataContract]
        public class CrystalReportReturn : CswWebSvcReturn
        {
            public CrystalReportReturn()
            {
                Data = new CrystalReportData();
            }
            [DataMember]
            public CrystalReportData Data;
        }

        [DataContract]
        public class CrystalReportData
        {
            [DataMember]
            public string reportUrl = string.Empty;
            [DataMember]
            public bool hasResults = false;
        }

        #endregion WCF Data Objects

        // Need to double " in string values
        private static string _csvSafe( string str )
        {
            return str.Replace( "\"", "\"\"" );
        }

        public static void runReport( ICswResources CswResources, CswNbtWebServiceReport.ReportReturn Return, CswNbtWebServiceReport.ReportData reportParams )
        {
            JObject ret = new JObject();
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            DataTable rptDataTbl = _getReportTable( CswResources, Return, reportParams );
            CswNbtGrid cg = new CswNbtGrid( NbtResources );
            ret = cg.DataTableToJSON( rptDataTbl );
            reportParams.gridJSON = ret.ToString();
            Return.Data = reportParams;
        }

        public static void runReportCSV( ICswResources CswResources, CswNbtWebServiceReport.ReportReturn Return, CswNbtWebServiceReport.ReportData reportParams )
        {
            DataTable rptDataTbl = _getReportTable( CswResources, Return, reportParams );
            reportParams.stream = wsTools.ReturnCSVStream( rptDataTbl );
            Return.Data = reportParams;
        }

        private static DataTable _getReportTable( ICswResources CswResources, CswNbtWebServiceReport.ReportReturn Return, CswNbtWebServiceReport.ReportData reportParams )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            DataTable rptDataTbl = new DataTable();
            CswPrimaryKey pk = CswConvert.ToPrimaryKey( reportParams.nodeId );
            if( CswTools.IsPrimaryKey( pk ) )
            {
                CswNbtObjClassReport reportNode = NbtResources.Nodes[pk];
                if( string.Empty != reportNode.SQL.Text )
                {
                    string ReportSql = "";
                    //Case 30293: We are not trying to solve all of the (usability) issues with SQL Reporting today;
                    //rather, we just want to return friendlier errors when SQL faults occur
                    try
                    {
                        ReportSql = CswNbtObjClassReport.ReplaceReportParams( reportNode.SQL.Text, reportParams.ReportParamDictionary );
                        CswArbitrarySelect cswRptSql = NbtResources.makeCswArbitrarySelect( "report_sql", ReportSql );
                        rptDataTbl = cswRptSql.getTable();
                        if( string.IsNullOrEmpty( rptDataTbl.TableName ) && null != reportNode )
                        {
                            rptDataTbl.TableName = reportNode.ReportName.Text;
                        }
                    }
                    catch( CswSqlException CswException )
                    {
                        CswDniException NewException = new CswDniException( CswEnumErrorType.Warning, "SQL Execution failed with error: " + CswException.OracleError, "Could not execute SQL: {" + CswException.Sql + "}", CswException );
                        //CswException.Add( NewException );
                        throw NewException;
                    }
                    catch( Exception Ex )
                    {
                        throw new CswDniException( CswEnumErrorType.Warning, "Invalid SQL.", "Could not execute SQL: {" + ReportSql + "}", Ex );
                    }
                }
                else
                {
                    throw ( new CswDniException( "Report has no SQL to run!" ) );
                }
            }
            return rptDataTbl;
        }

        public static Collection<CswNbtWebServiceReport.ReportData.ReportParam> FormReportParamsToCollection( NameValueCollection FormData )
        {
            Collection<CswNbtWebServiceReport.ReportData.ReportParam> reportParams = new Collection<CswNbtWebServiceReport.ReportData.ReportParam>();
            foreach( string key in FormData.AllKeys )
            {
                if( false == key.Equals( "reportid" ) ) //reportid is a special case and is used above
                {
                    CswNbtWebServiceReport.ReportData.ReportParam param = new CswNbtWebServiceReport.ReportData.ReportParam();
                    param.name = key;
                    param.value = FormData[key];
                    reportParams.Add( param );
                }
            }
            return reportParams;
        }

        public static void getReportInfo( ICswResources CswResources, CswNbtWebServiceReport.ReportReturn Return, CswNbtWebServiceReport.ReportData Request )
        {
            CswNbtResources NBTResources = (CswNbtResources) CswResources;
            CswPrimaryKey pk = new CswPrimaryKey();
            pk = CswConvert.ToPrimaryKey( Request.nodeId );
            if( CswTools.IsPrimaryKey( pk ) )
            {
                CswNbtObjClassReport reportNode = NBTResources.Nodes[pk];

                Request.doesSupportCrystal = ( false == reportNode.RPTFile.Empty );

                Request.reportParams = new Collection<ReportData.ReportParam>();
                foreach( var paramPair in reportNode.ExtractReportParams( NBTResources.Nodes[NBTResources.CurrentNbtUser.UserId] ) )
                {
                    ReportData.ReportParam paramObj = new ReportData.ReportParam();
                    paramObj.name = paramPair.Key;
                    paramObj.value = paramPair.Value;
                    Request.reportParams.Add( paramObj );
                }
            }
            Return.Data = Request;
        }


        #region Crystal

        public static void runCrystalReport( ICswResources CswResources, CswNbtWebServiceReport.CrystalReportReturn Return, CswNbtWebServiceReport.ReportData Request )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswPrimaryKey ReportPk = CswConvert.ToPrimaryKey( Request.nodeId );
            if( CswTools.IsPrimaryKey( ReportPk ) )
            {
                CswNbtObjClassReport ReportNode = CswNbtResources.Nodes.GetNode( ReportPk );
                if( null != ReportNode )
                {
                    //Request.reportParams = FormReportParamsToCollection( HttpContext.Current.Request.Form );

                    string ReportSql = CswNbtObjClassReport.ReplaceReportParams( ReportNode.SQL.Text, Request.ReportParamDictionary );
                    CswArbitrarySelect ReportSelect = CswNbtResources.makeCswArbitrarySelect( "Report_" + ReportNode.NodeId.ToString() + "_Select", ReportSql );
                    DataTable ReportTable = ReportSelect.getTable();
                    if( ReportTable.Rows.Count > 0 )
                    {

                        // Get the Report Layout File
                        Int32 JctNodePropId = ReportNode.RPTFile.JctNodePropId;
                        if( JctNodePropId > 0 )
                        {
                            CswFilePath FilePathTools = new CswFilePath( CswNbtResources );
                            string ReportTempFileName = FilePathTools.getFullReportFilePath( JctNodePropId.ToString() );
                            if( !File.Exists( ReportTempFileName ) )
                            {
                                ( new FileInfo( ReportTempFileName ) ).Directory.Create(); //creates the /rpt directory if it doesn't exist

                                CswTableSelect JctSelect = CswNbtResources.makeCswTableSelect( "getReportLayoutBlob_select", "blob_data" );
                                JctSelect.AllowBlobColumns = true;
                                CswCommaDelimitedString SelectColumns = new CswCommaDelimitedString();
                                SelectColumns.Add( "blobdata" );
                                DataTable JctTable = JctSelect.getTable( SelectColumns, "jctnodepropid", JctNodePropId, "", true );

                                if( JctTable.Rows.Count > 0 )
                                {
                                    if( !JctTable.Rows[0].IsNull( "blobdata" ) )
                                    {
                                        byte[] BlobData = JctTable.Rows[0]["blobdata"] as byte[];
                                        FileStream fs = new FileStream( ReportTempFileName, FileMode.CreateNew );
                                        BinaryWriter BWriter = new BinaryWriter( fs, System.Text.Encoding.Default );
                                        BWriter.Write( BlobData );
                                    }
                                    else
                                    {
                                        throw new CswDniException( CswEnumErrorType.Warning, "Report is missing RPT file", "Report's RPTFile blobdata is null" );
                                    }
                                }
                            }
                            if( File.Exists( ReportTempFileName ) )
                            {
                                Return.Data.reportUrl = _saveCrystalReport( CswNbtResources, ReportTempFileName, ReportNode.ReportName.Text, ReportTable );
                                Return.Data.hasResults = true;
                            }
                        } // if( JctNodePropId > 0 )
                    } // if(ReportTable.Rows.Count > 0) 
                    else
                    {
                        Return.Data.hasResults = false;
                    }
                } // if(null != ReportNode)
            } // if( CswTools.IsPrimaryKey( ReportId ) )
        } // LoadReport()


        private static string _saveCrystalReport( CswNbtResources CswNbtResources, string ReportTempFileName, string ReportName, DataTable ReportTable )
        {
            CswTempFile TempFile = new CswTempFile( CswNbtResources );
            string DestinationFileName = DateTime.Now.Ticks + "_" + CswTools.SafeFileName( ReportName ) + ".pdf";
            string DestFilePath, DestWebPath;
            TempFile.getFullTempFilePath( DestinationFileName, out DestFilePath, out DestWebPath );

            DiskFileDestinationOptions dFileDOpts = new DiskFileDestinationOptions();
            dFileDOpts.DiskFileName = DestFilePath;

            ReportDocument oRD = new ReportDocument();
            oRD.Load( ReportTempFileName );
            oRD.SetDataSource( ReportTable );

            ExportOptions eOpts = new ExportOptions();
            eOpts = oRD.ExportOptions;
            eOpts.ExportDestinationType = ExportDestinationType.DiskFile;
            eOpts.ExportFormatType = ExportFormatType.PortableDocFormat;
            eOpts.DestinationOptions = dFileDOpts;

            oRD.Export();
            oRD.Close();

            return DestWebPath;
        }
        #endregion Crystal



    } // class CswNbtWebServiceReport
} // namespace ChemSW.Nbt.WebServices