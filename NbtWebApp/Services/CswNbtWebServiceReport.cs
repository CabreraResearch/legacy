using System;
using System.Data;
using System.Web;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.Logic;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceReport
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtNode _reportNode = null;

        public CswNbtWebServiceReport( CswNbtResources CswNbtResources,CswNbtNode reportNode)
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

                CswArbitrarySelect cswRptSql = _CswNbtResources.makeCswArbitrarySelect( "report_sql", report.SQL.Text );
                DataTable rptDataTbl = cswRptSql.getTable();

                if( "csv" == rformat.ToLower() )
                {
                    wsTools.ReturnCSV( Context, rptDataTbl );
                }
                else
                {
                    CswNbtActGrid cg = new CswNbtActGrid( _CswNbtResources );
                    ret["griddata"] = cg.DataTableToJSON( rptDataTbl );  //rformat!=csv
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