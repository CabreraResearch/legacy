///// <reference path="~/app/CswApp-vsdoc.js" />


//(function ($) {
//    "use strict";

//    var pluginName = 'CswNodeTree';

//    var methods = {
//        'init': function (options) {   // see parseOptions() below for options definitions

//            var rootnode = false;  // root node of tree
//            //var selectid = null;   // node to select, once populated

//            var o = parseOptions(options);

//            var idPrefix = o.ID + '_';
//            var $parent = $(this);
//            var parent = Csw.literals.factory($parent);

//            var treeDiv = parent.div({ ID: idPrefix });

//            var url = o.RunTreeUrl;
//            var dataParam = {
//                // UsePaging: o.UsePaging,
//                ViewId: o.viewid,
//                IdPrefix: Csw.string(idPrefix),
//                // IsFirstLoad: true,
//                // ParentNodeKey: '',
//                IncludeNodeRequired: o.IncludeNodeRequired,
//                IncludeNodeKey: Csw.string(o.cswnbtnodekey),
//                IncludeNodeId: Csw.string(o.nodeid),
//                // ShowEmpty: o.showempty,
//                // ForSearch: o.forsearch,
//                NodePk: Csw.string(o.nodeid),
//                IncludeInQuickLaunch: o.IncludeInQuickLaunch,
//                DefaultSelect: o.DefaultSelect
//            };

//            if (Csw.isNullOrEmpty(o.viewid)) {
//                url = o.NodeTreeUrl;
//            }

//            Csw.ajax.post({
//                url: url,
//                data: dataParam,
//                stringify: false,
//                success: function (data) {

//                    var ret = makeTree(o, parent, data, treeDiv, url);
//                    rootnode = ret.rootnode;
//                    treeDiv = ret.treeDiv;

//                } // success
//            }); // ajax

//            return treeDiv;
//        },

//        'selectNode': function (optSelect) {
//            var o = {
//                newnodeid: '',
//                newcswnbtnodekey: ''
//            };
//            if (optSelect) {
//                Csw.extend(o, optSelect);
//            }
//            var $treediv = $(this);
//            var idPrefix = $treediv.CswAttrDom('id');
//            $treediv.jstree('select_node', '#' + idPrefix + o.newcswnbtnodekey);
//        },

//        'expandAll': function () {
//            var $treediv = $(this);
//            var rootnode = $treediv.find('li').first();
//            $treediv.jstree('open_all', rootnode);
//        },

//        // For making a tree without using the regular mechanism for fetching tree data
//        'makeTree': function (treeData, options) {   // see parseOptions() below for options definitions
//            var $parent = $(this);
//            var parent = Csw.literals.factory($parent);

//            var o = parseOptions(options);

//            var ret = makeTree(parseOptions(options), parent, treeData);
//            return ret.treeDiv;
//        },

//        'checkedNodes': function () {
//            var $treediv = $(this);
//            var idPrefix = $treediv.CswAttrDom('id');
//            var $nodechecks = $('.' + idPrefix + 'check:checked');
//            var ret = [];

//            if (false === Csw.isNullOrEmpty($nodechecks, true)) {
//                var n = 0;
//                $nodechecks.each(function () {
//                    var $nodecheck = $(this);
//                    ret[n] = {
//                        nodeid: $nodecheck.CswAttrNonDom('nodeid'),
//                        nodename: $nodecheck.CswAttrNonDom('nodename'),
//                        cswnbtnodekey: $nodecheck.CswAttrNonDom('cswnbtnodekey')
//                    };
//                    n++;
//                });
//            }
//            return ret;
//        }

//    };

//    function expandAll(treeDiv, rootnode, toggleLink) {
//        treeDiv.$.jstree('open_all', rootnode);

//        toggleLink.text('Collapse All')
//            .unbind('click')
//            .click(function () { collapseAll(treeDiv, rootnode, toggleLink); return false; });
//    }

//    function collapseAll(treeDiv, rootnode, toggleLink) {
//        treeDiv.$.jstree('close_all', rootnode);

//        // show first level
//        treeDiv.$.jstree('open_node', rootnode);

//        toggleLink.text('Expand All')
//            .unbind('click')
//            .click(function () { expandAll(treeDiv, rootnode, toggleLink); return false; });
//    }

