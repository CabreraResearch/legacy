set /p ThisVersionNo=

DeployNbt.pl %ThisVersionNo% > d:\log\dailylog.txt 2>&1

echo "Deploy Finished"

@REM pause

"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\aspnet_compiler.exe" -v /NbtWebApp -p "d:\kiln\Nbt\Nbt\NbtWebApp"

echo "ASP Precompile Finished"

@REM pause     