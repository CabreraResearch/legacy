/* global window:true, Ext:true */

(function() {

    Ext.define('Ext.Csw2.JoinStore', {
        extend: 'Ext.data.Store',
        autoSync: true,
        model: 'Ext.Csw2.SQLJoin',
        proxy: {
            type: 'memory'
        }
    });

}());