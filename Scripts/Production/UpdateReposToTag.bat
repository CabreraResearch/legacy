echo "Enter the release name tag to update to (For example, Horatio_2011.8.2.1):"
set /p TagName=

echo "Pulling Source Code from Main..."

hg pull -R /kiln/Common/CswCommon
hg pull -R /kiln/Common/CswConfigUI
hg pull -R /kiln/Common/CswWebControls
hg pull -R /kiln/Common/CswLogService
hg pull -R /kiln/nbt/nbt
hg pull -R /kiln/Common/StructureSearch

echo "Pull from Main Completed."

echo "Updating working directories to tag..."

hg update -R /kiln/Common/CswCommon -r "%TagName%" -C
hg update -R /kiln/Common/CswConfigUI -r "%TagName%" -C
hg update -R /kiln/Common/CswWebControls -r "%TagName%" -C
hg update -R /kiln/Common/CswLogService -r "%TagName%" -C
hg update -R /kiln/Common/StructureSearch -r "%TagName%" -C
hg update -R /kiln/nbt/nbt -r "%TagName%" -C

echo "Update completed."

pause