use strict;

my $dir = $ARGV[0];
my $destfile = "$dir\\app\\CswApp-vsdoc.js";

unlink($destfile);

my $js = "";
$js .= extractFile("$dir\\app\\_first-vsdoc.js");
$js .= extractFile("$dir\\app\\Main.js");
$js .= extract("$dir\\app\\actions");
$js .= extract("$dir\\app\\composites");
$js .= extract("$dir\\app\\controls");
$js .= extract("$dir\\app\\core");
$js .= extract("$dir\\app\\events");
$js .= extract("$dir\\app\\fieldtypes_dep");
$js .= extract("$dir\\app\\layouts");
$js .= extract("$dir\\app\\literals");
$js .= extract("$dir\\app\\nodes");
$js .= extract("$dir\\app\\pagecmp");
$js .= extract("$dir\\app\\props");
$js .= extract("$dir\\app\\proptypes");
$js .= extract("$dir\\app\\tools");
$js .= extract("$dir\\app\\types");
$js .= extract("$dir\\app\\view");
$js .= extract("$dir\\app\\wizards");
$js .= extractFile("$dir\\app\\_last-vsdoc.js");

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
        if($line !~ m/-vsdoc\.js/ && 
           $line !~ m/-debug\.js/)
        {
            $ret .= $line;         
        }
    }
    close(JSFILE);
    return $ret;
}

printf("Finished generating CswApp-vsdoc.js\n");