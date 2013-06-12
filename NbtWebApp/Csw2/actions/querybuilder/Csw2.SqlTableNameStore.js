/* jshint undef: true, unused: true */
/* global window:true, Ext:true, $: true */

/**
* The Table Store represents the data bound to a Database Table
*/
(function _joinsStoreIIFE(nameSpace) {


    /**
     * Instance a data type collection to describe a table in the table Tree
    */
    var SqlTableNameModel = nameSpace.models.model({
        name: 'Ext.$om$.SqlTableNameModel',
        dataTypeCollection: [
            ['id'],
            ['tableName'],
            ['tableAlias']
        ]
    });

    nameSpace.actions.querybuilder.lift('SqlTableNameModel', SqlTableNameModel);

    /**
     * Define the store
    */
    var SqlTableNameStore = nameSpace.stores.store({ name: 'Ext.$om$.SqlTableNameStore', model: nameSpace.actions.querybuilder.SqlTableNameModel });

    /**
     * Put the class into the namespace
    */
    nameSpace.actions.querybuilder.lift('SqlTableNameStore', SqlTableNameStore);


}(window.$om$));