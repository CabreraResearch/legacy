/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";

    var pluginName = 'CswNodeTree';

    var methods = {
        'init': function (options)     // options are defined in _getTreeContent()
        {
            function getFirstLevel($treediv, pagesize, pageno) {
                var realpagesize = tryParseNumber(pagesize, 0);
                var realpageno = tryParseNumber(pageno, 0);

                CswAjaxJson({
                    url: o.fetchTreeFirstLevelUrl,
                    data: {
                        ViewId: o.viewid,
                        IdPrefix: tryParseString(idPrefix),
                        PageSize: realpagesize,
                        PageNo: realpageno,
                        ForSearch: o.forsearch
                    },
                    stringify: false,
                    success: function (data) {
                        // this page
                        recurseNodes($treediv, 2, data.tree);

                        // next page
                        if (isTrue(data.more)) {
                            getFirstLevel($treediv, realpagesize, realpageno + 1);
                        }

                        // children
                        // Note: root is level 1
                        //       "first" level is actually level 2
                        //       so the next level is level 3
                        getLevel($treediv, 3, data.nodecountstart, data.nodecountend);

                    } // success
                }); // ajax
            } // getFirstLevel()

            function getLevel($treediv, level, parentstart, parentend) {
                var reallevel = tryParseNumber(level, 0);
                var realparentstart = tryParseNumber(parentstart, 0);
                var realparentend = tryParseNumber(parentend, 0);

                CswAjaxJson({
                    url: o.fetchTreeLevelUrl,
                    data: {
                        ViewId: o.viewid,
                        IdPrefix: tryParseString(idPrefix),
                        Level: reallevel,
                        ParentRangeStart: realparentstart,
                        ParentRangeEnd: realparentend,
                        ForSearch: o.forsearch
                    },
                    stringify: false,
                    success: function (data) {
                        // this level
                        recurseNodes($treediv, reallevel, data.tree);

                        // children
                        if (tryParseNumber(data.nodecountend, -1) > 0) {
                            getLevel($treediv, reallevel + 1, data.nodecountstart, data.nodecountend);
                        }
                    } // success
                }); // ajax
            } // getLevel()

            function recurseNodes($treediv, level, nodescoll) {
                var parent = false;
                each(nodescoll, function (childObj, childKey, thisObj, value) {
                    if (false === isNullOrEmpty(childObj)) {
                        if (false === isNullOrEmpty(childObj.attr.parentkey)) {
                            parent = findParent($treediv, childObj.attr.parentkey);
                        }
                        addNodeToTree($treediv, level, parent, childObj);
                        if (false === isNullOrEmpty(childObj.children) && childObj.children.length > 0) {
                            recurseNodes($treediv, (level + 1), childObj.children);
                        }
                    }
                }); // each
            } // recurseNodes()

            function findParent($treediv, parentkey) {
                // Using attribute selector $('li[cswnbtnodekey=""]') doesn't seem to work, so we'll do it manually
                var ret = false;
                $.each($treediv.find('li'), function (childkey, value) {
                    var childObj = $(value);
                    if (childObj.attr('cswnbtnodekey') === parentkey) {
                        ret = childObj;
                    }
                });
                return ret;
            } // findParent()

            function addNodeToTree($treediv, level, parentnode, childjs) {
                if (false === isNullOrEmpty(childjs)) {
                    if (isNullOrEmpty(parentnode) || parentnode === false) {
                        parentnode = rootnode;
                    }
                    var newnode = $treediv.jstree("create", parentnode, "last", childjs, false, true);
                    if (rootnode === false) {
                        rootnode = newnode;
                    }

                    if (level === selectlevel && isNullOrEmpty(selectid)) {
                        selectid = childjs.attr.id;
                    }

                    if (selectid === childjs.attr.id) {
                        $treediv.jstree('select_node', '#' + selectid);
                    }
                }
            } // addNodeToTree()

            function removeNodeFromTree($treediv, treenode) {
                return $treediv.jstree("remove", treenode);
            } // removeNodeFromTree()


            var rootnode = false;  // root node of tree
            var selectid = null;   // node to select, once populated
            var selectlevel = -1;  // select first node of level, if no selectid is defined

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
                UsePaging: true,
                UseScrollbars: true,
                onSelectNode: null, // function(optSelect) { var o =  { nodeid: '',  nodename: '', iconurl: '', cswnbtnodekey: '', viewid: '' }; return o; },
                onInitialSelectNode: undefined,
                onViewChange: null, // function(newviewid, newviewmode) {},    // if the server returns a different view than what we asked for (e.g. case 21262)
                SelectFirstChild: true,
                ShowCheckboxes: false,
                IncludeInQuickLaunch: true
            };
            if (options) $.extend(o, options);

            if (false === isFunction(o.onInitialSelectNode)) {
                o.onInitialSelectNode = o.onSelectNode;
            }

            var idPrefix = o.ID + '_';
            var $treediv = $('<div id="' + idPrefix + '" />')
                                .appendTo($(this));
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
                    if (isTrue(data.result)) {
                        var treePlugins = ["themes", "ui", "types", "crrm"];
                        var jsonTypes = data.types;

                        if (false === isNullOrEmpty(o.nodeid)) {
                            selectid = idPrefix + o.nodeid;
                        }

                        var newviewid = data.viewid;
                        if (false === isNullOrEmpty(newviewid) && o.viewid !== newviewid) {
                            o.viewid = newviewid;
                            if (isFunction(o.onViewChange)) {
                                o.onViewChange(o.viewid, 'tree');
                            }
                        }

                        var treeThemes;
                        if (o.SelectFirstChild) {
                            switch (data.viewmode) {
                                case CswViewMode.list.name:
                                    selectlevel = 1;
                                    treeThemes = { "dots": false };
                                    break;

                                case CswViewMode.tree.name:
                                    selectlevel = 2;
                                    treeThemes = { "dots": true };
                                    break;

                                default:
                                    CswError(ChemSW.makeClientSideError(ChemSW.enums.ErrorType.warning.name, 'Cannot load a ' + data.viewmode + ' view as a tree.'));
                                    break;
                            }
                        } else {
                            if (isNullOrEmpty(selectid)) {
                                selectid = idPrefix + 'root';
                            }
                        }

                        $treediv.jstree({
                            "ui": {
                                "select_limit": 1
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
                                UsePaging: o.UsePaging,
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

                        // DO NOT define an onSuccess() function here that interacts with the tree.
                        // The tree has initalization events that appear to happen asynchronously,
                        // and thus having an onSuccess() function that changes the selected node will
                        // cause a race condition.

                        addNodeToTree($treediv, 1, false, data.root);

                        getFirstLevel($treediv, 2, 0);

                        removeNodeFromTree($treediv, $('.jstree-loading'));

                    } // if (isTrue(data.result)) {
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
            var $treediv = $(this).children('.treediv');
            var idPrefix = $treediv.CswAttrDom('id');
            $treediv.jstree('select_node', '#' + idPrefix + o.newnodeid);
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
            UsePaging: '',
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
            UsePaging: '',
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

