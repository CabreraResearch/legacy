-- ****** Object: Stored Procedure NBT.CREATENTVIEW Script Date: 10/22/2012 10:27:49 AM ******
CREATE procedure createNTview(ntid in number) is
  cursor props is
  select v.*,s.propcolname subfieldname,s.is_default,s.subfieldname subfieldalias from vwNtPropDefs v
    join field_types_subfields s on s.fieldtypeid=v.fieldtypeid and s.reportable='1'
    where v.nodetypeid=ntid order by lower(propname);
  var_sql varchar2(32000);
  var_line varchar(2000);
  pname varchar2(200);
  pcount number;
  viewname varchar2(30);
  objid number;
  ntcount number;
begin
  --dbms_output.enable(32000);


 select count(*) into ntcount from nodetypes where nodetypeid=ntid;

 if(ntcount>0) then
  select substr(nodetypename,1,29),objectclassid into viewname,objid from nodetypes where nodetypeid=ntid;

  var_line:='create or replace view ' || OraColLen('NT',alnumonly(viewname,''),'') || ' as select n.nodeid ';
  --dbms_output.put_line(var_line);
  var_sql := var_sql || var_line;
  pcount:=0;

  for rec in props loop
      pcount:=pcount+1;
      --dbms_output.put_line(to_char(pcount) || '|' || safeSqlParam(rec.propname) || '|' || rec.subfieldname || '|' || rec.fieldtype || '|' || rec.objectclass || '|' || rec.nodetypename);
      pname := to_char(rec.nodetypepropid);
      if(rec.is_default='1') then
        var_line := ',(select ' || rec.subfieldname || ' from vwNpv where nid=n.nodeid and ntpid=' || to_char(rec.nodetypepropid);
        var_line := var_line || ')' || OraColLen('P',alnumonly(upper(pname),''),'');
        --dbms_output.put_line(var_line);
        var_sql := var_sql || var_line;
      else
        if(rec.fieldtype='Relationship' or rec.fieldtype='Location') then
          if(rec.fktype='NodeTypeId') then
            var_line := ',(select ' || rec.subfieldname || ' from vwNpv where nid=n.nodeid and ntpid=' || to_char(rec.nodetypepropid) || ') ';
            var_line := var_line  || OraColLen('P',alnumonly(upper(pname || '_' || rec.nodetypename),''),'_NTFK');
          --  dbms_output.put_line(var_line);
            var_sql := var_sql || var_line;
          else
            var_line := ',(select ' || rec.subfieldname || ' from vwNpv where nid=n.nodeid and ntpid=' || to_char(rec.nodetypepropid) || ') ';
            var_line:=var_line || OraColLen('P',alnumonly(upper(pname || rec.objectclass),''),'_OCFK');
          --  dbms_output.put_line(var_line);
            var_sql := var_sql || var_line;
          end if;
        else
          var_line := ',(select ' || rec.subfieldname || ' from vwNpv where nid=n.nodeid and ntpid=' || to_char(rec.nodetypepropid) || ') ';
          var_line:=var_line || OraColLen('P',alnumonly(upper(pname || '_' || rec.subfieldalias),''),'');
          --dbms_output.put_line(var_line);
          var_sql := var_sql || var_line;
        end if;
      end if;
  end loop;

  if(pcount>0) then
    var_line := ' from nodes n where n.nodetypeid=' || to_char(ntid);
    --dbms_output.put_line(var_line);
    var_sql := var_sql || var_line;
    execute immediate (var_sql);
    commit;
    createOCview(objid);
  end if;
 end if;

end createNTview;
/
