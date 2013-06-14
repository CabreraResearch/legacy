/* jshint undef: true, unused: true */
/* global n$:true, window:true, Ext:true, $: true */

(function _proxyMemoryClassIIFE(n$) {

    /**
     * Internal class to define a Memory Data Loader. This class cannot be directly instanced.
     */
    var DataLoaderMemory = function() {
        var that = n$.dataLoaders._dataLoader(n$.dataLoaders.constants.types.memory);
        return that;
    };

    n$.instanceOf.register('DataLoaderMemory', DataLoaderMemory);

    /**
     * Instance a new Memory Data Loader. This is the mechanisms by which Stores are populated with data.
     */
    n$.dataLoaders.register('dataLoaderMemory', function () {
        return new DataLoaderMemory();
    });

}(window.$nameSpace$));
