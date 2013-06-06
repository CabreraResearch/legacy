/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

/**
* The Tabble Join Store represents the join data bound between columns across tables
*/
(function _joinsStoreIIFE() {
    
    /**
     * Instance a collection of fields to describe a JOIN in the SQL output table
    */
    var SqlDragDropTableJoinModel = Csw2.models.model({
        name: 'Ext.Csw2.SqlDragDropTableJoinModel',
        fields: [
            ['id'],
            ['leftTableId'],
            ['rightTableId'],
            ['leftTableField'],
            ['rightTableField'],
            ['joinCondition'],
            ['joinType', 'boolean']
        ]
    });

    Csw2.actions.querybuilder.lift('SqlDragDropTableJoinModel', SqlDragDropTableJoinModel);

    /**
     * Define the store
    */
    var SqlDragDropTableJoinStore = Csw2.stores.store({ name: 'Ext.Csw2.SqlDragDropTableJoinStore', model: Csw2.actions.querybuilder.SqlDragDropTableJoinModel });

    /**
     * Put the class into the namespace
    */
    Csw2.actions.querybuilder.lift('SqlDragDropTableJoinStore', SqlDragDropTableJoinStore);

    

}());