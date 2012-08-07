echo exit | sqlplus nbt/nbt@w2008x64Db @nbt_nuke.sql

impdp.exe nbt/nbt@w2008x64Db DUMPFILE=NBT_MASTER_11G.dmp DIRECTORY=NBTDUMPS REMAP_SCHEMA=nbt_master:nbt

echo exit | sqlplus nbt/nbt@w2008x64Db @nbt_finalize_ora.sql

C:\kiln\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Debug\NbtUpdt.exe -accessid 1 -mode prod
