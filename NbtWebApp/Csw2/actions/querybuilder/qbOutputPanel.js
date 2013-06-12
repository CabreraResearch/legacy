/* global window:true, Ext:true */

/*
 * Responsible for rendering the final SQL output
*/
(function(nameSpace) {

    /*
     * Define a panel
    */
    var panel = nameSpace.panels.panel({
        name: 'Ext.$om$.qbOutputPanel',
        alias: ['widget.qbOutputPanel'],
        id: 'qbOutputPanel'
    });

    panel.listeners.add(nameSpace.panels.constants.listeners.afterlayout, function() {
        window.SyntaxHighlighter.highlight();
    });

    var qbOutputPanel = panel.init();
    nameSpace.actions.querybuilder.lift('qbOutputPanel', qbOutputPanel);

}(window.$om$));