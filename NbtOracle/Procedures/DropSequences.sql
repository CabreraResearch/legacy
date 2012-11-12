-- ****** Object: Stored Procedure NBT.DROP_SEQUENCES Script Date: 10/22/2012 10:27:54 AM ******
CREATE procedure DROP_SEQUENCES is
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
