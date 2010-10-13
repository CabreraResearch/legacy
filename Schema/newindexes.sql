drop index nodetype1;
create index nodetype1 on nodetypes(firstversionid,nodetypeid);
analyze table nodetypes estimate statistics sample 10 percent;
commit;

drop index materialspk;
create index materialpk on materials(materialid);
drop index nbtmaterials1;
create index nbtmaterials1 on materials(materialid,lower(nodename),nodetypeid,deleted);
drop index nbtmaterials2;
create index nbtmaterials2 on materials(materialid,deleted);
drop index nbtmaterials3;
create index nbtmaterials3 on materials(lower(nodename),nodetypeid,deleted);
analyze table materials estimate statistics sample 10 percent;
commit;

drop index packagespk;
create index packagespk on packages(packageid);
drop index nbtpackages1;
create index nbtpackages1 on packages(packageid,lower(nodename),nodetypeid,deleted);
drop index nbtpackages2;
create index nbtpackages2 on packages(materialid,lower(nodename),packageid,nodetypeid,deleted);
analyze table packages estimate statistics sample 10 percent;
commit;

drop index containerspk;
create index containerspk on containers(containerid);
drop index nbtcontainers1;
create index nbtcontainers1 on containers(containerid,lower(nodename),nodetypeid,deleted);
drop index nbtcontainers2;
create index nbtcontainers2 on containers(packdetailid,containerid,lower(nodename),nodetypeid,deleted);
analyze table containers estimate statistics sample 10 percent;
commit;

drop index nodetypeprops1;
create index nodetypeprops1 on nodetype_props(nodetypepropid, nodetypeid, lower(propname));
analyze table nodetype_props estimate statistics sample 20 percent;
commit;

drop index nodeview1;
create index nodeview1 on node_views (lower(viewname), roleid, userid, visibility);
analyze table node_views estimate statistics sample 20 percent;
commit;
