echo exit | sqlplus nbt/nbt@w2008x64Db @nbt_nuke.sql

impdp.exe nbt/nbt@w2008x64Db DUMPFILE=NBT_MASTER_11G.dmp DIRECTORY=NBTDUMPS REMAP_SCHEMA=nbt_master:nbt
rem imp.exe nbt/nbt@w2008x64Db file=C:\kiln\Nbt\Nbt\Schema\Dumps\Nbt_Master_01G32.dmp full=yes buffer=10000000 

echo exit | sqlplus nbt/nbt@w2008x64Db @nbt_finalize_ora.sql

C:\kiln\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Debug\NbtUpdt.exe -accessid 1 -mode prod
