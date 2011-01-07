echo exit | sqlplus nbt/nbt@orcl @nbt_nuke.sql

impdp.exe nbt/nbt@orcl DUMPFILE=NBT_MASTER.dmp DIRECTORY=NBTDUMPS REMAP_SCHEMA=nbt_base:nbt

echo exit | sqlplus nbt/nbt@orcl @nbt_finalize_01H13_ora.sql
