/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _listenerIIFE() {

    /**
     * Define the listener methods which are available to this class.
    */
    var treeListeners = Object.create(null);
    treeListeners.afterrender = 'afterrender';
    treeListeners.itemdblclick = 'itemdblclick';

    Csw2.constant(Csw2.trees, 'listeners', treeListeners);
    
    /**
     * Create a new listeners collection. This returns a listeners object with an add method.
    */
    Csw2.trees.listeners.lift('listeners', function () {
        var ret = Csw2.makeListeners('treeListeners', 'trees');
        return ret;
    });


}());