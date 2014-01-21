(function () {

    Csw.layouts.register('previewNode', function (cswHelpers) {
        var cswPrivate = {};
        Csw.extend(cswPrivate, cswHelpers);

        var cswPublic = {};

        cswPublic.render = function (div) {
            div.div({ cssclass: 'CswIdentityTabHeader' }).append('Preview Node Layout');
            var previewPanel = window.Ext.create('Ext.panel.Panel', {
                renderTo: div.getId(),
                layout: {
                    align: 'stretch',
                    padding: 1
                }
            });
            cswPrivate.renderTab(previewPanel.id, Csw.int32MinVal);
        };

        return cswPublic;
    });
})();