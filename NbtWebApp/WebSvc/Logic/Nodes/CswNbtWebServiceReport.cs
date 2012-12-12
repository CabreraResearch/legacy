using System.Data;
using System.Web;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Grid;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;
using NbtWebApp.WebSvc.Returns;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;
using System.Collections.Generic;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceReport
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtNode _reportNode = null;

        private HttpContext _Context = HttpContext.Current;

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
            public Collection<ReportParam> reportParams;

            [DataContract]
            public class ReportParam
            {
                [DataMember]
                public string name = string.Empty;
                [DataMember]
                public string value = string.Empty;
            }

            private Dictionary<string, string> _reportParamsDictionary = null;
            public Dictionary<string, string> ReportParamDictionary{
                get{ 
                    if( null == _reportParamsDictionary){
                        _reportParamsDictionary = new Dictionary<string,string>();
                        foreach(ReportParam param in reportParams){
                            _reportParamsDictionary.Add(param.name, param.value);
                        }
                    }
                    return _reportParamsDictionary; 
                }
            }
            
        }

        #endregion WCF Data Objects

        public CswNbtWebServiceReport( CswNbtResources CswNbtResources, CswNbtNode reportNode )
        {
            _CswNbtResources = CswNbtResources;
            _reportNode = reportNode;
        }

        public CswNbtWebServiceReport( CswNbtResources CswNbtResources, CswNbtNode reportNode, HttpContext Context )
        {
            _CswNbtResources = CswNbtResources;
            _reportNode = reportNode;
            _Context = Context;
        }

        public JObject runReport( string rformat, HttpContext Context )
        {
            CswNbtObjClassReport report = (CswNbtObjClassReport) _reportNode;
            JObject ret = new JObject();

            if( string.Empty != report.SQL.Text )
            {

                string ReportSql = report.getUserContextSql( _CswNbtResources.CurrentNbtUser.Username );
                CswArbitrarySelect cswRptSql = _CswNbtResources.makeCswArbitrarySelect( "report_sql", ReportSql );
                DataTable rptDataTbl = cswRptSql.getTable();
                if( string.IsNullOrEmpty( rptDataTbl.TableName ) && null != _reportNode )
                {
                    CswNbtObjClassReport nodeAsReport = _reportNode;
                    rptDataTbl.TableName = nodeAsReport.ReportName.Text;
                }

                if( "csv" == rformat.ToLower() )
                {
                    wsTools.ReturnCSV( Context, rptDataTbl );
                }
                else
                {
                    CswNbtGrid cg = new CswNbtGrid( _CswNbtResources );
                    ret = cg.DataTableToJSON( rptDataTbl );  //rformat!=csv
                }
            }
            else
            {
                throw ( new CswDniException( "Report has no SQL to run!" ) );
            }
            return ret;
        } // runReport()

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
                    reportNode.SQL.Text = ReplaceReportParams( reportParams.reportParams, reportNode );
                    string ReportSql = reportNode.getUserContextSql( NbtResources.CurrentNbtUser.Username );
                    CswArbitrarySelect cswRptSql = NbtResources.makeCswArbitrarySelect( "report_sql", ReportSql );
                    rptDataTbl = cswRptSql.getTable();
                    if( string.IsNullOrEmpty( rptDataTbl.TableName ) && null != reportNode )
                    {
                        rptDataTbl.TableName = reportNode.ReportName.Text;
                    }
                }
                else
                {
                    throw ( new CswDniException( "Report has no SQL to run!" ) );
                }
            }
            return rptDataTbl;
        }

        public static string ReplaceReportParams( Collection<CswNbtWebServiceReport.ReportData.ReportParam> reportParams, CswNbtObjClassReport reportNode) //{param1}=someval,{param2}=anotherval
        {
            string replacedSQL = reportNode.SQL.Text;
            foreach( CswNbtWebServiceReport.ReportData.ReportParam param in reportParams )
            {
                replacedSQL = replacedSQL.Replace( param.name, "'" + param.value + "'" );
            }
            return replacedSQL;
        }

    } // class CswNbtWebServiceReport
} // namespace ChemSW.Nbt.WebServices