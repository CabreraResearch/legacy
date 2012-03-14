
use strict;

use FindBin;
use lib $FindBin::Bin;
use cssVariables;
my %vars = %cssVariables::vars;

my $dir = $ARGV[0];
my $destfile = "$dir\\js\\ChemSW.min.css";

printf("Starting compile: css\n");

unlink($destfile);

my $param = "";
$param .= extract("$dir\\csw.css");

sub extract
{
    my $filelist = "";
    my $path = $_[0];
    opendir(CSSDIR, $path) or die("Cannot open css directory: $path ; $!");
    while((my $filename = readdir(CSSDIR)))
    {
        if($filename =~ /.*\.css$/ ) 
        {
            printf("Compiling: $path\\$filename\n");
            $filelist .= "--js $path\\$filename ";
            `java -jar "$dir\\..\\..\\..\\ThirdParty\\YUICompressor\\build\\yuicompressor-2.4.7.jar" $path\\$filename >> $destfile`;
        }
    }
    closedir(CSSDIR);
    return $filelist;
}


