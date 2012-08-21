/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="../globals/CswEnums.js" />

window.initMain = window.initMain || function (undefined) {

    "use strict";
    //Csw.debug.group('Csw');
    var mainTree;
    var mainGridId = 'CswNodeGrid';
    var mainTableId = 'CswNodeTable';

    var universalsearch;

    var mainviewselect;

    function startSpinner() {
        $('#ajaxSpacer').hide();
        $('#ajaxImage').show();
        //if (true === window.displayAllExceptions) {
        //    Csw.debug.group('ajax');
        //}
    }
    Csw.subscribe(Csw.enums.events.ajax.globalAjaxStart, startSpinner);

    function stopSpinner() {
        $('#ajaxImage').hide();
        $('#ajaxSpacer').show();
        //if (true === window.displayAllExceptions) {
        //    Csw.debug.groupEnd('ajax');
        //}
    };
    Csw.subscribe(Csw.enums.events.ajax.globalAjaxStop, stopSpinner);

    function onObjectClassButtonClick(eventOj, opts) {
        Csw.debug.assert(false === Csw.isNullOrEmpty(opts.data), 'opts.data is null.');
        var actionJson = opts.data.actionData;
        Csw.publish(Csw.enums.events.afterObjectClassButtonClick, opts.data.action);
        switch (Csw.string(opts.data.action).toLowerCase()) {
            case Csw.enums.nbtButtonAction.nothing:
                //Do nothing
                break;
            case Csw.enums.nbtButtonAction.dispense:
                clear({ centertop: true, centerbottom: true });
                actionJson.actionname = 'DispenseContainer';
                handleAction(actionJson);
                break;
            case Csw.enums.nbtButtonAction.editprop:
                $.CswDialog('EditNodeDialog', {
                    nodeids: [Csw.string(actionJson.nodeid)],
                    filterToPropId: Csw.string(actionJson.propidattr),
                    title: Csw.string(actionJson.title),
                    onEditNode: function (nodeid, nodekey, close) {
                        Csw.tryExec(close);
                    }
                });
                break;

            case Csw.enums.nbtButtonAction.loadView:
                clear({ centertop: true, centerbottom: true });
                Csw.debug.assert(false === Csw.isNullOrEmpty(actionJson), 'actionJson is null.');
                Csw.publish(Csw.enums.events.RestoreViewContext, actionJson);
                break;

            case Csw.enums.nbtButtonAction.popup:
                Csw.debug.assert(false === Csw.isNullOrEmpty(actionJson), 'actionJson is null.');
                Csw.openPopup(actionJson.url);
                break;

            case Csw.enums.nbtButtonAction.reauthenticate:
                clear({ centertop: true, centerbottom: true });
                /* case 24669 */
                Csw.cookie.clearAll();
                Csw.ajax.post({
                    urlMethod: 'reauthenticate',
                    data: { PropId: Csw.string(opts.propid) },
                    success: function () {
                        Csw.clientChanges.unsetChanged();
                        Csw.window.location('Main.html');
                    }
                });
                
                break;

            case Csw.enums.nbtButtonAction.receive:
                clear({ centertop: true, centerbottom: true });
                actionJson.actionname = 'Receiving';
                handleAction(actionJson);
                break;

            case Csw.enums.nbtButtonAction.request:
                Csw.debug.assert(false === Csw.isNullOrEmpty(actionJson), 'actionJson is null.');
                switch (actionJson.requestaction) {
                    case 'Dispose':
                        refreshHeaderMenu();
                        break;
                    default:
                        $.CswDialog('AddNodeDialog', {
                            nodetypeid: actionJson.requestItemNodeTypeId,
                            propertyData: actionJson.requestItemProps,
                            text: actionJson.titleText,
                            onSaveImmediate: function () {
                                refreshHeaderMenu();
                            }
                        });
                        break;
                }
                break;

            default:
                Csw.debug.error('No event has been defined for button click ' + opts.data.action);
                break;
        }
    }
    Csw.subscribe(Csw.enums.events.objectClassButtonClick, onObjectClassButtonClick);

    function refreshMain(eventObj, data) {
        Csw.clientChanges.unsetChanged();
        multi = false;
        clear({ all: true });
        Csw.tryExec(refreshSelected, data);
    }
    Csw.subscribe('refreshMain', refreshMain);

    function loadImpersonation(eventObj, actionData) {
        if (false === Csw.isNullOrEmpty(actionData.userid)) {
            handleImpersonation(actionData.userid, actionData.username, function () {
                initAll(function () {
                    handleItemSelect({
                        actionid: actionData.actionid,
                        viewid: actionData.viewid,
                        nodeid: actionData.selectedNodeId,
                        viewmode: actionData.viewmode,
                        actionname: actionData.actionname,
                        actionurl: actionData.actionurl,
                        type: actionData.type
                    });
                });
            });
        } else {
            handleItemSelect({
                actionid: actionData.actionid,
                viewid: actionData.viewid,
                nodeid: actionData.selectedNodeId,
                viewmode: actionData.viewmode,
                actionname: actionData.actionname,
                actionurl: actionData.actionurl,
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
                    $('#watermark').text(result.watermark);
                }
            }
        });
    }

    function handleImpersonation(userid, username, onSuccess) {
        var u = Csw.cookie.get(Csw.cookie.cookieNames.Username);
        Csw.ajax.post({
            url: '/NbtWebApp/wsNBT.asmx/impersonate',
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
        var $header = $('#header_menu');
        var u = Csw.cookie.get(Csw.cookie.cookieNames.Username);
        $header.empty();
        $header.CswMenuHeader({
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
                    url: '/NbtWebApp/wsNBT.asmx/endImpersonation',
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

    initAll();

    function initAll(onSuccess) {
        //if (debugOn()) Csw.debug.log('Main.initAll()');
        $('#CenterBottomDiv').CswLogin('init', {
            'onAuthenticate': function (u) {
                $('#header_username').text(u)
                     .hover(function () { $(this).CswAttrDom('title', Csw.clientSession.getExpireTime()); });
                refreshDashboard();
                refreshHeaderMenu();
                universalsearch = Csw.composites.universalSearch({}, {
                    $searchbox_parent: $('#SearchDiv'),
                    $searchresults_parent: $('#RightDiv'),
                    $searchfilters_parent: $('#LeftDiv'),
                    onBeforeSearch: function () {
                        clear({ all: true });
                    },
                    onAfterSearch: function () {
                        refreshMainMenu();
                    },
                    onAfterNewSearch: function (searchid) {
                        Csw.clientState.setCurrentSearch(searchid);
                    },
                    onAddView: function (viewid, viewmode) {
                        refreshViewSelect();
                    },
                    onLoadView: function (viewid, viewmode) {
                        handleItemSelect({
                            'type': 'view',
                            'viewid': viewid,
                            'viewmode': viewmode
                        });
                    }
                });

                var headerQuota = Csw.literals.factory($('#header_quota'));
                Csw.actions.quotaImage(headerQuota);



                // handle querystring arguments
                var loadCurrent = false;
                var qs = Csw.queryString();
                if (false == Csw.isNullOrEmpty(qs.action)) {
                    var actopts = {};
                    $.extend(actopts, qs);
                    handleAction({ actionname: qs.action, ActionOptions: actopts });

                } else if (false == Csw.isNullOrEmpty(qs.viewid)) {
                    var setView = function (viewid, viewmode) {
                        handleItemSelect({
                            type: 'view',
                            viewid: viewid,
                            viewmode: viewmode
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
                    loadCurrent = true;  // load the current context (probably the welcome page) below the report

                } else if (false == Csw.isNullOrEmpty(qs.clear)) {
                    Csw.clientState.clearCurrent();
                    loadCurrent = true;

                } else {
                    loadCurrent = true;
                }

                if (Csw.isNullOrEmpty(onSuccess) && loadCurrent) {
                    onSuccess = function () {
                        var current = Csw.clientState.getCurrent();
                        if (false === Csw.isNullOrEmpty(current.viewid)) {
                            handleItemSelect({
                                'type': 'view',
                                'viewid': current.viewid,
                                'viewmode': current.viewmode
                            });
                        } else if (false === Csw.isNullOrEmpty(current.actionname)) {
                            handleItemSelect({
                                'type': 'action',
                                'actionname': current.actionname,
                                'actionurl': current.actionurl
                            });
                        } else if (false === Csw.isNullOrEmpty(current.reportid)) {
                            handleItemSelect({
                                'type': 'report',
                                'reportid': current.reportid
                            });
                        } else if (false === Csw.isNullOrEmpty(current.searchid)) {
                            handleItemSelect({
                                'type': 'search',
                                'searchid': current.searchid
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

    function refreshDashboard() {
        $('#header_dashboard').empty().CswDashboard();
    }

    // initAll()

    function refreshViewSelect(onSuccess) {
        var selectDiv = Csw.literals.factory($('#ViewSelectDiv'));
        selectDiv.empty();
        mainviewselect = selectDiv.viewSelect({
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
            $.extend(o, options);
        }

        if (o.all || o.left) {
            $('#LeftDiv').empty();
        }
        if (o.all || o.right) {
            $('#RightDiv').empty();
        }
        if (o.all || o.centertop) {
            $('#CenterTopDiv').empty();
        }
        if (o.all || o.centerbottom) {
            $('#CenterBottomDiv').empty();
        }
        if (o.all) {
            Csw.setGlobalProp('uniqueIdCount', 0);
            $('#MainMenuDiv').empty();
        }
    }

    function refreshWelcome() {
        //if (debugOn()) Csw.debug.log('Main.refreshWelcome()');
        clear({ all: true });

        $('#CenterBottomDiv').CswWelcome('initTable', {
            'onLinkClick': handleItemSelect,
            //            'onSearchClick': function (view) {
            //                var viewid = view.viewid;
            //                var viewmode = view.viewmode;
            //                handleItemSelect({ 'viewid': viewid, 'viewmode': viewmode, 'linktype': 'search' });
            //                refreshSearchPanel({ 'viewid': viewid, 'searchType': 'view' });
            //            },
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
            viewmode: 'tree', // Grid, Tree, List
            linktype: 'link', // WelcomeComponentType: Link, Search, Text, Add
            viewid: '',
            actionname: '',
            actionurl: '',
            reportid: '',
            searchid: '',
            nodeid: '',
            cswnbtnodekey: ''
        };
        if (options) {
            $.extend(o, options);
        }

        multi = false; /* Case 26134. Revert multi-edit selection when switching views, etc. */
        var linkType = Csw.string(o.linktype).toLowerCase();

        var type = Csw.string(o.type).toLowerCase();

        function itemIsSupported() {
            var ret = (linkType === 'search' ||
                       false === Csw.isNullOrEmpty(o.viewid) ||
                       type === 'action' ||
                       type === 'search' ||
                       type === 'report');
            return ret;
        }

        if (Csw.clientChanges.manuallyCheckChanges() && itemIsSupported()) {

            if (false === Csw.isNullOrEmpty(o.viewid)) {
                clear({ all: true });
                var renderView = function () {

                    Csw.clientState.setCurrentView(o.viewid, o.viewmode);

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
                    var viewMode = Csw.string(o.viewmode).toLowerCase();
                    switch (viewMode) {
                        case 'grid':
                            getViewGrid({ 'viewid': o.viewid, 'nodeid': o.nodeid, 'cswnbtnodekey': o.cswnbtnodekey, 'showempty': linkOpt.showempty, 'forsearch': linkOpt.forsearch });
                            break;
                        case 'table':
                            getViewTable({ 'viewid': o.viewid, 'nodeid': o.nodeid, 'cswnbtnodekey': o.cswnbtnodekey });
                            break;
                        default:
                            refreshNodesTree({ 'viewid': o.viewid, 'viewmode': o.viewmode, 'nodeid': '', 'cswnbtnodekey': '', 'showempty': linkOpt.showempty, 'forsearch': linkOpt.forsearch });
                            break;
                    }
                };

                if (Csw.isNullOrEmpty(o.viewmode)) {
                    Csw.ajax.post({
                        url: Csw.enums.ajaxUrlPrefix + 'getViewMode',
                        data: { ViewId: o.viewid },
                        success: function (data) {
                            o.viewmode = Csw.string(data.viewmode, 'tree');
                            renderView();
                        }
                    });
                } else {
                    renderView();
                }

            } else if (false === Csw.isNullOrEmpty(type)) {
                switch (type) {
                    case 'action':
                        clear({ all: true });
                        handleAction({
                            'actionname': o.actionname,
                            'actionurl': o.actionurl
                        });
                        break;
                    case 'search':
                        clear({ all: true });
                        universalsearch.restoreSearch(o.searchid);
                        break;
                    case 'report':
                        handleReport(o.reportid);
                        break;
                }
            }

            refreshQuickLaunch();
        } else { // if (manuallyCheckChanges() && itemIsSupported())
            //do nothing
        }
    }


    function handleReport(reportid) {
        Csw.openPopup("Report.html?reportid=" + reportid);
    }

    function refreshMainMenu(options) {
        //if (debugOn()) Csw.debug.log('Main.refreshMainMenu()');

        var o = {
            viewid: '',
            viewmode: '',
            nodeid: '',
            cswnbtnodekey: '',
            prefix: 'csw',
            grid: ''
        };

        if (options) {
            $.extend(o, options);
        }

        $('#MainMenuDiv').CswMenuMain({
            'viewid': o.viewid,
            'nodeid': o.nodeid,
            'cswnbtnodekey': o.cswnbtnodekey,
            'onAddNode': function (nodeid, cswnbtnodekey) {
                refreshSelected({ 'nodeid': nodeid, 'cswnbtnodekey': cswnbtnodekey, 'IncludeNodeRequired': true });
            },
            'onMultiEdit': function () {
                switch (o.viewmode) {
                    case Csw.enums.viewMode.grid.name:
                        //                        multi = (false === o.grid.isMulti());
                        //                        var g = {
                        //                            canEdit: multi,
                        //                            canDelete: multi,
                        //                            gridOpts: {
                        //                                //reinit: true,
                        //                                multiselect: multi
                        //                            }
                        //                        };
                        //                        o.grid.changeGridOpts(g, ['Action', 'Delete']);

                        o.grid.toggleShowCheckboxes();

                        break;
                    default:
                        multi = (false === multi);
                        refreshSelected({ nodeid: o.nodeid, viewmode: o.viewmode, cswnbtnodekey: o.cswnbtnodekey });
                        break;
                }
            },
            'onPrintView': function () {
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
            //            'onSearch':
            //                 {
            //                     'onViewSearch': function () {
            //                         var genericSearchId = Csw.makeId({ 'ID': mainSearchId, prefix: o.prefix, suffix: 'generic' });
            //                         var viewSearchId = Csw.makeId({ 'ID': mainSearchId, prefix: o.prefix, suffix: 'view' });
            //                         refreshSearchPanel({
            //                             'genericSearchId': genericSearchId,
            //                             'viewSearchId': viewSearchId,
            //                             'searchType': 'view',
            //                             'cswnbtnodekey': o.cswnbtnodekey,
            //                             'viewid': o.viewid
            //                         });
            //                     },
            //                     'onGenericSearch': function () {
            //                         var genericSearchId = Csw.makeId({ 'ID': mainSearchId, prefix: o.prefix, suffix: 'generic' });
            //                         var viewSearchId = Csw.makeId({ 'ID': mainSearchId, prefix: o.prefix, suffix: 'view' });
            //                         refreshSearchPanel({
            //                             'genericSearchId': genericSearchId,
            //                             'viewSearchId': viewSearchId,
            //                             'searchType': 'generic',
            //                             'cswnbtnodekey': o.cswnbtnodekey
            //                         });
            //                     }
            //                 },
            'onEditView': function () {
                handleAction({
                    'actionname': 'Edit_View',
                    'ActionOptions': {
                        'viewid': Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId),
                        'viewmode': Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewMode)
                    }
                });
            },
            'onSaveView': function (newviewid) {
                handleItemSelect({ 'viewid': newviewid, 'viewmode': Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewMode) });
            },
            'Multi': multi,
            nodeTreeCheck: mainTree
        });
    }

    //    function refreshSearchPanel(options) {
    //        //if (debugOn()) Csw.debug.log('Main.refreshSearchPanel()');

    //        var o = {
    //            viewid: '',
    //            nodetypeid: '',
    //            cswnbtnodekey: '',
    //            viewSearchId: '',
    //            genericSearchId: '',
    //            searchType: 'generic'
    //        };

    //        if (options) {
    //            $.extend(o, options);
    //        }

    //        $('#CenterTopDiv').children('#' + o.viewSearchId)
    //             .empty();
    //        $('#CenterTopDiv').children('#' + o.genericSearchId)
    //             .empty();
    //        var $thisSearchForm = '';

    //        switch (o.searchType.toLowerCase()) {
    //            case 'generic':
    //                {
    //                    $thisSearchForm = makeSearchForm({ 'cswnbtnodekey': o.cswnbtnodekey, 'ID': o.genericSearchId });
    //                    break;
    //                }
    //            case 'view':
    //                {
    //                    $thisSearchForm = makeSearchForm({ 'viewid': o.viewid, 'cswnbtnodekey': o.cswnbtnodekey, 'ID': o.viewSearchId });
    //                    break;
    //                }
    //        }
    //        return $thisSearchForm;
    //    }

    //    function makeSearchForm(options) {
    //        var o = {
    //            viewid: '',
    //            nodetypeid: '',
    //            cswnbtnodekey: '',
    //            ID: ''
    //        };
    //        if (options) {
    //            $.extend(o, options);
    //        }

    //        clear({ centertop: true });

    //        var onSearchSubmit = function (searchviewid, searchviewmode) {
    //            clear({ right: true, centerbottom: true });
    //            var viewMode = searchviewmode;
    //            if (viewMode === 'list') {
    //                viewMode = 'tree';
    //            }
    //            Csw.clientState.setCurrentView(searchviewid, viewMode);

    //            refreshSelected({
    //                viewmode: viewMode,
    //                viewid: searchviewid,
    //                forsearch: true,
    //                showempty: false,
    //                cswnbtnodekey: '', //do not want a view of only this node
    //                nodeid: ''
    //            });
    //        };
    //        var onClearSubmit = function (parentviewid, parentviewmode) {
    //            clear({ centertop: true }); //clear Search first

    //            var viewid;
    //            if (Csw.isNullOrEmpty(parentviewid)) {
    //                viewid = Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId);
    //            } else {
    //                viewid = parentviewid;
    //            }

    //            if (false === Csw.isNullOrEmpty(viewid)) {
    //                clear({ right: true, centerbottom: true }); //wait to clear rest until we have a valid viewid
    //                var viewmode;
    //                if (Csw.isNullOrEmpty(parentviewmode)) {
    //                    viewmode = Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewMode);
    //                } else {
    //                    viewmode = (parentviewmode === 'list') ? 'tree' : parentviewmode;
    //                }

    //                Csw.clientState.setCurrentView(viewid, viewmode);

    //                refreshSelected({
    //                    viewmode: viewmode,
    //                    viewid: viewid,
    //                    forsearch: true,
    //                    showempty: false,
    //                    cswnbtnodekey: '', //do not want a view of only this node
    //                    nodeid: ''
    //                });
    //            }
    //        };
    //        var onSearchClose = function () {
    //            clear({ centertop: true });
    //        };
    //        var $search = $('#CenterTopDiv').CswSearch({
    //            'parentviewid': o.viewid,
    //            'cswnbtnodekey': o.cswnbtnodekey,
    //            'nodetypeid': o.nodetypeid,
    //            'ID': o.ID,
    //            'onSearchSubmit': onSearchSubmit,
    //            'onClearSubmit': onClearSubmit,
    //            'onSearchClose': onSearchClose
    //        });
    //        return $search;
    //    }

    function getViewGrid(options) {
        //if (debugOn()) Csw.debug.log('Main.getViewGrid()');

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

        if (options) $.extend(o, options);

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
            parent: Csw.literals.factory($('#CenterTopDiv')),
            viewid: o.viewid,
            onEditFilters: function (newviewid) {
                var newopts = o;
                newopts.viewid = newviewid;
                // set the current view to be the session view, so filters are saved
                Csw.clientState.setCurrentView(newviewid, Csw.enums.viewMode.grid.name);
                getViewGrid(newopts);
            } // onEditFilters
        }); // viewFilters

        $('#CenterBottomDiv').CswNodeGrid('init', {
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
            $.extend(o, options);
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
            parent: Csw.literals.factory($('#CenterTopDiv')),
            viewid: o.viewid,
            onEditFilters: function (newviewid) {
                var newopts = o;
                newopts.viewid = newviewid;
                // set the current view to be the session view, so filters are saved
                Csw.clientState.setCurrentView(newviewid, Csw.enums.viewMode.table.name);
                getViewTable(newopts);
            } // onEditFilters
        }); // viewFilters
        
        $('#CenterBottomDiv').CswNodeTable('init', {
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
                viewid: '',
                nodeid: '',
                nodename: '',
                iconurl: '',
                cswnbtnodekey: ''
            };
            if (options) {
                $.extend(o, options);
            }

            Csw.cookie.set(Csw.cookie.cookieNames.CurrentNodeId, o.nodeid);
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentNodeKey, o.cswnbtnodekey);

            if (o.nodeid !== '' && o.nodeid !== 'root') {
                getTabs({ 'nodeid': o.nodeid, 'cswnbtnodekey': o.cswnbtnodekey });
                refreshMainMenu({ viewid: o.viewid, viewmode: Csw.enums.viewMode.tree.name, nodeid: o.nodeid, cswnbtnodekey: o.cswnbtnodekey });
            } else {
                showDefaultContentTree({ viewid: o.viewid, viewmode: Csw.enums.viewMode.tree.name });
                refreshMainMenu({ viewid: o.viewid, viewmode: Csw.enums.viewMode.tree.name, nodeid: '', cswnbtnodekey: '' });
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
        if (viewopts) $.extend(v, viewopts);
        clear({ right: true });
        $('#RightDiv').CswDefaultContent(v);

    } // showDefaultContentTree()

    function showDefaultContentTable(viewopts) {
        var v = {
            viewid: '',
            viewmode: '',
            onAddNode: function (nodeid, cswnbtnodekey) {
                refreshSelected({ 'nodeid': nodeid, 'cswnbtnodekey': cswnbtnodekey, 'IncludeNodeRequired': true });
            }
        };
        if (viewopts) $.extend(v, viewopts);
        clear({ centerbottom: true });
        var div = Csw.literals.div({
            $parent: $('#CenterBottomDiv'),
            ID: 'deftbldiv',
            align: 'center'
        });
        div.css({ textAlign: 'center' });
        div.append('No Results.');

        div.$.CswDefaultContent(v);

    } // showDefaultContentTable()

    function getTabs(options) {
        //if (debugOn()) Csw.debug.log('Main.getTabs()');

        var o = {
            nodeid: '',
            cswnbtnodekey: ''
        };
        if (options) $.extend(o, options);

        clear({ right: true });
        var parent = Csw.literals.factory($('#RightDiv'));
        //$('#RightDiv').CswNodeTabs({
        Csw.layouts.tabsAndProps(parent, {
            ID: 'nodetabs',
            nodeids: [o.nodeid],
            nodekeys: [o.cswnbtnodekey],
            onSave: function () {
                Csw.clientChanges.unsetChanged();
                // case 24304
                // refreshSelected({ 'nodeid': nodeid, 'cswnbtnodekey': nodekey });
            },
            tabid: Csw.cookie.get(Csw.cookie.cookieNames.CurrentTabId),
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
                    'actionname': 'Edit_View',
                    'ActionOptions': {
                        viewid: viewid,
                        viewmode: Csw.enums.viewMode.grid.name,
                        startingStep: 2,
                        IgnoreReturn: true
                    }
                });
            },
            ShowCheckboxes: multi,
            nodeTreeCheck: mainTree
        });
    }

    function refreshSelected(options) {
        //if (debugOn()) Csw.debug.log('Main.refreshSelected()');

        if (Csw.clientChanges.manuallyCheckChanges()) {
            var o = {
                nodeid: '',
                cswnbtnodekey: '',
                nodename: '',
                iconurl: '',
                viewid: '',
                viewmode: '',
                showempty: false,
                forsearch: false,
                IncludeNodeRequired: false
            };
            if (options) $.extend(o, options);

            if (Csw.isNullOrEmpty(o.viewid)) {
                o.viewid = Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId);
            }
            if (Csw.isNullOrEmpty(o.viewmode)) {
                o.viewmode = Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewMode);
            }

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
                    //default: //tree
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
                    // reload the welcome page
                    refreshWelcome();
                    break;
            } // switch
        } // if (manuallyCheckChanges())
    }

    // refreshSelected()

    var multi = false;

    function refreshNodesTree(options) {
        //if (debugOn()) Csw.debug.log('Main.refreshNodesTree()');

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
        if (options) {
            $.extend(o, options);
        }

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
            parent: Csw.literals.factory($('#LeftDiv')),
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
            parent: Csw.literals.factory($('#LeftDiv')),
            forsearch: o.forsearch,
            //showempty: getEmptyTree,
            onSelectNode: function (optSelect) {
                onSelectTreeNode({
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
            //nodename: o.nodename,
            IncludeNodeRequired: o.IncludeNodeRequired,
            onViewChange: function (newviewid, newviewmode) {
                Csw.clientState.setCurrentView(newviewid, newviewmode);
            }
        });
    }

    // refreshNodesTree()


    function handleAction(options) {
        var o = {
            'actionname': '',
            'actionurl': '',
            'ActionOptions': {}
        };
        if (options) {
            $.extend(o, options);
        }

        var designOpt = {};

        Csw.clientState.setCurrentAction(o.actionname, o.actionurl);
        var centerTopDiv = Csw.literals.factory($('#CenterTopDiv'));

        Csw.ajax.post({
            urlMethod: 'SaveActionToQuickLaunch',
            'data': { 'ActionName': o.actionname }
        });

        clear({ 'all': true });
        refreshMainMenu();
        switch (o.actionname) {
            //			case 'Assign_Inspection':                                                                                
            //				break;                                                                                
            //			case 'Assign_Tests':                                                                                
            //				break;                                                                                
            // NOTE: Create Inspection currently only works if you are logged in as chemsw_admin                                                                                
            case 'Create_Inspection':
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
                                viewmode: 'tree',
                                viewid: viewid
                            });
                        });
                    },
                    startingStep: o.ActionOptions.startingStep,
                    menuRefresh: refreshSelected
                };
                Csw.nbt.createInspectionWizard(centerTopDiv, designOpt);

                break;

            case 'Create_Material':
                var createOpt = {
                    //                    viewid: o.ActionOptions.viewid,
                    //                    viewmode: o.ActionOptions.viewmode,
                    onCancel: function () {
                        clear({ 'all': true });
                        Csw.clientState.setCurrent(Csw.clientState.getLast());
                        refreshSelected();
                    },
                    onFinish: function (viewid) {
                        clear({ 'all': true });
                        handleItemSelect({
                            type: 'view',
                            viewmode: 'tree',
                            viewid: viewid
                        });
                    },
                    startingStep: o.ActionOptions.startingStep
                    //                    menuRefresh: refreshSelected
                };
                Csw.nbt.createMaterialWizard(centerTopDiv, createOpt);
                break;

            case 'DispenseContainer':
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
                        capacity: Csw.deserialize(o.capacity),
                        requestItemId: requestItemId,
                        title: title,
                        location: o.location,
                        material: o.material,
                        barcode: o.barcode,
                        containerNodeTypeId: o.containernodetypeid,
                        containerObjectClassId: o.containerobjectclassid
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
                            viewmode: 'tree',
                            viewid: viewid
                        });
                    }
                };
                Csw.nbt.dispenseContainerWizard(centerTopDiv, designOpt);

                break;

            //			case 'Design':                                                                                  
            //				break;                                                                                  
            case 'Edit_View':
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
                            handleItemSelect({ 'viewid': viewid, 'viewmode': viewmode });
                        }
                    },
                    onAddView: function (deletedviewid) {
                        //refreshViewSelect();
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

                $('#CenterTopDiv').CswViewEditor(editViewOptions);

                break;
            //			case 'Enter_Results':                                                                                  
            //				break;                                                                                  

            case 'Future_Scheduling':
                Csw.nbt.futureSchedulingWizard(centerTopDiv, {
                    onCancel: refreshSelected,
                    onFinish: function (viewid, viewmode) {
                        handleItemSelect({ 'viewid': viewid, 'viewmode': viewmode });
                    }
                });
                break;

            //			case 'Import_Fire_Extinguisher_Data':                                                                                  
            //				break;                                                                                  
            //			case 'Inspection_Design':                                                                                  
            //				break;                                                                                  
            case 'Quotas':
                Csw.actions.quotas(centerTopDiv, {
                    onQuotaChange: function () {
                        var quotaHeader = Csw.literals.factory($('#header_quota'));
                        Csw.actions.quotaImage(quotaHeader);
                    }
                });

                break;
            case 'Modules':
                Csw.actions.modules(centerTopDiv, {
                    onModuleChange: function () {
                        refreshDashboard();
                        refreshViewSelect();
                    }
                });
                break;
            case 'Receiving':
                o.onFinish = function (viewid) {
                    clear({ 'all': true });
                    handleItemSelect({
                        type: 'view',
                        viewmode: 'tree',
                        viewid: viewid
                    });
                };
                o.onCancel = function () {
                    clear({ 'all': true });
                    Csw.clientState.setCurrent(Csw.clientState.getLast());
                    refreshSelected();
                };
                Csw.nbt.receiveMaterialWizard(centerTopDiv, o);
                break;
            case 'Sessions':
                Csw.actions.sessions(centerTopDiv);
                break;

            case 'Submit_Request':
                Csw.actions.submitRequest(centerTopDiv, {
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

            case 'Subscriptions':
                Csw.actions.subscriptions(centerTopDiv);
                break;

            case 'View_Scheduled_Rules':
                var rulesOpt = {
                    exitFunc: function () {
                        clear({ 'all': true });
                        Csw.clientState.setCurrent(Csw.clientState.getLast());
                        refreshSelected();
                    },
                    menuRefresh: refreshSelected
                };

                Csw.nbt.scheduledRulesWizard(centerTopDiv, rulesOpt);
                break;
            //			case 'Load_Mobile_Data':                                                                                  
            //				break;                                                                                  
            //			case 'Receiving':                                                                                  
            //				break;                                                                                  
            //			case 'Split_Samples':                                                                                  
            //				break;                                                                                  
            //			case 'View_By_Location':                                                                                  
            //				break;                                                                                  
            default:
                if (false == Csw.isNullOrEmpty(o.actionurl)) {
                    Csw.window.location(o.actionurl);
                }
                break;
        }
    }

    // _handleAction()
};

