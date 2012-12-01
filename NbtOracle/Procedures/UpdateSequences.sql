-- ****** Object: Stored Procedure NBT.UPDATE_SEQUENCES Script Date: 10/22/2012 10:27:58 AM ******
CREATE PROCEDURE update_sequences authid current_user IS
  cursor cur is select columnname, tablename from data_dictionary where columntype = 'pk' and isview= '0';

  cursor cur2 is select t.sequenceid, replace(t.sequencename, ' ', '') sequencename, x.maxval
                   from sequences t
                   join (select s.sequenceid,
                                (nvl(max(j.field1_numeric),0) + 1) maxval
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
