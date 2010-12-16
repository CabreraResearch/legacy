echo exit | sqlplus nbt/nbt@orcl @nbt_nuke.sql

impdp.exe nbt/nbt@orcl DUMPFILE=FE_Sales_Demo.dmp DIRECTORY=NBTDUMPS

echo exit | sqlplus nbt/nbt@orcl @nbt_finalize_01H11_ora.sql
