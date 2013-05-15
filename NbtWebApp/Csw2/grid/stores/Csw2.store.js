/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _storeIIFE() {

    /**
     * A Store is a collection of data that is to be rendered in a View or Panel.
     * This private class can never be directly instanced.
    */
    var Store = function(name, proxy, model) {
        var that = Csw2.classDefinition({
            name: name,
            extend: 'Ext.data.Store',
            onDefine: function(classDef) {
                Csw2.property(classDef, 'autoSync', true);
                Csw2.property(classDef, 'proxy', proxy || Csw2.grids.stores.proxy('memory'));
                Csw2.property(classDef, 'model', model);
            }
        });
        
        return that;
    };

    Csw2.instanceof.lift('Store', Store);

    /**
     * Instance a new Store for consumption by an Ext view or panel
     * @param name {String} A name for the store class
     * @param proxy {Csw2.grids.stores.proxy} A proxy for loading data into the store
     * @param model {String} The model of the store
    */
    Csw2.grids.stores.lift('store', function(name, proxy, model) {
        if(!(proxy instanceof Csw2.instanceof.Proxy)) {
            throw new Error('Cannot create a Store without a Proxy');
        }
        var ret = new Store(name, proxy, model);
        return ret;
    });

}());