//    function parseOptions(options) {
//        var o = {
//            ID: '',
//            RunTreeUrl: '/NbtWebApp/wsNBT.asmx/runTree',
//            fetchTreeFirstLevelUrl: '/NbtWebApp/wsNBT.asmx/fetchTreeFirstLevel',
//            fetchTreeLevelUrl: '/NbtWebApp/wsNBT.asmx/fetchTreeLevel',
//            NodeTreeUrl: '/NbtWebApp/wsNBT.asmx/getTreeOfNode',
//            viewid: '',       // loads an arbitrary view
//            viewmode: '',
//            showempty: false, // if true, shows an empty tree (primarily for search)
//            forsearch: false, // if true, used to override default behavior of list views
//            nodeid: '',       // if viewid are not supplied, loads a view of this node
//            cswnbtnodekey: '',
//            IncludeNodeRequired: false,
//            //UsePaging: true,
//            UseScrollbars: true,
//            onSelectNode: null, // function (optSelect) { var o =  { nodeid: '',  nodename: '', iconurl: '', cswnbtnodekey: '', viewid: '' }; return o; },
//            onInitialSelectNode: undefined,
//            onViewChange: null, // function (newviewid, newviewmode) {},    // if the server returns a different view than what we asked for (e.g. case 21262)
//            //SelectFirstChild: true,
//            ShowCheckboxes: false,
//            ValidateCheckboxes: true,
//            ShowToggleLink: true,
//            IncludeInQuickLaunch: true,
//            //Delay: 250,
//            DefaultSelect: Csw.enums.nodeTree_DefaultSelect.firstchild.name,
//            height: ''
//        };
//        if (options) Csw.extend(o, options);

//        if (false === Csw.isFunction(o.onInitialSelectNode)) {
//            o.onInitialSelectNode = o.onSelectNode;
//        }
//        return o;
//    }

//    function makeTreeDiv(o, onToggleLinkClick) {
//        var idPrefix = o.ID + '_';

//        var toggleLink;
//        if (o.ShowToggleLink) {
//            toggleLink = parent.a({
//                ID: idPrefix + 'toggle',
//                value: 'Expand All',
//                onClick: onToggleLinkClick
//            });
//            toggleLink.hide();
//        }


//        var treeDiv = parent.div({ ID: idPrefix });
//        if (o.UseScrollbars) {
//            treeDiv.addClass('treediv');
//        } else {
//            treeDiv.addClass('treediv_noscroll');
//        }
//        if (false === Csw.isNullOrEmpty(o.height)) {
//            treeDiv.css({ height: o.height });
//        }

//        return {
//            treeDiv: treeDiv,
//            toggleLink: toggleLink
//        };
//    }

//    function makeTree(o, parent, data, treeDiv, toggleLink, url) {
//        var rootnode;

//        makeTreeDiv(o, function () { expandAll(treeDiv, rootnode, toggleLink); return false; });

//        if (Csw.isNullOrEmpty(data)) {
//            Csw.error.showError(Csw.enums.errorType.error.name, 'The requested view cannot be rendered as a Tree.', 'View with ViewId: ' + o.viewid + ' does not exists or is not a Tree view.');
//        } else {
//            var treePlugins = ["themes", "ui", "types", "crrm", "json_data"];
//            var jsonTypes = data.types;

//            var newviewid = data.newviewid;
//            if (false === Csw.isNullOrEmpty(newviewid) && o.viewid !== newviewid) {
//                o.viewid = newviewid;
//                if (Csw.isFunction(o.onViewChange)) {
//                    o.onViewChange(o.viewid, o.viewmode);
//                }
//            }

//            var treeThemes = { "dots": true };
//            if (o.viewmode === Csw.enums.viewMode.list.name) {
//                treeThemes = { "dots": false };
//            }

//            treeDiv.$.jstree({
//                "json_data": {
//                    "data": data.root
//                },
//                "core": {
//                    "open_parents": true
//                },
//                "ui": {
//                    "select_limit": 1,
//                    "selected_parent_close": false,
//                    "selected_parent_open": true
//                },
//                "themes": treeThemes,
//                "types": {
//                    "types": jsonTypes,
//                    "max_children": -2,
//                    "max_depth": -2
//                },
//                "plugins": treePlugins
//            }); // jstree()

//            treeDiv.bind('select_node.jstree', function (e, newData) {
//                return firstSelectNode({
//                    e: e,
//                    data: newData,
//                    url: url,
//                    $treediv: treeDiv.$,
//                    IdPrefix: treeDiv.$.CswAttrDom('id'),
//                    onSelectNode: o.onSelectNode,
//                    onInitialSelectNode: o.onInitialSelectNode,
//                    viewid: o.viewid,
//                    //UsePaging: o.UsePaging,
//                    forsearch: o.forsearch
//                });

//            }).bind('hover_node.jstree', function (e, bindData) {
//                var $hoverLI = $(bindData.rslt.obj[0]);
//                //var nodeid = $hoverLI.CswAttrDom('id').substring(idPrefix.length);
//                var nodeid = $hoverLI.CswAttrNonDom('nodeid');
//                var cswnbtnodekey = $hoverLI.CswAttrNonDom('cswnbtnodekey');
//                Csw.nodeHoverIn(bindData.args[1], nodeid, cswnbtnodekey);

