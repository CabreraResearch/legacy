using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28890
    /// </summary>
    public class CswUpdateSchema_02A_Case28890 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 28890; }
        }

        public override void update()
        {
            string NewGetViewInfoSQL = @"select v.nodeviewid, v.viewname, v.viewmode, v.visibility, v.roleid,
v.userid, v.category, r.nodename rolename, u.nodename username, v.issystem
from node_views v
left outer join nodes r on v.roleid = r.nodeid
left outer join nodes u on v.userid = u.nodeid
where visibility != 'Property'
order by lower(v.viewname)";

            _CswNbtSchemaModTrnsctn.UpdateS4( "getAllViewInfo", NewGetViewInfoSQL );

            string NewGetVisibleViewInfoSQL = @"select v.nodeviewid, v.viewname, v.viewmode, v.visibility, v.roleid, v.userid,
v.category, r.nodename rolename, u.nodename username, v.viewxml, v.issystem
,lower(NVL(v.category, v.viewname)) mssqlorder
from node_views v
left outer join nodes r on v.roleid = r.nodeid
left outer join nodes u on v.userid = u.nodeid
where ((visibility = 'Global') or
       (visibility = 'Role' and roleid = :getroleid) or
       (visibility = 'User' and userid = :getuserid))
       #addclause
order by #orderbyclause";
            _CswNbtSchemaModTrnsctn.UpdateS4( "getVisibleViewInfo", NewGetVisibleViewInfoSQL );

            CswNbtView LocationsView = _CswNbtSchemaModTrnsctn.restoreView( "Locations", CswEnumNbtViewVisibility.Global );
            if( null != LocationsView && LocationsView.Category == "System" )
            {
                LocationsView.IsSystem = true;
                LocationsView.save();
            }
        } // update()

    }//class CswUpdateSchema_02A_Case28890
}//namespace ChemSW.Nbt.Schema