set /p username=
set /p database=

echo exit | sqlplus %username%/nbt@%database% @nbt_nuke.sql

imp.exe %username%/nbt@%database% FILE=Dumps\Nbt_Master_01G32.dmp FULL=Y TOUSER=%username%

echo exit | sqlplus %username%/nbt@%database% @nbt_finalize_01H21_ora.sql







