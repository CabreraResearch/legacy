set /p username=
set /p database=

echo exit | sqlplus %username%/nbt@%database% @nbt_nuke.sql

impdp.exe %username%/nbt@%database% DUMPFILE=NBT_MASTER_11G.dmp DIRECTORY=NBTDUMPS REMAP_SCHEMA=nbt_master:%username%

echo exit | sqlplus %username%/nbt@%database% @nbt_finalize_ora.sql
