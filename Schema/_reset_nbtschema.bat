set /p accessid=
set /p username=
set /p password=

impdp.exe %username%/%password%@nbttest DUMPFILE=d:\kiln\nbt\nbt\schema\NBT_MASTER_11G.DMP DIRECTORY=EXPORTS REMAP_SCHEMA=nbt_master:%username%

D:\kiln\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Release\NbtUpdt.exe -accessid %accessid%

