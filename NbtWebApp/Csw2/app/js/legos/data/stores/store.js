/* jshint undef: true, unused: true */
/* global n$:true, window:true, Ext:true, $: true */

(function _storeIIFE(n$) {


    /**
     * A Store is a collection of data that is to be rendered in a View or Panel.
     * This private class can never be directly instanced.
     * @param name {String} A name for the store class
     * @param dataLoader {n$.dataLoaders} A dataLoader for loading data into the store
     * @param dataModel {n$.dataModels.dataModel} The dataModel of the store
    */
    var Store = function (name, dataLoader, dataModel) {
        'use strict';
        var that = n$.classDefinition({
            name: name,
            extend: 'Ext.data.Store',
            onDefine: function (classDef) {
                n$.property(classDef, 'autoSync', true);
                n$.property(classDef, 'proxy', dataLoader || n$.dataLoaders.dataLoaderMemory());
                n$.property(classDef, 'model', dataModel);
                delete classDef.initComponent;
            }
        });

        return that;
    };

    n$.instanceOf.register('Store', Store);

    n$.stores.register('store',
        /**
         * Instance a new Store for consumption by an Ext view or panel
         * @param storeDef.name {String} A name for the store class
         * @param storeDef.dataLoader {n$.dataLoaders} A dataLoader for loading data into the store
         * @param storeDef.dataModel {n$.dataModels.dataModel} The dataModel of the store
         * @returns {n$.stores.store} A n$ store
        */
        function store(storeDef) {
            'use strict';
            if (!storeDef) {
                throw new Error('Cannot create a Store without options.');
            }
            if (!(storeDef.dataLoader instanceof n$.instanceOf.DataLoader)) {
                storeDef.dataLoader = n$.dataLoaders.dataLoaderMemory();
            }
            if (!storeDef.dataModel) {
                throw new Error('Cannot create a Store without a DataModel.');
            }
            var ret = new Store(storeDef.name, storeDef.dataLoader, storeDef.dataModel);
            return ret.init();
        });

}(window.$nameSpace$));
