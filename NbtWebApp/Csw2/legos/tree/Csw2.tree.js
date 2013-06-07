/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _treelIIFE() {

    /**
     * Private class representing the construnction of a tree. It returns a Csw2.tree.tree instance with collections for adding columns and listeners.
     * @param treeName {String} The ClassName of the tree to associate with ExtJS
     * @param requires {Array} An array of ExtJS dependencies
     * @param extend {String} [extend='Ext.tree.tree'] An ExtJs class name to extend, usually the tree tree
     * @param alias {Array} [alias] An array of aliases to reference the tree
     * @param id {String} An id to uniquely identify the tree
     * @param store {Csw2.trees.stores.store} A store to provide data to the tree
     * @param plugins {Array} An array of plugins to load with the tree
    */
    var Tree = function(name, requires, extend, alias, id, store, plugins) {
        var that = window.Csw2.classDefinition({
            name: name,
            requires: requires,
            extend: extend || 'Ext.tree.Panel',
            alias: alias,
            id: id,
            store: store,
            plugins: plugins,
            namespace: 'trees'
        });

        return that;
    };

    Csw2.instanceOf.lift('Tree', Tree);

    /**
     * Create a tree object.
     * @param treeDef.name {String} The ClassName of the tree to associate with ExtJS
     * @param treeDef.requires {Array} An array of ExtJS dependencies
     * @param treeDef.extend {String} [extend='Ext.tree.tree'] An ExtJs class name to extend, usually the tree tree
     * @param treeDef.alias {Array} [alias] An array of aliases to reference the tree
     * @param treeDef.id {String} An id to uniquely identify the tree
     * @param treeDef.store {Csw2.trees.stores.store} A store to provide data to the tree
     * @param treeDef.plugins {Array} An array of plugins to load with the tree
     * @returns {Csw.trees.tree} A tree object. Exposese listeners and columns collections. Call init when ready to construct the tree. 
    */
    Csw2.trees.lift('tree', function(treeDef) {
        if(!(treeDef)) {
            throw new Error('Cannot instance a tree without properties');
        }
        if (!(treeDef.name)) {
            throw new Error('Cannot instance a tree without a classname');
        }
        var tree = new Tree(treeDef.name, treeDef.requires, treeDef.extend, treeDef.alias, treeDef.id, treeDef.store, treeDef.plugins);
        return tree;
    });


}());