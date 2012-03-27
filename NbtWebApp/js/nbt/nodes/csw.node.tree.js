/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function csw_nbt_nodeTree() {
    "use strict";

    var nodeTree = function _nodeTree(opts) {

        var internal = {
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
        if (opts) $.extend(internal, opts);

        var external = {
            treeDiv: null
        };


        internal.make = function (data, viewid, viewmode, url) {

            var treeThemes = { "dots": true };
            if (viewmode === Csw.enums.viewMode.list.name) {
                treeThemes = { "dots": false };
            }

            external.treeDiv.$.jstree({
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
                return internal.firstSelectNode({
                    e: e,
                    data: newData,
                    url: url,
                    viewid: viewid
                });
            }
            external.treeDiv.bind('select_node.jstree', selectNode);

            function hoverNode(e, bindData) {
                var $hoverLI = $(bindData.rslt.obj[0]);
                //var nodeid = $hoverLI.CswAttrDom('id').substring(internal.idPrefix.length);
                var nodeid = $hoverLI.CswAttrNonDom('nodeid');
                var cswnbtnodekey = $hoverLI.CswAttrNonDom('cswnbtnodekey');
                Csw.nodeHoverIn(bindData.args[1], nodeid, cswnbtnodekey);
            }
            external.treeDiv.bind('hover_node.jstree', hoverNode);

            function deHoverNode() {
                Csw.jsTreeGetSelected(external.treeDiv.$);
                Csw.nodeHoverOut();
            }
            external.treeDiv.bind('dehover_node.jstree', deHoverNode);

            external.treeDiv.$.jstree('select_node', Csw.tryParseElement(data.selectid));

            internal.rootnode = external.treeDiv.find('li').first();

            if (Csw.bool(internal.ShowCheckboxes)) {

                external.treeDiv.find('li').$.each(function () {
                    var $childObj = $(this);
                    var thisid = Csw.string($childObj.CswAttrDom('id'));
                    var thiskey = Csw.string($childObj.CswAttrDom('cswnbtnodekey'));
                    var thisnodeid = Csw.string($childObj.CswAttrNonDom('nodeid'), thisid.substring(internal.idPrefix.length));
                    var thisrel = Csw.string($childObj.CswAttrNonDom('rel'));
                    var altName = Csw.string($childObj.find('a').first().text());
                    var thisnodename = Csw.string($childObj.CswAttrNonDom('nodename'), altName).trim();

                    var $cb = $('<input type="checkbox" class="' + internal.idPrefix + 'check" id="check_' + thisid + '" rel="' + thisrel + '" nodeid="' + thisnodeid + '" nodename="' + thisnodename + '" cswnbtnodekey="' + thiskey + '"></input>');
                    $cb.prependTo($childObj);
                    if (internal.ValidateCheckboxes) {
                        $cb.click(function () { return internal.validateCheck($cb); });
                    }
                }); // each()

            } // if (Csw.bool(internal.ShowCheckboxes)) {

            if (internal.ShowToggleLink && internal.toggleLink) {
                internal.toggleLink.show();
            }
            // DO NOT define an onSuccess() function here that interacts with the tree.
            // The tree has initalization events that appear to happen asynchronously,
            // and thus having an onSuccess() function that changes the selected node will
            // cause a race condition.

        }; // internal.make()

        internal.firstSelectNode = function (myoptions) {
            var m = {
                e: '',
                data: '',
                url: '',
                onSelectNode: internal.onSelectNode,
                viewid: '',
                forsearch: internal.forsearch
            };
            if (myoptions) $.extend(m, myoptions);

            // case 21715 - don't trigger onSelectNode event on first event
            var m2 = {};
            $.extend(m2, m);
            m2.onSelectNode = internal.onInitialSelectNode;
            internal.handleSelectNode(m2);

            // rebind event for next select
            external.treeDiv.unbind('select_node.jstree');
            external.treeDiv.bind('select_node.jstree', function (e, data) {
                return internal.handleSelectNode({
                    e: e,
                    data: data,
                    url: m.url,
                    viewid: m.viewid
                });
            });
        }; // internal.firstSelectNode()

        internal.handleSelectNode = function (myoptions) {
            var m = {
                e: '',
                data: '',
                url: '',
                onSelectNode: internal.onSelectNode,
                viewid: '',
                forsearch: internal.forsearch
            };
            if (myoptions) $.extend(m, myoptions);

            var selected = Csw.jsTreeGetSelected(external.treeDiv.$);
            var optSelect = {
                nodeid: selected.$item.CswAttrNonDom('nodeid'),
                nodename: selected.text,
                iconurl: selected.iconurl,
                cswnbtnodekey: selected.$item.CswAttrNonDom('cswnbtnodekey'),
                nodespecies: selected.$item.CswAttrNonDom('species'),
                viewid: m.viewid
            };

            internal.clearChecks();
            Csw.tryExec(m.onSelectNode, optSelect);
        }; // internal.handleSelectNode()

        internal.validateCheck = function ($checkbox) {
            var $selected = Csw.jsTreeGetSelected(external.treeDiv.$);
            return ($selected.$item.CswAttrNonDom('rel') === $checkbox.CswAttrNonDom('rel'));
        };

        internal.clearChecks = function () {
            $('.' + internal.idPrefix + 'check').CswAttrDom('checked', '');
        };


        // Typical mechanism for fetching tree data and making a tree
        external.init = function (options) {
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

            var url = internal.RunTreeUrl;
            var dataParam = {
                ViewId: o.viewid,
                IdPrefix: internal.idPrefix,
                IncludeNodeRequired: o.IncludeNodeRequired,
                IncludeNodeKey: Csw.string(o.cswnbtnodekey),
                IncludeNodeId: Csw.string(o.nodeid),
                NodePk: Csw.string(o.nodeid),
                IncludeInQuickLaunch: o.IncludeInQuickLaunch,
                DefaultSelect: o.DefaultSelect
            };

            if (Csw.isNullOrEmpty(o.viewid)) {
                url = internal.NodeTreeUrl;
            }

            Csw.ajax.post({
                url: url,
                data: dataParam,
                stringify: false,
                success: function (data) {

                    if (Csw.isNullOrEmpty(data)) {
                        Csw.error.showError(Csw.enums.errorType.error.name,
                                            'The requested view cannot be rendered as a Tree.',
                                            'View with ViewId: ' + o.viewid + ' does not exists or is not a Tree view.');
                    } else {

                        var newviewid = data.newviewid;
                        var newviewmode = data.newviewmode;
                        if (false === Csw.isNullOrEmpty(newviewid) && o.viewid !== newviewid) {
                            o.viewid = newviewid;
                            o.viewmode = newviewmode;
                            Csw.tryExec(o.onViewChange, newviewid, newviewmode);
                        }

                        internal.make(data, o.viewid, o.viewmode, url);
                    }

                } // success
            }); // ajax

        }; // external.init()

        // For making a tree without using the regular mechanism for fetching tree data
        external.makeTree = function (treeData) {
            internal.make(treeData, '', 'tree', '');
        };


        external.selectNode = function (optSelect) {
            var o = {
                newnodeid: '',
                newcswnbtnodekey: ''
            };
            if (optSelect) {
                $.extend(o, optSelect);
            }
            external.treeDiv.$.jstree('select_node', '#' + internal.idPrefix + o.newcswnbtnodekey);
        };


        external.expandAll = function () {
            if (external.treeDiv && internal.rootnode) {
                external.treeDiv.$.jstree('open_all', internal.rootnode.$);
            }

            if (internal.toggleLink) {
                internal.toggleLink.text('Collapse All')
                    .unbind('click')
                    .click(function () {
                        external.collapseAll();
                        return false;
                    });
            }
        };

        external.collapseAll = function () {
            external.treeDiv.$.jstree('close_all', internal.rootnode.$);

            // show first level
            external.treeDiv.$.jstree('open_node', internal.rootnode.$);

            internal.toggleLink.text('Expand All')
                .unbind('click')
                .click(function () {
                    external.expandAll();
                    return false;
                });
        };

        external.checkedNodes = function () {
            var $nodechecks = $('.' + internal.idPrefix + 'check:checked');
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

            if (false === Csw.isFunction(internal.onInitialSelectNode)) {
                internal.onInitialSelectNode = internal.onSelectNode;
            }
            internal.idPrefix = Csw.string(internal.ID) + '_';

            if (internal.ShowToggleLink) {
                internal.toggleLink = internal.parent.link({
                    ID: internal.idPrefix + 'toggle',
                    value: 'Expand All',
                    onClick: external.expandAll
                });
                internal.toggleLink.hide();
            }

            external.treeDiv = internal.parent.div({ ID: internal.IdPrefix });

            if (internal.UseScrollbars) {
                external.treeDiv.addClass('treediv');
            } else {
                external.treeDiv.addClass('treediv_noscroll');
            }
            if (false === Csw.isNullOrEmpty(internal.height)) {
                external.treeDiv.css({ height: internal.height });
            }
            if (false === Csw.isNullOrEmpty(internal.width)) {
                external.treeDiv.css({ width: internal.width });
            }

        })(); // constructor

        return external;
    };

    Csw.nbt.register('nodeTree', nodeTree);
    Csw.nbt.nodeTree = Csw.nbt.nodeTree || nodeTree;

})();