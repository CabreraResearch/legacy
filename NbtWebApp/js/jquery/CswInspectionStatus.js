/// <reference path="../js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />

;  (function ($) { /// <param name="$" type="jQuery" />

	$.fn.CswInspectionStatus = function (options) 
	{
		var o = {
			Url: '/NbtWebApp/wsNBT.asmx/getInspectionStatusGrid'
		};
		if(options) $.extend(o, options);

		var $parent = $(this);
		var $div = $('<div></div>')
					.appendTo($parent);

		
		CswAjaxJSON({
			'url': o.Url,
			'data': {},
			'success': function(gridJson) 
				{
					$div.empty();
					var $gridPager = $('<div id="' + o.ID + '_gp" style="width:100%; height:20px;" />')
										.appendTo($div);
					var $grid = $('<table id="'+ o.ID + '_gt" />')
										.appendTo($div);

					var mygridopts = {
						'autowidth': true,
						'height': 180,
						'pager': $gridPager
					};
					$.extend(gridJson, mygridopts);

					$grid.jqGrid(gridJson)
						.hideCol('NODEIDSTR');

				}, // success
			'error': function() 
				{
				}
		});

		return $div;

	} // $.fn.CswInspectionStatus
}) (jQuery);

