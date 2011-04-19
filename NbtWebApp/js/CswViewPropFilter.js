; (function ($) {
	
    var PluginName = "CswViewPropFilter";

    var methods = {

        'init': function(options) 
		{
            var o = { 
                //URLs
                'getNewPropsUrl': '/NbtWebApp/wsNBT.asmx/getViewPropFilterUI',

                //options
			    'viewid': '',
                'viewxml': '',
                '$propsXml': '',
                'proparbitraryid': '',
                'idprefix': 'csw',
                'propRow': 1,
                'firstColumn': 3,
                'includePropertyName': false,

                'selectedSubfieldVal': '',
                'selectedFilterVal': '',

                'propIdName': 'filtarbitraryid',
                'propIdSuffix': ''
		    };
		
            if(options) $.extend(o, options);
        
            var $propFilterTable = $(this); //must call on a table
            
            if( o.$propsXml === '' || o.$propsXml === undefined )
            {
                CswAjaxXml({ 
		            'url': o.getNewPropsUrl,
		            'data': "ViewXml=" + o.viewxml + "&PropArbitraryId=" + o.proparbitraryid,
                    'success': function($xml) { 

                                o.$propsXml = $xml.children('nodetypeprops').children('property');
                                renderPropFiltRow();
                    } //success
                }); //ajax
            }
            else
            {
                renderPropFiltRow();
            }

            function renderPropFiltRow()
            {
                var propertyId = o.$propsXml.attr('propid');
                var propertyName = o.$propsXml.attr('propname');
                var filtArbitraryId = o.$propsXml.attr('filtarbitraryid');
                
                if( o.includePropertyName )
                {
                    //Row propRow, Column 3: property
                    var $propSelectCell = $propFilterTable.CswTable('cell', o.propRow, o.firstColumn) //3
                                                          .empty();
                    var propCellId = makeId({ID: propertyId,prefix: o.idprefix});
                    var $props = $propSelectCell.CswDOM('span',{ID: propCellId, value: propertyName});
                }
                
                var fieldtype = o.$propsXml.attr('fieldtype');
                var $defaultFilter = o.$propsXml.children('defaultsubfield').attr('filter');
                var $subfieldCell = $propFilterTable.CswTable('cell', o.propRow, 4)                                                    .empty();                var defaultSubFieldId = makeId({ID: 'default_filter_' + o.propIdName, suffix: o.propIdSuffix, prefix: o.idprefix});                var $defaultSubField = $subfieldCell.CswDOM('span', {                                                    ID: defaultSubFieldId,                                                    value: $defaultFilter,                                                    cssclass: 'csw_viewbuilder_default_filter' })                                                .attr({align:"center"});
                $defaultSubField.hide(); //for Search
                //Row propRow, Column 4: subfield picklist 
                var $subfieldCell = $propFilterTable.CswTable('cell', o.propRow, (o.firstColumn + 1)) //4
                var $subfieldsOptions = $(xmlToString(o.$propsXml.children('subfields').children('select')))
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
                var $filtersCell = $propFilterTable.CswTable('cell', o.propRow, (o.firstColumn + 2)) //5
                                                   .empty();

                var $filtersOptions =  $(xmlToString(o.$propsXml.children('propertyfilters').children('subfield[column=' + $subfield + ']').children('select')))
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

                //Row propRow, Column 6: filter input
                var $propFilterValueCell = $propFilterTable.CswTable('cell', o.propRow, (o.firstColumn + 3)) //6
                                                           .empty();
                
                var filtValInputId = makeId({'ID': 'search_input_' + o.propIdName, suffix: o.propIdSuffix, 'prefix': o.idprefix});
                if( fieldtype === 'List' )
                {
                    $propFilterValueCell.append( $(xmlToString($propsXml.children('filtersoptions').children('select'))) );
                }
                else if( fieldtype === 'Logical' )
                {
                    $propFilterValueCell.CswTristateCheckBox('init',{'ID': filtValInputId, 'Checked': defaultValue}); 
                }
                else
                {
                    var filtValue;
                    if( defaultValue !== '' && defaultValue != undefined )
                    {
                        filtValue = defaultValue;
                    }
                    else
                    {
                        filtValue = propertyName;
                        if(filtValue !== $subfieldsOptions.find(':selected').text() )
                        {
                            filtValue += "'s " +  $subfieldsOptions.find(':selected').text();
                        }  
                    }
                    var $filtValInput = $propFilterValueCell.CswDOM('input',{
                                                            ID: filtValInputId,
                                                            type: 'text',
                                                            cssclass: 'csw_search_input',
                                                            text: filtValue })
                                                    .attr('autocomplete','on')
                                                    .attr('autofocus','true')
                                                    .attr({width:"200px"});
                }
            }
            return $propFilterTable;
        }, // 'init': function(options) {
        'getFilterJson': function(options)
        {
            var $thisProp = $(this);
            var filtArbitraryId = $thisProp.attr('filtarbitraryid');
            var o = {
                objectpk: '',
                relatedidtype: '',
                fieldtype: $thisProp.attr('fieldtype'),
                propId: $thisProp.attr('propid'),
                idprefix: '',
                $parent: '',
                propIdName: 'filtarbitraryid',
                propIdSuffix: filtArbitraryId
            };
            if(options) $.extend(o,options);

            var searchInputId = makeId({ID: 'search_input_' + o.propIdName, suffix: o.propIdSuffix, prefix: o.idprefix});
            var searchListId = makeId({ID: 'filtersoptions_select_' + o.propIdName, suffix: o.propIdSuffix, prefix: o.idprefix});
            var subFieldId = makeId({ID: 'subfield_select_' + o.propIdName, suffix: o.propIdSuffix, prefix: o.idprefix});
            var filterId = makeId({ID: 'filter_select_' + o.propIdName, suffix: o.propIdSuffix, prefix: o.idprefix})
            var propArbitraryId = $thisProp.attr('proparbitraryid');

            var thisNodeProp = {}; //to return
            
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
                
                var $subField = o.$parent.CswDOM('findelement',{ID: subFieldId});
                var subFieldText = $subField.find(':selected').text();

                var $filter = o.$parent.CswDOM('findelement',{ID: filterId});
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
        'makeFilter': function(options)
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


