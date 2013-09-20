/// <reference path="CswApp-vsdoc.js" />
//Q.longStackSupport = true; //enable "full" stack traces across the entire promise chain. Useful but a memory hog. Only for debugging!

window.initMain = window.initMain || function (undefined) {

    Csw.main.onReady.then(function () {

        Csw.main.centerBottomDiv.$.CswLogin('init', {
            onAuthenticate: Csw.main.initAll
        }); // CswLogin

    });


    Csw.main.register('is', (function () {
        var isMulti = false;
        var isOneTimeReset = false;

        var trueOrFalse = function (val) {
            var ret = false;
            if (val === true) {
                ret = true;
            }
            return ret;
        };
        return {
            get multi() {
                return isMulti;
            },
            set multi(nuVal) {
                isMulti = trueOrFalse(nuVal);
                return isMulti;
            },
            toggleMulti: function () {
                isMulti = !isMulti;
                return isMulti;
            },
            get oneTimeReset() {
                var ret = isOneTimeReset;
                if (true === ret) {
                    isOneTimeReset = false;
                }
                return ret;
            },
            set oneTimeReset(nuVal) {
                isOneTimeReset = trueOrFalse(nuVal);
                return isOneTimeReset;
            }
        };
    }()));

    Csw.main.tabsAndProps = null;
    Csw.main.mainMenu = null;
    Csw.main.mainTree = null;
    Csw.main.mainviewselect = null;
    Csw.main.universalsearch = null;

    Csw.main.register('setUsername', function () {
        var originalU = Csw.clientSession.originalUserName();
        var currentU = Csw.clientSession.currentUserName();
        if (Csw.isNullOrEmpty(originalU)) {
            Csw.main.headerUsername.text(currentU + '@' + Csw.clientSession.currentAccessId())
                .$.hover(function () { $(this).prop('title', Csw.clientSession.getExpireTime()); });
        } else {
            Csw.main.headerUsername.text(originalU + ' as ' + currentU + '@' + Csw.clientSession.currentAccessId())
                .$.hover(function () { $(this).prop('title', Csw.clientSession.getExpireTime()); });
        }
    });

    Csw.subscribe(Csw.enums.events.main.reauthenticate, function (eventObj) {
        Csw.main.setUsername();
    });
    

    Csw.main.register('refreshViewSelect', function (onSuccess) {
        Csw.main.viewSelectDiv.empty();
        Csw.main.mainviewselect = Csw.main.viewSelectDiv.viewSelect({
            name: 'mainviewselect',
            onSelect: Csw.main.handleItemSelect,
            onSuccess: onSuccess
        });
        return Csw.main.mainviewselect.promise;
    });
    
    Csw.main.register('initAll', function (onSuccess) {

        Csw.main.setUsername();

        Csw.main.universalsearch = Csw.composites.universalSearch(null, {
            searchBoxParent: Csw.main.searchDiv,
            searchResultsParent: Csw.main.rightDiv,
            searchFiltersParent: Csw.main.leftDiv,
            onBeforeSearch: function () {
                Csw.main.clear({ all: true });
            },
            onAfterSearch: function (search) {
                Csw.main.refreshMainMenu({ nodetypeid: search.getFilterToNodeTypeId() });
            },
            onAfterNewSearch: function (searchid) {
                Csw.clientState.setCurrentSearch(searchid);
            },
            onAddView: function (viewid, viewmode) {
                Csw.main.refreshViewSelect();
            },
            onLoadView: function (viewid, viewmode) {
                Csw.main.handleItemSelect({
                    type: 'view',
                    itemid: viewid,
                    mode: viewmode
                });
            }
        });
        var menu = Csw.main.refreshHeaderMenu();
        var ready = Q.all([
            Csw.main.refreshDashboard(),
            menu.ajax,
            Csw.main.universalsearch.ready,
            Csw.actions.quotaImage(Csw.main.headerQuota)
        ]);
        ready.fail(function (err) {
            Csw.debug.error(err);
        });
        ready.then(function() {
            Csw.main.finishInitAll(onSuccess);
        });
        return ready;
    });// initAll()

    Csw.main.register('finishInitAll', function (onSuccess) {

        // handle querystring arguments
        var loadCurrent = Csw.main.handleQueryString();

        if (false === Csw.isFunction(onSuccess)) {   
            if (loadCurrent) {
                var finishInit = function () {
                    var current = Csw.clientState.getCurrent();
                    if (false === Csw.isNullOrEmpty(current.viewid)) {
                        Csw.main.handleItemSelect({
                            type: 'view',
                            itemid: current.viewid,
                            mode: current.viewmode
                        });
                    } else if (false === Csw.isNullOrEmpty(current.actionname)) {
                        Csw.main.handleItemSelect({
                            type: 'action',
                            name: current.actionname,
                            url: current.actionurl
                        });
                    } else if (false === Csw.isNullOrEmpty(current.reportid)) {
                        Csw.main.handleItemSelect({
                            type: 'report',
                            itemid: current.reportid
                        });
                    } else if (false === Csw.isNullOrEmpty(current.searchid)) {
                        Csw.main.handleItemSelect({
                            type: 'search',
                            itemid: current.searchid
                        });
                    } else {
                        Csw.main.refreshWelcomeLandingPage();
                    }
                };
                finishInit();
            }
        } else {
            Csw.tryExec(onSuccess);
        }


    }); // _finishInitAll()


    //We're ready. Resolve the 'main' promise.
    Csw.main.isReady(true);

};


