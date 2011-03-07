; (function ($) {
	$.fn.CswQuickLaunch = function (options) {

		var o = {
			Url: '/NbtWebApp/wsNBT.asmx/getQuickLaunchItems',
			onLinkClick: function(optSelect) { } //viewid, actionid
		};

		if (options) {
			$.extend(o, options);
		}
		var $this = $(this);

		CswAjaxXml({
			url: o.Url,
			data: "{ UserId: '' }",
			success: function ($xml) {
				var $QuickLaunchDiv = $('<div id="quicklaunchdiv"><ul id="launchitems"></ul></div>')
									.appendTo($this);
				var $list = $QuickLaunchDiv.children();
				$xml.children("items").children("item").each(function() {
					
					if(debug)
					{
					}

					var $item = $(this);
					var optSelect = {
							type: $item.attr('type'),
							viewmode: $item.attr('viewmode'),
							text: $item.attr('text'), 
							url: $item.attr('url'),
							viewid: $item.attr('itemid') //actions provide their own links. itemid will only be used as viewid.
					};
							
					switch(optSelect.type)
					{
						case 'View':
						
							$('<li><a href="#' + optSelect.text + '_' + optSelect.type + optSelect.viewid +'">' + optSelect.text + '</a></li>')
								 .appendTo($list) 
								 .children('a')
								 .click(function() { o.onLinkClick(optSelect); return false; });
							break;
						case 'Action': 
							$('<li><a href=' + optSelect.url + '>' + optSelect.text + '</a></li>') 
								.appendTo($list);
							break;
					}
				});
				
			} // success{}
		});

		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);


