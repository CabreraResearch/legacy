(function () {

    Csw.layouts.register('addNode', function (cswHelpers) {
        var cswPrivate = {};
        Csw.extend(cswPrivate, cswHelpers);

        var cswPublic = {};
        
        cswPublic.activeTabId = 0;

        cswPublic.render = function (div) {
            div.div({ cssclass: 'CswIdentityTabHeader' }).append('Add Node Layout');
            var addPanel = window.Ext.create('Ext.panel.Panel', {
                renderTo: div.getId(),
                layout: {
                    align: 'stretch',
                    padding: 1
                }
            });
            cswPrivate.renderTab(addPanel.id, Csw.int32MinVal);
        };

        return cswPublic;
    });
})();