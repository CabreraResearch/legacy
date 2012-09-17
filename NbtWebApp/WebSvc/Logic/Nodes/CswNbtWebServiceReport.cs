using System.Data;
using System.Web;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Grid;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceReport
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtNode _reportNode = null;

        public CswNbtWebServiceReport( CswNbtResources CswNbtResources, CswNbtNode reportNode )
        {
            _CswNbtResources = CswNbtResources;
            _reportNode = reportNode;
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

    } // class CswNbtWebServiceReport
} // namespace ChemSW.Nbt.WebServices