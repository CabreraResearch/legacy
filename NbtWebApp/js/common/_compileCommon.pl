use strict;

my $dir = $ARGV[0];
my $destfile = "$dir\\CswCommon.min.js";

unlink($destfile);

my $param = "";
$param .= "--js $dir\\js\\ChemSW-vsdoc.js ";

`java -jar "$dir\\..\\..\\..\\ThirdParty\\ClosureCompiler\\compiler.jar" $param --js_output_file $destfile`;

printf("Finished compiling CswCommon.min.js javascript\n");