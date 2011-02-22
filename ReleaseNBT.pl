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
	my $result = `$_[0] 2>&1`;
	printf $result;
}

#---------------------------------------------------------------------------------
printf("Step 1: Pull and Update from Main\n");

&runCommand("hg pull -R c:/kiln/Common/CswCommon -u main");
&runCommand("hg pull -R c:/kiln/Common/CswConfigUI -u main");
&runCommand("hg pull -R c:/kiln/Common/CswWebControls -u main");
&runCommand("hg pull -R c:/kiln/nbt/nbt -u main");
&runCommand("hg pull -R c:/kiln/Install/nbt -u main");

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
		$file = "c:/kiln/Nbt/Nbt/NbtWebApp/_Assembly.txt";
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
		$file = "c:/kiln/Nbt/Nbt/NbtWebApp/_Version.txt";
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
			$file = "c:/kiln/Nbt/Nbt/$component/Properties/AssemblyInfo.cs";
		}
		else
		{
			$file = "c:/kiln/Common/$component/Properties/AssemblyInfo.cs";
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

&runCommand("c:\kiln\nbt\nbt\nbtwebapp\js\_compile.pl");

&runCommand("\"c:/Program Files (x86)/Microsoft Visual Studio 10.0/Common7/Tools/vsvars32.bat\" && devenv c:/kiln/nbt/nbt/Nbt.sln /Build \"Release\" /Project NbtSetup");

&checkContinue;

#---------------------------------------------------------------------------------
printf("Step 4: Copy install files to Install repository\n");

copy("c:/kiln/nbt/nbt/NbtSetup/Release/NbtSetup.msi", "c:/kiln/Install/nbt/NbtSetup.msi")
	or printf("Could not copy c:/kiln/nbt/nbt/NbtSetup/bin/Release/NbtSetup.msi to c:/kiln/Install/nbt/NbtSetup.msi: $!\n");

copy("c:/kiln/nbt/nbt/NbtSetup/Release/setup.exe", "c:/kiln/Install/nbt/setup.exe")
	or printf("Could not copy c:/kiln/nbt/nbt/NbtSetup/bin/Release/setup.exe to c:/kiln/Install/nbt/setup.exe: $!\n");

copy("c:/kiln/nbt/nbt/Schema/Dumps/Nbt_Master_11g.dmp", "c:/kiln/Install/Nbt/Schema/Nbt_Master_11g.dmp")
	or printf("Could not copy c:/kiln/nbt/nbt/Schema/Dumps/Nbt_Master_11g.dmp to c:/kiln/Install/Nbt/Schema/Nbt_Master_11g.dmp: $!\n");

copy("c:/kiln/nbt/nbt/Schema/Dumps/Nbt_Master_10g.dmp", "c:/kiln/Install/Nbt/Schema/Nbt_Master_10g.dmp")
	or printf("Could not copy c:/kiln/nbt/nbt/Schema/Dumps/Nbt_Master_10g.dmp to c:/kiln/Install/Nbt/Schema/Nbt_Master_10g.dmp: $!\n");

&checkContinue;

#---------------------------------------------------------------------------------
printf("Step 5: Commit, Label, and Push to Main\n");


&runCommand("hg commit -R c:/kiln/Common/CswCommon -m \"Automated commit for release: $assemblyno\"");
&runCommand("hg tag -R c:/kiln/Common/CswCommon \"CswCommon ".$versions{"CswCommon"}."\"");
&runCommand("hg push -R c:/kiln/Common/CswCommon main");

&runCommand("hg commit -R c:/kiln/Common/CswConfigUI -m \"Automated commit for release: $assemblyno\"");
&runCommand("hg tag -R c:/kiln/Common/CswConfigUI \"CswConfigUI ".$versions{"CswConfigUI"}."\"");
&runCommand("hg push -R c:/kiln/Common/CswConfigUI main");

&runCommand("hg commit -R c:/kiln/Common/CswWebControls -m \"Automated commit for release: $assemblyno\"");
&runCommand("hg tag -R c:/kiln/Common/CswWebControls \"CswWebControls ".$versions{"CswWebControls"}."\"");
&runCommand("hg push -R c:/kiln/Common/CswWebControls main");

&runCommand("hg commit -R c:/kiln/nbt/nbt -m \"Automated commit for release: $assemblyno\"");
&runCommand("hg tag -R c:/kiln/nbt/nbt \"Nbt ".$versions{"NbtLogic"}."\"");
&runCommand("hg push -R c:/kiln/nbt/nbt main");

&runCommand("hg commit -R c:/kiln/Install/nbt -m \"Automated commit for release: $assemblyno\"");
&runCommand("hg tag -R c:/kiln/Install/nbt \"$assemblyno\"");
&runCommand("hg push -R c:/kiln/Install/nbt main");

#---------------------------------------------------------------------------------
printf("Build complete.\n");
