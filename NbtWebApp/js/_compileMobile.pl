use strict;

my $dir = $ARGV[0];
my $destfile = "$dir\\js\\mobile\\CswAllMobile.min.js";

unlink($destfile);

my $param = "";
$param .= extract("$dir\\js");
$param .= extract("$dir\\js\\jquery\\common");
$param .= extract("$dir\\js\\mobile");
$param .= extract("$dir\\js\\mobile\\clientdb");
$param .= extract("$dir\\js\\mobile\\fieldtypes");
$param .= extract("$dir\\js\\mobile\\objectclasses");
$param .= extract("$dir\\js\\mobile\\pages");
$param .= extract("$dir\\js\\mobile\\sync");

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
           $filename !~ /.min./ ) 
		{
			printf("Compiling: $path\\$filename\n");
			$filelist .= "--js $path\\$filename ";
		}
	}
	closedir(JSDIR);
	return $filelist;
}

printf("Finished compiling javascript\n");