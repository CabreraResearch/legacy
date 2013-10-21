/// <reference path="CswApp-vsdoc.js" />

(function _initMain() {

    Csw.main.onReady.then(function() {
        
        Csw.main.register('handleReport', function (reportid) {
            Csw.openPopup("Report.html?reportid=" + reportid);
        });

        Csw.main.register('handleItemSelect', function (options) {

            var o = {
                type: 'view', // Action, Report, View, Search
                mode: 'tree', // Grid, Tree, List
                linktype: 'link', // LandingPageItemType: Link, Text, Add
                itemid: '',
                name: '',
                url: '',
                iconurl: '',
                nodeid: '',
                nodekey: '',
                unhideallgridcols: false,
            };
            if (options) {
                Csw.extend(o, options);
            }

            Csw.main.is.multi = false; /* Case 26134. Revert multi-edit selection when switching views, etc. */
            var linkType = Csw.string(o.linktype).toLowerCase();

            var type = Csw.string(o.type).toLowerCase();

            //Now is a good time to purge outstanding Node-specific events
            Csw.main.universalsearch.enable();

            if (Csw.clientChanges.manuallyCheckChanges()) { // && itemIsSupported()) {
                Csw.main.initGlobalEventTeardown();
                if (false === Csw.isNullOrEmpty(type)) {
                    switch (type) {
                        case 'action':
                            Csw.main.clear({ all: true });
                            Csw.main.handleAction({
                                'actionname': o.name,
                                'actionurl': o.url
                            });
                            break;
                        case 'search':
                            Csw.main.clear({ all: true });
                            Csw.main.universalsearch.restoreSearch(o.itemid);
                            break;
                        case 'report':
                            Csw.main.handleReport(o.itemid);
                            break;
                        case 'view':
                            Csw.main.clear({ all: true });
                            var renderView = function () {

                                Csw.clientState.setCurrentView(o.itemid, o.mode);

                                var linkOpt = {
                                    showempty: false,
                                    forsearch: false
                                };

                                switch (linkType) {
                                    case 'search':
                                        linkOpt.showempty = true;
                                        linkOpt.forsearch = true;
                                        break;
                                }
                                var viewMode = Csw.string(o.mode).toLowerCase();
                                switch (viewMode) {
                                    case 'grid':
                                        Csw.main.getViewGrid({
                                            'viewid': o.itemid, 'nodeid': o.nodeid, 'nodekey': o.nodekey, 'showempty': linkOpt.showempty, 'forsearch': linkOpt.forsearch, 'unhideallgridcols': o.unhideallgridcols
                                        });
                                        break;
                                    case 'table':
                                        Csw.main.getViewTable({ 'viewid': o.itemid }); //, 'nodeid': o.nodeid, 'nodekey': o.nodekey });
                                        break;
                                    default:
                                        Csw.main.refreshNodesTree({ 'viewid': o.itemid, 'viewmode': o.mode, 'nodeid': o.nodeid, 'nodekey': '', 'showempty': linkOpt.showempty, 'forsearch': linkOpt.forsearch });
                                        break;
                                }
                            };

                            if (Csw.isNullOrEmpty(o.mode)) {
                                Csw.ajax.deprecatedWsNbt({
                                    url: 'getViewMode',
                                    data: { ViewId: o.viewid },
                                    success: function (data) {
                                        o.mode = Csw.string(data.viewmode, 'tree');

                                        renderView();
                                    }
                                });
                            } else {
                                renderView();
                            }
                            break;
                    }

                    return Csw.main.refreshViewSelect();
                }
            } //if (Csw.clientChanges.manuallyCheckChanges() && itemIsSupported()) {

        }); //handleItemSelect

    });
}());