/// <reference path="~/app/CswApp-vsdoc.js" />

/*global Csw:true*/
(function csw_nbt_nodeTree() {
    "use strict";

    Csw.jsTreeGetSelected = Csw.jsTreeGetSelected ||
       Csw.register('jsTreeGetSelected', function ($treediv) {
           var idPrefix = $treediv.CswAttrDom('id');
           var $SelectedItem = $treediv.jstree('get_selected');
           var ret = {
               'iconurl': $SelectedItem.children('a').children('ins').css('background-image'),
               'id': Csw.string($SelectedItem.CswAttrDom('id')).substring(idPrefix.length),
               'text': Csw.string($SelectedItem.children('a').first().text()).trim(),
               '$item': $SelectedItem
           };
           return ret;
       });

    Csw.nbt.nodeTree = Csw.nbt.nodeTree ||
    Csw.nbt.register('nodeTree', function (opts) {

        var cswPrivate = {
            name: '',
            parent: null,
            //showempty: false, // if true, shows an empty tree (primarily for search)
            forsearch: false, // if true, used to override default behavior of list views
            UseScrollbars: true,
            onSelectNode: null, // function (optSelect) { var o =  { nodeid: '',  nodename: '', iconurl: '', nodekey: '', viewid: '' }; return o; },
            onInitialSelectNode: null,
            ShowCheckboxes: false,
            ValidateCheckboxes: true,
            ShowToggleLink: true,
            height: '',
            width: '',

            RunTreeUrl: 'runTree',
            fetchTreeFirstLevelUrl: 'fetchTreeFirstLevel',
            fetchTreeLevelUrl: 'fetchTreeLevel',
            NodeTreeUrl: 'getTreeOfNode',
            rootnode: null,
            toggleLink: null
        };
        if (opts) Csw.extend(cswPrivate, opts);

        var cswPublic = {
            menuDiv: null,
            treeDiv: null
        };


        cswPrivate.make = function (data, viewid, viewmode, url) {

            var treeThemes = { dots: true };
            if (viewmode === Csw.enums.viewMode.list.name) {
                treeThemes = { dots: false };
            }
            cswPublic.treeDiv.$.jstree({
                json_data: {
                    data: data.root
                },
                core: {
                    open_parents: true
                },
                ui: {
                    select_limit: 1,
                    selected_parent_close: false,
                    selected_parent_open: true
                },
                themes: treeThemes,
                types: {
                    types: data.types,
                    max_children: -2,
                    max_depth: -2
                },
                plugins: ["themes", "ui", "types", "crrm", "json_data"]
            }); // jstree()

            function selectNode(e, newData) {
                return cswPrivate.firstSelectNode({
                    e: e,
                    data: newData,
                    url: url,
                    viewid: viewid
                });
            }
            cswPublic.treeDiv.bind('select_node.jstree', selectNode);

//            function hoverNode(e, bindData) {
//                var $hoverLI = $(bindData.rslt.obj[0]);
//                //var nodeid = $hoverLI.CswAttrDom('id').substring(cswPrivate.idPrefix.length);
//                cswPrivate.hoverNodeId = $hoverLI.CswAttrNonDom('nodeid');
//                var nodekey = $hoverLI.CswAttrNonDom('nodekey');
//                Csw.nodeHoverIn(bindData.args[1], cswPrivate.hoverNodeId, nodekey);
//            }
//            cswPublic.treeDiv.bind('hover_node.jstree', hoverNode);

//            function deHoverNode() {
//                Csw.jsTreeGetSelected(cswPublic.treeDiv.$);
//                Csw.nodeHoverOut(null, cswPrivate.hoverNodeId);
//            }
//            cswPublic.treeDiv.bind('dehover_node.jstree', deHoverNode);

            cswPublic.treeDiv.$.jstree('select_node', Csw.tryParseElement(data.selectid));

            cswPrivate.rootnode = cswPublic.treeDiv.find('li').first();

            cswPublic.treeDiv.find('li').$.each(function () {
                var $childObj = $(this);
                var thisid = Csw.string($childObj.CswAttrDom('id'));
                var thiskey = Csw.string($childObj.CswAttrDom('nodekey'));
                var thisnodeid = Csw.string($childObj.CswAttrNonDom('nodeid'), thisid.substring(cswPrivate.idPrefix.length));
                var thisrel = Csw.string($childObj.CswAttrNonDom('rel'));
                var altName = Csw.string($childObj.find('a').first().text());
                var thisnodename = Csw.string($childObj.CswAttrNonDom('nodename'), altName).trim();
                var thislocked = Csw.bool($childObj.CswAttrNonDom('locked'));
                var thisdisabled = ($childObj.CswAttrNonDom('disabled') === 'disabled');

                var $cb = $('<input type="checkbox" class="' + cswPrivate.idPrefix + 'check" id="check_' + thisid + '" rel="' + thisrel + '" nodeid="' + thisnodeid + '" nodename="' + thisnodename + '" nodekey="' + thiskey + '"></input>');
                $cb.prependTo($childObj);
                if (false === Csw.bool(cswPrivate.ShowCheckboxes)) {
                    $cb.hide();
                }
                Csw.subscribe('CswMultiEdit', (function _onMultiInvoc() {
                    return function _onMulti(eventObj, multiOpts) {
                        if (multiOpts && multiOpts.viewid === viewid) {
                            if (multiOpts.multi || Csw.bool(cswPrivate.ShowCheckboxes)) {
                                $cb.show();
                                if (cswPrivate.ValidateCheckboxes) {
                                    $cb.click(function () { return cswPrivate.validateCheck($cb); });
                                }
                            } else { // if (Csw.bool(cswPrivate.ShowCheckboxes)) {
                                $cb.hide();
                            }
                        } else {
                            //Csw.debug.assert(multiOpts.viewid === viewid, 'CswMultiEdit event pusblished for viewid "' + multiOpts.viewid + '" but was subscribed to from viewid "' + viewid + '".');
                            Csw.unsubscribe('CswMultiEdit', null, _onMulti);
                            Csw.unsubscribe('CswMultiEdit', null, _onMultiInvoc);
                        }
                    };
                }()));

                if (thislocked) {
                    $('<img src="Images/quota/lock.gif" title="Quota exceeded" />')
                        .appendTo($childObj.children('a').first());
                }

                if (thisdisabled) {
                    $childObj.children('a').addClass('disabled');
                }

            }); // each()

            if (cswPrivate.ShowToggleLink && cswPrivate.toggleLink) {
                cswPrivate.toggleLink.show();
            }
            // DO NOT define an onSuccess() function here that interacts with the tree.
            // The tree has initalization events that appear to happen asynchronously,
            // and thus having an onSuccess() function that changes the selected node will
            // cause a race condition.

        }; // cswPrivate.make()

        cswPrivate.firstSelectNode = function (myoptions) {
            var m = {
                e: '',
                data: '',
                url: '',
                onSelectNode: cswPrivate.onSelectNode,
                viewid: '',
                forsearch: cswPrivate.forsearch
            };
            if (myoptions) Csw.extend(m, myoptions);

            // case 21715 - don't trigger onSelectNode event on first event
            var m2 = {};
            Csw.extend(m2, m);
            m2.onSelectNode = cswPrivate.onInitialSelectNode;
            cswPrivate.handleSelectNode(m2);

            // rebind event for next select
            cswPublic.treeDiv.unbind('select_node.jstree');
            cswPublic.treeDiv.bind('select_node.jstree', function (e, data) {
                return cswPrivate.handleSelectNode({
                    e: e,
                    data: data,
                    url: m.url,
                    viewid: m.viewid
                });
            });
        }; // cswPrivate.firstSelectNode()

        cswPrivate.handleSelectNode = function (myoptions) {
            var m = {
                e: '',
                data: '',
                url: '',
                onSelectNode: cswPrivate.onSelectNode,
                viewid: '',
                forsearch: cswPrivate.forsearch
            };
            if (myoptions) Csw.extend(m, myoptions);

            var selected = Csw.jsTreeGetSelected(cswPublic.treeDiv.$);
            var optSelect = {
                nodeid: selected.$item.CswAttrNonDom('nodeid'),
                nodename: selected.text,
                iconurl: selected.iconurl,
                nodekey: selected.$item.CswAttrNonDom('nodekey'),
                nodespecies: selected.$item.CswAttrNonDom('species'),
                viewid: m.viewid
            };

            cswPrivate.clearChecks();

            // case 25844 - open children
            cswPublic.treeDiv.$.jstree('open_node', selected.$item);

            Csw.tryExec(m.onSelectNode, optSelect);
        }; // cswPrivate.handleSelectNode()

        cswPrivate.validateCheck = function ($checkbox) {
            var $selected = Csw.jsTreeGetSelected(cswPublic.treeDiv.$);
            return ($selected.$item.CswAttrNonDom('rel') === $checkbox.CswAttrNonDom('rel'));
        };

        cswPrivate.clearChecks = function () {
            $('.' + cswPrivate.idPrefix + 'check').CswAttrDom('checked', '');
        };


        // Typical mechanism for fetching tree data and making a tree
        cswPublic.init = function (options) {
            var o = {
                viewid: '',       // loads an arbitrary view
                viewmode: '',
                nodeid: '',       // if viewid are not supplied, loads a view of this node
                nodekey: '',
                IncludeNodeRequired: false,
                IncludeInQuickLaunch: true,
                onViewChange: null, // function (newviewid, newviewmode) {},    // if the server returns a different view than what we asked for (e.g. case 21262)
                DefaultSelect: Csw.enums.nodeTree_DefaultSelect.firstchild.name
            };
            if (options) Csw.extend(o, options);

            var url = cswPrivate.RunTreeUrl;
            var dataParam = {
                ViewId: o.viewid,
                IdPrefix: cswPrivate.idPrefix,
                IncludeNodeRequired: o.IncludeNodeRequired,
                IncludeNodeKey: Csw.string(o.nodekey),
                IncludeNodeId: Csw.string(o.nodeid),
                NodePk: Csw.string(o.nodeid),
                IncludeInQuickLaunch: o.IncludeInQuickLaunch,
                DefaultSelect: o.DefaultSelect
            };

            if (Csw.isNullOrEmpty(o.viewid)) {
                url = cswPrivate.NodeTreeUrl;
            }

            Csw.ajax.post({
                urlMethod: url,
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

                        cswPrivate.make(data, o.viewid, o.viewmode, url);
                    }

                } // success
            }); // ajax

        }; // cswPublic.init()

        // For making a tree without using the regular mechanism for fetching tree data
        cswPublic.makeTree = function (treeData) {
            cswPrivate.make(treeData, '', 'tree', '');
        };


        cswPublic.selectNode = function (optSelect) {
            var o = {
                newnodeid: '',
                newcswnbtnodekey: ''
            };
            if (optSelect) {
                Csw.extend(o, optSelect);
            }
            cswPublic.treeDiv.$.jstree('select_node', '#' + cswPrivate.idPrefix + o.newcswnbtnodekey);
        };


        cswPublic.expandAll = function () {
            if (cswPublic.treeDiv && cswPrivate.rootnode) {
                cswPublic.treeDiv.$.jstree('open_all', cswPrivate.rootnode.$);
            }

            if (cswPrivate.toggleLink) {
                cswPrivate.toggleLink.text('Collapse All')
                    .unbind('click')
                    .click(function () {
                        cswPublic.collapseAll();
                        return false;
                    });
            }
        };

        cswPublic.collapseAll = function () {
            cswPublic.treeDiv.$.jstree('close_all', cswPrivate.rootnode.$);

            // show first level
            cswPublic.treeDiv.$.jstree('open_node', cswPrivate.rootnode.$);

            cswPrivate.toggleLink.text('Expand All')
                .unbind('click')
                .click(function () {
                    cswPublic.expandAll();
                    return false;
                });
        };

        cswPublic.checkedNodes = function () {
            var $nodechecks = $('.' + cswPrivate.idPrefix + 'check:checked');
            var ret = [];

            if (false === Csw.isNullOrEmpty($nodechecks, true)) {
                var n = 0;
                $nodechecks.each(function () {
                    var $nodecheck = $(this);
                    ret[n] = {
                        nodeid: $nodecheck.CswAttrNonDom('nodeid'),
                        nodename: $nodecheck.CswAttrNonDom('nodename'),
                        nodekey: $nodecheck.CswAttrNonDom('nodekey')
                    };
                    n++;
                });
            }
            return ret;
        };


        (function constructor() {

            if (false === Csw.isFunction(cswPrivate.onInitialSelectNode)) {
                cswPrivate.onInitialSelectNode = cswPrivate.onSelectNode;
            }
            cswPrivate.idPrefix = Csw.string(cswPrivate.name) + '_';

            cswPrivate.table = cswPrivate.parent.table();
            var cell11 = cswPrivate.table.cell(1, 1);
            var cell12 = cswPrivate.table.cell(1, 2);
            var cell21 = cswPrivate.table.cell(2, 1);

            cell11.css({ width: '100px', verticalAlign: 'middle' });
            cell12.css({ width: '170px' });
            cell21.propDom('colspan', 2);

            cswPrivate.toggleDiv = cell11.div({
                name: 'toggleDiv'
            });
            cswPublic.menuDiv = cell12.div({
                name: 'menuDiv',
                align: 'right'
            });
            cswPublic.treeDiv = cell21.div({
                name: 'treeDiv'
            });


            if (cswPrivate.ShowToggleLink) {
                cswPrivate.toggleLink = cswPrivate.toggleDiv.a({
                    name: 'toggleLink',
                    value: 'Expand All',
                    onClick: cswPublic.expandAll
                });
                cswPrivate.toggleLink.hide();
            }

            if (cswPrivate.UseScrollbars) {
                cswPublic.treeDiv.addClass('treediv');
            } else {
                cswPublic.treeDiv.addClass('treediv_noscroll');
            }
            if (false === Csw.isNullOrEmpty(cswPrivate.height)) {
                cswPublic.treeDiv.css({ height: cswPrivate.height });
            }
            if (false === Csw.isNullOrEmpty(cswPrivate.width)) {
                cswPublic.treeDiv.css({ width: cswPrivate.width });
            }

        })(); // constructor

        return cswPublic;
    });


})();