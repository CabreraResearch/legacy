; (function ($) {
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
        o.$searchTable = $parent.CswDOM('div',{ID: 'CswSearchForm', prefix: o.idprefix});
        
        var $topspan = o.$searchTable.CswDOM('span');

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
                
                $('.' + ViewBuilder_CssClasses.subfield_select.name).each(function() { $(this).show(); });
                $('.' + ViewBuilder_CssClasses.filter_select.name).each(function() { $(this).show(); });
                $('.' + ViewBuilder_CssClasses.default_filter.name).each(function() { $(this).hide(); });
                o.$link.text('Simple');
                isHidden = false;
            }
            else
            {
                $('.' + ViewBuilder_CssClasses.subfield_select.name).each(function() { $(this).hide(); });
                $('.' + ViewBuilder_CssClasses.filter_select.name).each(function() { $(this).hide(); });
                $('.' + ViewBuilder_CssClasses.default_filter.name).each(function() { $(this).show(); });
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
                    var nodeTypeId = makeId({ID: 'viewbuilderpropid', suffix: $thisProp.attr('viewbuilderpropid'), prefix: o.idprefix});
                    var $nodeType = $nodeTypeCell.CswDOM('span',{
                                                                ID: nodeTypeId,
                                                                value: $thisProp.attr('metadatatypename'),
                                                                cssclass: ViewBuilder_CssClasses.metadatatype_static.name})
                                                  .attr('relatedidtype',$thisProp.attr('relatedidtype') );
                    o.selectedSubfieldVal = ''; 
                    o.selectedFilterVal = '';
                    o.isHidden = true;               
                    var filtArbitraryId = $thisProp.attr('filtarbitraryid');
                    var $propFilterRow = o.$searchTable.CswViewPropFilter('init', {
                                                    'idprefix': 'csw',
                                                    'propRow': propRow,
                                                    'firstColumn': 3,
                                                    'includePropertyName': true,
                                                    '$propsXml': $thisProp,
                                                    'filtarbitraryid': filtArbitraryId
                                                });
                    propRow++;
            });
            
            o.bottomRow = propRow;
            o.bottomCell = 1;
            o.searchtype = 'viewsearch';
            
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
        
            //prop row(s) 1-?, Columns 3-6
            var propRow = 1;
            //Row propRow, Column 3: properties 
            var $propSelectCell = o.$searchTable.CswTable('cell', propRow, 3)
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
                                    o.$searchTable.CswViewPropFilter('init', {
                                                'idprefix': 'csw',
                                                'propRow': propRow,
                                                'firstColumn': 3,
                                                'includePropertyName': false,
                                                '$propsXml': $selectedProp,
                                                viewbuilderpropid: propertyId
                                            }); 
                                   });
                                
            if(o.selectedPropVal !== '' )
            {
                $props.val(o.selectedPropVal).attr('selected',true);
            }
            $propSelectCell.append($props);
            var propertyId = $props.find(':selected').val();
            var $selectedProp = o.$propsXml.children('propertyfilters').children('property[viewbuilderpropid='+ propertyId +']');
        
            var $propFilter = o.$searchTable.CswViewPropFilter('init', {
                                                'idprefix': 'csw',
                                                'propRow': propRow,
                                                'firstColumn': 3,
                                                'includePropertyName': false,
                                                '$propsXml': $selectedProp,
                                                'viewbuilderpropid': propertyId
                                            });
            
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
                                o.$searchTable.CswViewPropFilter('init', {
                                                'idprefix': 'csw',
                                                'propRow': propRow,
                                                'firstColumn': 3,
                                                'includePropertyName': false,
                                                '$propsXml': $selectedProp,
                                                'viewbuilderpropid': propertyId
                                            });
                        }
                    });
        } // getNewProps()

        function renderSearchButtons()
        {
            var $clearPosition = o.$searchTable;
            var clearCellNumber = o.bottomCell;
            var advancedCellNumber = clearCellNumber + 1;
            var cellRow = o.bottomRow;
            if(o.searchtype === 'nodetypesearch')
            {
                //Row i, Column 1: cell for clear/advanced                                            
                var $splitCell = o.$searchTable.CswTable('cell', o.bottomRow, o.bottomCell)
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
                                                    value: 'init' })
                                                    .click(function() {
                                                            o.isHidden = modAdvanced({'$link': $advancedLink });
                                                    });  
            modAdvanced({$link: $advancedLink});                                   
            //Row i, Column 5: search button
            var $searchButtonCell = o.$searchTable.CswTable('cell', o.bottomRow, o.searchBtnCell)
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
            
            CswAjaxXml({ 
		        'url': o.getClientSearchXmlUrl,
		        'data': "ViewIdNum=" + o.viewid + "&SelectedNodeTypeIdNum=" + o.nodetypeid + "&IdPrefix=" + o.idprefix + "&NodeKey=" + o.cswnbtnodekey,
                'success': function($xml) { 
                    $topspandiv.empty();
                    o.searchtype = $xml.attr('searchtype');
                    var searchTableId = makeId({prefix: o.idprefix, ID: 'search_tbl'});
                    o.$searchTable = $topspandiv.CswTable('init', { 
                                    ID: searchTableId, 
                                    cellpadding: 1,
                                    cellspacing: 1,
                                    cellalign: 'left',
                                    align: 'center'
                                    });
					o.$searchTable.css("background-color", "0099FF");
					o.$searchTable.attr('frame', 'border');
                    switch(o.searchtype)
                    {
                        case 'nodetypesearch':
                        {
                            o.$nodeTypesXml = $xml.children('nodetypes');
                            o.$propsXml = $xml.children('viewbuilderprops');
                            renderNodeTypeSearchContent();
                            break;
                        }
                        case 'viewsearch':
                        {
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
                            var viewbuildpropid = $thisProp.val();
                            var fieldtype = o.$propsXml.children('propertyfilters')
                                                       .children('property[propname="' + propName + '"][viewbuilderpropid="' + viewbuildpropid + '"]')
                                                       .attr('fieldtype');
                            var thisNodeProp = $thisProp.CswViewPropFilter('getFilterJson',{objectpk: objectPk,
                                                                                          relatedidtype: relatedIdType,  
                                                                                          fieldtype: fieldtype,
                                                                                          idprefix: o.idprefix,
                                                                                          $parent: o.$searchTable,
                                                                                          'viewbuilderpropid': viewbuildpropid
                                                                        }); 
                            props.push( thisNodeProp );
                        });
                    searchOpt = {
                        'viewbuilderprops' : props
                    };
                    
                    break;
                }
                case 'viewsearch':
                {
                    searchUrl = o.doViewSearchUrl;
                    o.$propsXml.children('property').each(function() {
                            var PropFilter = $(this).CswViewPropFilter('getFilterJson',{idprefix: o.idprefix, $parent: o.$searchTable});
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

    return o.$searchTable;

	 // function(options) {
    };
})(jQuery);


//function getBottomSpan()
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