﻿; (function ($) {
	$.fn.CswSearch = function (options) {

        var o = { 
            //DOM to persist
            '$parent': '',
            '$searchTable': '',
                
            //URLs
            'getNewPropsUrl': '/NbtWebApp/wsNBT.asmx/getNodeTypeSearchProps',
            'doViewSearchUrl': '/NbtWebApp/wsNBT.asmx/doViewSearch',
            'doNodeSearchUrl': '/NbtWebApp/wsNBT.asmx/doNodeTypeSearch',
            'getSearchableViewsUrl': '/NbtWebApp/wsNBT.asmx/getSearchableViews',
            'getClientSearchXmlUrl': '/NbtWebApp/wsNBT.asmx/getClientSearchXml',

            //options
			'viewid': '',
            'nodetypeid': '',
            'cswnbtnodekey': '',
            'relatedidtype': '',
            'idprefix': 'csw',
            'propsCount': 1,
            'isHidden': true,
                
            //XML to persist
            '$propsXml': '',
            '$nodeTypesXml': '',
            '$nodeTypesSelect': '',

            'onSearchSubmit': function (view) {}, 

            //For submit
            'selectedPropVal': '',
            'selectedSubfieldVal': '',
            'selectedFilterVal': '',

            'bottomRow': 2,
            'bottomCell': 1,
            'propRow': '',
            'clearBtnCell': 1,
            'searchBtnCell': 6,
            'searchtype': ''
		};
		
        if(options) $.extend(o, options);
        
        var $parent = $(this);
        var $cswSearchForm = $parent.CswDOM('div',{ID: 'CswSearchForm', prefix: o.idprefix});
        o.$parent = $cswSearchForm; //refactor $parent for name clarity
        
        var $topspan = $cswSearchForm.CswDOM('span');

        var $topspandiv = $topspan.CswDOM('div',{
                                    ID: 'search_criteria_div',
                                    prefix: o.idprefix});
        
        //o.$cswSearchForm.CswDOM('break',{count: 5});

        //var $bottomspan = $cswSearchForm.CswDOM('span');
        
        //var $bottomspandiv = $bottomspan.CswDOM('div',{
        //                            ID: 'change_viewbuilder_div',
        //                            prefix: o.idprefix });


        init();

        function modAdvanced(options)
        {
            var isHidden;
            var o = {
                '$link': ''
            };
            if(options) $.extend(o,options);
    
            if('Advanced' === o.$link.text())
            {
                $('.csw_viewbuilder_subfield_select').each(function() { $(this).show(); });
                $('.csw_viewbuilder_filter_select').each(function() { $(this).show(); });
                $('.csw_viewbuilder_default_filter').each(function() { $(this).hide(); });
                o.$link.text('Simple');
                isHidden = false;
            }
            else
            {
                $('.csw_viewbuilder_subfield_select').each(function() { $(this).hide(); });
                $('.csw_viewbuilder_filter_select').each(function() { $(this).hide(); });
                $('.csw_viewbuilder_default_filter').each(function() { $(this).show(); });
                o.$link.text('Advanced');
                isHidden = true;
            }
            return isHidden; 
        } // modAdvanced()

        function renderViewBasedSearchContent()
        {
            //skip cell 1,1
            var andRow = 2;
            while(andRow <= o.propsCount) //eventually this will be configurable: and/or, or, and/not, etc
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
                    var $nodeTypeCell = o.$searchTable.CswTable('cell', propRow, 2);
                    var nodeTypeId = makeId({ID: 'viewbuilderpropid', suffix: $thisProp.attr('propid'), prefix: o.idprefix});
                    var $nodeType = $nodeTypeCell.CswDOM('span',{
                                                                ID: nodeTypeId,
                                                                value: $thisProp.attr('metadatatypename'),
                                                                cssclass: 'csw_viewbuilder_metadatatype_static'})
                                                  .attr('relatedidtype',$thisProp.attr('relatedidtype') );
                    o.$parent = o.$searchTable;
                    o.selectedSubfieldVal = ''; 
                    o.selectedFilterVal = '';
                    o.isHidden = true;               
                    renderViewPropsAndControls({
                        '$thisProp': $thisProp,
                        'propRow': propRow
                    });
                    propRow++;
            });
            
            o.bottomRow = propRow;
            o.bottomCell = 1;
            o.searchtype = 'viewsearch';
            o.$parent = o.$searchTable;
            
            renderSearchButtons();
        } // renderViewBasedSearchContent()

        function renderNodeTypeSearchContent()
        {
            //Row 1, Column 1: empty (contains 'and' for View search)
            //Row 1, Column 2: nodetypeselect picklist
            var $typeSelectCell = o.$searchTable.CswTable('cell', 1, 2);
            var $nodeTypesSelect = $(xmlToString(o.$nodeTypesXml.children('select')))
                                   .change( function() {
                                       var $thisSelect = $(this);
                                       var r = {
                                            'objectPk': $thisSelect.val(),
                                            'relatedIdType': $thisSelect.find(':selected').attr('title'),
                                            'cswnbtnodekey': '',
                                            'idprefix': '',
                                            '$propsXml': '',
                                            '$parent': o.$searchTable,
                                            '$nodeTypesSelect': $thisSelect 
                                       };
                                       $.extend(o,r);
                                       getNewProps();  });
            o.$nodeTypesSelect = $nodeTypesSelect;
            $typeSelectCell.append($nodeTypesSelect);
        
            o.$parent = o.$searchTable;
        
            //prop row(s) 1-?, Columns 3-6
            renderNodePropsAndControls();
            o.bottomRow = (o.propsCount + 1);
            o.bottomCell = 2;
            o.searchtype = 'nodetypesearch';
            renderSearchButtons();
        } // renderNodeTypeSearchContent()

        function getNewProps()
        {
            CswAjaxXml({ 
		                'url': o.getNewPropsUrl,
		                'data': "RelatedIdType=" + o.relatedIdType + "&ObjectPk=" + o.nodetypeid + "&IdPrefix=" + o.idprefix + "&NodeKey=" + o.cswnbtnodekey,
                        'success': function($xml) { 
                                o.$propsXml = $xml;
                                renderNodePropsAndControls();
                        }
                    });
        } // getNewProps()

        function renderNodePropsAndControls()
        {
            var propRow = 1;
            while(propRow <= o.propsCount) //in case we want to add multiple rows later       
            {
                //Row propRow, Column 3: properties 
                var $propSelectCell = o.$parent.CswTable('cell', propRow, 3)
                                        .empty();
                var $props = $(xmlToString(o.$propsXml.children('properties').children('select')))
                                .change(function() {
                                        var $this = $(this);
                                        var r = {
                                            'selectedPropVal': $this.val(),
                                            'selectedSubfieldVal': '',
                                            'selectedFilterVal': ''
                                        };
                                        $.extend(o,r);
                                        renderNodePropsAndControls(); });
                                
                if(o.selectedPropVal !== '' )
                {
                    $props.val(o.selectedPropVal).attr('selected',true);
                }
                $propSelectCell.append($props);
                var propertyId = $props.find(':selected').val();
                var $selectedProp = o.$propsXml.children('propertyfilters').children('property[propid='+ propertyId +']');
        
                var $defaultFilter = $selectedProp.children('defaultsubfield').attr('filter');
                var fieldtype = $selectedProp.attr('fieldtype');

                //Row propRow, Column 4: default filter
                var $subfieldCell = o.$parent.CswTable('cell', propRow, 4)
                                    .empty();
        
                var $defaultSubField =  $subfieldCell.CswDOM('span',{
                                                                ID: 'default_filter_' + propRow,
                                                                prefix: o.idprefix,
                                                                value: $defaultFilter,
                                                                cssclass: 'csw_viewbuilder_default_filter'
                                                            })
                                        .attr({align:"center"});
                if(!o.isHidden)
                {
                    $defaultSubField.hide()
                }

                //Row propRow, Column 4: subfield picklist (visible on 'advanced' click)
                var $subfieldsOptions = $(xmlToString($selectedProp.children('subfields').children('select')))
                                        .change(function() {
                                            var $this = $(this);
                                            var r = {
                                                'selectedPropVal': $props.val(),
                                                'selectedSubfieldVal': $this.val(),
                                                'selectedFilterVal': '',
                                                'isHidden': false
                                            };
                                            $.extend(o,r);
                                            renderNodePropsAndControls() });
                if(o.isHidden)
                {
                    $subfieldsOptions.hide();
                }
                if(o.selectedSubfieldVal !== '')
                {
                    $subfieldsOptions.val(o.selectedSubfieldVal).attr('selected',true);
                }
                $subfieldCell.append($subfieldsOptions);
                var $subfield = $subfieldsOptions.find(':selected').val();
                var defaultValue = $subfieldsOptions.find(':selected').attr('defaultvalue');

                //Row propRow, Column 5: filter picklist
                var $filtersCell = o.$parent.CswTable('cell', propRow, 5)
                                    .empty();

                var $filtersOptions =  $(xmlToString($selectedProp.children('propertyfilters').children('subfield[column=' + $subfield + ']').children('select')))
                                        .change(function() {
                                            var $this = $(this);
                                            var r = {
                                                '$thisProp': $props.val(),
                                                'selectedSubfieldVal': $subfieldsOptions.val(),
                                                'selectedFilterVal': $this.val(),
                                                'isHidden': false
                                            };
                                            $.extend(o,r);
                                            renderNodePropsAndControls() });
                if(o.isHidden)
                {
                    $filtersOptions.hide();
                }
                if(o.selectedFilterVal !== '')
                {
                    $filtersOptions.val(o.selectedFilterVal).attr('selected',true);
                }
                $filtersCell.append($filtersOptions);
                var $filter = $filtersOptions.find(':selected').val();

                //Row propRow, Column 6: search input
                var $searchBoxCell = o.$parent.CswTable('cell', propRow, 6)
                                .empty();
                var searchInputId = makeId({'ID': 'search_input_viewbuilderpropid', 'suffix': propertyId, 'prefix': o.idprefix});
                if( fieldtype === 'List' )
                {
                    $searchBoxCell.append( $(xmlToString($selectedProp.children('filtersoptions').children('select'))) );
                }
                else if( fieldtype === 'Logical' )
                {
                    $searchBoxCell.CswTristateCheckBox('init',{'ID': searchInputId}); 
                }
                else
                {
                    var searchSuggest;
                    if( defaultValue !== '' && defaultValue != undefined )
                    {
                        searchSuggest = defaultValue;
                    }
                    else
                    {
                        searchSuggest = $props.find(':selected').text();
                        if(searchSuggest !== $subfieldsOptions.find(':selected').text() )
                        {
                            searchSuggest += "'s " +  $subfieldsOptions.find(':selected').text();
                        }  
                    }
                    var $searchInput = $searchBoxCell.CswDOM('input',{
                                                            ID: searchInputId,
                                                            type: 'text',
                                                            cssclass: 'csw_viewbuilder_input',
                                                            placeholder: searchSuggest })
                                                    .attr('autocomplete','on')
                                                    .attr('autofocus','true')
                                                    .attr({width:"200px"});
                }
            
                propRow++;
            }
        } // renderNodePropsAndControls()

        function renderViewPropsAndControls(options)
        {
            var ren = {
                '$thisProp': '',
                'propRow': ''
            };
            if(options) $.extend(ren,options);

            var $selectedProp = ren.$thisProp;            
            var propRow = ren.propRow;
            var propertyId = $selectedProp.attr('propid');
            var propertyName = $selectedProp.attr('propname');
            var filtArbitraryId = $selectedProp.attr('filtarbitraryid');
                
            //Row propRow, Column 3: property
            var $propSelectCell = o.$parent.CswTable('cell', propRow, 3)
                                    .empty();
            var propId = makeId({ID: propertyId, prefix: o.idprefix});
            var $props = $propSelectCell.CswDOM('span',{
                                                    ID: propId,
                                                    value: propertyName});
        
            var $defaultFilter = $selectedProp.children('defaultsubfield').attr('filter');
            var fieldtype = $selectedProp.attr('fieldtype');

            //Row propRow, Column 4: default filter
            var $subfieldCell = o.$parent.CswTable('cell', propRow, 4)
                                .empty();
            var defaultSubFieldId = makeId({ID: 'default_filter', suffix: filtArbitraryId, prefix: o.idprefix});
            var $defaultSubField = $subfieldCell.CswDOM('span', {
                                                    ID: defaultSubFieldId,
                                                    value: $defaultFilter,
                                                    cssclass: 'csw_viewbuilder_default_filter' })
                                                .attr({align:"center"});
            if(!o.isHidden)
            {
                $defaultSubField.hide()
            }

            //Row propRow, Column 4: subfield picklist (visible on 'advanced' click)
            var $subfieldsOptions = $(xmlToString($selectedProp.children('subfields').children('select')))
                                    .change(function() {
                                        var $this = $(this);
                                        var r = {
                                            'selectedSubfieldVal': $this.val(),
                                            'selectedFilterVal': '',
                                            'isHidden': false
                                        };
                                        $.extend(o,r);
                                        renderViewPropsAndControls({'$thisProp': $selectedProp, 'propRow': propRow }); });
            if(o.isHidden)
            {
                $subfieldsOptions.hide();
            }
            if(o.selectedSubfieldVal !== '')
            {
                $subfieldsOptions.val(o.selectedSubfieldVal).attr('selected',true);
            }
            $subfieldCell.append($subfieldsOptions);
            var $subfield = $subfieldsOptions.find(':selected').val();
            var defaultValue = $subfieldsOptions.find(':selected').attr('defaultvalue');

            //Row propRow, Column 5: filter picklist
            var $filtersCell = o.$parent.CswTable('cell', propRow, 5)
                                .empty();

            var $filtersOptions =  $(xmlToString($selectedProp.children('propertyfilters').children('subfield[column=' + $subfield + ']').children('select')))
                                    .change(function() {
                                        var $this = $(this);
                                        var r = {
                                            'selectedSubfieldVal': $subfieldsOptions.val(),
                                            'selectedFilterVal': $filtersOptions.val(),
                                            'isHidden': false
                                        };
                                        $.extend(o,r);
                                        renderViewPropsAndControls({'$thisProp': $selectedProp, 'propRow': propRow }); });
            if(o.isHidden)
            {
                $filtersOptions.hide();
            }
            if(o.selectedFilterVal !== '')
            {
                $filtersOptions.val(o.selectedFilterVal).attr('selected',true);
            }
            $filtersCell.append($filtersOptions);
            var $filter = $filtersOptions.find(':selected').val();

            //Row propRow, Column 6: search input
            var $searchBoxCell = o.$parent.CswTable('cell', propRow, 6)
                            .empty();
         
            if( fieldtype === 'List' )
            {
                $searchBoxCell.append( $(xmlToString($selectedProp.children('filtersoptions').children('select'))) );
            }
            else if( fieldtype === 'Logical' )
            {
                var searchBoxCellId = makeId({'ID': 'search_input_filtarbitraryid', suffix: filtArbitraryId, 'prefix': o.idprefix});
                $searchBoxCell.CswTristateCheckBox('init',{'ID': searchBoxCellId, 'Checked': defaultValue}); 
            }
            else
            {
                var searchSuggest;
                if( defaultValue !== '' && defaultValue !== undefined )
                {
                    searchSuggest = defaultValue;
                }
                else
                {
                    searchSuggest = propertyName;
                    if(searchSuggest !== $subfieldsOptions.find(':selected').text() )
                    {
                        searchSuggest += "'s " +  $subfieldsOptions.find(':selected').text();
                    }
                }
                var searchInputId = makeId({ID: 'search_input_filtarbitraryid', suffix: filtArbitraryId, prefix: o.idprefix});
                var $searchInput = $searchBoxCell.CswDOM('input',{
                                                        ID: searchInputId,
                                                        type: 'text',
                                                        cssclass: 'csw_viewbuilder_input',
                                                        placeholder: searchSuggest
                                        })
                                        .attr('autocomplete','on')
                                        .attr('autofocus','true')
                                        .attr({width:"200px"});
                if( fieldtype === 'Date' )
                {
                    $searchInput.datepicker(); //define the dt format later if necessary
                }
            }
        
        } // renderViewPropsAndControls()

        function renderSearchButtons()
        {
            var $clearPosition = o.$parent;
            var clearCellNumber = o.bottomCell;
            var advancedCellNumber = clearCellNumber + 1;
            var cellRow = o.bottomRow;
            if(o.searchtype === 'nodetypesearch')
            {
                //Row i, Column 1: cell for clear/advanced                                            
                var $splitCell = o.$parent.CswTable('cell', o.bottomRow, o.bottomCell)
                                    .empty();
                var splitCellTableId = makeId({prefix: o.idprefix, ID: 'split_cell_table'});
                var $splitCellTable = $splitCell.CswTable('init',{ID: splitCellTableId, 
                                                                    cellpadding: 1,
                                                                    cellspacing: 1,
                                                                    cellalign: 'left',
                                                                    align: 'left'
                                                                    });
                $clearPosition = $splitCellTable;
                clearCellNumber = 1;
                advancedCellNumber = 2;
                cellRow = 1;
            }
            
            //Row i, Column 1 (1/1): clear button
            var $clearButtonCell = $clearPosition.CswTable('cell', cellRow, clearCellNumber)
                                    .empty();
            var clearButtonId = makeId({ID: 'clear_button', prefix: o.idprefix});
            var $clearButton = $clearButtonCell.CswButton({ID: clearButtonId,
                                                            enabledText: 'Clear', 
                                                            disabledText: 'Clear',
                                                            disableOnClick: false, 
                                                            onclick: function() { reInit(); }
                                            });
                                            
            //Row i, Column 1 (1/2): advanced link
            var $advancedLinkCell = $clearPosition.CswTable('cell', cellRow, advancedCellNumber)
                                    .empty();
            var advancedLinkId = makeId({ID: 'advanced_options', prefix: o.idprefix});
            var $advancedLink = $advancedLinkCell.CswDOM('link',{
                                                    ID: advancedLinkId,
                                                    href: '#advanced',
                                                    value: 'Advanced' })
                                                    .click(function() {
                                                            o.isHidden = modAdvanced({'$link': $advancedLink });
                                                    });  
                                               
            //Row i, Column 5: search button
            var $searchButtonCell = o.$parent.CswTable('cell', o.bottomRow, o.searchBtnCell)
                                    .attr({align:"right"})
                                    .empty();
            var searchButtonId = makeId({ID: 'search_button', prefix: o.idprefix});
            var $searchButton = $searchButtonCell.CswButton({ID: searchButtonId, 
                                                            enabledText: 'Search', 
                                                            disabledText: 'Searching', 
                                                            enableAfterClick: true,
                                                            onclick: function() { doSearch(); }
                                                });
        } // renderSearchButtons()

        function reInit()
        {
            $topspandiv.empty();
            var searchTableId = makeId({prefix: o.idprefix, ID: 'search_tbl'});
            o.$searchTable = $topspandiv.CswTable('init', { 
                                            ID: searchTableId, 
                                            cellpadding: 1,
                                            cellspacing: 1,
                                            cellalign: 'center',
                                            align: 'center'
                                            });
            switch(o.searchtype)
            {
                case 'nodetypesearch':
                {
                    renderNodeTypeSearchContent();
                }
                case 'viewsearch':
                {
                    renderViewBasedSearchContent();
                }
            }
        } // reInit()

        function init()
        {
            //var $titlespan = $('<span style="align: center;">Search</span>');
            //o.$parent.append( $titlespan );
            $topspandiv.empty();
            CswAjaxXml({ 
		        'url': o.getClientSearchXmlUrl,
		        'data': "ViewIdNum=" + o.viewid + "&SelectedNodeTypeIdNum=" + o.nodetypeid + "&IdPrefix=" + o.idprefix + "&NodeKey=" + o.cswnbtnodekey,
                'success': function($xml) { 
                    o.searchtype = $xml.attr('searchtype');
                    var searchTableId = makeId({prefix: o.idprefix, ID: 'search_tbl'});
                    switch(o.searchtype)
                    {
                        case 'nodetypesearch':
                        {
                            o.$searchTable = $topspandiv.CswTable('init', { 
                                    ID: searchTableId, 
                                    cellpadding: 1,
                                    cellspacing: 1,
                                    cellalign: 'center',
                                    align: 'center'
                                    });
                            o.$nodeTypesXml = $xml.children('nodetypes');
                            o.$propsXml = $xml.children('nodetypeprops');
                            renderNodeTypeSearchContent();
                            break;
                        }
                        case 'viewsearch':
                        {
                            o.$searchTable = $topspandiv.CswTable('init', { 
                                    ID: searchTableId, 
                                    cellpadding: 1,
                                    cellspacing: 1,
                                    cellalign: 'left',
                                    align: 'center'
                                    });
                            o.$propsXml = $xml.children('properties');
                            o.propsCount = $xml.children('properties').children('property').size();
                            
                            renderViewBasedSearchContent();
                            break;
                        }
                    }
                    //getBottomSpan({'$bottomspandiv': $bottomspandiv, 'initOptions': o, 'idprefix': o.idprefix, '$cswSearchForm': o.$cswSearchForm});		
			    } // success
					
		    }); // CswAjaxXml
        } // init()

//        function getBottomSpan()
//        {                                                                                                                                                                                                                                            {
//            var $bottomTable = o.$bottomspandiv.CswTable('init', {ID: o.idprefix + '_change_viewbuilder_tbl', 
//                                                                cellpadding: 1,
//                                                                cellspacing: 1,
//                                                                cellalign: 'center',
//                                                                align: 'right'});
//            //Row 1, Column 1: load a search
//            var $loadTableCell = $bottomTable.CswTable('cell', 1, 1);
//            $loadTableCell.html('Load a Search:');
//        
//            var $viewSelect;
//                                                                                                
//            CswAjaxXml({ 
//			    'url': o.getSearchableViewsUrl,
//			    'data': "IsMobile=" + false + "&OrderBy=" + "&IdPrefix=" + o.idprefix,
//                'success': function($views) { 
//                        //Row 1, Column 2: view select
//                        var $viewSelectCell = $bottomTable.CswTable('cell', 1, 2);
//                        $viewSelect = $(xmlToString($views));
//                        $viewSelectCell.append($viewSelect);
//                    }
//                });

//            //Row 1, Column 3: load button
//            var $loadButtonCell = $bottomTable.CswTable('cell', 1, 3);
//            var $loadButton = $loadButtonCell.CswButton({ID: 'load_button', 
//                                                prefix: o.idprefix,
//                                                enabledText: 'Load', 
//                                                disabledText: 'Loading', 
//                                                onclick: function() {
//                                                        var r = {
//                                                            'viewid': $viewSelect.find(':selected').val(),
//                                                            'viewmode': $viewSelect.find(':selected').attr('title'),
//                                                        };
//                                                        $.extend(o,r);                                                    
//                                                        o.$cswSearchForm.empty();
//                                                        
//                                                        //$.CswCookie('set', CswCookieName.CurrentView.ViewId, r.viewid);
//                                                        //$.CswCookie('set', CswCookieName.CurrentView.ViewMode, r.viewmode);
//                                                        init();
//                                                        window.location.replace('NewMain.html');
//                                                    }
//                                                });

//            //Row 2, Column 2: new custom search
//            var $customSearchCell = $bottomTable.CswTable('cell', 2, 2);
//            var $customSearch = $customSearchCell.CswDOM('link',{
//                                                            ID: 'new_custom_viewbuilder',
//                                                            prefix: o.idprefix,
//                                                            href: '#customsearch',
//                                                            value: 'New Custom Search' }); 
//        } // getBottomSpan()

        function doSearch()
        {
            var searchOpt;

            var props = [];
            var propno = 1;
            var searchUrl;
            switch(o.searchtype)
            {
                case 'nodetypesearch':
                {
                    searchUrl = o.doNodeSearchUrl;
                    var objectPk = o.$nodeTypesSelect.val();
                    var relatedIdType = o.$nodeTypesSelect.find(':selected').attr('title');

                    $('.csw_viewbuilder_properties_select').each(function() {
                            var $thisProp = $(this);
                            var propName = $thisProp.text();
                            var propId = $thisProp.val();
                        
                            var $subField = o.$parent.CswDOM('findelement',{ID: 'subfield_select_viewbuilderpropid_' + propId, prefix: o.idprefix});
                            var subFieldText = $subField.find(':selected').text();
                        
                            var $filter = o.$parent.CswDOM('findelement',{ID: 'filter_select_viewbuilderpropid_' + propId, prefix: o.idprefix});
                            var filterText = $filter.find(':selected').val();

                            var fieldtype = o.$propsXml.children('propertyfilters').children('property[propname="' + propName + '"][propid="' + propId + '"]').attr('fieldtype');
                        
                            var $searchInput = o.$parent.CswDOM('findelement',{ID: 'search_input_viewbuilderpropid_' + propId, prefix: o.idprefix});
                            var searchText;
                            if( fieldtype === 'Logical' )
                            {
                                searchText = $searchInput.CswTristateCheckBox('value');
                            }
                            else if( fieldtype === 'List' )
                            {
                                $searchList = o.$parent.CswDOM('findelement',{ID: 'filtersoptions_select_viewbuilderpropid_' + propId, prefix: o.idprefix});
                                searchText = $searchList.find(':selected').val();
                            }
                            else
                            {
                                searchText = $searchInput.val();
                            }
                            var thisNodeProp = {
                                    objectpk: objectPk,
                                    relatedidtype: relatedIdType,
                                    propid: propId,
                                    subfield: subFieldText,
                                    filter: filterText,
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
                    searchUrl = o.doViewSearchUrl;
                    o.$propsXml.children('property').each(function() {
                            var PropFilter = $(this).CswViewPropFilter('getFilterJson');
                            props.push(PropFilter);
                        });
                    searchOpt = { 
                            viewprops: props,
                            viewid: o.viewid
                    };
                    break;
                }
            }

            if(searchOpt)
            {
                CswAjaxJSON({ 
			    'url': searchUrl,
			    'data': "{SearchJson: \"" + jsonToString(searchOpt) + "\"}",
                'success': function(view) { 
                        o.viewid = view.sessionviewid;
                        o.searchtype = 'viewsearch'; //the next search will be always be based on the view returned
                        init(); //our arbitraryid's have probably changed. need to pull fresh XML.
                        o.onSearchSubmit({viewid: view.sessionviewid, viewmode: view.viewmode});
                    }
                });
            }
        } // doSearch()

    return $cswSearchForm;

	 // function(options) {
    };
})(jQuery);


