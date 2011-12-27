/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../pagecmp/CswWizard.js" />
/// <reference path="CswViewPropFilter.js" />
/// <reference path="../controls/CswButton.js" />
/// <reference path="../controls/CswSelect.js" />
/// <reference path="../controls/CswGrid.js" />

(function ($) { /// <param name="$" type="jQuery" />


	// This was extracted from CswViewEditor

	$.fn.CswViewContentTree = function (options) {
		var o = {
			ViewInfoUrl: '/NbtWebApp/wsNBT.asmx/getViewInfo',
			viewid: ''
		};
		if (options) $.extend(o, options);

		function viewJsonHtml(viewJson) {
			var types = {};
			var $ret = $('<ul></ul>');
			var $root = makeViewRootHtml(viewJson, types)
						.appendTo($ret);

			return { html: $ret.html(), types: types };
		}

		function makeViewRootHtml(itemJson, types) {
			var arbid = 'root';
			var name = itemJson.viewname;
			var rel = 'root';
			types.root = { icon: { image: tryParseString(itemJson.iconfilename)} };
			var linkclass = viewEditClasses.vieweditor_viewrootlink.name;

			var $ret = makeViewListItem(arbid, linkclass, name, false, childPropNames.root, rel);

			if (itemJson.hasOwnProperty(childPropNames.childrelationships.name)) {
				var rootRelationships = itemJson[childPropNames.childrelationships.name];
				makeViewRelationshipsRecursive(rootRelationships, types, $ret);
			}

			return $ret;
		}

		function makeViewRelationshipHtml(itemJson, types) {
			var arbid = itemJson.arbitraryid;
			var name = itemJson.secondname;
			var propname = tryParseString(itemJson.propname);
			if (!isNullOrEmpty(propname)) {
				if (itemJson.propowner === "First") {
					name += " (by " + itemJson.firstname + "'s " + propname + ")";
				} else {
					name += " (by " + propname + ")";
				}
			}
			var rel = tryParseString(itemJson.secondtype) + '_' + tryParseString(itemJson.secondid);
			var linkclass = viewEditClasses.vieweditor_viewrellink.name;
			types[rel] = { icon: { image: tryParseString(itemJson.secondiconfilename)} };

			var $ret = makeViewListItem(arbid, linkclass, name, childPropNames.childrelationships, rel);

			if (itemJson.hasOwnProperty(childPropNames.properties.name)) {
				var propJson = itemJson[childPropNames.properties.name];
				if (!isNullOrEmpty(propJson)) {
					var $propUl = $('<ul></ul>');
					for (var prop in propJson) {
						if (propJson.hasOwnProperty(prop)) {
							var thisProp = propJson[prop];
							if (false === isNullOrEmpty(thisProp)) {
								var $propLi = makeViewPropertyHtml(thisProp, types);
								if (false === isNullOrEmpty($propLi)) {
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
			if (!isNullOrEmpty(relationshipJson)) {
				var $ul = $('<ul></ul>');
				for (var relationship in relationshipJson) {
					if (relationshipJson.hasOwnProperty(relationship)) {
						var thisRelationship = relationshipJson[relationship];
						var $rel = makeViewRelationshipHtml(thisRelationship, types);
						if (false === isNullOrEmpty($rel)) {
							$ul.append($rel);
						}
						if (thisRelationship.hasOwnProperty(childPropNames.childrelationships.name)) {
							var childRelationships = thisRelationship[childPropNames.childrelationships.name];
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
			var linkclass = viewEditClasses.vieweditor_viewproplink.name;
			if (false === isNullOrEmpty(name)) {
				$ret = makeViewListItem(arbid, linkclass, name, childPropNames.properties, rel);
			}
			if (!isNullOrEmpty($ret)) {
				var $filtUl = $('<ul></ul>');
				if (itemJson.hasOwnProperty(childPropNames.propfilters.name)) {
					var filterJson = itemJson[childPropNames.propfilters.name];
					if (!isNullOrEmpty(filterJson)) {
						for (var filter in filterJson) {
							if (filterJson.hasOwnProperty(filter)) {
								var thisFilt = filterJson[filter];
								if (false === isNullOrEmpty(thisFilt)) {
									var $filtLi = makeViewPropertyFilterHtml(thisFilt, types, arbid);
									if (false === isNullOrEmpty($filtLi)) {
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

		function makeViewPropertyFilterHtml(itemJson, types, propArbId) {
			var $ret = null;

			$ret = $('<li></li>');
			var rel = 'filter';
			if (!isNullOrEmpty(itemJson)) {
				var filtArbitraryId = tryParseString(itemJson.arbitraryid);

				var selectedSubfield = tryParseString(itemJson.subfield, itemJson.subfieldname);
				var selectedFilterMode = tryParseString(itemJson.filtermode);
				var filterValue = tryParseString(itemJson.value);
				var name = selectedSubfield + ' ' + selectedFilterMode + ' ' + filterValue;
				var $filtLink = makeViewListItem(filtArbitraryId, viewEditClasses.vieweditor_viewfilterlink.name, name, false, childPropNames.filters, rel);
				if (false === isNullOrEmpty($filtLink)) {
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

		CswAjaxJson({
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

