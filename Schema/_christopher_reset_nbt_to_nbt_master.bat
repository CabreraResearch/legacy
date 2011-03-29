echo exit | sqlplus nbt/nbt@baal @nbt_nuke.sql

impdp.exe nbt/nbt@baal DUMPFILE=NBT_MASTER_11g.dmp DIRECTORY=NBTDUMPS REMAP_SCHEMA=nbt_master:nbt

echo exit | sqlplus nbt/nbt@baal @nbt_finalize_01H21_ora.sql
