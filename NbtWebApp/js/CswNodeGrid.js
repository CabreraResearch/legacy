/// <reference path="../jquery/jquery-1.4.4.js" />

; (function ($) {
	$.fn.CswNodeGrid = function (optJqGrid, optCswNodeGrid) {

		
		var o = {
			// jqGrid properties
			datatype: "local", 
			height: 300,
			rowNum:10, 
			autoencode: true,
			//autowidth: true, 
			rowList:[10,25,50],  
			//editurl:"/Popup_EditNode.aspx",
			sortname: "nodeid", 
			shrinkToFit: true,
			viewrecords: true,  
			emptyrecords:"No Data to Display", 
			sortorder: "asc", 
			multiselect: true,
			// CswNodeGrid properties
			GridUrl: '/NbtWebApp/wsNBT.asmx/getGrid',
			viewid: '',
			id: "CswNodeGrid",
			nodeid: '',
			cswnbtnodekey: '',
			gridTable: "_gridOuter",
			gridPager: "_gridPager"
		};
		
		if (optJqGrid) {
			$.extend(o, optJqGrid);
		}

		var gridData = [];
		var gridRows = [];

		var gridTableId = o.id + o.gridTable;
		var $gridOuter = $.CswTable({ ID: gridTableId })
							.appendTo($(this));
		
		var gridPagerId = o.id + o.gridPager;
		var $gridPager = $('<div id="' + gridPagerId + '" style="width:100%; height:20px;" />')
						 .appendTo($(this));
		
        if(debug)
        {
            log('CswNodeGrid');
        }

		CswAjaxJSON({
			url: o.GridUrl,
			data: "{ViewPk: '" +  o.viewid + "', 'CswNbtNodeKey': '" + o.cswnbtnodekey + "'}", //" + o.cswnbtnodekey + "
			success: function (gridJson) {
					
					gridData = gridJson.grid;
					gridRows = gridData.rows;

					var ViewName = gridJson.viewname;
					var columns = gridJson.columnnames;
					var columnDefinition = gridJson.columndefinition
					var gridWidth =  gridJson.viewwidth;
					if( gridWidth == '' )
					{
						gridWidth = 650;
					}
						
					var jqGridOptions = {
						data: gridRows,
						colNames: columns,  
						colModel: columnDefinition, 
						width: gridWidth,
						pager: $gridPager, 
						caption: ViewName,
						toppager: true,
					};

					var optSearch = {
						//id: o.id + '_search_button',
						caption: "Search...",
						Find: "Find",
						Reset: "Reset",
						odata : ['equal', 'not equal', 'less', 'less or equal','greater','greater or equal', 'begins with','does not begin with','is in','is not in','ends with','does not end with','contains','does not contain'],
						groupOps: [ { op: "AND", text: "all" }, { op: "OR", text: "any" } ],
						matchText: "match",
						rulesText: "rules"
					};

//					var optEdit = {
//						//id: o.id + '_edit_button',
//						addCaption: "Add Row 3",
//						editCaption: "Edit Row",
//						bSubmit: "Submit",
//						bCancel: "Cancel",
//						bClose: "Close",
//						saveData: "Data has been changed! Save changes?",
//						bYes : "Yes",
//						bNo : "No",
//						bExit : "Cancel",
//						editfunc: function() {
//								$.CswDialog('EditNodeDialog', nodeid, onEditNode, cswnbtnodekey);
//								return false;
//							}
//					};

//					var optAdd = {
//						//id: o.id + '_add_button',
//						add: true,
//						addCaption: "Add row 1",
//						addtext:"",
//						addtitle: "Add row 2",
//						addfunc: function() {
//								alert('hey');
//								$.CswDialog('AddNodeDialog', $this.attr('nodetypeid'), onAddNode);
//								return false;
//							}
//					};

//					var optView = {
//						//id: o.id + '_view_button',
//						caption: "View Record 0",
//						viewCaption: "View Record 1",
//						bClose: "Close"
//					};

					var optDel = {
						//id: o.id + '_delete_btton',
						caption: "Delete",
						msg: "Delete selected record(s)?",
						bSubmit: "Delete",
						bCancel: "Cancel",
						
					};

					var optNav = {
						cloneToTop: true,

						//edit
						edit: true,
						edittext: "",
						edittitle: "Edit row",
						editfunc: function() {
								$.CswDialog('EditNodeDialog', nodeid, onEditNode, cswnbtnodekey);
								return false;
							},

						//add
						add: true,
						addtext:"",
						addtitle: "Add row",
						addfunc: function() {
								alert('hey');
								$.CswDialog('AddNodeDialog', $this.attr('nodetypeid'), onAddNode);
								return false;
							},

						//delete
						del: true,
						deltext: "",
						deltitle: "Delete row",
						delfunc: function() {
								$.CswDialog('DeleteNodeDialog', nodename, nodeid, onDeleteNode, cswnbtnodekey);
								return false;
							},
						
						//search
						search: true,
						searchtext: "",
						searchtitle: "Find records",
						
						//refresh
						refreshtext: "",
						refreshtitle: "Reload Grid",
						alertcap: "Warning",
						alerttext: "Please, select row",
						
						//view
						view: true,
						viewtext: "",
						viewtitle: "View row",
						viewfunc: function() {
								$.CswDialog('ViewNodeDialog', nodeid, onEditNode, cswnbtnodekey);
								return false;
							}
					};

					$.extend(jqGridOptions, o);

					$gridOuter.jqGrid(jqGridOptions)
									  .hideCol('nodeid')
									  .hideCol('cswnbtnodekey');
					
					//all JSON option past optNav define the behavior of the built-in pop-up
					$gridOuter.jqGrid('navGrid', '#'+gridPagerId, optNav, {}, {}, optDel, optSearch, {} );
					
					// remove some double elements from top 					var topPagerDiv = $('#' + $gridOuter[0].id + '_toppager')[0];         					$("#edit_" + $gridOuter[0].id + "_top", topPagerDiv).remove();        					$("#del_" + $gridOuter[0].id + "_top", topPagerDiv).remove();         					$("#search_" + $gridOuter[0].id + "_top", topPagerDiv).remove();         					$("#add_" + $gridOuter[0].id + "_top", topPagerDiv).remove();     					$("#view_" + $gridOuter[0].id + "_top", topPagerDiv).remove();					//$("#" + $gridOuter[0].id + "_toppager_center", topPagerDiv).remove(); 					$(".ui-paging-info", topPagerDiv).remove();

					$gridOuter.jqGrid('navButtonAdd', '#' + $gridOuter[0].id + '_toppager_left' , { 											caption: "Columns",							                buttonicon: 'ui-icon-wrench',							                onClickButton: function() {							                    $gridOuter.jqGrid('columnChooser', {													done: function(perm) {							                            if (!perm) { return false; }							                            this.jqGrid('remapColumns', perm, true);														}													});												}											});
			} // success{}
		});

		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);

