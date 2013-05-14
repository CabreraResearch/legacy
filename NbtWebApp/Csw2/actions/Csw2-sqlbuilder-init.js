/* jshint undef: true, unused: true */
/* global Ext  */

// Init the singleton.  Any tag-based quick tips will start working.
Ext.tip.QuickTipManager.init();

// create main application namespace Csw2.sql
Ext.namespace('Csw2.sql');

window.initSqlUI([{
                        "allowDrop": false,
                        "text": "library",
                        "leaf": true
                    }, {
                        "allowDrop": false,
                        "text": "floor",
                        "leaf": true
                    }, {
                        "allowDrop": false,
                        "text": "aisle",
                        "leaf": true
                    }, {
                        "allowDrop": false,
                        "text": "shelf",
                        "leaf": true
                    }, {
                        "allowDrop": false,
                        "text": "employee",
                        "leaf": true
                    }, {
                        "allowDrop": false,
                        "text": "schedule",
                        "leaf": true
                    }, {
                        "allowDrop": false,
                        "text": "book",
                        "leaf": true
                    }, {
                        "allowDrop": false,
                        "text": "agency",
                        "leaf": true
                    }, {
                        "allowDrop": false,
                        "text": "author",
                        "leaf": true
                    }]);

Ext.application({
    name: 'Csw2',
    appFolder: 'sql',
    autoCreateViewport: false,
    launch: function(){
        // copy application to Csw2.sql so that Csw2.sql.app can be used as an application singleton
        var qbWindow = Ext.create('Ext.Csw2');
    	qbWindow.show();
        Ext.apply(Csw2.sql, this);
    }
});