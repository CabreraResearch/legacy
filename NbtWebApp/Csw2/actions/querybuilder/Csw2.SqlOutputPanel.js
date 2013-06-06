/* global window:true, Ext:true */

/*
 * Responsible for rendering the final SQL output
*/
(function() {

    /*
     * Define a panel
    */
    var panel = Csw2.panels.panel({
        name: 'Ext.Csw2.SqlOutputPanel',
        alias: ['widget.SqlOutputPanel'],
        id: 'SqlOutputPanel'
    });

    panel.listeners.add(Csw2.panels.constants.listeners.afterlayout, function() {
        window.SyntaxHighlighter.highlight();
    });

    var SqlOutputPanel = panel.init();
    Csw2.actions.querybuilder.lift('SqlOutputPanel', SqlOutputPanel);

}());