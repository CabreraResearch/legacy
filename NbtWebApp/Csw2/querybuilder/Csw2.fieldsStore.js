/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

/**
* The Fields Store represents the data bound to a grid
*/
(function _fieldsStoreIIFE() {

    //Csw2.dependsOn(['Csw2.fieldsModel'], function () {

        /**
         * Define the proxy
        */
        var proxy = Csw2.grids.stores.proxy('memory');

        /**
         * Define the store
        */
        var store = Csw2.grids.stores.store(proxy, 'Ext.Csw2.SQLFieldsModel');

        /**
         * Create the ExtJs class
        */
        var SqlFieldStore = Csw2.define('Ext.Csw2.SQLFieldsStore', store);

        /**
         * Put the class into the namespace
        */
        Csw2.lift('sqlFieldsStore', SqlFieldStore);

   // });

}());