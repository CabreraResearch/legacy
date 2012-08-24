use strict;

my $dir = $ARGV[0];
my $destfile = "$dir\\mobile\\CswAllMobile.min.js";

unlink($destfile);

my $param = "";

$param .= extract("$dir\\mobile\\globals");
$param .= extract("$dir\\mobile\\tools");
$param .= extract("$dir\\mobile");
$param .= extract("$dir\\mobile\\clientdb");
$param .= extract("$dir\\mobile\\controls");
$param .= extract("$dir\\mobile\\fieldtypes");
$param .= extract("$dir\\mobile\\objectclasses");
$param .= extract("$dir\\mobile\\pages");
$param .= extract("$dir\\mobile\\sync");

`java -jar "$dir\\..\\..\\..\\ThirdParty\\ClosureCompiler\\compiler.jar" $param --js_output_file $destfile`;

sub extract
{
    my $filelist = "";
    my $path = $_[0];
    opendir(JSDIR, $path) or die("Cannot open js directory: $path ; $!");
    while((my $filename = readdir(JSDIR)))
    {
        if($filename =~ /.*\.js$/ &&
           $filename !~ /-vsdoc/ &&
           $filename !~ /.min./ ) 
        {
            $filelist .= "--js $path\\$filename ";
        }
    }
    closedir(JSDIR);
    return $filelist;
}

printf("Finished compiling CswAllMobile.min.js javascript\n");