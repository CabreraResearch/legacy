hg pull -R /kiln/Common/CswCommon -u main
hg pull -R /kiln/Common/CswConfigUI -u main
hg pull -R /kiln/Common/CswWebControls -u main
hg pull -R /kiln/Common/CswLogService -u main
hg pull -R /kiln/nbt/nbt -u main

echo "Pull from Main Complete"

pause

set /p DriveLetter=

net stop "ChemSW Log Service
net stop "NbtSchedService
msbuild %DriveLetter%:\kiln\Nbt\Nbt\Nbt.sln

echo "Compile Finished"

pause

"c:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_compiler.exe" -v /NbtWebApp -p %DriveLetter%":\kiln\Nbt\Nbt\NbtWebApp"

echo "ASP Precompile Finished"

pause

%DriveLetter%:\kiln\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Debug\NbtUpdt.exe -all

echo "Schema update complete"

pause