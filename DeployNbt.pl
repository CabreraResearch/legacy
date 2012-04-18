use strict;
use File::Copy;

#---------------------------------------------------------------------------------
# Arguments

my $increment = "1";
if($#ARGV > 0)
{
	die( "Usage: DeployNbt.pl [increment]\n" );
}
elsif($#ARGV == 0)
{
	$increment = $ARGV[0];
}

#---------------------------------------------------------------------------------
# Manually configured information

my @components = (
	"CswCommon", 
	"CswConfigUI", 
	"CswWebControls", 
	"CswLogService", 
	"Nbt",
	"NbtImport",
	"NbtHelp",
	"DailyBuildTools"
);

my $orclserver = "golem";
my $orcldumpdir = "ChemSWDumpDirectory";
my $masterdumpdir = "ChemSWDumpDirectory";

my %repopaths;
foreach my $component (@components)
{
	if($component eq "NbtHelp")
	{
		$repopaths{$component} = "d:/kiln/Nbt/Nbt/NbtWebApp/help";
	}
	elsif($component eq "Nbt" || $component eq "NbtImport")
	{
		$repopaths{$component} = "d:/kiln/Nbt/$component";
	}
	elsif($component eq "DailyBuildTools")
	{
		$repopaths{$component} = "d:/kiln/$component";
	} else {
		$repopaths{$component} = "d:/kiln/Common/$component";
	}
}


#---------------------------------------------------------------------------------

my ($sec,$min,$hour,$mday,$mon,$year,$wday,$yday,$isdst) = localtime(time);
printf "%4d-%02d-%02d %02d:%02d:%02d\n", $year+1900, $mon+1, $mday, $hour, $min, $sec;

#---------------------------------------------------------------------------------
# 1. pull from Main

&runCommand( "net stop \"ChemSW Log Service\"");

&runCommand( "net stop \"ChemSW NBT Schedule Service\"");

&runCommand( "taskkill -f /IM nbtschedservice.exe" );

foreach my $component (@components)
{
	&runCommand("hg pull -u -R ". $repopaths{$component});
}

#---------------------------------------------------------------------------------
# 2. update versions

my $datestr = ($year + 1900).".". ($mon + 1) .".$mday";

open( ASSEMBLYFILE, "< ". $repopaths{"Nbt"} ."/NbtWebApp/_Assembly.txt" ) 
	or die( "Could not open _Assembly.txt" );
my $assemblyname = <ASSEMBLYFILE>;
close( ASSEMBLYFILE );
$assemblyname =~ /^(\w+?)_.*$/;
my $releasename = $1;
my $assemblyno = $releasename ."_". $datestr .".". $increment; 

foreach my $component (@components)
{
	printf("Setting $component to $datestr.$increment\n");
	
	my $file;
	if($component eq "NbtImport" || $component eq "NbtHelp")
	{
		# no file to update
	}
	else 
	{
		if($component eq "Nbt")
		{
			# get NBT sub-components:
			my $dir = $repopaths{$component};
			opendir(my $dh, $dir) || die "can't opendir $dir: $!";
			my @subdirs = grep { -d "$dir/$_" } readdir($dh);
			foreach my $subdir (@subdirs)
			{
				if( ! ($subdir eq "." || 
					   $subdir eq ".." ||
					   $subdir eq ".hg" ||
					   $subdir eq "NbtSetup" ||
					   $subdir eq "Schema" ||
					   $subdir eq "TestApps" ||
					   $subdir eq "packages"
					   ))
				{
					if($subdir eq "NbtWebApp")   # special case
					{
						$file = $repopaths{$component} ."/NbtWebApp/_Version.txt";
						if(open( FOUT, "> $file" ) )
						{
							printf( FOUT "$component $datestr.$increment" );
							close( FOUT );
						} else {
							printf("ERROR: Could not open $file \n");
						}
						$file = $repopaths{$component} ."/NbtWebApp/_Assembly.txt";
						if(open( FOUT, "> $file" ) )
						{
							printf( FOUT "$assemblyno" );
							close( FOUT );
						} else {
							printf("ERROR: Could not open $file \n");
						}
					} 
					else 
					{
						$file = $repopaths{$component} ."/$subdir/Properties/AssemblyInfo.cs";
						&setversion($file, "$datestr.$increment");
					}
				}
			}
			closedir $dh;
		}
		else
		{
			if($component eq "CswLogService")  # special case
			{
				$file = $repopaths{$component} ."/CswLogService/Properties/AssemblyInfo.cs";
			}
			elsif($component eq "DailyBuildTools")   # special case
			{
				$file = $repopaths{$component} ."/DailyBuildWeb/DailyBuildWeb/Properties/AssemblyInfo.cs";
				&setversion($file, "$datestr.$increment");
			} 
			else
			{
				$file = $repopaths{$component} ."/Properties/AssemblyInfo.cs";
			}
			&setversion($file, "$datestr.$increment");
		}
	}
}  # foreach my $component (@components)

#---------------------------------------------------------------------------------
# 3. compile


&runCommand( "taskkill /F /IM NbtSchedService.exe");  # force kill outstanding threads

#&runCommand( $repopaths{"Nbt"} ."/nbtwebapp/js/_compile.pl");

&runCommand("\"c:/Program Files (x86)/Microsoft Visual Studio 10.0/Common7/Tools/vsvars32.bat\" && ".
            "devenv ". $repopaths{"Nbt"} ."/Nbt.sln /Rebuild \"Release\"");

&runCommand("\"c:/Program Files (x86)/Microsoft Visual Studio 10.0/Common7/Tools/vsvars32.bat\" && ".
            "devenv ". $repopaths{"DailyBuildTools"} ."/DailyBuildweb/DailyBuildWeb.sln /Rebuild \"Release\"");

&runCommand( "net start \"ChemSW Log Service\"");

#---------------------------------------------------------------------------------
# 4. tags

foreach my $component (@components)
{
	my $path = $repopaths{$component};
	&runCommand("hg commit -R $path -m \"Automated commit for release: $assemblyno\"");
      # &runCommand("hg tag -R $path \"$component $datestr.$increment\"");
	&runCommand("hg tag -R $path \"$assemblyno\"");
	&runCommand("hg push -R $path");
}



#---------------------------------------------------------------------------------
# subroutines

sub setversion
{
	my $file = $_[0];
	my $version = $_[1];
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

sub runCommand
{
	printf("$_[0]\n");
#	my $result = `$_[0] 2>&1`;
	my $result = `$_[0]`;
	printf $result;
}


#---------------------------------------------------------------------------------
# 5. run command line schema updater

&runCommand( $repopaths{"Nbt"} ."/NbtSchemaUpdaterCmdLn/bin/Release/NbtUpdt.exe -all");


#---------------------------------------------------------------------------------
# 6. start schedule service

&runCommand( "net start \"ChemSW NBT Schedule Service\"");

