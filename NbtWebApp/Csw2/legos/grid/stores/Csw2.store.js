/* jshint undef: true, unused: true */
/* global n$:true, window:true, Ext:true, $: true */

(function _storeIIFE(n$) {

    /**
     * A Store is a collection of data that is to be rendered in a View or Panel.
     * This private class can never be directly instanced.
    */
    var Store = function (name, proxy, model) {
        'use strict';
        var that = n$.classDefinition({
            name: name,
            extend: 'Ext.data.Store',
            onDefine: function (classDef) {
                n$.property(classDef, 'autoSync', true);
                n$.property(classDef, 'proxy', proxy || n$.stores.proxy('memory'));
                n$.property(classDef, 'model', model);
            }
        });

        return that;
    };

    n$.instanceOf.lift('Store', Store);

    n$.stores.lift('store',
        /**
         * Instance a new Store for consumption by an Ext view or panel
         * @param name {String} A name for the store class
         * @param proxy {n$.stores.proxy} A proxy for loading data into the store
         * @param model {String} The model of the store
        */
        function store(name, proxy, model) {
            'use strict';
            if (!(proxy instanceof n$.instanceOf.Proxy)) {
                throw new Error('Cannot create a Store without a Proxy');
            }
            var ret = new Store(name, proxy, model);
            return ret;
        });

}(window.$nameSpace$));
