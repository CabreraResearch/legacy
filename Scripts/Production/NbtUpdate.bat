echo "Stopping Services..."

net stop "ChemSW Log Service"
net stop "ChemSW NBT Schedule Service"
taskkill /F /IM "NbtSchedService.exe"

echo "Services stopped."

echo "Compiling new code..."

msbuild D:\kiln\Nbt\Nbt\Nbt.sln /p:Configuration=Release /p:Platform="x64"

echo "Compile Finished."

pause

echo "Starting Schema updater..."

D:\kiln\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Release\NbtUpdt.exe -all

echo "Schema update completed."

echo "Listing Updated Customer Schema Versions"

D:\kiln\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Release\NbtUpdt.exe -version

echo "Version Listing Complete"

pause

echo "Restarting Services..."

net start "ChemSW Log Service"
net start "ChemSW NBT Schedule Service"
iisreset

echo "Services Restarted."

echo "All done!"

pause