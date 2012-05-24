set /p database=
set /p username=
set /p password=

echo exit | sqlplus %username%/%password%@%database% @nbt_nuke.sql

impdp.exe %username%/%password%@%database% DUMPFILE=NBT_MASTER_11G.dmp DIRECTORY=NBTDUMPS REMAP_SCHEMA=nbt_master:%username%

echo exit | sqlplus %username%/%password%@%database% @nbt_finalize_ora.sql



echo exit | sqlplus %username%/%password%@%database% @nbt_finalize_ora.sql
echo exit | sqlplus %username%/%password%@%database% @indexes.sql