/* global window:true, Ext:true */

(function(nameSpace) {

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
    var tree = nameSpace.trees.tree({
        name: 'Ext.$om$.qbTablesTree',
        alias: ['widget.qbTablesTree'],
        id: 'qbTablesTree',
        //TODO: expose
        store: nameSpace.trees.treeStore({
            rootText: 'Tables',
            children: [
                nameSpace.trees.treeNode({ text: 'library' }),
                nameSpace.trees.treeNode({ text: 'shelf' }),
                nameSpace.trees.treeNode({ text: 'floor' }),
                nameSpace.trees.treeNode({ text: 'room' }),
                nameSpace.trees.treeNode({ text: 'book' })]
        })
    });

    /**
     * Add the listeners
    */
    tree.listeners.add(nameSpace.trees.constants.listeners.afterrender, function (extView, eOpts) {
        var that = extView;
        initTreeDragZone(that, tree);
    });

    tree.listeners.add(nameSpace.trees.constants.listeners.itemdblclick, function (extView, record, item, index, e, eOpts) {
        var qbTablePanel;
        // add a qbSqlWindowTable to the qbTablePanel component
        qbTablePanel = Ext.getCmp('qbTablePanel');
        qbTablePanel.add({
            xtype: 'qbSqlWindowTable',
            constrain: true,
            title: record.get('text')
        }).show();
    });
    
    tree.init();

}(window.$om$));


