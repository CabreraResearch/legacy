/// <reference path="../js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var PluginName = 'CswFieldTypeAuditHistoryGrid';

    var methods = {
        init: function(o) {

			var $Div = $(this);
			$Div.contents().remove();

			var gridJson = $.parseJSON(o.$propxml.text());

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
							nodeid: o.nodeid,
							onEditNode: o.onEditNode
						};
						if (rowid !== null) 
						{
							editOpt.date = $grid.jqGrid('getCell', rowid, 'CHANGEDATE');
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
			$grid.jqGrid(gridJson);
				//.hideCol('NODEIDSTR');


        },
        save: function(o) {
                
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeAuditHistoryGrid = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
