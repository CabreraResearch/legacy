(function () {

    var isDesignModeVisible = false;
    Csw.designmode.isDesignModeVisible = Csw.designmode.isDesignModeVisible ||
        Csw.designmode.register('isDesignModeVisible', function () {
            /// <summary>
            /// Getter for isSidebarVisible
            /// </summary>
            /// <returns type="">True if the Design Mode sidebar is visible</returns>
            return isDesignModeVisible;
        });

    Csw.layouts.register('designmode', function (options) {

        var cswPrivate = {
            nodeid: '',
            sidebarDiv: Csw.main.sidebarDiv,
            sidebarOptions: {},
            nodeLayoutDiv: Csw.main.rightDiv,
            nodelayoutOptions: {},
            onClose: function () { },
            renderInNewView: false
        };
        if (options) {
            Csw.extend(cswPrivate, options);
        }

        var cswPublic = {};
        var sidebar, nodelayout;

        cswPublic.tearDown = function () {
            if (sidebar) {
                sidebar.tearDown();
            }
            if (nodelayout) {
                nodelayout.tearDown();
            }
            isDesignModeVisible = false;
        };

        (function _pre() {
            if (cswPrivate.renderInNewView) {
                Csw.clientDb.setItem('openDesignMode', true);
                Csw.dialogs.closeAll();
                Csw.main.handleItemSelect({
                    type: 'view',
                    mode: 'tree',
                    nodeid: cswPrivate.nodeid
                });
            } else {
                var sidebarDiv = Csw.designmode.factory(cswPrivate.sidebarDiv, 'sidebar');
                sidebar = sidebarDiv.sidebar(cswPrivate.sidebarOptions);

                cswPrivate.nodelayoutOptions.onClose = function() {
                    cswPublic.tearDown();
                    cswPrivate.onClose();
                };

                nodelayout = Csw.layouts.designmodenodelayout(cswPrivate.nodeLayoutDiv, cswPrivate.nodelayoutOptions);
                nodelayout.setSidebar(sidebar);
                sidebar.setNodeLayout(nodelayout);
            }
        })();

        (function _post() {

            isDesignModeVisible = true;
            Csw.subscribe('designModeTearDown', cswPublic.tearDown);

        })();

        return cswPublic;
    });
}());