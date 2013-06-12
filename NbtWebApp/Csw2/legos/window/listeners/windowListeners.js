/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _listenerIIFE(nameSpace) {

    /**
     * Define the listener methods which are available to this class.
    */
    var windowListeners = nameSpace.object();
    windowListeners.add('beforeclose', 'beforeclose');
    windowListeners.add('beforeshow', 'beforeshow');
    windowListeners.add('show', 'show');
    nameSpace.constant(nameSpace.okna, 'listeners', windowListeners);

    nameSpace.okna.listeners.lift('listeners',
        /**
         * Create a new listeners collection. This returns a listeners object with an add method.
        */
        function listeners() {
            'use strict';
            var ret = nameSpace.makeListeners('windowListeners', 'okna');
            return ret;
        });


}(window.$om$));