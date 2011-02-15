/// <reference path="../jquery/jquery-1.4.4.js" />

; (function ($) {
	$.fn.CswNodeGrid = function (options) {

		var o = {
			GridUrl: '/NbtWebApp/wsNBT.asmx/getGridJSON',
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
		
		var gridData = [];
		var dataView;
		var grid;
		var pager;
		var columnpicker;

		var $gridOuter = $('<table id="gridOuter" style="width:600px; height:500px; " />')
						.appendTo($(this));
//		var $gridHeader = $('<div id="gridHeader" class="grid-header" style="width:100%;" />')
//						.addClass("ui-state-default ui-corner-all")
//						.mouseover(function(e) {
//							$(e.target).addClass("ui-state-hover")
//						})
//						.mouseout(function(e) {
//							$(e.target).removeClass("ui-state-hover")
//						})
//						.appendTo($gridOuter);
//		var $gridLabel = $('<label />')
//						.appendTo($gridHeader);
//		//<span style="float:right" class="ui-icon ui-icon-search" title="Toggle search panel" onclick="toggleFilterRow()"></span>
//		var $gridContent = $('<div id="gridContents" style="width:100%; height:500px;" />')
//							.appendTo($gridOuter);
		var $gridPager = $('<div id="gridPager" style="width:100%; height:20px;" />')
						 .appendTo($(this));

		CswAjaxJSON({
			url: o.GridUrl,
			data: "{ViewId: '" +  o.viewid + "'}",
			success: function (gridJson) {
					
					console.log(gridJson);
					
					gridData = gridJson.grid;
					jQuery($gridOuter).jqGrid({ 
						//url: o.GridUrl, 
						datatype: "local", 
						data: gridData,
						colNames:['Id','Equipment','Assembly'], 
						colModel:[ 
							{name:'id',index:'id', width:75}, 
							{name:'equipment',index:'equipment', width:90}, 
							{name:'assembly',index:'assembly', width:100} 
							], 
						rowNum:10, 
						autowidth: true, 
						rowList:[10,20,30], 
						pager: jQuery($gridPager), 
						sortname: 'id', 
						viewrecords: true, 
						sortorder: "desc", 
						caption: o.GridUrl })
					.navGrid($gridPager,{edit:false,add:false,del:false}); 

				/*function requiredFieldValidator(value) {
					if (value == null || value == undefined || !value.length)
					{
						return {valid:false, msg:"This is a required field"};
					}
					else
					{
						return {valid:true, msg:null};
					}
				}

				function comparer(a,b) {
					var x = a[sortcol], y = b[sortcol];
					return (x == y ? 0 : (x > y ? 1 : -1));
				}

				console.log(gridJson);
				var ViewName = gridJson.viewname;
				$gridLabel.text(ViewName);

				var columns = gridJson.columns;
				gridData = gridJson.grid;
				console.log(gridData);
				var sortcol = "id";
				var sortdir = 1;

				dataView = new Slick.Data.DataView();
				dataView.beginUpdate();
				dataView.setItems(gridData);
				//dataView.setPagingOptions({pageSize:20});
				dataView.setFilter( false );
				dataView.endUpdate();

				console.log(dataView);

				grid = new Slick.Grid( $gridContent, dataView, columns, options );
				grid.setSelectionModel(new Slick.CellSelectionModel());

				pager = new Slick.Controls.Pager( dataView, grid, $gridPager );
				columnpicker = new Slick.Controls.ColumnPicker(columns, grid, options);
 
				grid.onAddNewRow.subscribe(function(e, args) {
					var item = args.item;
					var column = args.column;
					grid.invalidateRow(data.length);
					data.push(item);
					grid.updateRowCount();
					grid.render();
				});

				grid.onKeyDown.subscribe(function(e) {
					// select all rows on ctrl-a
					if (e.which != 65 || !e.ctrlKey)
						return false;

					var rows = [];
					selectedRowIds = [];

					for (var i = 0; i < dataView.getLength(); i++) {
						rows.push(i);
						selectedRowIds.push(dataView.getItem(i).id);
					}

					grid.setSelectedRows(rows);
					e.preventDefault();
				});

				grid.onSelectedRowsChanged.subscribe(function(e) {
						selectedRowIds = [];
						var rows = grid.getSelectedRows();
						for (var i = 0, l = rows.length; i < l; i++) {
							var item = dataView.getItem(rows[i]);
							if (item) selectedRowIds.push(item.id);
						}
				});

				grid.onSort.subscribe(function(e, args) {
					sortdir = args.sortAsc ? 1 : -1;
					sortcol = args.sortCol.field;

					// using native sort with comparer
					// preferred method but can be very slow in IE with huge datasets
					dataView.sort(comparer, args.sortAsc);
				});
				
				dataView.onRowCountChanged.subscribe(function(e,args) {
					grid.updateRowCount();
					grid.render();
				});

				dataView.onRowsChanged.subscribe(function(e,args) {
					grid.invalidateRows(args.rows);
					grid.render();

					if (selectedRowIds.length > 0)
					{
						// since how the original data maps onto rows has changed,
						// the selected rows in the grid need to be updated
						var selRows = [];
						for (var i = 0; i < selectedRowIds.length; i++)
						{
							var idx = dataView.getRowById(selectedRowIds[i]);
							if (idx != undefined)
								selRows.push(idx);
						}

						grid.setSelectedRows(selRows);
					}
				});

				dataView.onPagingInfoChanged.subscribe(function(e,pagingInfo) {
					var isLastPage = pagingInfo.pageSize*(pagingInfo.pageNum+1)-1 >= pagingInfo.totalRows;
					var enableAddRow = isLastPage || pagingInfo.pageSize==0;
					var options = grid.getOptions();

					if (options.enableAddRow != enableAddRow)
						grid.setOptions({enableAddRow:enableAddRow});
				});

				$($gridContent).show();

//				dataView.beginUpdate();
//				dataView.setItems(gridData);
//				dataView.setFilter( false );
//				dataView.endUpdate();

//				grid.updateRowCount();
//				grid.render();
				*/
			} // success{}
		});

		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);

