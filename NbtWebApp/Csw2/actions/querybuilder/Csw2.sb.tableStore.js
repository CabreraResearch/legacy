/* global window, Csw2, Ext:true */

(function() {

    Ext.define('Ext.csw2.SqlTableStore', {
        extend: 'Ext.data.Store',
        autoSync: true,
        model: 'Ext.csw2.SqlTableModel',
        proxy: {
            type: 'memory'
        }
    });

}());