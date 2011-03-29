/// <reference path="../jquery/jquery-1.5.js" />

; (function ($) {
	var PluginName = "CswSearch";

    function modAdvanced(options)
    {
        var o = {
                '$link': '',
                '$filtersOptions': '',
                '$subfieldCell': '',
                '$defaultSubfield': '',
                '$subfieldsOptions': '',
                '$listOptions': ''
        };
        if(options) $.extend(o,options);
        
        if('Advanced' == o.$link.text())
        {
            o.$filtersOptions.show();
            o.$defaultSubfield.hide();
            o.$subfieldsOptions.show();
            if(o.Fieldtype == 'List')
            {
                o.$listOptions.show();
            }

            o.$link.text('Hide');
        }
        else
        {
            o.$filtersOptions.hide();
            o.$defaultSubfield.show();
            o.$subfieldsOptions.hide();
            o.$listOptions.hide();
          
            o.$link.text('Advanced');
        }
        return false; 
    } 

    function renderNodeTypeSearchContent(options)
    {
        var o = {
            '$bottomspandiv': '',
            '$parent': '',
            '$searchXml': '',
            '$searchTable': '',
            'nodeTypeId': '',
            '$property': '',
            '$filter': '',
            '$subfield': '',
            '$fieldType': '',
            'initOptions': {}
        };

        if(options) $.extend(o,options);
        
        var initOptions = o.initOptions;

        var propertyVal = o.$property;
        var $searchCriteria = o.$searchXml.children('nodetypeprops').children('propertyfilters').children('property[propname='+ propertyVal +']');

        //Row 1, Column 1: nodetypeselect picklist
        var $typeSelectCell = o.$searchTable.CswTable('cell', 1, 1);
        var $nodeTypes = $(xmlToString(o.$searchXml.children('nodetypes').children('select')));
        $nodeTypes.change( function() {
                           initOptions.viewid = '';
                           initOptions.nodetypeid = $(this).val();
                           init(true,initOptions,o);
                      });
        $typeSelectCell.append($nodeTypes);
                                                                                        
        //Row 1, Column 2: properties picklist
        var $propSelectCell = o.$searchTable.CswTable('cell', 1, 2);
        var $props = $(xmlToString(o.$searchXml.children('nodetypeprops').children('properties').children('select')))
                     .change(function() {
                        var reInit = {};   
                        $.extend(reInit,o);
                        reInit.$property = '';
                        reInit.$filter = '';
                        reInit.$subfield = '';
                        reInit.$fieldType = '';
                        init(false, {viewid: '', nodetypeid: $nodeTypes.val()}, reInit);
                     });
        $propSelectCell.append($props);

        //Row 1, Column 3: default filter
        var $subfieldCell = o.$searchTable.CswTable('cell', 1, 3);
        var $defaultSubField = $('<span>' + o.$filter + '</span>')
                               .attr({align:"center"});
        $subfieldCell.append($defaultSubField);
                    

        //Row 1, Column 3: subfield picklist (visible on 'advanced' click)
        var $subfieldsOptions = $(xmlToString($searchCriteria.children('subfields').children('select')))
                                .hide();
        $subfieldCell.append($subfieldsOptions);

        //Row 1, Column 4: filter picklist
        var $filtersCell = o.$searchTable.CswTable('cell', 1, 4);
        var $filtersOptions =  $(xmlToString($searchCriteria.children('propertyfilters').children('subfield[column=' + o.$subfield + ']').children('select')))
                               .hide();
        $filtersCell.append($filtersOptions);

        //Row 1, Column 5: list (fieldtype) options
        var $listCell = o.$searchTable.CswTable('cell', 1, 5);
        var $listOptions =  $(xmlToString($searchCriteria.children('filtersoptions')))
                            .hide();
        $listCell.append($listOptions);
                              
        //Row 1, Column 6: search box
        var $searchBoxCell = o.$searchTable.CswTable('cell', 1, 6);
        var $searchInput = $('<input type="text" name="search_input" id="search_input" autocomplete="on" autofocus="true" placeholder="' + propertyVal + ' search" width="200px" />');
        $searchBoxCell.append($searchInput);
                                            
        var $splitCell = o.$searchTable.CswTable('cell', 2, 1);
        var $splitCellTable = $splitCell.CswTable('init',{ID: 'split_cell_table', 
                                                            cellpadding: 1,
                                                            cellspacing: 1,
                                                            cellalign: 'center',
                                                            align: 'left'
                                                            });
        //Row 2, Column 1 (1/1): clear button
        var $clearButtonCell = $splitCellTable.CswTable('cell', 1, 1);
        var $clearButton = $('<input type="button" name="clear_button" id="clear_button" value="Clear" />')
                           .click(function() {
                                initOptions.viewid = '';
                                initOptions.nodetypeid = $nodeTypes.val();
                                init(initOptions);
                           });

        $clearButtonCell.append($clearButton);
                                            
        //Row 2, Column 1 (1/2): advanced link
        var $advancedLinkCell = $splitCellTable.CswTable('cell', 1, 2);
        var $advancedLink = $('<a href="#advanced">Advanced</a>')
                            .click(function() {
                                modAdvanced({
                                    '$link': $advancedLink,
                                    '$filtersOptions': $filtersOptions,
                                    '$defaultSubfield': $defaultSubField,
                                    '$subfieldsOptions': $subfieldsOptions,
                                    '$listOptions': $listOptions
                                    });
                            });
        $advancedLinkCell.append($advancedLink);
                                                    
        //Row 2, Column 6: search button
        var $searchButtonCell = o.$searchTable.CswTable('cell', 2, 6).attr({align:"right"});
        var $searchButton = $('<input type="button" name="search_button" id="search_button" value="Search" />');
        $searchButtonCell.append($searchButton);

        //Bottom Span
        var $bottomTable = o.$bottomspandiv.CswTable('init', {ID: 'change_search_tbl', 
                                                            cellpadding: 1,
                                                            cellspacing: 1,
                                                            cellalign: 'center',
                                                            align: 'right'});
        //Row 1, Column 1: load a search
        var $loadTableCell = $bottomTable.CswTable('cell', 1, 1);
        $loadTableCell.html('Load a Search:');
                                                                                        
        CswAjaxXml({ 
			'url': initOptions.SearchableViewsUrl,
			'data': "IsMobile=" + false + "&OrderBy=",
            'success': function($views) { 
                    log($views);                                    
                    //Row 1, Column 2: view select
                    var $viewSelectCell = $bottomTable.CswTable('cell', 1, 2);
                    var $viewSelect = $(xmlToString($views));
                    $viewSelectCell.append($viewSelect);
                }
            });

        //Row 1, Column 3: load button
        var $loadButtonCell = $bottomTable.CswTable('cell', 1, 3);
        var $loadButton = $('<input type="button" name="load_button" id="load_button" value="Load" />');
        $loadButtonCell.append($loadButton);

        //Row 2, Column 2: new custom search
        var $customSearchCell = $bottomTable.CswTable('cell', 2, 2);
        var $customSearch = $('<a href="#customsearch">New Custom Search</a>');
        $customSearchCell.append($customSearch);

    }

    function init(options)
    {
        var o = { 
			'RenderSearchUrl': '',
			'ExecViewSearchUrl': '',
            'ExecNodeSearchUrl': '',
            'SearchableViewsUrl': '',
            '$parent': '',
            '$searchXml': '',
            '$searchTable': '',
            viewid: '',
            nodetypeid: '',
            relatedidtype: '',
            'onSearch': function() { }
		};
		if(options) {
			$.extend(o, options);
		}
        var $titlespan = o.$parent.html('<span style="align: center;">Search</span>');
        var $topspan = o.$parent.html('<span></span>');
        var $topspandiv = $topspan.html('<div id="search_criteria_div"></div>');
        var $padding = o.$parent.html('<br/><br/><br/><br/><br/>');
        var $bottomspan = o.$parent.html('<span></span>');
        var $bottomspandiv = $bottomspan.html('<div id="change_search_div"></div>');

        CswAjaxXml({ 
		    'url': o.RenderSearchUrl,
		    'data': "ViewIdNum=" + o.viewid + "&SelectedNodeTypeIdNum=" + o.nodetypeid,
            'success': function($xml) { 
                                    
                $searchTable = $topspandiv.CswTable('init', { 
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
                        var nodeTypeId = $('#node_type_select').val();
                        var $property = $xml.children('nodetypeprops').children('properties').attr('defaultprop');
                        var $searchCriteria = $xml.children('nodetypeprops').children('propertyfilters').children('property[propname='+ $property +']');
                        var $filter = $searchCriteria.children('defaultsubfield').attr('filter');
                        var $subfield = $searchCriteria.children('defaultsubfield').attr('subfield');
                        var $fieldType = $searchCriteria.attr('fieldtype');

                        renderNodeTypeSearchContent({
                            '$parent': o.$parent,
                            '$searchXml': $xml,
                            '$searchTable': $searchTable,
                            '$bottomspandiv': $bottomspandiv,
                            'nodeTypeId': nodeTypeId,
                            '$property': $property,
                            '$filter': $filter,
                            '$subfield': $subfield,
                            '$fieldType': $fieldType,
                            'initOptions': o
                        });

                    }
                    case 'viewsearch':
                    {


                    }

                }
							        
			} // success
					
		}); // CswAjaxXml
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
                'onSearch': function() { },
                '$parent': $(this)
			};
			if(options) {
				$.extend(o, options);
			}
            init(o);
        }
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


