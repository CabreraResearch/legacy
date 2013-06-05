/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function () {

    /**
     * Instance a collection of fields to describe a JOIN in the SQL output table
    */
    var joinModel = Csw2.models.model({
        name: 'Ext.Csw2.SQLJoin',
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

    Csw2.lift('joinModel', joinModel);
}());