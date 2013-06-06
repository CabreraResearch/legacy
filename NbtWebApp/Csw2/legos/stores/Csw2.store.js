/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _storeIIFE() {

    /**
     * A Store is a collection of data that is to be rendered in a View or Panel.
     * This private class can never be directly instanced.
     * @param name {String} A name for the store class
     * @param proxy {Csw2.stores.proxy} A proxy for loading data into the store
     * @param model {Csw2.models.model} The model of the store
    */
    var Store = function(name, proxy, model) {
        var that = Csw2.classDefinition({
            name: name,
            extend: 'Ext.data.Store',
            onDefine: function(classDef) {
                Csw2.property(classDef, 'autoSync', true);
                Csw2.property(classDef, 'proxy', proxy || Csw2.stores.proxy('memory'));
                Csw2.property(classDef, 'model', model);
                delete classDef.initComponent;
            }
        });
        
        return that;
    };

    Csw2.instanceOf.lift('Store', Store);

    /**
     * Instance a new Store for consumption by an Ext view or panel
     * @param storeDef.name {String} A name for the store class
     * @param storeDef.proxy {Csw2.stores.proxy} A proxy for loading data into the store
     * @param storeDef.model {Csw2.models.model} The model of the store
     * @returns {Csw2.stores.store} A Csw2 store
    */
    Csw2.stores.lift('store', function(storeDef) {
        if (!storeDef) {
            throw new Error('Cannot create a Store without options.');
        }
        if (!(storeDef.proxy instanceof Csw2.instanceOf.Proxy)) {
            storeDef.proxy = Csw2.stores.proxy('memory');
        }
        if (!storeDef.model) {
            throw new Error('Cannot create a Store without a Model.');
        }
        var ret = new Store(storeDef.name, storeDef.proxy, storeDef.model);
        return ret.init();
    });

}());