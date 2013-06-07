/* global window:true, Ext:true */

(function() {

    var initTreeDragZone = function(thisTree, t) {
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
        alias: ['widget.qbTablesTree'],
        id: 'qbTablesTree',
        //TODO: expose
        store: Csw2.trees.treeStore({
            rootText: 'Tables',
            children: [
                Csw2.trees.treeNode({ text: 'library' }),
                Csw2.trees.treeNode({ text: 'shelf' }),
                Csw2.trees.treeNode({ text: 'floor' }),
                Csw2.trees.treeNode({ text: 'room' }),
                Csw2.trees.treeNode({ text: 'book' })]
        })
    });

    /**
     * Add the listeners
    */
    tree.listeners.add(Csw2.trees.constants.listeners.afterrender, function (extView, eOpts) {
        var that = extView;
        initTreeDragZone(that, tree);
    });

    tree.listeners.add(Csw2.trees.constants.listeners.itemdblclick, function (extView, record, item, index, e, eOpts) {
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


