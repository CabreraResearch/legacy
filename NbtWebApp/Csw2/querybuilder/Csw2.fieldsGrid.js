/* jshint undef: true, unused: true */
/* global Ext, Csw2  */

(function() {

    /**
     * Define the class definition for the grid
    */
    var gridDef = window.Csw2.classDefinition({
        requires: ['Ext.ux.CheckColumn'],
        extend: 'Ext.grid.Panel',
        alias: ['widget.sqlfieldsgrid'],
        id: 'SQLFieldsGrid',
        store: 'SQLFieldsStore',
        plugins: [window.Ext.create('Ext.grid.plugin.CellEditing', {
            clicksToEdit: 1
        })]
    });

    /**
     * Show column lines
    */
    gridDef.columnLines = true;

    var listeners = Csw2.grids.listeners.listeners();
    listeners.add(Csw2.constants.gridListeners.render, function(view) {
        this.dd = {};
        this.dd.dropZone = new Ext.grid.ViewDropZone({
            view: view,
            ddGroup: 'SQLTableGridDDGroup',
            handleNodeDrop: function(data, record, position) {
                // What should happen after the drop?
            }
        });
    })
        .add(Csw2.constants.gridListeners.drop, function (node, data, dropRec, dropPosition) {
        // add new rows to the SQLFieldsGrid after a drop
            Csw.each(data.records, function(rec) {
                Csw2.sql.builder.sqlSelect.addFieldRecord(rec, false);
            });
    });

    /**
     * Assign listeners to the view
    */
    gridDef.viewConfig = {
        listeners: listeners
    };

    /**
     * init command
    */
    gridDef.initComponent = function() {
        this.callParent(arguments);
    };

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
    var columns = Csw2.grids.columns.columns();
    columns.add(actionColumn)
        .add(Csw2.grids.columns.checkColumn(false, 'Output', true))
        .add(Csw2.grids.columns.gridColumn(false, 'Expression', true, 0.225, 'textfield'))
        .add(Csw2.grids.columns.gridColumn(false, 'Aggregate', true, null, 'textfield'))
        .add(Csw2.grids.columns.gridColumn(false, 'Alias', true, null, 'textfield'))
        .add(Csw2.grids.columns.gridColumn(false, 'Sort Type', true))
        .add(Csw2.grids.columns.gridColumn(false, 'Sort Order', true))
        .add(Csw2.grids.columns.checkColumn(false, 'Grouping', true))
        .add(Csw2.grids.columns.gridColumn(false, 'Criteria', true, null, 'textfield'));

    gridDef.columns = columns.value;

    /**
     *Create the grid
    */
    var fieldsGrid = Csw2.define('Ext.Csw2.SQLFieldsGrid', gridDef);

    Csw2.lift('fieldsGrid', fieldsGrid);


}());