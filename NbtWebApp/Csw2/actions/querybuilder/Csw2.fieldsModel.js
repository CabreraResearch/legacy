/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function() {

    //Csw2.dependsOn(['Csw2.fields.field'], function () {

        var fields = Csw2.grids.fields.fields();
        fields.add(Csw2.grids.fields.field('id'))
              .add(Csw2.grids.fields.field('tableName'))
              .add(Csw2.grids.fields.field('tableId'))
              .add(Csw2.grids.fields.field('extCmpId'))
              .add(Csw2.grids.fields.field('tableAlias'))
              .add(Csw2.grids.fields.field('field'))
              .add(Csw2.grids.fields.field('output', 'boolean'))
              .add(Csw2.grids.fields.field('expression'))
              .add(Csw2.grids.fields.field('aggregate'))
              .add(Csw2.grids.fields.field('alias'))
              .add(Csw2.grids.fields.field('sortType'))
              .add(Csw2.grids.fields.field('sortOrder'))
              .add(Csw2.grids.fields.field('grouping', 'boolean'))
              .add(Csw2.grids.fields.field('criteria'));

        var fieldDef = Csw2.classDefinition({
            name: 'Ext.Csw2.SQLFieldsModel',
            extend: 'Ext.data.Model',
            onDefine: function (def) {
                def.fields = fields.value;
            }
        });

        /**
         * Instance a collection of fields to describe a row in the SQL output table
        */
        var fieldsModel = fieldDef.init();

    
    

        Csw2.lift('fieldsModel', fieldsModel);


 //   });

}());

//var fieldsModel = Ext.define('Ext.Csw2.SQLFieldsModel', {
//    extend: 'Ext.data.Model',
//    fields: [{
//        name: 'id',
//        type: 'string'
//    }, {
//        name: 'tableName',
//        type: 'string'
//    }, {
//        name: 'tableId',
//        type: 'string'
//    }, {
//        name: 'extCmpId',
//        type: 'string'
//    }, {
//        name: 'tableAlias',
//        type: 'string'
//    }, {
//        name: 'field',
//        type: 'string'
//    }, {
//        name: 'output',
//        type: 'boolean'
//    }, {
//        name: 'expression',
//        type: 'string'
//    }, {
//        name: 'aggregate',
//        type: 'string'
//    }, {
//        name: 'alias',
//        type: 'string'
//    }, {
//        name: 'sortType',
//        type: 'string'
//    }, {
//        name: 'sortOrder',
//        type: 'int'
//    }, {
//        name: 'grouping',
//        type: 'boolean'
//    }, {
//        name: 'criteria',
//        type: 'string'
//    }]
//});