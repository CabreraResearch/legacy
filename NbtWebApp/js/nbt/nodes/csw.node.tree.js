/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function csw_nbt_nodeTree() {
    "use strict";

    var nodeTree = function _nodeTree(opts) {

        var cswPrivateVar = {
            ID: '',
            parent: null,
            //showempty: false, // if true, shows an empty tree (primarily for search)
            forsearch: false, // if true, used to override default behavior of list views
            UseScrollbars: true,
            onSelectNode: null, // function (optSelect) { var o =  { nodeid: '',  nodename: '', iconurl: '', cswnbtnodekey: '', viewid: '' }; return o; },
            onInitialSelectNode: null,
            ShowCheckboxes: false,
            ValidateCheckboxes: true,
            ShowToggleLink: true,
            height: '',
            width: '',

            RunTreeUrl: '/NbtWebApp/wsNBT.asmx/runTree',
            fetchTreeFirstLevelUrl: '/NbtWebApp/wsNBT.asmx/fetchTreeFirstLevel',
            fetchTreeLevelUrl: '/NbtWebApp/wsNBT.asmx/fetchTreeLevel',
            NodeTreeUrl: '/NbtWebApp/wsNBT.asmx/getTreeOfNode',
            rootnode: null,
            toggleLink: null
        };
        if (opts) $.extend(cswPrivateVar, opts);

        var cswPublicRet = {
            treeDiv: null
        };


        cswPrivateVar.make = function (data, viewid, viewmode, url) {

            var treeThemes = { "dots": true };
            if (viewmode === Csw.enums.viewMode.list.name) {
                treeThemes = { "dots": false };
            }

            cswPublicRet.treeDiv.$.jstree({
                "json_data": {
                    "data": data.root
                },
                "core": {
                    "open_parents": true
                },
                "ui": {
                    "select_limit": 1,
                    "selected_parent_close": false,
                    "selected_parent_open": true
                },
                "themes": treeThemes,
                "types": {
                    "types": data.types,
                    "max_children": -2,
                    "max_depth": -2
                },
                "plugins": ["themes", "ui", "types", "crrm", "json_data"]
            }); // jstree()

            function selectNode(e, newData) {
                return cswPrivateVar.firstSelectNode({
                    e: e,
                    data: newData,
                    url: url,
                    viewid: viewid
                });
            }
            cswPublicRet.treeDiv.bind('select_node.jstree', selectNode);

            function hoverNode(e, bindData) {
                var $hoverLI = $(bindData.rslt.obj[0]);
                //var nodeid = $hoverLI.CswAttrDom('id').substring(cswPrivateVar.idPrefix.length);
                var nodeid = $hoverLI.CswAttrNonDom('nodeid');
                var cswnbtnodekey = $hoverLI.CswAttrNonDom('cswnbtnodekey');
                Csw.nodeHoverIn(bindData.args[1], nodeid, cswnbtnodekey);
            }
            cswPublicRet.treeDiv.bind('hover_node.jstree', hoverNode);

            function deHoverNode() {
                Csw.jsTreeGetSelected(cswPublicRet.treeDiv.$);
                Csw.nodeHoverOut();
            }
            cswPublicRet.treeDiv.bind('dehover_node.jstree', deHoverNode);

            cswPublicRet.treeDiv.$.jstree('select_node', Csw.tryParseElement(data.selectid));

            cswPrivateVar.rootnode = cswPublicRet.treeDiv.find('li').first();

            cswPublicRet.treeDiv.find('li').$.each(function () {
                var $childObj = $(this);
                var thisid = Csw.string($childObj.CswAttrDom('id'));
                var thiskey = Csw.string($childObj.CswAttrDom('cswnbtnodekey'));
                var thisnodeid = Csw.string($childObj.CswAttrNonDom('nodeid'), thisid.substring(cswPrivateVar.idPrefix.length));
                var thisrel = Csw.string($childObj.CswAttrNonDom('rel'));
                var altName = Csw.string($childObj.find('a').first().text());
                var thisnodename = Csw.string($childObj.CswAttrNonDom('nodename'), altName).trim();
                var thislocked = Csw.bool($childObj.CswAttrNonDom('locked'));

                if (Csw.bool(cswPrivateVar.ShowCheckboxes)) {
                    var $cb = $('<input type="checkbox" class="' + cswPrivateVar.idPrefix + 'check" id="check_' + thisid + '" rel="' + thisrel + '" nodeid="' + thisnodeid + '" nodename="' + thisnodename + '" cswnbtnodekey="' + thiskey + '"></input>');
                    $cb.prependTo($childObj);
                    if (cswPrivateVar.ValidateCheckboxes) {
                        $cb.click(function () { return cswPrivateVar.validateCheck($cb); });
                    }
                } // if (Csw.bool(cswPrivateVar.ShowCheckboxes)) {

                if (thislocked) {
                    $('<img src="Images/quota/lock.gif" title="Quota exceeded" />')
                        .appendTo($childObj.children('a').first());
                }

            }); // each()

            if (cswPrivateVar.ShowToggleLink && cswPrivateVar.toggleLink) {
                cswPrivateVar.toggleLink.show();
            }
            // DO NOT define an onSuccess() function here that interacts with the tree.
            // The tree has initalization events that appear to happen asynchronously,
            // and thus having an onSuccess() function that changes the selected node will
            // cause a race condition.

        }; // cswPrivateVar.make()

        cswPrivateVar.firstSelectNode = function (myoptions) {
            var m = {
                e: '',
                data: '',
                url: '',
                onSelectNode: cswPrivateVar.onSelectNode,
                viewid: '',
                forsearch: cswPrivateVar.forsearch
            };
            if (myoptions) $.extend(m, myoptions);

            // case 21715 - don't trigger onSelectNode event on first event
            var m2 = {};
            $.extend(m2, m);
            m2.onSelectNode = cswPrivateVar.onInitialSelectNode;
            cswPrivateVar.handleSelectNode(m2);

            // rebind event for next select
            cswPublicRet.treeDiv.unbind('select_node.jstree');
            cswPublicRet.treeDiv.bind('select_node.jstree', function (e, data) {
                return cswPrivateVar.handleSelectNode({
                    e: e,
                    data: data,
                    url: m.url,
                    viewid: m.viewid
                });
            });
        }; // cswPrivateVar.firstSelectNode()

        cswPrivateVar.handleSelectNode = function (myoptions) {
            var m = {
                e: '',
                data: '',
                url: '',
                onSelectNode: cswPrivateVar.onSelectNode,
                viewid: '',
                forsearch: cswPrivateVar.forsearch
            };
            if (myoptions) $.extend(m, myoptions);

            var selected = Csw.jsTreeGetSelected(cswPublicRet.treeDiv.$);
            var optSelect = {
                nodeid: selected.$item.CswAttrNonDom('nodeid'),
                nodename: selected.text,
                iconurl: selected.iconurl,
                cswnbtnodekey: selected.$item.CswAttrNonDom('cswnbtnodekey'),
                nodespecies: selected.$item.CswAttrNonDom('species'),
                viewid: m.viewid
            };

            cswPrivateVar.clearChecks();

            // case 25844 - open children
            cswPublicRet.treeDiv.$.jstree('open_node', selected.$item);

            Csw.tryExec(m.onSelectNode, optSelect);
        }; // cswPrivateVar.handleSelectNode()

        cswPrivateVar.validateCheck = function ($checkbox) {
            var $selected = Csw.jsTreeGetSelected(cswPublicRet.treeDiv.$);
            return ($selected.$item.CswAttrNonDom('rel') === $checkbox.CswAttrNonDom('rel'));
        };

        cswPrivateVar.clearChecks = function () {
            $('.' + cswPrivateVar.idPrefix + 'check').CswAttrDom('checked', '');
        };


        // Typical mechanism for fetching tree data and making a tree
        cswPublicRet.init = function (options) {
            var o = {
                viewid: '',       // loads an arbitrary view
                viewmode: '',
                nodeid: '',       // if viewid are not supplied, loads a view of this node
                cswnbtnodekey: '',
                IncludeNodeRequired: false,
                IncludeInQuickLaunch: true,
                onViewChange: null, // function (newviewid, newviewmode) {},    // if the server returns a different view than what we asked for (e.g. case 21262)
                DefaultSelect: Csw.enums.nodeTree_DefaultSelect.firstchild.name
            };
            if (options) $.extend(o, options);

            var url = cswPrivateVar.RunTreeUrl;
            var dataParam = {
                ViewId: o.viewid,
                IdPrefix: cswPrivateVar.idPrefix,
                IncludeNodeRequired: o.IncludeNodeRequired,
                IncludeNodeKey: Csw.string(o.cswnbtnodekey),
                IncludeNodeId: Csw.string(o.nodeid),
                NodePk: Csw.string(o.nodeid),
                IncludeInQuickLaunch: o.IncludeInQuickLaunch,
                DefaultSelect: o.DefaultSelect
            };

            if (Csw.isNullOrEmpty(o.viewid)) {
                url = cswPrivateVar.NodeTreeUrl;
            }

            Csw.ajax.post({
                url: url,
                data: dataParam,
                stringify: false,
                success: function (data) {

                    if (Csw.isNullOrEmpty(data)) {
                        Csw.error.showError(Csw.enums.errorType.error.name,
                                            'The requested view cannot be rendered as a Tree.',
                                            'View with ViewId: ' + o.viewid + ' does not exist or is not a Tree view.');
                    } else {

                        var newviewid = data.newviewid;
                        var newviewmode = data.newviewmode;
                        if (false === Csw.isNullOrEmpty(newviewid) && o.viewid !== newviewid) {
                            o.viewid = newviewid;
                            o.viewmode = newviewmode;
                            Csw.tryExec(o.onViewChange, newviewid, newviewmode);
                        }

                        cswPrivateVar.make(data, o.viewid, o.viewmode, url);
                    }

                } // success
            }); // ajax

        }; // cswPublicRet.init()

        // For making a tree without using the regular mechanism for fetching tree data
        cswPublicRet.makeTree = function (treeData) {
            cswPrivateVar.make(treeData, '', 'tree', '');
        };


        cswPublicRet.selectNode = function (optSelect) {
            var o = {
                newnodeid: '',
                newcswnbtnodekey: ''
            };
            if (optSelect) {
                $.extend(o, optSelect);
            }
            cswPublicRet.treeDiv.$.jstree('select_node', '#' + cswPrivateVar.idPrefix + o.newcswnbtnodekey);
        };


        cswPublicRet.expandAll = function () {
            if (cswPublicRet.treeDiv && cswPrivateVar.rootnode) {
                cswPublicRet.treeDiv.$.jstree('open_all', cswPrivateVar.rootnode.$);
            }

            if (cswPrivateVar.toggleLink) {
                cswPrivateVar.toggleLink.text('Collapse All')
                    .unbind('click')
                    .click(function () {
                        cswPublicRet.collapseAll();
                        return false;
                    });
            }
        };

        cswPublicRet.collapseAll = function () {
            cswPublicRet.treeDiv.$.jstree('close_all', cswPrivateVar.rootnode.$);

            // show first level
            cswPublicRet.treeDiv.$.jstree('open_node', cswPrivateVar.rootnode.$);

            cswPrivateVar.toggleLink.text('Expand All')
                .unbind('click')
                .click(function () {
                    cswPublicRet.expandAll();
                    return false;
                });
        };

        cswPublicRet.checkedNodes = function () {
            var $nodechecks = $('.' + cswPrivateVar.idPrefix + 'check:checked');
            var ret = [];

            if (false === Csw.isNullOrEmpty($nodechecks, true)) {
                var n = 0;
                $nodechecks.each(function () {
                    var $nodecheck = $(this);
                    ret[n] = {
                        nodeid: $nodecheck.CswAttrNonDom('nodeid'),
                        nodename: $nodecheck.CswAttrNonDom('nodename'),
                        cswnbtnodekey: $nodecheck.CswAttrNonDom('cswnbtnodekey')
                    };
                    n++;
                });
            }
            return ret;
        };


        (function constructor() {

            if (false === Csw.isFunction(cswPrivateVar.onInitialSelectNode)) {
                cswPrivateVar.onInitialSelectNode = cswPrivateVar.onSelectNode;
            }
            cswPrivateVar.idPrefix = Csw.string(cswPrivateVar.ID) + '_';

            if (cswPrivateVar.ShowToggleLink) {
                cswPrivateVar.toggleLink = cswPrivateVar.parent.a({
                    ID: cswPrivateVar.idPrefix + 'toggle',
                    value: 'Expand All',
                    onClick: cswPublicRet.expandAll
                });
                cswPrivateVar.toggleLink.hide();
            }

            cswPublicRet.treeDiv = cswPrivateVar.parent.div({ ID: cswPrivateVar.IdPrefix });

            if (cswPrivateVar.UseScrollbars) {
                cswPublicRet.treeDiv.addClass('treediv');
            } else {
                cswPublicRet.treeDiv.addClass('treediv_noscroll');
            }
            if (false === Csw.isNullOrEmpty(cswPrivateVar.height)) {
                cswPublicRet.treeDiv.css({ height: cswPrivateVar.height });
            }
            if (false === Csw.isNullOrEmpty(cswPrivateVar.width)) {
                cswPublicRet.treeDiv.css({ width: cswPrivateVar.width });
            }

        })(); // constructor

        return cswPublicRet;
    };

    Csw.nbt.register('nodeTree', nodeTree);
    Csw.nbt.nodeTree = Csw.nbt.nodeTree || nodeTree;

})();