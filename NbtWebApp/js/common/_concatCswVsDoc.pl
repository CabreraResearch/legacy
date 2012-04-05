use strict;

my $dir = $ARGV[0];
my $destfile = "$dir\\js\\CswCommon-vsdoc.js";

unlink($destfile);

my $js = "";
$js .= extractFile("$dir\\js\\_first-vsdoc.js");
$js .= extractFile("$dir\\js\\ChemSW.js");
$js .= extract("$dir\\js\\common\\composites");
$js .= extract("$dir\\js\\common\\controls");
$js .= extract("$dir\\js\\common\\layouts");
$js .= extract("$dir\\js\\common\\literals");
$js .= extract("$dir\\js\\common\\core");
$js .= extract("$dir\\js\\common\\events");
$js .= extract("$dir\\js\\common\\tools");
$js .= extract("$dir\\js\\common\\types");
$js .= extractFile("$dir\\js\\_last-vsdoc.js");

open(VSDOC, "> $destfile") or die("Cannot open vsdoc file: $destfile ; $!");
print VSDOC $js;
close(VSDOC); 

sub extract
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
    printf("Assembled $filename into CswCommon-vsdoc.js\n");
    close(JSFILE);
    return $ret;
}

printf("Finished generating CswCommon-vsdoc.js\n");