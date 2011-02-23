/// <reference path="../jquery/jquery-1.4.4.js" />

; (function ($) {
	$.fn.CswNodeGrid = function (options) {

		var o = {
			GridUrl: '/NbtWebApp/wsNBT.asmx/getGridJson',
			viewid: '',
			enableColumnReorder: true,
			editable: true,
			enableAddRow: true,
			enableCellNavigation: true,
			asyncEditorLoading: true,
			autoEdit: false,
			autoHeight: false,
			topPanelHeight: 25,
			forceFitColumns: false
			//,onSelectNode: function(nodeid) { }
		};
		
		if (options) {
			$.extend(o, options);
		}
		
		var gridData;
		var dataView;
		var grid;
		var pager;
		var columnpicker;

		var $gridOuter = $('<table id="gridOuter" />')
						.appendTo($(this));
		var $gridPager = $('<div id="gridPager" style="width:100%; height:20px;" />')
						 .appendTo($(this));

		CswAjaxJSON({
			url: o.GridUrl,
			data: "{ViewId: '" +  o.viewid + "'}",
			success: function (gridJson) {
					
					console.log(gridJson);
					
					gridData = gridJson.grid;
					gridRows = gridData.rows;
					console.log(gridData);

					var ViewName = gridJson.viewname;
					var columns = gridJson.columnnames;
					var columnDefinition = gridJson.columndefinition
					var gridWidth =  gridJson.viewwidth;
					if( gridWidth == '' )
					{
						widthgridWidth = 500;
					}

					jQuery($gridOuter).jqGrid({ 
						datatype: "local", 
						data: gridRows,
						colNames: columns,  
						colModel: columnDefinition, 
						height: 300,
						width: gridWidth,
						rowNum:10, 
						autoencode: true,
						autowidth: true, 
						rowList:[10,20,30], 
						pager: jQuery($gridPager), 
						sortname: 'id', 
						shrinkToFit: true,
						viewrecords: true, 
						sortorder: "asc", 
						multiselect: true,
						caption: ViewName });
					jQuery($gridOuter).jqGrid('navGrid', $gridPager, {edit:true,add:true,del:true} ); 
					
			} // success{}
		});

		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);

