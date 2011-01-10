echo exit | sqlplus nbt_master/nbt_master@orcl @nbt_nuke.sql

impdp.exe nbt_master/nbt_master@orcl DUMPFILE=NBT_MASTER.dmp DIRECTORY=NBTDUMPS REMAP_SCHEMA=nbt_base:nbt_master

echo exit | sqlplus nbt_master/nbt_master@orcl @nbt_finalize_01H13_ora.sql
