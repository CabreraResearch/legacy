use strict;
use warnings;
use Time::Local;

my ($sec,$min,$hour,$mday,$mon,$year,$wday,$yday,$isdst) = localtime(time);
my $datestr = ($year + 1900).".". ($mon + 1) .".$mday";

my $dir = $ARGV[0];
my $includesFile = "$dir\\MainCswIncludes.html";
unlink($includesFile);

sub deleteYesterdays;
sub deleteYesterdays {
    my $path = $_[0];
    opendir(JSDIR, $path) or die("Cannot open js directory: $path ; $!");
    while((my $filename = readdir(JSDIR)))
    {
        if($filename =~ /Csw.*\.min\.(js|css)$/) 
        {
            unlink("$path\\$filename");
        }
    }
    closedir(JSDIR);
}
deleteYesterdays("$dir\\css") or die $!;
deleteYesterdays("$dir") or die $!;

my $includesContent = <<HTML;
<link rel="stylesheet" type="text/css" href="css/ChemSW.$datestr.min.css" />
<script type="text/javascript" src="CswApp.$datestr.min.js"></script>
HTML

open(my $include, '>', $includesFile) or die $!;
print $include $includesContent;
close($include);

printf("Finished building MainCswIncludes.html\n");