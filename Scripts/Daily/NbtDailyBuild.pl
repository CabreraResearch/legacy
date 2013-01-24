use strict;
use File::Copy;

#---------------------------------------------------------------------------------
# Arguments

my $increment = "1";
my $kilnpath = "C:\kiln";
if($#ARGV != 1)
{
	die( "Usage: DeployNbt.pl [increment] [kilnpath]\n" );
}
else
{
	$increment = $ARGV[0];
	$kilnpath = $ARGV[1];
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
	"StructureSearch",
    "incandescentsw"
);

my %repopaths;
foreach my $component (@components)
{
	if($component eq "NbtHelp")
	{
		$repopaths{$component} = "$kilnpath/Nbt/Nbt/NbtWebApp/help";
	}
	elsif($component eq "Nbt" || $component eq "NbtImport")
	{
		$repopaths{$component} = "$kilnpath/Nbt/$component";
	}
	elsif($component eq "DailyBuildTools")
	{
		$repopaths{$component} = "$kilnpath/$component";
	} 
    elsif($component eq "incandescentsw")
	{
		$repopaths{$component} = "$kilnpath/incandescentsw/chemsw-fe";
	} 
	else 
	{
		$repopaths{$component} = "$kilnpath/Common/$component";
	}
}


#---------------------------------------------------------------------------------

my ($sec,$min,$hour,$mday,$mon,$year,$wday,$yday,$isdst) = localtime(time);
printf "%4d-%02d-%02d %02d:%02d:%02d\n", $year+1900, $mon+1, $mday, $hour, $min, $sec;

#---------------------------------------------------------------------------------
# 1. pull from Main

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
					   $subdir eq "Scripts" ||
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
		elsif( $component eq "incandescentsw" ) 
		{
			$file = $repopaths{$component} ."/_Assembly.txt";
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
			if($component eq "CswLogService")  # special case
			{
				$file = $repopaths{$component} ."/CswLogService/Properties/AssemblyInfo.cs";
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
# 3. tags

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