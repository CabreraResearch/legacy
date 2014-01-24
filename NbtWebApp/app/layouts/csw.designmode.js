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
            tabid: '',
            viewid: '',
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

        //Deconstructs the object when leaving design mode
        cswPublic.tearDown = function () {
            if (sidebar) {
                sidebar.tearDown();
            }
            if (nodelayout) {
                nodelayout.tearDown();
            }
            isDesignModeVisible = false;
        };

        //Returns the user to the original view after clicking "Close Design Mode"
        cswPublic.close = function () {
            cswPublic.tearDown();
            Csw.main.handleItemSelect({
                type: 'view',
                mode: 'tree',
                nodeid: cswPrivate.nodeid,
                itemid: cswPrivate.viewid
            });
            cswPrivate.onClose();
        };

        (function _pre() {
            if (cswPrivate.renderInNewView) {
                Csw.clientDb.setItem('openDesignMode', true);
                Csw.dialogs.closeAll();
                cswPublic.close();
            } else {
                var sidebarDiv = Csw.designmode.factory(cswPrivate.sidebarDiv, 'sidebar');
                sidebar = sidebarDiv.sidebar(cswPrivate.sidebarOptions);

                cswPrivate.nodelayoutOptions.onClose = cswPublic.close;
                cswPrivate.nodelayoutOptions.tabid = cswPrivate.tabid;

                nodelayout = Csw.layouts.designmodenodelayout(cswPrivate.nodeLayoutDiv, cswPrivate.nodelayoutOptions);
                nodelayout.setSidebar(sidebar);
                sidebar.setNodeLayout(nodelayout);
                nodelayout.init();

            }
        })();

        (function _post() {

            isDesignModeVisible = true;
            Csw.subscribe('designModeTearDown', cswPublic.tearDown);

        })();

        return cswPublic;
    });
}());