use strict;

my ($sec,$min,$hour,$mday,$mon,$year,$wday,$yday,$isdst) = localtime(time);
my $datestr = ($year + 1900).".". ($mon + 1) .".$mday";

my $dir = $ARGV[0];

my $destfile = "$dir\\CswCommon.$datestr.min.js";

unlink($destfile);

my $param = "";
$param .= "--js $dir\\js\\CswCommon-vsdoc.js ";

`java -jar "$dir\\..\\..\\..\\ThirdParty\\ClosureCompiler\\compiler.jar" $param --js_output_file $destfile`;

printf("Finished compiling CswCommon.$datestr.min.js javascript\n");