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
            CswNbtObjClassReport report = CswNbtNodeCaster.AsReport(_reportNode);
            JObject ret = new JObject();

            //if we are not RPT, then just run the SQL
            if( report.RPTFile.Empty )
            {
                if( string.Empty != report.SQL.Text )
                {
                    
                    CswArbitrarySelect cswRptSql = _CswNbtResources.makeCswArbitrarySelect( "report_sql", report.SQL.Text );
                    DataTable rptDataTbl = cswRptSql.getTable();
                    
                    if( "csv" == rformat.ToLower() )
                    {
                        Context.Response.ClearContent();                        
                        //Context.Response.ContentType = "application/vnd.ms-excel";
                        bool headers = false;
                        //DataColumn rows
                        foreach( DataRow dr in rptDataTbl.Rows )
                        {
                            int idx = 0;
                            if( false == headers )
                            {
                                foreach(DataColumn dc in rptDataTbl.Columns )
                                {
                                    if( idx > 0 ) Context.Response.Write( "," );
                                    Context.Response.Write( "\"" + dc.ColumnName.ToString() + "\"" );
                                    idx++;
                                }
                                Context.Response.Write( "\r\n" );
                                idx = 0;
                                headers = true;
                            }
                            foreach( DataColumn dc in rptDataTbl.Columns )
                            {
                                if( idx > 0 ) Context.Response.Write( "," );
                                Context.Response.Write( "\"" + dr[dc].ToString() + "\"" );
                                idx++;
                            }
                            Context.Response.Write( "\r\n" );
                        }

                        Context.Response.AddHeader( "Content-Disposition", "attachment; filename=export.csv;" );
                        Context.Response.End();
                    }
                    else
                    {
                        CswGridData cg = new CswGridData( _CswNbtResources );
                        ret["griddata"] = cg.DataTableToJSON( rptDataTbl );  //rformat!=csv
                    }
                }
                else throw ( new CswDniException( "Report has no SQL to run!" ) );
            }
            else
            {
                //we are CRPE, run as such...
                throw ( new CswDniException( "CRPE report not implemented yet." ) );
            }
            return ret;

        }


    } // class CswNbtWebServiceTable
} // namespace ChemSW.Nbt.WebServices