/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _listenerIIFE() {

    /**
     * Define the listener methods which are available to this class.
    */
    var gridListeners = Object.create(null);
    gridListeners.render = 'render';
    gridListeners.drop = 'drop';
    gridListeners.bodyscroll = 'bodyscroll';

    Csw2.constant(Csw2.grids, 'listeners', gridListeners);
    
    /**
     * Create a new listeners collection. This returns a listeners object with an add method.
    */
    Csw2.grids.listeners.lift('listeners', function () {
        var ret = Csw2.makeListeners('gridListeners', 'grids');
        return ret;
    });


}());