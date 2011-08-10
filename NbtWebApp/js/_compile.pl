use strict;

my $dir = $ARGV[0];
my $destfile = "$dir\\js\\CswAll.min.js";

unlink($destfile);

my $param = "";
$param .= extract("$dir\\js\\globals");
$param .= extract("$dir\\js");
$param .= extract("$dir\\js\\main");
$param .= extract("$dir\\js\\main\\actions");
$param .= extract("$dir\\js\\main\\controls");
$param .= extract("$dir\\js\\main\\fieldtypes");
$param .= extract("$dir\\js\\main\\node");
$param .= extract("$dir\\js\\main\\pagecmp");
$param .= extract("$dir\\js\\main\\tools");
$param .= extract("$dir\\js\\main\\view");

`java -jar "$dir\\..\\..\\..\\ThirdParty\\ClosureCompiler\\compiler.jar" $param --js_output_file $destfile`;

sub extract
{
	my $filelist = "";
	my $path = $_[0];
	opendir(JSDIR, $path) or die("Cannot open js directory: $path ; $!");
	while((my $filename = readdir(JSDIR)))
	{
		if($filename =~ /.*\.js$/ &&
		   $filename !~ /-vsdoc/) 
		{
			printf("Compiling: $path\\$filename\n");
			$filelist .= "--js $path\\$filename ";
		}
	}
	closedir(JSDIR);
	return $filelist;
}

printf("Finished compiling javascript\n");