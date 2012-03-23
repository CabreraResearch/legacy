set /p username=
set /p database=

echo exit | sqlplus %username%/nbt@%database% @nbt_nuke.sql

impdp.exe %username%/nbt@%database% DUMPFILE=NBT_MASTER_11G.dmp DIRECTORY=NBTDUMPS REMAP_SCHEMA=nbt_master:%username%

echo exit | sqlplus %username%/nbt@%database% @nbt_finalize_ora.sql

C:\kiln\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Debug\NbtUpdt.exe -accessid %username% -mode prod

echo exit | sqlplus nbt/nbt@w2008x64Db @nbt_finalize_ora.sql
echo exit | sqlplus nbt/nbt@w2008x64Db @indexes.sql