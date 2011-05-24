hg pull -R /kiln/Common/CswCommon -u main
hg pull -R /kiln/Common/CswConfigUI -u main
hg pull -R /kiln/Common/CswWebControls -u main
hg pull -R /kiln/Common/CswLogService -u main
hg pull -R /kiln/nbt/nbt -u main

echo "Pull from Main Complete"

pause

set /p KilnDriveLetter=
%KilnDriveLetter%:\kiln\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Debug\NbtUpdt.exe -all

echo "Schema update complete"

pause