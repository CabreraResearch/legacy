/// <reference path="CswApp-vsdoc.js" />

(function _mainTearDown() {

    Csw.main.onReady.then(function() {
        
        Csw.main.register('refreshSelected', function (options) {
            Csw.main.initGlobalEventTeardown();
            if (Csw.clientChanges.manuallyCheckChanges()) {
                var o = {
                    nodeid: '',
                    nodekey: '',
                    nodename: '',
                    iconurl: '',
                    viewid: '',
                    viewmode: '',
                    searchid: '',
                    showempty: false,
                    forsearch: false,
                    IncludeNodeRequired: false
                };
                Csw.extend(o, options);

                if (Csw.isNullOrEmpty(o.viewid)) {
                    o.viewid = Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId);
                }
                if (Csw.isNullOrEmpty(o.viewmode)) {
                    o.viewmode = Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewMode);
                }
                if (Csw.isNullOrEmpty(o.searchid)) {
                    o.searchid = Csw.cookie.get(Csw.cookie.cookieNames.CurrentSearchId);
                }

                if (false === Csw.isNullOrEmpty(o.searchid)) { //if we have a searchid, we are probably looking at a search
                    Csw.main.universalsearch.restoreSearch(o.searchid);
                } else {
                    var viewMode = Csw.string(o.viewmode).toLowerCase();
                    switch (viewMode) {
                        case 'grid':
                            Csw.main.getViewGrid({
                                viewid: o.viewid,
                                nodeid: o.nodeid,
                                nodekey: o.nodekey,
                                showempty: o.showempty,
                                forsearch: o.forsearch
                            });
                            break;
                        case 'list':
                            Csw.main.refreshNodesTree({
                                nodeid: o.nodeid,
                                nodekey: o.nodekey,
                                nodename: o.nodename,
                                viewid: o.viewid,
                                viewmode: o.viewmode,
                                showempty: o.showempty,
                                forsearch: o.forsearch,
                                IncludeNodeRequired: o.IncludeNodeRequired
                            });
                            break;
                        case 'table':
                            Csw.main.getViewTable({
                                viewid: o.viewid //,
                                //                            nodeid: o.nodeid,
                                //                            nodekey: o.nodekey
                            });
                            break;
                        case 'tree':
                            Csw.main.refreshNodesTree({
                                nodeid: o.nodeid,
                                nodekey: o.nodekey,
                                nodename: o.nodename,
                                viewid: o.viewid,
                                viewmode: o.viewmode,
                                showempty: o.showempty,
                                forsearch: o.forsearch,
                                IncludeNodeRequired: o.IncludeNodeRequired
                            });
                            break;
                        default:
                            Csw.main.refreshWelcomeLandingPage();
                            break;
                    } // switch
                } // if (false === Csw.isNullOrEmpty(o.searchid))
            } // if (manuallyCheckChanges())
        }); // refreshSelected()
        Csw.subscribe(Csw.enums.events.main.refreshSelected,
            function (eventObj, opts) {
                Csw.main.refreshSelected(opts);
            });


    });
}());