/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../_Global.js" />

;  (function ($) { /// <param name="$" type="jQuery" />

	$.fn.CswInspectionStatus = function (options) 
	{
		var o = {
			Url: '/NbtWebApp/wsNBT.asmx/getInspectionStatusGrid',
			onEditNode: function() {}
		};
		if(options) $.extend(o, options);

		var $parent = $(this);
		var $div = $('<div></div>')
					.appendTo($parent);

		
		CswAjaxJson({
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
						'datatype': 'local', 
						'height': 180,
						'pager': $gridPager,
						'emptyrecords': 'No Results',
						'loadtext': 'Loading...',
						'multiselect': false,
						'rowList': [10,25,50],  
						'rowNum': 10
					} 
					
					var optNav = {
						'add': false,
						'view': false,
						'del': false,
						'refresh': false,

						'edit': true,
						'edittext': "",
						'edittitle': "Edit row",
						'editfunc': function(rowid) 
							{
								var editOpt = {
									nodeid: '',
									onEditNode: o.onEditNode
								};
								if (rowid !== null) 
								{
									editOpt.nodeid = $grid.jqGrid('getCell', rowid, 'NODEIDSTR');
									$.CswDialog('EditNodeDialog', editOpt);
								}
								else
								{
									alert('Please select a row to edit');
								}
							}
					};
					$.extend(gridJson, mygridopts);

					$grid.jqGrid(gridJson)
						 .navGrid('#'+$gridPager.CswAttrDom('id'), optNav, {}, {}, {}, {}, {} ); 
					$grid.jqGrid(gridJson)
						.hideCol('NODEID')
						.hideCol('NODEIDSTR');

				}, // success
			'error': function() 
				{
				}
		});

		return $div;

	} // $.fn.CswInspectionStatus
}) (jQuery);

