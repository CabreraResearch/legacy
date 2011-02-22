use strict;

my $param = "";
$param .= extract("c:\\kiln\\nbt\\nbt\\nbtwebapp\\js");
$param .= extract("c:\\kiln\\nbt\\nbt\\nbtwebapp\\js\\fieldtypes");

`java -jar "C:\\kiln\\ThirdParty\\ClosureCompiler\\compiler.jar" $param --js_output_file c:\\kiln\\nbt\\nbt\\nbtwebapp\\js\\CswAll.min.js`;

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

