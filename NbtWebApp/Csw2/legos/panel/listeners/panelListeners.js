/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _listenerIIFE(nameSpace) {

    /**
     * Define the listener methods which are available to this class.
    */
    var panelListeners = nameSpace.object();
    panelListeners.afterlayout = 'afterlayout';
    nameSpace.constant(nameSpace.panels, 'listeners', panelListeners);


    nameSpace.panels.listeners.lift('listeners',
        /**
         * Create a new listeners collection. This returns a listeners object with an add method.
        */
        function panellisteners() {
            'use strict';
            var ret = nameSpace.makeListeners('panelListeners', 'panels');
            return ret;
        });
    

}(window.$om$));