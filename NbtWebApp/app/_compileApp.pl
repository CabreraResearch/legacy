use strict;

my ($sec,$min,$hour,$mday,$mon,$year,$wday,$yday,$isdst) = localtime(time);
my $datestr = ($year + 1900).".". ($mon + 1) .".$mday";

my $dir = $ARGV[0];

my $destfile = "$dir\\CswApp.$datestr.min.js";
my $vsdocfile = "$dir\\app\\CswApp-vsdoc.js";

unlink($destfile);
unlink($vsdocfile);

my $param = "";
$param .= "--js $dir\\app\\ChemSW.js ";
$param .= extractPath("$dir\\app\\types");
$param .= extractPath("$dir\\app\\core");
$param .= extractPath("$dir\\app\\tools");
$param .= extractPath("$dir\\app\\actions");
$param .= extractPath("$dir\\app\\composites");
$param .= extractPath("$dir\\app\\controls");
$param .= extractPath("$dir\\app\\events");
$param .= extractPath("$dir\\app\\layouts");
$param .= extractPath("$dir\\app\\literals");
$param .= extractPath("$dir\\app\\nodes");
$param .= extractPath("$dir\\app\\pagecmp");
$param .= extractPath("$dir\\app\\proptypes");
$param .= extractPath("$dir\\app\\view");
$param .= extractPath("$dir\\app\\wizards");
$param .= "--js $dir\\app\\Main.js ";

`java -jar "$dir\\..\\..\\..\\ThirdParty\\ClosureCompiler\\compiler.jar" $param --compilation_level=WHITESPACE_ONLY --language_in=ECMASCRIPT5 --js_output_file $destfile`;

my $vsparam = "";
$vsparam .= extractFile("$dir\\app\\_first-vsdoc.js");
$vsparam .= extractFile("$dir\\app\\ChemSW.js");
$vsparam .= extractContent("$dir\\app\\actions");
$vsparam .= extractContent("$dir\\app\\composites");
$vsparam .= extractContent("$dir\\app\\controls");
$vsparam .= extractContent("$dir\\app\\core");
$vsparam .= extractContent("$dir\\app\\events");
$vsparam .= extractContent("$dir\\app\\layouts");
$vsparam .= extractContent("$dir\\app\\literals");
$vsparam .= extractContent("$dir\\app\\nodes");
$vsparam .= extractContent("$dir\\app\\pagecmp");
$vsparam .= extractContent("$dir\\app\\proptypes");
$vsparam .= extractContent("$dir\\app\\tools");
$vsparam .= extractContent("$dir\\app\\types");
$vsparam .= extractContent("$dir\\app\\view");
$vsparam .= extractContent("$dir\\app\\wizards");
$vsparam .= extractFile("$dir\\app\\Main.js");
$vsparam .= extractFile("$dir\\app\\_last-vsdoc.js");

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
print JSFILE "//@ sourceMappingURL=app/app.js.map";
close (JSFILE);

printf("Finished compiling CswApp.$datestr.min.js javascript\n");