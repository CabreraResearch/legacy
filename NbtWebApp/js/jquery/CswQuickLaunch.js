/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../_Global.js" />

; (function ($) {
	$.fn.CswQuickLaunch = function (options) {

		var o = {
			Url: '/NbtWebApp/wsNBT.asmx/getQuickLaunchItems',
			onViewClick: function(viewid, viewmode) { },
			onActionClick: function(actionname, actionurl) { },
			onSuccess: function() { }
		};

		if (options) {
			$.extend(o, options);
		}
		var $this = $(this);

		var dataXml = {
			UserId: ''
		}

		CswAjaxXml({
			url: o.Url,
			data: dataXml,
			stringify: false,
			success: function ($xml) {
				var $QuickLaunchDiv = $('<div id="quicklaunchdiv"><ul id="launchitems"></ul></div>')
									.appendTo($this);
				var $list = $QuickLaunchDiv.children();
				$xml.children("items").children("item").each(function() {
					var $item = $(this);

					var launchtype = $item.CswAttrXml('launchtype');
					var viewmode = $item.CswAttrXml('viewmode');
					var text = $item.CswAttrXml('text'); 
					var viewid = $item.CswAttrXml('itemid'); //actions provide their own links. itemid will only be used as viewid.
					var actionname = $item.CswAttrXml('actionname');
					var actionurl = $item.CswAttrXml('actionurl');
							
					var $li = $('<li></li>')
								.appendTo($list);

					switch(launchtype.toLowerCase()) //webservice converts to lower case
					{
						case 'view':
							$('<a href="#' + text + '_' + launchtype + '_' + viewmode + '_' + viewid +'">' + text + '</a>')
								 .appendTo($li) 
								 .click(function() { o.onViewClick(viewid, viewmode); return false; });
							break;
						case 'action': 
							$('<a href="#">' + text + '</a>') 
								.appendTo($li)
								.click(function() { o.onActionClick(actionname, actionurl); return false; });
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


