using ChemSW.Nbt.Sched;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29707
    /// </summary>
    public class CswUpdateSchema_02B_Case29707 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 29707; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.UpdateS4( CswEnumNbtScheduleRuleNames.UpdtInspection, @"select n.nodeid
                                                          from nodes n
                                                          join nodetypes t on n.nodetypeid = t.nodetypeid
                                                          join object_class o on t.objectclassid = o.objectclassid

                                                          join (select op.objectclassid,
                                                                       p.nodetypeid,
                                                                       j.nodeid,
                                                                       p.propname,
                                                                       j.field1_date duedate,
                                                                       j.field1_numeric warningdays
                                                                  from object_class_props op
                                                                  join nodetype_props p on op.objectclasspropid =
                                                                                           p.objectclasspropid
                                                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                                                 where op.propname like 'Due Date') dd on (dd.objectclassid =
                                                                                                          o.objectclassid and
                                                                                                          dd.nodeid = n.nodeid and
                                                                                                          dd.nodetypeid =
                                                                                                          t.nodetypeid)

                                                          join (select op.objectclassid,
                                                                       p.nodetypeid,
                                                                       j.nodeid,
                                                                       p.propname,
                                                                       j.field1 status
                                                                  from object_class_props op
                                                                  join nodetype_props p on op.objectclasspropid =
                                                                                           p.objectclasspropid
                                                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                                                 where op.propname like 'Status') s on (s.objectclassid =
                                                                                                       o.objectclassid and
                                                                                                       s.nodeid = n.nodeid and
                                                                                                       s.nodetypeid = t.nodetypeid)

                                                         where o.objectclass = 'InspectionDesignClass'
                                                           and trunc(sysdate) > (dd.duedate)
                                                           and s.status = 'Pending'" );
        } // update()

    }//class CswUpdateSchema_02B_Case29707

}//namespace ChemSW.Nbt.Schema