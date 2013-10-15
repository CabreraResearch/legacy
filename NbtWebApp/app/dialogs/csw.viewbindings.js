/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {


    Csw.dialogs.register('viewbindings', function (cswPrivate) {
        'use strict';

        var cswPublic = {};

        (function _preCtor() {
            cswPrivate.gridData = cswPrivate.gridData || {};

            cswPrivate.gridData.Order = cswPrivate.gridData.Order || { colDefs: [], data: { items: [] } };
            cswPrivate.gridData.Bindings = cswPrivate.gridData.Bindings || { colDefs: [], data: { items: [] } };
            cswPrivate.gridData.Relationships = cswPrivate.gridData.Relationships || { colDefs: [], data: { items: [] } };

            cswPrivate.width = cswPrivate.width || 900;
            cswPrivate.height = cswPrivate.height || 600;
        }());

        cswPrivate.updateDisplayedTab = function (tabName, el, eventObj, callBack) {

        };

        (function _postCtor() {

            var bindingsDialog = Csw.layouts.dialog({
                title: 'View Import Bindings',
                width: cswPrivate.width,
                height: cswPrivate.height
            });
            bindingsDialog.open();

            var tabstrip = bindingsDialog.div.tabStrip({
                onTabSelect: cswPrivate.updateDisplayedTab,
            });
            tabstrip.setSize({ width: cswPrivate.width -100, height: cswPrivate.height - 100 });
            
            tabstrip.setTitle('Import Definition');
            cswPrivate.orderTab = tabstrip.addTab({ title: 'Order' });
            cswPrivate.bindingsTab = tabstrip.addTab({ title: 'Bindings' });
            cswPrivate.relationshipsTab = tabstrip.addTab({ title: 'Relationships' });
        }());


        return cswPublic;
    });
}());