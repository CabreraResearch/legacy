/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function() {

    //Csw2.dependsOn(['Csw2.fields.field'], function () {

        var fields = Csw2.grids.fields.fields();
        fields.add(Csw2.grids.fields.field('id', 'string'))
              .add(Csw2.grids.fields.field('tableName', 'string'))
              .add(Csw2.grids.fields.field('tableId', 'string'))
              .add(Csw2.grids.fields.field('extCmpId', 'string'))
              .add(Csw2.grids.fields.field('tableAlias', 'string'))
              .add(Csw2.grids.fields.field('field', 'string'))
              .add(Csw2.grids.fields.field('output', 'boolean'))
              .add(Csw2.grids.fields.field('expression', 'string'))
              .add(Csw2.grids.fields.field('aggregate', 'string'))
              .add(Csw2.grids.fields.field('alias', 'string'))
              .add(Csw2.grids.fields.field('sortType', 'string'))
              .add(Csw2.grids.fields.field('sortOrder', 'string'))
              .add(Csw2.grids.fields.field('grouping', 'boolean'))
              .add(Csw2.grids.fields.field('criteria', 'string'));

        var fieldDef = Csw2.classDefinition({
            name: 'Ext.Csw2.SQLFieldsModel',
            extend: 'Ext.data.Model'
        });
        fieldDef.fields = fields.value;

        /**
         * Instance a collection of fields to describe a row in the SQL output table
        */
        var fieldsModel = fieldDef.init();

        Csw2.lift('fieldsModel', fieldsModel);


 //   });

}());