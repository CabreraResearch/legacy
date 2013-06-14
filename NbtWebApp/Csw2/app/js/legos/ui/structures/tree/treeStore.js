/* jshint undef: true, unused: true */
/* global n$:true, window:true, Ext:true, $: true */

(function _treelIIFE(n$) {

    /**
     * Private class representing an instance of a tree store. It returns a new n$.tree.treeStore instance.
     * @param rootText {String} The text to display for the root node
     * @param children {Array} [children=[]] An array of tree node children
     * @param dataLoader {n$.dataLoaders.dataLoader} [dataLoader=n$.dataLoaders.dataLoaderMemory] A proxy to render the tree
    */
    var TreeStore = function (rootText, children, dataLoader) {
        'use strict';
        var that = Ext.create('Ext.data.TreeStore', {
            root: n$.trees.treeNode({
                text: rootText,
                expanded: true,
                children: children || []
            }),
            proxy: dataLoader
        });

        return that;
    };

    n$.instanceOf.register('TreeStore', TreeStore);

    n$.trees.register('treeStore',
        /**
         * Create a tree object.
         * @param treeDef.rootText {String} The text to display for the root node
         * @param treeDef.children {Array} [children=[]] An array of tree node children
         * @param treeDef.dataLoader {n$.dataLoaders.dataLoader} [dataLoader=n$.dataLoaders.dataLoaderMemory] A proxy to render the tree
         * @returns {Csw.trees.treeStore} A tree store object.
        */
        function treeStoreFunc(treeDef) {
            'use strict';
            if (!(treeDef)) {
                throw new Error('Cannot instance a tree store without properties');
            }
            if (!(treeDef.dataLoader instanceof n$.instanceOf.DataLoader)) {
                treeDef.dataLoader = n$.dataLoaders.dataLoaderMemory();
            }
            var treeStore = new TreeStore(treeDef.rootText, treeDef.children, treeDef.dataLoader);
            if (treeDef.load) {
                treeStore.load();
            }
            return treeStore;
        });


}(window.$nameSpace$));
