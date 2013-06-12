/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _storeIIFE(nameSpace) {

    /**
     * A Store is a collection of data that is to be rendered in a View or Panel.
     * This private class can never be directly instanced.
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
            }
        });

        return that;
    };

    nameSpace.instanceOf.lift('Store', Store);

    nameSpace.stores.lift('store',
        /**
         * Instance a new Store for consumption by an Ext view or panel
         * @param name {String} A name for the store class
         * @param proxy {nameSpace.stores.proxy} A proxy for loading data into the store
         * @param model {String} The model of the store
        */
        function store(name, proxy, model) {
            'use strict';
            if (!(proxy instanceof nameSpace.instanceOf.Proxy)) {
                throw new Error('Cannot create a Store without a Proxy');
            }
            var ret = new Store(name, proxy, model);
            return ret;
        });

}(window.$om$));