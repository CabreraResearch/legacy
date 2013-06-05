/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function () {

    //Csw2.dependsOn(['Csw2.fields.field'], function () {

    /**
        * Instance a collection of fields to describe a row in the SQL output table
    */
    var fieldsModel = Csw2.models.model({
        name: 'Ext.Csw2.SQLFieldsModel',
        fields: [
            ['id'],
            ['tableName'],
            ['tableId'],
            ['extCmpId'],
            ['tableAlias'],
            ['field'],
            ['output', 'boolean'],
            ['expression'],
            ['aggregate'],
            ['alias'],
            ['sortType'],
            ['sortOrder'],
            ['grouping', 'boolean'],
            ['criteria']
        ]
    });

    Csw2.lift('fieldsModel', fieldsModel);


    //   });

}());

