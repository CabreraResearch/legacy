set /p KilnDrive=

hg pull -R %KilnDrive%:/kiln/Common/CswCommon -u main
hg pull -R %KilnDrive%:/kiln/Common/CswConfigUI -u main
hg pull -R %KilnDrive%:/kiln/Common/CswWebControls -u main
hg pull -R %KilnDrive%:/kiln/Common/CswLogService -u main
hg pull -R %KilnDrive%:/kiln/nbt/nbt -u main
