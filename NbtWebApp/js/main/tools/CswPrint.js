/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />

//#region CswPrint

var CswPrint = CswPrint || function (onSuccess) {
    "use strict;"
    var printWindow, printDoc, $styleDiv, $newBody;
    
    printWindow = window.open();
    printDoc = printWindow.document;
        
    printDoc.open();
    printDoc.write('<!DOCTYPE html PUBLIC \'-//W3C//DTD XHTML 1.0 Transitional//EN\' \'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\'>');
    printDoc.write('<html>');
    printDoc.write('<head>');
    printDoc.write('<link rel="stylesheet" type="text/css" href="Content/themes/base/jquery.ui.all.css" />');
    printDoc.write('<link rel="stylesheet" type="text/css" href="js/thirdparty/jquery/themes/theme-aristo/css/Aristo/Aristo.css" />');
    printDoc.write('<link rel="stylesheet" type="text/css" href="Content/jquery.jqGrid/ui.jqgrid.css" />');
    printDoc.write('<title>');
    printDoc.write(document.title);
    printDoc.write('</title>');
    printDoc.write('</head>');
    printDoc.write('<body></body>');
    printDoc.write('</html>');
    printDoc.close();

    $newBody = $(printDoc).find('body');
    
    /* 
    In theory, this would create a DIV with all the styles required, but it's probably not necessary to do so.
    $styleDiv = $('<div>').append( $('style').clone() ) 
    */
    $styleDiv = $newBody.CswDiv();
        
    if(isFunction(onSuccess)) {
        onSuccess($styleDiv);
    }
    
    /*
    print() is globally blocking. Just let the user initiate print.
    printWindow.print();
    */
    
    return false;
};


//#endregion CswPrint