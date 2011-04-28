echo Schema1

echo exit | sqlplus nbt_master/nbt@madeye @nbt_nuke.sql

impdp.exe nbt_master/nbt@madeye DUMPFILE=NBT_MASTER_11G.dmp DIRECTORY=NBTDUMPS 

echo exit | sqlplus nbt_master/nbt@madeye @nbt_finalize_ora.sql


echo Schema2

echo exit | sqlplus nbt_schema1/nbt@madeye @nbt_nuke.sql

impdp.exe nbt_schema1/nbt@madeye DUMPFILE=NBT_MASTER_11G.dmp DIRECTORY=NBTDUMPS 

echo exit | sqlplus nbt_schema1/nbt@madeye @nbt_finalize_ora.sql


echo Schema3

echo exit | sqlplus nbt_schema2/nbt@madeye @nbt_nuke.sql

impdp.exe nbt_schema2/nbt@madeye DUMPFILE=NBT_MASTER_11G.dmp DIRECTORY=NBTDUMPS 

echo exit | sqlplus nbt_schema2/nbt@madeye @nbt_finalize_ora.sql

