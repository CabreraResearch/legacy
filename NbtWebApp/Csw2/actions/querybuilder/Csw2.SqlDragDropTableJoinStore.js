/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

/**
* The Tabble Join Store represents the join data bound between columns across tables
*/
(function _joinsStoreIIFE(nameSpace) {

    /**
     * Instance a collection of fields to describe a JOIN in the SQL output table
    */
    var SqlDragDropTableJoinModel = nameSpace.models.model({
        name: 'Ext.$om$.SqlDragDropTableJoinModel',
        dataTypeCollection: [
            ['id'],
            ['leftTableId'],
            ['rightTableId'],
            ['leftTableField'],
            ['rightTableField'],
            ['joinCondition'],
            ['joinType', 'boolean']
        ]
    });

    nameSpace.actions.querybuilder.lift('SqlDragDropTableJoinModel', SqlDragDropTableJoinModel);

    /**
     * Define the store
    */
    var SqlDragDropTableJoinStore = nameSpace.stores.store({ name: 'Ext.$om$.SqlDragDropTableJoinStore', model: nameSpace.actions.querybuilder.SqlDragDropTableJoinModel });

    /**
     * Put the class into the namespace
    */
    nameSpace.actions.querybuilder.lift('SqlDragDropTableJoinStore', SqlDragDropTableJoinStore);



}(window.$om$));