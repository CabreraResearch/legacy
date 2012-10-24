-- ****** Object: User NBT Script Date: 10/22/2012 10:28:20 AM ******
CREATE USER "NBT" 
  IDENTIFIED BY ******* 
  DEFAULT TABLESPACE "USERS" 
  TEMPORARY TABLESPACE "TEMP" 
  QUOTA UNLIMITED ON "USERS" 
  QUOTA 0 K ON "SYSTEM"
/
-- ****** Object: Roles for user NBT Script Date: 10/22/2012 10:28:20 AM ******
GRANT RESOURCE,
  CONNECT 
  TO "NBT"
/
-- ****** Object: System privileges for user NBT Script Date: 10/22/2012 10:28:20 AM ******
GRANT CREATE ANY SEQUENCE,
  CREATE VIEW,
  SELECT ANY TABLE,
  UNLIMITED TABLESPACE 
  TO "NBT"
/
-- ****** Object: Privileges on database objects for user NBT Script Date: 10/22/2012 10:28:21 AM ******
GRANT WRITE,
  READ,
  WRITE,
  READ 
  ON "SYS"."NBTDUMPS" 
  TO "NBT"
/
