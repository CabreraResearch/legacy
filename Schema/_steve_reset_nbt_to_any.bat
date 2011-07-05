set /p DUMPFILE= 
set /p SCHEMAUSER= 

echo exit | sqlplus nbt/nbt@orcl @nbt_nuke.sql

impdp.exe nbt/nbt@orcl DUMPFILE=%DUMPFILE% DIRECTORY=NBTDUMPS REMAP_SCHEMA=%SCHEMAUSER%:nbt

echo exit | sqlplus nbt/nbt@orcl @nbt_finalize_ora.sql

C:\kiln\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Debug\NbtUpdt.exe -accessid 1 -mode prod

pause