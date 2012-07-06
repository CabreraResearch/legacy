use strict;

my ($sec,$min,$hour,$mday,$mon,$year,$wday,$yday,$isdst) = localtime(time);
my $datestr = ($year + 1900).".". ($mon + 1) .".$mday";

my $dir = $ARGV[0];
my $destfile = "$dir\\CswNbt.$datestr.min.js";

unlink($destfile);

my $param = "";
$param .= extract("$dir\\js\\nbt");
$param .= extract("$dir\\js\\nbt\\actions");
$param .= extract("$dir\\js\\nbt\\controls");
$param .= extract("$dir\\js\\nbt\\layouts");
$param .= extract("$dir\\js\\nbt\\literals");
$param .= extract("$dir\\js\\nbt\\fieldtypes");
$param .= extract("$dir\\js\\nbt\\nodes");
$param .= extract("$dir\\js\\nbt\\props");
$param .= extract("$dir\\js\\nbt\\pagecmp");
$param .= extract("$dir\\js\\nbt\\tools");
$param .= extract("$dir\\js\\nbt\\view");
$param .= extract("$dir\\js\\nbt\\wizards");

`java -jar "$dir\\..\\..\\..\\ThirdParty\\ClosureCompiler\\compiler.jar" $param --create_source_map $dir/js/nbt.js.map --source_map_format=V3 --js_output_file $destfile`;

sub extract
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

open(JSFILE, ">>$destfile") or die("Cannot open js directory: $destfile; $!");
print JSFILE "//@ sourceMappingURL=js/nbt.js.map";
close (JSFILE);

printf("Finished compiling CswNbt.$datestr.min.js javascript\n");