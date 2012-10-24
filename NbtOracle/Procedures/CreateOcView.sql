-- ****** Object: Stored Procedure NBT.CREATEOCVIEW Script Date: 10/22/2012 10:27:52 AM ******
CREATE procedure createOCview(objid in number) is
cursor props is select v.*
from vwObjProps v
where v.objectclassid=objid and objectclasspropid is not null order by objectclasspropid;
var_sql varchar2(20000);
aline varchar(2000);
pcount number;
viewname varchar(30);
begin
--dbms_output.enable(21000);
--dbms_output.put_line(to_char(objid));
select substr(objectclass,1,29) into viewname from object_class where objectclassid=objid;

var_sql:='create or replace view ' || OraColLen('OC',alnumonly(viewname,''),'') || ' as select n.nodeid ';
--dbms_output.put_line(var_sql);
pcount:=0;

for rec in props loop
pcount:=pcount+1;

--OLD aline:=',(select ' || rec.subfieldname || ' from vwNpvname where nid=n.nodeid and safepropname=''' || rec.safepropname || ''') ' || upper(rec.safepropname);
aline:=',(select ' || rec.subfieldname || ' from vwNpvname where nid=n.nodeid and objectclasspropid=' || rec.objectclasspropid || ') ' || 'OP_' || to_char(rec.objectclasspropid);
--dbms_output.put_line(aline);
var_sql:=var_sql || aline;

if(rec.fieldtype='Relationship' or rec.fieldtype='Location') then
--OLD aline:=',(select field1_fk from vwNpvname where nid=n.nodeid and safepropname=''' || rec.safepropname || ''') ' || OraColLen('',upper(rec.safepropname),'_FK');
aline:=',(select field1_fk from vwNpvname where nid=n.nodeid and objectclasspropid=' || rec.objectclasspropid || ') OP_' || rec.objectclasspropid || '_FK';
-- dbms_output.put_line(aline);
var_sql:=var_sql || aline;
end if;

end loop;

if(pcount>0) then
aline:= ' from nodes n join nodetypes nt on nt.nodetypeid=n.nodetypeid and nt.objectclassid=' || to_char(objid);
--dbms_output.put_line(aline);
var_sql:=var_sql || aline;
--dbms_output.put_line(var_sql);
execute immediate (var_sql);
commit;
end if;

end createOCview;
/
