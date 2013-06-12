/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _storeIIFE(nameSpace) {


    /**
     * A Store is a collection of data that is to be rendered in a View or Panel.
     * This private class can never be directly instanced.
     * @param name {String} A name for the store class
     * @param proxy {nameSpace.stores.proxy} A proxy for loading data into the store
     * @param model {nameSpace.models.model} The model of the store
    */
    var Store = function (name, proxy, model) {
        'use strict';
        var that = nameSpace.classDefinition({
            name: name,
            extend: 'Ext.data.Store',
            onDefine: function (classDef) {
                nameSpace.property(classDef, 'autoSync', true);
                nameSpace.property(classDef, 'proxy', proxy || nameSpace.stores.proxy('memory'));
                nameSpace.property(classDef, 'model', model);
                delete classDef.initComponent;
            }
        });

        return that;
    };

    nameSpace.instanceOf.lift('Store', Store);

    nameSpace.stores.lift('store',
        /**
         * Instance a new Store for consumption by an Ext view or panel
         * @param storeDef.name {String} A name for the store class
         * @param storeDef.proxy {nameSpace.stores.proxy} A proxy for loading data into the store
         * @param storeDef.model {nameSpace.models.model} The model of the store
         * @returns {nameSpace.stores.store} A nameSpace store
        */
        function store(storeDef) {
            'use strict';
            if (!storeDef) {
                throw new Error('Cannot create a Store without options.');
            }
            if (!(storeDef.proxy instanceof nameSpace.instanceOf.Proxy)) {
                storeDef.proxy = nameSpace.stores.proxy('memory');
            }
            if (!storeDef.model) {
                throw new Error('Cannot create a Store without a Model.');
            }
            var ret = new Store(storeDef.name, storeDef.proxy, storeDef.model);
            return ret.init();
        });

}(window.$om$));