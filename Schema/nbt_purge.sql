------------------------
-- Expunge all user data

delete from jct_nodes_props;
delete from jct_nodes_props_audit;
delete from nodetype_props;
delete from nodetype_props_audit;
delete from node_views;
delete from nodes;
delete from nodes_audit;
delete from nodetype_tabset;
delete from nodetypes;
delete from nodetypes_audit;
commit;


drop sequence seq_fieldtypeid;
drop sequence seq_fkeydefid; 
drop sequence seq_jctnodepropid; 
drop sequence seq_nodeid; 
drop sequence seq_nodetypeid; 
drop sequence seq_nodetypepropid; 
drop sequence seq_nodetypetabsetid; 
drop sequence seq_nodeviewid; 
drop sequence seq_objectclassid; 
drop sequence seq_objectclasspropid;
commit;




----------------------------------------------------------
-- Create User and Role nodetypes and an 'admin' node so we can login

create sequence seq_nodetypeid minvalue 1 start with 1 increment by 1;
create sequence seq_nodetypepropid minvalue 1 start with 1 increment by 1;
create sequence seq_jctnodepropid minvalue 1 start with 1 increment by 1;
create sequence seq_nodetypetabsetid minvalue 1 start with 1 increment by 1;
create sequence seq_nodeid minvalue 1 start with 1 increment by 1;
create sequence seq_nodeviewid start with 1 minvalue 1 increment by 1;


insert into nodetypes (iconfilename, nametemplate, nodetypeid, nodetypename, objectclassid)
values ((select iconfilename from object_class where objectclass = 'RoleClass'), '[Name]', seq_nodetypeid.nextval, 'Role', (select objectclassid from object_class where objectclass = 'RoleClass'));
insert into nodetypes (iconfilename, nametemplate, nodetypeid, nodetypename, objectclassid)
values ((select iconfilename from object_class where objectclass = 'UserClass'), '[Last Name], [First Name]', seq_nodetypeid.nextval, 'User', (select objectclassid from object_class where objectclass = 'UserClass'));
update nodetypes set firstversionid = nodetypeid where firstversionid is null;
commit;


insert into nodetype_tabset (nodetypeid, nodetypetabsetid, tabname, taborder)
values ((select nodetypeid from nodetypes where nodetypename = 'Role'), seq_nodetypetabsetid.nextval, 'Role', 1);

insert into nodetype_tabset (nodetypeid, nodetypetabsetid, tabname, taborder)
values ((select nodetypeid from nodetypes where nodetypename = 'User'), seq_nodetypetabsetid.nextval, 'User', 1);
commit;


insert into node_views (auditflag,nodeviewid, roleid, userid, viewname,viewxml, visibility,category)
select '0', seq_nodeviewid.nextval, '','',propname,viewxml,'Property', ''
  from object_class_props
 where (objectclassid = (select objectclassid from object_class where objectclass = 'UserClass')
    or objectclassid = (select objectclassid from object_class where objectclass = 'RoleClass'))
   and viewxml is not null;
commit;

create sequence tmp_display_row minvalue 1 start with 1 increment by 1;
commit;

insert into nodetype_props (display_col, display_row, fieldtypeid, fktype, fkvalue, 
isbatchentry, isfk, isrequired, isunique, nodetypeid, nodetypepropid, nodetypetabsetid, 
objectclasspropid, propname, nodeviewid)
select '1', tmp_display_row.nextval, fieldtypeid, fktype, fkvalue,  
isbatchentry, isfk, isrequired, isunique, (select nodetypeid from nodetypes where nodetypename = 'Role'),
seq_nodetypepropid.nextval, (select nodetypetabsetid from nodetype_tabset where tabname = 'Role'), objectclasspropid, propname, 
(select nodeviewid from node_views where object_class_props.propname = node_views.viewname)
from object_class_props
where objectclassid = (select objectclassid from object_class where objectclass = 'RoleClass');
commit;

drop sequence tmp_display_row;
commit;

create sequence tmp_display_row minvalue 1 start with 1 increment by 1;
commit;

insert into nodetype_props (display_col, display_row, fieldtypeid,  fktype, fkvalue, 
isbatchentry, isfk, isrequired, isunique, nodetypeid, nodetypepropid, nodetypetabsetid, 
objectclasspropid, propname, nodeviewid)
select '1', tmp_display_row.nextval, fieldtypeid, fktype, fkvalue,  
isbatchentry, isfk, isrequired, isunique, (select nodetypeid from nodetypes where nodetypename = 'User'),
seq_nodetypepropid.nextval, (select nodetypetabsetid from nodetype_tabset where tabname = 'User'), objectclasspropid, propname, 
(select nodeviewid from node_views where object_class_props.propname = node_views.viewname)
from object_class_props
where objectclassid = (select objectclassid from object_class where objectclass = 'UserClass');
commit;

drop sequence tmp_display_row;
commit;

update nodetype_props set firstpropversionid = nodetypepropid where firstpropversionid is null;
commit;


insert into nodes (nodeid, nodename, nodetypeid)
values (seq_nodeid.nextval, 'Administrator', (select nodetypeid from nodetypes where nodetypename = 'Role'));

