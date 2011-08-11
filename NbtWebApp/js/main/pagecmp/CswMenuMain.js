/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />

; (function ($) { /// <param name="$" type="jQuery" />
	
	$.fn.CswMenuMain = function (options) {
	/// <summary>
	///   Generates an action menu for the current view
	/// </summary>
	/// <param name="options" type="Object">
	///     A JSON Object
	///     &#10;1 - options.viewid: a viewid
	///     &#10;2 - options.nodeid: nodeid
	///     &#10;3 - options.cswnbtnodekey: a node key
	///     &#10;4 - options.onAddNode: function() {}
	///     &#10;5 - options.onMultiEdit: function() {}
	///     &#10;6 - options.onSearch: { onViewSearch: function() {}, onGenericSearch: function() {} }
	///     &#10;7 - options.onEditView: function() {}
	/// </param>
		var o = {
			Url: '/NbtWebApp/wsNBT.asmx/getMainMenu',
			viewid: '',
			nodeid: '',
			cswnbtnodekey: '',
			propid: '',
			onAddNode: null, // function(nodeid, cswnbtnodekey) { },
			onMultiEdit: null, // function() { },
			onSearch: {
			     onViewSearch: null, // function() {}, 
			     onGenericSearch: null // function() {}
		    },
			onEditView: null, // function(viewid) { },
			onSaveView: null, // function(newviewid) { },
			Multi: false,
			NodeCheckTreeId: ''
		};
		if (options) $.extend(o, options);

		var $MenuDiv = $(this);

		var dataXml = {
			ViewId: o.viewid,
			SafeNodeKey: o.cswnbtnodekey,
			PropIdAttr: o.propid
		};

		CswAjaxJson({
			url: o.Url,
			data: dataXml,
			stringify: false,
			success: function (data) {
			    var $ul = $('<ul class="topnav"></ul>');

			    $MenuDiv.text('')
    			    .append($ul);

			    for (var itemKey in data) {
			        if (data.hasOwnProperty(itemKey)) {

			            var menuItem = data[itemKey];
			            if (!isNullOrEmpty(itemKey))
			            {
			                var menuItemOpts = {
			                    $ul: $ul,
			                    itemKey: itemKey,
			                    itemJson: menuItem,
			                    onAlterNode: o.onAddNode,
			                    onMultiEdit: o.onMultiEdit,
			                    onEditView: o.onEditView,
			                    onSaveView: o.onSaveView,
			                    onSearch: o.onSearch,
			                    Multi: o.Multi,
			                    NodeCheckTreeId: o.NodeCheckTreeId
			                };
			                var $li = HandleMenuItem(menuItemOpts);

			                if (isTrue(menuItem.haschildren)) {
			                    delete menuItem.haschildren;
			                    var $subul = $('<ul class="subnav"></ul>')
    			                    .appendTo($li);
			                    for (var childItem in menuItem) {
			                        if (menuItem.hasOwnProperty(childItem)) {
			                            var thisChild = menuItem[childItem];
			                            var subMenuItemOpts = {
			                                $ul: $subul,
			                                itemKey: childItem,
			                                itemJson: thisChild
			                            };
			                            $.extend(menuItemOpts, subMenuItemOpts);
			                            HandleMenuItem(menuItemOpts);
			                        }
			                    }
			                }
			            }
			        }
			    }

			    $ul.CswMenu();

			} // success{}
		}); // $.ajax({

		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);

