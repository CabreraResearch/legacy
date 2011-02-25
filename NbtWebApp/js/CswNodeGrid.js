/// <reference path="../jquery/jquery-1.4.4.js" />

; (function ($) {
	$.fn.CswNodeGrid = function (options, dom) {

		// jqGrid properties
		var o = {
			datatype: "local", 
			height: 300,
			rowNum:10, 
			autoencode: true,
			autowidth: true, 
			rowList:[10,25,50],  
			editurl:"/Popup_EditNode.aspx",
			sortname: "nodeid", 
			shrinkToFit: true,
			viewrecords: true,  
			emptyrecords:"No Data to Display", 
			sortorder: "asc", 
			multiselect: true,
		};
		
		if (options) {
			$.extend(o, options);
		}

		// CswNodeGrid properties
		var d = {
			GridUrl: '/NbtWebApp/wsNBT.asmx/getGridJson',
			viewid: '',
			id: "CswNodeGrid",
			gridTable: "_gridOuter",
			gridPager: "_gridPager"
		};
		
		if(dom) {
			$.extend(d,dom);
		}

		var gridData = [];
		var gridRows = [];

		var gridTableId = d.id + d.gridTable;
		var $gridOuter = makeTable(gridTableId)
						.appendTo($(this));
		
		var gridPagerId = d.id + d.gridPager;
		var $gridPager = $('<div id="' + gridPagerId + '" style="width:100%; height:20px;" />')
						 .appendTo($(this));
		
		CswAjaxJSON({
			url: d.GridUrl,
			data: "{ViewId: '" +  d.viewid + "'}",
			success: function (gridJson) {
					
					gridData = gridJson.grid;
					gridRows = gridData.rows;

					var ViewName = gridJson.viewname;
					var columns = gridJson.columnnames;
					var columnDefinition = gridJson.columndefinition
					var gridWidth =  gridJson.viewwidth;
					if( gridWidth == '' )
					{
						widthgridWidth = 500;
					}
						
					var jqGridOptions = {
						data: gridRows,
						colNames: columns,  
						colModel: columnDefinition, 
						width: gridWidth,
						pager: $gridPager, 
						caption: ViewName
					};

					var optSearch = {
						id: d.id,
						caption: "Search...",
						Find: "Find",
						Reset: "Reset",
						odata : ['equal', 'not equal', 'less', 'less or equal','greater','greater or equal', 'begins with','does not begin with','is in','is not in','ends with','does not end with','contains','does not contain'],
						groupOps: [ { op: "AND", text: "all" }, { op: "OR", text: "any" } ],
						matchText: "match",
						rulesText: "rules"
					}
					var optEdit = {
						id: d.id,
						addCaption: "Add Record",
						editCaption: "Edit Record",
						bSubmit: "Submit",
						bCancel: "Cancel",
						bClose: "Close",
						saveData: "Data has been changed! Save changes?",
						bYes : "Yes",
						bNo : "No",
						bExit : "Cancel"
					}
					var optAdd = {};
					var optView = {
						id: d.id,
						caption: "View Record",
						bClose: "Close"
					}
					var optDel = {
						id: d.id,
						caption: "Delete",
						msg: "Delete selected record(s)?",
						bSubmit: "Delete",
						bCancel: "Cancel"
					}
					var optNav = {
						id: d.id,
						edit: true,
						edittext: "",
						edittitle: "Edit selected row",
						add: true,
						addtext:"",
						addtitle: "Add new row",
						del: true,
						deltext: "",
						deltitle: "Delete selected row",
						search: true,
						searchtext: "",
						searchtitle: "Find records",
						refreshtext: "",
						refreshtitle: "Reload Grid",
						alertcap: "Warning",
						alerttext: "Please, select row",
						view: true,
						viewtext: "",
						viewtitle: "View selected row"
					}

					$.extend(jqGridOptions, o);

					jQuery($gridOuter).jqGrid(jqGridOptions)
									  .hideCol('nodeid')
									  .navGrid( '#'+gridPagerId, optNav, optEdit, optAdd, optDel, optSearch, optView );
					
			} // success{}
		});

		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);

