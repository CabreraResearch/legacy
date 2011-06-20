echo exit | sqlplus nbt/nbt@orcl @nbt_nuke.sql

impdp.exe nbt/nbt@orcl DUMPFILE=NBT_MASTER_11g.dmp DIRECTORY=NBTDUMPS REMAP_SCHEMA=nbt_master:nbt

echo exit | sqlplus nbt/nbt@orcl @nbt_finalize_ora.sql

C:\kiln\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Debug\NbtUpdt.exe -accessid 1 -mode prod

pause