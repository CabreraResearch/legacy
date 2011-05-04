echo exit | sqlplus nbt_master/nbt_master@orcl @nbt_nuke.sql

impdp.exe nbt_master/nbt_master@orcl DUMPFILE=NBT_MASTER_11G.dmp DIRECTORY=NBTDUMPS

echo exit | sqlplus nbt_master/nbt_master@orcl @nbt_finalize_ora.sql
