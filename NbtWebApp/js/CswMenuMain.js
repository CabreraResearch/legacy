; (function ($) {
	$.fn.CswMenuMain = function (options) {

		var o = {
			Url: '/NbtWebApp/wsNBT.asmx/getMainMenu',
			viewid: '',
			nodeid: '',
			cswnbtnodekey: '',
			onAddNode: function(newnodeid,newcswnbtnodekey) { }
		};

		if (options) {
			$.extend(o, options);
		}

		var $MenuDiv = $(this);

		CswAjaxXml({
			url: o.Url,
			data: "ViewId=" + o.viewid + "&NodePk=" + o.nodeid + "&NodeKey=" + o.cswnbtnodekey,
			success: function ($xml) {
				var $ul = $('<ul class="topnav"></ul>');

				$MenuDiv.text('')
						.append($ul);

				$xml.children().each(function() {
					var $this = $(this);
					if($this.attr('text') != undefined)
					{
						var $li = HandleMenuItem($ul, $this, null, o.onAddNode);
						
						if($this.children().length > 1) {
							var $subul = $('<ul class="subnav"></ul>')
											.appendTo($li);
							$this.children().each(function() {
								HandleMenuItem($subul, $(this), null, o.onAddNode);
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

