use strict;

my ($sec,$min,$hour,$mday,$mon,$year,$wday,$yday,$isdst) = localtime(time);
my $datestr = ($year + 1900).".". ($mon + 1) .".$mday";

printf("Started compiling ChemSW.$datestr.min.css\n");

use FindBin;
use lib $FindBin::Bin;
use cssVariables;
my %vars = %cssVariables::vars;

my $dir = $ARGV[0];
my $destfile = "$dir\\css\\ChemSW.$datestr.min.css";
unlink($destfile);

open(CSSDOC, "> $destfile") or die("Cannot open vsdoc file: $destfile ; $!");
close(CSSDOC);

my $path = "$dir\\css\\";

extractFile("ChemSW.css");
extractFiles();

printf("Finished compiling ChemSW.$datestr.min.css\n");

printf("Running CSS postprocessor\n");

postProcessor("ChemSW.$datestr.min.css", "ChemSW.$datestr.min.css");

printf("Finished running CSS postprocessor\n");

sub extractFiles
{
    my $filelist = "";
    opendir(CSSDIR, $path) or die("Cannot open css directory: $path ; $!");
    while((my $filename = readdir(CSSDIR)))
    {
        extractFile($filename);
    }
    closedir(CSSDIR);
    return $filelist;
}

sub extractFile
{
    my $filename = $_[0];
    if($filename !~ /_dev/ &&
       $filename =~ /.*\.css$/ &&
       $filename !~ /.min\.css/) 
    {
        open(CSSFILE, "$path\\$filename") or die("Cannot open css file: $filename ; $!");        
        `java -jar "$dir\\..\\..\\..\\ThirdParty\\YUICompressor\\build\\yuicompressor-2.4.7.jar" $path\\$filename >> $destfile`;
        postProcessor($filename, "_dev.$filename");
        close(CSSFILE);
    }
    return $filename;
}

sub postProcessor
{
    my $filename = $_[0];
    my $destfile = $_[1];

    if($filename =~ /.*\.css$/) 
    {    
        open(CSS1, "$path\\$filename") or die("Could not open file: $filename ; $!");
        my $processedfile = "";
        while(my $line = <CSS1>)
        {
            while((my $key, my $value) = each %vars) 
            {
                $line =~ s/$key/$value/g;
            }
            $processedfile .= $line;
        }
        close(CSS1);

        open(CSS2, "> $path\\$destfile") or die("Could not open file: $destfile ; $!");
        print CSS2 $processedfile;
        close(CSS2);
    }
}