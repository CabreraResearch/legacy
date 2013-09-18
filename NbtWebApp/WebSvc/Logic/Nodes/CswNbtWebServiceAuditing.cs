using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Grid.ExtJs;
using ChemSW.Nbt.Grid;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Security;
using Newtonsoft.Json.Linq;
using System.Data;
namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceAuditing
    {
        private readonly CswNbtResources _CswNbtResources;
        public CswNbtWebServiceAuditing( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public JObject getAuditHistoryGrid( CswNbtNode Node, bool JustDateColumn )
        {
            JObject ret = new JObject();
            if( Node != null )
            {
                string SQL = @"select ja.recordcreated as ChangeDate ";
                if( !JustDateColumn )
                {
                    SQL += @", x.transactionusername as Username,
                               x.auditeventname as Context,
                               np.propname as Propname,
                               ja.gestaltsearch as Value,
                               x.audittransactionid as AuditId,
                               ft.fieldtype as FieldType ";
                }
                SQL += @" from jct_nodes_props_audit ja
                          join audit_transactions x on ja.audittransactionid = x.audittransactionid
                          join TABLE(" + CswNbtAuditTableAbbreviation.getAuditLookupFunctionNameForRealTable( "nodetype_props" ) +
                                    @"(ja.recordcreated)) np on (np.nodetypepropid = ja.nodetypepropid)
                          join field_types ft on ft.fieldtypeid = np.fieldtypeid
                         where ja.nodeid = :nodeid
                           and x.transactionusername not in (:sysusernames)
                          order by AuditId desc";

                CswCommaDelimitedString sysUserNames = new CswCommaDelimitedString( 0, "'" );
                foreach( CswEnumSystemUserNames sysUserName in CswEnumSystemUserNames.getValues() )
                {
                    sysUserNames.Add( sysUserName.ToString() );
                }

                CswArbitrarySelect HistorySelect = _CswNbtResources.makeCswArbitrarySelect( "CswNbtWebServiceAuditing_getAuditHistory_select", SQL );
                HistorySelect.addParameter( "nodeid", Node.NodeId.PrimaryKey.ToString() );
                HistorySelect.addParameter( "sysusernames", sysUserNames.ToString() );

                DataTable HistoryTable = HistorySelect.getTable();

                //for the audit grid we want to group by audittransactionid, but show the changedate
                //also, we mask the password value column and not show the encrypted password
                string mutatingRowsAuditId = "";
                string changeToDate = "";
                foreach( DataRow row in HistoryTable.Rows )
                {
                    string currentAuditId = row["AuditId"].ToString();
                    if( currentAuditId != mutatingRowsAuditId ) //we're onto a new group
                    {
                        mutatingRowsAuditId = currentAuditId;
                        changeToDate = row["ChangeDate"].ToString();
                    }
                    row["ChangeDate"] = changeToDate;

                    if( row["FieldType"].ToString().Equals( "Password" ) )
                    {
                        row["Value"] = "[password changed]";
                    }
                }
                HistoryTable.Columns.Remove( "FieldType" );
                HistoryTable.Columns.Remove( "AuditId" );

                CswNbtGrid g = new CswNbtGrid( _CswNbtResources );
                ret = g.DataTableToJSON( HistoryTable, GroupByCol: "ChangeDate", GroupByColType: CswEnumExtJsXType.datecolumn );
            }
            return ret;

        } // _getAuditHistoryGrid()

    } // class CswNbtWebServiceAuditing

} // namespace ChemSW.Nbt.WebServices
