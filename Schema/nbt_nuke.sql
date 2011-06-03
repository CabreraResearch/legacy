-- drop all tables
create or replace procedure DROP_TABLES is
begin
  DECLARE
    CURSOR cur_objects(obj_type VARCHAR2) IS
      SELECT object_name FROM user_objects WHERE object_type IN (obj_type);

    obj_name VARCHAR(200);
    sql_str  VARCHAR(500);

  BEGIN
    OPEN cur_objects('TABLE');

    LOOP
      FETCH cur_objects
        INTO obj_name;
      EXIT WHEN cur_objects%NOTFOUND;

      sql_str := 'drop TABLE ' || obj_name;
      EXECUTE IMMEDIATE sql_str;

    END LOOP;

    CLOSE cur_objects;
  END;

end;

exec drop_tables;
commit;

drop procedure drop_tables;
commit;

exec drop_sequences;
commit;
