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


//    Csw.newWindow = Csw.newWindow ||
//        Csw.register('newWindow', function (bodyhtml) { //onSuccess) {
//            var printWindow, printDoc;

//            printWindow = window.open();
//            printDoc = printWindow.document;

//            var html = '<!DOCTYPE html PUBLIC \'-//W3C//DTD XHTML 1.0 Transitional//EN\' \'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\'>';
//            html += '<html>';
//            html += '<head>';
//            html += '<link rel="stylesheet" type="text/css" href="Content/themes/base/jquery.ui.all.css" />';
//            html += '<link rel="stylesheet" type="text/css" href="//ajax.aspnetcdn.com/ajax/jquery.ui/1.8.19/themes/redmond/jquery-ui.css" />';
//            //html += '<link rel="stylesheet" type="text/css" href="Content/jquery.jqGrid/ui.jqgrid.css" />';
//            html += '<title>';
//            html += printDoc.title;
//            html += '</title>';
//            html += '</head>';
//            html += '<body>';
//            html += bodyhtml;
//            html += '</body>';
//            html += '</html>';
//            
//            printDoc.open();
//            printDoc.write(html);
//            printDoc.close();

//            //In theory, this would create a DIV with all the styles required, but it's probably not necessary to do so.
//            //$styleDiv = $('<div>').append( $('style').clone() ) 

//            //print() is globally blocking. Just let the user initiate print.
//            //printWindow.print();

//            return false;
//        });

    
} ());
