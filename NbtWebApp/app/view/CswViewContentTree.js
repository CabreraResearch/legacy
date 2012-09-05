
/// <reference path="~app/CswApp-vsdoc.js" />

(function ($) { 
    "use strict";

    // This was extracted from CswViewEditor

    $.fn.CswViewContentTree = function (options) {
        var o = {
            ViewInfoUrl: '/NbtWebApp/wsNBT.asmx/getViewInfo',
            viewid: ''
        };
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
            types.root = { icon: { image: Csw.string(itemJson.iconfilename)} };
            var linkclass = Csw.enums.cssClasses_ViewEdit.vieweditor_viewrootlink.name;

            var $ret = makeViewListItem(arbid, linkclass, name, false, Csw.enums.viewChildPropNames.root, rel);

            if (itemJson.hasOwnProperty(Csw.enums.viewChildPropNames.childrelationships.name)) {
                var rootRelationships = itemJson[Csw.enums.viewChildPropNames.childrelationships.name];
                makeViewRelationshipsRecursive(rootRelationships, types, $ret);
            }

            return $ret;
        }

        function makeViewRelationshipHtml(itemJson, types) {
            var arbid = itemJson.arbitraryid;
            var name = itemJson.secondname;
            var propname = Csw.string(itemJson.propname);
            if (!Csw.isNullOrEmpty(propname)) {
                if (itemJson.propowner === "First") {
                    name += " (by " + itemJson.firstname + "'s " + propname + ")";
                } else {
                    name += " (by " + propname + ")";
                }
            }
            var rel = Csw.string(itemJson.secondtype) + '_' + Csw.string(itemJson.secondid);
            var linkclass = Csw.enums.cssClasses_ViewEdit.vieweditor_viewrellink.name;
            types[rel] = { icon: { image: Csw.string(itemJson.secondiconfilename)} };

            var $ret = makeViewListItem(arbid, linkclass, name, Csw.enums.viewChildPropNames.childrelationships, rel);

            if (itemJson.hasOwnProperty(Csw.enums.viewChildPropNames.properties.name)) {
                var propJson = itemJson[Csw.enums.viewChildPropNames.properties.name];
                if (!Csw.isNullOrEmpty(propJson)) {
                    var $propUl = $('<ul></ul>');
                    for (var prop in propJson) {
                        if (propJson.hasOwnProperty(prop)) {
                            var thisProp = propJson[prop];
                            if (false === Csw.isNullOrEmpty(thisProp)) {
                                var $propLi = makeViewPropertyHtml(thisProp, types);
                                if (false === Csw.isNullOrEmpty($propLi)) {
                                    $propUl.append($propLi);
                                }
                            }
                        }
                    }
                    if ($propUl.children().length > 0) {
                        $ret.append($propUl);
                    }
                }
            }
            return $ret;
        }

        function makeViewRelationshipsRecursive(relationshipJson, types, $content) {
            if (!Csw.isNullOrEmpty(relationshipJson)) {
                var $ul = $('<ul></ul>');
                for (var relationship in relationshipJson) {
                    if (relationshipJson.hasOwnProperty(relationship)) {
                        var thisRelationship = relationshipJson[relationship];
                        var $rel = makeViewRelationshipHtml(thisRelationship, types);
                        if (false === Csw.isNullOrEmpty($rel)) {
                            $ul.append($rel);
                        }
                        if (thisRelationship.hasOwnProperty(Csw.enums.viewChildPropNames.childrelationships.name)) {
                            var childRelationships = thisRelationship[Csw.enums.viewChildPropNames.childrelationships.name];
                            makeViewRelationshipsRecursive(childRelationships, types, $rel);
                        }
                    }
                }
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
                $ret = makeViewListItem(arbid, linkclass, name, Csw.enums.viewChildPropNames.properties, rel);
            }
            if (!Csw.isNullOrEmpty($ret)) {
                var $filtUl = $('<ul></ul>');
                if (itemJson.hasOwnProperty(Csw.enums.viewChildPropNames.propfilters.name)) {
                    var filterJson = itemJson[Csw.enums.viewChildPropNames.propfilters.name];
                    if (!Csw.isNullOrEmpty(filterJson)) {
                        for (var filter in filterJson) {
                            if (filterJson.hasOwnProperty(filter)) {
                                var thisFilt = filterJson[filter];
                                if (false === Csw.isNullOrEmpty(thisFilt)) {
                                    var $filtLi = makeViewPropertyFilterHtml(thisFilt, types, arbid);
                                    if (false === Csw.isNullOrEmpty($filtLi)) {
                                        $filtUl.append($filtLi);
                                    }
                                }
                            }
                        }
                    }
                }
                if ($filtUl.children().length > 0) {
                    $ret.append($filtUl);
                }
            }
            types.property = { icon: { image: "Images/view/property.gif"} };
            return $ret;
        }

        function makeViewPropertyFilterHtml(itemJson, types) {
            var $ret = $('<li></li>');
            var rel = 'filter';
            if (!Csw.isNullOrEmpty(itemJson)) {
                var filtArbitraryId = Csw.string(itemJson.arbitraryid);

                var selectedSubfield = Csw.string(itemJson.subfield, itemJson.subfieldname);
                var selectedFilterMode = Csw.string(itemJson.filtermode);
                var filterValue = Csw.string(itemJson.value);
                var name = selectedSubfield + ' ' + selectedFilterMode + ' ' + filterValue;
                var $filtLink = makeViewListItem(filtArbitraryId, Csw.enums.cssClasses_ViewEdit.vieweditor_viewfilterlink.name, name, false, Csw.enums.viewChildPropNames.filters, rel);
                if (false === Csw.isNullOrEmpty($filtLink)) {
                    $ret = $filtLink;
                }
            }

            types.filter = { icon: { image: "Images/view/filter.gif"} };
            return $ret;
        }

        function makeViewListItem(arbid, linkclass, name, propName, rel) {
            var $ret = $('<li id="' + arbid + '" rel="' + rel + '" class="jstree-open"></li>');
            $ret.append($('<a href="#" class="' + linkclass + '" arbid="' + arbid + '">' + name + '</a>'));
            return $ret;
        }

        var $tree = $(this);

        Csw.ajax.post({
            url: o.ViewInfoUrl,
            data: { ViewId: o.viewid },
            success: function (data) {
                var viewJson = data.TreeView;

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
                }); // tree
            } // success
        }); // ajax

        return $tree;

    }; // $.fn.CswViewContentTree
})(jQuery);

