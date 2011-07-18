set /p username=
set /p database=

echo exit | sqlplus mcmaster/nbt@golem @nbt_nuke.sql

impdp.exe mcmaster/nbt@golem DUMPFILE=MARSHFIELD_2011.7.15.1.dmp DIRECTORY=MADEYE_DUMPS REMAP_SCHEMA=marshfield:mcmaster

echo exit | sqlplus mcmaster/nbt@golem @nbt_finalize_ora.sql
