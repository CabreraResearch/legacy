@ECHO OFF
setlocal

IF "%1"=="" GOTO Usage
GOTO Run

:Usage
echo # 
echo # Invalid Argument
echo # Usage: %0 [KilnPath]
echo # Example: %0 C:\kiln
echo #   [KilnPath]: The local absolute root of Kiln (e.g., "C:\kiln" )
echo # 
GOTO End

:Run

call %1\Nbt\Nbt\Scripts\Links\make_one_link %1 \Nbt\Nbt\bin
call %1\Nbt\Nbt\Scripts\Links\make_one_link %1 \Nbt\Nbt\NbtWebApp

:End
