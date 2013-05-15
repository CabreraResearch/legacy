/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _proxyClassIIFE() {

    /**
     * Internal class to define a Proxy. This class cannot be directly instanced.
     */
    var Proxy = function(type) {
        var that = this;

        Csw2.property(that, 'type', type);

        return that;
    };

    Csw2.instanceof.lift('Proxy', Proxy);

    /**
     * Instance a new Proxy. Proxies are the mechanisms by which Stores are populated with data.
     * Currently, only Proxy types of 'memory' are supported.
     * @param type {String} The type of proxy
     */
    Csw2.grids.stores.lift('proxy', function(type) {
        if(type !== 'memory') {
            throw new Error('Only proxy types of "memory" are supported.');
        }
        var ret = new Proxy(type);
        
        return ret;
    });

}());