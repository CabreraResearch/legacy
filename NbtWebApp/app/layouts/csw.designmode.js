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
            sidebarDiv: Csw.main.sidebarDiv,
            sidebarOptions: {},
            nodeLayoutDiv: Csw.main.rightDiv,
            nodelayoutOptions: {}
        };
        if (options) {
            Csw.extend(cswPrivate, options);
        }
        
        var cswPublic = {};
        var sidebar, nodelayout;

        (function _pre() {

            var sidebarDiv = Csw.designmode.factory(cswPrivate.sidebarDiv, 'sidebar');
            sidebar = sidebarDiv.sidebar(cswPrivate.sidebarOptions);

            nodelayout = Csw.layouts.designmodenodelayout(cswPrivate.nodeLayoutDiv, cswPrivate.nodelayoutOptions);

        })();

        cswPublic.tearDown = function () {
            if (sidebar) {
                sidebar.tearDown();
            }
            if (nodelayout) {
                nodelayout.tearDown();
            }
            isDesignModeVisible = false;
        };

        (function _post() {

            isDesignModeVisible = true;
            Csw.subscribe('designModeTearDown', cswPublic.tearDown);
            
        })();

        return cswPublic;
    });
}());