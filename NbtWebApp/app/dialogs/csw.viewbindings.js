/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {


    Csw.dialogs.register('viewbindings', function (cswPrivate) {
        'use strict';

        var cswPublic = {};

        (function _preCtor() {
            cswPrivate.gridData = cswPrivate.gridData || {};

            cswPrivate.gridData.Order = cswPrivate.gridData.Order || { fields: [], columns: [], data: { items: [] } };
            cswPrivate.gridData.Bindings = cswPrivate.gridData.Bindings || { fields: [], columns: [], data: { items: [] } };
            cswPrivate.gridData.Relationships = cswPrivate.gridData.Relationships || { fields: [], columns: [], data: { items: [] } };
            
            //because the CswExtJsGrid object is configured for the old-style webservices, we must massage it a bit to display properly here
               ["Order", "Bindings", "Relationships"].forEach(function (tableName) {
                   for (var itemNo = 0; itemNo < cswPrivate.gridData[tableName].data.items.length; itemNo++) {
                       cswPrivate.gridData[tableName].data.items[itemNo] = cswPrivate.gridData[tableName].data.items[itemNo].Row;
                   }
               });

            cswPrivate.width = cswPrivate.width || 900;
            cswPrivate.height = cswPrivate.height || 600;

            cswPrivate.tabs = {};
            cswPrivate.currentTab = '';

        }());


        cswPrivate.updateDisplayedTab = function (tabName, el, eventObj, callBack) {
            //NOTE: this design assumes that the tab name is the same as the grid for that sheet in cswPrivate.gridData
            
            cswPrivate.tabs[tabName].csw.empty();
            
            var contentGrid = cswPrivate.tabs[tabName].csw.grid({
                fields: cswPrivate.gridData[tabName].fields,
                columns: cswPrivate.gridData[tabName].columns,
                data: cswPrivate.gridData[tabName].data,
                showActionColumn: false,
                width: cswPrivate.width - 100,
                height: cswPrivate.height - 100,
                usePaging: false,
            });
            
            cswPrivate.currentTab = tabName;
        };
        

        (function _postCtor() {
            
         //first build the dialog itself
            var bindingsDialog = Csw.layouts.dialog({
                title: 'View Import Bindings',
                width: cswPrivate.width,
                height: cswPrivate.height
            });
            bindingsDialog.open();

         //create the tab container for the binding grids
            var tabstrip = bindingsDialog.div.tabStrip({
                onTabSelect: cswPrivate.updateDisplayedTab,
            });
            tabstrip.setSize({ width: cswPrivate.width -100, height: cswPrivate.height - 100 });
            
            tabstrip.setTitle('Import Definition');
            cswPrivate.tabs.Order = tabstrip.addTab({ title: 'Order' });
            cswPrivate.tabs.Bindings = tabstrip.addTab({ title: 'Bindings' });
            cswPrivate.tabs.Relationships = tabstrip.addTab({ title: 'Relationships' });

         //set the active tab to a default value
            cswPrivate.updateDisplayedTab('Order');
            

            //create a download button for the current binding
            var downloadBindings = bindingsDialog.div.buttonExt({
                enabledText: 'Download Bindings',
                disabledText: 'Generating File...',
                disableOnClick: false,
                onClick: function () {
                    // Return an .xls file that the User can save
                    var action = 'Services/Import/downloadImportDefinition';
                    
                    var $form = $('<form method="POST" action="' + action + '"></form>').appendTo($('body'));
                    var form = Csw.literals.factory($form);

                    form.input({
                        name: 'importdefname',
                        value: cswPrivate.importDefName,
                    });

                    form.$.submit();
                    form.remove();
                }
            });
        }());


        return cswPublic;
    });
}());