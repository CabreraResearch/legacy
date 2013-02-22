/// <reference path="CswApp-vsdoc.js" />

window.initMain = window.initMain || function (undefined) {

    var cswPrivate = {
        tabsAndProps: null,
        is: (function() {
            var isMulti = false;
            var isExtReady = false;
            var isDocumentReady = false;
            var isOneTimeReset = false;
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
                }
            };
        }())
    };

    $(document).ready(function() {
        cswPrivate.is.documentReady = true;
        Csw.publish(Csw.enums.events.domready);
    });
    window.Ext.onReady(function() {
        cswPrivate.is.extReady = true;
        Csw.publish(Csw.enums.events.domready);
    });

    Csw.subscribe(Csw.enums.events.domready, function _onReady() {

        if (cswPrivate.is.documentReady && cswPrivate.is.extReady) {

            Csw.unsubscribe(Csw.enums.events.domready, null, _onReady); //We only need to execute once
            
            var mainTree;
            var mainGridId = 'CswNodeGrid';
            var mainTableId = 'CswNodeTable';
            var universalsearch;
            var mainviewselect;

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
                    Csw.unsubscribe('CswMultiEdit'); //omitting a function handle removes all
                    Csw.unsubscribe('CswNodeDelete'); //omitting a function handle removes all
                    Csw.publish('initPropertyTearDown'); //omitting a function handle removes all
                    cswPrivate.is.multi = false;
                    cswPrivate.is.oneTimeReset = true;
                    Csw.clientChanges.unsetChanged();
                });

            var startSpinner = function() {
                Csw.main.ajaxImage.show();
                Csw.main.ajaxSpacer.hide();
            };
            Csw.subscribe(Csw.enums.events.ajax.globalAjaxStart, startSpinner);

            var stopSpinner = function() {
                Csw.main.ajaxImage.hide();
                Csw.main.ajaxSpacer.show();
            };
            Csw.subscribe(Csw.enums.events.ajax.globalAjaxStop, stopSpinner);

            function refreshMain(eventObj, data) {
                Csw.clientChanges.unsetChanged();
                cswPrivate.is.multi = false;
                clear({ all: true });
                Csw.tryExec(refreshSelected, data);
            }

            Csw.subscribe(Csw.enums.events.main.refresh, refreshMain);

            function loadImpersonation(eventObj, actionData) {
                if (false === Csw.isNullOrEmpty(actionData.userid)) {
                    handleImpersonation(actionData.userid, actionData.username, function () {
                        initAll(function () {
                            handleItemSelect({
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
                    handleItemSelect({
                        itemid: Csw.string(actionData.actionid, actionData.viewid),
                        nodeid: actionData.selectedNodeId,
                        mode: actionData.viewmode,
                        name: actionData.actionname,
                        url: actionData.actionurl,
                        type: actionData.type
                    });
                }
            }

            Csw.subscribe(Csw.enums.events.RestoreViewContext, loadImpersonation);

            // watermark
            if (-1 === window.internetExplorerVersionNo) {
                Csw.ajax.post({
                    urlMethod: 'getWatermark',
                    success: function (result) {
                        if (false === Csw.isNullOrEmpty(result.watermark)) {
                            Csw.main.watermark.text(result.watermark);
                        }
                    }
                });
            }

            function handleImpersonation(userid, username, onSuccess) {
                var u = Csw.cookie.get(Csw.cookie.cookieNames.Username);
                Csw.ajax.post({
                    urlMethod: 'impersonate',
                    data: { UserId: userid },
                    success: function (data) {
                        if (Csw.bool(data.result)) {
                            Csw.cookie.set(Csw.cookie.cookieNames.OriginalUsername, u);
                            Csw.cookie.set(Csw.cookie.cookieNames.Username, u + ' as ' + username);
                            Csw.tryExec(onSuccess);
                        }
                    } // success
                }); // ajax
            }

            function refreshHeaderMenu() {
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
                    onImpersonate: function(userid, username) {
                        handleImpersonation(userid, username, function() {
                            Csw.goHome();
                        });
                    },
                    onEndImpersonation: function() {
                        Csw.ajax.post({
                            urlMethod: 'endImpersonation',
                            success: function(data) {
                                if (Csw.bool(data.result)) {
                                    Csw.cookie.set(Csw.cookie.cookieNames.Username, Csw.cookie.get(Csw.cookie.cookieNames.OriginalUsername));
                                    Csw.cookie.clear(Csw.cookie.cookieNames.OriginalUsername);
                                    Csw.goHome();
                                }
                            } // success
                        }); // ajax
                    } // onEndImpersonation
                }); // CswMenuHeader
            }

            Csw.subscribe(Csw.enums.events.main.refreshHeader, function (eventObj, opts) {
                refreshHeaderMenu(opts);
            });

            function handleQueryString() {
                var ret = false;
                var qs = Csw.queryString();

                if (Csw.clientSession.isDebug(qs)) {
                    Csw.clientSession.enableDebug();
                    Csw.cookie.set(Csw.cookie.cookieNames.LogoutPath, 'Dev.html');
                    Csw.setGlobalProp('homeUrl', 'Dev.html');
                }

                if (false == Csw.isNullOrEmpty(qs.action)) {
                    var actopts = {};
                    Csw.extend(actopts, qs);
                    Csw.main.handleAction({ actionname: qs.action, ActionOptions: actopts });

                } else if (false == Csw.isNullOrEmpty(qs.viewid)) {
                    var setView = function (viewid, viewmode) {
                        handleItemSelect({
                            type: 'view',
                            itemid: viewid,
                            mode: viewmode
                        });
                    };
                    if (Csw.isNullOrEmpty(qs.viewmode)) {
                        Csw.ajax.post({
                            url: Csw.enums.ajaxUrlPrefix + 'getViewMode',
                            data: { ViewId: qs.viewid },
                            success: function (data) {
                                setView(qs.viewid, Csw.string(data.viewmode, 'tree'));
                            }
                        });
                    } else {
                        setView(qs.viewid, Csw.string(qs.viewmode));
                    }

                } else if (false == Csw.isNullOrEmpty(qs.nodeid)) {
                    handleItemSelect({
                        type: 'view',
                        mode: 'tree',
                        linktype: 'link',
                        nodeid: qs.nodeid
                    });

                } else if (false == Csw.isNullOrEmpty(qs.reportid)) {
                    handleReport(qs.reportid);
                    ret = true; // load the current context (probably the welcome landing page) below the report

                } else if (false == Csw.isNullOrEmpty(qs.clear)) {
                    Csw.clientState.clearCurrent();
                    ret = true;

                } else {
                    ret = true;
                }

                return ret;
            }

            function setUsername(username) {
                Csw.clientSession.setUsername(username);
                Csw.main.headerUsername.text(username)
                    .$.hover(function () { $(this).CswAttrDom('title', Csw.clientSession.getExpireTime()); });
            }

            Csw.subscribe(Csw.enums.events.main.reauthenticate, function (eventObj, username) {
                setUsername(username);
            });

            function initAll(onSuccess) {
                Csw.main.centerBottomDiv.$.CswLogin('init', {
                    'onAuthenticate': function (u) {
                        setUsername(u);
                        refreshDashboard();
                        refreshHeaderMenu();
                        universalsearch = Csw.composites.universalSearch(null, {
                            searchBoxParent: Csw.main.searchDiv,
                            searchResultsParent: Csw.main.rightDiv,
                            searchFiltersParent: Csw.main.leftDiv,
                            onBeforeSearch: function () {
                                clear({ all: true });
                            },
                            onAfterSearch: function (search) {
                                refreshMainMenu({ nodetypeid: search.getFilterToNodeTypeId() });
                            },
                            onAfterNewSearch: function (searchid) {
                                Csw.clientState.setCurrentSearch(searchid);
                            },
                            onAddView: function (viewid, viewmode) {
                                refreshViewSelect();
                            },
                            onLoadView: function (viewid, viewmode) {
                                handleItemSelect({
                                    type: 'view',
                                    itemid: viewid,
                                    mode: viewmode
                                });
                            }
                        });

                        Csw.actions.quotaImage(Csw.main.headerQuota);

                        // handle querystring arguments
                        var loadCurrent = handleQueryString();

                        if (Csw.isNullOrEmpty(onSuccess) && loadCurrent) {
                            onSuccess = function () {
                                var current = Csw.clientState.getCurrent();
                                if (false === Csw.isNullOrEmpty(current.viewid)) {
                                    handleItemSelect({
                                        type: 'view',
                                        itemid: current.viewid,
                                        mode: current.viewmode
                                    });
                                } else if (false === Csw.isNullOrEmpty(current.actionname)) {
                                    handleItemSelect({
                                        type: 'action',
                                        name: current.actionname,
                                        url: current.actionurl
                                    });
                                } else if (false === Csw.isNullOrEmpty(current.reportid)) {
                                    handleItemSelect({
                                        type: 'report',
                                        itemid: current.reportid
                                    });
                                } else if (false === Csw.isNullOrEmpty(current.searchid)) {
                                    handleItemSelect({
                                        type: 'search',
                                        itemid: current.searchid
                                    });
                                } else {
                                    refreshWelcomeLandingPage();
                                }
                            };
                        }
                        Csw.tryExec(onSuccess);

                    } // onAuthenticate
                }); // CswLogin

            }

            function refreshDashboard() {
                Csw.main.headerDashboard.empty().$.CswDashboard();
            }

            // initAll()

            function refreshViewSelect(onSuccess) {
                Csw.main.viewSelectDiv.empty();
                mainviewselect = Csw.main.viewSelectDiv.viewSelect({
                    name: 'mainviewselect',
                    onSelect: handleItemSelect,
                    onSuccess: onSuccess
                });
            }

            function clear(options) {
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
                    Csw.setGlobalProp('uniqueIdCount', 0);
                    Csw.main.mainMenuDiv.empty();
                }
            }

            Csw.subscribe(Csw.enums.events.main.clear, function (eventObj, opts) {
                clear(opts);
            });

            function refreshWelcomeLandingPage() {
                setLandingPage(function () {
                    Csw.layouts.landingpage(Csw.main.centerBottomDiv, {
                        name: 'welcomeLandingPage',
                        Title: '',
                        onLinkClick: handleItemSelect,
                        onAddClick: function (itemData) {
                            $.CswDialog('AddNodeDialog', {
                                text: itemData.Text,
                                nodetypeid: itemData.NodeTypeId,
                                onAddNode: function (nodeid, nodekey) {
                                    clear({ all: true });
                                    refreshNodesTree({ 'nodeid': nodeid, 'nodekey': nodekey, 'IncludeNodeRequired': true });
                                }
                            });
                        },
                        onTabClick: function (itemData) {
                            Csw.cookie.set(Csw.cookie.cookieNames.CurrentTabId, itemData.TabId);
                            handleItemSelect(itemData);
                        },
                        onAddComponent: refreshWelcomeLandingPage,
                        landingPageRequestData: {
                            RoleId: ''
                        }
                    });
                });
            }

            function setLandingPage(loadLandingPage) {
                clear({ all: true });
                loadLandingPage();
                refreshMainMenu();
                refreshViewSelect();
            }

            var refreshLandingPage = function (eventObj, opts) {
                clear({ all: true });
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
                    onLinkClick: handleItemSelect,
                    onAddClick: function (itemData) {
                        $.CswDialog('AddNodeDialog', {
                            text: itemData.Text,
                            nodetypeid: itemData.NodeTypeId,
                            relatednodeid: layData.RelatedNodeId,
                            relatednodename: layData.RelatedNodeName,
                            relatednodetypeid: layData.RelatedNodeTypeId,
                            relatedobjectclassid: layData.RelatedObjectClassId,
                            onAddNode: function (nodeid, nodekey) {
                                clear({ all: true });
                                refreshNodesTree({ nodeid: nodeid, nodekey: nodekey, IncludeNodeRequired: true });
                            }
                        });
                    },
                    onTabClick: function (itemData) {
                        Csw.cookie.set(Csw.cookie.cookieNames.CurrentTabId, itemData.TabId);
                        handleItemSelect(itemData);
                    },
                    onButtonClick: function (itemData) {
                        Csw.controls.nodeButton(Csw.main.centerBottomDiv, {
                            name: itemData.Text,
                            value: itemData.ActionName,
                            mode: 'landingpage',
                            propId: itemData.NodeTypePropId
                        });
                    },
                    onAddComponent: function () { Csw.publish('refreshLandingPage'); },
                    landingPageRequestData: layData,
                    onActionLinkClick: function (viewId) {
                        handleItemSelect({
                            type: 'view',
                            mode: 'tree',
                            itemid: viewId
                        });
                    },
                    isConfigurable: layData.isConfigurable
                });
                refreshMainMenu();
                refreshViewSelect();
            };
            Csw.subscribe('refreshLandingPage', refreshLandingPage);

            function handleItemSelect(options) {
                //if (debugOn()) Csw.debug.log('Main.handleItemSelect()');
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

                cswPrivate.is.multi = false; /* Case 26134. Revert multi-edit selection when switching views, etc. */
                var linkType = Csw.string(o.linktype).toLowerCase();

                var type = Csw.string(o.type).toLowerCase();

                //Now is a good time to purge outstanding Node-specific events
                
                
                if (Csw.clientChanges.manuallyCheckChanges()) { // && itemIsSupported()) {
                    Csw.main.initGlobalEventTeardown();
                    if (false === Csw.isNullOrEmpty(type)) {
                        switch (type) {
                        case 'action':
                            clear({ all: true });
                            Csw.main.handleAction({
                                'actionname': o.name,
                                'actionurl': o.url
                            });
                            break;
                        case 'search':
                            clear({ all: true });
                            universalsearch.restoreSearch(o.itemid);
                            break;
                        case 'report':
                            handleReport(o.itemid);
                            break;
                        case 'view':
                            clear({ all: true });
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
                                    getViewGrid({ 'viewid': o.itemid, 'nodeid': o.nodeid, 'nodekey': o.nodekey, 'showempty': linkOpt.showempty, 'forsearch': linkOpt.forsearch });
                                    break;
                                case 'table':
                                    getViewTable({ 'viewid': o.itemid }); //, 'nodeid': o.nodeid, 'nodekey': o.nodekey });
                                    break;
                                default:
                                    refreshNodesTree({ 'viewid': o.itemid, 'viewmode': o.mode, 'nodeid': o.nodeid, 'nodekey': '', 'showempty': linkOpt.showempty, 'forsearch': linkOpt.forsearch });
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

                        refreshViewSelect();
                    }
                } //if (Csw.clientChanges.manuallyCheckChanges() && itemIsSupported()) {

            } //handleItemSelect

            function handleReport(reportid) {
                Csw.openPopup("Report.html?reportid=" + reportid);
            }

            function refreshMainMenu(options) {
                var o = {
                    parent: Csw.main.mainMenuDiv,
                    viewid: '',
                    viewmode: '',
                    nodeid: '',
                    nodekey: '',
                    nodetypeid: '',
                    propid: '',
                    grid: '',
                    limitMenuTo: '',
                    readonly: false
                };
                Csw.extend(o, options);

                Csw.main.mainMenuDiv.empty();

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
                            ReadOnly: o.readonly
                        }
                    },
                    onAlterNode: function(nodeid, nodekey) {
                        var state = Csw.clientState.getCurrent();
                        refreshSelected({ 'nodeid': nodeid, 'nodekey': nodekey, 'IncludeNodeRequired': true, 'searchid': state.searchid });
                    },
                    onMultiEdit: function() {
                        switch (o.viewmode) {
                        case Csw.enums.viewMode.grid.name:
                            o.grid.toggleShowCheckboxes();
                            break;
                        default:
                            Csw.publish('CswMultiEdit', {
                                multi: cswPrivate.is.toggleMulti(),
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
                    onSaveView: function (newviewid) {
                        handleItemSelect({ 'viewid': newviewid, 'viewmode': Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewMode) });
                    },
                    onPrintView: function () {
                        switch (o.viewmode) {
                            case Csw.enums.viewMode.grid.name:
                                if (false == Csw.isNullOrEmpty(o.grid)) {
                                    o.grid.print();
                                }
                                break;
                            default:
                                Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, 'View Printing is not enabled for views of type ' + o.viewmode));
                                break;
                        }
                    },
                    Multi: cswPrivate.is.multi,
                    nodeTreeCheck: mainTree
                };

                o.parent.menu(menuOpts);

            }

            function getViewGrid(options) {
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

                o.onEditNode = function () { grid.reload(); };
                o.onDeleteNode = function () { grid.reload(); };
                o.onRefresh = function (options) {
                    clear({ centertop: true, centerbottom: true });
                    Csw.clientChanges.unsetChanged();
                    cswPrivate.is.multi = false; // semi-kludge for multi-edit batch op
                    refreshSelected(options);
                };
                clear({ centertop: true, centerbottom: true });

                Csw.nbt.viewFilters({
                    name: 'main_viewfilters',
                    parent: Csw.main.centerTopDiv,
                    viewid: o.viewid,
                    onEditFilters: function (newviewid) {
                        var newopts = o;
                        newopts.viewid = newviewid;
                        // set the current view to be the session view, so filters are saved
                        Csw.clientState.setCurrentView(newviewid, Csw.enums.viewMode.grid.name);
                        getViewGrid(newopts);
                    } // onEditFilters
                }); // viewFilters
                var div = Csw.main.centerBottomDiv.div({ suffix: window.Ext.id() });
                div.empty();
                var grid = Csw.nbt.nodeGrid(div, {
                    viewid: o.viewid,
                    nodeid: o.nodeid,
                    nodekey: o.nodekey,
                    showempty: getEmptyGrid,
                    name: mainGridId,
                    //'onAddNode': o.onAddNode,
                    onEditNode: o.onEditNode,
                    onDeleteNode: o.onDeleteNode,
                    onRefresh: o.onRefresh,
                    onSuccess: function (grid) {
                        if (o.doMenuRefresh) {
                            refreshMainMenu({
                                viewid: o.viewid,
                                viewmode: Csw.enums.viewMode.grid.name,
                                grid: grid//,
                                //nodeid: o.nodeid,  // case 26914
                                //nodekey: o.nodekey
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
            }


            function getViewTable(options) {
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

                o.onEditNode = function () { getViewTable(o); };
                o.onDeleteNode = function () { getViewTable(o); };

                clear({ centertop: true, centerbottom: true });

                Csw.nbt.viewFilters({
                    name: 'main_viewfilters',
                    parent: Csw.main.centerTopDiv,
                    viewid: o.viewid,
                    onEditFilters: function (newviewid) {
                        var newopts = o;
                        newopts.viewid = newviewid;
                        // set the current view to be the session view, so filters are saved
                        Csw.clientState.setCurrentView(newviewid, Csw.enums.viewMode.table.name);
                        getViewTable(newopts);
                    } // onEditFilters
                }); // viewFilters

                Csw.nbt.nodeTable(Csw.main.centerBottomDiv, {
                    viewid: o.viewid,
//            nodeid: o.nodeid,
//            nodekey: o.nodekey,
                    name: mainTableId,
                    Multi: cswPrivate.is.multi,
                    //'onAddNode': o.onAddNode,
                    onEditNode: o.onEditNode,
                    onDeleteNode: o.onDeleteNode,
                    onSuccess: function() {
                        refreshMainMenu({
                            viewid: o.viewid,
                            viewmode: Csw.enums.viewMode.table.name//,
                            //                    nodeid: o.nodeid,
                            //                    nodekey: o.nodekey
                        });
                    },
                    onNoResults: showDefaultContentTable
                });
            }

            var onSelectTreeNode = function (options) {
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
                        getTabs({
                            viewid: o.viewid,
                            nodeid: o.nodeid,
                            nodekey: o.nodekey
                        });
                        refreshMainMenu({
                            parent: o.tree.menuDiv,
                            viewid: o.viewid,
                            viewmode: Csw.enums.viewMode.tree.name,
                            nodeid: o.nodeid,
                            nodekey: o.nodekey
                        });
                    } else {
                        showDefaultContentTree({ viewid: o.viewid, viewmode: Csw.enums.viewMode.tree.name });
                        refreshMainMenu({
                            parent: o.tree.menuDiv,
                            viewid: o.viewid,
                            viewmode: Csw.enums.viewMode.tree.name,
                            nodeid: '',
                            nodekey: ''
                        });
                    }
                }
            }; // onSelectTreeNode()

            function showDefaultContentTree(viewopts) {
                var v = {
                    viewid: '',
                    viewmode: '',
                    onAddNode: function (nodeid, nodekey) {
                        refreshSelected({ 'nodeid': nodeid, 'nodekey': nodekey, 'IncludeNodeRequired': true });
                    }
                };
                if (viewopts) Csw.extend(v, viewopts);
                clear({ right: true });
                Csw.main.rightDiv.$.CswDefaultContent(v);

            } // showDefaultContentTree()

            function showDefaultContentTable(viewopts) {
                var v = {
                    viewid: '',
                    viewmode: '',
                    onAddNode: function (nodeid, nodekey) {
                        refreshSelected({ 'nodeid': nodeid, 'nodekey': nodekey, 'IncludeNodeRequired': true });
                    }
                };
                if (viewopts) Csw.extend(v, viewopts);
                clear({ centerbottom: true });
                var div = Csw.main.centerBottomDiv.div({
                    name: 'deftbldiv',
                    align: 'center'
                });
                div.css({ textAlign: 'center' });
                div.append('No Results.');

                div.$.CswDefaultContent(v);

            } // showDefaultContentTable()

            function getTabs(options) {
                Csw.publish('initPropertyTearDown');
                var o = {
                    nodeid: '',
                    nodekey: '',
                    viewid: ''
                };
                Csw.extend(o, options);

                clear({ right: true });

                if (cswPrivate.is.oneTimeReset ||
                    Csw.isNullOrEmpty(cswPrivate.tabsAndProps) ||
                    o.viewid !== cswPrivate.tabsAndProps.getViewId()) {
                    cswPrivate.tabsAndProps = Csw.layouts.tabsAndProps(Csw.main.rightDiv, {
                        name: 'nodetabs',
                        globalState: {
                            viewid: o.viewid,
                            currentNodeId: o.nodeid,
                            currentNodeKey: o.nodekey
                        },
                        tabState: {
                            ShowCheckboxes: cswPrivate.is.multi,
                            tabid: Csw.cookie.get(Csw.cookie.cookieNames.CurrentTabId)
                        },
                        onSave: function() {
                            Csw.clientChanges.unsetChanged();
                        },
                        onBeforeTabSelect: function() {
                            return Csw.clientChanges.manuallyCheckChanges();
                        },
                        Refresh: function(options) {
                            Csw.clientChanges.unsetChanged();
                            cswPrivate.is.multi = false; // semi-kludge for multi-edit batch op
                            refreshSelected(options);
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
                        nodeTreeCheck: mainTree
                    });
                } else {
                    cswPrivate.tabsAndProps.resetTabs(o.nodeid, o.nodekey);
                }
            }

            function refreshSelected(options) {
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
                        universalsearch.restoreSearch(o.searchid);
                    } else {
                        var viewMode = Csw.string(o.viewmode).toLowerCase();
                        switch (viewMode) {
                            case 'grid':
                                getViewGrid({
                                    viewid: o.viewid,
                                    nodeid: o.nodeid,
                                    nodekey: o.nodekey,
                                    showempty: o.showempty,
                                    forsearch: o.forsearch
                                });
                                break;
                            case 'list':
                                refreshNodesTree({
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
                                getViewTable({
                                    viewid: o.viewid //,
                                    //                            nodeid: o.nodeid,
                                    //                            nodekey: o.nodekey
                                });
                                break;
                            case 'tree':
                                refreshNodesTree({
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
                                refreshWelcomeLandingPage();
                                break;
                        } // switch
                    } // if (false === Csw.isNullOrEmpty(o.searchid))
                } // if (manuallyCheckChanges())
            } // refreshSelected()
            Csw.subscribe(Csw.enums.events.main.refreshSelected,
                function (eventObj, opts) {
                    refreshSelected(opts);
                });
            
            function refreshNodesTree(options) {
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

                clear({ left: true });

                Csw.nbt.viewFilters({
                    name: 'main_viewfilters',
                    parent: Csw.main.leftDiv,
                    viewid: o.viewid,
                    onEditFilters: function (newviewid) {
                        var newopts = o;
                        newopts.viewid = newviewid;
                        // set the current view to be the session view, so filters are saved
                        Csw.clientState.setCurrentView(newviewid, o.viewmode);
                        refreshNodesTree(newopts);
                    } // onEditFilters
                }); // viewFilters
                
                mainTree = Csw.nbt.nodeTreeExt(Csw.main.leftDiv, {
                    forSearch: o.forsearch,
                    onBeforeSelectNode: Csw.clientChanges.manuallyCheckChanges,
                    onSelectNode: function (optSelect) {
                        onSelectTreeNode({
                            tree: mainTree,
                            viewid: optSelect.viewid,
                            nodeid: optSelect.nodeid,
                            nodekey: optSelect.nodekey
                        });
                    },
                    isMulti: cswPrivate.is.multi,
                    state: {
                        viewId: o.viewid,
                        viewMode: o.viewmode,
                        nodeId: o.nodeid,
                        nodeKey: o.nodekey,
                        includeInQuickLaunch: true,
                        includeNodeRequired: o.IncludeNodeRequired,
                        onViewChange: function (newviewid, newviewmode) {
                            Csw.clientState.setCurrentView(newviewid, newviewmode);
                        }
                    }
                });
            } // refreshNodesTree()


            Csw.main.handleAction = Csw.main.handleAction ||
                Csw.main.register('handleAction', function(options) {
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

                    clear({ 'all': true });
                    refreshMainMenu();

                    var actionName = Csw.string(o.actionname).replace('_', ' ').trim().toLowerCase();
                    switch (actionName) {
                    case 'create inspection':
                        designOpt = {
                            name: 'cswInspectionDesignWizard',
                            viewid: o.ActionOptions.viewid,
                            viewmode: o.ActionOptions.viewmode,
                            onCancel: function() {
                                clear({ 'all': true });
                                Csw.clientState.setCurrent(Csw.clientState.getLast());
                                refreshSelected();
                            },
                            onFinish: function(viewid) {
                                clear({ 'all': true });
                                handleItemSelect({
                                    type: 'view',
                                    mode: 'tree',
                                    itemid: viewid
                                });

                            },
                            startingStep: o.ActionOptions.startingStep,
                            menuRefresh: refreshSelected
                        };
                        Csw.nbt.createInspectionWizard(Csw.main.centerTopDiv, designOpt);

                        break;
                    case 'create material':
                        var createOpt = {
                            state: o.state,
                            request: o.requestitem,
                            onCancel: function() {
                                clear({ 'all': true });
                                Csw.clientState.setCurrent(Csw.clientState.getLast());
                                refreshSelected();
                            },
                            onFinish: function(actionData) {
                                var createMaterialLandingPage = function() {
                                    setLandingPage(function() {
                                        Csw.layouts.landingpage(Csw.main.centerBottomDiv, {
                                            name: 'createMaterialLandingPage',
                                            Title: 'Created:',
                                            ActionId: actionData.ActionId,
                                            ObjectClassId: actionData.RelatedObjectClassId,
                                            onLinkClick: handleItemSelect,
                                            onAddClick: function(itemData) {
                                                $.CswDialog('AddNodeDialog', {
                                                    text: itemData.Text,
                                                    nodetypeid: itemData.NodeTypeId,
                                                    relatednodeid: actionData.RelatedNodeId,
                                                    relatednodename: actionData.RelatedNodeName,
                                                    relatednodetypeid: actionData.RelatedNodeTypeId,
                                                    relatedobjectclassid: actionData.RelatedObjectClassId,
                                                    onAddNode: function(nodeid, nodekey) {
                                                        clear({ all: true });
                                                        refreshNodesTree({ 'nodeid': nodeid, 'nodekey': nodekey, 'IncludeNodeRequired': true });
                                                    }
                                                });
                                            },
                                            onTabClick: function(itemData) {
                                                Csw.cookie.set(Csw.cookie.cookieNames.CurrentTabId, itemData.TabId);
                                                handleItemSelect(itemData);
                                            },
                                            onButtonClick: function(itemData) {
                                                Csw.controls.nodeButton(Csw.main.centerBottomDiv, {
                                                    name: itemData.Text,
                                                    value: itemData.ActionName,
                                                    mode: 'landingpage',
                                                    propId: itemData.NodeTypePropId
                                                });
                                            },
                                            onAddComponent: createMaterialLandingPage,
                                            landingPageRequestData: actionData,
                                            onActionLinkClick: function(viewId) {
                                                handleItemSelect({
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
                                initialQuantity: Csw.deserialize(o.initialQuantity),
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
                            onCancel: function() {
                                clear({ 'all': true });
                                Csw.clientState.setCurrent(Csw.clientState.getLast());
                                refreshSelected();
                            },
                            onFinish: function(viewid) {
                                clear({ 'all': true });
                                handleItemSelect({
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
                            onCancel: function() {
                                clear({ all: true });
                                Csw.clientState.setCurrent(Csw.clientState.getLast());
                                refreshSelected();
                            },
                            onSubmit: function(viewid) {
                                clear({ all: true });
                                Csw.clientState.setCurrent(Csw.clientState.getLast());
                                refreshSelected();
                            }
                        };
                        Csw.actions.containerMove(Csw.main.centerTopDiv, designOpt);
                        break;
                    case 'edit view':
                        var editViewOptions = {
                            'viewid': o.ActionOptions.viewid,
                            'viewmode': o.ActionOptions.viewmode,
                            'onCancel': function() {
                                clear({ 'all': true });
                                Csw.clientState.setCurrent(Csw.clientState.getLast());
                                refreshSelected();
                            },
                            'onFinish': function(viewid, viewmode) {
                                clear({ 'all': true });
                                //refreshViewSelect();
                                if (Csw.bool(o.ActionOptions.IgnoreReturn)) {
                                    Csw.clientState.setCurrent(Csw.clientState.getLast());
                                    refreshSelected();
                                } else {
                                    handleItemSelect({ itemid: viewid, mode: viewmode });
                                }
                            },
                            onAddView: function(deletedviewid) {
                            },
                            onDeleteView: function(deletedviewid) {
                                var current = Csw.clientState.getCurrent();
                                if (current.viewid == deletedviewid) {
                                    Csw.clientState.clearCurrent();
                                }
                                var last = Csw.clientState.getLast();
                                if (last.viewid == deletedviewid) {
                                    Csw.clientState.clearLast();
                                }
                                refreshViewSelect();
                            },
                            'startingStep': o.ActionOptions.startingStep
                        };

                        Csw.main.centerTopDiv.$.CswViewEditor(editViewOptions);
                        break;
                    case 'future scheduling':
                        Csw.nbt.futureSchedulingWizard(Csw.main.centerTopDiv, {
                            onCancel: refreshSelected,
                            onFinish: function(viewid, viewmode) {
                                handleItemSelect({ itemid: viewid, mode: viewmode });
                            }
                        });
                        break;
                    case 'hmis reporting':
                        Csw.actions.hmisReporting(Csw.main.centerTopDiv, {
                            onSubmit: function () {
                                refreshWelcomeLandingPage();
                            },
                            onCancel: function () {
                                clear({ 'all': true });
                                Csw.clientState.setCurrent(Csw.clientState.getLast());
                                refreshSelected();
                            }
                        });
                    break;
                    //			case 'Import_Fire_Extinguisher_Data':                                                                                                 
                    //				break;                                                                                                 
                    //			case 'Inspection_Design':                                                                                                 
                    //				break;                                                                                                 
                    case 'quotas':
                        Csw.actions.quotas(Csw.main.centerTopDiv, {
                            onQuotaChange: function() {
                                Csw.actions.quotaImage(Csw.main.headerQuota);
                            }
                        });

                        break;
                    case 'modules':
                        Csw.actions.modules(Csw.main.centerTopDiv, {
                            onModuleChange: function() {
                                refreshHeaderMenu();
                                refreshDashboard();
                                refreshViewSelect();
                            }
                        });
                        break;
                    case 'receiving':
                        o.onFinish = function(viewid) {
                            clear({ 'all': true });
                            handleItemSelect({
                                type: 'view',
                                mode: 'tree',
                                itemid: viewid
                            });
                        };
                        o.onCancel = function() {
                            clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            refreshSelected();
                        };
                        Csw.nbt.receiveMaterialWizard(Csw.main.centerTopDiv, o);
                        break;
                    case 'reconciliation':
                        var reconciliationOptions = {
                            onCancel: function() {
                                clear({ 'all': true });
                                Csw.clientState.setCurrent(Csw.clientState.getLast());
                                refreshSelected();
                            },
                            onFinish: function() {
                                refreshWelcomeLandingPage();
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
                            onSubmit: function() {
                                //Nada
                            },
                            onCancel: function() {
                                clear({ 'all': true });
                                Csw.clientState.setCurrent(Csw.clientState.getLast());
                                refreshSelected();
                            }
                        });
                        break;
                    case 'subscriptions':
                        Csw.actions.subscriptions(Csw.main.centerTopDiv);
                        break;
                    case 'tier ii reporting':
                        Csw.actions.tierIIReporting(Csw.main.centerTopDiv, {
                            onSubmit: function () {
                                refreshWelcomeLandingPage();
                            },
                            onCancel: function () {
                                clear({ 'all': true });
                                Csw.clientState.setCurrent(Csw.clientState.getLast());
                                refreshSelected();
                            }
                        });
                        break;
                    case 'view scheduled_rules':
                    case 'view scheduled rules':
                        var rulesOpt = {
                            onCancel: function() {
                                clear({ 'all': true });
                                Csw.clientState.setCurrent(Csw.clientState.getLast());
                                refreshSelected();
                            },
                            menuRefresh: refreshSelected
                        };

                        Csw.actions.scheduledRules(Csw.main.centerTopDiv, rulesOpt);
                        break;
                    case 'upload legacy mobile data':
                        Csw.nbt.legacyMobileWizard(Csw.main.centerTopDiv, {
                            onCancel: refreshSelected,
                            onFinish: function (viewid, viewmode) {
                                handleItemSelect({ itemid: viewid, mode: viewmode });
                            }
                        });
                        break;
                    case 'kioskmode':
                        Csw.actions.kioskmode(Csw.main.centerTopDiv, {
                            onCancel: function() {
                                clear({ 'all': true });
                                Csw.clientState.setCurrent(Csw.clientState.getLast());
                                refreshSelected();
                            }
                        });
                        break;
                    default:
                        if (false == Csw.isNullOrEmpty(o.actionurl)) {
                            Csw.window.location(o.actionurl);
                        } else {
                            refreshWelcomeLandingPage();
                        }
                        break;
                    }
                });

            Csw.subscribe(Csw.enums.events.main.handleAction, function(eventObj, opts) {
                Csw.main.handleAction(opts);
            }); // _handleAction()


            (function _postCtor() {
                //Case 28307: don't exec anything until all function expressions have been read
                initAll();
            } ());

        }
    });
};


