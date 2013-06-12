/* global window:true, Ext:true */

/*
 * Responsible for rendering the final SQL output
*/
(function() {

    /*
     * Define a panel
    */
    var panel = Csw2.panels.panel({
        name: 'Ext.$om$.qbOutputPanel',
        alias: ['widget.qbOutputPanel'],
        id: 'qbOutputPanel'
    });

    panel.listeners.add(Csw2.panels.constants.listeners.afterlayout, function() {
        window.SyntaxHighlighter.highlight();
    });

    var qbOutputPanel = panel.init();
    Csw2.actions.querybuilder.lift('qbOutputPanel', qbOutputPanel);

}());