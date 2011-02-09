echo exit | sqlplus nbt_master/nbt@dracula @nbt_nuke.sql

impdp.exe nbt_master/nbt@dracula DUMPFILE=NBT_MASTER_11G.dmp DIRECTORY=NBTDUMPS 

echo exit | sqlplus nbt_master/nbt@dracula @nbt_finalize_01H13_ora.sql
