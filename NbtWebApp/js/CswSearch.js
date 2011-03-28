/// <reference path="../jquery/jquery-1.5.js" />

; (function ($) {
	var PluginName = "CswSearch";

    function modAdvanced(options)
    {
        var o = {
                '$link': '',
                '$filtersCell': '',
                '$subfieldsCell': '',
                '$listCell':''
        };
        if(options) $.extend(o,options);
        if('Advanced' == o.$link.text())
        {
            o.$filtersCell.show();
            o.$subfieldsCell.show();
            if(o.Fieldtype == 'List')
            {
                o.$listCell.show();
            }             
            o.$link.text('Hide');
        }
        else
        {
            o.$filtersCell.hide();
            o.$subfieldsCell.hide();
            o.$listCell.hide();
            o.$link.text('Advanced');
        }
        return false; 
    }
    
    var selectedData = [];

    function renderNodeTypeSearchContent(options)
    {



    }

	var methods = {
	
		'getSearchForm': function(options) 
			{
				var o = { 
					'RenderSearchUrl': '/NbtWebApp/wsNBT.asmx/getClientSearchXml',
					'ExecViewSearchUrl': '/NbtWebApp/wsNBT.asmx/doViewSearch',
                    'ExecNodeSearchUrl': '/NbtWebApp/wsNBT.asmx/doNodeTypeSearch',
                    'SearchableViewsUrl': '/NbtWebApp/wsNBT.asmx/getSearchableViews',
                    viewid: '',
                    nodetypeid: '',
                    relatedidtype: '',
                    'onSearch': function() { }
				};
				if(options) {
					$.extend(o, options);
				}
                var $parent = $(this);
                var $titlespan = $parent.html('<span style="align: center;">Search</span>');
                var $topspan = $parent.html('<span></span>');
                var $topspandiv = $topspan.html('<div id="search_criteria_div"></div>');
                var $padding = $parent.html('<br/><br/><br/><br/><br/>');
                var $bottomspan = $parent.html('<span></span>');
                var $bottomspandiv = $bottomspan.html('<div id="change_search_div"></div>');

                CswAjaxXml({ 
			        'url': o.RenderSearchUrl,
			        'data': "ViewIdNum=" + o.viewid + "&SelectedNodeTypeIdNum=" + o.nodetypeid,
                    'success': function($xml) { 
                                    
                                    var $table = $topspandiv.CswTable('init', { 
                                                            ID: 'search_tbl', 
                                                            cellpadding: 1,
                                                            cellspacing: 1,
                                                            cellalign: 'center',
                                                            align: 'center'
                                                            });
                                    var searchtype = $xml.attr('searchtype');
                                    switch(searchtype)
                                    {
                                        case 'nodetypesearch':
                                        {
                                            renderNodeTypeSearchContent({
                                                '$table': $table,
                                                'NodeType': $('#node_type_select').val(),
                                                'Property': $xml.children('nodetypeprops').children('properties').attr('defaultprop'),
                                                'Filter': '',
                                                'Subfield': '',
                                                'FieldType': ''
                                            });
                                            
                                            //Row 1, Column 1: nodetypeselect picklist
                                            var $typeSelectCell = $table.CswTable('cell', 1, 1);
                                            var $option = $xml.children('nodetypes');
                                            var optionList = xmlToString($option);
                                            $typeSelectCell.html(optionList);
                                            $('#node_type_select');
                                            
                                            var currentSelected = {
                                                NodeType: $('#node_type_select').val(),
                                                Property: $xml.children('nodetypeprops').children('properties').attr('defaultprop'),
                                                Filter: '',
                                                Subfield: '',
                                                FieldType: ''
                                                };

                                            var $searchCriteria = $xml.children('nodetypeprops').children('propertyfilters').children('property[propname='+ currentSelected.Property +']');
                                            currentSelected.Filter = $searchCriteria.children('defaultsubfield').attr('filter');
                                            currentSelected.Subfield = $searchCriteria.children('defaultsubfield').attr('subfield');        
                                            currentSelected.FieldType = $searchCriteria.attr('fieldtype');
                                            $.extend(currentSelected,selectedData);
                                            
                                            //Row 1, Column 2: properties picklist
                                            var $propSelectCell = $table.CswTable('cell', 1, 2);
                                            var $props = $xml.children('nodetypeprops').children('properties');
                                            var propList = xmlToString($props);
                                            $propSelectCell.html(propList);
                                            $('#properties_select');

                                            //Row 1, Column 3: default filter
                                            var $staticSubfieldCell = $table.CswTable('cell', 1, 3);
                                            $staticSubfieldCell.html('<span>' + currentSelected.Filter + '</span>').css("align","center");

                                            //Row 1, Column 4: subfield picklist
                                            var $allSubfields = $searchCriteria.children('subfields');        
                                            var subfieldsPicklist = xmlToString($allSubfields);
                                            var $subfieldsCell = $table.CswTable('cell', 1, 4);
                                            $subfieldsCell.html(subfieldsPicklist).hide();

                                            //Row 1, Column 5: filter picklist
                                            var $allFilters = $searchCriteria.children('propertyfilters').children('subfield[column=' + currentSelected.Subfield + ']');
                                            var filtersPicklist = xmlToString($allFilters);
                                            var $filtersCell = $table.CswTable('cell', 1, 5);
                                            $filtersCell.html(filtersPicklist).hide();

                                            //Row 1, Column 6: list (fieldtype) options
                                            var $listOptions = $searchCriteria.children('filtersoptions');
                                            var list = xmlToString($listOptions);
                                            var $listCell = $table.CswTable('cell', 1, 6);
                                            $listCell.html(list).hide();
                                           
                                            //Row 1, Column 7: search box
                                            var $searchBoxCell = $table.CswTable('cell', 1, 7);
                                            var searchInput = '<input type="text" name="search_input" id="search_input" autocomplete="on" autofocus="true" placeholder="' + currentSelected.Property + ' search" width="200px" />';
                                            $searchBoxCell.html(searchInput);

                                            
                                            var $splitCell = $table.CswTable('cell', 2, 1);
                                            var $splitCellTable = $splitCell.CswTable('init',{ID: 'split_cell_table', 
                                                                                                cellpadding: 1,
                                                                                                cellspacing: 1,
                                                                                                cellalign: 'center',
                                                                                                align: 'left'
                                                                                                });
                                            //Row 2, Column 1 (1/1): clear button
                                            var $clearButtonCell = $splitCellTable.CswTable('cell', 1, 1);
                                            var clearButton = '<input type="button" name="clear_button" id="clear_button" value="Clear" />';
                                            $clearButtonCell.html(clearButton);
                                            
                                            

                                            //Row 2, Column 1 (1/2): advanced link
                                            var $advancedLinkCell = $splitCellTable.CswTable('cell', 1, 2);
                                            var $advancedLink = $('<a href="#advanced">Advanced</a>')
                                                                  .appendTo($advancedLinkCell);
                                            $advancedLink.click(function() {
                                                                  modAdvanced({
                                                                        '$link': $advancedLink,
                                                                        '$filtersCell': $filtersCell,
                                                                        '$subfieldsCell': $subfieldsCell,
                                                                        '$listCell': $listCell
                                                                        });
                                                                });
                                            
                                            //Row 2, Column 4: search button
                                            var $searchButtonCell = $table.CswTable('cell', 2, 4).css("align","right");
                                            var searchButtonCell = '<input type="button" name="search_button" id="search_button" value="Search" />';
                                            $searchButtonCell.html(searchButtonCell);

                                            //Bottom Span
                                            var $bottomTable = $bottomspandiv.CswTable('init', {ID: 'change_search_tbl', 
                                                                                                cellpadding: 1,
                                                                                                cellspacing: 1,
                                                                                                cellalign: 'center',
                                                                                                align: 'right'});
                                            //Row 1, Column 1: load a search
                                            var $loadTableCell = $bottomTable.CswTable('cell', 1, 1);
                                            $loadTableCell.html('Load a Search:');
                                                                                        
                                            CswAjaxXml({ 
			                                    'url': o.SearchableViewsUrl,
			                                    'data': "IsMobile=" + false + "&OrderBy=",
                                                'success': function($views) { 
                                                        
                                                        //Row 1, Column 2: view select
                                                        var $viewSelectCell = $bottomTable.CswTable('cell', 1, 2);
                                                        var viewPicklist = xmlToString($views);
                                                        $viewSelectCell.html(viewPicklist);
                                                    }
                                                });

                                            //Row 1, Column 3: load button
                                            var $loadButtonCell = $bottomTable.CswTable('cell', 1, 3);
                                            var loadButton = '<input type="button" name="load_button" id="load_button" value="Load" />';
                                            $loadButtonCell.html(loadButton);

                                            //Row 2, Column 2: new custom search
                                            var $customSearchCell = $bottomTable.CswTable('cell', 2, 2);
                                            $customSearchCell.html('<a href="#customsearch">New Custom Search</a>');

                                        }
                                        case 'viewsearch':
                                        {


                                        }

                                    }
							        
						        } // each
					         // success
			        }); // CswAjaxXml

				
			} // getAddItemForm
		
	};

	
	// Method calling logic
	$.fn.CswSearch = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};

})(jQuery);


