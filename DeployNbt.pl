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
	"Nbt"
);

my $orclserver = "madeye";
my $orcldumpdir = "MADEYE_DUMPS";
my $masterdumpdir = "NBTDUMPS";

my %schemata;  
# $schemata{name} = password
$schemata{"nbt_master"} = "nbt";

# this one will always be reset to the master
my $masterschema = "nbt_master";

my %repopaths;
foreach my $component (@components)
{
	if($component eq "Nbt")
	{
		$repopaths{$component} = "c:/kiln/$component/$component";
	} else {
		$repopaths{$component} = "c:/kiln/Common/$component";
	}
}

#---------------------------------------------------------------------------------
# 1. pull from Main

foreach my $component (@components)
{
	&runCommand("hg pull -u -R ". $repopaths{$component});
}

#---------------------------------------------------------------------------------
# 2. update versions

my ($sec,$min,$hour,$mday,$mon,$year,$wday,$yday,$isdst) = localtime(time);
my $datestr = ($year + 1900).".". ($mon + 1) .".$mday";

open( ASSEMBLYFILE, "< ". $repopaths{"Nbt"} ."/NbtWebApp/_Assembly.txt" ) 
	or die( "Could not open _Assembly.txt" );
my $assemblyname = <ASSEMBLYFILE>;
close( ASSEMBLYFILE );

my $assemblyno = "$assemblyname $datestr.$increment"; 

foreach my $component (@components)
{
	printf("Setting $component to $datestr.$increment\n");
	
	my $file;
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
				   $subdir eq "TestApps"))
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
				} else {
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
		else
		{
			$file = $repopaths{$component} ."/Properties/AssemblyInfo.cs";
		}
		&setversion($file, "$datestr.$increment");
	}
}  # foreach my $component (@components)

#---------------------------------------------------------------------------------
# 3. compile

&runCommand( "net stop \"ChemSW Log Service\"");

&runCommand( $repopaths{"Nbt"} ."/nbtwebapp/js/_compile.pl");

&runCommand("\"c:/Program Files (x86)/Microsoft Visual Studio 10.0/Common7/Tools/vsvars32.bat\" && ".
            "devenv ". $repopaths{"Nbt"} ."/Nbt.sln /Build \"Release\"");

&runCommand( "net start \"ChemSW Log Service\"");

#---------------------------------------------------------------------------------
# 4. back up existing schemata

foreach my $schema (keys %schemata)
{
	my $password = $schemata{$schema};
	&runCommand( "C:/app/client64/product/11.2.0/client_1/BIN/expdp.exe ". $schema ."/". $password ."\@". $orclserver ." DUMPFILE=". $schema ."_". $datestr .".". $increment .".dmp DIRECTORY=". $orcldumpdir );
}

#---------------------------------------------------------------------------------
# 5. reset master schema

my $masterpassword = $schemata{$masterschema};

&runCommand( "echo exit | sqlplus ". $masterschema ."/". $masterpassword ."\@". $orclserver ." \@". $repopaths{"Nbt"} ."/Schema/nbt_nuke.sql" );

&runCommand( "impdp.exe ". $masterschema ."/". $masterpassword ."@". $orclserver ." DUMPFILE=NBT_MASTER_11G.dmp DIRECTORY=". $masterdumpdir );

&runCommand( "echo exit | sqlplus ". $masterschema ."/". $masterpassword ."\@". $orclserver ." \@". $repopaths{"Nbt"} ."/Schema/nbt_finalize_ora.sql" );

#---------------------------------------------------------------------------------
# 6. run command line schema updater

&runCommand( $repopaths{"Nbt"} ."/NbtSchemaUpdaterCmdLn/bin/Release/NbtUpdt.exe -all");

#---------------------------------------------------------------------------------
# 7. tags

foreach my $component (@components)
{
	my $path = $repopaths{$component};
	&runCommand("hg commit -R $path -m \"Automated commit for release: $assemblyno\"");
	&runCommand("hg tag -R $path \"$component $datestr.$increment\"");
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