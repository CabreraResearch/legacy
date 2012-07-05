use strict;

my ($sec,$min,$hour,$mday,$mon,$year,$wday,$yday,$isdst) = localtime(time);
my $datestr = ($year + 1900).".". ($mon + 1) .".$mday";

my $dir = $ARGV[0];

my $destfile = "$dir\\CswCommon.$datestr.min.js";

unlink($destfile);

my $param = "";
$param .= "--js $dir\\js\\CswCommon-vsdoc.js ";

`java -jar "$dir\\..\\..\\..\\ThirdParty\\ClosureCompiler\\compiler.jar" $param  --create_source_map $dir/js/common.js.map --source_map_format=V3 --js_output_file $destfile`;

open(JSFILE, ">>$destfile") or die("Cannot open js directory: $destfile; $!");
print JSFILE "//@ sourceMappingURL=js/common.js.map";
close (JSFILE);

printf("Finished compiling CswCommon.$datestr.min.js javascript\n");