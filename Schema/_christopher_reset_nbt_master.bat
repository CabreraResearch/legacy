echo exit | sqlplus cf_nbt_master/nbt@madeye @nbt_nuke.sql

impdp.exe cf_nbt_master/nbt@madeye DUMPFILE=NBT_MASTER_11G.dmp DIRECTORY=NBTDUMPS REMAP_SCHEMA=nbt_master:cf_nbt_master

echo exit | sqlplus cf_nbt_master/nbt@madeye @nbt_finalize_01H24_ora.sql
