use strict;

my $dir = $ARGV[0];
my $destfile = "$dir\\js\\ChemSW.min.css";

unlink($destfile);

my $param = "";
$param .= extract("$dir\\csw.css");

sub extract
{
    my $filelist = "";
    my $path = $_[0];
    opendir(JSDIR, $path) or die("Cannot open css directory: $path ; $!");
    while((my $filename = readdir(JSDIR)))
    {
        if($filename =~ /.*\.css$/ ) 
        {
            printf("Compiling: $path\\$filename\n");
            $filelist .= "--js $path\\$filename ";
            `java -jar "$dir\\..\\..\\..\\ThirdParty\\YUICompressor\\build\\yuicompressor-2.4.7.jar" $path\\$filename >> $destfile`;
        }
    }
    closedir(JSDIR);
    return $filelist;
}

printf("Finished compiling css\n");
