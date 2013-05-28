/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function() {

    //#region CORE 

    /**
     * Custom Errors
    */
    window.Csw2.makeSubNameSpace('errors');

    /**
     * Type checking
    */
    window.Csw2.makeSubNameSpace('is');

    /**
     * To instance check classes
    */
    window.Csw2.makeSubNameSpace('instanceOf');

    /**
     * Type conversion
    */
    window.Csw2.makeSubNameSpace('to');

    //#endregion CORE 

    //#region ACTIONS 

    /**
     * Actions
    */
    window.Csw2.makeSubNameSpace('actions');

    /**
     * Query Builder
    */
    window.Csw2.actions.makeSubNameSpace('querybuilder');

    /**
     * SQL
    */
    window.Csw2.actions.makeSubNameSpace('sql');

    //#endregion ACTIONS 

    //#region DOM 

    /**
     * The MetaData namespace. Represents the structures of nameSpaceName nodes, elements and properties.
     */
    window.Csw2.makeSubNameSpace('metadata');

    /**
     * The node namespace. Represents an nameSpaceName Node and its properties.
     * [1]: This class is responsible for constructing the DOM getters (properties on this object which reference Nodes in the DOM tree)
     * [2]: This class exposes helper methods which can get/set properties on this instance of the node.
     * [3]: This class validates the execution of these methods (e.g. Is the node still in the DOM; has it been GC'd behind our backs)
     * [4]: Maintaining an im-memory representation of tree with children/parents
     */
    window.Csw2.makeSubNameSpace('node');

    //#endregion DOM

    //#region EXT

    /**
     * Fields
    */
    window.Csw2.makeSubNameSpace('fields');

    /**
     * Models
    */
    window.Csw2.makeSubNameSpace('models');

    /**
     *Grids
    */
    window.Csw2.makeSubNameSpace('grids');

    /**
     * Grids Fields
    */
    window.Csw2.grids.makeSubNameSpace('fields');

    /**
     * Grids Columns
    */
    window.Csw2.grids.makeSubNameSpace('columns');

    /**
     * Grids Listeners
    */
    window.Csw2.grids.makeSubNameSpace('listeners');

    /**
     * Grids Stores
    */
    window.Csw2.grids.makeSubNameSpace('stores');

    /**
     * Panels
    */
    window.Csw2.makeSubNameSpace('panels');

    /**
     * Panel Listeners
    */
    window.Csw2.panels.makeSubNameSpace('listeners');

    /**
     * Trees
    */
    window.Csw2.makeSubNameSpace('trees');

    /**
     * Tree Listeners
    */
    window.Csw2.trees.makeSubNameSpace('listeners');

    //#endregion EXT


}());