set /p username=
set /p database=
set /p datapumpdir= 
set /p filename=
set /p password=

echo exit | sqlplus %username%/%password%@%database% @nbt_nuke.sql

impdp.exe %username%/%password%@%database% DUMPFILE=%filename% DIRECTORY=%datapumpdir% REMAP_SCHEMA=nbt_master:%username%

echo exit | sqlplus %username%/%password%@%database% @nbt_finalize_ora.sql
