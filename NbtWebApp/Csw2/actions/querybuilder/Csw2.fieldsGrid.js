/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function() {

    /**
     * Define the grid
    */
    var grid = Csw2.grids.grid({
        name: 'Ext.Csw2.SQLFieldsGrid',
        requires: ['Ext.ux.CheckColumn'],
        alias: ['widget.sqlfieldsgrid'],
        id: 'SQLFieldsGrid',
        store: 'SQLFieldsStore',
        plugins: [window.Ext.create('Ext.grid.plugin.CellEditing', {
            clicksToEdit: 1
        })],
        columnLines: true
    });

    /**
     * Add the listeners
    */
    grid.listeners.add(Csw2.grids.constants.listeners.render, function(view) {
            this.dd = {};
            this.dd.dropZone = new Ext.grid.ViewDropZone({
                view: view,
                ddGroup: 'SQLTableGridDDGroup',
                handleNodeDrop: function(data, record, position) {
                    // What should happen after the drop?
                }
            });
        })
        .add(Csw2.grids.constants.listeners.drop, function (node, data, dropRec, dropPosition) {
        // add new rows to the SQLFieldsGrid after a drop
            Csw2.each(data.records, function(rec) {
                Csw2.sqlBuilder.sqlSelect.addFieldRecord(rec, false);
            });
    });
    
    /**
     * Field Grid specific method
     * @param grid {Ext.grid.View} the grid
     * @param record {Object} The row in question
     * @param index {Number} The position of the row
     * @param direction {Number} The direction of movement
    */
    var moveGridRow = function(grid, record, index, direction) {
        var store = grid.getStore();
        if (direction < 0) {
            index--;
            if (index < 0) {
                return;
            }
        }
        else {
            index++;
            if (index >= grid.getStore().getCount()) {
                return;
            }
        }
        // prepare manual syncing
        store.suspendAutoSync();
        // disable firing store events
        store.suspendEvents();
        // remove record and insert record at new index
        store.remove(record);
        store.insert(index, record);
        // enable firing store events
        store.resumeEvents();
        store.resumeAutoSync();
        // manual sync the store
        store.sync();
    };

    /**
     * Define the action column
    */
    var actionColumn = Csw2.grids.columns.actionColumn(false, 'Action', true);
    actionColumn.addItem(
        Csw2.grids.columns.columnItem('../images/sqlbuilder/up_arrow.gif', 'Move Column Up', function onGetClass(index) {
            return index === 0;
        },
        function onHandler(grid, rowIndex, colIndex) {
            var rec = grid.getStore().getAt(rowIndex);
            moveGridRow(grid, rec, rowIndex, - 1);
        })
    ).addItem(
        Csw2.grids.columns.columnItem('../images/sqlbuilder/down_arrow.gif', 'Move Column Down', function onGetClass(index, store) {
            return ((index + 1) == store.getCount());
        },
        function onHandler(grid, rowIndex, colIndex) {
            var rec = grid.getStore().getAt(rowIndex);
            moveGridRow(grid, rec, rowIndex, 1);
        })
    ).addItem(
        Csw2.grids.columns.columnItem('../images/sqlbuilder/remove.gif', 'Remove Column', null, function onHandler(grid, rowIndex, colIndex) {
            var rec = grid.getStore().getAt(rowIndex),
            store, tableId, tableGrid, selectionModel, bDel = true;
            // rec contains column grid model, the one to remove
            // get tableId of original sqltable
            tableId = rec.get('extCmpId');
            // get the sql tables grid and its selection
            tableGrid = Ext.getCmp(tableId).down('gridpanel');
            selectionModel = tableGrid.getSelectionModel();
            Ext.Array.each(selectionModel.getSelection(), function(selection) {
                // deselect the selection wich corresponds to the column
                // we want to remove from the column grid
                if (rec.get('id') == selection.get('id')) {
                    // deselect current selection
                    // deselection will lead to removal, look for method deselect at the SQLTableGrid
                    selectionModel.deselect(selection);
                    bDel = false;
                }
            });
            if (bDel) {
                store = grid.getStore();
                store.remove(rec);
            }
        })
    );

    /**
     * Define the columns
    */
    grid.columnCollection.add(actionColumn)
        .add(Csw2.grids.columns.checkColumn(false, 'Output', true))
        .add(Csw2.grids.columns.gridColumn(false, 'Expression', true, 0.225, 'textfield'))
        .add(Csw2.grids.columns.gridColumn(false, 'Aggregate', true, null, 'textfield'))
        .add(Csw2.grids.columns.gridColumn(false, 'Alias', true, null, 'textfield'))
        .add(Csw2.grids.columns.gridColumn(false, 'Sort Type', true))
        .add(Csw2.grids.columns.gridColumn(false, 'Sort Order', true))
        .add(Csw2.grids.columns.checkColumn(false, 'Grouping', true))
        .add(Csw2.grids.columns.gridColumn(false, 'Criteria', true, null, 'textfield'));
    
    /**
     *Create the grid
    */
    var fieldsGrid = grid.init();

    

    /**
     * Hoist the final product
    */
    Csw2.lift('fieldsGrid', fieldsGrid);


}());

