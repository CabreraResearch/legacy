-- ****** Object: Stored Procedure NBT.CREATEALLNTVIEWS Script Date: 10/22/2012 10:27:46 AM ******
CREATE procedure CreateAllNtViews is
  cursor nts is
        select nodetypeid,nodetypename,firstversionid from nodetypes
        where firstversionid=nodetypeid order by lower(nodetypename);
  cursor ntsdel is
	select object_name from user_objects where object_type='VIEW' and object_name like 'NT%' or object_name like 'OC%';
  var_sql varchar2(200);
begin
  for delrec in ntsdel loop
	var_sql := 'drop view ' || delrec.object_name;
	execute immediate (var_sql);
  end loop;
  commit;

  for rec in nts loop
    --dbms_output.put_line('createntview(' || to_char(rec.nodetypeid) || ',' || rec.nodetypename || ')');
    CreateNtView(rec.nodetypeid);
  end loop;
end CreateAllNtViews;
/