insert into nodes (nodeid, nodename, nodetypeid)
values (seq_nodeid.nextval, 'admin, admin', (select nodetypeid from nodetypes where nodetypename = 'User'));
commit;



insert into jct_nodes_props (field1, jctnodepropid, nodeid, nodetypepropid, gestalt)
values ('Administrator', seq_jctnodepropid.nextval, (select nodeid from nodes where nodename = 'Administrator'),
(select nodetypepropid from nodetype_props where propname = 'Name' and nodetypeid = (select nodetypeid from nodetypes where nodetypename = 'Role')), 'Administrator');

insert into jct_nodes_props (field1, jctnodepropid, nodeid, nodetypepropid, gestalt)
values ('1', seq_jctnodepropid.nextval, (select nodeid from nodes where nodename = 'Administrator'),
(select nodetypepropid from nodetype_props where propname = 'Administrator' and nodetypeid = (select nodetypeid from nodetypes where nodetypename = 'Role')), '1');

insert into jct_nodes_props (field1, jctnodepropid, nodeid, nodetypepropid, gestalt)
values ('1', seq_jctnodepropid.nextval, (select nodeid from nodes where nodename = 'Administrator'),
(select nodetypepropid from nodetype_props where propname = 'Designer' and nodetypeid = (select nodetypeid from nodetypes where nodetypename = 'Role')), '1');

insert into jct_nodes_props (field1, jctnodepropid, nodeid, nodetypepropid, gestalt)
values ('30', seq_jctnodepropid.nextval, (select nodeid from nodes where nodename = 'Administrator'),
(select nodetypepropid from nodetype_props where propname = 'Timeout' and nodetypeid = (select nodetypeid from nodetypes where nodetypename = 'Role')), '30');


insert into jct_nodes_props (field1, field2, field3, field4, field5, jctnodepropid, nodeid, nodetypepropid, gestalt)
values (
'|' || (select nodetypeid from nodetypes where nodetypename = 'Role') || '||' || (select nodetypeid from nodetypes where nodetypename = 'User') || '|',
'|' || (select nodetypeid from nodetypes where nodetypename = 'Role') || '||' || (select nodetypeid from nodetypes where nodetypename = 'User') || '|',
'|' || (select nodetypeid from nodetypes where nodetypename = 'Role') || '||' || (select nodetypeid from nodetypes where nodetypename = 'User') || '|',
'|' || (select nodetypeid from nodetypes where nodetypename = 'Role') || '||' || (select nodetypeid from nodetypes where nodetypename = 'User') || '|',
'|' || (select nodetypeid from nodetypes where nodetypename = 'Role') || '||' || (select nodetypeid from nodetypes where nodetypename = 'User') || '|',
seq_jctnodepropid.nextval, 
(select nodeid from nodes where nodename = 'Administrator'),
(select nodetypepropid from nodetype_props where propname = 'Permissions' and nodetypeid = (select nodetypeid from nodetypes where nodetypename = 'Role')), 
'');

commit;


insert into jct_nodes_props (field1, jctnodepropid, nodeid, nodetypepropid, gestalt)
values ('admin', seq_jctnodepropid.nextval, (select nodeid from nodes where nodename = 'admin, admin'),
(select nodetypepropid from nodetype_props where propname = 'Last Name' and nodetypeid = (select nodetypeid from nodetypes where nodetypename = 'User')), 'admin');

insert into jct_nodes_props (field1, jctnodepropid, nodeid, nodetypepropid, gestalt)
values ('admin', seq_jctnodepropid.nextval, (select nodeid from nodes where nodename = 'admin, admin'),
(select nodetypepropid from nodetype_props where propname = 'First Name' and nodetypeid = (select nodetypeid from nodetypes where nodetypename = 'User')), 'admin');

insert into jct_nodes_props (field1, jctnodepropid, nodeid, nodetypepropid, gestalt)
values ('admin', seq_jctnodepropid.nextval, (select nodeid from nodes where nodename = 'admin, admin'),
(select nodetypepropid from nodetype_props where propname = 'Username' and nodetypeid = (select nodetypeid from nodetypes where nodetypename = 'User')), 'admin');

insert into jct_nodes_props (field1, jctnodepropid, nodeid, nodetypepropid, gestalt)
values ('21232f297a57a5a743894a0e4a801fc3', seq_jctnodepropid.nextval, (select nodeid from nodes where nodename = 'admin, admin'),
(select nodetypepropid from nodetype_props where propname = 'Password' and nodetypeid = (select nodetypeid from nodetypes where nodetypename = 'User')), '21232f297a57a5a743894a0e4a801fc3');

insert into jct_nodes_props (field1, field1_fk, jctnodepropid, nodeid, nodetypepropid, gestalt)
values ('Administrator', (select nodeid from nodes where nodename = 'Administrator'), seq_jctnodepropid.nextval, 
(select nodeid from nodes where nodename = 'admin, admin'),
(select nodetypepropid from nodetype_props where propname = 'Role' and nodetypeid = (select nodetypeid from nodetypes where nodetypename = 'User')), (select nodeid from nodes where nodename = 'Administrator'));

commit;
