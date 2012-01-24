/// <reference path="../globals/Global.js" />
/// <reference path="../globals/CswEnums.js" />
/// <reference path="../globals/CswGlobalTools.js" />
/// <reference path="../globals/CswPrototypeExtensions.js" />
/// <reference path="../../Scripts/jquery-1.7.1-vsdoc.js" />

window.initMain = window.initMain || function (undefined) {

    "use strict";

    var MainTreeId = 'main';
    var MainGridId = 'CswNodeGrid';
    var MainTableId = 'CswNodeTable';
    var MainSearchId = 'CswSearchForm';

    window.onBeforeAjax = function (watchGlobal) {
        if (watchGlobal) {
            $('#ajaxSpacer').hide();
            $('#ajaxImage').show();
        }
    };
    window.onAfterAjax = function (succeeded) {
        if (!ajaxInProgress()) {
            $('#ajaxImage').hide();
            $('#ajaxSpacer').show();
        }
    };

    // handle querystring arguments
    var qs = $.CswQueryString();
    if (false == isNullOrEmpty(qs.viewid)) {
        setCurrentView(qs.viewid, tryParseString(qs.viewmode, 'tree'));
        window.location = "Main.html";
    } else if (false == isNullOrEmpty(qs.reportid)) {
        setCurrentReport(qs.reportid);
        window.location = "Main.html";
    } else if (false == isNullOrEmpty(qs.clear)) {
        clearCurrent();
        window.location = "Main.html";
    } else {
        initAll();
    }


    function initAll() {
        //if (debugOn()) log('Main.initAll()');

        $('#CenterBottomDiv').CswLogin('init', {
            'onAuthenticate': function (u) {
                $('#header_username').text(u)
                     .hover(function () { $(this).CswAttrDom('title', getExpireTime()); });
                $('#header_dashboard').CswDashboard();

                $('#header_quota').CswQuotaImage();

                $('#header_menu').CswMenuHeader({
                    'onLogout': function () {
                        Logout();
                    },
                    'onQuotas': function () {
                        _handleAction({ 'actionname': 'Quotas' });
                    },
                    'onSessions': function () {
                        _handleAction({ 'actionname': 'Sessions' });
                    }
                }); // CswMenuHeader

                refreshViewSelect();

                var current = getCurrent();
                if (false === isNullOrEmpty(current.viewid)) {
                    handleItemSelect({
                        'type': 'view',
                        'viewid': current.viewid,
                        'viewmode': current.viewmode
                    });
                } else if (false === isNullOrEmpty(current.actionname)) {
                    handleItemSelect({
                        'type': 'action',
                        'actionname': current.actionname,
                        'actionurl': current.actionurl
                    });
                } else if (false === isNullOrEmpty(current.reportid)) {
                    handleItemSelect({
                        'type': 'report',
                        'reportid': current.reportid
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
            'onActionClick': function (actionname, actionurl) { handleItemSelect({ 'type': 'action', 'actionname': actionname, 'actionurl': actionurl }); }
        }); // CswQuickLaunch
    }


    function clear(options) {
        //if (debugOn()) log('Main.clear()');

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
            $('#MainMenuDiv').empty();
        }
    }

    function refreshWelcome() {
        //if (debugOn()) log('Main.refreshWelcome()');
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
        //if (debugOn()) log('Main.handleItemSelect()');
        var o = {
            type: 'view', // Action, Report, View
            viewmode: 'tree', // Grid, Tree, List
            linktype: 'link', // WelcomeComponentType: Link, Search, Text, Add
            viewid: '',
            actionname: '',
            actionurl: '',
            reportid: '',
            nodeid: '',
            cswnbtnodekey: ''
        };
        if (options) {
            $.extend(o, options);
        }

        var linkType = tryParseString(o.linktype).toLowerCase();

        var type = tryParseString(o.type).toLowerCase();

        function itemIsSupported() {
            var ret = (linkType === 'search' || false === isNullOrEmpty(o.viewid) ||
                 type === 'action' || type === 'report');
            return ret;
        }

        if (manuallyCheckChanges() && itemIsSupported()) {
            clear({ all: true });

            if (!isNullOrEmpty(o.viewid)) {
                setCurrentView(o.viewid, o.viewmode);

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
                var viewMode = tryParseString(o.viewmode).toLowerCase();
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
            } else if (!isNullOrEmpty(type)) {
                switch (type) {
                    case 'action':
                        _handleAction({
                            'actionname': o.actionname,
                            'actionurl': o.actionurl
                        });
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
        //if (debugOn()) log('Main.refreshMainMenu()');

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
                    case CswViewMode.grid.name:
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
                    case CswViewMode.grid.name:
                        if (contains(o, 'grid') &&
                             false == isNullOrEmpty(o.grid)) {
                            o.grid.print();
                        }
                        break;
                    default:
                        CswError(ChemSW.makeClientSideError(ChemSW.enums.ErrorType.warning.name, 'View Printing is not enabled for views of type ' + o.viewmode));
                        break;
                }
            },
            'onSearch':
                 {
                     'onViewSearch': function () {
                         var genericSearchId = makeId({ 'ID': MainSearchId, prefix: o.prefix, suffix: 'generic' });
                         var viewSearchId = makeId({ 'ID': MainSearchId, prefix: o.prefix, suffix: 'view' });
                         var $searchForm = refreshSearchPanel({
                             'genericSearchId': genericSearchId,
                             'viewSearchId': viewSearchId,
                             'searchType': 'view',
                             'cswnbtnodekey': o.cswnbtnodekey,
                             'viewid': o.viewid
                         });
                     },
                     'onGenericSearch': function () {
                         var genericSearchId = makeId({ 'ID': MainSearchId, prefix: o.prefix, suffix: 'generic' });
                         var viewSearchId = makeId({ 'ID': MainSearchId, prefix: o.prefix, suffix: 'view' });
                         var $searchForm = refreshSearchPanel({
                             'genericSearchId': genericSearchId,
                             'viewSearchId': viewSearchId,
                             'searchType': 'generic',
                             'cswnbtnodekey': o.cswnbtnodekey
                         });
                     }
                 },
            'onEditView': function (viewid) {
                _handleAction({
                    'actionname': 'Edit_View',
                    'ActionOptions': {
                        'viewid': $.CswCookie('get', CswCookieName.CurrentViewId),
                        'viewmode': $.CswCookie('get', CswCookieName.CurrentViewMode)
                    }
                });
            },
            'onSaveView': function (newviewid) {
                handleItemSelect({ 'viewid': newviewid, 'viewmode': $.CswCookie('get', CswCookieName.CurrentViewMode) });
            },
            'Multi': multi,
            'NodeCheckTreeId': MainTreeId
        });
    }

    function refreshSearchPanel(options) {
        //if (debugOn()) log('Main.refreshSearchPanel()');

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

        var $viewSearch = $('#CenterTopDiv').children('#' + o.viewSearchId)
             .empty();
        var $genericSearch = $('#CenterTopDiv').children('#' + o.genericSearchId)
             .empty();
        var $thisSearchForm;

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
            setCurrentView(searchviewid, viewMode);

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
            if (isNullOrEmpty(parentviewid)) {
                viewid = $.CswCookie('get', CswCookieName.CurrentViewId);
            } else {
                viewid = parentviewid;
            }

            if (!isNullOrEmpty(viewid)) {
                clear({ right: true, centerbottom: true }); //wait to clear rest until we have a valid viewid
                var viewmode = 'tree';
                if (isNullOrEmpty(parentviewmode)) {
                    viewmode = $.CswCookie('get', CswCookieName.CurrentViewMode);
                } else {
                    viewmode = (parentviewmode === 'list') ? 'tree' : parentviewmode;
                }

                setCurrentView(viewid, viewmode);

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
        //if (debugOn()) log('Main.getViewGrid()');

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
        var getEmptyGrid = (isTrue(o.showempty));
        if (isNullOrEmpty(o.nodeid)) {
            o.nodeid = $.CswCookie('get', CswCookieName.CurrentNodeId);
        }
        if (isNullOrEmpty(o.cswnbtnodekey)) {
            o.cswnbtnodekey = $.CswCookie('get', CswCookieName.CurrentNodeKey);
        }
        if (!isNullOrEmpty(o.viewid)) {
            $.CswCookie('get', CswCookieName.CurrentViewId);
        }

        //		o.onAddNode = function (nodeid, cswnbtnodekey)
        //			{
        //				var ocopy = o;
        //				var x = { 'nodeid': nodeid, 'cswnbtnodekey': cswnbtnodekey };
        //				$.extend(ocopy, x);
        //				getViewGrid(ocopy);
        //			};
        o.onEditNode = function () { getViewGrid(o); };
        o.onDeleteNode = function () { getViewGrid(o); };

        clear({ centerbottom: true });

        $('#CenterBottomDiv').CswNodeGrid('init', {
            viewid: o.viewid,
            nodeid: o.nodeid,
            cswnbtnodekey: o.cswnbtnodekey,
            showempty: getEmptyGrid,
            ID: MainGridId,
            //'onAddNode': o.onAddNode,
            onEditNode: o.onEditNode,
            onDeleteNode: o.onDeleteNode,
            onSuccess: function (grid) {
                if (o.doMenuRefresh) {
                    refreshMainMenu({
                        viewid: o.viewid,
                        viewmode: CswViewMode.grid.name,
                        grid: grid,
                        nodeid: o.nodeid,
                        cswnbtnodekey: o.cswnbtnodekey
                    });
                }
            },
            onEditView: function (viewid) {
                _handleAction({
                    'actionname': 'Edit_View',
                    'ActionOptions': {
                        viewid: viewid,
                        viewmode: CswViewMode.grid.name,
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
        if (isNullOrEmpty(o.nodeid)) {
            o.nodeid = $.CswCookie('get', CswCookieName.CurrentNodeId);
        }
        if (isNullOrEmpty(o.cswnbtnodekey)) {
            o.cswnbtnodekey = $.CswCookie('get', CswCookieName.CurrentNodeKey);
        }
        if (!isNullOrEmpty(o.viewid)) {
            $.CswCookie('get', CswCookieName.CurrentViewId);
        }

        //		o.onAddNode = function (nodeid, cswnbtnodekey)
        //			{
        //				var ocopy = o;
        //				var x = { 'nodeid': nodeid, 'cswnbtnodekey': cswnbtnodekey };
        //				$.extend(ocopy, x);
        //				getViewTable(ocopy);
        //			};
        o.onEditNode = function () { getViewTable(o); };
        o.onDeleteNode = function () { getViewTable(o); };

        clear({ centerbottom: true });

        $('#CenterBottomDiv').CswNodeTable('init', {
            viewid: o.viewid,
            nodeid: o.nodeid,
            cswnbtnodekey: o.cswnbtnodekey,
            ID: MainTableId,
            //'onAddNode': o.onAddNode,
            onEditNode: o.onEditNode,
            onDeleteNode: o.onDeleteNode,
            onSuccess: function () {
                //				refreshMainMenu({ viewid: o.viewid,
                //					viewmode: CswViewMode.table.name,
                //					nodeid: o.nodeid,
                //					cswnbtnodekey: o.cswnbtnodekey
                //				});
            }
        });
    }

    function onSelectTreeNode(options) {
        //if (debugOn()) log('Main.onSelectTreeNode()');

        if (manuallyCheckChanges()) {
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

            $.CswCookie('set', CswCookieName.CurrentNodeId, o.nodeid);
            $.CswCookie('set', CswCookieName.CurrentNodeKey, o.cswnbtnodekey);

            if (o.nodeid !== '' && o.nodeid !== 'root') {
                getTabs({ 'nodeid': o.nodeid, 'cswnbtnodekey': o.cswnbtnodekey });
                refreshMainMenu({ viewid: o.viewid, viewmode: CswViewMode.tree.name, nodeid: o.nodeid, cswnbtnodekey: o.cswnbtnodekey });
            } else {
                showDefaultContent({ viewid: o.viewid, viewmode: CswViewMode.tree.name });
                refreshMainMenu({ viewid: o.viewid, viewmode: CswViewMode.tree.name, nodeid: '', cswnbtnodekey: '' });
            }
        }
    } // onSelectTreeNode()

    function showDefaultContent(options) {
        var o = {
            viewid: '',
            viewmode: ''
        };
        if (options) $.extend(o, options);

        CswAjaxJson({
            url: 'NbtWebApp/wsNBT.asmx/getDefaultContent',
            data: { ViewId: o.viewid },
            success: function (data) {
                clear({ 'right': true });


                log(data);

            }
        });

        $('#RightDiv').append('This is a test');


    } // setDefaultContent()


    function getTabs(options) {
        //if (debugOn()) log('Main.getTabs()');

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
            onSave: function (nodeid, nodekey) {
                unsetChanged();
                // case 24304
                // refreshSelected({ 'nodeid': nodeid, 'cswnbtnodekey': nodekey });
            },
            tabid: $.CswCookie('get', CswCookieName.CurrentTabId),
            onBeforeTabSelect: function (tabid) {
                return manuallyCheckChanges();
            },
            Refresh: function (nodeid, nodekey) {
                unsetChanged();
                refreshSelected({ 'nodeid': nodeid, 'cswnbtnodekey': nodekey });
            },
            onTabSelect: function (tabid) {
                $.CswCookie('set', CswCookieName.CurrentTabId, tabid);
            },
            onPropertyChange: function (propid, propname) {
                //if(debug) log('Property Changed: propid = ' + propid + ', propname = ' + propname);
                setChanged();
            },
            onEditView: function (viewid) {
                _handleAction({
                    'actionname': 'Edit_View',
                    'ActionOptions': {
                        viewid: viewid,
                        viewmode: CswViewMode.grid.name,
                        startingStep: 2,
                        IgnoreReturn: true
                    }
                });
            },
            ShowCheckboxes: multi,
            NodeCheckTreeId: MainTreeId
        });
    }

    function refreshSelected(options) {
        //if (debugOn()) log('Main.refreshSelected()');

        if (manuallyCheckChanges()) {
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

            if (isNullOrEmpty(o.viewid)) {
                o.viewid = $.CswCookie('get', CswCookieName.CurrentViewId);
            }
            if (isNullOrEmpty(o.viewmode)) {
                o.viewmode = $.CswCookie('get', CswCookieName.CurrentViewMode);
            }

            var viewMode = tryParseString(o.viewmode).toLowerCase();
            switch (viewMode) {
                case 'grid':
                    getViewGrid({
                        'viewid': o.viewid,
                        'nodeid': o.nodeid,
                        'cswnbtnodekey': o.cswnbtnodekey,
                        'showempty': o.showempty,
                        'forsearch': o.forsearch
                    });
                    break;
                case 'table':
                    getViewTable({
                        'viewid': o.viewid,
                        'nodeid': o.nodeid,
                        'cswnbtnodekey': o.cswnbtnodekey
                    });
                    break;
                case 'tree':
                    //default: //tree
                    refreshNodesTree({
                        'nodeid': o.nodeid,
                        'cswnbtnodekey': o.cswnbtnodekey,
                        'nodename': o.nodename,
                        'viewid': o.viewid,
                        'viewmode': o.viewmode,
                        'showempty': o.showempty,
                        'forsearch': o.forsearch,
                        'IncludeNodeRequired': o.IncludeNodeRequired
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
        //if (debugOn()) log('Main.refreshNodesTree()');

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

        var getEmptyTree = (isTrue(o.showempty));
        if (isNullOrEmpty(o.nodeid)) {
            o.nodeid = $.CswCookie('get', CswCookieName.CurrentNodeId);
        }
        if (isNullOrEmpty(o.cswnbtnodekey)) {
            o.cswnbtnodekey = $.CswCookie('get', CswCookieName.CurrentNodeKey);
        }
        if (isNullOrEmpty(o.viewid)) {
            o.viewid = $.CswCookie('get', CswCookieName.CurrentViewId);
        }

        clear({ left: true });

        $('#LeftDiv').CswNodeTree('init', {
            'ID': MainTreeId,
            'viewid': o.viewid,
            'viewmode': o.viewmode,
            'nodeid': o.nodeid,
            'cswnbtnodekey': o.cswnbtnodekey,
            'nodename': o.nodename,
            'showempty': getEmptyTree,
            'forsearch': o.forsearch,
            'IncludeNodeRequired': o.IncludeNodeRequired,
            'onViewChange': function (newviewid, newviewmode) {
                setCurrentView(newviewid, newviewmode);
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


    function _handleAction(options) {
        var o = {
            'actionname': '',
            'actionurl': '',
            'ActionOptions': {}
        };
        if (options) {
            $.extend(o, options);
        }

        setCurrentAction(o.actionname, o.actionurl);

        CswAjaxJson({
            'url': '/NbtWebApp/wsNBT.asmx/SaveActionToQuickLaunch',
            'data': { 'ActionName': o.actionname }
        });

        function _setupOOCInspections() {
            clear({ 'all': true });

            $('#CenterTopDiv').CswInspectionStatus({
                'onEditNode': function () {
                    _setupOOCInspections();
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
                        setCurrent(getLast());
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
                        handleItemSelect({
                            type: 'view',
                            viewmode: 'tree',
                            viewid: viewid
                        });
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

                var EditViewOptions = {
                    'viewid': o.ActionOptions.viewid,
                    'viewmode': o.ActionOptions.viewmode,
                    'onCancel': function () {
                        clear({ 'all': true });
                        setCurrent(getLast());
                        refreshSelected();
                    },
                    'onFinish': function (viewid, viewmode) {
                        clear({ 'all': true });
                        if (isTrue(o.ActionOptions.IgnoreReturn)) {
                            setCurrent(getLast());
                            refreshSelected();
                        } else {
                            handleItemSelect({ 'viewid': viewid, 'viewmode': viewmode });
                        }
                    },
                    'startingStep': o.ActionOptions.startingStep
                };

                $('#CenterTopDiv').CswViewEditor(EditViewOptions);

                break;
            //			case 'Enter_Results':             
            //				break;             
            //			case 'Future_Scheduling':             
            //				break;             
            //			case 'Import_Fire_Extinguisher_Data':             
            //				break;             
            //			case 'Inspection_Design':             
            //				break;             
            case 'OOC_Inspections':
                _setupOOCInspections();

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
                        setCurrent(getLast());
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
                if (!isNullOrEmpty(o.actionurl)) {
                    window.location = o.actionurl;
                }
                break;
        }
    }

    // _handleAction()
};
