
/// <reference path="~/app/CswApp-vsdoc.js" />

(function ($) {
    "use strict";

    // This was extracted from CswViewEditor

    $.fn.CswViewContentTree = function (options) {

        var o = {
            ViewInfoUrl: 'getViewInfo',
            viewid: '',
            viewstr: '',
            showDelete: false,
            onClick: function () { },
            onDeleteClick: function () { }
        };
        var deleteBtns = [];
        if (options) Csw.extend(o, options);

        function viewJsonHtml(viewJson) {
            var types = {};
            var $ret = $('<ul></ul>');
            /* Root */
            makeViewRootHtml(viewJson, types)
                        .appendTo($ret);

            return { html: $ret.html(), types: types };
        }

        function makeViewRootHtml(itemJson, types) {
            var arbid = 'root';
            var name = itemJson.viewname;
            var rel = 'root';
            types.root = { icon: { image: Csw.string(itemJson.iconfilename) } };
            var linkclass = Csw.enums.cssClasses_ViewEdit.vieweditor_viewrootlink.name;

            var $ret = makeViewListItem(arbid, linkclass, name, Csw.enums.viewChildPropNames.root, rel, false);

            if (itemJson && itemJson[Csw.enums.viewChildPropNames.childrelationships.name]) {
                var rootRelationships = itemJson[Csw.enums.viewChildPropNames.childrelationships.name];
                makeViewRelationshipsRecursive(rootRelationships, types, $ret);
            }

            return $ret;
        }

        function makeViewRelationshipHtml(itemJson, types) {
            var arbid = itemJson.arbitraryid;
            var name = itemJson.secondname;
            var propname = Csw.string(itemJson.propname);
            if (false === Csw.isNullOrEmpty(propname)) {
                if (itemJson.propowner === "First") {
                    name += " (by " + itemJson.firstname + "'s " + propname + ")";
                } else {
                    name += " (by " + propname + ")";
                }
            }
            var rel = Csw.string(itemJson.secondtype) + '_' + Csw.string(itemJson.secondid);
            var linkclass = Csw.enums.cssClasses_ViewEdit.vieweditor_viewrellink.name;
            types[rel] = { icon: { image: Csw.getIconUrlStem(16) + Csw.string(itemJson.secondiconfilename) } };

            var $ret = makeViewListItem(arbid, linkclass, name, Csw.enums.viewChildPropNames.childrelationships, rel, itemJson.showdelete);

            if (itemJson && itemJson[Csw.enums.viewChildPropNames.childrelationships.name]) {
                var propJson = itemJson[Csw.enums.viewChildPropNames.properties.name];
                if (false === Csw.isNullOrEmpty(propJson)) {
                    var $propUl = $('<ul></ul>');
                    Csw.iterate(propJson, function (thisProp) {
                        if (false === Csw.isNullOrEmpty(thisProp)) {
                            var $propLi = makeViewPropertyHtml(thisProp, types);
                            if (false === Csw.isNullOrEmpty($propLi)) {
                                $propUl.append($propLi);
                            }
                        }
                    });
                    if ($propUl.children().length > 0) {
                        $ret.append($propUl);
                    }
                }
            }
            return $ret;
        }

        function makeViewRelationshipsRecursive(relationshipJson, types, $content) {
            if (false === Csw.isNullOrEmpty(relationshipJson)) {
                var $ul = $('<ul></ul>');
                Csw.iterate(relationshipJson, function (thisRelationship) {
                    var $rel = makeViewRelationshipHtml(thisRelationship, types);
                    if (false === Csw.isNullOrEmpty($rel)) {
                        $ul.append($rel);
                    }
                    if (thisRelationship && thisRelationship[Csw.enums.viewChildPropNames.childrelationships.name]) {
                        var childRelationships = thisRelationship[Csw.enums.viewChildPropNames.childrelationships.name];
                        makeViewRelationshipsRecursive(childRelationships, types, $rel);
                    }
                });
                if ($ul.children().length > 0) {
                    $content.append($ul);
                }
            }
        }

        function makeViewPropertyHtml(itemJson, types) {
            var $ret = $('<li></li>');
            var arbid = itemJson.arbitraryid;
            var name = itemJson.name;
            var rel = 'property';
            var linkclass = Csw.enums.cssClasses_ViewEdit.vieweditor_viewproplink.name;
            if (false === Csw.isNullOrEmpty(name)) {
                $ret = makeViewListItem(arbid, linkclass, name, Csw.enums.viewChildPropNames.properties, rel, itemJson.showdelete);
            }
            if (false === Csw.isNullOrEmpty($ret)) {
                var $filtUl = $('<ul></ul>');
                var filterJson = itemJson[Csw.enums.viewChildPropNames.propfilters.name];
                if (false === Csw.isNullOrEmpty(filterJson)) {
                    Csw.iterate(filterJson, function (thisFilt) {
                        if (false === Csw.isNullOrEmpty(thisFilt)) {
                            var $filtLi = makeViewPropertyFilterHtml(thisFilt, types, arbid);
                            if (false === Csw.isNullOrEmpty($filtLi)) {
                                $filtUl.append($filtLi);
                            }
                        }
                    });
                }
                if ($filtUl.children().length > 0) {
                    $ret.append($filtUl);
                }
            }
            types.property = { icon: { image: "Images/view/property.gif" } };
            return $ret;
        }

        function makeViewPropertyFilterHtml(itemJson, types) {
            var $ret = $('<li></li>');
            var rel = 'filter';
            if (false === Csw.isNullOrEmpty(itemJson)) {
                var filtArbitraryId = Csw.string(itemJson.arbitraryid);

                var selectedSubfield = Csw.string(itemJson.subfield, itemJson.subfieldname);
                var selectedFilterMode = Csw.string(itemJson.filtermode);
                var filterValue = Csw.string(itemJson.value);
                var name = selectedSubfield + ' ' + selectedFilterMode + ' ' + filterValue;
                var $filtLink = makeViewListItem(filtArbitraryId, Csw.enums.cssClasses_ViewEdit.vieweditor_viewfilterlink.name, name, false, Csw.enums.viewChildPropNames.filters, rel, itemJson.showdelete);
                if (false === Csw.isNullOrEmpty($filtLink)) {
                    $ret = $filtLink;
                }
            }

            types.filter = { icon: { image: "Images/view/filter.gif" } };
            return $ret;
        }

        function makeViewListItem(arbid, linkclass, name, propName, rel, showDelete) {
            var $ret = $('<li id="' + arbid + '" rel="' + rel + '" class="jstree-open"></li>');
            $ret.append($('<a href="#" class="' + linkclass + '" arbid="' + arbid + '">' + name + '</a>'));
            if (showDelete) {
                makeDeleteSpan(arbid, $ret);
            }
            return $ret;
        }

        function makeDeleteSpan(arbid, $parent) {
            var td = Csw.literals.span({
                $parent: $parent,
                cssclass: Csw.enums.cssClasses_ViewEdit.vieweditor_deletespan.name
            }).propNonDom('arbid', arbid);

            td.icon({
                name: arbid + '_delete',
                iconType: Csw.enums.iconType.x,
                hovertext: 'Delete'
            }).css({ display: 'inline-block' });
            return td.$;
        }

        function bindDeleteBtns(stepno) {
            $('.' + Csw.enums.cssClasses_ViewEdit.vieweditor_deletespan.name).each(function () {
                var $span = $(this);
                var arbid = $span.CswAttrNonDom('arbid');
                var $btn = $span.children('div').first();
                $btn.bind('click', function () {
                    Csw.tryExec(o.onDeleteClick, arbid);
                });
            });
        }

        var $tree = $(this);

        Csw.ajax.post({
            urlMethod: o.ViewInfoUrl,
            data: {
                ViewId: o.viewid,
                ViewString: o.viewstr
            },
            success: function (data) {
                //var viewJson = data.view.TreeView; //TreeView is not longer the root of View.ToJSON()
                var viewJson = data.view;

                var treecontent = viewJsonHtml(viewJson);

                $tree.jstree({
                    "html_data":
                    {
                        "data": treecontent.html
                    },
                    "ui": {
                        "select_limit": 1 //,
                        //"initially_select": selectid,
                    },
                    "types": {
                        "types": treecontent.types,
                        "max_children": -2,
                        "max_depth": -2
                    },
                    "plugins": ["themes", "html_data", "ui", "types", "crrm"]
                }) // tree
                    .bind('select_node.jstree', function (node, ref_node) {
                        Csw.tryExec(o.onClick, node, ref_node);
                    });
                bindDeleteBtns();
            } // success
        }); // ajax

        return $tree;

    }; // $.fn.CswViewContentTree
})(jQuery);

