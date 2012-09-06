:: parameter 1: gives the local absolute root of Kiln (e.g., "C:\kiln" )

call %1\Nbt\Nbt\Scripts\make_one_link %1 \Common\CswLogService\CswLogService\bin
call %1\Nbt\Nbt\Scripts\make_one_link %1 \Nbt\Nbt\NbtSchedService\bin
call %1\Nbt\Nbt\Scripts\make_one_link %1 \Nbt\Nbt\NbtSchemaImporter\bin
call %1\Nbt\Nbt\Scripts\make_one_link %1 \Nbt\Nbt\NbtSchemaUpdater\bin
call %1\Nbt\Nbt\Scripts\make_one_link %1 \Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin
call %1\Nbt\Nbt\Scripts\make_one_link %1 \Nbt\Nbt\NbtWebApp



