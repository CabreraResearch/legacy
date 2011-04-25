echo exit | sqlplus cf_nbt/nbt@madeye @nbt_nuke.sql

impdp.exe cf_nbt/nbt@madeye DUMPFILE=NBT_MASTER_11g.dmp DIRECTORY=NBTDUMPS REMAP_SCHEMA=nbt_master:cf_nbt

echo exit | sqlplus cf_nbt/nbt@madeye @nbt_finalize_01H24_ora.sql
