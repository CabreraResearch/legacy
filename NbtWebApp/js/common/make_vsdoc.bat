@echo off

ren > ChemSW-vsdoc.js 2> nul

for /r %%i in (*.js) DO call :concat %%i
goto :eof

:concat
type "%1" >> ChemSW-vsdoc.js;
goto :eof