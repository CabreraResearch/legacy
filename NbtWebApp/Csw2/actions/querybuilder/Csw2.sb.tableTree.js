/* global window:true, Ext:true */

(function() {

    var initTreeDragZone = function(thisTree) {
        // init tree view as a ViewDragZone
        thisTree.view.dragZone = new Ext.tree.ViewDragZone({
            view: thisTree.view,
            ddGroup: 'sqlDDGroup',
            dragText: '{0} Selected Table{1}',
            repairHighlightColor: 'c3daf9',
            repairHighlight: Ext.enableFx
        });
    };

    /**
     * Define the grid
    */
    var tree = Csw2.trees.tree({
        name: 'Ext.Csw2.qbTablesTree',
        extend: 'Ext.tree.Panel',
        alias: ['widget.qbTablesTree'],
        id: 'qbTablesTree',
        store: Csw2.treeStore.treeStore()
    });

    /**
     * Add the listeners
    */
    tree.listeners.add(Csw2.trees.constants.listeners.afterrender, function () {
        var that = this;
        initTreeDragZone(that);
    });

    tree.listeners.add(Csw2.trees.constants.listeners.itemdblclick, function () {
        var qbTablePanel;
        // add a sqltable to the qbTablePanel component
        qbTablePanel = Ext.getCmp('qbTablePanel');
        qbTablePanel.add({
            xtype: 'sqltable',
            constrain: true,
            title: record.get('text')
        }).show();
    });
    
    tree.init();

}());