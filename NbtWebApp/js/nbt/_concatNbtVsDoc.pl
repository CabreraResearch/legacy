use strict;

my $dir = $ARGV[0];
my $destfile = "$dir\\js\\CswNbt-vsdoc.js";

unlink($destfile);

my $js = "";
$js .= extractFile("$dir\\js\\_first-vsdoc.js");
$js .= extractFile("$dir\\js\\nbt\\Main.js");
$js .= extract("$dir\\js\\nbt\\actions");
$js .= extract("$dir\\js\\nbt\\controls");
$js .= extract("$dir\\js\\nbt\\layouts");
$js .= extract("$dir\\js\\nbt\\literals");
$js .= extract("$dir\\js\\nbt\\fieldtypes");
$js .= extract("$dir\\js\\nbt\\nodes");
$js .= extract("$dir\\js\\nbt\\pagecmp");
$js .= extract("$dir\\js\\nbt\\props");
$js .= extract("$dir\\js\\nbt\\tools");
$js .= extract("$dir\\js\\nbt\\view");
$js .= extract("$dir\\js\\nbt\\wizards");
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
    close(JSFILE);
    return $ret;
}

printf("Finished generating CswNbt-vsdoc.js\n");