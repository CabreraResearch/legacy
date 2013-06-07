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
    var TreeNode = function(text, children, expanded, leaf) {
        var that = this;

        if (text) {
            Csw2.property(that, 'text', text);
        }
        if (!children) {
            leaf = true;
        } else {
            Csw2.property(that, 'children', children);
            if (Csw2.is.bool(expanded)) {
                Csw2.property(that, 'expanded', expanded);
            }
        }
        

        return that;
    };

    Csw2.instanceOf.lift('TreeNode', TreeNode);

    /**
     * Create a tree object.
     * @returns {Csw.trees.tree} A tree object. Exposese listeners and columns collections. Call init when ready to construct the tree. 
    */
    Csw2.trees.lift('treeNode', function(nodeDef) {
        if (!(nodeDef)) {
            throw new Error('Cannot instance a tree node without properties');
        }
        var tree = new TreeNode();
        return tree;
    });


}());