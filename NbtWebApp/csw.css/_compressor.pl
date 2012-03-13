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

printf("Finished compiling css\n");

printf("Running CSS postprocessor\n");

open(CSS, $destfile) or die("Could not open file: $destfile ; $!");
my $processedfile = "";
while(my $line = <CSS>)
{
    while((my $key, my $value) = each %vars) 
    {
        $line =~ s/$key/$value/g;
    }
    $processedfile .= $line;
}
close(CSS);

open(CSS2, "> $destfile") or die("Could not open file: $destfile ; $!");
print CSS2 $processedfile;
close(CSS2);

printf("Finished running CSS postprocessor\n");
