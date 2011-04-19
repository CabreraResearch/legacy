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
			prefix: "CswNodeGrid",
			nodeid: '',
			cswnbtnodekey: '',
			gridTableID: "gridTable",
			gridPagerID: "gridPager",
			onAddNode: function(nodeid,cswnbtnodekey){},
			onEditNode: function(nodeid,cswnbtnodekey){},
			onDeleteNode: function(nodeid,cswnbtnodekey){}
		};
		
		if (optJqGrid) {
			$.extend(o, optJqGrid);
		}
        var $parent = $(this);

		var gridData = [];
		var gridRows = [];

		var $gridTable = $parent.CswTable('init', { ID: o.gridTableID, prefix: o.prefix });
		var $gridPager = $parent.CswDOM('div',{ID: o.gridPagerID, prefix: o.prefix})
                                 .css('width','100%')
                                 .css('height','20px');
		
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
					if( gridWidth === '' )
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
								var editOpt = {
                                    cswnbtnodekey: '',
                                    nodeid: '',
                                    onEditNode: o.onEditNode
                                };
                                if (rowid !== null) 
                                {
									editOpt.cswnbtnodekey = $gridTable.jqGrid('getCell', rowid, 'cswnbtnodekey');
									editOpt.nodeid = $gridTable.jqGrid('getCell', rowid, 'nodeid');
								    $.CswDialog('EditNodeDialog', editOpt);
								}
                                else
                                {
                                    alert('Please select a row to edit');
                                }
                                return editOpt.CswNbtNodeKey;
							},

						//add
						add: true,
						addtext:"",
						addtitle: "Add row",
						addfunc: function() {
								var addOpt = {
                                    'nodetypeid': NodeTypeId,
                                    'onAddNode': o.onAddNode
                                }
                                $.CswDialog('AddNodeDialog', addOpt);
								return addOpt.nodetypeid;
							},

						//delete
						del: true,
						deltext: "",
						deltitle: "Delete row",
						delfunc: function(rowid) {
								var delOpt = {
                                    'cswnbtnodekey': '',
                                    'nodeid': '',
                                    'nodename': '',
                                    'onDeleteNode': o.onDeleteNode
                                };
                                if (rowid !== null) {
									delOpt.cswnbtnodekey = $gridTable.jqGrid('getCell', rowid, 'cswnbtnodekey');
									delOpt.nodeid = $gridTable.jqGrid('getCell', rowid, 'nodeid');
									delOpt.nodename = $gridTable.jqGrid('getCell', rowid, 'nodename');
								    $.CswDialog('DeleteNodeDialog', delOpt);
                                }
                                else
                                {
                                    alert('Please select a row to delete');
                                }
								return delOpt.cswnbtnodekey;
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
						viewtitle: "View row"
						//viewfunc: none--use jqGrid built-in function for read-only
					};

					$.extend(jqGridOptions, o); 

                    $gridTable.jqGrid(jqGridOptions)
									  .hideCol('nodeid')
									  .hideCol('cswnbtnodekey')
									  .hideCol('nodename')
                                      //all JSON options past 'optNav' define the behavior of the built-in pop-up
                                      .navGrid('#'+$gridPager.attr('id'), optNav, {}, {}, {}, optSearch, {} );
					
					//$gridTable.jqGrid('navGrid', '#'+$gridPager.attr('id'), optNav, {}, {}, {}, optSearch, {} );
					

					//remove some dup elements from top pager
					var topPagerDiv = $('#' + $gridTable[0].id + '_toppager')[0];         
					$("#edit_" + $gridTable[0].id + "_top", topPagerDiv).remove();        
					$("#del_" + $gridTable[0].id + "_top", topPagerDiv).remove();         
					$("#search_" + $gridTable[0].id + "_top", topPagerDiv).remove();         
					$("#add_" + $gridTable[0].id + "_top", topPagerDiv).remove();     
					$("#view_" + $gridTable[0].id + "_top", topPagerDiv).remove();
					$("#" + $gridTable[0].id + "_toppager_center", topPagerDiv).remove(); 
					$(".ui-paging-info", topPagerDiv).remove();

					//add custom button to nav panel
					$gridTable.jqGrid('navButtonAdd', '#' + $gridTable[0].id + '_toppager_left' , { 
											caption: "Columns",
											buttonicon: 'ui-icon-wrench',
											onClickButton: function() {
												$gridTable.jqGrid('columnChooser', {
													done: function(perm) {
														if (!perm) { return false; }
															$gridTable.jqGrid('remapColumns', perm, true);
														}
													});
												}
											});
			} // success{} 
		});
		
		
		// Adapted from 
		// http://stackoverflow.com/questions/2549466/is-there-a-way-to-make-jqgrid-scroll-to-the-bottom-when-a-new-row-is-added/2549654#2549654

		function getGridRowHeight (targetGrid) {
			var height = null; // Default

			try{
				height = jQuery(targetGrid).find('tbody').find('tr:first').outerHeight();
			}
			catch(e){
			 //catch and just suppress error
			}

			return height;
		}

		function scrollToRow (targetGrid, id) {
			var rowHeight = getGridRowHeight(targetGrid) || 23; // Default height
			var index = jQuery(targetGrid).getInd(id);
			jQuery(targetGrid).closest(".ui-jqgrid-bdiv").scrollTop(rowHeight * index);
		}
		
		
		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);

