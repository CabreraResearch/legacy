use strict;

my $dir = $ARGV[0];
my $destfile = "$dir\\js\\CswAll.min.js";

unlink($destfile);

my $param = "";
$param .= extract("$dir\\js");
$param .= extract("$dir\\js\\actions");
$param .= extract("$dir\\js\\controls");
$param .= extract("$dir\\js\\fieldtypes");
$param .= extract("$dir\\js\\node");
$param .= extract("$dir\\js\\pagecmp");
$param .= extract("$dir\\js\\tools");
$param .= extract("$dir\\js\\view");

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

printf("Finished compiling javascript\n");