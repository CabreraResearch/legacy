del \\w2008x64db\kiln\Nbt\Nbt\Schema\Dumps\Nbt_11g.dmp
expdp.exe nbt/nbt@w2008x64db DUMPFILE=NBT_11g.dmp DIRECTORY=NBTDUMPS
del C:\Nbt_11g.7z
"C:\Program Files\7-zip\7z" a -t7z C:\Nbt.7z \\w2008x64db\kiln\Nbt\Nbt\Schema\Dumps\Nbt_11g.dmp