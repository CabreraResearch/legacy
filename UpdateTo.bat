echo "Please connect the VPN to Fairfield."

pause



echo "Enter the release name tag to update to (For example, Horatio_2011.8.2.1):"
set /p TagName=



echo "Stopping Services..."

net stop "ChemSW Log Service"
net stop "ChemSW NBT Schedule Service"

echo "Services stopped."

pause



echo "Pulling Source Code from Main..."

hg pull -R /kiln/Common/CswCommon
hg pull -R /kiln/Common/CswConfigUI
hg pull -R /kiln/Common/CswWebControls
hg pull -R /kiln/Common/CswLogService
hg pull -R /kiln/nbt/nbt

echo "Pull from Main Completed."

pause



echo "Updating working directories to tag..."

hg update -R /kiln/Common/CswCommon -r "%TagName%"
hg update -R /kiln/Common/CswConfigUI -r "%TagName%"
hg update -R /kiln/Common/CswWebControls -r "%TagName%"
hg update -R /kiln/Common/CswLogService -r "%TagName%"
hg update -R /kiln/nbt/nbt -r "%TagName%"

echo "Update completed."

pause



echo "Compiling new code..."

msbuild C:\kiln\Nbt\Nbt\Nbt.sln /p:Configuration=Release

echo "Compile Finished."

pause



echo "Starting ASP Precompile..."

"c:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_compiler.exe" -v /NbtWebApp -p "C:\kiln\Nbt\Nbt\NbtWebApp"

echo "ASP Precompile Finished"

pause



echo "Starting Schema updater..."

C:\kiln\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Release\NbtUpdt.exe -all

echo "Schema update completed."

pause



echo "Restarting Services..."

net start "ChemSW Log Service"
net start "ChemSW NBT Schedule Service"
iisreset

echo "Services Restarted."



echo "All done!"

pause