/*

var fieldsGrid = Ext.define('Ext.Csw2.SQLFieldsGrid', {
        requires: ['Ext.ux.CheckColumn'],
        extend: 'Ext.grid.Panel',
        alias: ['widget.sqlfieldsgrid'],
        id: 'SQLFieldsGrid',
        store: 'SQLFieldsStore',
        columnLines: true,
        plugins: [Ext.create('Ext.grid.plugin.CellEditing', {
            clicksToEdit: 1
        })],
        viewConfig: {
            listeners: {
                render: function (view) {
                    this.dd = {};
                    this.dd.dropZone = new Ext.grid.ViewDropZone({
                        view: view,
                        ddGroup: 'SQLTableGridDDGroup',
                        handleNodeDrop: function (data, record, position) {
                            // Was soll nach dem Drop passieren?
                        }
                    });
                },
                drop: function (node, data, dropRec, dropPosition) {
                    // add new rows to the SQLFieldsGrid after a drop
                    for (var i = 0, l = data.records.length; i < l; i++) {
                        ux.vqbuilder.sqlSelect.addFieldRecord(data.records[i], false);
                    }
                }
            }
        },
        columns: [{
            xtype: 'actioncolumn',
            menuDisabled: true,
            text: 'Action',
            width: 60,
            moveGridRow: function (grid, record, index, direction) {
                var store = grid.getStore();
                if (direction < 0) {
                    index--;
                    if (index < 0) {
                        return;
                    }
                }
                else {
                    index++;
                    if (index >= grid.getStore().getCount()) {
                        return;
                    }
                }
                // prepare manual syncing
                store.suspendAutoSync();
                // disable firing store events
                store.suspendEvents();
                // remove record and insert record at new index
                store.remove(record);
                store.insert(index, record);
                // enable firing store events
                store.resumeEvents();
                store.resumeAutoSync();
                // manual sync the store
                store.sync();
            },
            items: [{
                icon: 'resources/images/up_arrow.gif',
                tooltip: 'Move Column Up',
                getClass: function (value, metadata, record) {
                    var store, index;
                    store = record.store;
                    index = store.indexOf(record);
                    if (index == 0) {
                        return 'x-action-icon-disabled';
                    }
                    else {
                        return 'x-grid-center-icon';
                    }
                },
                handler: function (grid, rowIndex, colIndex) {
                    var rec = grid.getStore().getAt(rowIndex);
                    this.moveGridRow(grid, rec, rowIndex, -1);
                }
            }, {
                icon: 'resources/images/down_arrow.gif',
                getClass: function (value, metadata, record) {
                    var store, index;
                    store = record.store;
                    index = store.indexOf(record);
                    if ((index + 1) == store.getCount()) {
                        return 'x-action-icon-disabled';
                    }
                    else {
                        return 'x-grid-center-icon';
                    }
                },
                tooltip: 'Move Column Down',
                handler: function (grid, rowIndex, colIndex) {
                    var rec = grid.getStore().getAt(rowIndex);
                    this.moveGridRow(grid, rec, rowIndex, 1);
                }
            }, {
                icon: 'resources/images/remove.gif',
                iconCls: 'x-grid-center-icon',
                tooltip: 'Delete Column',
                handler: function (grid, rowIndex, colIndex) {
                    var rec = grid.getStore().getAt(rowIndex), store, tableId, tableGrid, selectionModel, bDel = true;
                    // rec contains column grid model, the one to remove
                    // get tableId of original sqltable
                    tableId = rec.get('extCmpId');
                    // get the sql tables grid and its selection
                    tableGrid = Ext.getCmp(tableId).down('gridpanel');
                    selectionModel = tableGrid.getSelectionModel();
                    Ext.Array.each(selectionModel.getSelection(), function (selection) {
                        // deselect the selection wich corresponds to the column 
                        // we want to remove from the column grid
                        if (rec.get('id') == selection.get('id')) {
                            // deselect current selection
                            // deselection will lead to removal, look for method deselect at the SQLTableGrid
                            selectionModel.deselect(selection);
                            bDel = false;
                        }
                    });
                    if (bDel) {
                        store = grid.getStore();
                        store.remove(rec);
                    }
                }
            }]
        }, {
            xtype: 'checkcolumn',
            sortable: false,
            text: 'Output',
            flex: 0.075,
            menuDisabled: true,
            dataIndex: 'output',
            align: 'center'
        }, {
            xtype: 'gridcolumn',
            text: 'Expression',
            sortable: false,
            menuDisabled: true,
            flex: 0.225,
            dataIndex: 'expression',
            editor: 'textfield'
        }, {
            xtype: 'gridcolumn',
            text: 'Aggregate',
            flex: 0.125,
            sortable: false,
            menuDisabled: true,
            dataIndex: 'aggregate',
            editor: 'textfield'
        }, {
            xtype: 'gridcolumn',
            text: 'Alias',
            flex: 0.125,
            sortable: false,
            menuDisabled: true,
            dataIndex: 'alias',
            editor: 'textfield'
        }, {
            xtype: 'gridcolumn',
            text: 'Sort Type',
            flex: 0.125,
            sortable: false,
            menuDisabled: true,
            dataIndex: 'sorttype'
        }, {
            xtype: 'gridcolumn',
            text: 'Sort Order',
            flex: 0.125,
            sortable: false,
            menuDisabled: true,
            dataIndex: 'sortorder'
        }, {
            xtype: 'checkcolumn',
            text: 'Grouping',
            flex: 0.075,
            sortable: false,
            menuDisabled: true,
            dataIndex: 'grouping',
            align: 'center'
        }, {
            xtype: 'gridcolumn',
            text: 'Criteria',
            flex: 0.125,
            sortable: false,
            menuDisabled: true,
            dataIndex: 'criteria',
            editor: 'textfield'
        }],
        initComponent: function () {
            this.callParent(arguments);
        }
    });


*/