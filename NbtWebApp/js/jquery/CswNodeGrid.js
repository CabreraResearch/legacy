/// <reference path="../js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
	
	var methods = {
	
		'init': function (optJqGrid) {

			var o = {
				GridUrl: '/NbtWebApp/wsNBT.asmx/getGrid',
				viewid: '',
                showempty: false,
				ID: '',
				nodeid: '',
				cswnbtnodekey: '',
				gridTableID: 'gridTable',
				gridPagerID: 'gridPager',
                'reinit': false,
                'EditMode': EditMode.Edit.name,
				//onAddNode: function(nodeid,cswnbtnodekey){},
				onEditNode: function(nodeid,cswnbtnodekey){},
				onDeleteNode: function(nodeid,cswnbtnodekey){}
			};
		
			if (optJqGrid) {
				$.extend(o, optJqGrid);
			}
			
            var $parent = $(this);
            if( o.reinit ) $parent.empty();

			var jqGrid = {};
        
            var gridTableId = makeId({ ID: o.gridTableID, prefix: o.ID });

		    var $gridTable = $parent.CswTable('init', { ID: gridTableId });
		
            var gridPagedId = makeId({ID: o.gridPagerID, prefix: o.ID});
            var $gridPager = $parent.CswDiv('init',{ID: gridPagedId});
									     //.css('width','100%')
									     //.css('height','20px');
            
            var dataJson = {ViewId: o.viewid, SafeNodeKey: o.cswnbtnodekey, ShowEmpty: o.showempty };		
			
            CswAjaxJSON({
				url: o.GridUrl,
				data: dataJson,
				success: function (gridJson) {
					    
						jqGridOpt = gridJson.jqGridOpt;

						var NodeTypeId = gridJson.nodetypeid;
                        var tableWidth = ( !isNullOrEmpty( jqGridOpt.width ) ) ? '650' : jqGridOpt.width;
						//$gridTable.css('width',tableWidth + 'px');
                        
						var canEdit = isTrue( jqGridOpt.CanEdit );
                        var canDelete = isTrue( jqGridOpt.CanDelete );

						var jqGridOptions = {
                            autoencode: true,
                            //autowidth: true,
                            altRows: false,
                            caption: '',
                            datatype: 'local', 
				            emptyrecords: 'No Results',
                            height: '300',
                            loadtext: 'Loading...',
				            multiselect: false,
							pager: $gridPager, 
							rowList:[10,25,50],  
				            rowNum:10, 
				            shrinkToFit: true,
				            sortname: '', 
				            sortorder: 'asc', 
                            toppager: false,
                            viewrecords: true,
                            width: tableWidth + 'px'
						};
                        $.extend(jqGridOptions,jqGridOpt);
                            
                        //include the top pager if the row count is very large
                        if(jqGridOptions.rowNum >= 50) jqGridOptions.toppager = true;
						
                        var optSearch = {};
						var optNav = {};

                        if( o.EditMode === EditMode.PrintReport.name )
                        {
                            jqGridOptions.caption = '';
                            $gridTable.jqGrid(jqGridOptions);
                        }
                        else
                        {
                            optSearch = {
							    caption: "Search...",
							    Find: "Find",
							    Reset: "Reset",
							    odata : ['equal', 'not equal', 'less', 'less or equal','greater','greater or equal', 'begins with','does not begin with','is in','is not in','ends with','does not end with','contains','does not contain'],
							    groupOps: [ { op: "AND", text: "all" }, { op: "OR", text: "any" } ],
							    matchText: "match",
							    rulesText: "rules"
						    };

                            var optNavEdit = {
							    edit: true,
							    edittext: "",
							    edittitle: "Edit row",
							    editfunc: function(rowid) 
                                {
									var editOpt = {
										cswnbtnodekey: '',
										nodeid: '',
										onEditNode: o.onEditNode
									};
									if (rowid !== null) 
									{
										editOpt.cswnbtnodekey = $gridTable.jqGrid('getCell', rowid, 'cswnbtnodekey');
										editOpt.nodeid = $gridTable.jqGrid('getCell', rowid, 'nodeidstr');
										$.CswDialog('EditNodeDialog', editOpt);
									}
									else
									{
										alert('Please select a row to edit');
									}
									return editOpt.CswNbtNodeKey;
								}
                            };

                            var optNavDelete = {
                                del: true,
							    deltext: "",
							    deltitle: "Delete row",
							    delfunc: function(rowid) 
                                {
									var delOpt = {
										'cswnbtnodekey': '',
										'nodeid': '',
										'nodename': '',
										'onDeleteNode': o.onDeleteNode
									};
									if (rowid !== null) {
										delOpt.cswnbtnodekey = $gridTable.jqGrid('getCell', rowid, 'cswnbtnodekey');
										delOpt.nodename = $gridTable.jqGrid('getCell', rowid, 'nodename');
										$.CswDialog('DeleteNodeDialog', delOpt);
									}
									else
									{
										alert('Please select a row to delete');
									}
									return delOpt.cswnbtnodekey;
								}
                            };

						    optNav = {	
                                cloneToTop: false,

							    add: false,
                                del: false,
                                edit: false,
						
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

                            if( canEdit ) $.extend(optNav,optNavEdit);
                            if( canDelete ) $.extend(optNav,optNavDelete);
                            if( canEdit || canDelete ) jqGridOptions.multiselect = true;

                            $gridTable.jqGrid(jqGridOptions)
										 .navGrid('#'+$gridPager.CswAttrDom('id'), optNav, {}, {}, {}, optSearch, {} ); 
                                         //all JSON options past 'optNav' define the behavior of the built-in pop-up
                        } // else

						//remove some dup elements from top pager
//						var topPagerDiv = $('#' + $gridTable[0].id + '_toppager')[0];         
//						$("#edit_" + $gridTable[0].id + "_top", topPagerDiv).remove();        
//						$("#del_" + $gridTable[0].id + "_top", topPagerDiv).remove();         
//						$("#search_" + $gridTable[0].id + "_top", topPagerDiv).remove();         
//						$("#add_" + $gridTable[0].id + "_top", topPagerDiv).remove();     
//						$("#view_" + $gridTable[0].id + "_top", topPagerDiv).remove();
//						$("#" + $gridTable[0].id + "_toppager_center", topPagerDiv).remove(); 
//						$(".ui-paging-info", topPagerDiv).remove();

//						//add custom button to nav panel
//						$gridTable.jqGrid('navButtonAdd', '#' + $gridTable[0].id + '_toppager_left' , { 
//												caption: "Columns",
//												buttonicon: 'ui-icon-wrench',
//												onClickButton: function() {
//													$gridTable.jqGrid('columnChooser', {
//														done: function(perm) {
//															if (!perm) { return false; }
//																$gridTable.jqGrid('remapColumns', perm, true);
//															}
//														});
//													}
//												});

				} // success{} 
			}); // ajax
			return $gridTable;
		}, // 'init'
	
        'scrollToSelectedRow': function() {
			var $gridTable = $(this);
			var rowid = $gridTable.jqGrid('getGridParam', 'selrow');
			scrollToRow($gridTable, rowid);
		} // 'scrollToSelectedRow'

	}; // methods



	// Row scrolling adapted from 
	// http://stackoverflow.com/questions/2549466/is-there-a-way-to-make-jqgrid-scroll-to-the-bottom-when-a-new-row-is-added/2549654#2549654

	function getGridRowHeight ($grid) {
		var height = null; // Default

		try{
			height = $grid.find('tbody').find('tr:first').outerHeight();
		}
		catch(e){
			//catch and just suppress error
		}

		return height;
	}

	function scrollToRow ($grid, id) {
		var rowHeight = getGridRowHeight($grid) || 23; // Default height
		var index = $grid.getInd(id);
		$grid.closest(".ui-jqgrid-bdiv").scrollTop(rowHeight * (index - 1));
	}


	$.fn.CswNodeGrid = function (method) {
		/// <summary>
        ///   Generates a jqGrid
        /// </summary>
        /// <param name="method" type="String">
        ///     A string defining the function to call
        ///     &#10;1 - 'init': creates a grid from a view
        ///     &#10;2 - 'scrollToSelectedRow': scrolls to a specific row in the grid
        /// </param>
        
        // Method calling logic
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
	}

})(jQuery);

