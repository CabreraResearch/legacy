/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _treelIIFE(nameSpace) {

    /**
     * Private class representing an instance of a tree store. It returns a new nameSpace.tree.treeStore instance.
     * @param rootText {String} The text to display for the root node
     * @param children {Array} [children=[]] An array of tree node children
     * @param proxy {String} [proxy='memory'] A proxy to render the tree
    */
    var TreeStore = function (rootText, children, proxy) {
        'use strict';
        var that = Ext.create('Ext.data.TreeStore', {
            root: nameSpace.trees.treeNode({
                text: rootText,
                expanded: true,
                children: children
            }),
            proxy: proxy
        });

        return that;
    };

    nameSpace.instanceOf.lift('TreeStore', TreeStore);

    nameSpace.trees.lift('treeStore',
        /**
         * Create a tree object.
         * @param treeDef.rootText {String} The text to display for the root node
         * @param treeDef.children {Array} [children=[]] An array of tree node children
         * @param treeDef.proxy {String} [proxy='memory'] A proxy to render the tree
         * @returns {Csw.trees.treeStore} A tree store object.
        */
        function treeStoreFunc(treeDef) {
            'use strict';
            if (!(treeDef)) {
                throw new Error('Cannot instance a tree store without properties');
            }
            if (!(treeDef.proxy instanceof nameSpace.instanceOf.Proxy)) {
                treeDef.proxy = nameSpace.stores.proxy('memory');
            }
            var treeStore = new TreeStore(treeDef.rootText, treeDef.children, treeDef.proxy);
            return treeStore;
        });


}(window.$om$));