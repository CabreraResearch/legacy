/// <reference path="../js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />

;  (function ($) { /// <param name="$" type="jQuery" />

	$.fn.CswInspectionStatus = function (options) 
	{
		var o = {
		};
		if(options) $.extend(o, options);

		var $parent = $(this);
		var $div = $('<div></div>')
					.appendTo($parent);

		$div.text('This is Inspection Status');

		return $div;

	} // $.fn.CswInspectionStatus
}) (jQuery);

