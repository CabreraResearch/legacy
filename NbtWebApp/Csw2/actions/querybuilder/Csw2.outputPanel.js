/* global window:true, Ext:true */

(function() {

    var panel = Csw2.panels.panel('Ext.Csw2.SQLOutputPanel', {
        alias: ['widget.sqloutputpanel'],
        id: 'SQLOutputPanel'
    });

    panel.listeners.add(Csw2.constants.panelListeners.afterlayout, function() {
        SyntaxHighlighter.highlight();
    });

    panel.init();
    

}());