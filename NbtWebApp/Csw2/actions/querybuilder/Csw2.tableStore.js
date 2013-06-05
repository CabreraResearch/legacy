/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

/**
* The Table Store represents the data bound to a Database Table
*/
(function _joinsStoreIIFE() {

    //Csw2.dependsOn(['Csw2.fieldsModel'], function () {



    /**
     * Define the store
    */
    var store = Csw2.stores.store({ name: 'Ext.Csw2.SqlTableStore', model: Csw2.tableModel });
    
    /**
     * Put the class into the namespace
    */
    Csw2.lift('sqlTableStore', store);

    // });

}());