use strict;

my $dir = $ARGV[0];
my $destfile = "$dir\\js\\ChemSW.min.js";

unlink($destfile);

my $param = "";
$param .= extract("$dir\\csw.js");
$param .= extract("$dir\\csw.js\\_loadfirst");
$param .= extract("$dir\\csw.js\\core");
$param .= extract("$dir\\csw.js\\components");
$param .= extract("$dir\\csw.js\\controls");
$param .= extract("$dir\\csw.js\\nodes");
$param .= extract("$dir\\csw.js\\props");
$param .= extract("$dir\\csw.js\\tools");

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
		   $filename !~ /.min\.js/) 
		{
			printf("Compiling: $path\\$filename\n");
			$filelist .= "--js $path\\$filename ";
		}
	}
	closedir(JSDIR);
	return $filelist;
}

printf("Finished compiling csw.js javascript\n");