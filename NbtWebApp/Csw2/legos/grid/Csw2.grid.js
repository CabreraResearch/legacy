/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _gridIIFE() {

    /**
     * Define the properties which are available to Grid.
    */
    var gridProperties = Object.create(null);
    gridProperties.columnLines = 'columnLines';
    gridProperties.border = 'border';
    gridProperties.hideHeaders = 'hideHeaders';
    gridProperties.selModel = 'selModel';
    Csw2.constant(Csw2.grids, 'properties', gridProperties);

    /**
     * Private class representing the construnction of a grid. It returns a Csw2.grid.grid instance with collections for adding columns and listeners.
     * @param name {String} The ClassName of the grid to associate with ExtJS
     * @param requires {Array} An array of ExtJS dependencies
     * @param extend {String} [extend='Ext.grid.Panel'] An ExtJs class name to extend, usually the grid panel
     * @param alias {Array} [alias] An array of aliases to reference the grid
     * @param id {String} An id to uniquely identify the grid
     * @param store {Csw2.stores.store} A store to provide data to the grid
     * @param plugins {Array} An array of plugins to load with the grid
     * @param columnLines {Boolean} 
     * @param onInit {Function} [onInit] Optional callback to be applied on construction
    */
    var Grid = function(name, requires, extend, alias, id, store, plugins, columnLines, onInit) {
        var that = window.Csw2.classDefinition({
            name: name,
            requires: requires,
            extend: extend || 'Ext.grid.Panel',
            alias: alias,
            id: id,
            store: store,
            plugins: plugins,
            constant: 'gridProperties',
            namespace: 'grids',
            onDefine: function(classDef) {
                Csw2.property(classDef, 'columns', columns.value);
            }
        });
        
        if (columnLines === true || columnLines === false) {
            Csw2.property(that, Csw2.grids.constants.properties.columnLines, columnLines);
        }

        if (onInit) {
            that.addInitComponent(function(them) {
                onInit(them);
            });
        }
        
        var columns = Csw2.grids.columns.columns();
        Csw2.property(that, 'columnCollection', columns, false, false, false);
        
        return that;
    };

    Csw2.instanceOf.lift('Grid', Grid);

    /**
     * Create a grid object.
     * @returns {Csw.grids.grid} A grid object. Exposese listeners and columns collections. Call init when ready to construct the grid. 
    */
    Csw2.grids.lift('grid', function(gridDef) {
        if(!(gridDef)) {
            throw new Error('Cannot instance a Grid without properties');
        }
        if (!(gridDef.name)) {
            throw new Error('Cannot instance a Grid without a classname');
        }
        var grid = new Grid(gridDef.name, gridDef.requires, gridDef.extend, gridDef.alias, gridDef.id, gridDef.store, gridDef.plugins, gridDef.columnLines, gridDef.onInit);
        return grid;
    });


}());