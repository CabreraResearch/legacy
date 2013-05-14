/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _gridIIFE() {

    /**
     * Private class representing the construnction of a grid. It returns a Csw2.grid.grid instance with collections for adding columns and listeners.
     * @param gridName {String} The ClassName of the grid to associate with ExtJS
     * @param requires {Array} An array of ExtJS dependencies
     * @param extend {String} [extend='Ext.grid.Panel'] An ExtJs class name to extend, usually the grid panel
     * @param alias {Array} [alias] An array of aliases to reference the grid
     * @param id {String} An id to uniquely identify the grid
     * @param store {Csw2.grids.stores.store} A store to provide data to the grid
     * @param plugins {Array} An array of plugins to load with the grid
     * @param columnLines {Boolean} 
    */
    var Grid = function(gridName, requires, extend, alias, id, store, plugins, columnLines) {
        var that = this;

        var classDef = window.Csw2.classDefinition({
            requires: requires,
            extend: extend,
            alias: alias,
            id: id,
            store: store,
            plugins: plugins
        });
        
        if (columnLines === true || columnLines === false) {
            Csw2.property(that, 'columnLines', columnLines);
        }

        Csw2.property(classDef, 'initComponent', function() {
            this.callParent(arguments);
        });

        Csw2.property(that, 'listeners', Csw2.grids.listeners.listeners());
        Csw2.property(that, 'columns', Csw2.grids.columns.columns());
        
        Csw2.property(that, 'init', function () {
            Csw2.property(classDef, 'viewConfig', {});
            Csw2.property(classDef.viewConfig, 'listeners', that.listeners);

            Csw2.property(classDef, 'columns', that.columns.value);

            return Csw2.define(gridName, classDef);
        });

        return that;
    };

    Csw2.instanceof.lift('Grid', Grid);

    /**
     * Create a grid object.
     * @returns {Csw.grids.grid} A grid object. Exposese listeners and columns collections. Call init when ready to construct the grid. 
    */
    Csw2.grids.lift('grid', function(gridName, gridDef) {
        if(!(gridDef)) {
            throw new Error('Cannot instance a Grid without properties');
        }
        if (!(gridName)) {
            throw new Error('Cannot instance a Grid without a classname');
        }
        var grid = new Grid(gridName, gridDef.requires, gridDef.extend, gridDef.alias, gridDef.id, gridDef.store, gridDef.plugins, gridDef.columnLines);
        return grid;
    });


}());