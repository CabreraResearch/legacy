/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.dialogs.register('confirmDialog', function (cswPrivate) {
        'use strict';

        (function _preCtor() {
            cswPrivate.title = cswPrivate.title || 'Accept or Decline';
            cswPrivate.name = cswPrivate.name || 'ConfirmDialog',
            cswPrivate.height = cswPrivate.height || 400;
            cswPrivate.width = cswPrivate.width || 600;
            cswPrivate.message = cswPrivate.message || '';
            cswPrivate.gridData = cswPrivate.gridData || null; // { fields: [], columns: [], data: [] }
            cswPrivate.onOpen = cswPrivate.onOpen || function () { };
            cswPrivate.onClose = cswPrivate.onClose || function () { };
            cswPrivate.onYes = cswPrivate.onYes || function () { };
            cswPrivate.onNo = cswPrivate.onNo || function () { };
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
            inputDialog.open();
            
            //inputDialog.div.span({ text: cswPrivate.message, align: 'center' });
            //inputDialog.div.br({ number: 2 });

            if (false === Csw.isNullOrEmpty(cswPrivate.gridData)) {
                var fieldTable = inputDialog.div.table();
                fieldTable.cell(1, 1).div().grid({
                    name: cswPrivate.name + "_grid",
                    stateId: cswPrivate.name + "_stateId",
                    fields: cswPrivate.gridData.fields,
                    columns: cswPrivate.gridData.columns,
                    data: {
                        items: cswPrivate.gridData.items
                    },
                    pageSize: 25,
                    showActionColumn: false,
                    width: 500,
                    height: 300,
                });
            }

            inputDialog.div.button({
                enabledText: 'Yes',
                onClick: function() {
                     cswPrivate.onYes();
                },
            });

            inputDialog.div.button({
                enabledText: 'No',
                onClick: function () {
                    cswPrivate.onNo();
                }
            });

            //inputDialog.open();

            return inputDialog;
        }());
    });
}());