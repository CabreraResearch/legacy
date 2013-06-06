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
    var TreeStore = function(treeName, requires, extend, alias, id, store, plugins) {
        var that = window.Csw2.classDefinition({
            name: name,
            requires: requires,
            extend: extend || 'Ext.tree.tree',
            alias: alias,
            id: id,
            store: store,
            plugins: plugins,
            constant: 'gridProperties',
            namespace: 'grids',
            onDefine: function (classDef) {
                Csw2.property(classDef, 'columns', columns.value);
            }
        });

        this.store = Ext.create('Ext.data.TreeStore', {
            root: {
                text: 'Tables',
                expanded: true,
                children: this.tables
            },
            proxy: {
                type: 'memory',
                reader: {
                    type: 'json'
                }
            }
        });

        if (onInit) {
            that.addInitComponent(function (them) {
                onInit(them);
            });
        }

        return that;
    };

    Csw2.instanceOf.lift('TreeStore', TreeStore);

    /**
     * Create a tree object.
     * @returns {Csw.trees.tree} A tree object. Exposese listeners and columns collections. Call init when ready to construct the tree. 
    */
    Csw2.trees.lift('treeStore', function(treeDef) {
        if(!(treeDef)) {
            throw new Error('Cannot instance a tree store without properties');
        }
        if (!(treeDef.proxy instanceof Csw2.instanceOf.Proxy)) {
            treeDef.proxy = Csw2.stores.proxy('memory');
        }
        var tree = new Tree(treeName, treeDef.requires, treeDef.extend, treeDef.alias, treeDef.id, treeDef.store, treeDef.plugins);
        return tree;
    });


}());