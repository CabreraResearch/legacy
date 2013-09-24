@ECHO OFF
setlocal EnableDelayedExpansion

rem | count the number of arguments passed in to the script
set argC=0
for %%x in (%*) do Set /A argC+=1


rem | if the count of arguments does not match 3, echo usage text
if NOT %argC% == 3 (
  echo.
  echo Usage: connect_to_nbt.bat ^<Username^> ^<Password^> ^<Database^>
  echo.
  goto end
)

set NbtUser=%1
set NbtPass=%2
set NbtDatabase=%3

echo exit | sqlplus %NbtUser%/%NbtPass%@%NbtDatabase% as sysdba @nbt.sql