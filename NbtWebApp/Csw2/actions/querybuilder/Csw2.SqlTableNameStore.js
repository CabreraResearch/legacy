/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

/**
* The Table Store represents the data bound to a Database Table
*/
(function _joinsStoreIIFE() {

    
        /**
         * Instance a collection of fields to describe a table in the table Tree
        */
        var SqlTableNameModel = Csw2.models.model({
            name: 'Ext.Csw2.SqlTableNameModel',
            fields: [
                ['id'],
                ['tableName'],
                ['tableAlias']
            ]
        });

        Csw2.actions.querybuilder.lift('SqlTableNameModel', SqlTableNameModel);

        /**
         * Define the store
        */
        var SqlTableNameStore = Csw2.stores.store({ name: 'Ext.Csw2.SqlTableNameStore', model: Csw2.actions.querybuilder.SqlTableNameModel });

        /**
         * Put the class into the namespace
        */
        Csw2.actions.querybuilder.lift('SqlTableNameStore', SqlTableNameStore);

    
}());