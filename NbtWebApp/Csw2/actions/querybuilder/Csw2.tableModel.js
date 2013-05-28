/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function () {

    //var fields = Csw2.grids.fields.fields();
    //fields.add(Csw2.grids.fields.field('id', 'string'))
    //    .add(Csw2.grids.fields.field('tableName', 'string'))
    //    .add(Csw2.grids.fields.field('tableAlias', 'string'));
          

    //var tableDef = Csw2.classDefinition({
    //    name: 'Ext.Csw2.SqlTableModel',
    //    extend: 'Ext.data.Model',
    //    onDefine: function (def) {
    //        def.fields = fields.value;
    //        delete def.initComponent;
    //    }
    //});
    
    ///**
    // * Instance a collection of fields to describe a JOIN in the SQL output table
    //*/

    //var SqlTableModel = tableDef.init();

   // Csw2.lift('SqlTableModel', SqlTableModel);
    

    Ext.define('Ext.Csw2.SqlTableModel', {
               extend: 'Ext.data.Model',
               fields: [{
                  name: 'id',
                  type: 'string'
              }, {
                      name: 'tableName',
                      type: 'string'
                  }, {
                      name: 'tableAlias',
                      type: 'string'
                  }]
              });

}());