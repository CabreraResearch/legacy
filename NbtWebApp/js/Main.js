/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="../globals/CswEnums.js" />

window.initMain = window.initMain || function (undefined) {

    "use strict";

    var mainTreeId = 'main';
    var mainGridId = 'CswNodeGrid';
    var mainTableId = 'CswNodeTable';
    var mainSearchId = 'CswSearchForm';

    var universalsearch;

    function startSpinner() {
        $('#ajaxSpacer').hide();
        $('#ajaxImage').show();
    }

    Csw.subscribe(Csw.enums.events.ajax.globalAjaxStart, startSpinner);

    function stopSpinner() {
        $('#ajaxImage').hide();
        $('#ajaxSpacer').show();
    };

    Csw.subscribe(Csw.enums.events.ajax.globalAjaxStop, stopSpinner);

    // handle querystring arguments
    var qs = $.CswQueryString();
    if (false == Csw.isNullOrEmpty(qs.viewid)) {
        var setView = function () {
            Csw.clientState.setCurrentView(qs.viewid, Csw.string(qs.viewmode));
            window.location = "Main.html";
        };
        if (Csw.isNullOrEmpty(qs.viewmode)) {
            Csw.ajax.post({
                url: Csw.enums.ajaxUrlPrefix + 'getViewMode',
                data: { ViewId: qs.viewid },
                success: function (data) {
                    qs.viewmode = Csw.string(data.viewmode, 'tree');
                    setView();
                }
            });
        } else {
            setView();
        }
    } else if (false == Csw.isNullOrEmpty(qs.reportid)) {
        Csw.clientState.setCurrentReport(qs.reportid);
        window.location = "Main.html";
    } else if (false == Csw.isNullOrEmpty(qs.clear)) {
        Csw.clientState.clearCurrent();
        window.location = "Main.html";
    } else {
        initAll();
    }

    function initAll() {
        //if (debugOn()) Csw.log('Main.initAll()');
        $('#CenterBottomDiv').CswLogin('init', {
            'onAuthenticate': function (u) {
                $('#header_username').text(u)
                     .hover(function () { $(this).CswAttrDom('title', Csw.clientSession.getExpireTime()); });
                $('#header_dashboard').CswDashboard();

                universalsearch = Csw.controls.universalSearch({
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
                    onLoadView: function (viewid, viewmode) {
                        handleItemSelect({
                            'type': 'view',
                            'viewid': viewid,
                            'viewmode': viewmode
                        });
                    }
                });

                $('#header_quota').CswQuotaImage();

                $('#header_menu').CswMenuHeader({
                    'onLogout': function () {
                        Csw.clientSession.logout();
                    },
                    'onQuotas': function () {
                        handleAction({ 'actionname': 'Quotas' });
                    },
                    'onSessions': function () {
                        handleAction({ 'actionname': 'Sessions' });
                    }
                }); // CswMenuHeader

                refreshViewSelect();

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
            } // onAuthenticate
        }); // CswLogin

    }

    // initAll()

    function refreshViewSelect() {
        $('#ViewSelectDiv').CswViewSelect({
            'ID': 'mainviewselect',
            'onSelect': handleItemSelect
        }); // CswViewSelect
    }

    function refreshQuickLaunch() {
        $('#QuickLaunch').empty();

        $('#QuickLaunch').CswQuickLaunch({
            'onViewClick': function (viewid, viewmode) { handleItemSelect({ 'viewid': viewid, 'viewmode': viewmode }); },
            'onActionClick': function (actionname, actionurl) { handleItemSelect({ 'type': 'action', 'actionname': actionname, 'actionurl': actionurl }); },
            'onSearchClick': function (searchid) { handleItemSelect({ 'type': 'search', 'searchid': searchid }); }
        }); // CswQuickLaunch
    }


    function clear(options) {
        //if (debugOn()) Csw.log('Main.clear()');

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
        //if (debugOn()) Csw.log('Main.refreshWelcome()');
        clear({ all: true });

        $('#CenterBottomDiv').CswWelcome('initTable', {
            'onLinkClick': handleItemSelect,
            'onSearchClick': function (view) {
                var viewid = view.viewid;
                var viewmode = view.viewmode;
                handleItemSelect({ 'viewid': viewid, 'viewmode': viewmode, 'linktype': 'search' });
                refreshSearchPanel({ 'viewid': viewid, 'searchType': 'view' });
            },
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
        //if (debugOn()) Csw.log('Main.handleItemSelect()');
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
            clear({ all: true });

            if (false === Csw.isNullOrEmpty(o.viewid)) {
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
                        handleAction({
                            'actionname': o.actionname,
                            'actionurl': o.actionurl
                        });
                        break;
                    case 'search':
                        universalsearch.restoreSearch(o.searchid);
                        break;
                    case 'report':
                        window.location = "Report.aspx?reportid=" + o.reportid;
                        break;
                }
            }

            refreshQuickLaunch();
        } else { // if (manuallyCheckChanges() && itemIsSupported())
            //do nothing
        }
    }

    // handleItemSelect()

    function refreshMainMenu(options) {
        //if (debugOn()) Csw.log('Main.refreshMainMenu()');

        var o = {
            viewid: '',
            viewmode: '',
            nodeid: '',
            cswnbtnodekey: '',
            prefix: 'csw'
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
                        multi = (false === o.grid.isMulti());
                        var g = {
                            gridOpts: {
                                //reinit: true,
                                multiselect: multi
                            }
                        };
                        o.grid.changeGridOpts(g);
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
                        if (Csw.contains(o, 'grid') &&
                             false == Csw.isNullOrEmpty(o.grid)) {
                            o.grid.print();
                        }
                        break;
                    default:
                        Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, 'View Printing is not enabled for views of type ' + o.viewmode));
                        break;
                }
            },
            'onSearch':
                 {
                     'onViewSearch': function () {
                         var genericSearchId = Csw.controls.dom.makeId({ 'ID': mainSearchId, prefix: o.prefix, suffix: 'generic' });
                         var viewSearchId = Csw.controls.dom.makeId({ 'ID': mainSearchId, prefix: o.prefix, suffix: 'view' });
                         refreshSearchPanel({
                             'genericSearchId': genericSearchId,
                             'viewSearchId': viewSearchId,
                             'searchType': 'view',
                             'cswnbtnodekey': o.cswnbtnodekey,
                             'viewid': o.viewid
                         });
                     },
                     'onGenericSearch': function () {
                         var genericSearchId = Csw.controls.dom.makeId({ 'ID': mainSearchId, prefix: o.prefix, suffix: 'generic' });
                         var viewSearchId = Csw.controls.dom.makeId({ 'ID': mainSearchId, prefix: o.prefix, suffix: 'view' });
                         refreshSearchPanel({
                             'genericSearchId': genericSearchId,
                             'viewSearchId': viewSearchId,
                             'searchType': 'generic',
                             'cswnbtnodekey': o.cswnbtnodekey
                         });
                     }
                 },
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
            'NodeCheckTreeId': mainTreeId
        });
    }

    function refreshSearchPanel(options) {
        //if (debugOn()) Csw.log('Main.refreshSearchPanel()');

        var o = {
            viewid: '',
            nodetypeid: '',
            cswnbtnodekey: '',
            viewSearchId: '',
            genericSearchId: '',
            searchType: 'generic'
        };

        if (options) {
            $.extend(o, options);
        }

        $('#CenterTopDiv').children('#' + o.viewSearchId)
             .empty();
        $('#CenterTopDiv').children('#' + o.genericSearchId)
             .empty();
        var $thisSearchForm = '';

        switch (o.searchType.toLowerCase()) {
            case 'generic':
                {
                    $thisSearchForm = makeSearchForm({ 'cswnbtnodekey': o.cswnbtnodekey, 'ID': o.genericSearchId });
                    break;
                }
            case 'view':
                {
                    $thisSearchForm = makeSearchForm({ 'viewid': o.viewid, 'cswnbtnodekey': o.cswnbtnodekey, 'ID': o.viewSearchId });
                    break;
                }
        }
        return $thisSearchForm;
    }

    function makeSearchForm(options) {
        var o = {
            viewid: '',
            nodetypeid: '',
            cswnbtnodekey: '',
            ID: ''
        };
        if (options) {
            $.extend(o, options);
        }

        clear({ centertop: true });

        var onSearchSubmit = function (searchviewid, searchviewmode) {
            clear({ right: true, centerbottom: true });
            var viewMode = searchviewmode;
            if (viewMode === 'list') {
                viewMode = 'tree';
            }
            Csw.clientState.setCurrentView(searchviewid, viewMode);

            refreshSelected({
                viewmode: viewMode,
                viewid: searchviewid,
                forsearch: true,
                showempty: false,
                cswnbtnodekey: '', //do not want a view of only this node
                nodeid: ''
            });
        };
        var onClearSubmit = function (parentviewid, parentviewmode) {
            clear({ centertop: true }); //clear Search first

            var viewid;
            if (Csw.isNullOrEmpty(parentviewid)) {
                viewid = Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId);
            } else {
                viewid = parentviewid;
            }

            if (false === Csw.isNullOrEmpty(viewid)) {
                clear({ right: true, centerbottom: true }); //wait to clear rest until we have a valid viewid
                var viewmode;
                if (Csw.isNullOrEmpty(parentviewmode)) {
                    viewmode = Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewMode);
                } else {
                    viewmode = (parentviewmode === 'list') ? 'tree' : parentviewmode;
                }

                Csw.clientState.setCurrentView(viewid, viewmode);

                refreshSelected({
                    viewmode: viewmode,
                    viewid: viewid,
                    forsearch: true,
                    showempty: false,
                    cswnbtnodekey: '', //do not want a view of only this node
                    nodeid: ''
                });
            }
        };
        var onSearchClose = function () {
            clear({ centertop: true });
        };
        var $search = $('#CenterTopDiv').CswSearch({
            'parentviewid': o.viewid,
            'cswnbtnodekey': o.cswnbtnodekey,
            'nodetypeid': o.nodetypeid,
            'ID': o.ID,
            'onSearchSubmit': onSearchSubmit,
            'onClearSubmit': onClearSubmit,
            'onSearchClose': onSearchClose
        });
        return $search;
    }

    function getViewGrid(options) {
        //if (debugOn()) Csw.log('Main.getViewGrid()');

        var o = {
            viewid: '',
            nodeid: '',
            showempty: false,
            cswnbtnodekey: '',
            doMenuRefresh: true,
            onAddNode: '',
            onEditNode: '',
            onDeleteNode: ''
        };
        if (options) {
            $.extend(o, options);
        }

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

        clear({ centerbottom: true });

        $('#CenterBottomDiv').CswNodeGrid('init', {
            viewid: o.viewid,
            nodeid: o.nodeid,
            cswnbtnodekey: o.cswnbtnodekey,
            showempty: getEmptyGrid,
            ID: mainGridId,
            //'onAddNode': o.onAddNode,
            onEditNode: o.onEditNode,
            onDeleteNode: o.onDeleteNode,
            onSuccess: function (grid) {
                if (o.doMenuRefresh) {
                    refreshMainMenu({
                        viewid: o.viewid,
                        viewmode: Csw.enums.viewMode.grid.name,
                        grid: grid,
                        nodeid: o.nodeid,
                        cswnbtnodekey: o.cswnbtnodekey
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

        clear({ centerbottom: true });

        $('#CenterBottomDiv').CswNodeTable('init', {
            viewid: o.viewid,
            nodeid: o.nodeid,
            cswnbtnodekey: o.cswnbtnodekey,
            ID: mainTableId,
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
        //if (debugOn()) Csw.log('Main.onSelectTreeNode()');
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
        var $div = $('#CenterBottomDiv').CswDiv({ ID: 'deftbldiv' });
        $div.CswAttrDom('align', 'center');
        $div.css({ textAlign: 'center' });
        $div.append('No Results.<BR/><BR/>');
        $div.CswDefaultContent(v);

    } // showDefaultContentTable()

    function getTabs(options) {
        //if (debugOn()) Csw.log('Main.getTabs()');

        var o = {
            nodeid: '',
            cswnbtnodekey: ''
        };
        if (options) $.extend(o, options);

        clear({ right: true });
        $('#RightDiv').CswNodeTabs({
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
            Refresh: function (nodeid, nodekey) {
                Csw.clientChanges.unsetChanged();
                refreshSelected({ 'nodeid': nodeid, 'cswnbtnodekey': nodekey });
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
            NodeCheckTreeId: mainTreeId
        });
    }

    function refreshSelected(options) {
        //if (debugOn()) Csw.log('Main.refreshSelected()');

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
            if (options) {
                $.extend(o, options);
            }

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
        //if (debugOn()) Csw.log('Main.refreshNodesTree()');

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
        }
        if (Csw.isNullOrEmpty(o.cswnbtnodekey)) {
            o.cswnbtnodekey = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeKey);
        }
        if (Csw.isNullOrEmpty(o.viewid)) {
            o.viewid = Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId);
        }

        clear({ left: true });

        $('#LeftDiv').CswNodeTree('init', {
            'ID': mainTreeId,
            'viewid': o.viewid,
            'viewmode': o.viewmode,
            'nodeid': o.nodeid,
            'cswnbtnodekey': o.cswnbtnodekey,
            'nodename': o.nodename,
            'showempty': getEmptyTree,
            'forsearch': o.forsearch,
            'IncludeNodeRequired': o.IncludeNodeRequired,
            'onViewChange': function (newviewid, newviewmode) {
                Csw.clientState.setCurrentView(newviewid, newviewmode);
            },
            'onSelectNode': function (optSelect) {
                onSelectTreeNode({
                    'viewid': optSelect.viewid,
                    'nodeid': optSelect.nodeid,
                    'cswnbtnodekey': optSelect.cswnbtnodekey
                });
            },
            'ShowCheckboxes': multi
        }); // CswNodesTree
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

        Csw.clientState.setCurrentAction(o.actionname, o.actionurl);

        Csw.ajax.post({
            'url': '/NbtWebApp/wsNBT.asmx/SaveActionToQuickLaunch',
            'data': { 'ActionName': o.actionname }
        });

        function setupOocInspections() {
            clear({ 'all': true });

            $('#CenterTopDiv').CswInspectionStatus({
                'onEditNode': function () {
                    setupOocInspections();
                }
            });
        }

        switch (o.actionname) {
            //			case 'Assign_Inspection':                                              
            //				break;                                              
            //			case 'Assign_Tests':                                              
            //				break;                                              
            // NOTE: Create Inspection currently only works if you are logged in as chemsw_admin                                              
            case 'Create_Inspection':
                clear({ 'all': true });

                var designOpt = {
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
                        refreshViewSelect();
                        refreshSelected({
                            type: 'view',
                            viewmode: 'tree',
                            viewid: viewid
                        });
                        //                        handleItemSelect({
                        //                            type: 'view',
                        //                            viewmode: 'tree',
                        //                            viewid: viewid
                        //                        });
                    },
                    startingStep: o.ActionOptions.startingStep,
                    menuRefresh: refreshSelected
                };

                $('#CenterTopDiv').CswInspectionDesign(designOpt);

                break;
            //			case 'Design':                                              
            //				break;                                              
            case 'Edit_View':
                clear({ 'all': true });

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
                        if (Csw.bool(o.ActionOptions.IgnoreReturn)) {
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            refreshSelected();
                        } else {
                            handleItemSelect({ 'viewid': viewid, 'viewmode': viewmode });
                        }
                    },
                    'startingStep': o.ActionOptions.startingStep
                };

                $('#CenterTopDiv').CswViewEditor(editViewOptions);

                break;
            //			case 'Enter_Results':                                              
            //				break;                                              

            case 'Future_Scheduling':                                      
                clear({ 'all': true });
                Csw.actions.futureScheduling({
                    $parent: $('#CenterTopDiv'),
                    onCancel: function() { 
                        refreshSelected();
                    },
                    onFinish: function(viewid, viewmode) {
                        handleItemSelect({ 'viewid': viewid, 'viewmode': viewmode });
                    }
                });
            	break;                                      

            //			case 'Import_Fire_Extinguisher_Data':                                              
            //				break;                                              
            //			case 'Inspection_Design':                                              
            //				break;                                              

            case 'OOC_Inspections':
                setupOocInspections();

                break;
            case 'Quotas':
                clear({ 'all': true });
                $('#CenterTopDiv').CswQuotas({
                    onQuotaChange: function () {
                        $('#header_quota').CswQuotaImage();
                    }
                });

                break;
            case 'Sessions':
                clear({ 'all': true });
                $('#CenterTopDiv').CswSessions();

                break;
            case 'View_Scheduled_Rules':
                clear({ 'all': true });

                var rulesOpt = {
                    exitFunc: function () {
                        clear({ 'all': true });
                        Csw.clientState.setCurrent(Csw.clientState.getLast());
                        refreshSelected();
                    },
                    menuRefresh: refreshSelected
                };

                $('#CenterTopDiv').CswScheduledRulesGrid(rulesOpt);

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
                    window.location = o.actionurl;
                }
                break;
        }
    }

    // _handleAction()
};
