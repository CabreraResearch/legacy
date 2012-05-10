echo "Enter the release name tag to update to (For example, Horatio_2011.8.2.1):"
set TagName=%1


echo "Stopping Services..."

net stop "ChemSW Log Service"
net stop "ChemSW NBT Schedule Service"

echo "Services stopped."


echo "Pulling Source Code from Main..."

hg pull -R /kiln/Common/CswCommon
hg pull -R /kiln/Common/CswConfigUI
hg pull -R /kiln/Common/CswWebControls
hg pull -R /kiln/Common/CswLogService
hg pull -R /kiln/nbt/nbt

hg pull -R /kiln/ThirdParty/ClosureCompiler
hg pull -R /kiln/ThirdParty/OracleDataAccess 
hg pull -R /kiln/ThirdParty/YUICompressor

echo "Pull from Main Completed."


echo "Updating working directories to tag..."

hg update -R /kiln/Common/CswCommon -r "%TagName%" -C
hg update -R /kiln/Common/CswConfigUI -r "%TagName%" -C
hg update -R /kiln/Common/CswWebControls -r "%TagName%" -C
hg update -R /kiln/Common/CswLogService -r "%TagName%" -C
hg update -R /kiln/nbt/nbt -r "%TagName%" -C

echo "Update completed."



echo "Compiling new code..."

msbuild C:\kiln\Nbt\Nbt\Nbt.sln /p:Configuration=Release

echo "Compile Finished."




echo "Starting Schema updater..."

C:\kiln\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Release\NbtUpdt.exe -all

echo "Schema update completed."



echo "Restarting Services..."

net start "ChemSW Log Service"
net start "ChemSW NBT Schedule Service"
iisreset

echo "Services Restarted."

echo "All done!"

