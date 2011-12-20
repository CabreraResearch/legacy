/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />

//#region CswPrint

var CswPrint = CswPrint || function ($element) {
    "use strict;"
    
    var printWindow, printDoc, $styleDiv;        
    $element = $($element);
    
    if(false === isNullOrEmpty($element, true)) {

        printWindow = window.open();
        printDoc = printWindow.document;
        
        printDoc.open();
        printDoc.write('<!DOCTYPE html PUBLIC \'-//W3C//DTD XHTML 1.0 Transitional//EN\' \'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\'>');
        printDoc.write('<html>');
        printDoc.write('<body>');
        printDoc.write('<head>');
        printDoc.write('<title>');
        printDoc.write(document.title);
        printDoc.write('</title>');
        printDoc.write('</head>');
        printDoc.write('</body>');
        printDoc.write('</html>');
        printDoc.close();

        $styleDiv = $('<div>').append( $('style').clone() )
                        .appendTo('body');
        $styleDiv.append($element);

        printWindow.print();
    }
};


//#endregion CswPrint