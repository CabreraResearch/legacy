set /p ThisVersionNo=

net stop W3SVC

net stop "ChemSW Log Service" > D:\log\dailylog.txt

net stop "ChemSW NBT Schedule Service" >> D:\log\dailylog.txt

taskkill -f /IM "nbtschedservice.exe" >> D:\log\dailylog.txt

D:\kiln\nbt\nbt\Scripts\Daily\NbtDailyBuild.pl %ThisVersionNo% >> D:\log\dailylog.txt 2>&1

echo "Deploy Finished" >> D:\log\dailylog.txt

msbuild D:\kiln\Nbt\Nbt\Nbt.sln /p:Configuration=Release /p:Platform="x64" >> D:\log\dailylog.txt

msbuild D:\kiln\DailyBuildTools\DailyBuildWeb\DailyBuildWeb.sln /p:Configuration=Release >> D:\log\dailylog.txt

net start "ChemSW Log Service" >> D:\log\dailylog.txt

iisreset

D:\kiln\Nbt\Nbt\NbtSchemaUpdaterCmdLn\bin\Release\NbtUpdt.exe -all >> D:\log\dailylog.txt

net start "ChemSW NBT Schedule Service" >> D:\log\dailylog.txt

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\aspnet_compiler.exe -v /NbtWebApp -p d:\kiln\Nbt\Nbt\NbtWebApp >> D:\log\dailylog.txt

echo "ASP Precompile Finished" >> D:\log\dailylog.txt    