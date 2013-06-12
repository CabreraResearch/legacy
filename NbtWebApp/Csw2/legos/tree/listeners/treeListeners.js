/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _listenerIIFE(nameSpace) {

    /**
     * Define the listener methods which are available to this class.
    */
    var treeListeners = Object.create(null);
    treeListeners.afterrender = 'afterrender';
    treeListeners.itemdblclick = 'itemdblclick';

    nameSpace.constant(nameSpace.trees, 'listeners', treeListeners);

    nameSpace.trees.listeners.lift('listeners',
        /**
         * Create a new listeners collection. This returns a listeners object with an add method.
        */
        function listeners() {
            'use strict';
            var ret = nameSpace.makeListeners('treeListeners', 'trees');
            return ret;
        });


}(window.$om$));