/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function () {
    'use strict';

    Csw.openPopup = Csw.openPopup ||
        Csw.register('openPopup', function (url, height, width) {
            var popup = window.open(url, null, 'height=' + height + ', width=' + width + ', status=no, resizable=yes, scrollbars=yes, toolbar=yes, location=no, menubar=yes');
            popup.focus();
            return popup;
        });


    Csw.newWindow = Csw.newWindow ||
        Csw.register('newWindow', function (onSuccess) {
            var printWindow, printDoc, $newBody, newFactory;

            printWindow = window.open();
            printDoc = printWindow.document;

            printDoc.open();
            printDoc.write('<!DOCTYPE html PUBLIC \'-//W3C//DTD XHTML 1.0 Transitional//EN\' \'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\'>');
            printDoc.write('<html>');
            printDoc.write('<head>');
            printDoc.write('<link rel="stylesheet" type="text/css" href="Content/themes/base/jquery.ui.all.css" />');
            printDoc.write('<link rel="stylesheet" type="text/css" href="js/thirdparty/theme-aristo/css/Aristo/Aristo.css" />');
            printDoc.write('<link rel="stylesheet" type="text/css" href="Content/jquery.jqGrid/ui.jqgrid.css" />');
            printDoc.write('<title>');
            printDoc.write(printDoc.title);
            printDoc.write('</title>');
            printDoc.write('</head>');
            printDoc.write('<body></body>');
            printDoc.write('</html>');
            printDoc.close();

            $newBody = $(printDoc).find('body');
            newFactory = Csw.literals.factory($newBody);

            /* 
        In theory, this would create a DIV with all the styles required, but it's probably not necessary to do so.
        $styleDiv = $('<div>').append( $('style').clone() ) 
        */

            if (Csw.isFunction(onSuccess)) {
                onSuccess(newFactory.div());
            }

            /*
        print() is globally blocking. Just let the user initiate print.
        printWindow.print();
        */

            return false;
        });

    
} ());