use strict;
use File::Copy;

sub checkContinue
{
	my $check;
	printf "Step complete.  Continue? (y/n): ";
	chomp($check = <>);
	if($check eq "n") {
	  die("User terminated build process.\n");
	}
	printf "\n";
}

sub runCommand
{
	printf("$_[0]\n");
#	my $result = `$_[0] 2>&1`;
	my $result = `$_[0]`;
	printf $result;
}

#---------------------------------------------------------------------------------
printf("Step 1: Pull and Update from Main\n");

&runCommand("hg pull -R D:/kiln/Common/CswCommon -u main -C");
&runCommand("hg pull -R D:/kiln/Common/CswConfigUI -u main -C");
&runCommand("hg pull -R D:/kiln/Common/CswWebControls -u main -C");
&runCommand("hg pull -R D:/kiln/Common/CswLogService -u main -C");
&runCommand("hg pull -R D:/kiln/nbt/nbt -u main -C");
&runCommand("hg pull -R D:/kiln/Install/nbt -u main -C");

&checkContinue;

#---------------------------------------------------------------------------------
printf("Step 2: Update Version Numbers\n");


open( VERSIONSFILE, "< ReleaseNBTversions.txt" ) or die( "Could not open ReleaseNBTVersions.txt" );
my %versions;
foreach my $vline (<VERSIONSFILE>) 
{
	my @vline = split /\s/, $vline;
	$versions{$vline[0]} = $vline[1];
}
my $assemblyno = $versions{"Assembly"};

			 
foreach my $component (keys %versions)
{
	my $version = $versions{$component};
	printf("Setting $component to $version\n");

	my $file;
	if($component eq "Assembly")
	{
		$file = "d:/kiln/Nbt/Nbt/NbtWebApp/_Assembly.txt";
		if(open( FOUT, "> $file" ) )
		{
			printf( FOUT $version );
			close( FOUT );
		} else {
			printf("ERROR: Could not open $file: $!\n");
		}
	}
	elsif($component eq "NbtWebApp")
	{
		$file = "d:/kiln/Nbt/Nbt/NbtWebApp/_Version.txt";
		if(open( FOUT, "> $file" ) )
		{
			printf( FOUT $version );
			close( FOUT );
		} else {
			printf("ERROR: Could not open $file: $!\n");
		}
	}
	else
	{
		if($component =~ /^Nbt/)
		{
			$file = "d:/kiln/Nbt/Nbt/$component/Properties/AssemblyInfo.cs";
		}
		elsif($component =~ /^CswLogService/)
		{
			$file = "d:/kiln/Common/$component/$component/Properties/AssemblyInfo.cs";
		}
		else
		{
			$file = "d:/kiln/Common/$component/Properties/AssemblyInfo.cs";
		}
		
		if(open( FIN, "< $file" ) )
		{
			if(open( FOUT, "> $file.new" ) )
			{
				foreach my $line (<FIN>) 
				{
					if($line =~ /\[assembly\: AssemblyVersion\( "(\d+\.\d+\.\d+\.\d+)" \)\]/ )
					{
						$line =~ s/$1/$version/g;
					}	
					elsif($line =~ /\[assembly\: AssemblyFileVersion\( "(\d+\.\d+\.\d+\.\d+)" \)\]/ )
					{
						$line =~ s/$1/$version/g;
					}	
					printf( FOUT $line );
				}
				close( FIN );
				close( FOUT );

				rename "$file.new", "$file" 
					or printf("ERROR: Could not rename $file.new to $file: $!\n");
			} else {
				printf("ERROR: Could not open $file.new: $!\n");
			}
		} else {
			printf("ERROR: Could not open $file: $!\n");
		}
	}
}

&checkContinue;

#---------------------------------------------------------------------------------
printf("Step 3: Compile\n");

&runCommand("d:\\kiln\\nbt\\nbt\\nbtwebapp\\js\\_compile.pl");

