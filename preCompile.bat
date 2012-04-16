net stop "ChemSW Log Service"
taskkill /F /IM "CswLogService.exe"

net stop "ChemSW NBT Schedule Service"
taskkill /F /IM "NbtSchedService.exe"

net stop "NbtSchemaUpdater.exe"
taskkill /F /IM "NbtSchemaUpdater.exe"



pause "Csw Services Stopped"