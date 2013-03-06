@ECHO OFF
setlocal

IF "%1"=="" GOTO Usage
GOTO Run

:Usage
echo # 
echo # Invalid Argument
echo # Usage: %0 [KilnPath]
echo # Example: %0 C:\kiln
echo #   [KilnPath]: The local absolute root of Kiln (e.g., "C:\kiln" )
echo # 
GOTO End

:Run

call %1\Nbt\Nbt\Scripts\Links\make_one_link %1 \Common\CswLogService\CswLogService\bin
call %1\Nbt\Nbt\Scripts\Links\make_one_link %1 \Nbt\Nbt\NbtSchedService\bin
call %1\Nbt\Nbt\Scripts\Links\make_one_link %1 \Nbt\Nbt\NbtSchemaImporter\bin
call %1\Nbt\Nbt\Scripts\Links\make_one_link %1 \Nbt\Nbt\NbtSchemaUpdater\bin
call %1\Nbt\Nbt\Scripts\Links\make_one_link %1 \Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin
call %1\Nbt\Nbt\Scripts\Links\make_one_link %1 \Nbt\Nbt\NbtWebApp
call %1\Nbt\Nbt\Scripts\Links\make_one_link %1 \Nbt\Nbt\NbtWcfServices
call %1\Nbt\Nbt\Scripts\Links\make_one_link %1 \Nbt\Nbt\TestApps\SchedulerGuiTestMt\bin
call %1\Nbt\Nbt\Scripts\Links\make_one_link "C:\Program Files (x86)\Microsoft Visual Studio 11.0" \Common7\IDE\CommonExtensions\Microsoft
call %1\Nbt\Nbt\Scripts\Links\make_one_link "C:\Program Files (x86)" \JetBrains\ReSharper\v7.0

:End
