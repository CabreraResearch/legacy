hg pull -R /kiln/Common/CswCommon -u main
hg pull -R /kiln/Common/CswConfigUI -u main
hg pull -R /kiln/Common/CswWebControls -u main
hg pull -R /kiln/Common/CswLogService -u main

echo "Pull from Common Complete"

hg pull -R /kiln/nbt/nbt -u main
hg pull -R /kiln/nbt/NbtImport -u main
hg pull -R /kiln/nbt/nbt/NbtWebApp/help -u

echo "Pull from Nbt Complete"

hg pull -R /kiln/ThirdParty/ClosureCompiler -u
hg pull -R /kiln/ThirdParty/OracleDataAccess -u
hg pull -R /kiln/ThirdParty/YUICompressor -u

echo "Pull from ThirdParty Complete"

pause
