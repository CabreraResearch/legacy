

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-16
    /// </summary>
    public class CswUpdateSchemaTo01L16 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 16 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 24769

            string GenNodeS4 = @"select n.nodeid,n.nodename
                                                          from nodes n
                                                          join nodetypes t on n.nodetypeid = t.nodetypeid
                                                          join object_class o on t.objectclassid = o.objectclassid

                                                          join (select op.objectclassid,
                                                                       p.nodetypeid,
                                                                       j.nodeid,
                                                                       p.propname,
                                                                       j.field1 enabled
                                                                  from object_class_props op
                                                                  join nodetype_props p on op.objectclasspropid =
                                                                                           p.objectclasspropid
                                                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                                                 where op.propname = 'Enabled') e on (e.objectclassid =
                                                                                                     o.objectclassid and
                                                                                                     e.nodeid = n.nodeid and
                                                                                                     e.nodetypeid = t.nodetypeid)

                                                          left outer join (select op.objectclassid,
                                                                                  p.nodetypeid,
                                                                                  j.nodeid,
                                                                                  p.propname,
                                                                                  j.field1_date finalduedate
                                                                             from object_class_props op
                                                                             join nodetype_props p on op.objectclasspropid =
                                                                                                      p.objectclasspropid
                                                                             join jct_nodes_props j on j.nodetypepropid =
                                                                                                       p.nodetypepropid
                                                                            where op.propname = 'Final Due Date') fdd on (fdd.objectclassid =
                                                                                                                         o.objectclassid and
                                                                                                                         fdd.nodeid =
                                                                                                                         n.nodeid and
                                                                                                                         fdd.nodetypeid =
                                                                                                                         t.nodetypeid)

                                                          join (select op.objectclassid,
                                                                       p.nodetypeid,
                                                                       j.nodeid,
                                                                       p.propname,
                                                                       j.field1_date nextduedate
                                                                  from object_class_props op
                                                                  join nodetype_props p on op.objectclasspropid =
                                                                                           p.objectclasspropid
                                                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                                                 where op.propname = 'Next Due Date') ndd on (ndd.objectclassid =
                                                                                                             o.objectclassid and
                                                                                                             ndd.nodeid = n.nodeid and
                                                                                                             ndd.nodetypeid =
                                                                                                             t.nodetypeid)

                                                          join (select op.objectclassid,
                                                                       p.nodetypeid,
                                                                       j.nodeid,
                                                                       p.propname,
                                                                       j.field1_numeric warningdays
                                                                  from object_class_props op
                                                                  join nodetype_props p on op.objectclasspropid =
                                                                                           p.objectclasspropid
                                                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                                                 where op.propname = 'Warning Days') wd on (wd.objectclassid =
                                                                                                           o.objectclassid and
                                                                                                           wd.nodeid = n.nodeid and
                                                                                                           wd.nodetypeid =
                                                                                                           t.nodetypeid)

                                                  

                                                         where ((o.objectclass = 'GeneratorClass' and e.enabled = '1' and
                                                               sysdate >= (ndd.nextduedate - wd.warningdays) and
                                                               (fdd.finalduedate is null or sysdate <= fdd.finalduedate)))";

            _CswNbtSchemaModTrnsctn.UpdateS4( "GenNode", GenNodeS4 );

            #endregion Case 24769


        }//Update()

    }//class CswUpdateSchemaTo01L16

}//namespace ChemSW.Nbt.Schema


