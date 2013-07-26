/// <reference path="CswApp-vsdoc.js" />

window.initMain = window.initMain || function (undefined) {

    Csw.main.register('is', (function() {
        var isMulti = false;
        var isExtReady = true;
        var isDocumentReady = true;
        var isOneTimeReset = false;
        var isDashDone = false;
        var isMenuDone = false;
        var isSearchDone = false;
        var isQuotaDone = false;

        var trueOrFalse = function(val) {
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
            toggleMulti: function() {
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
    
    (function _initMain() {
        Csw.main.body = Csw.main.body || Csw.main.register('body', Csw.domNode({ ID: 'body' })); // case 27563 review K3663 comment 1
        Csw.main.ajaxImage = Csw.main.ajaxImage || Csw.main.register('ajaxImage', Csw.domNode({ ID: 'ajaxImage' }));
        Csw.main.ajaxSpacer = Csw.main.ajaxSpacer || Csw.main.register('ajaxSpacer', Csw.domNode({ ID: 'ajaxSpacer' }));
        Csw.main.centerBottomDiv = Csw.main.centerBottomDiv || Csw.main.register('centerBottomDiv', Csw.domNode({ ID: 'CenterBottomDiv' }));
        Csw.main.centerTopDiv = Csw.main.centerTopDiv || Csw.main.register('centerTopDiv', Csw.domNode({ ID: 'CenterTopDiv' }));
        Csw.main.headerDashboard = Csw.main.headerDashboard || Csw.main.register('headerDashboard', Csw.domNode({ ID: 'header_dashboard' }));
        Csw.main.headerMenu = Csw.main.headerMenu || Csw.main.register('headerMenu', Csw.domNode({ ID: 'header_menu' }));
        Csw.main.headerQuota = Csw.main.headerQuota || Csw.main.register('headerQuota', Csw.domNode({ ID: 'header_quota' }));
        Csw.main.headerUsername = Csw.main.headerUsername || Csw.main.register('headerUsername', Csw.domNode({ ID: 'header_username' }));
        Csw.main.leftDiv = Csw.main.leftDiv || Csw.main.register('leftDiv', Csw.domNode({ ID: 'LeftDiv' }));
        Csw.main.mainMenuDiv = Csw.main.mainMenuDiv || Csw.main.register('mainMenuDiv', Csw.domNode({ ID: 'MainMenuDiv' }));
        Csw.main.rightDiv = Csw.main.rightDiv || Csw.main.register('rightDiv', Csw.domNode({ ID: 'RightDiv' }));
        Csw.main.searchDiv = Csw.main.searchDiv || Csw.main.register('searchDiv', Csw.domNode({ ID: 'SearchDiv' }));
        Csw.main.viewSelectDiv = Csw.main.viewSelectDiv || Csw.main.register('viewSelectDiv', Csw.domNode({ ID: 'ViewSelectDiv' }));
        Csw.main.watermark = Csw.main.watermark || Csw.main.register('watermark', Csw.domNode({ ID: 'watermark' }));

    }());

    Csw.main.initGlobalEventTeardown = Csw.main.initGlobalEventTeardown ||
        Csw.main.register('initGlobalEventTeardown', function () {
            Csw.unsubscribe('onAnyNodeButtonClick'); //omitting a function handle removes all
            Csw.unsubscribe('CswMultiEdit'); //omitting a function handle removes all
            Csw.unsubscribe('CswNodeDelete'); //omitting a function handle removes all
            Csw.publish('initPropertyTearDown');
            Csw.main.is.multi = false;
            Csw.main.is.oneTimeReset = true;
            Csw.clientChanges.unsetChanged();
        });

    Csw.main.register('refreshMain', function(eventObj, data) {
        Csw.clientChanges.unsetChanged();
        Csw.main.is.multi = false;
        Csw.main.clear({ all: true });
        Csw.tryExec(Csw.main.refreshSelected, data);
    });
    Csw.subscribe(Csw.enums.events.main.refresh, Csw.main.refreshMain);

    Csw.main.register('loadImpersonation', function(eventObj, actionData) {
        if (false === Csw.isNullOrEmpty(actionData.userid)) {
            Csw.main.handleImpersonation(actionData.userid, actionData.username, function() {
                Csw.main.initAll(function() {
                    Csw.main.handleItemSelect({
                        itemid: Csw.string(actionData.actionid, actionData.viewid),
                        nodeid: actionData.selectedNodeId,
                        mode: actionData.viewmode,
                        name: actionData.actionname,
                        url: actionData.actionurl,
                        type: actionData.type
                    });
                });
            });
        } else {
            Csw.main.handleItemSelect({
                itemid: Csw.string(actionData.actionid, actionData.viewid),
                nodeid: actionData.selectedNodeId,
                mode: actionData.viewmode,
                name: actionData.actionname,
                url: actionData.actionurl,
                type: actionData.type
            });
        }
    });
    Csw.subscribe(Csw.enums.events.RestoreViewContext, Csw.main.loadImpersonation);


    Csw.ajax.post({
        urlMethod: 'getWatermark',
        success: function (result) {
            if (false === Csw.isNullOrEmpty(result.watermark)) {
                Csw.main.watermark.text(result.watermark);
            }
        }
    });

    Csw.main.register('handleImpersonation', function(userid, username, onSuccess) {
        //var u = Csw.cookie.get(Csw.cookie.cookieNames.Username);
        Csw.ajax.post({
            urlMethod: 'impersonate',
            data: { UserId: userid },
            success: function(data) {
                if (Csw.bool(data.result)) {
                    Csw.tryExec(onSuccess);
                }
            } // success
        }); // ajax
    });

    Csw.main.register('refreshHeaderMenu', function(onSuccess) {
        var u = Csw.cookie.get(Csw.cookie.cookieNames.Username);
        Csw.main.headerMenu.empty();
        Csw.main.headerMenu.menu({
            width: '100%',
            ajax: {
                urlMethod: 'getHeaderMenu',
                data: {}
            },
            onLogout: function() {
                Csw.clientSession.logout();
            },
            onQuotas: function() {
                Csw.main.handleAction({ 'actionname': 'Quotas' });
            },
            onModules: function() {
                Csw.main.handleAction({ 'actionname': 'Modules' });
            },
            onSubmitRequest: function() {
                Csw.main.handleAction({ 'actionname': 'Submit_Request' });
            },
            onSessions: function() {
                Csw.main.handleAction({ 'actionname': 'Sessions' });
            },
            onSubscriptions: function() {
                Csw.main.handleAction({ 'actionname': 'Subscriptions' });
            },
            onLoginData: function() {
                Csw.main.handleAction({ 'actionname': 'Login_Data' });
            },
            onImpersonate: function(userid, username) {
                Csw.main.handleImpersonation(userid, username, function() {
                    Csw.clientState.clearCurrent();
                    Csw.window.location(Csw.getGlobalProp('homeUrl'));
                });
            },
            onEndImpersonation: function() {
                Csw.ajax.post({
                    urlMethod: 'endImpersonation',
                    success: function(data) {
                        if (Csw.bool(data.result)) {
                            Csw.clientState.clearCurrent();
                            Csw.window.location(Csw.getGlobalProp('homeUrl'));
                        }
                    } // success
                }); // ajax
            }, // onEndImpersonation
            onReturnToNbtManager: function() {
                Csw.publish(Csw.enums.events.main.clear, { centertop: true, centerbottom: true });
                var sessionid = Csw.cookie.get(Csw.cookie.cookieNames.SessionId);
                /* case 24669 */
                Csw.cookie.clearAll();
                Csw.ajax.post({
                    urlMethod: 'nbtManagerReauthenticate',
                    success: function(result) {
                        Csw.clientChanges.unsetChanged();
                        Csw.publish(Csw.enums.events.main.reauthenticate, { username: result.username, customerid: result.customerid });
                        Csw.window.location('Main.html');
                    }
                });
            },
            onSuccess: onSuccess
        }); // CswMenuHeader
    });
    Csw.subscribe(Csw.enums.events.main.refreshHeader, function (eventObj, opts) {
        Csw.main.refreshHeaderMenu(opts);
    });

    Csw.main.register('handleQueryString', function() {
        var ret = false;
        var qs = Csw.queryString();

        if (Csw.clientSession.isDebug(qs)) {
            Csw.clientSession.enableDebug();
            if (window.location.pathname.endsWith('Dev.html')) {
                if (Csw.isNullOrEmpty(Csw.cookie.get(Csw.cookie.cookieNames.LogoutPath))) {
                    Csw.cookie.set(Csw.cookie.cookieNames.LogoutPath, 'Dev.html');
                }
                Csw.setGlobalProp('homeUrl', 'Dev.html');
            }
        }

        if (false == Csw.isNullOrEmpty(qs.action)) {
            var actopts = {};
            Csw.extend(actopts, qs);
            Csw.main.handleAction({ actionname: qs.action, ActionOptions: actopts });

        } else if (false == Csw.isNullOrEmpty(qs.viewid)) {
            var setView = function(viewid, viewmode) {
                Csw.main.handleItemSelect({
                    type: 'view',
                    itemid: viewid,
                    mode: viewmode
                });
            };
            if (Csw.isNullOrEmpty(qs.viewmode)) {
                Csw.ajax.post({
                    url: Csw.enums.ajaxUrlPrefix + 'getViewMode',
                    data: { ViewId: qs.viewid },
                    success: function(data) {
                        setView(qs.viewid, Csw.string(data.viewmode, 'tree'));
                    }
                });
            } else {
                setView(qs.viewid, Csw.string(qs.viewmode));
            }

        } else if (false == Csw.isNullOrEmpty(qs.nodeid)) {
            Csw.main.handleItemSelect({
                type: 'view',
                mode: 'tree',
                linktype: 'link',
                nodeid: qs.nodeid
            });

        } else if (false == Csw.isNullOrEmpty(qs.reportid)) {
            Csw.main.handleReport(qs.reportid);
            ret = true; // load the current context (probably the welcome landing page) below the report

        } else if (false == Csw.isNullOrEmpty(qs.clear)) {
            Csw.clientState.clearCurrent();
            ret = true;

        } else {
            ret = true;
        }

        return ret;
    });

    Csw.main.register('setUsername', function() {
        var originalU = Csw.clientSession.originalUserName();
        var currentU = Csw.clientSession.currentUserName();
        if (Csw.isNullOrEmpty(originalU)) {
            Csw.main.headerUsername.text(currentU + '@' + Csw.clientSession.currentAccessId())
                .$.hover(function() { $(this).prop('title', Csw.clientSession.getExpireTime()); });
        } else {
            Csw.main.headerUsername.text(originalU + ' as ' + currentU + '@' + Csw.clientSession.currentAccessId())
                .$.hover(function() { $(this).prop('title', Csw.clientSession.getExpireTime()); });
        }
    });
    Csw.subscribe(Csw.enums.events.main.reauthenticate, function (eventObj) {
        Csw.main.setUsername();
    });

    Csw.main.register('initAll', function(onSuccess) {
        var afterSuccessfulAuthentication = function() {
            Csw.main.setUsername();
            Csw.main.refreshDashboard(function() {
                Csw.main.is.dashDone = true;
                Csw.main.finishInitAll(onSuccess);
            });
            Csw.main.refreshHeaderMenu(function() {
                Csw.main.is.menuDone = true;
                Csw.main.finishInitAll(onSuccess);
            });
            Csw.main.universalsearch = Csw.composites.universalSearch(null, {
                searchBoxParent: Csw.main.searchDiv,
                searchResultsParent: Csw.main.rightDiv,
                searchFiltersParent: Csw.main.leftDiv,
                onBeforeSearch: function() {
                    Csw.main.clear({ all: true });
                },
                onAfterSearch: function(search) {
                    Csw.main.refreshMainMenu({ nodetypeid: search.getFilterToNodeTypeId() });
                },
                onAfterNewSearch: function(searchid) {
                    Csw.clientState.setCurrentSearch(searchid);
                },
                onAddView: function(viewid, viewmode) {
                    Csw.main.refreshViewSelect();
                },
                onLoadView: function(viewid, viewmode) {
                    Csw.main.handleItemSelect({
                        type: 'view',
                        itemid: viewid,
                        mode: viewmode
                    });
                },
                onSuccess: function() {
                    Csw.main.is.searchDone = true;
                    Csw.main.finishInitAll(onSuccess);
                }
            });

            Csw.actions.quotaImage(Csw.main.headerQuota, {
                onSuccess: function() {
                    Csw.main.is.quotaDone = true;
                    Csw.main.finishInitAll(onSuccess);
                }
            });
        };

        Csw.main.centerBottomDiv.$.CswLogin('init', {
            onAuthenticate: afterSuccessfulAuthentication
        }); // CswLogin
    });// initAll()

    Csw.main.register('finishInitAll', function(onSuccess) {
        if (Csw.main.is.menuDone === true &&
            Csw.main.is.quotaDone === true &&
            Csw.main.is.searchDone === true &&
            Csw.main.is.dashDone === true) {

            // handle querystring arguments
            var loadCurrent = Csw.main.handleQueryString();

            if (Csw.isNullOrEmpty(onSuccess)) {
                if (loadCurrent) {
                    var finishInit = function() {
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

            Csw.main.is.menuDone = false;
            Csw.main.is.quotaDone = false;
            Csw.main.is.searchDone = false;
            Csw.main.is.dashDone = false;

        } // if(_headerInitDone == true)
    }); // _finishInitAll()


    Csw.main.register('refreshDashboard', function(onSuccess) {
        if (false === Csw.main.is.dashDone) {
            Csw.main.headerDashboard.empty();
            Csw.main.headerDashboard.$.CswDashboard({ onSuccess: onSuccess });
        }
    });


    Csw.main.register('refreshViewSelect', function(onSuccess) {
        Csw.main.viewSelectDiv.empty();
        Csw.main.mainviewselect = Csw.main.viewSelectDiv.viewSelect({
            name: 'mainviewselect',
            onSelect: Csw.main.handleItemSelect,
            onSuccess: onSuccess
        });
    });

    Csw.main.register('clear', function(options) {
        ///<summary>Clears the contents of the page.</summary>
        ///<param name="options">An object representing the elements to clear: all, left, right, centertop, centerbottom.</param>
        //if (debugOn()) Csw.debug.log('Main.clear()');

        var o = {
            left: false,
            right: false,
            centertop: false,
            centerbottom: false,
            all: false
        };
        if (options) {
            Csw.extend(o, options);
        }

        if (o.all || o.left) {
            Csw.main.leftDiv.empty();
        }
        if (o.all || o.right) {
            Csw.main.rightDiv.empty();
        }
        if (o.all || o.centertop) {
            Csw.main.centerTopDiv.empty();
        }
        if (o.all || o.centerbottom) {
            Csw.main.centerBottomDiv.empty();
        }
        if (o.all) {
            Csw.main.mainMenuDiv.empty();
        }
    });

    Csw.subscribe(Csw.enums.events.main.clear, function (eventObj, opts) {
        Csw.main.clear(opts);
    });

    Csw.main.refreshWelcomeLandingPage = function() {
        Csw.main.universalsearch.enable();
        Csw.main.setLandingPage(function () {
            Csw.layouts.landingpage(Csw.main.centerBottomDiv, {
                name: 'welcomeLandingPage',
                Title: '',
                onLinkClick: Csw.main.handleItemSelect,
                onAddClick: function (itemData) {
                    if (false === Csw.isNullOrEmpty(itemData.ActionName)) {
                        Csw.main.handleAction({ actionname: itemData.ActionName });
                    } else {
                        $.CswDialog('AddNodeDialog', {
                            text: itemData.Text,
                            nodetypeid: itemData.NodeTypeId,
                            onAddNode: function(nodeid, nodekey) {
                                Csw.main.clear({ all: true });
                                Csw.main.refreshNodesTree({ 'nodeid': nodeid, 'nodekey': nodekey, 'IncludeNodeRequired': true });
                            }
                        });
                    }
                },
                onTabClick: function (itemData) {
                    Csw.cookie.set(Csw.cookie.cookieNames.CurrentTabId, itemData.TabId);
                    Csw.main.handleItemSelect(itemData);
                },
                onAddComponent: Csw.main.refreshWelcomeLandingPage,
                landingPageRequestData: {
                    RoleId: ''
                }
            });
        });
    };
    
    Csw.main.register('setLandingPage', function(loadLandingPage) {
        Csw.main.clear({ all: true });
        loadLandingPage();
        Csw.main.refreshMainMenu();
        Csw.main.refreshViewSelect();
    });

    Csw.main.register('refreshLandingPage', function(eventObj, opts) {
        Csw.main.clear({ all: true });
        var layData = {
            ActionId: '',
            RelatedObjectClassId: '',
            RelatedNodeName: '',
            RelatedNodeTypeId: '',
            isConfigurable: false,
            Title: '',
            name: 'CswLandingPage'
        };
        Csw.extend(layData, opts);

        Csw.layouts.landingpage(Csw.main.centerBottomDiv, {
            name: layData.name,
            Title: layData.Title,
            ActionId: layData.ActionId,
            ObjectClassId: layData.RelatedObjectClassId,
            onLinkClick: Csw.main.handleItemSelect,
            onAddClick: function(itemData) {
                if (false === Csw.isNullOrEmpty(itemData.ActionName)) {
                    Csw.main.handleAction({ actionname: itemData.ActionName });
                } else {
                    $.CswDialog('AddNodeDialog', {
                        text: itemData.Text,
                        nodetypeid: itemData.NodeTypeId,
                        relatednodeid: layData.RelatedNodeId,
                        relatednodename: layData.RelatedNodeName,
                        relatednodetypeid: layData.RelatedNodeTypeId,
                        relatedobjectclassid: layData.RelatedObjectClassId,
                        onAddNode: function(nodeid, nodekey) {
                            Csw.main.clear({ all: true });
                            Csw.main.refreshNodesTree({ nodeid: nodeid, nodekey: nodekey, IncludeNodeRequired: true });
                        }
                    });
                }
            },
            onTabClick: function(itemData) {
                Csw.cookie.set(Csw.cookie.cookieNames.CurrentTabId, itemData.TabId);
                Csw.main.handleItemSelect(itemData);
            },
            onButtonClick: function(itemData) {
                Csw.composites.nodeButton(Csw.main.centerBottomDiv, {
                    name: itemData.Text,
                    value: itemData.ActionName,
                    mode: 'landingpage',
                    propId: itemData.NodeTypePropId
                });
            },
            onAddComponent: function() { Csw.publish('refreshLandingPage'); },
            landingPageRequestData: layData,
            onActionLinkClick: function(viewId) {
                Csw.main.handleItemSelect({
                    type: 'view',
                    mode: 'tree',
                    itemid: viewId
                });
            },
            isConfigurable: layData.isConfigurable
        });
        Csw.main.refreshMainMenu();
        Csw.main.refreshViewSelect();

    });
    Csw.subscribe('refreshLandingPage', Csw.main.refreshLandingPage);

    Csw.main.register('handleItemSelect', function(options) {

        var o = {
            type: 'view', // Action, Report, View, Search
            mode: 'tree', // Grid, Tree, List
            linktype: 'link', // LandingPageItemType: Link, Text, Add
            itemid: '',
            name: '',
            url: '',
            iconurl: '',
            nodeid: '',
            nodekey: ''
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
                    var renderView = function() {

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
                            Csw.main.getViewGrid({ 'viewid': o.itemid, 'nodeid': o.nodeid, 'nodekey': o.nodekey, 'showempty': linkOpt.showempty, 'forsearch': linkOpt.forsearch });
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
                        Csw.ajax.post({
                            url: Csw.enums.ajaxUrlPrefix + 'getViewMode',
                            data: { ViewId: o.viewid },
                            success: function(data) {
                                o.mode = Csw.string(data.viewmode, 'tree');

                                renderView();
                            }
                        });
                    } else {
                        renderView();
                    }
                    break;
                }

                Csw.main.refreshViewSelect();
            }
        } //if (Csw.clientChanges.manuallyCheckChanges() && itemIsSupported()) {

    }); //handleItemSelect

    Csw.main.register('handleReport', function(reportid) {
        Csw.openPopup("Report.html?reportid=" + reportid);
    });

    Csw.main.register('refreshMainMenu', function(options) {
        var o = {
            parent: Csw.main.mainMenuDiv,
            viewid: '',
            viewmode: '',
            nodeid: '',
            nodekey: '',
            nodetypeid: '',
            propid: '',
            limitMenuTo: '',
            readonly: false
        };
        Csw.extend(o, options);

        //Csw.main.mainMenuDiv.empty();
        if (Csw.main.mainMenu) {
            Csw.main.mainMenu.abort();
        }

        var menuOpts = {
            width: '',
            ajax: {
                urlMethod: 'getMainMenu',
                data: {
                    ViewId: o.viewid,
                    SafeNodeKey: o.nodekey,
                    NodeTypeId: o.nodetypeid,
                    PropIdAttr: o.propid,
                    LimitMenuTo: o.limitMenuTo,
                    ReadOnly: o.readonly,
                    NodeId: o.nodeid
                }
            },
            onAlterNode: function(nodeid, nodekey) {
                var state = Csw.clientState.getCurrent();
                Csw.main.refreshSelected({ 'nodeid': nodeid, 'nodekey': nodekey, 'IncludeNodeRequired': true, 'searchid': state.searchid });
            },
            onMultiEdit: function() {
                switch (o.viewmode) {
                case Csw.enums.viewMode.grid.name:
                    o.nodeGrid.grid.toggleShowCheckboxes();
                    break;
                default:
                    Csw.publish('CswMultiEdit', {
                        multi: Csw.main.is.toggleMulti(),
                        nodeid: o.nodeid,
                        viewid: o.viewid
                    });
                    //refreshSelected({ nodeid: o.nodeid, viewmode: o.viewmode, nodekey: o.nodekey });
                    break;
                } // switch
            },
            onEditView: function() {
                Csw.main.handleAction({
                    'actionname': 'Edit_View',
                    'ActionOptions': {
                        'viewid': Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId),
                        'viewmode': Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewMode)
                    }
                });
            },
            onSaveView: function(newviewid) {
                Csw.main.handleItemSelect({ 'viewid': newviewid, 'viewmode': Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewMode) });
            },
            onPrintView: function() {
                switch (o.viewmode) {
                case Csw.enums.viewMode.grid.name:
                    if (false == Csw.isNullOrEmpty(o.nodeGrid.grid)) {
                        o.nodeGrid.grid.print();
                    }
                    break;
                default:
                    Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, 'View Printing is not enabled for views of type ' + o.viewmode));
                    break;
                }
            },
            Multi: Csw.main.is.multi,
            viewMode: o.viewmode,
            nodeTreeCheck: Csw.main.mainTree,
            nodeGrid: o.nodeGrid
        };

        Csw.main.mainMenu = o.parent.menu(menuOpts);

    });

    Csw.main.register('getViewGrid', function(options) {
        var o = {
            viewid: '',
            nodeid: '',
            showempty: false,
            nodekey: '',
            doMenuRefresh: true,
            onAddNode: '',
            onEditNode: '',
            onDeleteNode: '',
            onRefresh: ''
        };

        Csw.extend(o, options);

        // Defaults
        var getEmptyGrid = (Csw.bool(o.showempty));
        if (Csw.isNullOrEmpty(o.nodeid)) {
            o.nodeid = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeId);
        }
        if (Csw.isNullOrEmpty(o.nodekey)) {
            o.nodekey = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeKey);
        }
        if (false === Csw.isNullOrEmpty(o.viewid)) {
            Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId);
        }

        o.onEditNode = function() { nodeGrid.grid.reload(true); };
        o.onDeleteNode = function() { nodeGrid.grid.reload(true); };
        o.onRefresh = function(options) {
            Csw.main.clear({ centertop: true, centerbottom: true });
            Csw.clientChanges.unsetChanged();
            Csw.main.is.multi = false; // semi-kludge for multi-edit batch op
            Csw.main.refreshSelected(options);
        };
        Csw.main.clear({ centertop: true, centerbottom: true });

        Csw.nbt.viewFilters({
            name: 'main_viewfilters',
            parent: Csw.main.centerTopDiv,
            viewid: o.viewid,
            onEditFilters: function(newviewid) {
                var newopts = o;
                newopts.viewid = newviewid;
                // set the current view to be the session view, so filters are saved
                Csw.clientState.setCurrentView(newviewid, Csw.enums.viewMode.grid.name);
                Csw.main.getViewGrid(newopts);
            } // onEditFilters
        }); // viewFilters
        var div = Csw.main.centerBottomDiv.div({ suffix: window.Ext.id() });
        div.empty();
        var nodeGrid = Csw.nbt.nodeGrid(div, {
            viewid: o.viewid,
            nodeid: o.nodeid,
            nodekey: o.nodekey,
            showempty: getEmptyGrid,
            onEditNode: o.onEditNode,
            onDeleteNode: o.onDeleteNode,
            onRefresh: o.onRefresh,
            onSuccess: function(thisNodeGrid) {
                if (o.doMenuRefresh) {
                    Csw.main.refreshMainMenu({
                        viewid: o.viewid,
                        viewmode: Csw.enums.viewMode.grid.name,
                        nodeGrid: thisNodeGrid
                    });
                }
            },
            onEditView: function(viewid) {
                Csw.main.handleAction({
                    'actionname': 'Edit_View',
                    'ActionOptions': {
                        viewid: viewid,
                        viewmode: Csw.enums.viewMode.grid.name,
                        startingStep: 2,
                        IgnoreReturn: true
                    }
                });
            }
        });
    });


    Csw.main.register('getViewTable', function(options) {
        var o = {
            viewid: '',
            //            nodeid: '',
            //            nodekey: '',
            //			doMenuRefresh: true,
            //			onAddNode: '',
            onEditNode: '',
            onDeleteNode: ''
        };
        if (options) {
            Csw.extend(o, options);
        }

        // Defaults
        if (Csw.isNullOrEmpty(o.nodeid)) {
            o.nodeid = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeId);
        }
        if (Csw.isNullOrEmpty(o.nodekey)) {
            o.nodekey = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeKey);
        }
        if (false === Csw.isNullOrEmpty(o.viewid)) {
            Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId);
        }

        o.onEditNode = function() { Csw.main.getViewTable(o); };
        o.onDeleteNode = function() { Csw.main.getViewTable(o); };

        Csw.main.clear({ centertop: true, centerbottom: true });

        Csw.nbt.viewFilters({
            name: 'main_viewfilters',
            parent: Csw.main.centerTopDiv,
            viewid: o.viewid,
            onEditFilters: function(newviewid) {
                var newopts = o;
                newopts.viewid = newviewid;
                // set the current view to be the session view, so filters are saved
                Csw.clientState.setCurrentView(newviewid, Csw.enums.viewMode.table.name);
                Csw.main.getViewTable(newopts);
            } // onEditFilters
        }); // viewFilters

        Csw.nbt.nodeTable(Csw.main.centerBottomDiv, {
            viewid: o.viewid,
            //            nodeid: o.nodeid,
            //            nodekey: o.nodekey,
            Multi: Csw.main.is.multi,
            //'onAddNode': o.onAddNode,
            onEditNode: o.onEditNode,
            onDeleteNode: o.onDeleteNode,
            onSuccess: function() {
                Csw.main.refreshMainMenu({
                    viewid: o.viewid,
                    viewmode: Csw.enums.viewMode.table.name//,
                    //                    nodeid: o.nodeid,
                    //                    nodekey: o.nodekey
                });
            },
            onNoResults: Csw.main.showDefaultContentTable
        });
    });

    Csw.main.register('onSelectTreeNode', function(options) {
        //if (debugOn()) Csw.debug.log('Main.onSelectTreeNode()');
        if (Csw.clientChanges.manuallyCheckChanges()) {
            var o = {
                tree: null,
                viewid: '',
                nodeid: '',
                nodename: '',
                iconurl: '',
                nodekey: ''
            };
            Csw.extend(o, options);

            Csw.cookie.set(Csw.cookie.cookieNames.CurrentNodeId, o.nodeid);
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentNodeKey, o.nodekey);

            if (o.nodeid !== '' && o.nodeid !== 'root') {
                Csw.main.getTabs({
                    viewid: o.viewid,
                    nodeid: o.nodeid,
                    nodekey: o.nodekey
                });
                Csw.main.refreshMainMenu({
                    parent: o.tree.menuDiv,
                    viewid: o.viewid,
                    viewmode: Csw.enums.viewMode.tree.name,
                    nodeid: o.nodeid,
                    nodekey: o.nodekey
                });
            } else {
                Csw.main.showDefaultContentTree({ viewid: o.viewid, viewmode: Csw.enums.viewMode.tree.name });
                Csw.main.refreshMainMenu({
                    parent: o.tree.menuDiv,
                    viewid: o.viewid,
                    viewmode: Csw.enums.viewMode.tree.name,
                    nodeid: '',
                    nodekey: ''
                });
            }
        }
    }); // onSelectTreeNode()

    Csw.main.register('showDefaultContentTree', function(viewopts) {
        var v = {
            viewid: '',
            viewmode: '',
            onAddNode: function(nodeid, nodekey) {
                Csw.main.refreshSelected({ 'nodeid': nodeid, 'nodekey': nodekey, 'IncludeNodeRequired': true });
            }
        };
        if (viewopts) Csw.extend(v, viewopts);
        Csw.main.clear({ right: true });
        Csw.main.rightDiv.$.CswDefaultContent(v);

    }); // showDefaultContentTree()

    Csw.main.register('showDefaultContentTable', function(viewopts) {
        var v = {
            viewid: '',
            viewmode: '',
            onAddNode: function(nodeid, nodekey) {
                Csw.main.refreshSelected({ 'nodeid': nodeid, 'nodekey': nodekey, 'IncludeNodeRequired': true });
            }
        };
        if (viewopts) Csw.extend(v, viewopts);
        Csw.main.clear({ centerbottom: true });
        var div = Csw.main.centerBottomDiv.div({
            name: 'deftbldiv',
            align: 'center'
        });
        div.css({ textAlign: 'center' });
        div.append('No Results.');

        div.$.CswDefaultContent(v);

    }); // showDefaultContentTable()

    Csw.main.register('getTabs', function(options) {
        Csw.publish('initPropertyTearDown');
        var o = {
            nodeid: '',
            nodekey: '',
            viewid: ''
        };
        Csw.extend(o, options);

        Csw.main.clear({ right: true });

        if (Csw.main.is.oneTimeReset ||
            Csw.isNullOrEmpty(Csw.main.tabsAndProps) ||
            o.viewid !== Csw.main.tabsAndProps.getViewId()) {
            Csw.main.tabsAndProps = Csw.layouts.tabsAndProps(Csw.main.rightDiv, {
                name: 'nodetabs',
                tabState: {
                    viewid: o.viewid,
                    ShowCheckboxes: Csw.main.is.multi,
                    tabid: Csw.cookie.get(Csw.cookie.cookieNames.CurrentTabId),
                    nodeid: o.nodeid,
                    nodekey: o.nodekey
                },
                onSave: function() {
                    Csw.clientChanges.unsetChanged();
                },
                onBeforeTabSelect: function() {
                    return Csw.clientChanges.manuallyCheckChanges();
                },
                Refresh: function(options) {
                    Csw.clientChanges.unsetChanged();
                    Csw.main.is.multi = false; // semi-kludge for multi-edit batch op
                    Csw.main.refreshSelected(options);
                },
                onTabSelect: function(tabid) {
                    Csw.cookie.set(Csw.cookie.cookieNames.CurrentTabId, tabid);
                },
                onPropertyChange: function() {
                    Csw.clientChanges.setChanged();
                },
                onEditView: function(viewid) {
                    Csw.main.handleAction({
                        actionname: 'Edit_View',
                        ActionOptions: {
                            viewid: viewid,
                            viewmode: Csw.enums.viewMode.grid.name,
                            startingStep: 2,
                            IgnoreReturn: true
                        }
                    });
                },
                nodeTreeCheck: Csw.main.mainTree
            });
        } else {
            Csw.main.tabsAndProps.resetTabs(o.nodeid, o.nodekey);
        }
    });

    Csw.main.register('refreshSelected', function(options) {
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

    Csw.main.register('refreshNodesTree', function(options) {
        var o = {
            'nodeid': '',
            'nodekey': '',
            'nodename': '',
            'showempty': false,
            'forsearch': false,
            'iconurl': '',
            'viewid': '',
            'viewmode': 'tree',
            'IncludeNodeRequired': false
        };
        Csw.extend(o, options);

        if (Csw.isNullOrEmpty(o.nodeid)) {
            o.nodeid = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeId);
            if (Csw.isNullOrEmpty(o.nodekey)) {
                o.nodekey = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeKey);
            }
        }
        if (Csw.isNullOrEmpty(o.viewid)) {
            o.viewid = Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId);
        }

        Csw.main.clear({ left: true });

        Csw.nbt.viewFilters({
            name: 'main_viewfilters',
            parent: Csw.main.leftDiv,
            viewid: o.viewid,
            onEditFilters: function(newviewid) {
                var newopts = o;
                newopts.viewid = newviewid;
                // set the current view to be the session view, so filters are saved
                Csw.clientState.setCurrentView(newviewid, o.viewmode);
                Csw.main.refreshNodesTree(newopts);
            } // onEditFilters
        }); // viewFilters

        Csw.main.mainTree = Csw.nbt.nodeTreeExt(Csw.main.leftDiv, {
            forSearch: o.forsearch,
            onBeforeSelectNode: Csw.clientChanges.manuallyCheckChanges,
            onSelectNode: function(optSelect) {
                Csw.main.onSelectTreeNode({
                    tree: Csw.main.mainTree,
                    viewid: optSelect.viewid,
                    nodeid: optSelect.nodeid,
                    nodekey: optSelect.nodekey
                });
            },
            isMulti: Csw.main.is.multi,
            state: {
                viewId: o.viewid,
                viewMode: o.viewmode,
                nodeId: o.nodeid,
                nodeKey: o.nodekey,
                includeInQuickLaunch: true,
                includeNodeRequired: o.IncludeNodeRequired,
                onViewChange: function(newviewid, newviewmode) {
                    Csw.clientState.setCurrentView(newviewid, newviewmode);
                }
            }
        });
    }); // refreshNodesTree()

        Csw.main.register('handleAction', function (options) {
            var o = {
                actionname: '',
                actionurl: '',
                ActionOptions: {}
            };
            Csw.extend(o, options);

            Csw.main.initGlobalEventTeardown();

            var designOpt = {};

            Csw.clientState.setCurrentAction(o.actionname, o.actionurl);

            Csw.ajax.post({
                urlMethod: 'SaveActionToQuickLaunch',
                'data': { 'ActionName': o.actionname }
            });

            Csw.main.clear({ 'all': true });
            Csw.main.refreshMainMenu();
            Csw.main.universalsearch.enable();

            var actionName = Csw.string(o.actionname).replace(/_/g, ' ').trim().toLowerCase();
            switch (actionName) {
                case 'create inspection':
                    designOpt = {
                        name: 'cswInspectionDesignWizard',
                        viewid: o.ActionOptions.viewid,
                        viewmode: o.ActionOptions.viewmode,
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        },
                        onFinish: function (viewid) {
                            Csw.main.clear({ 'all': true });
                            Csw.main.handleItemSelect({
                                type: 'view',
                                mode: 'tree',
                                itemid: viewid
                            });

                        },
                        startingStep: o.ActionOptions.startingStep,
                        menuRefresh: Csw.main.refreshSelected
                    };
                    Csw.nbt.createInspectionWizard(Csw.main.centerTopDiv, designOpt);

                    break;
                case 'create material':
                    var createOpt = {
                        state: o.state,
                        request: o.requestitem,
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        },
                        onFinish: function (actionData) {
                            var createMaterialLandingPage = function () {
                                Csw.main.setLandingPage(function () {
                                    Csw.layouts.landingpage(Csw.main.centerBottomDiv, {
                                        name: 'createMaterialLandingPage',
                                        Title: 'Created:',
                                        ActionId: actionData.ActionId,
                                        ObjectClassId: actionData.RelatedObjectClassId,
                                        onLinkClick: Csw.main.handleItemSelect,
                                        onAddClick: function (itemData) {
                                            $.CswDialog('AddNodeDialog', {
                                                text: itemData.Text,
                                                nodetypeid: itemData.NodeTypeId,
                                                relatednodeid: actionData.RelatedNodeId,
                                                relatednodename: actionData.RelatedNodeName,
                                                relatednodetypeid: actionData.RelatedNodeTypeId,
                                                relatedobjectclassid: actionData.RelatedObjectClassId,
                                                onAddNode: function (nodeid, nodekey) {
                                                    Csw.main.clear({ all: true });
                                                    Csw.main.refreshNodesTree({ 'nodeid': nodeid, 'nodekey': nodekey, 'IncludeNodeRequired': true });
                                                }
                                            });
                                        },
                                        onTabClick: function (itemData) {
                                            Csw.cookie.set(Csw.cookie.cookieNames.CurrentTabId, itemData.TabId);
                                            Csw.main.handleItemSelect(itemData);
                                        },
                                        onButtonClick: function (itemData) {
                                            Csw.composites.nodeButton(Csw.main.centerBottomDiv, {
                                                name: itemData.Text,
                                                value: itemData.ActionName,
                                                mode: 'landingpage',
                                                propId: itemData.NodeTypePropId
                                            });
                                        },
                                        onAddComponent: createMaterialLandingPage,
                                        landingPageRequestData: actionData,
                                        onActionLinkClick: function (viewId) {
                                            Csw.main.handleItemSelect({
                                                type: 'view',
                                                mode: 'tree',
                                                itemid: viewId
                                            });
                                        },
                                        isConfigurable: actionData.isConfigurable
                                    });
                                });
                            };
                            createMaterialLandingPage();
                        },
                        startingStep: o.ActionOptions.startingStep
                    };
                    Csw.nbt.createMaterialWizard(Csw.main.centerTopDiv, createOpt);
                    break;
                case 'dispensecontainer':
                    var requestItemId = '', requestMode = '';
                    if (o.requestitem) {
                        requestItemId = o.requestitem.requestitemid;
                        requestMode = o.requestitem.requestMode;
                    }
                    var title = o.title;
                    if (false === title) {
                        title = 'Dispense from ';
                        if (false === Csw.isNullOrEmpty(o.barcode)) {
                            title += 'Barcode [' + o.barcode + ']';
                        } else {
                            title += 'Selected Container';
                        }
                    }
                    designOpt = {
                        title: title,
                        state: {
                            sourceContainerNodeId: o.sourceContainerNodeId,
                            currentQuantity: o.currentQuantity,
                            currentUnitName: o.currentUnitName,
                            precision: o.precision,
                            initialQuantity: o.initialQuantity,
                            requestItemId: requestItemId,
                            requestMode: requestMode,
                            title: title,
                            location: o.location,
                            material: o.material,
                            barcode: o.barcode,
                            containerNodeTypeId: o.containernodetypeid,
                            containerObjectClassId: o.containerobjectclassid,
                            customBarcodes: o.customBarcodes,
                            netQuantityEnforced: o.netQuantityEnforced
                        },
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        },
                        onFinish: function (viewid) {
                            Csw.main.clear({ 'all': true });
                            Csw.main.handleItemSelect({
                                type: 'view',
                                mode: 'tree',
                                itemid: viewid
                            });
                        }
                    };
                    Csw.nbt.dispenseContainerWizard(Csw.main.centerTopDiv, designOpt);
                    break;
                case 'movecontainer':
                    designOpt = {
                        title: o.title,
                        requestitemid: (o.requestitem) ? o.requestitem.requestitemid : '',
                        location: o.location,
                        onCancel: function () {
                            Csw.main.clear({ all: true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        },
                        onSubmit: function (viewid) {
                            Csw.main.clear({ all: true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        }
                    };
                    Csw.actions.containerMove(Csw.main.centerTopDiv, designOpt);
                    break;
                case 'edit view':
                    
                    Csw.nbt.vieweditor(Csw.main.centerTopDiv, {
                        onFinish: function (viewid, viewmode) {
                            Csw.main.clear({ 'all': true });
                            Csw.main.refreshViewSelect();
                            if (Csw.bool(o.ActionOptions.IgnoreReturn)) {
                                Csw.clientState.setCurrent(Csw.clientState.getLast());
                                Csw.main.refreshSelected();
                            } else {
                                Csw.main.handleItemSelect({ itemid: viewid, mode: viewmode });
                            }
                        },
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                            Csw.main.refreshViewSelect();
                        },
                        onDeleteView: function (deletedviewid) {
                            var current = Csw.clientState.getCurrent();
                            if (current.viewid == deletedviewid) {
                                Csw.clientState.clearCurrent();
                            }
                            var last = Csw.clientState.getLast();
                            if (last.viewid == deletedviewid) {
                                Csw.clientState.clearLast();
                            }
                            Csw.main.refreshViewSelect();
                        },
                        selectedViewId: o.ActionOptions.viewid,
                        startingStep: o.ActionOptions.startingStep
                    });
                    break;
                case 'future scheduling':
                    Csw.nbt.futureSchedulingWizard(Csw.main.centerTopDiv, {
                        onCancel: Csw.main.refreshSelected,
                        onFinish: function (viewid, viewmode) {
                            Csw.main.handleItemSelect({ itemid: viewid, mode: viewmode });
                        }
                    });
                    break;
                case 'hmis reporting':
                    Csw.actions.hmisReporting(Csw.main.centerTopDiv, {
                        onSubmit: function () {
                            Csw.main.refreshWelcomeLandingPage();
                        },
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        }
                    });
                    break;
                case 'login data':
                    Csw.actions.logindata(Csw.main.centerTopDiv, {
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        }
                    });
                    break;
                case 'quotas':
                    Csw.actions.quotas(Csw.main.centerTopDiv, {
                        onQuotaChange: function () {
                            Csw.actions.quotaImage(Csw.main.headerQuota);
                        }
                    });

                    break;
                case 'manage locations':
                    Csw.actions.managelocations(Csw.main.centerTopDiv, {
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        },
                        actionjson: o.ActionOptions
                    });
                    break;
                case 'delete demo data':
                    Csw.actions.deletedemodata(Csw.main.centerTopDiv, {
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        },
                        actionjson: o.ActionOptions
                    });
                    break;
                case 'modules':
                    Csw.actions.modules(Csw.main.centerTopDiv, {
                        onModuleChange: function () {
                            Csw.main.initAll();
                        }
                    });
                    break;
                case 'receiving':
                    o.onFinish = function (viewid) {
                        Csw.main.clear({ 'all': true });
                        Csw.main.handleItemSelect({
                            type: 'view',
                            mode: 'tree',
                            itemid: viewid
                        });
                    };
                    o.onCancel = function () {
                        Csw.main.clear({ 'all': true });
                        Csw.clientState.setCurrent(Csw.clientState.getLast());
                        Csw.main.refreshSelected();
                    };
                    Csw.nbt.receiveMaterialWizard(Csw.main.centerTopDiv, o);
                    break;
                case 'reconciliation':
                    var reconciliationOptions = {
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        },
                        onFinish: function () {
                            Csw.main.refreshWelcomeLandingPage();
                        },
                        startingStep: o.ActionOptions.startingStep
                    };
                    Csw.nbt.ReconciliationWizard(Csw.main.centerTopDiv, reconciliationOptions);
                    break;
                case 'sessions':
                    Csw.actions.sessions(Csw.main.centerTopDiv);
                    break;
                case 'submit request':
                    Csw.actions.requestCarts(Csw.main.centerTopDiv, {
                        onSubmit: function () {
                            //Nada
                        },
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        }
                    });
                    break;
                case 'subscriptions':
                    Csw.actions.subscriptions(Csw.main.centerTopDiv);
                    break;
                case 'tier ii reporting':
                    Csw.actions.tierIIReporting(Csw.main.centerTopDiv, {
                        onSubmit: function () {
                            Csw.main.refreshWelcomeLandingPage();
                        },
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        }
                    });
                    break;
                case 'view scheduled rules':
                    var rulesOpt = {
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        },
                        menuRefresh: Csw.main.refreshSelected
                    };

                    Csw.actions.scheduledRules(Csw.main.centerTopDiv, rulesOpt);
                    break;
                case 'upload legacy mobile data':
                    Csw.nbt.legacyMobileWizard(Csw.main.centerTopDiv, {
                        onCancel: Csw.main.refreshSelected,
                        onFinish: function (viewid, viewmode) {
                            Csw.main.handleItemSelect({ itemid: viewid, mode: viewmode });
                        }
                    });
                    break;
                case 'kiosk mode':
                    Csw.actions.kioskmode(Csw.main.centerTopDiv, {
                        onInit: function() {
                            Csw.main.universalsearch.disable();
                        },
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        }
                    });
                    break;
                default:
                    if (false == Csw.isNullOrEmpty(o.actionurl)) {
                        Csw.window.location(o.actionurl);
                    } else {
                        Csw.main.refreshWelcomeLandingPage();
                    }
                    break;
            }
        });

    Csw.subscribe(Csw.enums.events.main.handleAction, function (eventObj, opts) {
        Csw.main.handleAction(opts);
    }); // _handleAction()


    (function _postCtor() {
        //Case 28307: don't exec anything until all function expressions have been read
        Csw.main.initAll();
    }());


};


