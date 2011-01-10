echo exit | sqlplus nbt/nbt@orcl @nbt_nuke.sql

imp.exe nbt/nbt@orcl FILE=Dumps\Nbt_Master_01G32.dmp FULL=Y TOUSER=nbt

echo exit | sqlplus nbt/nbt@orcl @nbt_finalize_01H13_ora.sql







