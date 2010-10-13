
drop table materials;
drop table containers;
drop table inventory_groups;
drop table locations;
drop table materials_subclass;
drop table materials_synonyms;
drop table packages;
drop table packdetail;
drop table units_of_measure;
drop table users;
drop table vendors;
commit;

create table materials         as select * from materials@cafuserlink;
create table containers        as select * from containers@cafuserlink;
create table inventory_groups  as select * from inventory_groups@cafuserlink;
create table locations         as select * from locations@cafuserlink;
create table materials_subclass as select * from materials_subclass@cafuserlink;
create table materials_synonyms as select * from materials_synonyms@cafuserlink;
create table packages          as select * from packages@cafuserlink;
create table packdetail        as select * from packdetail@cafuserlink;
create table units_of_measure  as select * from units_of_measure@cafuserlink;
create table users             as select * from users@cafuserlink;
create table vendors           as select * from vendors@cafuserlink;
commit;

alter table materials       add nodetypeid number(12);
alter table containers        add nodetypeid number(12);
alter table inventory_groups  add nodetypeid number(12);
alter table locations         add nodetypeid number(12);
alter table materials_subclass add nodetypeid number(12);
alter table materials_synonyms add nodetypeid number(12);
alter table packages          add nodetypeid number(12);
alter table packdetail        add nodetypeid number(12);
alter table units_of_measure  add nodetypeid number(12);
alter table users             add nodetypeid number(12);
alter table vendors           add nodetypeid number(12);

alter table materials          add nodename varchar2(254);
alter table containers         add nodename varchar2(100);
alter table inventory_groups   add nodename varchar2(100);
alter table locations          add nodename varchar2(420);
alter table materials_subclass add nodename varchar2(100);
alter table materials_synonyms add nodename varchar2(254);
alter table packages           add nodename varchar2(100);
alter table packdetail         add nodename varchar2(100);
alter table units_of_measure   add nodename varchar2(100);
alter table users              add nodename varchar2(100);
alter table vendors            add nodename varchar2(100);
commit;

alter table materials          add pendingupdate char(1);
alter table containers         add pendingupdate char(1);
alter table inventory_groups   add pendingupdate char(1);
alter table locations          add pendingupdate char(1);
alter table materials_subclass add pendingupdate char(1);
alter table materials_synonyms add pendingupdate char(1);
alter table packages           add pendingupdate char(1);
alter table packdetail         add pendingupdate char(1);
alter table units_of_measure   add pendingupdate char(1);
alter table users              add pendingupdate char(1);
alter table vendors            add pendingupdate char(1);
commit;



update materials        set nodetypeid = '679', nodename = materialname;
update containers       set nodetypeid = '677', nodename = barcodeid;
update inventory_groups set nodetypeid = '681', nodename = inventorygroupname;
update locations        set nodetypeid = '682', nodename = pathname;
update materials_subclass set nodetypeid = '678', nodename = subclassname;
update materials_synonyms set nodetypeid = '680', nodename = synonymname;
update packages         set nodetypeid = '674', nodename = productno;
update packdetail       set nodetypeid = '676', nodename = catalogno;
update units_of_measure set nodetypeid = '683', nodename = unitofmeasurename;
update users            set nodetypeid = '684', nodename = username;
update vendors          set nodetypeid = '675', nodename = vendorname;
commit;

alter table packages add materialname varchar2(100);
alter table packages add manufacturer varchar2(100);
alter table packages add supplier varchar2(100);
alter table locations add inventorygroup varchar2(40);
alter table users	add supervisor varchar2(40);
alter table users	add homeinventorygroup varchar2(40);
alter table users	add defaultlocation varchar2(420);
alter table materials add materialsubclass varchar2(80);
alter table materials add creationsite varchar2(40);
alter table materials add creationuser varchar2(40);
alter table packdetail add packagename varchar2(50);
alter table packdetail add unitofmeasurename varchar2(40);
alter table materials_synonyms add materialname varchar2(254);
alter table containers add synonymname varchar2(254);
alter table containers add parentname varchar2(20);
alter table containers add originalparentname varchar2(20);
alter table containers add ownername varchar2(40);
alter table containers add project varchar2(80);
alter table containers add locationname varchar2(80);
alter table containers add movereqlocationname varchar2(80);
alter table containers add homelocationname varchar2(80);
alter table containers add unitofmeasurename varchar2(40);
alter table containers add tareunitofmeasurename varchar2(40);
alter table containers add packdetail varchar2(20);

/*
update packages set materialname = (select materialname from materials where materialid = packages.materialid) where materialname is null;
update packages set manufacturer = (select vendorname from vendors where vendorid = packages.manufacturerid) where manufacturer is null;
update packages set supplier = (select vendorname from vendors where vendorid = packages.supplierid) where supplier is null;
update locations set inventorygroup = (select inventorygroupname from inventory_groups where inventorygroupid = locations.inventorygroupid) where inventorygroup is null;
update users set supervisor = (select username from users x where x.userid = users.supervisorid) where supervisor is null;
update users set homeinventorygroup = (select inventorygroupname from inventory_groups where inventorygroupid = users.homeinventorygroupid) where homeinventorygroup is null;
update users set defaultlocation = (select pathname from locations where locationid = users.defaultlocationid) where defaultlocation is null;
update materials set materialsubclass = (select subclassname from materials_subclass where materialsubclassid = materials.materialsubclassid) where materialsubclass is null;
--update materials set creationsite = (select sitename from sites where siteid = materials.creationsiteid) where creationsite is null;
update materials set creationuser = (select username from users where userid = materials.creationuserid) where creationuser is null;
update packdetail set packagename = (select productno from packages where packageid = packdetail.packageid) where packagename is null;
update packdetail set unitofmeasurename = (select unitofmeasurename from units_of_measure where unitofmeasureid = packdetail.unitofmeasureid) where unitofmeasurename is null;
update materials_synonyms set materialname = (select materialname from materials where materialid = materials_synonyms.materialid) where materialname is null;
update containers set synonymname = (select synonymname from materials_synonyms where materialsynonymid = containers.materialsynonymid) where synonymname is null;
update containers set parentname = (select barcodeid from containers where containerid = containers.parentid) where parentname is null;
update containers set originalparentname = (select barcodeid from containers where containerid = containers.originalparentid) where originalparentname is null;
update containers set ownername = (select username from users where userid = containers.ownerid) where ownername is null;
update containers set project = (select projectname from projects where projectid = containers.projectid) where project is null;
update containers set locationname = (select pathname from locations where locationid = containers.locationid) where locationname is null;
update containers set movereqlocationname = (select pathname from locations where locationid = containers.locationidmovereq) where movereqlocationname is null;
update containers set homelocationname = (select pathname from locations where locationid = containers.homelocationid) where homelocationname is null;
update packdetail set unitofmeasurename = (select unitofmeasurename from units_of_measure where unitofmeasureid = containers.unitofmeasureid) where unitofmeasurename is null;
update containers set tareunitofmeasurename = (select unitofmeasurename from units_of_measure where unitofmeasureid = containers.tareunitofmeasureid) where tareunitofmeasurename is null;
update containers set packdetail = (select catalogno from packdetail where packdetailid = containers.packdetailid) where packdetail is null;
commit;
*/

update packages set pendingupdate = 1;
update locations set pendingupdate = 1;
update users set pendingupdate = 1;
update materials set pendingupdate = 1;
update packdetail set pendingupdate = 1;
update materials_synonyms set pendingupdate = 1;
update containers set pendingupdate = 1;
commit;

alter table materials drop column disposal;
alter table materials add disposal clob;
commit;
