/* global window:true, Ext:true */

(function () {

    var panel = Csw2.panels.panel({
        name: 'Ext.Csw2.qbTablePanel',
        alias: ['widget.qbTablePanel'],
        id: 'qbTablePanel'
    });

    var initDropTarget = function(thisPanel) {
        // init draw component inside qbwindow as a DropTarget
        thisPanel.dropTarget = Ext.create('Ext.dd.DropTarget', thisPanel.el, {
            ddGroup: 'sqlDDGroup',
            notifyDrop: function(source, event, data) {
                var qbTablePanel;
                // add a sqltable to the qbTablePanel component
                qbTablePanel = Ext.getCmp('qbTablePanel');
                qbTablePanel.add({
                    xtype: 'sqltable',
                    constrain: true,
                    title: data.records[0].get('text')
                }).show();
                return true;
            }
        });
    };

    panel.addProp('items', [{
        xtype: 'draw',
        listeners: {
            afterrender: function() {
                var thisPanel = this;
                initDropTarget(thisPanel);
            }
        }
    }]);

    panel.init();


}());