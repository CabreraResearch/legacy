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
	    };

		CswAjaxJson({
			url: o.Url,
			data: dataXml,
			stringify: false,
			success: function (data) {
				var $QuickLaunchDiv = $('<div id="quicklaunchdiv"><ul id="launchitems"></ul></div>')
									.appendTo($this);
				var $list = $QuickLaunchDiv.children();
				for (var item in data) {
				    if (data.hasOwnProperty(item)) {
				        var qlItem = data[item];
				        var launchtype = tryParseString(qlItem.launchtype);
				        var viewmode = tryParseString(qlItem.viewmode);
				        var text = tryParseString(qlItem.text);
				        var viewid = tryParseString(qlItem.itemid); //actions provide their own links. itemid will only be used as viewid.
				        var actionname = tryParseString(qlItem.actionname);
				        var actionurl = tryParseString(qlItem.actionurl);

				        var $li = $('<li></li>')
    				        .appendTo($list);

				        switch (launchtype.toLowerCase()) //webservice converts to lower case
				        {
				            case 'view':
				                $('<a href="#' + text + '_' + launchtype + '_' + viewmode + '_' + viewid + '">' + text + '</a>')
    				                .appendTo($li)
    				                .click(function() { o.onViewClick(viewid, viewmode); return false; });
				                break;
				            case 'action':
				                $('<a href="#">' + text + '</a>')
    				                .appendTo($li)
    				                .click(function() { o.onActionClick(actionname, actionurl); return false; });
				                break;
				        }
				    }
				}

			    o.onSuccess();

			} // success{}
		});

		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);


