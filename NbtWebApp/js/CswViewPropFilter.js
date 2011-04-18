; (function ($) {
	
    var PluginName = "CswViewPropFilter";

    var methods = {

        'init': function(options) 
		{
            var o = { 
                //URLs
                'getNewPropsUrl': '/NbtWebApp/wsNBT.asmx/getViewPropFilter',

                //options
			    'viewid': '',
                'viewxml': '',
                'proparbitraryid': '',
                'idprefix': 'csw',

                'selectedSubfieldVal': '',
                'selectedFilterVal': ''
		    };
		
            if(options) $.extend(o, options);
        
            var $parent = $(this);
            var $cswPropFilterRow = $parent.CswDOM('span',{ID: 'cswPropFilterRow', prefix: o.idprefix});
            var $propsXml;
            var $propFilterTable;

            init();

            function init()
                                                                                {
            CswAjaxXml({ 
		        'url': o.getNewPropsUrl,
		        'data': "ViewXml=" + o.viewxml + "&PropArbitraryId=" + o.proparbitraryid,
                'success': function($xml) { 

                            $propFilterTable = $cswPropFilterRow.CswTable('init', { 
                                        ID: makeId({prefix: o.idprefix, ID: 'search_tbl'}), 
                                        cellpadding: 1,
                                        cellspacing: 1,
                                        cellalign: 'center',
                                        align: 'center'
                                    });
                            
                            $propsXml = $xml.children('nodetypeprops').children('property');
                            renderPropFiltRow();
                } //success
            }); //ajax
        }

            function renderPropFiltRow()
                                                                                                                                                                                                                                                                                                                                                                        {
            var propertyId = $propsXml.attr('propid');
            var propertyName = $propsXml.attr('propname');
            var filtArbitraryId = $propsXml.attr('filtarbitraryid');
                
            //Row propRow, Column 3: property
            var $propSelectCell = $propFilterTable.CswTable('cell', propRow, 3)
                                                    .empty();
            var propCellId = makeId({ID: propertyId,prefix: o.idprefix});
            var $props = $propSelectCell.CswDOM('span',{ID: propCellId, value: propertyName});
        
            var $defaultFilter = $propsXml.children('defaultsubfield').attr('filter');
            var fieldtype = $propsXml.attr('fieldtype');

            //Row propRow, Column 4: subfield picklist 
            var $subfieldsOptions = $(xmlToString($propsXml.children('subfields').children('select')))
                                    .change(function() {
                                        var $this = $(this);
                                        var r = {
                                            'selectedSubfieldVal': $this.val(),
                                            'selectedFilterVal': ''
                                        };
                                        $.extend(o,r);
                                        renderPropFiltRow() });

            if(o.selectedSubfieldVal !== '')
            {
                $subfieldsOptions.val(o.selectedSubfieldVal).attr('selected',true);
            }
            $subfieldCell.append($subfieldsOptions);
            var $subfield = $subfieldsOptions.find(':selected').val();
            var defaultValue = $subfieldsOptions.find(':selected').attr('defaultvalue');

            //Row propRow, Column 5: filter picklist
            var $filtersCell = $propFilterTable.CswTable('cell', propRow, 5)
                                .empty();

            var $filtersOptions =  $(xmlToString($propsXml.children('propertyfilters').children('subfield[column=' + $subfield + ']').children('select')))
                                    .change(function() {
                                        var $this = $(this);
                                        var r = {
                                            'selectedSubfieldVal': $subfieldsOptions.val(),
                                            'selectedFilterVal': $this.val()
                                        };
                                        $.extend(o,r);
                                        renderPropFiltRow() });

            if(o.selectedFilterVal !== '')
            {
                $filtersOptions.val(o.selectedFilterVal).attr('selected',true);
            }
            $filtersCell.append($filtersOptions);
            var $filter = $filtersOptions.find(':selected').val();

            //Row propRow, Column 6: search input
            var $propFilterValueCell = $propFilterTable.CswTable('cell', propRow, 6)
                            .empty();
            
            if( fieldtype === 'List' )
            {
                $propFilterValueCell.append( $(xmlToString($propsXml.children('filtersoptions').children('select'))) );
            }
            else if( fieldtype === 'Logical' )
            {
                $propFilterValueCell.CswTristateCheckBox('init',{'ID': 'search_input_searchpropid_' + propertyId, 'prefix': o.idprefix}); 
            }
            else
            {
                var filtValueSuggest;
                if( defaultValue !== '' && defaultValue != undefined )
                {
                    filtValueSuggest = defaultValue;
                }
                else
                {
                    filtValueSuggest = $props.find(':selected').text();
                    if(filtValueSuggest !== $subfieldsOptions.find(':selected').text() )
                    {
                        filtValueSuggest += "'s " +  $subfieldsOptions.find(':selected').text();
                    }  
                }
                var filtValInputId = makeId({ID: 'search_input_searchpropid', prefix: o.idprefix, suffix:propertyId});
                var $filtValInput = $propFilterValueCell.CswDOM('input',{
                                                        ID: filtValInputId,
                                                        type: 'text',
                                                        cssclass: 'csw_search_input',
                                                        placeholder: filtValueSuggest })
                                                .attr('autocomplete','on')
                                                .attr('autofocus','true')
                                                .attr({width:"200px"});
            }
                            
			   
        } // renderPropFiltRow()

            return $cswPropFilterRow;
        }, // 'init': function(options) {
        'getFilterJson': function(options)
        {
            var o = {
                objectpk: '',
                relatedidtype: '',
                fieldtype: $thisProp.attr('fieldtype'),
                propId: $thisProp.attr('propid')
            };
            if(options) $.extend(o,options);

            var $thisProp = $(this);
            var thisNodeProp = {}; //to return
            var propName = $thisProp.val();
            var filtArbitraryId = $thisProp.attr('filtarbitraryid');
            var propArbitraryId = $thisProp.attr('proparbitraryid');
            
            var searchInputId = makeId({ID: 'search_input_filtarbitraryid', suffix: filtArbitraryId, prefix: o.idprefix});
            var $searchInput = o.$parent.CswDOM('findelement',{ID: searchInputId});
            var searchText;
            switch( o.fieldtype )
            { 
                case 'Logical': 
                {
                    searchText = $searchInput.CswTristateCheckBox('value');
                    break;
                }
                case 'List':
                {
                    var searchListId = makeId({ID: 'filtersoptions_select_filtarbitraryid', suffix: filtArbitraryId, prefix: o.idprefix});
                    var $searchList = o.$parent.CswDOM('findelement',{ID: searchListId});
                    searchText = $searchList.find(':selected').val();
                    break;
                }
                default:
                {
                    searchText = $searchInput.val();
                    break;
                }
            }
            if(searchText !== '')
            {
                var $subField = o.$parent.CswDOM('findelement',{ID: 'subfield_select_filtarbitraryid_' + filtArbitraryId, prefix: o.idprefix});
                var subFieldText = $subField.find(':selected').text();

                var $filter = o.$parent.CswDOM('findelement',{ID: 'filter_select_filtarbitraryid_' + filtArbitraryId, prefix: o.idprefix});
                var filterText = $filter.find(':selected').val();

                var relatedidtype = $thisProp.attr('relatedidtype');
                var propType = $thisProp.attr('proptype');
                                
                thisNodeProp = {
                    objectpk: o.objectpk, // for NodeType filters
                    relatedidtype: o.relatedidtype, // for NodeType filters
                    proptype: propType,
                    propid: o.propId,
                    filtarbitraryid: filtArbitraryId,
                    proparbitraryid: propArbitraryId,
                    relatedidtype: relatedidtype,
                    subfield: subFieldText,
                    filter: filterText,
                    searchtext: searchText  
                };
                
            }
            return thisNodeProp;
        }, // 'getFilterJson': function(options) { 
        'makefilter': function(options)
        {
            var o = {
                url: '/NbtWebApp/wsNBT.asmx/makeViewPropFilter',
                viewxml: '',
                filtJson: '',
                onSuccess: function() {}
            };
            if(options) $.extend(o,options);

            var $filterXml;

            CswAjaxXml({ 
			'url': o.url,
			'data': "ViewXml="  + o.viewxml + "&PropFiltJson=" + jsonToString(o.filtJson),
            'success': function($filter) { 
                    $filterXml = $filter;
                    o.onSuccess();
                }
            });

            return $filterXml;
        } // 'makefilter': function(options)
    } // methods 
	 
    $.fn.CswViewPropFilter = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};
})(jQuery);


