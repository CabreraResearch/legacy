echo "Stopping Services..."

net stop "ChemSW Log Service"
net stop "ChemSW NBT Schedule Service"
taskkill /F /IM "NbtSchedService.exe"

echo "Services stopped."

echo "Compiling new code..."

msbuild d:\kiln\Nbt\Nbt\Nbt.sln /p:Configuration=Release

echo "Compile Finished."


echo "Starting Schema updater..."

d:\kiln\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Release\NbtUpdt.exe -all

echo "Schema update completed."

echo "Restarting Services..."

net start "ChemSW Log Service"
net start "ChemSW NBT Schedule Service"
iisreset

echo "Services Restarted."

echo "All done!"