use strict;

my ($sec,$min,$hour,$mday,$mon,$year,$wday,$yday,$isdst) = localtime(time);
my $datestr = ($year + 1900).".". ($mon + 1) .".$mday";

my $dir = $ARGV[0];

my $destfile = "$dir\\CswCommon.$datestr.min.js";
my $vsdocfile = "$dir\\js\\CswCommon-vsdoc.js";

unlink($destfile);
unlink($vsdocfile);

my $param = "";
$param .= "--js $dir\\js\\ChemSW.js ";
$param .= extractPath("$dir\\js\\common");
$param .= extractPath("$dir\\js\\common\\composites");
$param .= extractPath("$dir\\js\\common\\controls");
$param .= extractPath("$dir\\js\\common\\core");
$param .= extractPath("$dir\\js\\common\\events");
$param .= extractPath("$dir\\js\\common\\layouts");
$param .= extractPath("$dir\\js\\common\\literals");
$param .= extractPath("$dir\\js\\common\\tools");
$param .= extractPath("$dir\\js\\common\\types");

`java -jar "$dir\\..\\..\\..\\ThirdParty\\ClosureCompiler\\compiler.jar" $param  --create_source_map $dir/js/common.js.map --source_map_format=V3 --js_output_file $destfile`;

my $vsparam = "";
$vsparam .= extractFile("$dir\\js\\_first-vsdoc.js");
$vsparam .= extractFile("$dir\\js\\ChemSW.js");
$vsparam .= extractContent("$dir\\js\\common");
$vsparam .= extractContent("$dir\\js\\common\\composites");
$vsparam .= extractContent("$dir\\js\\common\\controls");
$vsparam .= extractContent("$dir\\js\\common\\core");
$vsparam .= extractContent("$dir\\js\\common\\events");
$vsparam .= extractContent("$dir\\js\\common\\layouts");
$vsparam .= extractContent("$dir\\js\\common\\literals");
$vsparam .= extractContent("$dir\\js\\common\\tools");
$vsparam .= extractContent("$dir\\js\\common\\types");
$vsparam .= extractFile("$dir\\js\\_last-vsdoc.js");

open(VSDOC, "> $vsdocfile") or die("Cannot open vsdoc file: $vsdocfile ; $!");
print VSDOC $vsparam;
close(VSDOC); 

sub extractPath
{
    my $filelist = "";
    my $path = $_[0];
    opendir(JSDIR, $path) or die("Cannot open js directory: $path ; $!");
    while((my $filename = readdir(JSDIR)))
    {
        if($filename =~ /.*\.js$/ &&
           $filename !~ /-vsdoc/ &&
           $filename !~ /_first/ &&
           $filename !~ /_last/ &&
           $filename !~ /.min\.js/) 
        {
            $filelist .= "--js $path\\$filename ";
        }
    }
    closedir(JSDIR);
    return $filelist;
}

sub extractContent
{
    my $ret = "";
    my $path = $_[0];
    opendir(JSDIR, $path) or die("Cannot open js directory: $path ; $!");
    while((my $filename = readdir(JSDIR)))
    {
        if($filename =~ /.*\.js$/ &&
           $filename !~ /-vsdoc/ &&
           $filename !~ /_first/ &&
           $filename !~ /_last/ &&
           $filename !~ /.min\.js/) 
        {
            $ret .= extractFile("$path\\$filename");
        }
    }
    closedir(JSDIR);
    return $ret;
}

sub extractFile
{
    my $ret = "";
    my $filename = $_[0];
    open(JSFILE, "$filename") or die("Cannot open js file: $filename ; $!");
    while((my $line = <JSFILE>)) 
    {
        if($line !~ m/-vsdoc\.js/ )
        {
            $ret .= $line;         
        }
    }
    close(JSFILE);
    return $ret;
}

open(JSFILE, ">>$destfile") or die("Cannot open js directory: $destfile; $!");
print JSFILE "//@ sourceMappingURL=js/common.js.map";
close (JSFILE);

printf("Finished compiling CswCommon.$datestr.min.js javascript\n");