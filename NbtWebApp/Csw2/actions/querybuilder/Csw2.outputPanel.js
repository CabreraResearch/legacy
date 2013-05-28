/* global window:true, Ext:true */

(function() {

    var panel = Csw2.panels.panel({
        name: 'Ext.Csw2.SQLOutputPanel',
        alias: ['widget.sqloutputpanel'],
        id: 'SQLOutputPanel'
    });

    panel.listeners.add(Csw2.panels.constants.listeners.afterlayout, function() {
        window.SyntaxHighlighter.highlight();
    });

    panel.init();
    

}());