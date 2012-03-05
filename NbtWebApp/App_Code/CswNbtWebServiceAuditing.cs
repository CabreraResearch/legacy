﻿using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.Logic;

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
				string SQL = @"select na.recordcreated as ChangeDate ";
				if( !JustDateColumn )
				{
					SQL += @", 
								  x.transactionusername as Username, ";
								  // na.auditeventtype as EventType,     case 22590
					SQL += @"     x.auditeventname as Context";
				}
				SQL += @"		 from nodes n
							 join nodes_audit na on n.nodeid = na.nodeid
							 join audit_transactions x on na.audittransactionid = x.audittransactionid
							where n.nodeid = " + Node.NodeId.PrimaryKey.ToString() + @"
						  UNION
						   select ja.recordcreated as ChangeDate";
				if( !JustDateColumn )
				{
					SQL += @", 
								  x.transactionusername as Username, ";
								  // ja.auditeventtype as EventType,     case 22590
					SQL += @"     x.auditeventname as Context";
				}
				SQL += @"		 from nodes n
							 join jct_nodes_props j on n.nodeid = j.nodeid
							 join jct_nodes_props_audit ja on j.jctnodepropid = ja.jctnodepropid
							 join audit_transactions x on ja.audittransactionid = x.audittransactionid
							where n.nodeid = " + Node.NodeId.PrimaryKey.ToString() + @" 
							order by ChangeDate desc";

				CswArbitrarySelect HistorySelect = _CswNbtResources.makeCswArbitrarySelect( "CswNbtWebServiceAuditing_getAuditHistory_select", SQL );
				DataTable HistoryTable = HistorySelect.getTable();

				CswGridData g = new CswGridData( _CswNbtResources );
				ret["jqGridOpt"] = g.DataTableToJSON( HistoryTable );
			}
			return ret;

		} // _getAuditHistoryGrid()

	} // class CswNbtWebServiceAuditing

} // namespace ChemSW.Nbt.WebServices
