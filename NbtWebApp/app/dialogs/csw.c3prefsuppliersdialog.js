/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.dialogs.register('c3PrefSuppliersDialog', function (cswPrivate) {
        'use strict';

        (function _preCtor() {
            cswPrivate.title = 'Set Preferred ACD Suppliers';
            cswPrivate.name = 'C3PrefSuppliersDialog',
            cswPrivate.height = 600;
            cswPrivate.width = 550;
            cswPrivate.selected = '';
            cswPrivate.onSave = cswPrivate.onSave || null;
        }());

        function onOpen() {

            Csw.ajaxWcf.post({
                urlMethod: 'ChemCatCentral/GetACDSuppliers',
                success: function (data) {

                    cswPrivate.supplierList = Csw.dialogs.multiselectedit({
                        opts: data.ACDSuppliers,
                        title: 'c3acd_setPrefSuppliers',
                        required: false,
                        inDialog: false,
                        parent: cswPrivate.table.cell(1, 1).div(),
                        height: 400,
                        width: 500,
                        onChange: function (selected) {
                            cswPrivate.selected = selected;
                        },
                        usePaging: true
                    });
                }
            });
        }//onOpen()

        function onSave() {
            // Get all of the selected options from the control,
            // return them to the server,
            // and save them on the User
            Csw.ajaxWcf.post({
                urlMethod: 'ChemCatCentral/UpdateACDPrefSuppliers',
                data: cswPrivate.selected.toString(),
                success: function (data) {
                    cswPrivate.onSave(cswPrivate.selected);
                    Csw.clientChanges.unsetChanged();
                    cswPrivate.prefSuppliersDialog.close();
                }
            });
        };

        return (function () {
            'use strict';
            cswPrivate.prefSuppliersDialog = Csw.layouts.dialog({
                title: cswPrivate.title,
                width: cswPrivate.width,
                height: cswPrivate.height,
                onOpen: function () {
                    onOpen();
                },
                onClose: cswPrivate.onClose
            });

            cswPrivate.table = cswPrivate.prefSuppliersDialog.div.table();

            cswPrivate.table.cell(2, 1).button({
                enabledText: 'Save',
                onClick: function () {
                    onSave();
                },
            });

            cswPrivate.prefSuppliersDialog.open();

            return cswPrivate.prefSuppliersDialog;
        }());
    });
}());