set /p username=
set /p database=
set /p datapumpdir= 
set /p filename=
set /p password=
set /p fromuser=

echo exit | sqlplus %username%/%password%@%database% @nbt_nuke.sql

impdp.exe %username%/%password%@%database% DUMPFILE=%filename% DIRECTORY=%datapumpdir% REMAP_SCHEMA=%fromuser%:%username%

echo exit | sqlplus %username%/%password%@%database% @nbt_finalize_ora.sql
