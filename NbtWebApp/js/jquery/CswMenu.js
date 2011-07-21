/// <reference path="../js/thirdparty/jquery/core/jquery-1.6.2-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />

/* Adapted from http://www.noupe.com/tutorial/drop-down-menu-jquery-css.html */

; (function ($) { /// <param name="$" type="jQuery" />
	$.fn.CswMenu = function (options) {

		var o = {
		};

		if (options) {
			$.extend(o, options);
		}

		var $MenuUl = $(this);

		$MenuUl.children('li')
					  .click(TopMenuClick)
					  .hover(TopMenuClick, HideAllSubMenus);
		$MenuUl.find(".subnav").children('li').click(SubMenuClick);

		function TopMenuClick()
		{
			$this = $(this);

			HideAllSubMenus();

			// Show this subnav
			$this.find("ul.subnav")
					.slideDown('fast')
					.show();
		}

		function SubMenuClick(event)
		{
			HideAllSubMenus();
			// Prevent subnav elements from triggering topnav click
			if(event)
			{
				event.stopPropagation();
			}
		}
		
		function HideAllSubMenus()
		{
			 $MenuUl.find('ul').stop(true, true);
			 $MenuUl.find("ul.subnav").slideUp('fast');
		}


		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);
