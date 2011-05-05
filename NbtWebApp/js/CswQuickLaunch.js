; (function ($) {
	$.fn.CswQuickLaunch = function (options) {

		var o = {
			Url: '/NbtWebApp/wsNBT.asmx/getQuickLaunchItems',
			onLinkClick: function(optSelect) { },
			onSuccess: function() { }
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
							launchtype: $item.CswAttrXml('launchtype'),
							viewmode: $item.CswAttrXml('viewmode'),
							text: $item.CswAttrXml('text'), 
							url: $item.CswAttrXml('url'),
							viewid: $item.CswAttrXml('itemid') //actions provide their own links. itemid will only be used as viewid.
					};
							
					switch(optSelect.launchtype) //webservice converts to lower case
					{
						case 'View':
                        case 'view':
						
							$('<li><a href="#' + optSelect.text + '_' + optSelect.type + '_' + optSelect.viewmode + '_' + optSelect.viewid +'">' + optSelect.text + '</a></li>')
								 .appendTo($list) 
								 .children('a')
								 .click(function() { o.onLinkClick(optSelect); return false; });
							break;
                        case 'Action':
						case 'action': 
							$('<li><a href=' + optSelect.url + '>' + optSelect.text + '</a></li>') 
								.appendTo($list);
							break;
					}
				});
				
				o.onSuccess();

			} // success{}
		});

		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);


