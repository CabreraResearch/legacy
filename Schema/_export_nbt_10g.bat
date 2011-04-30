set /p username=
set /p database=

del Dumps\Nbt_Master_10g.dmp
c:\oracle\product\10.2.0\client_1\BIN\expdp.exe %username%/nbt@%database% DUMPFILE=NBT_MASTER_10g.dmp DIRECTORY=NBTDUMPS