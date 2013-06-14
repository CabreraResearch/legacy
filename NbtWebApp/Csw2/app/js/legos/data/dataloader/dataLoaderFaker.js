/* jshint undef: true, unused: true */
/* global n$:true, window:true, Ext:true, $: true */

(function _proxyFakerClassIIFE(n$) {

    /**
     * Internal class to define a Faker Data Loader. This class cannot be directly instanced.
     */
    var DataLoaderFaker = function() {

        var that = n$.dataLoaders._dataLoader(n$.dataLoaders.constants.types.Faker);
        return that;
    };

    n$.instanceOf.register('DataLoaderFaker', DataLoaderFaker);

    /**
     * Instance a new Proxy. Proxies are the mechanisms by which Stores are populated with data.
     * Currently, only Proxy types of 'memory' are supported.
     * @param type {String} The type of proxy
     */
    n$.dataLoaders.register('dataLoaderFaker', function () {
        
        var ret = new DataLoaderFaker(type);
        
        return ret;
    });

}(window.$nameSpace$));
