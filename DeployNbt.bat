@REM set /p ThisVersionNo=

DeployNbt.pl 1 > C:\nbtlog\dailylog.txt 2>&1

echo "Deploy Finished"

@REM pause

"C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_compiler.exe" -v /NbtWebApp -p "c:\kiln\Nbt\Nbt\NbtWebApp"

echo "ASP Precompile Finished"

@REM pause