/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.dialogs.register('alert', function (cswPrivate) {
        'use strict';

        (function _preCtor() {
            cswPrivate.title = cswPrivate.title || 'Alert';
            cswPrivate.height = cswPrivate.height || 200;
            cswPrivate.width = cswPrivate.width || 400;
            cswPrivate.message = cswPrivate.message || '';
            cswPrivate.onOpen = cswPrivate.onOpen || function () { };
            cswPrivate.onClose = cswPrivate.onClose || function () { };
        }());

        return (function () {
            'use strict';
            var alertDialog = Csw.layouts.dialog({
                title: cswPrivate.title,
                width: cswPrivate.width,
                height: cswPrivate.height,
                onOpen: cswPrivate.onOpen,
                onClose: cswPrivate.onClose
            });

            alertDialog.div.span({ text: cswPrivate.message, align: 'center' });
            alertDialog.div.br({ number: 2 });
            alertDialog.div.button({
                enabledText: 'OK',
                onClick: function () {
                    alertDialog.close();
                }
            });

            return alertDialog;
        }());
    });
}());