/// <reference path="../jquery/jquery-1.5.1.js" />

; (function ($) {
	var PluginName = "CswSearch";

    function modAdvanced(options)
    {
        var isHidden;
        var o = {
                '$link': '',
                '$filtersOptions': '',
                '$subfieldCell': '',
                '$defaultSubfield': '',
                '$subfieldsOptions': ''
        };
        if(options) $.extend(o,options);
        
        if('Advanced' == o.$link.text())
        {
            o.$filtersOptions.show();
            o.$defaultSubfield.hide();
            o.$subfieldsOptions.show();
            o.$link.text('Hide');
            isHidden = false;
        }
        else
        {
            o.$filtersOptions.hide();
            o.$defaultSubfield.show();
            o.$subfieldsOptions.hide();
            o.$link.text('Advanced');
            isHidden = true;
        }
        return isHidden; 
    } 

    function renderNodeTypeSearchContent(options)
    {
        var o = {
            '$topspandiv': '',
            '$parent': '',
            '$nodeTypesXml': '',
            '$propsXml': '',
            '$searchTable': '',
            '$nodeTypesSelect': '',
            'initOptions': {}
        };

        if(options) $.extend(o,options);
        
        var initOptions = o.initOptions;
        
        
        //Row 1, Column 1: nodetypeselect picklist
        var $typeSelectCell = o.$searchTable.CswTable('cell', 1, 1);
        var $nodeTypesSelect = $(xmlToString(o.$nodeTypesXml.children('select')))
                               .change( function() {
                                   o.$parent = o.$searchTable; 
                                   o.$nodeTypesSelect = $(this);
                                   getNewProps( $(this).val(), $(this).find(':selected').attr('label'), o);
                                });
        o.$nodeTypesSelect = $nodeTypesSelect;
        $typeSelectCell.append($nodeTypesSelect);
        
        o.$parent = $searchTable;
        //row 1-2, Columns 2-6
        renderPropsAndControls(o);
    }

    function getNewProps(objectPk,relatedIdType,options)
    {
        CswAjaxXml({ 
		            'url': '/NbtWebApp/wsNBT.asmx/getNodeTypeSearchProps',
		            'data': "RelatedIdType=" + relatedIdType + "&ObjectPk=" + objectPk,
                    'success': function($xml) { 
                            options.$propsXml = $xml;
                            renderPropsAndControls(options);
                    }
                });
    }

    function renderPropsAndControls(options)
    {
        var o = {
            '$propsXml': '',
            '$parent': '',
            '$nodeTypesSelect': '',
            'selectedPropVal': '',
            'selectedSubfieldVal': '',
            'selectedFilterVal': '',
            'isHidden': true
        };
        if(options) $.extend(o,options);
        
        var nodeTypeId =  o.$nodeTypesSelect.val();
        var relatedIdType = o.$nodeTypesSelect.find(':selected').attr('label');
                
        //Row 1, Column 2: properties picklist
        var $propSelectCell = o.$parent.CswTable('cell', 1, 2)
                              .empty();
        
        var $props = $(xmlToString(o.$propsXml.children('properties').children('select')))
                     .change(function() {
                                o.selectedPropVal = $(this).val();
                                o.selectedSubfieldVal = '';
                                o.selectedFilterVal  = '';
                                renderPropsAndControls(o);
                     });
        if(o.selectedPropVal != '' )
        {
            $props.val(o.selectedPropVal).attr('selected',true);
        }
        $propSelectCell.append($props);
        var propertyId = $props.find(':selected').val();
        var $selectedProp = o.$propsXml.children('propertyfilters').children('property[propid='+ propertyId +']');
        
        var $defaultFilter = $selectedProp.children('defaultsubfield').attr('filter');
        var fieldtype = $selectedProp.attr('fieldtype');

        //Row 1, Column 3: default filter
        var $subfieldCell = o.$parent.CswTable('cell', 1, 3)
                            .empty();
        
        var $defaultSubField = $('<span>' + $defaultFilter + '</span>')
                               .attr({align:"center"});
        if(!o.isHidden)
        {
            $defaultSubField.hide()
        }
        $subfieldCell.append($defaultSubField);
                    

        //Row 1, Column 3: subfield picklist (visible on 'advanced' click)
        var $subfieldsOptions = $(xmlToString($selectedProp.children('subfields').children('select')))
                                .change(function() {
                                    o.selectedPropVal = $props.val();
                                    o.selectedSubfieldVal = $(this).val();
                                    o.selectedFilterVal  = '';
                                    renderPropsAndControls(o) });
        if(o.isHidden)
        {
            $subfieldsOptions.hide();
        }
        if(o.selectedSubfieldVal != '')
        {
            $subfieldsOptions.val(o.selectedSubfieldVal).attr('selected',true);
        }
        $subfieldCell.append($subfieldsOptions);
        var $subfield = $subfieldsOptions.find(':selected').val();

        //Row 1, Column 4: filter picklist
        var $filtersCell = o.$parent.CswTable('cell', 1, 4)
                           .empty();

        var $filtersOptions =  $(xmlToString($selectedProp.children('propertyfilters').children('subfield[column=' + $subfield + ']').children('select')))
                               .change(function() {
                                    o.selectedPropVal = $props.val();
                                    o.selectedSubfieldVal = $subfieldsOptions.val();
                                    o.selectedFilterVal  = $(this).val;
                                    renderPropsAndControls(o) });
        if(o.isHidden)
        {
            $filtersOptions.hide();
        }
        if(o.selectedFilterVal != '')
        {
            $filtersOptions.val(o.selectedFilterVal).attr('selected',true);
        }
        $filtersCell.append($filtersOptions);
        var $filter = $filtersOptions.find(':selected').val();

        //Row 1, Column 5: search input
        var $searchBoxCell = o.$parent.CswTable('cell', 1, 5)
                        .empty();
        var $searchInput;
        if( fieldtype == 'List' )
        {
            $searchInput = $(xmlToString($selectedProp.children('filtersoptions').children('select')));
        }
        else
        {
            var searchSuggest = $props.find(':selected').text();
            if(searchSuggest != $subfieldsOptions.find(':selected').text() )
            {
                searchSuggest += "'s " +  $subfieldsOptions.find(':selected').text();
            }  
            $searchInput = $('<input type="text" name="search_input" id="search_input" autocomplete="on" autofocus="true" placeholder="' + searchSuggest + '" width="200px" />');
        }
         $searchBoxCell.append($searchInput);
                                            
        var $splitCell = o.$parent.CswTable('cell', 2, 1)
                         .empty();
        
        var $splitCellTable = $splitCell.CswTable('init',{ID: 'split_cell_table', 
                                                            cellpadding: 1,
                                                            cellspacing: 1,
                                                            cellalign: 'center',
                                                            align: 'left'
                                                            });
        //Row 2, Column 1 (1/1): clear button
        var $clearButtonCell = $splitCellTable.CswTable('cell', 1, 1)
                               .empty();
        var $clearButton = $('<input type="button" name="clear_button" id="clear_button" value="Clear" />')
                           .click(function() {
                                reInit(o);
                           });

        $clearButtonCell.append($clearButton);
                                            
        //Row 2, Column 1 (1/2): advanced link
        var $advancedLinkCell = $splitCellTable.CswTable('cell', 1, 2)
                                .empty();
        var $advancedLink = $('<a href="#advanced">Advanced</a>')
                            .click(function() {
                                o.isHidden = modAdvanced({
                                    '$link': $advancedLink,
                                    '$filtersOptions': $filtersOptions,
                                    '$defaultSubfield': $defaultSubField,
                                    '$subfieldsOptions': $subfieldsOptions
                                    });
                            });
        $advancedLinkCell.append($advancedLink);
                                                    
        //Row 2, Column 5: search button
        var $searchButtonCell = o.$parent.CswTable('cell', 2, 5)
                                .attr({align:"right"})
                                .empty();
        var $searchButton = $('<input type="button" name="search_button" id="search_button" value="Search" />')
                            .click(function() {
                                    var searchOpt = {
                                            nodetypeprop: {
                                                objectpk: nodeTypeId,
                                                relatedidtype: relatedIdType,
                                                propid: $props.val(),
                                                subfield: $subfieldsOptions.find(':selected').text(),
                                                filter: $filtersOptions.val(),
                                                searchtext: $searchInput.text()  
                                                }
                                    };
                                    doNodesSearch(searchOpt);
                            });
        $searchButtonCell.append($searchButton);
    }

    function reInit(options)
    {
        if(options) 
        {
            o = {
                '$parent': '',
                '$nodeTypesXml': '',
                '$propsXml': '',
                '$searchTable': '',
                '$topspandiv': '',
                'initOptions': ''
            };
            $.extend(o,options);

            o.$topspandiv.empty();
        
            $searchTable = o.$topspandiv.CswTable('init', { 
                                            ID: 'search_tbl', 
                                            cellpadding: 1,
                                            cellspacing: 1,
                                            cellalign: 'center',
                                            align: 'center'
                                            });

            renderNodeTypeSearchContent({
                '$parent': o.$parent,
                '$nodeTypesXml': o.$nodeTypesXml,
                '$propsXml': o.$propsXml,
                '$searchTable': $searchTable,
                '$topspandiv': o.$topspandiv,
                'initOptions': o.initOptions
            });
        }
    }

    function init(options)
    {
        var o = { 
            '$parent': '',
            '$nodeTypesXml': '',
            '$propsXml': '',
            '$searchTable': '',
            '$topspandiv': '',
            viewid: '',
            'onSearch': function() { }
		};
		if(options) {
			$.extend(o, options);
		}
        
        //var $titlespan = $('<span style="align: center;">Search</span>');
        //o.$parent.append( $titlespan );
       
        var $topspan = $('<span></span>');
        o.$parent.append( $topspan );

        var $topspandiv = $('<div id="search_criteria_div"></div>');
        $topspan.append( $topspandiv );

        var $padding = $('<br/><br/><br/><br/><br/>');
        o.$parent.append( $padding );

        var $bottomspan = $('<span></span>');
        o.$parent.append($bottomspan);
        
        var $bottomspandiv = $('<div id="change_search_div"></div>');
        $bottomspan.append($bottomspandiv);

        CswAjaxXml({ 
		    'url': '/NbtWebApp/wsNBT.asmx/getClientSearchXml',
		    'data': "ViewIdNum=" + o.viewid + "&SelectedNodeTypeIdNum=" + o.nodetypeid,
            'success': function($xml) { 

                var $nodeTypesXml = $xml.children('nodetypes');
                var $propsXml = $xml.children('nodetypeprops');
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
                        renderNodeTypeSearchContent({
                            '$parent': o.$parent,
                            '$nodeTypesXml': $nodeTypesXml, 
                            '$propsXml': $propsXml,
                            '$searchTable': $searchTable,
                            '$topspandiv': $topspandiv,
                            'initOptions': o
                        });
                        getBottomSpan({'$bottomspandiv': $bottomspandiv});

                    }
                    case 'viewsearch':
                    {


                    }

                }
							        
			} // success
					
		}); // CswAjaxXml
    }

    function getBottomSpan(options)
    {
        var o = {
            '$bottomspandiv': '',
            'SearchableViewsUrl': '/NbtWebApp/wsNBT.asmx/getSearchableViews'
        };
        
        if(options) $.extend(o,options);

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
			'url': o.SearchableViewsUrl,
			'data': "IsMobile=" + false + "&OrderBy=",
            'success': function($views) { 
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

    function doNodesSearch(options)
    {
         var o = {
            nodetypeprop: {
                objectpk: '',
                relatedidtype: '',
                propid: '',
                subfield: '',
                filter: '',
                searchtext: ''
                }
        };
        
        if(options) $.extend(o,options);
        log(JSON.stringify(o));                                                                                       
        CswAjaxJSON({ 
			'url': '/NbtWebApp/wsNBT.asmx/doNodeTypeSearch',
			'data': '{"SearchJson": "' + $.param(o) + '"}',
            'success': function($viewid) { 
                    alert('hey');
                    //load the view
                }
            });
    }

	var methods = {
	
		'getSearchForm': function(options) 
		{
			var o = { 
				viewid: '',
                nodetypeid: '',
                relatedidtype: '',
                'onSearch': function() { },
                '$parent': $(this)
			};
			if(options) {
				$.extend(o, options);
			}
            init(o, true);
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


