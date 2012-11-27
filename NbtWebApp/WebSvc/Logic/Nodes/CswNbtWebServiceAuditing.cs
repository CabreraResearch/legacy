using System.Data;
using System;
using ChemSW.DB;
using ChemSW.Nbt.Grid;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.Security;
using ChemSW.Core;

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
                string SQL = @"select ja.recordcreated as ChangeDate";
                if( !JustDateColumn )
                {
                    SQL += @", 
                                  x.transactionusername as Username, 
                                  x.auditeventname as Context,
                                  np.propname as Propname,
                                  ja.field1 as Value,
                                  x.auditeventname as Type,
                                  x.audittransactionid as AuditId";
                }
                SQL += @"		from jct_nodes_props_audit ja

                                join audit_transactions x on ja.audittransactionid = x.audittransactionid
                                join nodetype_props np on ja.nodetypepropid = np.nodetypepropid
                                join field_types ft on ft.fieldtypeid = np.fieldtypeid
                            where ja.nodeid = " + Node.NodeId.PrimaryKey.ToString();

                CswCommaDelimitedString sysUserNames = new CswCommaDelimitedString();
                foreach( SystemUserNames sysUserName in Enum.GetValues( typeof( SystemUserNames ) ) )
                {
                    sysUserNames.Add( "'" + sysUserName.ToString() + "'" );
                }
                SQL += @" and x.transactionusername not in (" + sysUserNames + ") order by ChangeDate desc";

                CswArbitrarySelect HistorySelect = _CswNbtResources.makeCswArbitrarySelect( "CswNbtWebServiceAuditing_getAuditHistory_select", SQL );
                DataTable HistoryTable = HistorySelect.getTable();

                CswNbtGrid g = new CswNbtGrid( _CswNbtResources );
                ret = g.DataTableToJSON( HistoryTable );
            }
            return ret;

        } // _getAuditHistoryGrid()

    } // class CswNbtWebServiceAuditing

} // namespace ChemSW.Nbt.WebServices
