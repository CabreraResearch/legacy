/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function(nameSpace) {

    //#region CORE 

    /**
     * Custom Errors
    */
    nameSpace.makeSubNameSpace('errors');

    /**
     * Type checking
    */
    nameSpace.makeSubNameSpace('is');

    /**
     * To instance check classes
    */
    nameSpace.makeSubNameSpace('instanceOf');

    /**
     * Type conversion
    */
    nameSpace.makeSubNameSpace('to');

    //#endregion CORE 

    //#region ACTIONS 

    /**
     * Actions
    */
    nameSpace.makeSubNameSpace('actions');

    /**
     * Query Builder
    */
    nameSpace.actions.makeSubNameSpace('querybuilder');

    /**
     * SQL
    */
    nameSpace.actions.makeSubNameSpace('sql');

    //#endregion ACTIONS 

    //#region DOM 

    /**
     * The MetaData namespace. Represents the structures of nameSpaceName nodes, elements and properties.
     */
    nameSpace.makeSubNameSpace('metadata');

    /**
     * The node namespace. Represents an nameSpaceName Node and its properties.
     * [1]: This class is responsible for constructing the DOM getters (properties on this object which reference Nodes in the DOM tree)
     * [2]: This class exposes helper methods which can get/set properties on this instance of the node.
     * [3]: This class validates the execution of these methods (e.g. Is the node still in the DOM; has it been GC'd behind our backs)
     * [4]: Maintaining an im-memory representation of tree with children/parents
     */
    nameSpace.makeSubNameSpace('node');

    //#endregion DOM

    //#region EXT

    

    /**
     * Models
    */
    nameSpace.makeSubNameSpace('models');

    /**
     *Grids
    */
    nameSpace.makeSubNameSpace('grids');
    
    /**
     * Grids Columns
    */
    nameSpace.grids.makeSubNameSpace('columns');

    /**
     * Grids Listeners
    */
    nameSpace.grids.makeSubNameSpace('listeners');

    /**
     * Stores
    */
    nameSpace.makeSubNameSpace('stores');

    /**
     * Panels
    */
    nameSpace.makeSubNameSpace('panels');

    /**
     * Panel Listeners
    */
    nameSpace.panels.makeSubNameSpace('listeners');

    /**
     * Trees
    */
    nameSpace.makeSubNameSpace('trees');

    /**
     * Tree Listeners
    */
    nameSpace.trees.makeSubNameSpace('listeners');

    /**
     * Windows.
     * Aside: Since 'window' cannot be used _and_ since few synonyms of the word conjurre the same meaning, use the Russian: okno (window), okna (windows)
    */
    nameSpace.makeSubNameSpace('okna');

    /**
     * Window listeners
    */
    nameSpace.okna.makeSubNameSpace('listeners');


    //#endregion EXT


}(window.$om$));