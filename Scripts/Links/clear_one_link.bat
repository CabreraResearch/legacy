@ECHO OFF
setlocal

IF "%2"=="" GOTO Usage
GOTO Run

:Usage
echo # 
echo # Invalid Argument
echo # Usage: %0 [KilnPath] [EtcPath]
echo # Example: %0 C:\kiln \Nbt\Nbt\NbtSchedService\bin
echo #   [KilnPath]: The local absolute root of Kiln (e.g., "C:\kiln" )
echo #   [EtcPath]: Relative directory in which to make etc dir and links
echo # 
GOTO End

:Run

echo %1%2
cd %1%2
IF EXIST etc\CswDbCfgInfo.xml del etc\CswDbCfgInfo.xml
IF EXIST etc\CswSetupVariables.json del etc\CswSetupVariables.json
IF EXIST etc rmdir etc

:End
