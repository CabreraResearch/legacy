/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _treelIIFE() {

    /**
     * Private class representing the construnction of a tree node. It returns a Csw2.tree.treeNode instance.
     * @param text {String} The text to display
     * @param children {Array} [children] An array of tree node children
     * @param expanded {Boolean} [expanded=false] If children are provided, true to render the node expanded
     * @param leaf {Boolean} [leaf] If true, render the node as a leaf of the tree
     * @param allowDrop {Boolean} [allowDrop=false] If true, allow the node to be dropped
    */
    var TreeNode = function(text, children, expanded, leaf, allowDrop) {
        var that = this;

        if (text) {
            Csw2.property(that, 'text', text);
        }
        if (!children) {
            leaf = true;
        } else {
            Csw2.property(that, 'children', children);
            if (true !== expanded) {
                expanded = false;
            }
            Csw2.property(that, 'expanded', expanded);
        }
        if (true === leaf) {
            Csw2.property(that, 'leaf', true);
        }
        if (true !== allowDrop) {
            allowDrop = false;
        }
        Csw2.property(that, 'allowDrop', allowDrop);
        
        return that;
    };

    Csw2.instanceOf.lift('TreeNode', TreeNode);

    /**
     * Create a tree node object.
     * @param nodeDef.text {String} The text to display
     * @param nodeDef.children {Array} [children] An array of tree node children
     * @param nodeDef.expanded {Boolean} [expanded=false] If children are provided, true to render the node expanded
     * @param nodeDef.leaf {Boolean} [leaf] If true, render the node as a leaf of the tree
     * @param nodeDef.allowDrop {Boolean} [allowDrop=false] If true, allow the node to be dropped
     * @returns {Csw.trees.treeNode} A tree object. Exposese listeners and columns collections. Call init when ready to construct the tree. 
    */
    Csw2.trees.lift('treeNode', function(nodeDef) {
        if (!(nodeDef)) {
            throw new Error('Cannot instance a tree node without properties');
        }
        var node = new TreeNode(nodeDef.text, nodeDef.children, nodeDef.expanded, nodeDef.leaf, nodeDef.allowDrop);
        return node;
    });


}());