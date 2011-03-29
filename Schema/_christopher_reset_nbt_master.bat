echo exit | sqlplus nbt_master/nbt@baal @nbt_nuke.sql

impdp.exe nbt_master/nbt@baal DUMPFILE=NBT_MASTER_11G.dmp DIRECTORY=NBTDUMPS 

echo exit | sqlplus nbt_master/nbt@baal @nbt_finalize_01H21_ora.sql
