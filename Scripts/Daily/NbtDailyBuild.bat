@ECHO OFF
setlocal

set LOCALCONF=%~n0_conf.cmd
IF EXIST %LOCALCONF% goto ConfigContinue

REM Generate default setting file
>%LOCALCONF% echo REM Configuration File
>>%LOCALCONF% echo set KilnPath=D:\kiln
>>%LOCALCONF% echo set LogFile=D:\log\dailylog.txt
>>%LOCALCONF% echo set SchedServiceName=ChemSW NBT Schedule Service
>>%LOCALCONF% echo set ResetSchemaUsername=nbt_master
>>%LOCALCONF% echo set ResetSchemaPassword=hj345defwu9
>>%LOCALCONF% echo set ResetSchemaServer=nbttest

echo #
echo # Local configuration not yet set.
echo # A default configuration file (%LOCALCONF%) has been created.
echo # Review and edit this file, then run this process again.
echo #
goto End


:ConfigContinue
call %LOCALCONF%

IF "%2"=="" GOTO Usage
GOTO Run


:Usage
echo # 
echo # Invalid Arguments
echo # Usage: %0 [VersionNo] [ResetSchema]
echo # Example: %0 1 Y
echo #   [VersionNo]: Number
echo #         Final number in the build tag (e.g. the 3 in 2012.9.5.3).
echo #   [ResetSchema]: Y/N
echo #         Whether to reset the schema specified in config back to master
echo # 
GOTO End


:Run
set VersionNo=%1
set ResetSchema=%2

@ECHO ON

> %LogFile% echo ====================================================================
>>%LogFile% echo Starting Run
>>%LogFile% date /T
>>%LogFile% time /T

>>%LogFile% net stop W3SVC
>>%LogFile% net stop "ChemSW Log Service"
>>%LogFile% net stop "%SchedServiceName%"
REM We can't kill other sched services, so just wait for this one to end
REM >>%LogFile% taskkill -f /IM "nbtschedservice.exe"
timeout /T 30

>>%LogFile% echo ====================================================================
>>%LogFile% echo Starting Deploy
>>%LogFile% date /T
>>%LogFile% time /T

>>%LogFile% 2>&1 %KilnPath%\nbt\nbt\Scripts\Daily\NbtDailyBuild.pl %VersionNo% %KilnPath%

>>%LogFile% echo ====================================================================
>>%LogFile% echo Starting Build
>>%LogFile% date /T
>>%LogFile% time /T

>>%LogFile% msbuild %KilnPath%\Documentation\Documentation.sln /p:Configuration=Debug
>>%LogFile% msbuild %KilnPath%\Nbt\Nbt\Nbt.sln /p:Configuration=Release /p:Platform="x64"
>>%LogFile% msbuild %KilnPath%\DailyBuildTools\DailyBuildWeb\DailyBuildWeb.sln /p:Configuration=Release
>>%LogFile% net start "ChemSW Log Service"


:SchemaReset
IF "%ResetSchema%" NEQ "Y" GOTO Continue
>>%LogFile% echo ====================================================================
>>%LogFile% echo Starting Master Schema Reset
>>%LogFile% date /T
>>%LogFile% time /T

REM must reset nbt_master before schemaupdater runs
exit | >>%LogFile% sqlplus %ResetSchemaUsername%/%ResetSchemaPassword%@%ResetSchemaServer% @%KilnPath%\nbt\Nbt\Schema\nbt_nuke.sql
>>%LogFile% 2>&1 impdp.exe %ResetSchemaUsername%/%ResetSchemaPassword%@%ResetSchemaServer% DUMPFILE=NBT_MASTER_11G.DMP DIRECTORY=EXPORTS REMAP_SCHEMA=nbt_master:%ResetSchemaUsername% NOLOGFILE=Y


:Continue
>>%LogFile% echo ====================================================================
>>%LogFile% echo Starting Schema Updater
>>%LogFile% date /T
>>%LogFile% time /T

>>%LogFile% %KilnPath%\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Release\NbtUpdt.exe -all


>>%LogFile% echo ====================================================================
>>%LogFile% echo Starting Services
>>%LogFile% date /T
>>%LogFile% time /T

>>%LogFile% net start W3SVC
>>%LogFile% iisreset
>>%LogFile% net start "%SchedServiceName%"
>>%LogFile% C:\Windows\Microsoft.NET\Framework64\v4.0.30319\aspnet_compiler.exe -v /NbtWebApp -p %KilnPath%\Nbt\Nbt\NbtWebApp


>>%LogFile% echo ====================================================================
>>%LogFile% echo Finished Build 
>>%LogFile% date /T
>>%LogFile% time /T


:End
endlocal
