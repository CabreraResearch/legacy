
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';

    Csw.openPopup = Csw.openPopup ||
       Csw.register('openPopup', function (url, title, options) {
           var cswPrivate = {
               status: 'no',
               resizeable: 'yes',
               scrollbars: 'yes',
               toolbar: 'yes',
               location: 'no',
               menubar: 'yes',
               height: 600,
               width: 800
           };
           Csw.extend(cswPrivate, options);
           if(window.internetExplorerVersionNo > 0) {
               title = '';
           }
           var popup = window.open(url, Csw.string(title), Csw.params(cswPrivate, ','));
           popup.focus();
           return popup;
       });


    Csw.newWindow = Csw.newWindow ||
        Csw.register('newWindow', function (url, title, options, callBack) { //onSuccess) {
            var printWindow, printDoc;

            printWindow = Csw.openPopup(url, title, options);
            printDoc = printWindow.document;

            Csw.tryExec(callBack);

            return false;
        });

    
} ());
