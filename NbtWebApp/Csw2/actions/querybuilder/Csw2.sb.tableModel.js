/* global window:true, Ext:true */

(function() {

    Ext.define('Ext.csw2.SqlTableModel', {
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