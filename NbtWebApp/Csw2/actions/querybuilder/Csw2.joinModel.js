/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function () {

    var fields = Csw2.grids.fields.fields();
    fields.add(Csw2.grids.fields.field('id'))
          .add(Csw2.grids.fields.field('leftTableId'))
          .add(Csw2.grids.fields.field('rightTableId'))
          .add(Csw2.grids.fields.field('leftTableField'))
          .add(Csw2.grids.fields.field('rightTableField'))
          .add(Csw2.grids.fields.field('joinCondition'))
          .add(Csw2.grids.fields.field('joinType', 'boolean'));

    var fieldDef = Csw2.classDefinition({
        name: 'Ext.Csw2.SQLJoin',
        extend: 'Ext.data.Model',
        onDefine: function (def) {
            def.fields = fields.value;
        }
    });
    

    /**
     * Instance a collection of fields to describe a JOIN in the SQL output table
    */

    var joinModel = fieldDef.init();

    Csw2.lift('joinModel', joinModel);
}());