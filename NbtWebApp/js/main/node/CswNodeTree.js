/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";

    var pluginName = 'CswNodeTree';

    var methods = {
        'init': function (options) {

            //            function getFirstLevel($treediv, pagesize, pageno, selectFirst) {
            //                var realpagesize = tryParseNumber(pagesize, 10);
            //                if (realpagesize < 1) realpagesize = 10;
            //                var realpageno = tryParseNumber(pageno, 0);
            //                if (realpageno < 1) realpageno = 0;

            //                CswAjaxJson({
            //                    url: o.fetchTreeFirstLevelUrl,
            //                    data: {
            //                        ViewId: o.viewid,
            //                        IdPrefix: tryParseString(idPrefix),
            //                        PageSize: realpagesize,
            //                        PageNo: realpageno,
            //                        ForSearch: o.forsearch
            //                    },
            //                    stringify: false,
            //                    success: function (data) {
            //                        // this page
            //                        var hasChildren = recurseNodes($treediv, 2, data.tree, selectFirst);

            //                        // next page
            //                        if (isTrue(data.more)) {
            //                            setTimeout(function () { getFirstLevel($treediv, realpagesize, realpageno + 1); }, o.Delay);

            //                            // add a more node to click on instead
            //                            // var MoreNode = addNodeToTree($treediv, 2, false, "More");
            //                            // MoreNode.click(function () {
            //                            //     $(this).remove();
            //                            //     getFirstLevel($treediv, realpagesize, realpageno + 1, true);
            //                            // });
            //                        }

            //                        // children
            //                        // Note: root is level 1
            //                        //       "first" level is actually level 2
            //                        //       so the next level is level 3
            //                        if (hasChildren) {
            //                            setTimeout(function () { getLevel($treediv, 3, data.nodecountstart, data.nodecountend, realpagesize, 0); }, o.Delay);
            //                        }
            //                    } // success
            //                }); // ajax
            //            } // getFirstLevel()

            //            function getLevel($treediv, level, parentstart, parentend, pagesize, pageno) {
            //                var reallevel = tryParseNumber(level, 0);
            //                var realparentstart = tryParseNumber(parentstart, 0);
            //                var realparentend = tryParseNumber(parentend, 0);
            //                var realpagesize = tryParseNumber(pagesize, 10);
            //                if (realpagesize < 1) realpagesize = 10;
            //                var realpageno = tryParseNumber(pageno, 0);
            //                if (realpageno < 1) realpageno = 0;

            //                CswAjaxJson({
            //                    url: o.fetchTreeLevelUrl,
            //                    data: {
            //                        ViewId: o.viewid,
            //                        IdPrefix: tryParseString(idPrefix),
            //                        Level: reallevel,
            //                        ParentRangeStart: realparentstart,
            //                        ParentRangeEnd: realparentend,
            //                        PageSize: realpagesize,
            //                        PageNo: realpageno,
            //                        ForSearch: o.forsearch
            //                    },
            //                    stringify: false,
            //                    success: function (data) {

            //                        // this level, this page
            //                        var hasChildren = recurseNodes($treediv, reallevel, data.tree);

            //                        // this level, next page
            //                        if (isTrue(data.more)) {
            //                            setTimeout(function () { getLevel($treediv, reallevel, parentstart, parentend, realpagesize, realpageno + 1); }, o.Delay);
            //                        }

            //                        // children
            //                        if (hasChildren && tryParseNumber(data.nodecountend, -1) > 0) {
            //                            setTimeout(function () { getLevel($treediv, reallevel + 1, data.nodecountstart, data.nodecountend, realpagesize, 0); }, o.Delay);
            //                        }
            //                    }, // success
            //                    watchGlobal: false
            //                }); // ajax
            //            } // getLevel()

            //            function recurseNodes($treediv, level, nodescoll, selectFirst) {
            //                var hasChildren = false;
            //                var parent = false;
            //                each(nodescoll, function (childObj, childKey, thisObj, value) {
            //                    if (false === isNullOrEmpty(childObj)) {
            //                        if (false === isNullOrEmpty(childObj.attr.parentkey)) {
            //                            parent = findParent($treediv, childObj.attr.parentkey);
            //                        }
            //                        if (selectFirst) {
            //                            selectid = childObj.attr.id;
            //                            selectFirst = false;
            //                        }
            //                        addNodeToTree($treediv, level, parent, childObj);
            //                        if (false === isNullOrEmpty(childObj.children) && childObj.children.length > 0) {
            //                            recurseNodes($treediv, (level + 1), childObj.children);
            //                        }
            //                        hasChildren = hasChildren || (tryParseNumber(childObj.childcnt, 0) > 0);
            //                    }
            //                }); // each
            //                return hasChildren;
            //            } // recurseNodes()

            //            function findParent($treediv, parentkey) {
            //                // Using attribute selector $('li[cswnbtnodekey=""]') doesn't seem to work, so we'll do it manually
            //                var ret = false;
            //                $.each($treediv.find('li'), function (childkey, value) {
            //                    var childObj = $(value);
            //                    if (childObj.attr('cswnbtnodekey') === parentkey) {
            //                        ret = childObj;
            //                    }
            //                });
            //                return ret;
            //            } // findParent()

            //            function addNodeToTree($treediv, level, parent, childjs) {
            //                if (false === isNullOrEmpty(childjs)) {
            //                    if (isNullOrEmpty(parent) || parent === false) {
            //                        parent = rootnode;
            //                    }

            //                    var newnode = $treediv.jstree("create", parent, "last", childjs, false, true);
            //                    var newnodeid = newnode.CswAttrDom('id');
            //                    var newnodepk = tryParseString(newnodeid.substring(idPrefix.length));
            //                    var newnodename = '';
            //                    if (false === isNullOrEmpty(childjs.data)) {
            //                        newnodename = childjs.data;
            //                    }
            //                    if (isTrue(o.ShowCheckboxes)) {
            //                        var $checkbox = $('<input type="checkbox" class="' + idPrefix + 'check" id="check_' + newnodeid + '" rel="' + childjs.attr.rel + '" nodeid="' + newnodepk + '" nodename="' + newnodename + '"></input>')
            //                                            .prependTo(newnode);
            //                        $checkbox.click(function () { return handleCheck($treediv, $(this)); });
            //                    }

            //                    if (isTrue(newnode.CswAttrDom('locked'))) {
            //                        $('<img src="Images/quota/lock.gif" title="Quota exceeded" />')
            //                            .appendTo(newnode);
            //                    }

            //                    // case 21424 - Manufacture unique IDs on the expand <ins> for automated testing
            //                    newnode.children('ins').CswAttrDom('id', newnodeid + '_expand');

            //                    if (selectid === newnodeid) {
            //                        $treediv.jstree('select_node', newnode);
            //                    }
            //                }
            //                return newnode;
            //            } // addNodeToTree()

            //            function removeNodeFromTree($treediv, treenode) {
            //                return $treediv.jstree("remove", treenode);
            //            } // removeNodeFromTree()

            function expandAll() {
                $treediv.jstree('open_all', rootnode);

                $togglelink.text('Collapse All')
                           .unbind('click')
                           .click(collapseAll);
            }
            function collapseAll() {
                $treediv.jstree('close_all', rootnode);

                // show first level
                $treediv.jstree('open_node', rootnode);

                // show selected
                // this has weird effects
                //                var $sel = $treediv.jstree('get_selected');
                //                if (false === isNullOrEmpty($sel)) {
                //                    $treediv.jstree('open_node', $sel.parentsUntil('#' + rootnode.attr('id')));
                //                }

                $togglelink.text('Expand All')
                           .unbind('click')
                           .click(expandAll);
            }

            var rootnode = false;  // root node of tree
            //var selectid = null;   // node to select, once populated

            var o = {
                ID: '',
                RunTreeUrl: '/NbtWebApp/wsNBT.asmx/runTree',
                fetchTreeFirstLevelUrl: '/NbtWebApp/wsNBT.asmx/fetchTreeFirstLevel',
                fetchTreeLevelUrl: '/NbtWebApp/wsNBT.asmx/fetchTreeLevel',
                NodeTreeUrl: '/NbtWebApp/wsNBT.asmx/getTreeOfNode',
                viewid: '',       // loads an arbitrary view
                viewmode: '',
                showempty: false, // if true, shows an empty tree (primarily for search)
                forsearch: false, // if true, used to override default behavior of list views
                nodeid: '',       // if viewid are not supplied, loads a view of this node
                cswnbtnodekey: '',
                IncludeNodeRequired: false,
                //UsePaging: true,
                UseScrollbars: true,
                onSelectNode: null, // function(optSelect) { var o =  { nodeid: '',  nodename: '', iconurl: '', cswnbtnodekey: '', viewid: '' }; return o; },
                onInitialSelectNode: undefined,
                onViewChange: null, // function(newviewid, newviewmode) {},    // if the server returns a different view than what we asked for (e.g. case 21262)
                //SelectFirstChild: true,
                ShowCheckboxes: false,
                ShowToggleLink: true,
                IncludeInQuickLaunch: true,
                Delay: 250
            };
            if (options) $.extend(o, options);

            if (false === isFunction(o.onInitialSelectNode)) {
                o.onInitialSelectNode = o.onSelectNode;
            }

            var idPrefix = o.ID + '_';
            var $this = $(this);

            var $togglelink;
            if (o.ShowToggleLink) {
                $togglelink = $this.CswLink({
                    ID: o.ID + '_toggle',
                    value: 'Expand All',
                    href: '#',
                    onClick: function () { expandAll(); return false; }
                });
                $togglelink.hide();
            }

            var $treediv = $('<div id="' + idPrefix + '" />')
                                .appendTo($this);
            if (o.UseScrollbars) {
                $treediv.addClass('treediv');
            } else {
                $treediv.addClass('treediv_noscroll');
            }

            var url = o.RunTreeUrl;
            var dataParam = {
                // UsePaging: o.UsePaging,
                ViewId: o.viewid,
                IdPrefix: tryParseString(idPrefix),
                // IsFirstLoad: true,
                // ParentNodeKey: '',
                IncludeNodeRequired: o.IncludeNodeRequired,
                IncludeNodeKey: tryParseString(o.cswnbtnodekey),
                IncludeNodeId: tryParseString(o.nodeid),
                // ShowEmpty: o.showempty,
                // ForSearch: o.forsearch,
                // NodePk: tryParseString(o.nodeid),
                IncludeInQuickLaunch: o.IncludeInQuickLaunch
            };

            if (isNullOrEmpty(o.viewid)) {
                url = o.NodeTreeUrl;
            }

            CswAjaxJson({
                url: url,
                data: dataParam,
                stringify: false,
                success: function (data) {

                    var treePlugins = ["themes", "ui", "types", "crrm", "json_data"];
                    var jsonTypes = data.types;

                    var newviewid = data.newviewid;
                    if (false === isNullOrEmpty(newviewid) && o.viewid !== newviewid) {
                        o.viewid = newviewid;
                        if (isFunction(o.onViewChange)) {
                            o.onViewChange(o.viewid, o.viewmode);
                        }
                    }

                    //selectid = data.selectid;
                    var treeThemes = { "dots": true };
                    if (o.viewmode === CswViewMode.list.name) {
                        treeThemes = { "dots": false };
                    }
                    $treediv.jstree({
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
                            "types": jsonTypes,
                            "max_children": -2,
                            "max_depth": -2
                        },
                        "plugins": treePlugins
                    }); // jstree()

                    $treediv.bind('select_node.jstree', function (e, newData) {
                        return firstSelectNode({
                            e: e,
                            data: newData,
                            url: url,
                            $treediv: $treediv,
                            IdPrefix: idPrefix,
                            onSelectNode: o.onSelectNode,
                            onInitialSelectNode: o.onInitialSelectNode,
                            viewid: o.viewid,
                            //UsePaging: o.UsePaging,
                            forsearch: o.forsearch
                        });

                    }).bind('hover_node.jstree', function (e, bindData) {
                        var $hoverLI = $(bindData.rslt.obj[0]);
                        var nodeid = $hoverLI.CswAttrDom('id').substring(idPrefix.length);
                        var cswnbtnodekey = $hoverLI.CswAttrDom('cswnbtnodekey');
                        nodeHoverIn(bindData.args[1], nodeid, cswnbtnodekey);

                    }).bind('dehover_node.jstree', function () {
                        jsTreeGetSelected($treediv);
                        nodeHoverOut();
                    });

                    $treediv.jstree('select_node', '#' + data.selectid);
                    //setTimeout(function () { log('select: #' + data.selectid);  }, 1000);
                    rootnode = $treediv.find('li').first();

                    if (isTrue(o.ShowCheckboxes)) {

                        $treediv.find('li').each(function () {
                            var $childObj = $(this);
                            var thisid = $childObj.CswAttrDom('id');
                            var thisnodeid = $childObj.CswAttrDom('nodeid');
                            var thisrel = $childObj.CswAttrDom('rel');
                            var thisnodename = $childObj.CswAttrDom('nodename');
                            $('<input type="checkbox" class="' + idPrefix + 'check" id="check_' + thisid + '" rel="' + thisrel + '" nodeid="' + thisnodeid + '" nodename="' + thisnodename + '"></input>')
                                .prependTo($childObj)
                                .click(function () { return handleCheck($treediv, $(this)); });

                        });


                    }

                    $togglelink.show();

                    // DO NOT define an onSuccess() function here that interacts with the tree.
                    // The tree has initalization events that appear to happen asynchronously,
                    // and thus having an onSuccess() function that changes the selected node will
                    // cause a race condition.

                    //                    rootnode = addNodeToTree($treediv, 1, false, data.root);
                    //                    if (isTrue(data.result)) {
                    //                        getFirstLevel($treediv, data.pagesize, 0);
                    //                    } else {
                    //                        addNodeToTree($treediv, 1, rootnode, { data: { title: "No Results" }, attr: { id: idPrefix + 'noresults'} });
                    //                    }

                    //                    removeNodeFromTree($treediv, $('.jstree-loading'));

                } // success
            }); // ajax

            return $treediv;
        },

        'selectNode': function (optSelect) {
            var o = {
                newnodeid: '',
                newcswnbtnodekey: ''
            };
            if (optSelect) {
                $.extend(o, optSelect);
            }
            var $treediv = $(this);
            var idPrefix = $treediv.CswAttrDom('id');
            $treediv.jstree('select_node', '#' + idPrefix + o.newnodeid);
        },

        'expandAll': function () {
            var $treediv = $(this);
            var rootnode = $treediv.find('li').first();
            $treediv.jstree('open_all', rootnode);
        }

    };

    function firstSelectNode(myoptions) {
        var m = {
            e: '',
            data: '',
            url: '',
            $treediv: '',
            IdPrefix: '',
            onSelectNode: null, //function() {},
            onInitialSelectNode: null, //function() {},
            viewid: '',
            //UsePaging: '',
            forsearch: ''
        };
        if (myoptions) $.extend(m, myoptions);

        // case 21715 - don't trigger onSelectNode event on first event
        var m2 = {};
        $.extend(m2, m);
        m2.onSelectNode = m.onInitialSelectNode;
        handleSelectNode(m2);

        // rebind event for next select
        m.$treediv.unbind('select_node.jstree');
        m.$treediv.bind('select_node.jstree', function () { return handleSelectNode(m); });
    }

    function handleSelectNode(myoptions) {
        var m = {
            e: '',
            data: '',
            url: '',
            $treediv: '',
            IdPrefix: '',
            onSelectNode: function () { },
            viewid: '',
            //UsePaging: '',
            forsearch: ''
        };
        if (myoptions) $.extend(m, myoptions);

        var selected = jsTreeGetSelected(m.$treediv);
        var optSelect = {
            nodeid: selected.id,
            nodename: selected.text,
            iconurl: selected.iconurl,
            cswnbtnodekey: selected.$item.CswAttrDom('cswnbtnodekey'),
            nodespecies: selected.$item.CswAttrDom('species'),
            viewid: m.viewid
        };

        clearChecks(m.IdPrefix);
        m.onSelectNode(optSelect);
    }

    function handleCheck($treediv, $checkbox) {
        var $selected = jsTreeGetSelected($treediv);
        return ($selected.$item.CswAttrDom('rel') === $checkbox.CswAttrDom('rel'));
    }

    function clearChecks(IdPrefix) {
        $('.' + IdPrefix + 'check').CswAttrDom('checked', '');
    }

    // Method calling logic
    $.fn.CswNodeTree = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName);
        }

    };

})(jQuery);