/// <reference path="../js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var PluginName = 'CswAuditHistoryGrid';

    var methods = {
        init: function(options) {
			var o = {
				Url: '/NbtWebApp/wsNBT.asmx/getAuditHistoryGrid',
				ID: '',
				nodeid: '',
				JustDateColumn: false,
				onEditRow: function(date) {},
				onSelectRow: function(date) {},
				selectedDate: '',
				allowEditRow: true
			};
			if(options) $.extend(o, options);

			var $Div = $(this);
			$Div.contents().remove();

			CswAjaxJSON({
				'url': o.Url,
				'data': {
					'NodeId': o.nodeid, 
					'JustDateColumn': o.JustDateColumn 
				},
				'success': function(gridJson) {
				
					var $gridPager = $('<div id="' + o.ID + '_gp" style="width:100%; height:20px;" />')
										.appendTo($Div);
					var $grid = $('<table id="'+ o.ID + '_gt" />')
										.appendTo($Div);

					var mygridopts = {
						'autowidth': false,
						'datatype': 'local', 
						'height': 180,
						'width': '100%',
						'pager': $gridPager,
						'emptyrecords': 'No Results',
						'loadtext': 'Loading...',
						'multiselect': false,
						'rowList': [10,25,50],  
						'rowNum': 10,
						'onSelectRow': function(rowid, selected)
						{
							if (!PreventSelectTrigger && rowid !== null) 
							{
								o.onSelectRow($grid.jqGrid('getCell', rowid, 'CHANGEDATE'));
							}
						}
					};
					$.extend(gridJson, mygridopts);

					var optNav = {
						'add': false,
						'view': false,
						'del': false,
						'refresh': false,
						'search': false,

						'edit': o.allowEditRow,
						'edittext': "",
						'edittitle': "Edit row",
						'editfunc': function(rowid) 
							{
								if (rowid !== null) 
								{
									o.onEditRow($grid.jqGrid('getCell', rowid, 'CHANGEDATE'));
								}
								else
								{
									alert('Please select a row to edit');
								}
							}
					};

					$grid.jqGrid(gridJson)
							.navGrid('#'+$gridPager.CswAttrDom('id'), optNav, {}, {}, {}, {}, {} ); 

					// set selected row by date
					var PreventSelectTrigger = false;
					if(!isNullOrEmpty(o.selectedDate))
					{
						var dates = $grid.jqGrid('getCol', 'CHANGEDATE', true);
						var rowid = 0;
						for(var i in dates)
						{
							if(dates[i].value.toString() === o.selectedDate.toString())
								rowid = dates[i].id;
						}					
						PreventSelectTrigger = true;
						$grid.setSelection(rowid);
						PreventSelectTrigger = false;
					}

					// now bind the select event
					$grid.jqGrid({
					});

				}
			});


        }
    };
    
    // Method calling logic
    $.fn.CswAuditHistoryGrid = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
