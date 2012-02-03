echo exit | sqlplus david/david@golem @nbt_nuke.sql

impdp.exe david/david@golem DUMPFILE=NBT_MASTER_11G.dmp DIRECTORY=NBTDUMPS

echo exit | sqlplus david/david@golem @nbt_finalize_ora.sql

rem C:\kiln\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Debug\NbtUpdt.exe -accessid 1 -mode prod

pause