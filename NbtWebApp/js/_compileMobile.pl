use strict;

my $destfile = "\\kiln\\nbt\\nbt\\nbtwebapp\\js\\mobile\\CswAllMobile.min.js";

unlink($destfile);

my $param = "";
$param .= extract("\\kiln\\nbt\\nbt\\nbtwebapp\\js");
$param .= extract("\\kiln\\nbt\\nbt\\nbtwebapp\\js\\jquery\\common");
$param .= extract("\\kiln\\nbt\\nbt\\nbtwebapp\\js\\mobile");
$param .= extract("\\kiln\\nbt\\nbt\\nbtwebapp\\js\\mobile\\clientdb");
$param .= extract("\\kiln\\nbt\\nbt\\nbtwebapp\\js\\mobile\\fieldtypes");
$param .= extract("\\kiln\\nbt\\nbt\\nbtwebapp\\js\\mobile\\objectclasses");
$param .= extract("\\kiln\\nbt\\nbt\\nbtwebapp\\js\\mobile\\pages");
$param .= extract("\\kiln\\nbt\\nbt\\nbtwebapp\\js\\mobile\\sync");

`java -jar "\\kiln\\ThirdParty\\ClosureCompiler\\compiler.jar" $param --js_output_file $destfile`;

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