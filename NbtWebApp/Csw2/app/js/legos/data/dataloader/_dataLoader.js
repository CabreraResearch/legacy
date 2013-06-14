/// <reference path="../release/nsApp-vsdoc.js" />
/* jshint undef: true, unused: true */
/* global n$:true, window:true, Ext:true, $: true */

(function _dataLoaderClassIIFE(n$) {

    //Init the namespace
    n$.makeSubNameSpace('dataLoaders');

    //Properties
    var loaderProperties = n$.object();
    loaderProperties.add('type', 'type');
    n$.constant(n$.dataLoaders, 'properties', loaderProperties);

    //Supported Types
    var loaderTypes = n$.object();
    loaderTypes.add('memory', 'memory');
    loaderTypes.add('Faker', 'Faker');
    loaderTypes.add('promise', 'promise');
    loaderTypes.add('ajax', 'ajax');
    n$.constant(n$.dataLoaders, 'types', loaderTypes);
    
    /**
     * Internal class to define a Proxy. This class cannot be directly instanced.
     */
    var _DataLoader = function(type) {
        var that = this;

        n$.property(that, 'type', type);

        return that;
    };

    n$.instanceOf.register('DataLoader', _DataLoader);

    /**
     * Instance a new Data Loader (Proxy). Proxies are the mechanisms by which Stores are populated with data.
     * Currently, only Proxy types of 'memory' are supported.
     * @param type {String} The type of loader (e.g. memory, Faker, ajax)
     */
    n$.dataLoaders.register('_dataLoader', function (type) {
        if(!(n$.dataLoaders.constants.types.has(type))) {
            throw new Error('Data Loaders do not support a type of "' + type + '".');
        }
        var ret = new _DataLoader(type);
        
        return ret;
    });

}(window.$nameSpace$));
