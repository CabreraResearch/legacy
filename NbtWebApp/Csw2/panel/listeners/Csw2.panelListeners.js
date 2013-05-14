/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _listenerIIFE() {

    /**
     * Define the listener methods which are available to this class.
    */
    var panelListeners = Object.create(null);
    panelListeners.afterlayout = 'afterlayout';

    Csw2.constant('panelListeners', panelListeners);

    /**
     * Create a new listeners collection. This returns a listeners object with an add method.
    */
    Csw2.panels.listeners.lift('listeners', function () {
        var ret = Csw2.makeListeners('panelListeners', 'panels');
        return ret;
    });



}());