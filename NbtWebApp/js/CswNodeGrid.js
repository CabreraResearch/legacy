/// <reference path="../jquery/jquery-1.5.1.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/grid.treegrid.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/grid.celledit.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/grid.common.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/grid.custom.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/grid.formedit.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/grid.grouping.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/grid.import.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/grid.inlinedit.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/grid.jqueryui.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/grid.loader.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/grid.postext.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/grid.setcolumns.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/grid.subgrid.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/grid.tbltogrid.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/grid.base.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/JsonXml.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/jqDnR.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/jqModal.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/jquery.fmatter.js" />
/// <reference path="../jquery/jquery.jqGrid-3.8.2/src/jquery.searchFilter.js" />

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
			log(o);
		}

		CswAjaxJSON({
			url: o.GridUrl,
			data: "{ViewPk: '" +  o.viewid + "', 'CswNbtNodeKey': '" + o.cswnbtnodekey + "'}", //" + o.cswnbtnodekey + "
			success: function (gridJson) {
					
					log('here');

					gridData = gridJson.grid;
					gridRows = gridData.rows;

					var ViewName = gridJson.viewname;
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

					var optDel = {
						caption: "Delete",
						msg: "Delete selected record(s)?",
						bSubmit: "Delete",
						bCancel: "Cancel"
					};

					var optNav = {
						cloneToTop: true,

						//edit
						edit: true,
						edittext: "",
						edittitle: "Edit row",
						editfunc: function() {
								var rowid = $gridOuter.jqGrid('getGridParam', 'selrow');
								if (rowid !== null) {
									var cswnbtnodekey = $gridOuter.jqGrid('getCell', rowid, 'cswnbtnodekey');
									alert ("the cell with id="+rowid+" and the key='" + cswnbtnodekey + "' is selected.");
								}
								$.CswDialog('EditNodeDialog', nodeid, onEditNode, cswnbtnodekey);
								return false;
							},

						//add
						add: true,
						addtext:"",
						addtitle: "Add row",
						addfunc: function() {
								var rowid = $gridOuter.jqGrid('getGridParam', 'selrow');
								if (rowid !== null) {
									var cswnbtnodekey = $gridOuter.jqGrid('getCell', rowid, 'cswnbtnodekey');
									alert ("the cell with id="+rowid+" and the key='" + cswnbtnodekey + "' is selected.");
								}
								//alert('hey');
								$.CswDialog('AddNodeDialog', $this.attr('nodetypeid'), onAddNode);
								return false;
							},

						//delete
						del: true,
						deltext: "",
						deltitle: "Delete row",
						delfunc: function() {
								var rowid = $gridOuter.jqGrid('getGridParam', 'selrow');
								if (rowid !== null) {
									var cswnbtnodekey = $gridOuter.jqGrid('getCell', rowid, 'cswnbtnodekey');
									alert ("the cell with id="+rowid+" and the key='" + cswnbtnodekey + "' is selected.");
								}
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

					if(debug)
					{
						log(jqGridOptions);
						log(o);
					}

					$.extend(jqGridOptions, o);

					if(debug)
					{
						log(jqGridOptions);
						//log(o);
					}

					$gridOuter.jqGrid(jqGridOptions)
									  .hideCol('nodeid')
									  .hideCol('cswnbtnodekey');
					
					//all JSON option past 'optNav' define the behavior of the built-in pop-up
					$gridOuter.jqGrid('navGrid', '#'+gridPagerId, optNav, {}, {}, optDel, optSearch, {} );
					

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

