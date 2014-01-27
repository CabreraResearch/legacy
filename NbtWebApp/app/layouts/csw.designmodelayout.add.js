(function () {

    Csw.layouts.register('addNode', function (cswHelpers) {
        var cswPrivate = {};
        Csw.extend(cswPrivate, cswHelpers);

        var cswPublic = {};
        cswPrivate.bodyStyle = {
            background: '#F8F9FB'
        };
        
        cswPublic.activeTabId = 0;
        cswPublic.identityTabId = 0;

        cswPublic.render = function (div) {
            div.div({ cssclass: 'CswIdentityTabHeader' }).append('Add Node Layout');
            var addPanel = window.Ext.create('Ext.panel.Panel', {
                renderTo: div.getId(),
                border: 0,
                layout: {
                    align: 'stretch',
                    padding: 1
                }
            });
            cswPrivate.renderTab(addPanel.id, Csw.int32MinVal, cswPrivate.bodyStyle);
        };

        return cswPublic;
    });
})();