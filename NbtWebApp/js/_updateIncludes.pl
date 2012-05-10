use strict;

my ($sec,$min,$hour,$mday,$mon,$year,$wday,$yday,$isdst) = localtime(time);
my $datestr = ($year + 1900).".". ($mon + 1) .".$mday";

my $dir = $ARGV[0];
my $includesFile = "$dir\\MainCswIncludes.html";
unlink($includesFile);

my $includesContent = <<HTML;
<link rel="stylesheet" type="text/css" href="css/ChemSW.$datestr.min.css" />
<script type="text/javascript" src="CswCommon.$datestr.min.js"></script>
<script type="text/javascript" src="CswNbt.$datestr.min.js"></script>
HTML

open(my $include, '>', $includesFile) or die $!;
print $include $includesContent;
close($include);

printf("Finished building MainCswIncludes.html\n");