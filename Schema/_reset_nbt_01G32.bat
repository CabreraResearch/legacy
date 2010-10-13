echo exit | sqlplus nbt/nbt@orcl @D:\vault\Dn\Nbt\Schema\nbt_nuke.sql

imp.exe nbt/nbt@orcl FILE=D:\vault\Dn\Nbt\Schema\Dumps\Nbt_Master_01G32.dmp FULL=Y TOUSER=nbt

echo exit | sqlplus nbt/nbt@orcl @D:\vault\Dn\Nbt\Schema\nbt_finalize_01H06_ora.sql