&runCommand("\"d:/Program Files (x86)/Microsoft Visual Studio 10.0/Common7/Tools/vsvars32.bat\" && devenv d:/kiln/nbt/nbt/Nbt.sln /Build \"Release\" /Project NbtSetup");

&checkContinue;

#---------------------------------------------------------------------------------
printf("Step 4: Copy install files to Install repository\n");

copy("d:/kiln/nbt/nbt/NbtSetup/Release/NbtSetup.msi", "d:/kiln/Install/nbt/NbtSetup.msi")
	or printf("Could not copy d:/kiln/nbt/nbt/NbtSetup/bin/Release/NbtSetup.msi to d:/kiln/Install/nbt/NbtSetup.msi: $!\n");

copy("d:/kiln/nbt/nbt/NbtSetup/Release/setup.exe", "d:/kiln/Install/nbt/setup.exe")
	or printf("Could not copy d:/kiln/nbt/nbt/NbtSetup/bin/Release/setup.exe to d:/kiln/Install/nbt/setup.exe: $!\n");

copy("d:/kiln/nbt/nbt/Schema/Dumps/Nbt_Master_11g.dmp", "d:/kiln/Install/Nbt/Schema/Nbt_Master_11g.dmp")
	or printf("Could not copy d:/kiln/nbt/nbt/Schema/Dumps/Nbt_Master_11g.dmp to d:/kiln/Install/Nbt/Schema/Nbt_Master_11g.dmp: $!\n");

copy("d:/kiln/nbt/nbt/Schema/Dumps/Nbt_Master_10g.dmp", "d:/kiln/Install/Nbt/Schema/Nbt_Master_10g.dmp")
	or printf("Could not copy d:/kiln/nbt/nbt/Schema/Dumps/Nbt_Master_10g.dmp to d:/kiln/Install/Nbt/Schema/Nbt_Master_10g.dmp: $!\n");

&checkContinue;

#---------------------------------------------------------------------------------
printf("Step 5: Commit, Label, and Push to Main\n");


&runCommand("hg commit -R d:/kiln/Common/CswCommon -m \"Automated commit for release: $assemblyno\"");
&runCommand("hg tag -R d:/kiln/Common/CswCommon \"CswCommon ".$versions{"CswCommon"}."\"");
&runCommand("hg push -R d:/kiln/Common/CswCommon main");

&runCommand("hg commit -R d:/kiln/Common/CswConfigUI -m \"Automated commit for release: $assemblyno\"");
&runCommand("hg tag -R d:/kiln/Common/CswConfigUI \"CswConfigUI ".$versions{"CswConfigUI"}."\"");
&runCommand("hg push -R d:/kiln/Common/CswConfigUI main");

&runCommand("hg commit -R d:/kiln/Common/CswWebControls -m \"Automated commit for release: $assemblyno\"");
&runCommand("hg tag -R d:/kiln/Common/CswWebControls \"CswWebControls ".$versions{"CswWebControls"}."\"");
&runCommand("hg push -R d:/kiln/Common/CswWebControls main");

&runCommand("hg commit -R d:/kiln/Common/CswLogService -m \"Automated commit for release: $assemblyno\"");
&runCommand("hg tag -R d:/kiln/Common/CswLogService \"CswLogService ".$versions{"CswLogService"}."\"");
&runCommand("hg push -R d:/kiln/Common/CswLogService main");

&runCommand("hg commit -R d:/kiln/nbt/nbt -m \"Automated commit for release: $assemblyno\"");
&runCommand("hg tag -R d:/kiln/nbt/nbt \"Nbt ".$versions{"NbtLogic"}."\"");
&runCommand("hg push -R d:/kiln/nbt/nbt main");

&runCommand("hg commit -R d:/kiln/Install/nbt -m \"Automated commit for release: $assemblyno\"");
&runCommand("hg tag -R d:/kiln/Install/nbt \"$assemblyno\"");
&runCommand("hg push -R d:/kiln/Install/nbt main");

#---------------------------------------------------------------------------------
printf("Build complete.\n");
