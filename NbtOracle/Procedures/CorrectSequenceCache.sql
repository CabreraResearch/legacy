-- ****** Object: Stored Procedure NBT.CORRECT_SEQUENCE_CACHE Script Date: 10/22/2012 10:27:36 AM ******
CREATE PROCEDURE correct_sequence_cache authid current_user IS
  cursor cur is select columnname, tablename from data_dictionary where columntype = 'pk' and isview= '0' and tablename in ('nodes','jct_nodes_props','nodes_audit','jct_nodes_props_audit','sessionlist','sessiondata','audit_transactions');

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
      execute immediate 'alter sequence seq_' || rec.columnname || ' cache 1000';
  end loop;


  -- NBT Sequences
  for rec2 in cur2 loop
    execute immediate 'alter sequence seq_' || rec2.sequencename || ' cache 1000';
  end loop;
  end;

/
