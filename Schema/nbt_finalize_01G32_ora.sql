

CREATE OR REPLACE PROCEDURE update_sequences authid current_user IS
  cursor cur is select columnname, tablename from data_dictionary where columntype = 'pk' and isview= '0';

  cursor cur2 is select t.sequenceid, replace(t.sequencename, ' ', '') sequencename, x.maxval
                   from sequences t
                   join (select s.sequenceid,
                                max(TO_NUMBER(SUBSTR(j.field1, LENGTH(s.prep) + 1))) maxval
                           from sequences s
                           left outer join nodetype_props p on (s.sequenceid = p.sequenceid)
                           left outer join jct_nodes_props j on (p.nodetypepropid = j.nodetypepropid)
                          group by s.sequenceid) x on x.sequenceid = t.sequenceid;

  var_maxval NUMBER;
  var_seqval NUMBER;
  var_seqcnt number;
  begin

  -- note: system must:
  --  grant execute on this_user.update_sequences to this_user;
  --  grant create any sequence to this_user;

  -- Table Sequences
  for rec in cur loop
    execute immediate 'select count(*) from user_sequences where lower(sequence_name)=lower(''seq_' || rec.columnname || ''')' into var_seqcnt;

    if(var_seqcnt<1) then
      execute immediate 'create sequence seq_' || rec.columnname || ' minvalue 1 start with 1 increment by 1';
    end if;

    execute immediate 'select nvl(max(' || rec.columnname || '), 1) from ' || rec.tablename into var_maxval;
    execute immediate 'SELECT NVL(last_number,0) FROM user_sequences WHERE lower(sequence_name)=lower(''seq_' || rec.columnname || ''')' into var_seqval;

    if var_maxval >= var_seqval then
      execute immediate 'alter sequence seq_' || rec.columnname || ' increment by ' || to_char(greatest(var_maxval - var_seqval + 1, 1)) || ' ';
      execute immediate 'select seq_' || rec.columnname || '.nextval from dual' into var_seqval;
      execute immediate 'alter sequence seq_' || rec.columnname || ' increment by 1';
    end if;
  end loop;
  
  -- NBT Sequences
  for rec2 in cur2 loop
    execute immediate 'select count(*) from user_sequences where lower(sequence_name)=lower(''seq_' || rec2.sequencename || ''')' into var_seqcnt;

    if(var_seqcnt<1) then
      execute immediate 'create sequence seq_' || rec2.sequencename || ' minvalue 1 start with 1 increment by 1';
    end if;

    execute immediate 'SELECT NVL(last_number,0) FROM user_sequences WHERE lower(sequence_name)=lower(''seq_' || rec2.sequencename || ''')' into var_seqval;

    if rec2.maxval is not null and rec2.maxval >= var_seqval then
      execute immediate 'alter sequence seq_' || rec2.sequencename || ' increment by ' || to_char(greatest(rec2.maxval - var_seqval + 1, 1)) || ' ';
      execute immediate 'select seq_' || rec2.sequencename || '.nextval from dual' into var_seqval;
      execute immediate 'alter sequence seq_' || rec2.sequencename || ' increment by 1';
    end if;

  end loop;
  end;

/

exec update_sequences;
commit;


create or replace procedure DROP_SEQUENCES is
begin
  DECLARE
    CURSOR cur_objects(obj_type VARCHAR2) IS
      SELECT object_name FROM user_objects WHERE object_type IN (obj_type);

    obj_name VARCHAR(200);
    sql_str  VARCHAR(500);

  BEGIN
    OPEN cur_objects('SEQUENCE');

    LOOP
      FETCH cur_objects
        INTO obj_name;
      EXIT WHEN cur_objects%NOTFOUND;

      sql_str := 'drop SEQUENCE ' || obj_name;
      EXECUTE IMMEDIATE sql_str;

    END LOOP;

    CLOSE cur_objects;
  END;

end;
/



create or replace view nbtdata as
select n.nodeid, n.nodename,  t.nodetypeid, t.nodetypename, o.objectclassid, o.objectclass,
p.nodetypepropid, p.propname, op.objectclasspropid, op.propname objectclasspropname, f.fieldtype,
j.jctnodepropid, j.field1, j.field2, j.field3, j.field4, j.field5, j.field1_fk, j.gestalt, j.clobdata
from nodes n
join nodetypes t on n.nodetypeid = t.nodetypeid
join object_class o on t.objectclassid = o.objectclassid
join nodetype_props p on p.nodetypeid = t.nodetypeid
join field_types f on p.fieldtypeid = f.fieldtypeid
left outer join object_class_props op on p.objectclasspropid = op.objectclasspropid
left outer join jct_nodes_props j on (j.nodeid = n.nodeid and j.nodetypepropid = p.nodetypepropid)
order by lower(n.nodename), lower(p.propname);

commit;
