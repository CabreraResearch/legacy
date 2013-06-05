/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

/**
* The Fields Store represents the data bound to a grid
*/
(function _fieldsStoreIIFE() {

    //Csw2.dependsOn(['Csw2.fieldsModel'], function () {

    /**
     * Define the store
    */
    var store = Csw2.stores.store({ name: 'Ext.Csw2.SQLFieldsStore', model: Csw2.fieldsModel }); 

    /**
     * Put the class into the namespace
    */
    Csw2.lift('sqlFieldsStore', store);

    // });

}());