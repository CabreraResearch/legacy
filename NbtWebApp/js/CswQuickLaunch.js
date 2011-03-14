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
					
					var $item = $(this);
					var optSelect = {
							type: $item.attr('type'),
							viewmode: $item.attr('viewmode'),
							text: $item.attr('text'), 
							url: $item.attr('url'),
							viewid: $item.attr('itemid') //actions provide their own links. itemid will only be used as viewid.
					};
							
					switch(optSelect.type) //webservice converts to lower case
					{
						case 'view':
						
							$('<li><a href="#' + optSelect.text + '_' + optSelect.type + '_' + optSelect.viewmode + '_' + optSelect.viewid +'">' + optSelect.text + '</a></li>')
								 .appendTo($list) 
								 .children('a')
								 .click(function() { o.onLinkClick(optSelect); return false; });
							break;
						case 'action': 
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


