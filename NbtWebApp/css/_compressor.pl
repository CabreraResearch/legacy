use strict;

my $dir = $ARGV[0];
my $destfile = "$dir\\css\\ChemSW.min.css";

unlink($destfile);

printf("Compiling: $dir\\css\\_style.css\n");
`java -jar "$dir\\..\\..\\..\\ThirdParty\\YUICompressor\\build\\yuicompressor-2.4.7.jar" $dir\\css\\_style.css >> $destfile`;

printf("Compiling: $dir\\css\\csw.combobox.css\n");
`java -jar "$dir\\..\\..\\..\\ThirdParty\\YUICompressor\\build\\yuicompressor-2.4.7.jar" $dir\\css\\csw.combobox.css >> $destfile`;

printf("Compiling: $dir\\css\\csw.errormessage.css\n");
`java -jar "$dir\\..\\..\\..\\ThirdParty\\YUICompressor\\build\\yuicompressor-2.4.7.jar" $dir\\css\\csw.errormessage.css >> $destfile`;

printf("Compiling: $dir\\css\\csw.fieldtypes.css\n");
`java -jar "$dir\\..\\..\\..\\ThirdParty\\YUICompressor\\build\\yuicompressor-2.4.7.jar" $dir\\css\\csw.fieldtypes.css >> $destfile`;

printf("Compiling: $dir\\css\\csw.menu.css\n");
`java -jar "$dir\\..\\..\\..\\ThirdParty\\YUICompressor\\build\\yuicompressor-2.4.7.jar" $dir\\css\\csw.menu.css >> $destfile`;

printf("Compiling: $dir\\css\\csw.search.css\n");
`java -jar "$dir\\..\\..\\..\\ThirdParty\\YUICompressor\\build\\yuicompressor-2.4.7.jar" $dir\\css\\csw.search.css >> $destfile`;

printf("Compiling: $dir\\css\\csw.table.layout.css\n");
`java -jar "$dir\\..\\..\\..\\ThirdParty\\YUICompressor\\build\\yuicompressor-2.4.7.jar" $dir\\css\\csw.table.layout.css >> $destfile`;

printf("Compiling: $dir\\css\\csw.vieweditor.css\n");
`java -jar "$dir\\..\\..\\..\\ThirdParty\\YUICompressor\\build\\yuicompressor-2.4.7.jar" $dir\\css\\csw.vieweditor.css >> $destfile`;

printf("Compiling: $dir\\css\\csw.wizard.css\n");
`java -jar "$dir\\..\\..\\..\\ThirdParty\\YUICompressor\\build\\yuicompressor-2.4.7.jar" $dir\\css\\csw.wizard.css >> $destfile`;

printf("Finished compiling css\n");
