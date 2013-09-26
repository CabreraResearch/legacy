using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Grid;
using ChemSW.Nbt.ObjClasses;
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

            private CswNbtObjClassReport _ReportNode = null;
            public CswNbtObjClassReport ReportNode
            {
                get
                {
                    return _ReportNode;
                }
                set
                {
                    _ReportNode = value;
                }
            }

            private CswPrimaryKey _NodeId = null;

            public CswPrimaryKey NodeId
            {
                get
                {
                    return _NodeId;
                }
                set
                {
                    _NodeId = value;
                }
            }

            [DataMember( Name = "nodeId" )]
            public string nodeIdStr
            {
                get
                {
                    string Ret = string.Empty;
                    if( CswTools.IsPrimaryKey( NodeId ) )
                    {
                        Ret = NodeId.ToString();
                    }
                    return Ret;
                }
                set
                {
                    NodeId = new CswPrimaryKey();
                    NodeId.FromString( value );
                }
            }

            [DataMember]
            public string gridJSON = string.Empty;

            [DataMember]
            public int RowLimit = Int32.MinValue;

            [DataMember]
            public int RowCount = Int32.MinValue;

            [DataMember]
            public bool Truncated = false;

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

            DataTable rptDataTbl = _getReportTable( CswResources, reportParams );

            CswNbtGrid cg = new CswNbtGrid( NbtResources );
            ret = cg.DataTableToJSON( rptDataTbl );
            reportParams.gridJSON = ret.ToString();
            Return.Data = reportParams;
        }

        public static void runReportCSV( ICswResources CswResources, CswNbtWebServiceReport.ReportReturn Return, CswNbtWebServiceReport.ReportData reportParams )
        {
            DataTable rptDataTbl = _getReportTable( CswResources, reportParams );
            reportParams.stream = wsTools.ReturnCSVStream( rptDataTbl );
            Return.Data = reportParams;
        }

        private static DataTable _getReportTable( ICswResources CswResources, CswNbtWebServiceReport.ReportData reportParams )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            DataTable rptDataTbl = null;
            reportParams.ReportNode = NbtResources.Nodes[reportParams.NodeId];
            if( null != reportParams.ReportNode )
            {
                if( false == string.IsNullOrEmpty( reportParams.ReportNode.SQL.Text ) )
                {
                    string ReportSql = "";
                    //Case 30293: We are not trying to solve all of the (usability) issues with SQL Reporting today;
                    //rather, we just want to return friendlier errors when SQL faults occur
                    try
                    {
                        ReportSql = CswNbtObjClassReport.ReplaceReportParams( reportParams.ReportNode.SQL.Text, reportParams.ReportParamDictionary );
                        CswArbitrarySelect cswRptSql = NbtResources.makeCswArbitrarySelect( "report_sql", ReportSql );

                        reportParams.RowLimit = CswConvert.ToInt32( NbtResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.sql_report_resultlimit.ToString() ) );
                        if( 0 >= reportParams.RowCount )
                        {
                            reportParams.RowCount = 500;
                        }

                        //Getting 1 more than RowLimit in order to determine if truncation occurred
                        rptDataTbl = cswRptSql.getTable( PageLowerBoundExclusive: 0, PageUpperBoundInclusive: reportParams.RowLimit + 1, RequireOneRow: false, UseLogicalDelete: false );
                        if( string.IsNullOrEmpty( rptDataTbl.TableName ) && null != reportParams.ReportNode )
                        {
                            rptDataTbl.TableName = reportParams.ReportNode.ReportName.Text;
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
                    finally
                    {
                        if( null != rptDataTbl )
                        {
                            reportParams.RowCount = rptDataTbl.Rows.Count;
                            reportParams.Truncated = reportParams.RowCount > reportParams.RowLimit;
                            if( reportParams.Truncated )
                            {
                                rptDataTbl.Rows.RemoveAt( reportParams.RowCount - 1 );
                                reportParams.RowCount -= 1;
                            }
                        }
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

        public static void getReportInfo( ICswResources CswResources, CswNbtWebServiceReport.ReportReturn Return, CswNbtWebServiceReport.ReportData reportParams )
        {
            CswNbtResources NBTResources = (CswNbtResources) CswResources;
            reportParams.ReportNode = NBTResources.Nodes[reportParams.NodeId];
            if( null != reportParams.ReportNode )
            {
                reportParams.doesSupportCrystal = ( false == reportParams.ReportNode.RPTFile.Empty );

                reportParams.reportParams = new Collection<ReportData.ReportParam>();
                foreach( var paramPair in reportParams.ReportNode.ExtractReportParams( NBTResources.Nodes[NBTResources.CurrentNbtUser.UserId] ) )
                {
                    ReportData.ReportParam paramObj = new ReportData.ReportParam();
                    paramObj.name = paramPair.Key;
                    paramObj.value = paramPair.Value;
                    reportParams.reportParams.Add( paramObj );
                }
            }
            Return.Data = reportParams;
        }


        #region Crystal

        public static void runCrystalReport( ICswResources CswResources, CswNbtWebServiceReport.CrystalReportReturn Return, CswNbtWebServiceReport.ReportData reportParams )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            DataTable ReportTable = _getReportTable( CswNbtResources, reportParams );
            if( ReportTable.Rows.Count > 0 )
            {
                // Get the Report Layout File
                Int32 JctNodePropId = reportParams.ReportNode.RPTFile.JctNodePropId;
                if( JctNodePropId > 0 )
                {
                    CswFilePath FilePathTools = new CswFilePath( CswNbtResources );
                    string ReportTempFileName = FilePathTools.getFullReportFilePath( JctNodePropId.ToString() );
                    if( !File.Exists( ReportTempFileName ) )
                    {
                        DirectoryInfo DirectoryInfo = ( new FileInfo( ReportTempFileName ) ).Directory;
                        if( DirectoryInfo != null )
                        {
                            DirectoryInfo.Create(); //creates the /rpt directory if it doesn't exist
                        }

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
                                if( BlobData != null )
                                {
                                    BWriter.Write( BlobData );
                                }
                            }
                            else
                            {
                                throw new CswDniException( CswEnumErrorType.Warning, "Report is missing RPT file", "Report's RPTFile blobdata is null" );
                            }
                        }
                    }
                    if( File.Exists( ReportTempFileName ) )
                    {
                        Return.Data.reportUrl = _saveCrystalReport( CswNbtResources, ReportTempFileName, reportParams.ReportNode.ReportName.Text, ReportTable );
                        Return.Data.hasResults = true;
                    }
                } // if( JctNodePropId > 0 )
            } // if(ReportTable.Rows.Count > 0) 
            else
            {
                Return.Data.hasResults = false;
            }

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