/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _listenerIIFE() {

    /**
     * Define the listener methods which are available to this class.
    */
    var windowListeners = Csw2.object();
    windowListeners.add('beforeclose', 'beforeclose');
    windowListeners.add('beforeshow', 'beforeshow');
    windowListeners.add('show', 'show');
    Csw2.constant(Csw2.okna, 'listeners', windowListeners);
    
    /**
     * Create a new listeners collection. This returns a listeners object with an add method.
    */
    Csw2.okna.listeners.lift('listeners', function () {
        var ret = Csw2.makeListeners('windowListeners', 'okna');
        return ret;
    });


}());