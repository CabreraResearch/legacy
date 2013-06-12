/* jshint undef: true, unused: true */
/* global Ext  */

(function(nameSpace) {
    // Init the singleton.  Any tag-based quick tips will start working.
    Ext.tip.QuickTipManager.init();

    // create main application namespace $om$.sql
    Ext.namespace('$om$.sql');


    Ext.application({
        name: '$om$',
        appFolder: 'sql',
        autoCreateViewport: false,
        errorHandler: function(err) {
            Cs2.console.error(err);
        },
        launch: function() {
            Ext.Error.handle = this.errorHandler;
            // copy application to $om$.sql so that $om$.sql.app can be used as an application singleton
            var qbWindow = Ext.create('Ext.$om$');
            qbWindow.show();
            Ext.apply(nameSpace.sql, this);
        }
    });
}(window.$om$));