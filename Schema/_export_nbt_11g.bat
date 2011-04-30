set /p username=
set /p database=

del Dumps\Nbt_Master_11g.dmp
expdp.exe %username%/nbt@%database% DUMPFILE=NBT_MASTER_11g.dmp DIRECTORY=NBTDUMPS