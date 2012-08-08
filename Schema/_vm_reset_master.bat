echo exit | sqlplus nbt_master/nbt@w2008x64Db @nbt_nuke.sql

impdp.exe nbt_master/nbt@w2008x64Db DUMPFILE=NBT_MASTER_11G.dmp DIRECTORY=NBTDUMPS

echo exit | sqlplus nbt_master/nbt@w2008x64Db @nbt_finalize_ora.sql

C:\kiln\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Debug\NbtUpdt.exe -all -mode prod
