/// <reference path="../jquery/jquery-1.5.2.js" />

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
			gridPager: "_gridPager",
			onAddNode: function(nodeid){},
			onEditNode: function(nodeid,cswnbtnodekey){},
			onDeleteNode: function(nodeid,cswnbtnodekey){}
		};
		
		if (optJqGrid) {
			$.extend(o, optJqGrid);
		}

		var gridData = [];
		var gridRows = [];

		var gridTableId = o.id + o.gridTable;
		var $gridOuter = $(this).CswTable('init', { ID: gridTableId });
		
		var gridPagerId = o.id + o.gridPager;
		var $gridPager = $('<div id="' + gridPagerId + '" style="width:100%; height:20px;" />')
						 .appendTo($(this));
		
		CswAjaxJSON({
			url: o.GridUrl,
			data: "{ViewPk: '" +  o.viewid + "', 'SafeNodeKey': '" + o.cswnbtnodekey + "'}", //" + o.cswnbtnodekey + "
			success: function (gridJson) {
					
					gridData = gridJson.grid;
					gridRows = gridData.rows;

					var ViewName = gridJson.viewname;
					var NodeTypeId = gridJson.nodetypeid;
					var columns = gridJson.columnnames;
					var columnDefinition = gridJson.columndefinition;
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
						toppager: true
					};

					var optSearch = {
						caption: "Search...",
						Find: "Find",
						Reset: "Reset",
						odata : ['equal', 'not equal', 'less', 'less or equal','greater','greater or equal', 'begins with','does not begin with','is in','is not in','ends with','does not end with','contains','does not contain'],
						groupOps: [ { op: "AND", text: "all" }, { op: "OR", text: "any" } ],
						matchText: "match",
						rulesText: "rules"
					};

					var optNav = {
						cloneToTop: true,

						//edit
						edit: true,
						edittext: "",
						edittitle: "Edit row",
						editfunc: function(rowid) {
								if (rowid !== null) {
									var CswNbtNodeKey = $gridOuter.jqGrid('getCell', rowid, 'cswnbtnodekey');
									var NodeId = $gridOuter.jqGrid('getCell', rowid, 'nodeid');
									var RequestReadOnly = false;
								}
								$.CswDialog('EditNodeDialog', NodeId, o.onEditNode, CswNbtNodeKey, RequestReadOnly);
								return false;
							},

						//add
						add: true,
						addtext:"",
						addtitle: "Add row",
						addfunc: function() {
								$.CswDialog('AddNodeDialog', NodeTypeId, o.onAddNode);
								return false;
							},

						//delete
						del: true,
						deltext: "",
						deltitle: "Delete row",
						delfunc: function(rowid) {
								if (rowid !== null) {
									var CswNbtNodeKey = $gridOuter.jqGrid('getCell', rowid, 'cswnbtnodekey');
									var NodeId = $gridOuter.jqGrid('getCell', rowid, 'nodeid');
									var NodeName = $gridOuter.jqGrid('getCell', rowid, 'nodename');
								}
								$.CswDialog('DeleteNodeDialog', NodeName, NodeId, o.onDeleteNode, CswNbtNodeKey);
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
						viewfunc: function(rowid) {
								if (rowid !== null) {
									var CswNbtNodeKey = $gridOuter.jqGrid('getCell', rowid, 'cswnbtnodekey');
									var NodeId = $gridOuter.jqGrid('getCell', rowid, 'nodeid');
									var OnEditNode = function(options) {};
									var RequestReadOnly = true;
								}
								$.CswDialog('EditNodeDialog', NodeId, OnEditNode, CswNbtNodeKey, RequestReadOnly);
								return false;
							}
					};

					$.extend(jqGridOptions, o);

					$gridOuter.jqGrid(jqGridOptions)
									  .hideCol('nodeid')
									  .hideCol('cswnbtnodekey')
									  .hideCol('nodename');
					
					//all JSON option past 'optNav' define the behavior of the built-in pop-up
					$gridOuter.jqGrid('navGrid', '#'+gridPagerId, optNav, {}, {}, {}, optSearch, {} );
					

					//remove some dup elements from top pager
					var topPagerDiv = $('#' + $gridOuter[0].id + '_toppager')[0];         
					$("#edit_" + $gridOuter[0].id + "_top", topPagerDiv).remove();        
					$("#del_" + $gridOuter[0].id + "_top", topPagerDiv).remove();         
					$("#search_" + $gridOuter[0].id + "_top", topPagerDiv).remove();         
					$("#add_" + $gridOuter[0].id + "_top", topPagerDiv).remove();     
					$("#view_" + $gridOuter[0].id + "_top", topPagerDiv).remove();
					$("#" + $gridOuter[0].id + "_toppager_center", topPagerDiv).remove(); 
					$(".ui-paging-info", topPagerDiv).remove();

					//add custom button to nav panel
					$gridOuter.jqGrid('navButtonAdd', '#' + $gridOuter[0].id + '_toppager_left' , { 
											caption: "Columns",
											buttonicon: 'ui-icon-wrench',
											onClickButton: function() {
												$gridOuter.jqGrid('columnChooser', {
													done: function(perm) {
														if (!perm) { return false; }
															$gridOuter.jqGrid('remapColumns', perm, true);
														}
													});
												}
											});
			} // success{}
		});

		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);

