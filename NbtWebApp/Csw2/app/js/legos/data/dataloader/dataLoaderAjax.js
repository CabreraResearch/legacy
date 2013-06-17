/* jshint undef: true, unused: true */
/* global n$:true, window:true, Ext:true, $: true */

(function _proxyFakerClassIIFE(n$) {

    /**
     * Internal class to define a Faker Data Loader. This class cannot be directly instanced.
     */
    var DataLoaderAjax = function(url) {

        var that = n$.dataLoaders._dataLoader(n$.dataLoaders.constants.types.ajax);
        n$.property(that, 'url', url);
        return that;
    };

    n$.instanceOf.register('DataLoaderAjax', DataLoaderAjax);

    /**
     * Instance a new Proxy. Proxies are the mechanisms by which Stores are populated with data.
     * Currently, only Proxy types of 'memory' are supported.
     * @param type {String} The type of proxy
     */
    n$.dataLoaders.register('dataLoaderAjax', function (url) {
        
        var ret = new DataLoaderAjax(url);
        
        return ret;
    });

}(window.$nameSpace$));
