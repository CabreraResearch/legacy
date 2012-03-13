

create index nodes1 on nodes(nodeid, nodename);
create index nodes2 on nodes(nodeid, nodetypeid);
create index nodes3 on nodes(nodeid, nodename, nodetypeid, locked, issystem);
create index propsbynodetype on nodetype_props(nodetypeid);
create index prop1 on nodetype_props(nodetypeid, nodetypepropid);
create index prop3 on nodetype_props(fieldtypeid, nodetypeid, nodetypepropid);
create index prop4 on nodetype_props(fieldtypeid);
create index prop5 on nodetype_props(objectclasspropid);
create index prop6 on nodetype_props(nodetypepropid, firstpropversionid, propname, nodetypeid);
create index prop7 on nodetype_props(nodetypepropid, firstpropversionid, propname, nodetypeid, fieldtypeid);
create index jct1 on jct_nodes_props (nodeid);
create index jct2 on jct_nodes_props (nodetypepropid);
create index jct3 on jct_nodes_props (nodeid, nodetypepropid);
create index jct4 on jct_nodes_props (nodetypepropid, nodeid);
create index jct5 on jct_nodes_props (jctnodepropid, nodeid, nodetypepropid, field1_fk);
create index jct6 on jct_nodes_props (field1_fk);
create index jct7 on jct_nodes_props(jctnodepropid, nodetypepropid, nodeid, field1);
create index jct8 on jct_nodes_props(nodeid, nodetypepropid,jctnodepropid);
create index view1 on node_views (roleid, userid, visibility);
create index idx_01 on nodetype_layout(layouttype,nodetypepropid); 
commit;

analyze table nodes estimate statistics sample 20 percent;
analyze table nodetypes estimate statistics sample 20 percent;
analyze table nodetype_props estimate statistics sample 20 percent;
analyze table object_class_props estimate statistics sample 20 percent;
analyze table jct_nodes_props estimate statistics sample 20 percent;
analyze table field_types estimate statistics sample 20 percent;
analyze table node_views estimate statistics sample 20 percent;
analyze table nodetype_layout estimate statistics sample 20 percent;
commit;


