using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29367
    /// </summary>
    public class CswUpdateSchema_02A_Case29367 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29367; }
        }

        public override void update()
        {
            string NewUserViewInfoSQL = @"select v.nodeviewid, v.viewname, v.viewmode, v.visibility, v.roleid, v.userid, v.category, r.nodename rolename, u.nodename username, v.issystem
from node_views v
left outer join nodes r on v.roleid = r.nodeid
left outer join nodes u on v.userid = u.nodeid
where (visibility = 'User' and userid = :getuserid)
order by lower(NVL(v.category, v.viewname)), lower(v.viewname)";

            _CswNbtSchemaModTrnsctn.UpdateS4( "getUserViewInfo", NewUserViewInfoSQL );

            string NewGetViewInfoSQL = @"select v.nodeviewid, v.viewname, v.viewmode, v.visibility, v.roleid, v.userid, r.nodename rolename, u.nodename username, v.issystem
from node_views v
left outer join nodes r on v.roleid = r.nodeid
left outer join nodes u on v.userid = u.nodeid
where nodeviewid = :getviewid
order by lower(v.viewname)";
            _CswNbtSchemaModTrnsctn.UpdateS4( "getViewInfo", NewGetViewInfoSQL );
        } // update()

    }//class CswUpdateSchema_02A_Case29367
}//namespace ChemSW.Nbt.Schema