/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function(nameSpace) {
    
    /**
        * Instance a collection of fields to describe a row in the SQL output table
    */
    var SqlFineTuningModel = nameSpace.models.model({
        name: 'Ext.$om$.SqlFineTuningModel',
        dataTypeCollection: [
            ['id'],
            ['tableName'],
            ['tableId'],
            ['extCmpId'],
            ['tableAlias'],
            ['field'],
            ['output', 'boolean'],
            ['expression'],
            ['aggregate'],
            ['alias'],
            ['sortType'],
            ['sortOrder'],
            ['grouping', 'boolean'],
            ['criteria']
        ]
    });

    nameSpace.actions.querybuilder.lift('SqlFineTuningModel', SqlFineTuningModel);

    /**
     * Define the store
    */
    var SqlFineTuningStore = nameSpace.stores.store({ name: 'Ext.$om$.SqlFineTuningStore', model: nameSpace.actions.querybuilder.SqlFineTuningModel });

    /**
     * Put the class into the namespace
    */
    nameSpace.actions.querybuilder.lift('SqlFineTuningStore', SqlFineTuningStore);


    /**
     * Define the grid
    */
    var grid = nameSpace.grids.grid({
        name: 'Ext.$om$.qbFineTuningGrid',
        requires: ['Ext.ux.CheckColumn'],
        id: 'qbFineTuningGrid',
        store: 'SqlFineTuningStore',//new nameSpace.actions.querybuilder.SqlFineTuningStore(),
        alias: ['widget.qbFineTuningGrid'],
        plugins: [window.Ext.create('Ext.grid.plugin.CellEditing', {
            clicksToEdit: 1
        })],
        columnLines: true
    });

    /**
     * Add the listeners
    */
    grid.listeners.add(nameSpace.grids.constants.listeners.render, function(view) {
            this.dd = {};
            this.dd.dropZone = new Ext.grid.ViewDropZone({
                view: view,
                ddGroup: 'SQLTableGridDDGroup',
                handleNodeDrop: function(data, record, position) {
                    // What should happen after the drop?
                }
            });
        })
        .add(nameSpace.grids.constants.listeners.drop, function (node, data, dropRec, dropPosition) {
        // add new rows to the qbFineTuningGrid after a drop
            nameSpace.each(data.records, function(rec) {
                nameSpace.actions.sql.manager.select.fields.addFieldRecord(rec, false);
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
    var actionColumn = nameSpace.grids.columns.actionColumn(false, 'Action', true);
    actionColumn.addItem(
        nameSpace.grids.columns.columnItem('../images/sqlbuilder/up_arrow.gif', 'Move Column Up', function onGetClass(index) {
            return index === 0;
        },
        function onHandler(grid, rowIndex, colIndex) {
            var rec = grid.getStore().getAt(rowIndex);
            moveGridRow(grid, rec, rowIndex, - 1);
        })
    ).addItem(
        nameSpace.grids.columns.columnItem('../images/sqlbuilder/down_arrow.gif', 'Move Column Down', function onGetClass(index, store) {
            return ((index + 1) == store.getCount());
        },
        function onHandler(grid, rowIndex, colIndex) {
            var rec = grid.getStore().getAt(rowIndex);
            moveGridRow(grid, rec, rowIndex, 1);
        })
    ).addItem(
        nameSpace.grids.columns.columnItem('../images/sqlbuilder/remove.gif', 'Remove Column', null, function onHandler(grid, rowIndex, colIndex) {
            var rec = grid.getStore().getAt(rowIndex),
            store, tableId, tableGrid, selectionModel, bDel = true;
            // rec contains column grid model, the one to remove
            // get tableId of original qbSqlWindowTable
            tableId = rec.get('extCmpId');
            // get the sql tables grid and its selection
            tableGrid = Ext.getCmp(tableId).down('gridpanel');
            selectionModel = tableGrid.getSelectionModel();
            Ext.Array.each(selectionModel.getSelection(), function(selection) {
                // deselect the selection wich corresponds to the column
                // we want to remove from the column grid
                if (rec.get('id') == selection.get('id')) {
                    // deselect current selection
                    // deselection will lead to removal, look for method deselect at the qbTableGrid
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
        .add(nameSpace.grids.columns.checkColumn(false, 'Output', true))
        .add(nameSpace.grids.columns.gridColumn(false, 'Expression', true, 0.225, 'textfield'))
        .add(nameSpace.grids.columns.gridColumn(false, 'Aggregate', true, null, 'textfield'))
        .add(nameSpace.grids.columns.gridColumn(false, 'Alias', true, null, 'textfield'))
        .add(nameSpace.grids.columns.gridColumn(false, 'Sort Type', true))
        .add(nameSpace.grids.columns.gridColumn(false, 'Sort Order', true))
        .add(nameSpace.grids.columns.checkColumn(false, 'Grouping', true))
        .add(nameSpace.grids.columns.gridColumn(false, 'Criteria', true, null, 'textfield'));
    
    /**
     *Create the grid
    */
    grid.init();

}(window.$om$));