/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) { 
    "use strict";

    var pluginName = 'CswNodeTree';

    var methods = {
        'init': function (options) {
        
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
                onSelectNode: null, // function (optSelect) { var o =  { nodeid: '',  nodename: '', iconurl: '', cswnbtnodekey: '', viewid: '' }; return o; },
                onInitialSelectNode: undefined,
                onViewChange: null, // function (newviewid, newviewmode) {},    // if the server returns a different view than what we asked for (e.g. case 21262)
                //SelectFirstChild: true,
                ShowCheckboxes: false,
                ShowToggleLink: true,
                IncludeInQuickLaunch: true,
                //Delay: 250,
                DefaultSelect: Csw.enums.nodeTree_DefaultSelect.firstchild.name
            };
            if (options) $.extend(o, options);

            if (false === Csw.isFunction(o.onInitialSelectNode)) {
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
                IdPrefix: Csw.string(idPrefix),
                // IsFirstLoad: true,
                // ParentNodeKey: '',
                IncludeNodeRequired: o.IncludeNodeRequired,
                IncludeNodeKey: Csw.string(o.cswnbtnodekey),
                IncludeNodeId: Csw.string(o.nodeid),
                // ShowEmpty: o.showempty,
                // ForSearch: o.forsearch,
                // NodePk: Csw.string(o.nodeid),
                IncludeInQuickLaunch: o.IncludeInQuickLaunch,
                DefaultSelect: o.DefaultSelect
            };

            if (Csw.isNullOrEmpty(o.viewid)) {
                url = o.NodeTreeUrl;
            }

            Csw.ajax.post({
                url: url,
                data: dataParam,
                stringify: false,
                success: function (data) {

                    var treePlugins = ["themes", "ui", "types", "crrm", "json_data"];
                    var jsonTypes = data.types;

                    var newviewid = data.newviewid;
                    if (false === Csw.isNullOrEmpty(newviewid) && o.viewid !== newviewid) {
                        o.viewid = newviewid;
                        if (Csw.isFunction(o.onViewChange)) {
                            o.onViewChange(o.viewid, o.viewmode);
                        }
                    }

                    var treeThemes = { "dots": true };
                    if (o.viewmode === Csw.enums.viewMode.list.name) {
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
                        //var nodeid = $hoverLI.CswAttrDom('id').substring(idPrefix.length);
                        var nodeid = $hoverLI.CswAttrNonDom('nodeid');
                        var cswnbtnodekey = $hoverLI.CswAttrNonDom('cswnbtnodekey');
                        Csw.nodeHoverIn(bindData.args[1], nodeid, cswnbtnodekey);

                    }).bind('dehover_node.jstree', function () {
                        Csw.jsTreeGetSelected($treediv);
                        Csw.nodeHoverOut();
                    });

                    $treediv.jstree('select_node', Csw.controls.dom.tryParseElement(data.selectid));
                    //setTimeout(function () { Csw.log('select: #' + data.selectid);  }, 1000);
                    rootnode = $treediv.find('li').first();

                    if (Csw.bool(o.ShowCheckboxes)) {

                        $treediv.find('li').each(function () {
                            var $childObj = $(this);
                            var thisid = Csw.string($childObj.CswAttrDom('id'));
                            var thisnodeid = Csw.string($childObj.CswAttrNonDom('nodeid'), thisid.substring(idPrefix.length));
                            var thisrel = Csw.string($childObj.CswAttrNonDom('rel'));
                            var altName = Csw.string($childObj.find('a').first().text());
                            var thisnodename = Csw.string($childObj.CswAttrNonDom('nodename'), altName).trim();
                            $('<input type="checkbox" class="' + idPrefix + 'check" id="check_' + thisid + '" rel="' + thisrel + '" nodeid="' + thisnodeid + '" nodename="' + thisnodename + '"></input>')
                                .prependTo($childObj)
                                .click(function () { return handleCheck($treediv, $(this)); });

                        });


                    }

                    if (o.ShowToggleLink && $togglelink) {
                        $togglelink.show();
                    }
                    // DO NOT define an onSuccess() function here that interacts with the tree.
                    // The tree has initalization events that appear to happen asynchronously,
                    // and thus having an onSuccess() function that changes the selected node will
                    // cause a race condition.

                    //                    rootnode = addNodeToTree($treediv, 1, false, data.root);
                    //                    if (Csw.bool(data.result)) {
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
            $treediv.jstree('select_node', '#' + idPrefix + o.newcswnbtnodekey);
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
            onSelectNode: null, //function () {},
            onInitialSelectNode: null, //function () {},
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

        var selected = Csw.jsTreeGetSelected(m.$treediv);
        var optSelect = {
            nodeid: selected.$item.CswAttrNonDom('nodeid'),
            nodename: selected.text,
            iconurl: selected.iconurl,
            cswnbtnodekey: selected.$item.CswAttrNonDom('cswnbtnodekey'),
            nodespecies: selected.$item.CswAttrNonDom('species'),
            viewid: m.viewid
        };

        clearChecks(m.IdPrefix);
        m.onSelectNode(optSelect);
    }

    function handleCheck($treediv, $checkbox) {
        var $selected = Csw.jsTreeGetSelected($treediv);
        return ($selected.$item.CswAttrNonDom('rel') === $checkbox.CswAttrNonDom('rel'));
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