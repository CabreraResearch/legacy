; (function ($) {
	$.fn.CswMenuMain = function (options) {

		var o = {
			Url: '/NbtWebApp/wsNBT.asmx/getMainMenu',
			viewid: '',
			nodeid: '',
			cswnbtnodekey: '',
			onAddNode: function(nodeid, cswnbtnodekey) { },
			onMultiEdit: function() { },
			onEditView: function(viewid) { },
			Multi: false,
			NodeCheckTreeId: ''
		};

		if (options) {
			$.extend(o, options);
		}

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
					if($this.attr('text') != undefined)
					{
						var menuItemOpts = { 
							'$ul': $ul, 
							'$itemxml': $this, 
							'onAlterNode': o.onAddNode, 
							'onMultiEdit': o.onMultiEdit, 
							'onEditView': o.onEditView,
							'Multi': o.Multi, 
							'NodeCheckTreeId': o.NodeCheckTreeId 
						}

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
								HandleMenuItem(subMenuItemOpts);
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

