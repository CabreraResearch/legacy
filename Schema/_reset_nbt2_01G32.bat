echo exit | sqlplus nbt2/nbt2@orcl @D:\vault\Dn\Nbt\Schema\nbt_nuke.sql

imp.exe nbt2/nbt2@orcl FILE=D:\vault\Dn\Nbt\Schema\Dumps\Nbt_Master_01G32.dmp FULL=Y TOUSER=nbt2

echo exit | sqlplus nbt2/nbt2@orcl @D:\vault\Dn\Nbt\Schema\nbt_finalize_01H06_ora.sql







