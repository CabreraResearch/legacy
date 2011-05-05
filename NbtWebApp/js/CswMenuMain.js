/// <reference path="../jquery/jquery-1.6-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="_Global.js" />

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
			onAddNode: function(nodeid, cswnbtnodekey) { },
			onMultiEdit: function() { },
            onSearch: { onViewSearch: function() {}, onGenericSearch: function() {} },
			onEditView: function(viewid) { },
			Multi: false,
			NodeCheckTreeId: ''
		};
		if (options) $.extend(o, options);

		var $MenuDiv = $(this);

		CswAjaxXml({
			url: o.Url,
			data: "ViewNum=" + o.viewid + "&SafeNodeKey=" + o.cswnbtnodekey,
			success: function ($xml) {
				var $ul = $('<ul class="topnav"></ul>');

				$MenuDiv.text('')
						.append($ul);

				$xml.children().each(function() {
					var $this = $(this);
					if($this.CswAttrXml('text') !== undefined)
					{
						var menuItemOpts = { 
							'$ul': $ul, 
							'$itemxml': $this, 
							'onAlterNode': o.onAddNode, 
							'onMultiEdit': o.onMultiEdit, 
							'onEditView': o.onEditView,
							'onSearch': o.onSearch,
							'Multi': o.Multi, 
							'NodeCheckTreeId': o.NodeCheckTreeId 
						};
						var $li = HandleMenuItem(menuItemOpts);
						
						if($this.children().length >= 1) {
							var $subul = $('<ul class="subnav"></ul>')
											.appendTo($li);
							$this.children().each(function() {
								var subMenuItemOpts = {
									'$ul': $subul, 
									'$itemxml': $(this)
								};
								$.extend(menuItemOpts, subMenuItemOpts);
								HandleMenuItem(menuItemOpts);
							});
						}
					}

				});

				$ul.CswMenu();

			} // success{}
		}); // $.ajax({

		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);

