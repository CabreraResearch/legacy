// for CswViewPropFilter
var ViewBuilder_CssClasses = {
    subfield_select: { name: 'csw_viewbuilder_subfield_select' },
    filter_select: { name: 'csw_viewbuilder_filter_select' },
    default_filter: { name: 'csw_viewbuilder_default_filter' },
    filter_value: { name: 'csw_viewbuilder_filter_value' },
    metadatatype_static: { name: 'csw_viewbuilder_metadatatype_static' }
};

;  (function ($) {
	
    var PluginName = "CswViewPropFilter";

    function makePropFilterId(ID, options)
    {
        var FilterId = '';
        var Delimiter = '_';
        var o = {
            'proparbitraryid': '',
            'filtarbitraryid': '',
            'viewbuilderpropid': '',
            'idprefix': ''
        };
        if(options) $.extend(o,options);
        
        if( o.filtarbitraryid !== '' && o.filtarbitraryid !== undefined )
        {
            FilterId = makeId({ 'ID': ID + Delimiter + 'filtarbitraryid', 
                                'prefix': o.idprefix, 
                                'suffix': o.filtarbitraryid });
        }
        else if( o.viewbuilderpropid !== '' && o.viewbuilderpropid !== undefined )
        {
            FilterId = makeId({ 'ID': ID + Delimiter + 'viewbuilderpropid', 
                                'prefix': o.idprefix, 
                                'suffix': o.viewbuilderpropid });
        }
        else if( o.proparbitraryid !== '' && o.proparbitraryid !== undefined )
        {
            FilterId = makeId({ 'ID': ID + Delimiter + 'proparbitraryid', 
                                'prefix': o.idprefix, 
                                'suffix': o.proparbitraryid });
        }
        else if( o.idprefix !== '' && o.idprefix !== undefined )
        {
            FilterId = makeId({ 'ID': ID, 
                                'prefix': o.idprefix });
        }
        else
        {
            FilterId = ID;
        }
        return FilterId;
    }

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
                'filtarbitraryid': '',
                'viewbuilderpropid': '',
                'idprefix': 'csw',
                'propRow': 1,
                'firstColumn': 3,
                'includePropertyName': false,

                'selectedSubfieldVal': '',
                'selectedFilterVal': '',

                'autoFocusInput': false
		    };
		
            if(options) $.extend(o, options);
        
            var $propFilterTable = $(this); //must call on a table
            
            var filtOpt = {
                'proparbitraryid': o.proparbitraryid,
                'filtarbitraryid': o.filtarbitraryid,
                'viewbuilderpropid': o.viewbuilderpropid,
                'idprefix': o.idprefix
            };
                       
            if( ( o.$propsXml === '' || o.$propsXml === undefined ) &&
                o.proparbitraryid !== '' && o.proparbitraryid !== undefined )
            {
                CswAjaxXml({ 
		            'url': o.getNewPropsUrl,
		            'data': "ViewXml=" + o.viewxml + "&PropArbitraryId=" + o.proparbitraryid,
                    'success': function($xml) { 
                                o.$propsXml = $xml.children('propertyfilters').children('property');
                                filtOpt.filtarbitraryid = o.$propsXml.attr('filtarbitraryid');
                                renderPropFiltRow(filtOpt);
                    } //success
                }); //ajax
            }
            else
            {
                renderPropFiltRow(filtOpt);
            }

            function renderPropFiltRow(filtOpt)
            {
                var propertyId = o.$propsXml.attr('viewbuilderpropid');
                var propertyName = o.$propsXml.attr('propname');
                
                if( o.includePropertyName )
                {
                    //Row propRow, Column 3: property
                    var $propSelectCell = $propFilterTable.CswTable('cell', o.propRow, o.firstColumn) //3
                                                          .empty();
                    var propCellId = makePropFilterId(propertyName,filtOpt);
                    var $props = $propSelectCell.CswDOM('span',{ID: propCellId, value: propertyName});
                }
                
                var fieldtype = o.$propsXml.attr('fieldtype');
                var $defaultFilter = o.$propsXml.children('defaultsubfield').attr('filter');
                var $subfieldCell = $propFilterTable.CswTable('cell', o.propRow, 4)
                                                    .empty();

                var defaultSubFieldId = makePropFilterId('default_filter', filtOpt);
                var $defaultSubField = $subfieldCell.CswDOM('span', {
                                                    ID: defaultSubFieldId,
                                                    value: $defaultFilter,
                                                    cssclass: ViewBuilder_CssClasses.default_filter.name })
                                                .attr({align:"center"});

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
                
                var filtValInputId = makePropFilterId('propfilter_input', filtOpt);
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
                    var inputOpt = {
                        value: defaultValue,
                        placeholder: ''
                    };
                    if( inputOpt.value === '' || inputOpt.value === undefined )
                    {
                        o.placeholder = propertyName;
                        if(o.placeholder !== $subfieldsOptions.find(':selected').text() )
                        {
                            o.placeholder += "'s " +  $subfieldsOptions.find(':selected').text();
                        }  
                    }
                    var $filtValInput = $propFilterValueCell.CswInput('init', {ID: filtValInputId,
                                                                                type: inputTypes.text,
                                                                                cssclass: ViewBuilder_CssClasses.filter_value.name,
                                                                                value: inputOpt.value,
                                                                                placeholder: inputOpt.placeholder,
                                                                                width: "200px",
                                                                                autofocus: o.autoFocusInput,
                                                                                autocomplete: 'on'
                                                                       });
                }
            }
            return $propFilterTable;
        }, // 'init': function(options) {
        'getFilterJson': function(options)
        {
            var $thisProp = $(this);
            var o = {
                objectpk: '',
                relatedidtype: '',
                fieldtype: $thisProp.attr('fieldtype'),
                idprefix: '',
                $parent: '',
                proparbitraryid: $thisProp.attr('proparbitraryid'),
                filtarbitraryid: $thisProp.attr('filtarbitraryid'),
                viewbuilderpropid: $thisProp.attr('viewbuilderpropid')
            };
            if(options) $.extend(o,options);

            var filtOpt = {
                'proparbitraryid': o.proparbitraryid,
                'filtarbitraryid': o.filtarbitraryid,
                'viewbuilderpropid': o.viewbuilderpropid,
                'idprefix': o.idprefix
            };

            var filtValInputId = makePropFilterId('propfilter_input', filtOpt);
            var filtValListId = makePropFilterId('filtersoptions_select',filtOpt);
            var subFieldId = makePropFilterId('subfield_select',filtOpt);
            var filterId = makePropFilterId('filter_select',filtOpt);

            var thisNodeProp = {}; //to return
            
            var $filtInput = o.$parent.CswInput('get',{ID: filtValInputId});
            var filterValue;
            switch( o.fieldtype )
            { 
                case 'Logical': 
                {
                    filterValue = $filtInput.CswTristateCheckBox('value');
                    break;
                }
                case 'List':
                {
                    var $filtList = o.$parent.CswDOM('findelement',{ID: filtValListId});
                    filterValue = $filtList.find(':selected').val();
                    break;
                }
                default:
                {
                    filterValue = $filtInput.val();
                    break;
                }
            }
            if(filterValue !== '')
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
                    viewbuilderpropid: o.viewbuilderpropid,
                    filtarbitraryid: o.filtarbitraryid,
                    proparbitraryid: o.proparbitraryid,
                    relatedidtype: relatedidtype,
                    subfield: subFieldText,
                    filter: filterText,
                    filtervalue: filterValue  
                };
                
            } // if(filterValue !== '')
            return thisNodeProp;
        }, // 'getFilterJson': function(options) { 
        'makeFilter': function(options)
        {
            var o = {
                url: '/NbtWebApp/wsNBT.asmx/makeViewPropFilter',
                viewxml: '',
                filtJson: '',
                onSuccess: function($filterXml) {}
            };
            if(options) $.extend(o,options);

            //var $filterXml;

            CswAjaxXml({ 
			'url': o.url,
			'data': "ViewXml="  + o.viewxml + "&PropFiltJson=" + jsonToString(o.filtJson),
            'success': function($filter) { 
                    //$filterXml = $filter;
                    o.onSuccess($filter);
                }
            });

            //return $filterXml;
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


