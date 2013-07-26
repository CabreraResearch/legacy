/// <reference path="CswApp-vsdoc.js" />

window.initMain = window.initMain || function (undefined) {

    Csw.main.register('is', (function () {
        var isMulti = false;
        var isExtReady = true;
        var isDocumentReady = true;
        var isOneTimeReset = false;
        var isDashDone = false;
        var isMenuDone = false;
        var isSearchDone = false;
        var isQuotaDone = false;

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
            get extReady() {
                return isExtReady;
            },
            set extReady(nuVal) {
                isExtReady = trueOrFalse(nuVal);
                return isExtReady;
            },
            get documentReady() {
                return isDocumentReady;
            },
            set documentReady(nuVal) {
                isDocumentReady = trueOrFalse(nuVal);
                return isDocumentReady;
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
            },
            // see case 29072
            get menuDone() {
                return isMenuDone;
            },
            set menuDone(val) {
                isMenuDone = trueOrFalse(val);
            },
            get dashDone() {
                return isDashDone;
            },
            set dashDone(val) {
                isDashDone = trueOrFalse(val);
            },
            get searchDone() {
                return isSearchDone;
            },
            set searchDone(val) {
                isSearchDone = trueOrFalse(val);
            },
            get quotaDone() {
                return isQuotaDone;
            },
            set quotaDone(val) {
                isQuotaDone = trueOrFalse(val);
            }
        };
    }()));

    Csw.main.register('tabsAndProps', null);
    Csw.main.register('mainMenu', null);
    Csw.main.register('mainTree', null);
    Csw.main.register('mainviewselect', null);
    Csw.main.register('universalsearch', null);

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

    Csw.main.register('refreshDashboard', function (onSuccess) {
        if (false === Csw.main.is.dashDone) {
            Csw.main.headerDashboard.empty();
            Csw.main.headerDashboard.$.CswDashboard({ onSuccess: onSuccess });
        }
    });


    Csw.main.register('refreshViewSelect', function (onSuccess) {
        Csw.main.viewSelectDiv.empty();
        Csw.main.mainviewselect = Csw.main.viewSelectDiv.viewSelect({
            name: 'mainviewselect',
            onSelect: Csw.main.handleItemSelect,
            onSuccess: onSuccess
        });
    });


    Csw.main.onReady.then(function () {

        Csw.main.centerBottomDiv.$.CswLogin('init', {
            onAuthenticate: Csw.main.initAll
        }); // CswLogin

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
            },
            onSuccess: function () {
                Csw.main.is.searchDone = true;
                Csw.main.finishInitAll(onSuccess);
            }
        });

        var ready = Q.all([
            Csw.main.refreshDashboard(),
            Csw.main.refreshHeaderMenu(),
            Csw.main.universalsearch.ready,
            Csw.actions.quotaImage(Csw.main.headerQuota)
        ]);

        ready.then(function () {
            Csw.main.finishInitAll(onSuccess);
        })

    });// initAll()

    Csw.main.register('finishInitAll', function (onSuccess) {

        // handle querystring arguments
        var loadCurrent = Csw.main.handleQueryString();

        if (Csw.isNullOrEmpty(onSuccess)) {
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