//            }).bind('dehover_node.jstree', function () {
//                Csw.jsTreeGetSelected(treeDiv.$);
//                Csw.nodeHoverOut();
//            });

//            treeDiv.$.jstree('select_node', Csw.tryParseElement(data.selectid));
//            //setTimeout(function () { Csw.debug.log('select: #' + data.selectid);  }, 1000);
//            rootnode = treeDiv.find('li').first();

//            if (Csw.bool(o.ShowCheckboxes)) {

//                treeDiv.$.find('li').each(function () {
//                    var $childObj = $(this);
//                    var thisid = Csw.string($childObj.CswAttrDom('id'));
//                    var thiskey = Csw.string($childObj.CswAttrDom('cswnbtnodekey'));
//                    var thisnodeid = Csw.string($childObj.CswAttrNonDom('nodeid'), thisid.substring(idPrefix.length));
//                    var thisrel = Csw.string($childObj.CswAttrNonDom('rel'));
//                    var altName = Csw.string($childObj.find('a').first().text());
//                    var thisnodename = Csw.string($childObj.CswAttrNonDom('nodename'), altName).trim();

//                    var $cb = $('<input type="checkbox" class="' + idPrefix + 'check" id="check_' + thisid + '" rel="' + thisrel + '" nodeid="' + thisnodeid + '" nodename="' + thisnodename + '" cswnbtnodekey="' + thiskey + '"></input>');
//                    $cb.prependTo($childObj);
//                    if (o.ValidateCheckboxes) {
//                        $cb.click(function () { return validateCheck(treeDiv.$, $cb); });
//                    }
//                });


//            }

//            if (o.ShowToggleLink && toggleLink) {
//                toggleLink.show();
//            }
//            // DO NOT define an onSuccess() function here that interacts with the tree.
//            // The tree has initalization events that appear to happen asynchronously,
//            // and thus having an onSuccess() function that changes the selected node will
//            // cause a race condition.
//        }

//        return { rootnode: rootnode, treeDiv: treeDiv };
//    } // makeTree()

//    function firstSelectNode(myoptions) {
//        var m = {
//            e: '',
//            data: '',
//            url: '',
//            $treediv: '',
//            IdPrefix: '',
//            onSelectNode: null, //function () {},
//            onInitialSelectNode: null, //function () {},
//            viewid: '',
//            //UsePaging: '',
//            forsearch: ''
//        };
//        if (myoptions) Csw.extend(m, myoptions);

//        // case 21715 - don't trigger onSelectNode event on first event
//        var m2 = {};
//        Csw.extend(m2, m);
//        m2.onSelectNode = m.onInitialSelectNode;
//        handleSelectNode(m2);

//        // rebind event for next select
//        m.$treediv.unbind('select_node.jstree');
//        m.$treediv.bind('select_node.jstree', function () { return handleSelectNode(m); });
//    }

//    function handleSelectNode(myoptions) {
//        var m = {
//            e: '',
//            data: '',
//            url: '',
//            $treediv: '',
//            IdPrefix: '',
//            onSelectNode: function () { },
//            viewid: '',
//            //UsePaging: '',
//            forsearch: ''
//        };
//        if (myoptions) Csw.extend(m, myoptions);

//        var selected = Csw.jsTreeGetSelected(m.$treediv);
//        var optSelect = {
//            nodeid: selected.$item.CswAttrNonDom('nodeid'),
//            nodename: selected.text,
//            iconurl: selected.iconurl,
//            cswnbtnodekey: selected.$item.CswAttrNonDom('cswnbtnodekey'),
//            nodespecies: selected.$item.CswAttrNonDom('species'),
//            viewid: m.viewid
//        };

//        clearChecks(m.IdPrefix);
//        Csw.tryExec(m.onSelectNode, optSelect);
//    }

//    function validateCheck($treediv, $checkbox) {
//        var $selected = Csw.jsTreeGetSelected($treediv);
//        return ($selected.$item.CswAttrNonDom('rel') === $checkbox.CswAttrNonDom('rel'));
//    }

//    function clearChecks(IdPrefix) {
//        $('.' + IdPrefix + 'check').CswAttrDom('checked', '');
//    }

//    // Method calling logic
//    $.fn.CswNodeTree = function (method) {

//        if (methods[method]) {
//            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
//        } else if (typeof method === 'object' || !method) {
//            return methods.init.apply(this, arguments);
//        } else {
//            $.error('Method ' + method + ' does not exist on ' + pluginName);
//        }

//    };

//})(jQuery);