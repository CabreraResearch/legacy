set /p accessid=
set /p username=
set /p password=

echo exit | sqlplus %username%/%password%@nbttest @nbt_nuke.sql

impdp.exe %username%/%password%@nbttest DUMPFILE=NBT_MASTER_11G.DMP DIRECTORY=EXPORTS REMAP_SCHEMA=nbt_master:%username%

D:\kiln\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Release\NbtUpdt.exe -accessid %accessid%

echo COMPLETED!

pause