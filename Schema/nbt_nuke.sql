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

      sql_str := 'drop TABLE ' || obj_name || ' CASCADE CONSTRAINTS PURGE';
      EXECUTE IMMEDIATE sql_str;

    END LOOP;

    CLOSE cur_objects;
  END;

end;
/

exec drop_tables;
commit;

drop procedure drop_tables;
commit;


-- drop all sequences
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

exec drop_sequences;
commit;

drop procedure drop_sequences;
commit;


-- drop all views
create or replace procedure DROP_VIEWS is
begin
  DECLARE
    CURSOR cur_objects(obj_type VARCHAR2) IS
      SELECT object_name FROM user_objects WHERE object_type IN (obj_type);

    obj_name VARCHAR(200);
    sql_str  VARCHAR(500);

  BEGIN
    OPEN cur_objects('VIEW');

    LOOP
      FETCH cur_objects
        INTO obj_name;
      EXIT WHEN cur_objects%NOTFOUND;

      sql_str := 'drop VIEW ' || obj_name;
      EXECUTE IMMEDIATE sql_str;

    END LOOP;

    CLOSE cur_objects;
  END;

end;
/

exec drop_views;
commit;

drop procedure drop_views;
commit;

-- drop all functions
create or replace procedure DROP_FUNCTIONS is
begin
  DECLARE
    CURSOR cur_objects(obj_type VARCHAR2) IS
      SELECT object_name FROM user_objects WHERE object_type IN (obj_type);

    obj_name VARCHAR(200);
    sql_str  VARCHAR(500);

  BEGIN
    OPEN cur_objects('FUNCTION');

    LOOP
      FETCH cur_objects
        INTO obj_name;
      EXIT WHEN cur_objects%NOTFOUND;

      sql_str := 'drop FUNCTION ' || obj_name;
      EXECUTE IMMEDIATE sql_str;

    END LOOP;

    CLOSE cur_objects;
  END;

end;
/

exec drop_functions;
commit;

drop procedure drop_functions;
commit;



--THIS ONE ALWAYS LAST!
-- drop all procedures
create or replace procedure DROP_PROCEDURES is
begin
  DECLARE
    CURSOR cur_objects(obj_type VARCHAR2) IS
      SELECT object_name FROM user_objects WHERE object_type IN (obj_type);

    obj_name VARCHAR(200);
    sql_str  VARCHAR(500);

  BEGIN
    OPEN cur_objects('PROCEDURE');

    LOOP
      FETCH cur_objects
        INTO obj_name;
      EXIT WHEN cur_objects%NOTFOUND;

      if(obj_name != 'DROP_PROCEDURES') then
        sql_str := 'drop PROCEDURE ' || obj_name;
        EXECUTE IMMEDIATE sql_str;
      end if;

    END LOOP;

    CLOSE cur_objects;
  END;

end;
/

exec drop_procedures;
commit;

drop procedure drop_procedures;
commit;


