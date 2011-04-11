using System;
using System.Data;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using System.IO;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01H-26
	/// </summary>
	public class CswUpdateSchemaTo01H26 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 26 ); } }
		public CswUpdateSchemaTo01H26( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
		}

		public void update()
		{
			// case 21364
			// fill new viewmode column on node_views
			
			CswTableUpdate ViewsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H26_update_views", "node_views" );
			DataTable ViewsTable = ViewsUpdate.getTable();
			foreach( DataRow ViewsRow in ViewsTable.Rows )
			{
				CswNbtView ThisView = _CswNbtSchemaModTrnsctn.restoreView( CswConvert.ToInt32( ViewsRow["nodeviewid"] ) );
				ViewsRow["viewmode"] = ThisView.ViewMode.ToString();
			}
			ViewsUpdate.update( ViewsTable );
			
			// update S4s to include new viewmode column
			_CswNbtSchemaModTrnsctn.UpdateS4( "getViewInfo", @"select v.nodeviewid, v.viewname, v.viewmode, v.visibility, v.roleid, v.userid, r.nodename rolename, u.nodename username
from node_views v
left outer join nodes r on v.roleid = r.nodeid
left outer join nodes u on v.userid = u.nodeid
where nodeviewid = :getviewid
order by lower(v.viewname)" );

			_CswNbtSchemaModTrnsctn.UpdateS4( "getAllViewInfo", @"select v.nodeviewid, v.viewname, v.viewmode, v.visibility, v.roleid,
v.userid, v.category, r.nodename rolename, u.nodename username
from node_views v
left outer join nodes r on v.roleid = r.nodeid
left outer join nodes u on v.userid = u.nodeid
where visibility != 'Property'
order by lower(v.viewname)" );

			_CswNbtSchemaModTrnsctn.UpdateS4( "getVisibleViewInfo", @"select v.nodeviewid, v.viewname, v.viewmode, v.visibility, v.roleid, v.userid,
v.category, r.nodename rolename, u.nodename username, v.viewxml
,lower(NVL(v.category, v.viewname)) mssqlorder
from node_views v
left outer join nodes r on v.roleid = r.nodeid
left outer join nodes u on v.userid = u.nodeid
where ((visibility = 'Global') or
       (visibility = 'Role' and roleid = :getroleid) or
       (visibility = 'User' and userid = :getuserid))
       #addclause
order by #orderbyclause" );

			_CswNbtSchemaModTrnsctn.UpdateS4( "getUserViewInfo", @"select v.nodeviewid, v.viewname, v.viewmode, v.visibility, v.roleid, v.userid, v.category, r.nodename rolename, u.nodename username
from node_views v
left outer join nodes r on v.roleid = r.nodeid
left outer join nodes u on v.userid = u.nodeid
where (visibility = 'User' and userid = :getuserid)
order by lower(NVL(v.category, v.viewname)), lower(v.viewname)" );

		} // update()

	}//class CswUpdateSchemaTo01H26

}//namespace ChemSW.Nbt.Schema

