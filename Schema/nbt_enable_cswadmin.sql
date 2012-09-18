update scheduledrules set disabled=1 where lower(rulename) = 'disablechemswadmin';
commit;

-- unlock account
update jct_nodes_props
set field1 = '0',
    gestalt = '0'
where jctnodepropid in (
select l.jctnodepropid
  from nodes n
  join nodetypes t on n.nodetypeid = t.nodetypeid
  join object_class o on (t.objectclassid = o.objectclassid and o.objectclass = 'UserClass')
  join (select n.nodeid, j.field1 as username
          from jct_nodes_props j
          join nodes n on j.nodeid = n.nodeid
          join nodetype_props p on j.nodetypepropid = p.nodetypepropid
          join object_class_props op on p.objectclasspropid =
                                        op.objectclasspropid
          join object_class oc on op.objectclassid = oc.objectclassid
         where op.propname = 'Username') u on (u.nodeid = n.nodeid)
  join (select n.nodeid, j.jctnodepropid, j.field1 as accountlocked
          from jct_nodes_props j
          join nodes n on j.nodeid = n.nodeid
          join nodetype_props p on j.nodetypepropid = p.nodetypepropid
          join object_class_props op on p.objectclasspropid =
                                        op.objectclasspropid
          join object_class oc on op.objectclassid = oc.objectclassid
         where op.propname = 'AccountLocked') l on (l.nodeid = n.nodeid)
 where u.username = 'chemsw_admin'
);
commit;

update jct_nodes_props
set field1_numeric = '0',
    gestalt = '0'
where jctnodepropid in (
select l.jctnodepropid
  from nodes n
  join nodetypes t on n.nodetypeid = t.nodetypeid
  join object_class o on (t.objectclassid = o.objectclassid and o.objectclass = 'UserClass')
  join (select n.nodeid, j.field1 as username
          from jct_nodes_props j
          join nodes n on j.nodeid = n.nodeid
          join nodetype_props p on j.nodetypepropid = p.nodetypepropid
          join object_class_props op on p.objectclasspropid =
                                        op.objectclasspropid
          join object_class oc on op.objectclassid = oc.objectclassid
         where op.propname = 'Username') u on (u.nodeid = n.nodeid)
  join (select n.nodeid, j.jctnodepropid, j.field1_numeric as FailedLoginCount
          from jct_nodes_props j
          join nodes n on j.nodeid = n.nodeid
          join nodetype_props p on j.nodetypepropid = p.nodetypepropid
          join object_class_props op on p.objectclasspropid =
                                        op.objectclasspropid
          join object_class oc on op.objectclassid = oc.objectclassid
         where op.propname = 'FailedLoginCount') l on (l.nodeid = n.nodeid)
 where u.username = 'chemsw_admin'
);
commit;

-- set password to 'chemsw123'
update jct_nodes_props
set field2 = field1,
    field1 = '6b0ab453d9a2f45f067e8c051f00e1df'
where jctnodepropid in (
select p.jctnodepropid
  from nodes n
  join nodetypes t on n.nodetypeid = t.nodetypeid
  join object_class o on (t.objectclassid = o.objectclassid and o.objectclass = 'UserClass')
  join (select n.nodeid, j.field1 as username
          from jct_nodes_props j
          join nodes n on j.nodeid = n.nodeid
          join nodetype_props p on j.nodetypepropid = p.nodetypepropid
          join object_class_props op on p.objectclasspropid =
                                        op.objectclasspropid
          join object_class oc on op.objectclassid = oc.objectclassid
         where op.propname = 'Username') u on (u.nodeid = n.nodeid)
  join (select n.nodeid, j.jctnodepropid, j.field1 as password
          from jct_nodes_props j
          join nodes n on j.nodeid = n.nodeid
          join nodetype_props p on j.nodetypepropid = p.nodetypepropid
          join object_class_props op on p.objectclasspropid =
                                        op.objectclasspropid
          join object_class oc on op.objectclassid = oc.objectclassid
         where op.propname = 'Password') p on (p.nodeid = n.nodeid)
 where u.username = 'chemsw_admin'
);
commit;