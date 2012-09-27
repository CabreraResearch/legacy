/// <reference path="~/app/CswApp-vsdoc.js" />

/// <reference path="../globals/CswEnums.js" />

window.initMain = window.initMain || function (undefined) {

    'use strict';

    Csw.publish(Csw.enums.events.domready);
    //Csw.debug.group('Csw');
    var mainTree;
    var mainGridId = 'CswNodeGrid';
    var mainTableId = 'CswNodeTable';

    var universalsearch;

    var mainviewselect;

    (function _initMain() {
        Csw.main.body = Csw.main.body || Csw.main.register('body', Csw.literals.factory($('body')));     // case 27563 review K3663 comment 1
        Csw.main.ajaxImage = Csw.main.ajaxImage || Csw.main.register('ajaxImage', Csw.literals.factory($('#ajaxImage')));
        Csw.main.ajaxSpacer = Csw.main.ajaxSpacer || Csw.main.register('ajaxSpacer', Csw.literals.factory($('#ajaxSpacer')));
        Csw.main.centerBottomDiv = Csw.main.centerBottomDiv || Csw.main.register('centerBottomDiv', Csw.literals.factory($('#CenterBottomDiv')));
        Csw.main.centerTopDiv = Csw.main.centerTopDiv || Csw.main.register('centerTopDiv', Csw.literals.factory($('#CenterTopDiv')));
        Csw.main.headerDashboard = Csw.main.headerDashboard || Csw.main.register('headerDashboard', Csw.literals.factory($('#header_dashboard')));
        Csw.main.headerMenu = Csw.main.headerMenu || Csw.main.register('headerMenu', Csw.literals.factory($('#header_menu')));
        Csw.main.headerQuota = Csw.main.headerQuota || Csw.main.register('headerQuota', Csw.literals.factory($('#header_quota')));
        Csw.main.headerUsername = Csw.main.headerUsername || Csw.main.register('headerUsername', Csw.literals.factory($('#header_username')));
        Csw.main.leftDiv = Csw.main.leftDiv || Csw.main.register('leftDiv', Csw.literals.factory($('#LeftDiv')));
        Csw.main.mainMenuDiv = Csw.main.mainMenuDiv || Csw.main.register('mainMenuDiv', Csw.literals.factory($('#MainMenuDiv')));
        Csw.main.rightDiv = Csw.main.rightDiv || Csw.main.register('rightDiv', Csw.literals.factory($('#RightDiv')));
        Csw.main.searchDiv = Csw.main.searchDiv || Csw.main.register('searchDiv', Csw.literals.factory($('#SearchDiv')));
        Csw.main.viewSelectDiv = Csw.main.viewSelectDiv || Csw.main.register('viewSelectDiv', Csw.literals.factory($('#ViewSelectDiv')));
        Csw.main.watermark = Csw.main.watermark || Csw.main.register('watermark', Csw.literals.factory($('#watermark')));

    } ());


    var startSpinner = function() {
        Csw.main.ajaxImage.show();
        Csw.main.ajaxSpacer.hide();
    }
    Csw.subscribe(Csw.enums.events.ajax.globalAjaxStart, startSpinner);

    var stopSpinner = function() {
        Csw.main.ajaxImage.hide();
        Csw.main.ajaxSpacer.show();
    };
    Csw.subscribe(Csw.enums.events.ajax.globalAjaxStop, stopSpinner);

    function refreshMain(eventObj, data) {
        Csw.clientChanges.unsetChanged();
        multi = false;
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
            onLogout: function () {
                Csw.clientSession.logout();
            },
            onQuotas: function () {
                handleAction({ 'actionname': 'Quotas' });
            },
            onModules: function () {
                handleAction({ 'actionname': 'Modules' });
            },
            onSubmitRequest: function () {
                handleAction({ 'actionname': 'Submit_Request' });
            },
            onSessions: function () {
                handleAction({ 'actionname': 'Sessions' });
            },
            onSubscriptions: function () {
                handleAction({ 'actionname': 'Subscriptions' });
            },
            onImpersonate: function (userid, username) {
                handleImpersonation(userid, username, function () {
                    Csw.goHome();
                });
            },
            onEndImpersonation: function () {
                Csw.ajax.post({
                    urlMethod: 'endImpersonation',
                    success: function (data) {
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
        if (false == Csw.isNullOrEmpty(qs.action)) {
            var actopts = {};
            Csw.extend(actopts, qs);
            handleAction({ actionname: qs.action, ActionOptions: actopts });

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

        } else if (false == Csw.isNullOrEmpty(qs.reportid)) {
            //Csw.clientState.setCurrentReport(qs.reportid);
            //Csw.window.location("Main.html");
            handleReport(qs.reportid);
            ret = true;  // load the current context (probably the welcome page) below the report

        } else if (false == Csw.isNullOrEmpty(qs.clear)) {
            Csw.clientState.clearCurrent();
            ret = true;

        } else {
            ret = true;
        }

        if (Csw.bool(qs.debug) || 'dev.html' === Csw.string(qs.pageName).toLowerCase()) {
            Csw.clientSession.enableDebug();
            Csw.cookie.set(Csw.cookie.cookieNames.LogoutPath, 'Dev.html');
            Csw.setGlobalProp('homeUrl', 'Dev.html');
        }

        return ret;
    }

    function initAll(onSuccess) {
        Csw.main.centerBottomDiv.$.CswLogin('init', {
            'onAuthenticate': function (u) {
                Csw.main.headerUsername.text(u)
                     .$.hover(function () { $(this).CswAttrDom('title', Csw.clientSession.getExpireTime()); });
                refreshDashboard();
                refreshHeaderMenu();
                universalsearch = Csw.composites.universalSearch({}, {
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
                            refreshWelcome();
                        }
                    };
                }
                refreshViewSelect(onSuccess);

            } // onAuthenticate
        }); // CswLogin

    }
    initAll();
    function refreshDashboard() {
        Csw.main.headerDashboard.empty().$.CswDashboard();
    }

    // initAll()

    function refreshViewSelect(onSuccess) {
        Csw.main.viewSelectDiv.empty();
        mainviewselect = Csw.main.viewSelectDiv.viewSelect({
            ID: 'mainviewselect',
            onSelect: handleItemSelect,
            onSuccess: onSuccess
        });
    }

    function refreshQuickLaunch() {
        // Refresh the 'Recent' category in the view selector
        mainviewselect.refreshRecent();
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

    function refreshWelcome() {
        clear({ all: true });

        Csw.main.centerBottomDiv.$.CswWelcome('initTable', {
            'onLinkClick': handleItemSelect,
            'onAddClick': function (nodetypeid) {
                $.CswDialog('AddNodeDialog', {
                    'nodetypeid': nodetypeid,
                    'onAddNode': function (nodeid, cswnbtnodekey) {
                        clear({ all: true });
                        refreshNodesTree({ 'nodeid': nodeid, 'cswnbtnodekey': cswnbtnodekey, 'IncludeNodeRequired': true });
                    }
                });
            },
            'onAddComponent': refreshWelcome
        });
        refreshMainMenu();
        refreshQuickLaunch();
    }

    // refreshWelcome()

    function handleItemSelect(options) {
        //if (debugOn()) Csw.debug.log('Main.handleItemSelect()');
        var o = {
            type: 'view', // Action, Report, View, Search
            mode: 'tree', // Grid, Tree, List
            linktype: 'link', // WelcomeComponentType: Link, Search, Text, Add
            itemid: '',
            name: '',
            url: '',
            iconurl: '',
            nodeid: '',
            cswnbtnodekey: ''
        };
        if (options) {
            Csw.extend(o, options);
        }

        multi = false; /* Case 26134. Revert multi-edit selection when switching views, etc. */
        var linkType = Csw.string(o.linktype).toLowerCase();

        var type = Csw.string(o.type).toLowerCase();

        function itemIsSupported() {
            var ret = (linkType === 'search' ||
                false === Csw.isNullOrEmpty(o.itemid) ||
                type === 'action' ||
                type === 'search' ||
                type === 'report');
            return ret;
        }

        if (Csw.clientChanges.manuallyCheckChanges() && itemIsSupported()) {

            if (false === Csw.isNullOrEmpty(type)) {
                switch (type) {
                    case 'action':
                        clear({ all: true });
                        handleAction({
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
                                    getViewGrid({ 'viewid': o.itemid, 'nodeid': o.nodeid, 'cswnbtnodekey': o.cswnbtnodekey, 'showempty': linkOpt.showempty, 'forsearch': linkOpt.forsearch });
                                    break;
                                case 'table':
                                    getViewTable({ 'viewid': o.itemid, 'nodeid': o.nodeid, 'cswnbtnodekey': o.cswnbtnodekey });
                                    break;
                                default:
                                    refreshNodesTree({ 'viewid': o.itemid, 'viewmode': o.mode, 'nodeid': o.nodeid, 'cswnbtnodekey': '', 'showempty': linkOpt.showempty, 'forsearch': linkOpt.forsearch });
                                    break;
                            }
                        };

                        if (Csw.isNullOrEmpty(o.mode)) {
                            Csw.ajax.post({
                                url: Csw.enums.ajaxUrlPrefix + 'getViewMode',
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

                refreshQuickLaunch();
            }
        }   //if (Csw.clientChanges.manuallyCheckChanges() && itemIsSupported()) {

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
            cswnbtnodekey: '',
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
                    SafeNodeKey: o.cswnbtnodekey,
                    NodeTypeId: o.nodetypeid,
                    PropIdAttr: o.propid,
                    LimitMenuTo: o.limitMenuTo,
                    ReadOnly: o.readonly
                }
            },
            onAlterNode: function (nodeid, cswnbtnodekey) {
                var state = Csw.clientState.getCurrent();
                refreshSelected({ 'nodeid': nodeid, 'cswnbtnodekey': cswnbtnodekey, 'IncludeNodeRequired': true, 'searchid': state.searchid });
            },
            onMultiEdit: function () {
                switch (o.viewmode) {
                    case Csw.enums.viewMode.grid.name:
                        o.grid.toggleShowCheckboxes();
                        break;
                    default:
                        multi = (false === multi);
                        Csw.publish('CswMultiEdit', {
                            multi: multi,
                            nodeid: o.nodeid,
                            viewid: o.viewid
                        });
                        //refreshSelected({ nodeid: o.nodeid, viewmode: o.viewmode, cswnbtnodekey: o.cswnbtnodekey });
                        break;
                } // switch
            },
            onEditView: function () {
                handleAction({
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
            Multi: multi,
            nodeTreeCheck: mainTree
        };

        o.parent.menu(menuOpts);

    }

    function getViewGrid(options) {
        var o = {
            viewid: '',
            nodeid: '',
            showempty: false,
            cswnbtnodekey: '',
            doMenuRefresh: true,
            onAddNode: '',
            onEditNode: '',
            onDeleteNode: '',
            onRefresh: ''
        };

        if (options) Csw.extend(o, options);

        // Defaults
        var getEmptyGrid = (Csw.bool(o.showempty));
        if (Csw.isNullOrEmpty(o.nodeid)) {
            o.nodeid = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeId);
        }
        if (Csw.isNullOrEmpty(o.cswnbtnodekey)) {
            o.cswnbtnodekey = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeKey);
        }
        if (false === Csw.isNullOrEmpty(o.viewid)) {
            Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId);
        }

        o.onEditNode = function () { getViewGrid(o); };
        o.onDeleteNode = function () { getViewGrid(o); };
        o.onRefresh = function (options) {
            clear({ centertop: true, centerbottom: true });
            Csw.clientChanges.unsetChanged();
            multi = false;    // semi-kludge for multi-edit batch op
            refreshSelected(options);
        };
        clear({ centertop: true, centerbottom: true });

        var viewfilters = Csw.nbt.viewFilters({
            ID: 'main_viewfilters',
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

        Csw.main.centerBottomDiv.$.CswNodeGrid('init', {
            viewid: o.viewid,
            nodeid: o.nodeid,
            cswnbtnodekey: o.cswnbtnodekey,
            showempty: getEmptyGrid,
            ID: mainGridId,
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
                        //cswnbtnodekey: o.cswnbtnodekey
                    });
                }
            },
            onEditView: function (viewid) {
                handleAction({
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
            nodeid: '',
            cswnbtnodekey: '',
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
        if (Csw.isNullOrEmpty(o.cswnbtnodekey)) {
            o.cswnbtnodekey = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeKey);
        }
        if (false === Csw.isNullOrEmpty(o.viewid)) {
            Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId);
        }

        o.onEditNode = function () { getViewTable(o); };
        o.onDeleteNode = function () { getViewTable(o); };

        clear({ centertop: true, centerbottom: true });

        var viewfilters = Csw.nbt.viewFilters({
            ID: 'main_viewfilters',
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
            nodeid: o.nodeid,
            cswnbtnodekey: o.cswnbtnodekey,
            ID: mainTableId,
            Multi: multi,
            //'onAddNode': o.onAddNode,
            onEditNode: o.onEditNode,
            onDeleteNode: o.onDeleteNode,
            onSuccess: function () {
                refreshMainMenu({
                    viewid: o.viewid,
                    viewmode: Csw.enums.viewMode.table.name//,
                    //                    nodeid: o.nodeid,
                    //                    cswnbtnodekey: o.cswnbtnodekey
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
                cswnbtnodekey: ''
            };
            if (options) {
                Csw.extend(o, options);
            }

            Csw.cookie.set(Csw.cookie.cookieNames.CurrentNodeId, o.nodeid);
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentNodeKey, o.cswnbtnodekey);

            if (o.nodeid !== '' && o.nodeid !== 'root') {
                getTabs({ 'nodeid': o.nodeid, 'cswnbtnodekey': o.cswnbtnodekey });
                refreshMainMenu({
                    parent: o.tree.menuDiv,
                    viewid: o.viewid,
                    viewmode: Csw.enums.viewMode.tree.name,
                    nodeid: o.nodeid,
                    cswnbtnodekey: o.cswnbtnodekey
                });
            } else {
                showDefaultContentTree({ viewid: o.viewid, viewmode: Csw.enums.viewMode.tree.name });
                refreshMainMenu({
                    parent: o.tree.menuDiv,
                    viewid: o.viewid,
                    viewmode: Csw.enums.viewMode.tree.name,
                    nodeid: '',
                    cswnbtnodekey: ''
                });
            }
        }
    }; // onSelectTreeNode()

    function showDefaultContentTree(viewopts) {
        var v = {
            viewid: '',
            viewmode: '',
            onAddNode: function (nodeid, cswnbtnodekey) {
                refreshSelected({ 'nodeid': nodeid, 'cswnbtnodekey': cswnbtnodekey, 'IncludeNodeRequired': true });
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
            onAddNode: function (nodeid, cswnbtnodekey) {
                refreshSelected({ 'nodeid': nodeid, 'cswnbtnodekey': cswnbtnodekey, 'IncludeNodeRequired': true });
            }
        };
        if (viewopts) Csw.extend(v, viewopts);
        clear({ centerbottom: true });
        var div = Csw.main.centerBottomDiv.div({
            ID: 'deftbldiv',
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
            cswnbtnodekey: ''
        };
        Csw.extend(o, options);

        clear({ right: true });

        Csw.layouts.tabsAndProps(Csw.main.rightDiv, {
            ID: 'nodetabs',
            globalState: {
                nodeids: [o.nodeid],
                nodekeys: [o.cswnbtnodekey]
            },
            tabState: {
                ShowCheckboxes: multi,
                tabid: Csw.cookie.get(Csw.cookie.cookieNames.CurrentTabId)
            },
            onSave: function () {
                Csw.clientChanges.unsetChanged();
            },
            onBeforeTabSelect: function () {
                return Csw.clientChanges.manuallyCheckChanges();
            },
            Refresh: function (options) {
                Csw.clientChanges.unsetChanged();
                multi = false;    // semi-kludge for multi-edit batch op
                refreshSelected(options);
            },
            onTabSelect: function (tabid) {
                Csw.cookie.set(Csw.cookie.cookieNames.CurrentTabId, tabid);
            },
            onPropertyChange: function () {
                Csw.clientChanges.setChanged();
            },
            onEditView: function (viewid) {
                handleAction({
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
    }

    function refreshSelected(options) {
        Csw.publish('initPropertyTearDown');
        if (Csw.clientChanges.manuallyCheckChanges()) {
            var o = {
                nodeid: '',
                cswnbtnodekey: '',
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

            if (false === Csw.isNullOrEmpty(o.searchid)) { //if we have a searchid, we are probably looking at a search
                universalsearch.restoreSearch(o.searchid);
            } else {
                var viewMode = Csw.string(o.viewmode).toLowerCase();
                switch (viewMode) {
                    case 'grid':
                        getViewGrid({
                            viewid: o.viewid,
                            nodeid: o.nodeid,
                            cswnbtnodekey: o.cswnbtnodekey,
                            showempty: o.showempty,
                            forsearch: o.forsearch
                        });
                        break;
                    case 'list':
                        refreshNodesTree({
                            nodeid: o.nodeid,
                            cswnbtnodekey: o.cswnbtnodekey,
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
                            viewid: o.viewid,
                            nodeid: o.nodeid,
                            cswnbtnodekey: o.cswnbtnodekey
                        });
                        break;
                    case 'tree':
                        refreshNodesTree({
                            nodeid: o.nodeid,
                            cswnbtnodekey: o.cswnbtnodekey,
                            nodename: o.nodename,
                            viewid: o.viewid,
                            viewmode: o.viewmode,
                            showempty: o.showempty,
                            forsearch: o.forsearch,
                            IncludeNodeRequired: o.IncludeNodeRequired
                        });
                        break;
                    default:
                        refreshWelcome();
                        break;
                } // switch
            } // if (false === Csw.isNullOrEmpty(o.searchid))
        } // if (manuallyCheckChanges())
    } // refreshSelected()
    Csw.subscribe(Csw.enums.events.main.refreshSelected, refreshSelected);

    var multi = false;

    function refreshNodesTree(options) {
        var o = {
            'nodeid': '',
            'cswnbtnodekey': '',
            'nodename': '',
            'showempty': false,
            'forsearch': false,
            'iconurl': '',
            'viewid': '',
            'viewmode': 'tree',
            'IncludeNodeRequired': false
        };
        Csw.extend(o, options);

        var getEmptyTree = (Csw.bool(o.showempty));
        if (Csw.isNullOrEmpty(o.nodeid)) {
            o.nodeid = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeId);
            if (Csw.isNullOrEmpty(o.cswnbtnodekey)) {
                o.cswnbtnodekey = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeKey);
            }
        }
        if (Csw.isNullOrEmpty(o.viewid)) {
            o.viewid = Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId);
        }

        clear({ left: true });

        var viewfilters = Csw.nbt.viewFilters({
            ID: 'main_viewfilters',
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

        mainTree = Csw.nbt.nodeTree({
            ID: 'main',
            parent: Csw.main.leftDiv,
            forsearch: o.forsearch,
            onSelectNode: function (optSelect) {
                onSelectTreeNode({
                    tree: mainTree,
                    viewid: optSelect.viewid,
                    nodeid: optSelect.nodeid,
                    cswnbtnodekey: optSelect.cswnbtnodekey
                });
            },
            ShowCheckboxes: multi
        });
        mainTree.init({
            viewid: o.viewid,
            viewmode: o.viewmode,
            nodeid: o.nodeid,
            cswnbtnodekey: o.cswnbtnodekey,
            IncludeNodeRequired: o.IncludeNodeRequired,
            onViewChange: function (newviewid, newviewmode) {
                Csw.clientState.setCurrentView(newviewid, newviewmode);
            }
        });
    }    // refreshNodesTree()


    function handleAction(options) {
        var o = {
            'actionname': '',
            'actionurl': '',
            'ActionOptions': {}
        };
        if (options) {
            Csw.extend(o, options);
        }

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
                    ID: 'cswInspectionDesignWizard',
                    viewid: o.ActionOptions.viewid,
                    viewmode: o.ActionOptions.viewmode,
                    onCancel: function () {
                        clear({ 'all': true });
                        Csw.clientState.setCurrent(Csw.clientState.getLast());
                        refreshSelected();
                    },
                    onFinish: function (viewid) {
                        clear({ 'all': true });
                        refreshViewSelect(function () {
                            handleItemSelect({
                                type: 'view',
                                mode: 'tree',
                                itemid: viewid
                            });
                        });
                    },
                    startingStep: o.ActionOptions.startingStep,
                    menuRefresh: refreshSelected
                };
                Csw.nbt.createInspectionWizard(Csw.main.centerTopDiv, designOpt);

                break;

            case 'create material':
                var createOpt = {
                    onCancel: function () {
                        clear({ 'all': true });
                        Csw.clientState.setCurrent(Csw.clientState.getLast());
                        refreshSelected();
                    },
                    onFinish: function (viewid) {
                        clear({ 'all': true });
                        handleItemSelect({
                            type: 'view',
                            mode: 'tree',
                            itemid: viewid
                        });
                    },
                    startingStep: o.ActionOptions.startingStep
                };
                Csw.nbt.createMaterialWizard(Csw.main.centerTopDiv, createOpt);
                break;

            case 'dispensecontainer':
                var requestItemId = '';
                if (Csw.contains(o, 'requestitem')) {
                    requestItemId = o.requestitem.requestitemid;
                }
                var title = 'Dispense from ';
                if (false === Csw.isNullOrEmpty(o.barcode)) {
                    title += 'Barcode [' + o.barcode + ']';
                } else {
                    title += 'Selected Container';
                }
                designOpt = {
                    ID: Csw.makeId('cswDispenseContainerWizard'),
                    state: {
                        sourceContainerNodeId: o.sourceContainerNodeId,
                        currentQuantity: o.currentQuantity,
                        currentUnitName: o.currentUnitName,
                        precision: o.precision,
                        initialQuantity: Csw.deserialize(o.initialQuantity),
                        requestItemId: requestItemId,
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
                        clear({ 'all': true });
                        Csw.clientState.setCurrent(Csw.clientState.getLast());
                        refreshSelected();
                    },
                    onFinish: function (viewid) {
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

            case 'edit view':
                var editViewOptions = {
                    'viewid': o.ActionOptions.viewid,
                    'viewmode': o.ActionOptions.viewmode,
                    'onCancel': function () {
                        clear({ 'all': true });
                        Csw.clientState.setCurrent(Csw.clientState.getLast());
                        refreshSelected();
                    },
                    'onFinish': function (viewid, viewmode) {
                        clear({ 'all': true });
                        refreshViewSelect();
                        if (Csw.bool(o.ActionOptions.IgnoreReturn)) {
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            refreshSelected();
                        } else {
                            handleItemSelect({ itemid: viewid, mode: viewmode });
                        }
                    },
                    onAddView: function (deletedviewid) {
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
                        refreshViewSelect();
                    },
                    'startingStep': o.ActionOptions.startingStep
                };

                Csw.main.centerTopDiv.$.CswViewEditor(editViewOptions);
                break;


            case 'future scheduling':
                Csw.nbt.futureSchedulingWizard(Csw.main.centerTopDiv, {
                    onCancel: refreshSelected,
                    onFinish: function (viewid, viewmode) {
                        handleItemSelect({ itemid: viewid, mode: viewmode });
                    }
                });
                break;

            //			case 'Import_Fire_Extinguisher_Data':                                                                                            
            //				break;                                                                                            
            //			case 'Inspection_Design':                                                                                            
            //				break;                                                                                            
            case 'quotas':
                Csw.actions.quotas(Csw.main.centerTopDiv, {
                    onQuotaChange: function () {
                        Csw.actions.quotaImage(Csw.main.headerQuota);
                    }
                });

                break;
            case 'modules':
                Csw.actions.modules(Csw.main.centerTopDiv, {
                    onModuleChange: function () {
                        refreshDashboard();
                        refreshViewSelect();
                    }
                });
                break;
            case 'receiving':
                o.onFinish = function (viewid) {
                    clear({ 'all': true });
                    handleItemSelect({
                        type: 'view',
                        mode: 'tree',
                        itemid: viewid
                    });
                };
                o.onCancel = function () {
                    clear({ 'all': true });
                    Csw.clientState.setCurrent(Csw.clientState.getLast());
                    refreshSelected();
                };
                Csw.nbt.receiveMaterialWizard(Csw.main.centerTopDiv, o);
                break;
            case 'sessions':
                Csw.actions.sessions(Csw.main.centerTopDiv);
                break;

            case 'submit request':
                Csw.actions.submitRequest(Csw.main.centerTopDiv, {
                    onSubmit: function () {
                        clear({ 'all': true });
                        Csw.clientState.setCurrent(Csw.clientState.getLast());
                        refreshSelected();
                        refreshHeaderMenu();
                    },
                    onCancel: function () {
                        clear({ 'all': true });
                        Csw.clientState.setCurrent(Csw.clientState.getLast());
                        refreshSelected();
                    }
                });
                break;

            case 'subscriptions':
                Csw.actions.subscriptions(Csw.main.centerTopDiv);
                break;

            case 'view scheduled rules':
                var rulesOpt = {
                    exitFunc: function () {
                        clear({ 'all': true });
                        Csw.clientState.setCurrent(Csw.clientState.getLast());
                        refreshSelected();
                    },
                    menuRefresh: refreshSelected
                };

                Csw.nbt.scheduledRulesWizard(Csw.main.centerTopDiv, rulesOpt);
                break;

            default:
                if (false == Csw.isNullOrEmpty(o.actionurl)) {
                    Csw.window.location(o.actionurl);
                }
                break;
        }
    }
    Csw.subscribe(Csw.enums.events.main.handleAction, function (eventObj, opts) {
        handleAction(opts);
    });
    // _handleAction()
};

