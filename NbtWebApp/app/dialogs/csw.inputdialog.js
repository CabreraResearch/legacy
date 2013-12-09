/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.dialogs.register('inputDialog', function (cswPrivate) {
        'use strict';

        (function _preCtor() {
            cswPrivate.title = cswPrivate.title || 'Enter Information';
            cswPrivate.height = cswPrivate.height || 400;
            cswPrivate.width = cswPrivate.width || 600;
            cswPrivate.message = cswPrivate.message || '';
            cswPrivate.fields = cswPrivate.fields || {};
            cswPrivate.onOpen = cswPrivate.onOpen || function () { };
            cswPrivate.onClose = cswPrivate.onClose || function () { };
            cswPrivate.onOk = cswPrivate.onOk || function () { };
        }());

        return (function () {
            'use strict';
            var inputDialog = Csw.layouts.dialog({
                title: cswPrivate.title,
                width: cswPrivate.width,
                height: cswPrivate.height,
                onOpen: cswPrivate.onOpen,
                onClose: cswPrivate.onClose
            });


            inputDialog.div.span({ text: cswPrivate.message, align: 'center' });
            inputDialog.div.br({ number: 2 });
            
            var fieldTable = inputDialog.div.table();
            var fieldCount = 0;
            var fieldElements = {};
            for (var fieldName in cswPrivate.fields) {
                fieldCount = fieldCount + 1;
                fieldTable.cell(fieldCount, 1).span({ text: fieldName });
                fieldElements[fieldName] = fieldTable.cell(fieldCount, 2).input({
                    name: fieldName,
                    type: cswPrivate.fields[fieldName].type,
                });
            }//for each field

            inputDialog.div.button({
                enabledText: 'OK',
                onClick: function () { cswPrivate.onOk(fieldElements); },
            });

            inputDialog.div.button({
                enabledText: 'Cancel',
                onClick: function () {
                    inputDialog.close();
                }
            });

            inputDialog.open();
            
            return inputDialog;
        }());
    });
}());