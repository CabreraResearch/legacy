/// <reference path="../jquery/jquery-1.5.1.js" />

; (function ($) {
	var PluginName = "CswSearch";

    function modAdvanced(options)
    {
        var isHidden;
        var o = {'$link': ''};
        if(options) $.extend(o,options);
        
        if('Advanced' == o.$link.text())
        {
            $('.csw_search_subfield_select').each(function() { $(this).show(); });
            $('.csw_search_filter_select').each(function() { $(this).show(); });
            $('.csw_search_default_filter').each(function() { $(this).hide(); });
            o.$link.text('Simple');
            isHidden = false;
        }
        else
        {
            $('.csw_search_subfield_select').each(function() { $(this).hide(); });
            $('.csw_search_filter_select').each(function() { $(this).hide(); });
            $('.csw_search_default_filter').each(function() { $(this).show(); });
            o.$link.text('Advanced');
            isHidden = true;
        }
        return isHidden; 
    } 

    function renderViewBasedSearchContent(options)
    {
        var o = {
            '$topspandiv': '',
            '$parent': '',
            '$propsXml': '',
            '$searchTable': '',
            'propsCount': 1,
            'initOptions': {}
        };

        if(options) $.extend(o,options);
        
        var initOptions = o.initOptions;
        
        //skip cell 1,1
        var andRow = 2;
        while(andRow <= o.propsCount)        
        {
            //Row i, Column 1: and
            var $andCell = o.$searchTable.CswTable('cell', andRow, 1)
                           .attr({align:"right"});
            var $andText = $('<span>&nbsp;and&nbsp;</span>');
            $andCell.append($andText);
            andRow++;
        }
        
        var propRow = 1;
        o.$propsXml.children('property').each( function() {
                var $thisProp = $(this);
                var $nodeType = $('<span>' +  $thisProp.attr('nodetypename') + '</span>')
                                .attr('id','nodetypepropid_' + $thisProp.attr('propid') )
                                .attr('name','nodetypepropid_' + $thisProp.attr('propid') )
                                .attr('class','csw_search_nodetype_static' );
                
                var $nodeTypeCell = o.$searchTable.CswTable('cell', propRow, 2)
                                    .append($nodeType);
                renderViewPropsAndControls({
                                '$parent': o.$searchTable,
                                '$thisProp': $thisProp,
                                'propRow': propRow,
                                'selectedSubfieldVal': '',
                                'selectedFilterVal': '',
                                'isHidden': true,
                });
                propRow++;
            });
        var bottomRow = propRow;
        o.$parent = $searchTable;
        renderSearchButtons({
                'bottomRow': bottomRow, 
                'bottomCell': 1, 
                '$parent': o.$parent,
                '$propsXml': o.$propsXml,
                'searchtype': 'viewsearch'
        });
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
            'propsCount': 1,
            'initOptions': {}
        };

        if(options) $.extend(o,options);
        
        var initOptions = o.initOptions;
        
        //Row 1, Column 1: empty (contains 'and' for View search)
        //Row 1, Column 2: nodetypeselect picklist
        var $typeSelectCell = o.$searchTable.CswTable('cell', 1, 2);
        var $nodeTypesSelect = $(xmlToString(o.$nodeTypesXml.children('select')))
                               .change( function() {
                                   var $thisSelect = $(this);
                                   o.$parent = o.$searchTable; 
                                   o.$nodeTypesSelect = $thisSelect;
                                   getNewProps( $thisSelect.val(), $thisSelect.find(':selected').attr('title'), o);
                                });
        o.$nodeTypesSelect = $nodeTypesSelect;
        $typeSelectCell.append($nodeTypesSelect);
        
        o.$parent = $searchTable;
        
        //prop row(s) 1-?, Columns 3-6
        renderNodePropsAndControls(o);
        renderSearchButtons({
                'bottomRow': (o.propsCount + 1),
                'bottomCell': 2, 
                '$parent': o.$parent,
                '$nodeTypesSelect': o.$nodeTypesSelect,
                '$propsXml': o.$propsXml,
                'searchtype': 'nodetypesearch'
        });
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

    function renderNodePropsAndControls(options)
    {
        var o = {
            '$propsXml': '',
            '$parent': '',
            '$nodeTypesSelect': '',
            'selectedPropVal': '',
            'selectedSubfieldVal': '',
            'selectedFilterVal': '',
            'propsCount': 1,
            'isHidden': true,
        };
        if(options) $.extend(o,options);
        
        var propRow = 1;
        while(propRow <= o.propsCount) //in case we want to add multiple rows later       
        {
            //Row propRow, Column 3: properties 
            var $propSelectCell = o.$parent.CswTable('cell', propRow, 3)
                                    .empty();
            var $props = $(xmlToString(o.$propsXml.children('properties').children('select')))
                            .change(function() {
                                    var r = {};
                                    $.extend(r,o);
                                    r.selectedPropVal = $(this).val();
                                    r.selectedSubfieldVal = '';
                                    r.selectedFilterVal  = '';
                                    renderPropsAndControls(r);
                            });
            if(o.selectedPropVal != '' )
            {
                $props.val(o.selectedPropVal).attr('selected',true);
            }
            $propSelectCell.append($props);
            var propertyId = $props.val(); //$props.find(':selected').val();
            var $selectedProp = o.$propsXml.children('propertyfilters').children('property[propid='+ propertyId +']');
        
            var $defaultFilter = $selectedProp.children('defaultsubfield').attr('filter');
            var fieldtype = $selectedProp.attr('fieldtype');

            //Row propRow, Column 4: default filter
            var $subfieldCell = o.$parent.CswTable('cell', propRow, 4)
                                .empty();
        
            var $defaultSubField = $('<span id="default_filter_' + propRow + '" name="default_filter_' + propRow + '" class="csw_search_default_filter">' + $defaultFilter + '</span>')
                                    .attr({align:"center"});
            if(!o.isHidden)
            {
                $defaultSubField.hide()
            }
            $subfieldCell.append($defaultSubField);
                    

            //Row propRow, Column 4: subfield picklist (visible on 'advanced' click)
            var $subfieldsOptions = $(xmlToString($selectedProp.children('subfields').children('select')))
                                    .change(function() {
                                        var r = {};
                                        $.extend(r,o);
                                        r.selectedPropVal = $props.val();
                                        r.selectedSubfieldVal = $(this).val();
                                        r.selectedFilterVal  = '';
                                        r.isHidden = false;
                                        renderPropsAndControls(r) });
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

            //Row propRow, Column 5: filter picklist
            var $filtersCell = o.$parent.CswTable('cell', propRow, 5)
                                .empty();

            var $filtersOptions =  $(xmlToString($selectedProp.children('propertyfilters').children('subfield[column=' + $subfield + ']').children('select')))
                                    .change(function() {
                                        var r = {};
                                        $.extend(r,o);
                                        r.selectedPropVal = $props.val();
                                        r.selectedSubfieldVal = $subfieldsOptions.val();
                                        r.selectedFilterVal  = $(this).val;
                                        r.isHidden = false;
                                        renderPropsAndControls(r) });
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

            //Row propRow, Column 6: search input
            var $searchBoxCell = o.$parent.CswTable('cell', propRow, 6)
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
                $searchInput = $('<input type="text" />')
                                .attr('name', 'search_input_nodetypepropid_' + propertyId )
                                .attr('id', 'search_input_nodetypepropid_' + propertyId )
                                .attr('class', 'csw_search_input' )
                                .attr('autocomplete','on')
                                .attr('autofocus','true')
                                .attr('placeholder', searchSuggest)
                                .attr({width:"200px"});
            }
            $searchBoxCell.append($searchInput);
            
            propRow++;
        }
    }

    function renderViewPropsAndControls(options)
    {
        var o = {
            '$parent': '',
            '$thisProp': '',
            'propRow': 1,
            'selectedSubfieldVal': '',
            'selectedFilterVal': '',
            'isHidden': true,
        };
        if(options) $.extend(o,options);

        var $selectedProp = o.$thisProp;            
        var propRow = o.propRow;
        var propertyId = o.$thisProp.attr('propid');
                
        //Row propRow, Column 3: property
        var $propSelectCell = o.$parent.CswTable('cell', propRow, 3)
                                .empty();
        var $props = $('<span id="nodetypepropid_' + propertyId + '">' + o.$thisProp.attr('propname') + '</span>');
        $propSelectCell.append($props);

        
        var $defaultFilter = $selectedProp.children('defaultsubfield').attr('filter');
        var fieldtype = $selectedProp.attr('fieldtype');

        //Row propRow, Column 4: default filter
        var $subfieldCell = o.$parent.CswTable('cell', propRow, 4)
                            .empty();
        
        var $defaultSubField = $('<span id="default_filter_' + propRow + '" name="default_filter_' + propRow + '" class="csw_search_default_filter">' + $defaultFilter + '</span>')
                                .attr({align:"center"});
        if(!o.isHidden)
        {
            $defaultSubField.hide()
        }
        $subfieldCell.append($defaultSubField);
                    

        //Row propRow, Column 4: subfield picklist (visible on 'advanced' click)
        var $subfieldsOptions = $(xmlToString($selectedProp.children('subfields').children('select')))
                                .change(function() {
                                    var r = {};
                                    $.extend(r,o);
                                    r.$thisProp = $selectedProp;
                                    r.selectedSubfieldVal = $(this).val();
                                    r.selectedFilterVal  = '';
                                    r.isHidden = false;
                                    renderViewPropsAndControls(r) });
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

        //Row propRow, Column 5: filter picklist
        var $filtersCell = o.$parent.CswTable('cell', propRow, 5)
                            .empty();

        var $filtersOptions =  $(xmlToString($selectedProp.children('propertyfilters').children('subfield[column=' + $subfield + ']').children('select')))
                                .change(function() {
                                    var r = {};
                                    $.extend(r,o);
                                    r.$thisProp = $selectedProp;
                                    r.isHidden = false;
                                    r.selectedSubfieldVal = $subfieldsOptions.val();
                                    r.selectedFilterVal  = $filtersOptions.val();
                                    renderViewPropsAndControls(r) });
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

        //Row propRow, Column 6: search input
        var $searchBoxCell = o.$parent.CswTable('cell', propRow, 6)
                        .empty();
        var $searchInput;
        if( fieldtype == 'List' )
        {
            $searchInput = $(xmlToString($selectedProp.children('filtersoptions').children('select')));
        }
        else
        {
            var searchSuggest = o.$thisProp.attr('nodetypename');
            searchSuggest += "'s " +  $subfieldsOptions.find(':selected').text();
            $searchInput = $('<input type="text" />')
                            .attr('name', 'search_input_nodetypepropid_' + propertyId )
                            .attr('id', 'search_input_nodetypepropid_' + propertyId )
                            .attr('class', 'csw_search_input' )
                            .attr('autocomplete','on')
                            .attr('autofocus','true')
                            .attr('placeholder', searchSuggest)
                            .attr({width:"200px"});
        }
        $searchBoxCell.append($searchInput);
    }

    function renderSearchButtons(options)
    {
        var o = {
            'bottomRow': 2,
            'clearBtnCell': 1,
            'searchBtnCell': 6,
            '$parent': '',
            '$nodeTypesSelect': '',
            '$propsXml': '',
            'searchtype': ''
        };
        if(options) $.extend(o,options);

        //Row i, Column 1: cell for clear/advanced                                            
        var $splitCell = o.$parent.CswTable('cell', o.bottomRow, o.bottomCell)
                         .empty();
        
        var $splitCellTable = $splitCell.CswTable('init',{ID: 'split_cell_table', 
                                                            cellpadding: 1,
                                                            cellspacing: 1,
                                                            cellalign: 'left',
                                                            align: 'left'
                                                            });
        //Row i, Column 1 (1/1): clear button
        var $clearButtonCell = $splitCellTable.CswTable('cell', 1, 1)
                               .empty();
        var $clearButton = $('<input type="button" name="clear_button" id="clear_button" value="Clear" />')
                           .click(function() {
                                reInit(o);
                           });

        $clearButtonCell.append($clearButton);
                                            
        //Row i, Column 1 (1/2): advanced link
        var $advancedLinkCell = $splitCellTable.CswTable('cell', 1, 2)
                                .empty();
        var $advancedLink = $('<a href="#advanced">Advanced</a>')
                            .click(function() {
                                o.isHidden = modAdvanced({
                                    '$link': $advancedLink
                                    });
                            });
        $advancedLinkCell.append($advancedLink);
                                                    
        //Row i, Column 5: search button
        var $searchButtonCell = o.$parent.CswTable('cell', o.bottomRow, o.searchBtnCell)
                                .attr({align:"right"})
                                .empty();
        var $searchButton = $('<input type="button" name="search_button" id="search_button" value="Search" />')
                            .click(function() {
                                    doSearch({
                                        '$nodeTypesSelect': o.$nodeTypesSelect,
                                        '$propsXml': o.$propsXml,
                                        'searchtype': o.searchtype
                                    });
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
            'searchtype': '',
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
                o.searchtype = $xml.attr('searchtype');
                switch(o.searchtype)
                {
                    case 'nodetypesearch':
                    {
                        $searchTable = $topspandiv.CswTable('init', { 
                                ID: 'search_tbl', 
                                cellpadding: 1,
                                cellspacing: 1,
                                cellalign: 'center',
                                align: 'center'
                                });
                        var $nodePropsXml = $xml.children('nodetypeprops');
                        var $nodeTypesXml = $xml.children('nodetypes');
                        renderNodeTypeSearchContent({
                            '$parent': o.$parent,
                            '$nodeTypesXml': $nodeTypesXml, 
                            '$propsXml': $nodePropsXml,
                            '$searchTable': $searchTable,
                            '$topspandiv': $topspandiv,
                            'propsCount': 1, //only one NodeTypeProp by default
                            'initOptions': o
                        });
                        break;
                    }
                    case 'viewsearch':
                    {
                        $searchTable = $topspandiv.CswTable('init', { 
                                ID: 'search_tbl', 
                                cellpadding: 1,
                                cellspacing: 1,
                                cellalign: 'left',
                                align: 'center'
                                });
                        var $viewPropsXml = $xml.children('properties');
                        renderViewBasedSearchContent({
                            '$parent': o.$parent,
                            '$propsXml': $viewPropsXml,
                            '$searchTable': $searchTable,
                            '$topspandiv': $topspandiv,
                            'propsCount': $xml.children('properties').children('property').size(),
                            'initOptions': o
                        });
                        break;
                    }

                }
                getBottomSpan({'$bottomspandiv': $bottomspandiv});							        
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

    function doSearch(options)
    {
        var o = {
            '$nodeTypesSelect': '',
            '$propsXml': '',
            'searchtype': ''
        };

        var searchOpt;

        if(options) $.extend(o,options);
        
        var props = [];
        var propno = 1;
        switch(o.searchtype)
        {
            case 'nodetypesearch':
            {
                o.doSearchUrl = '/NbtWebApp/wsNBT.asmx/doNodeTypeSearch';
                var objectPk = o.$nodeTypesSelect.val();
                var relatedIdType = o.$nodeTypesSelect.find(':selected').attr('title');

                $('.csw_search_properties_select').each(function() {
                        var $thisProp = $(this);
                        var propName = $thisProp.text();
                        var propId = $thisProp.val();
                        var subField = $('#subfield_select_nodetypepropid_' + propId).find(':selected').text();
                        var filter = $('#filter_select_nodetypepropid_' + propId).find(':selected').val();
                        var searchText = $('#search_input_nodetypepropid_' + propId).val();
                        
                        var thisNodeProp = {
                                objectpk: objectPk,
                                relatedidtype: relatedIdType,
                                propid: propId,
                                subfield: subField,
                                filter: filter,
                                searchtext: searchText  
                                };
                        props.push( thisNodeProp );
                    });
                searchOpt = {
                    'nodetypeprops' : props
                };
                break;
            }
            case 'viewsearch':
            {
                o.doSearchUrl = '/NbtWebApp/wsNBT.asmx/doViewSearch';
                o.$propsXml.children('property').each(function() {
                        var $thisProp = $(this);
                        var propName = $thisProp.val();
                        var propId = $thisProp.attr('propid');
                        var searchText = $('#search_input_nodetypepropid_' + propId).val();
                        if(searchText != '')
                        {
                            var subField = $('#subfield_select_nodetypepropid_' + propId).find(':selected').text();
                            var filter = $('#filter_select_nodetypepropid_' + propId).find(':selected').val();
                            var viewPropType = $thisProp.attr('viewproptype');
                            var thisNodeProp = {
                                    viewproptype: viewPropType,
                                    propid: propId,
                                    subfield: subField,
                                    filter: filter,
                                    searchtext: searchText  
                                    };
                            props.push( thisNodeProp );
                        }
                    });
                searchOpt = { 
                        viewprops: props,
                        viewid: $.CswCookie('get', CswCookieName.CurrentView.ViewId)
                };
                break;
            }
        }

        if(searchOpt)
        {
            CswAjaxJSON({ 
			'url': o.doSearchUrl,
			'data': "{SearchJson: \"" + jsonToString(searchOpt) + "\"}",
            'success': function(view) { 
                    $.CswCookie('set', CswCookieName.CurrentView.ViewId, view.sessionviewid);
                    $.CswCookie('set', CswCookieName.CurrentView.ViewMode, view.viewmode);
                    window.location.replace('NewMain.html');
                }
            });
        }
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


