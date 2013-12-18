/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.dialogs.register('goToCart', function (cswPrivate) {
        'use strict';

        (function _preCtor() {
            cswPrivate.title = cswPrivate.title || 'Request Added';
            cswPrivate.height = cswPrivate.height || 150;
            cswPrivate.width = cswPrivate.width || 300;
            cswPrivate.requestName = cswPrivate.requestName || 'Request';
            cswPrivate.message = cswPrivate.message || cswPrivate.requestName + ' has been added to your cart.';
            cswPrivate.onOpen = cswPrivate.onOpen || function () { };
            cswPrivate.onClose = cswPrivate.onClose || function () { };
        }());

        return (function () {
            'use strict';
            
            var goToCartDialog = Csw.layouts.dialog({
                title: cswPrivate.title,
                width: cswPrivate.width,
                height: cswPrivate.height,
                onOpen: cswPrivate.onOpen,
                onClose: cswPrivate.onClose
            });

            goToCartDialog.div.span({ text: cswPrivate.message, align: 'center' });
            goToCartDialog.div.br({ number: 2 });
            var buttonTable = goToCartDialog.div.table();
            buttonTable.cell(1, 1).button({
                enabledText: 'OK',
                onClick: function () {
                    goToCartDialog.close();
                    Csw.publish(Csw.enums.events.main.refreshHeader);
                }
            });
            buttonTable.cell(1, 2).button({
                enabledText: 'View Cart',
                onClick: function () {
                    goToCartDialog.close();
                    Csw.publish(Csw.enums.events.main.clear, { centertop: true, centerbottom: true });
                    Csw.publish(Csw.enums.events.main.handleAction, {actionname: 'submit request'});
                }
            });

            return goToCartDialog;
        }());
    });
}());