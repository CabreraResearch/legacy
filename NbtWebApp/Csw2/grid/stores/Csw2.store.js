/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _storeIIFE() {

    /**
     * A Store is a collection of data that is to be rendered in a View or Panel.
     * This private class can never be directly instanced.
    */
    var Store = function() {
        var that = Csw2.classDefinition({extend: 'Ext.data.Store' });
        Object.defineProperties(that, {
            autoSync: {
                value: true,
                writable: true,
                configurable: true,
                enumerable: true
            },
            proxy: {
                value: Csw2.grids.stores.proxy('memory'),
                writable: true,
                configurable: true,
                enumerable: true
            },
            model: {
                value: '',
                writable: true,
                configurable: true,
                enumerable: true
            }
        });
        return that;
    };

    Csw2.instanceof.lift('Store', Store);

    /**
     * Instance a new Store for consumption by an Ext view or panel
     * @param proxy {Csw2.proxy} A proxy for loading data into the store
     * @param model {String} The model of the store
    */
    Csw2.grids.stores.lift('store', function(proxy, model) {
        if(!(proxy instanceof Csw2.instanceof.Proxy)) {
            throw new Error('Cannot create a Store without a Proxy');
        }
        var ret = new Store();
        ret.proxy = proxy;
        ret.model = model;
        return ret;
    });

}());