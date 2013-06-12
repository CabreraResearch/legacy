/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _proxyClassIIFE(nameSpace) {

    /**
     * Internal class to define a Proxy. This class cannot be directly instanced.
     */
    var Proxy = function(type) {
        var that = this;

        nameSpace.property(that, 'type', type);

        return that;
    };

    nameSpace.instanceOf.lift('Proxy', Proxy);

    /**
     * Instance a new Proxy. Proxies are the mechanisms by which Stores are populated with data.
     * Currently, only Proxy types of 'memory' are supported.
     * @param type {String} The type of proxy
     */
    nameSpace.stores.lift('proxy', function(type) {
        if(type !== 'memory') {
            throw new Error('Only proxy types of "memory" are supported.');
        }
        var ret = new Proxy(type);
        
        return ret;
    });

}(window.$om$));