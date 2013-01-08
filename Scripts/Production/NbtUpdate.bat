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

echo "Stopping Services..."

net stop "ChemSW Log Service"
net stop "ChemSW NBT Schedule Service"
taskkill /F /IM "NbtSchedService.exe"

echo "Services stopped."

echo "Compiling new code..."

msbuild %1\Nbt\Nbt\Nbt.sln /p:Configuration=Release /p:Platform="x64"

echo "Compile Finished."

pause

echo "Starting Schema updater..."

%1\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Release\NbtUpdt.exe -all

echo "Schema update completed."

echo "Listing Updated Customer Schema Versions"

%1\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Release\NbtUpdt.exe -version

echo "Version Listing Complete"

pause


echo "Restarting Services..."

net start "ChemSW Log Service"
net start "ChemSW NBT Schedule Service"
iisreset

echo "Services Restarted."

echo "All done!"

pause

:End