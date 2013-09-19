echo exit | sqlplus caf2/caf2@w2008x64Db @caf.sql

rem | apply triggers to tables

for /f %%a in ('dir /b ^"triggers\') do ( 
    echo Applying %%a to database...
	echo.
	sqlplus -s -l caf2/caf2@w2008x64Db @"triggers\%%a"
	echo.
	echo.
)

rem | fill nbtimportqueue

for /f %%a in ('dir /b ^"importqueue\') do (
    echo Applying %%a to database...
	echo.
	sqlplus -s -l caf2/caf2@w2008x64Db @"importqueue\%%a"
	echo.
	echo.  
)