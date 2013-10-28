@ECHO OFF
setlocal EnableDelayedExpansion

rem | count the number of arguments passed in to the script
set argC=0
for %%x in (%*) do Set /A argC+=1


rem | if the count of arguments does not match 3, echo usage text
if NOT %argC% == 3 (
  echo.
  echo Usage: connect_to_caf.bat ^<Username^> ^<Password^> ^<Database^>
  echo.
  goto end
)

set CAFUser=%1
set CAFPass=%2
set CAFDatabase=%3

echo exit | sqlplus %CAFUser%/%CAFPass%@%CAFDatabase% @caf.sql
