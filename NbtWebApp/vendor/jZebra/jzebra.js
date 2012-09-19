(function() {

    var jZebra = {
        
    };

    var sleepCounter = 0;

    jZebra.findPrinter = function() {
        var applet = document.jzebra;
        if (applet != null) {
            // Searches for locally installed printer with "zebra" in the name
            applet.findPrinter(document.getElementById("printerField").value);
        }
        jZebra.monitorFinding();
    }

    jZebra.findPrinters = function (defaultPrinter) {
        var applet = document.jzebra;
        if (applet != null) {
            // Searches for locally installed printer with "zebra" in the name
            applet.findPrinter(",");
        }
        jZebra.monitorFinding2(defaultPrinter);
    }

    jZebra.print = function () {
        var applet = document.jzebra;
        if (applet != null) {
            // Send characters/raw commands to applet using "append"
            // Hint:  Carriage Return = \r, New Line = \n, Escape Double Quotes= \"
            // Note:  "Unescape" is optional and is used to fix "%20" type URL encoded fields
            applet.append(unescape(document.getElementById("printField").value.replace("sample.html", applet.getVersion() + " sample.html")));


            //applet.append("A590,1600,2,3,1,1,N,\"jZebra " + applet.getVersion() + " sample.html\"\n");
            //applet.append("A590,1570,2,3,1,1,N,\"Testing the print() function\"\n");
            //applet.append("P1\n");

            // Send characters/raw commands to printer
            applet.print();
        }

        jZebra.monitorPrinting();

        /**
          *  PHP PRINTING:
          *  // Uses the php `"echo"` function in conjunction with jZebra `"append"` function
          *  // This assumes you have already assigned a value to `"$commands"` with php
          *  document.jzebra.append(<?php echo $commands; ?>);
          */

        /**
          *  SPECIAL ASCII ENCODING
          *  //applet.setEncoding("UTF-8");
          *  applet.setEncoding("Cp1252"); 
          *  applet.append("\xDA");
          *  applet.append(String.fromCharCode(218));
          *  applet.append(chr(218));
          */

    }

    jZebra.print64 = function () {
        var applet = document.jzebra;
        if (applet != null) {
            // Use jZebra's `"append64"` function. This will automatically convert provided
            // base64 encoded text into ascii/bytes, etc.
            applet.append64(document.getElementById("print64Field").value);

            // Send characters/raw commands to printer
            applet.print();
        }
        jZebra.monitorPrinting();
    }

    jZebra.printPages = function () {
        var applet = document.jzebra;
        if (applet != null) {
            applet.append("A590,1600,2,3,1,1,N,\"jZebra 1\"\n");
            applet.append("A590,1570,2,3,1,1,N,\"Testing the printPages() function\"\n");
            applet.append("P1\n");

            applet.append("A590,1600,2,3,1,1,N,\"jZebra 2\"\n");
            applet.append("A590,1570,2,3,1,1,N,\"Testing the printPages() function\"\n");
            applet.append("P1\n");

            applet.append("A590,1600,2,3,1,1,N,\"jZebra 3\"\n");
            applet.append("A590,1570,2,3,1,1,N,\"Testing the printPages() function\"\n");
            applet.append("P1\n");

            applet.append("A590,1600,2,3,1,1,N,\"jZebra 4\"\n");
            applet.append("A590,1570,2,3,1,1,N,\"Testing the printPages() function\"\n");
            applet.append("P1\n");

            applet.append("A590,1600,2,3,1,1,N,\"jZebra 5\"\n");
            applet.append("A590,1570,2,3,1,1,N,\"Testing the printPages() function\"\n");
            applet.append("P1\n");

            applet.append("A590,1600,2,3,1,1,N,\"jZebra 6\"\n");
            applet.append("A590,1570,2,3,1,1,N,\"Testing the printPages() function\"\n");
            applet.append("P1\n");

            applet.append("A590,1600,2,3,1,1,N,\"jZebra 7\"\n");
            applet.append("A590,1570,2,3,1,1,N,\"Testing the printPages() function\"\n");
            applet.append("P1\n");

            applet.append("A590,1600,2,3,1,1,N,\"jZebra 8\"\n");
            applet.append("A590,1570,2,3,1,1,N,\"Testing the printPages() function\"\n");
            applet.append("P1\n");

            // Mark the end of a label, in this case  P1 plus a newline character
            // jZebra knows to look for this and treat this as the end of a "page"
            // for better control of larger spooled jobs (i.e. 50+ labels)
            // i.e. applet.setEndOfDocument("P1\n");
            applet.setEndOfDocument(unescape(document.getElementById("endField").value));

            // The amount of labels to spool to the printer at a time. When
            // jZebra counts this many `EndOfDocument`'s, a new print job will 
            // automatically be spooled to the printer and counting will start
            // over.
            // i.e. applet.setDocumentsPerSpool("3");
            applet.setDocumentsPerSpool(document.getElementById("spoolField").value);

            // Send characters/raw commands to printer
            applet.print();

        }
        jZebra.monitorPrinting();
    }

    jZebra.printXML = function () {
        var applet = document.jzebra;
        if (applet != null) {
            // Appends the contents of an XML file from a SOAP response, etc.
            // a valid relative URL or a valid complete URL is required for the XML
            // file.  The second parameter must be a valid XML tag/node containing
            // base64 encoded data, i.e. <node_1>aGVsbG8gd29ybGQ=</node_1>
            // Example:
            //     applet.appendXML("http://yoursite.com/zpl.xml", "node_1");
            //     applet.appendXML("http://justtesting.biz/jZebra/dist/epl.xml", "v7:Image");
            var field = document.getElementById("xmlField").value;
            field = (field.indexOf("://") == -1) ? window.location.href + "/../" + field : field;
            applet.appendXML(field, "v7:Image");

            // Send characters/raw commands to printer
            //applet.print(); // Can't do this yet because of timing issues with XML
        }

        // Monitor the append status of the xml file, prints when appending if finished
        jZebra.monitorAppending();
    }

    jZebra.printHex = function () {
        var applet = document.jzebra;
        if (applet != null) {
            // Using jZebra's "append()" function, hexadecimanl data can be sent
            // by using JavaScript's "\x00" notation. i.e. "41 35 39 30 2c ...", etc
            // Example: 
            //     applet.append("\x41\x35\x39\x30\x2c"); // ...etc
            //	   applet.append(unescape("%41%35%39%30%2c")); // ...etc
            // 
            applet.append(unescape(document.getElementById("hexField").value));

            // Send characters/raw commands to printer
            applet.print();


        }

        jZebra.monitorPrinting();

        /**
          *  CHR/ASCII PRINTING:
          *  // Appends CHR(27) + CHR(29) using `"fromCharCode"` function
          *  // CHR(27) is commonly called the "ESCAPE" character
          *  document.jzebra.append(String.fromCharCode(27) + String.fromCharCode(29));
          */
    }


    jZebra.printFile = function () {
        var applet = document.jzebra;
        if (applet != null) {
            // Using jZebra's "appendFile()" function, a file containg your raw EPL/ZPL
            // can be sent directly to the printer
            // Example: 
            //     applet.appendFile("http://yoursite/zpllabel.txt"); // ...etc
            var field = document.getElementById("fileField").value;
            field = (field.indexOf("://") == -1) ? window.location.href + "/../" + field : field;

            applet.appendFile(field);
            applet.print();
        }

        jZebra.monitorPrinting();
    }

    jZebra.printImage = function () {
        var applet = document.jzebra;
        if (applet != null) {
            // Using jZebra's "appendImage()" function, a png, jpeg file
            // can be sent directly to the printer supressing the print dialog
            // Example:
            //     applet.appendImage("http://yoursite/logo1.png"); // ...etc

            // Sample only: Searches for locally installed printer with "pdf" in the name
            // Can't use Zebra, because this function needs a PostScript capable printer
            applet.findPrinter("PDF");
            while (!applet.isDoneFinding()) {
                // Wait
            }

            // Sample only: If a PDF printer isn't installed, try the Microsoft XPS Document
            // Writer'
            if (applet.getPrinterName() == null) {
                applet.findPrinter("Microsoft XPS Document Writer");
                while (!applet.isDoneFinding()) {
                    // Wait
                }
            }

            // No suitable printer found, exit
            if (applet.getPrinterName() == null) {
                alert("Could not find a suitable printer for images");
                return;
            }

            // Append our image (only one image can be appended per print)
            var field = document.getElementById("imageField").value;
            field = (field.indexOf("://") == -1) ? window.location.href + "/../" + field : field;

            applet.appendImage(field);
        }

        // Very important for images, uses printPS() insetad of print()
        jZebra.monitorAppending2();
    }

    jZebra.chr = function (i) {
        return String.fromCharCode(i);
    }

    jZebra.monitorFinding = function () {
        jZebra.monitorApplet('isDoneFinding()', 'alert("Found printer [" + document.jzebra.getPrinter() + "]")', 'monitor finding job');
    }

    jZebra.monitorPrinting = function () {
        //"alert\(\"Data sent to printer successfully\"\)"
        jZebra.monitorApplet("isDonePrinting()", 'alert("Data sent to printer [" + document.jzebra.getPrinter() + "] successfully.")', "monitor printing job");
    }

    /**
     * Monitors the Java applet until it is complete with the specified function
     *    appletFunction:  should return a "true" or "false"
     *    finishedFunction:  will be called if the function completes without error
     *    description:  optional description for errors, etc
     *
     * Example:
     *    monitorApplet('isDoneFinding()', 'alert(\\"Success\\")', '');
     */

    jZebra.monitorApplet = function (appletFunction, finishedFunction, description) {
        var NOT_LOADED = "jZebra hasn't loaded yet.";
        var INVALID_FUNCTION = 'jZebra does not recognize function: "' + appletFunction;
        +'"';
        var INVALID_PRINTER = "jZebra could not find the specified printer";
        if (document.jzebra != null) {
            var finished = false;
            try {
                finished = eval('document.jzebra.' + appletFunction);
            } catch(err) {
                alert('jZebra Exception:  ' + INVALID_FUNCTION);
                return;
            }
            if (!finished) {
                window.setTimeout(
                    function () {
                        jZebra.monitorApplet(appletFunction, finishedFunction.replace(/\"/g, '\\"'), description)
                    }, 100);
            } else {
                var p = document.jzebra.getPrinterName();
                if (p == null) {
                    alert("jZebra Exception:  " + INVALID_PRINTER);
                    return;
                }
                var e = document.jzebra.getException();
                if (e != null) {
                    var desc = description == "" ? "" : " [" + description + "] ";
                    alert("jZebra Exception: " + desc + document.jzebra.getExceptionMessage());
                    document.jzebra.clearException();
                } else {
                    eval(finishedFunction);
                }
            }
        } else {
            alert("jZebra Exception:  " + NOT_LOADED);
        }
    }

    // Used to show/hide code snippets on sample.html page

    jZebra.showHide = function (row) {
        var elem = document.getElementById(row);
        elem.style.display = (elem.style.display == "table-row") ? "none" : "table-row";
        var link = document.getElementById(row + "_link");
        link.innerHTML = (elem.style.display == "none") ?
            link.innerHTML.replace("-", "+").replace("hide", "show") :
            link.innerHTML.replace("+", "-").replace("show", "hide");
    }

    /*function monitorPrinting() {
    var applet = document.jzebra;
    if (applet != null) {
     if (!applet.isDonePrinting()) {
        window.setTimeout('monitorPrinting()', 100);
     } else {
        var e = applet.getException();
        alert(e == null ? "Printed Successfully" : "Exception occured: " + e.getLocalizedMessage());
     }
    } else {
          alert("Applet not loaded!");
      }
    }*/

    /*function monitorFinding() {
    var applet = document.jzebra;
    if (applet != null) {
     if (!applet.isDoneFinding()) {
        window.setTimeout('monitorFinding()', 100);
     } else {
        var printer = applet.getPrinterName();
            alert(printer == null ? "Printer not found" : "Printer \"" + printer + "\" found");
     }
    } else {
          alert("Applet not loaded!");
      }
    }*/

    jZebra.monitorFinding2 = function (defaultPrinter) {
        var applet = document.jzebra;
        if (applet != null) {
            if (!applet.isDoneFinding()) {
                window.setTimeout(function () {
                    jZebra.monitorFinding2(defaultPrinter)
                }, 100);
            } else {
                var listing = applet.getPrinters();
                var printers = listing.split(',');
                for (var i in printers) {
                    if (defaultPrinter == printers[i]) {
                        document.getElementById("printersList").options[i] = new Option(printers[i], printers[i], true);
                    } else {
                        document.getElementById("printersList").options[i] = new Option(printers[i], printers[i]);
                    }
                    //alert(printers[i]);
                }
                $('#printersList').val(defaultPrinter);
            }
        } else {
            alert("Applet not loaded!");
        }
    }

    jZebra.monitorAppending = function () {
        var applet = document.jzebra;
        if (applet != null) {
            if (!applet.isDoneAppending()) {
                window.setTimeout(jZebra.monitorAppending, 100);
            } else {
                applet.print(); // Don't print until all of the data has been appended
                jZebra.monitorPrinting();
            }
        } else {
            alert("Applet not loaded!");
        }
    }

    jZebra.monitorAppending2 = function () {
        var applet = document.jzebra;
        if (applet != null) {
            if (!applet.isDoneAppending()) {
                window.setTimeout(jZebra.monitorAppending2, 100);
            } else {
                applet.printPS(); // Don't print until all of the image data has been appended
                jZebra.monitorPrinting();
            }
        } else {
            alert("Applet not loaded!");
        }
    }

    jZebra.monitorLoading = function () {
        var applet = document.jzebra;
        if (document.jzebra != null) {
            try {
                if (document.jzebra.isActive()) {
                    document.getElementById("version").innerHTML = "<strong>Status:</strong>  jZebra " + applet.getVersion() + " loaded.";
                    if (navigator.appName == "Microsoft Internet Explorer") { // IE Fix
                        document.getElementById("logo").src = "http://jzebra.googlecode.com/files/logo_small.png"
                    } else { // Use embedded logo
                        window.alert('no img');
                    }
                }
            } catch(err) {
                // Firefox fix
                window.setTimeout(jZebra.monitorLoading, 500);
            }
        } else {
            window.setTimeout(jZebra.monitorLoading, 100);
        }
    }

    jZebra.displayLogo = function () {
        if (navigator.appName == "Microsoft Internet Explorer") { // IE Fix
            document.getElementById("logo").src = "http://jzebra.googlecode.com/files/logo_small.gif"
        } else {
            window.alert('no img');
        }
        jZebra.monitorLoading();
    }

    window.jZebra = jZebra;

}());