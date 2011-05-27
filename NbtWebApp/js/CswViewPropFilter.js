﻿/// <reference path="../jquery/jquery-1.6.1-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="_Global.js" />

// for CswViewPropFilter
var ViewBuilder_CssClasses = {
    subfield_select: { name: 'csw_viewbuilder_subfield_select' },
    filter_select: { name: 'csw_viewbuilder_filter_select' },
    default_filter: { name: 'csw_viewbuilder_default_filter' },
    filter_value: { name: 'csw_viewbuilder_filter_value' },
    metadatatype_static: { name: 'csw_viewbuilder_metadatatype_static' }
};

; (function ($) { /// <param name="$" type="jQuery" />
	
    var PluginName = "CswViewPropFilter";

    function makePropFilterId(ID, options)
    {
        var FilterId = '';
        var Delimiter = '_';
        var o = {
            'proparbitraryid': '',
            'filtarbitraryid': '',
            'viewbuilderpropid': '',
            'ID': ''
        };
        if(options) $.extend(o,options);
        
        if( !isNullOrEmpty( o.filtarbitraryid ) )
        {
            FilterId = makeId({ 'ID': ID + Delimiter + 'filtarbitraryid', 
                                'prefix': o.ID, 
                                'suffix': o.filtarbitraryid });
        }
        else if( !isNullOrEmpty( o.viewbuilderpropid ) )
        {
            FilterId = makeId({ 'ID': ID + Delimiter + 'viewbuilderpropid', 
                                'prefix': o.ID, 
                                'suffix': o.viewbuilderpropid });
        }
        else if(!isNullOrEmpty(  o.proparbitraryid ) )
        {
            FilterId = makeId({ 'ID': ID + Delimiter + 'proparbitraryid', 
                                'prefix': o.ID, 
                                'suffix': o.proparbitraryid });
        }
        else if( !isNullOrEmpty( o.ID ) )
        {
            FilterId = makeId({ 'ID': ID, 
                                'prefix': o.ID });
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
                'ID': '',
                'propRow': 1,
                'firstColumn': 3,
                'includePropertyName': false,
                'advancedIsHidden': false,

                'selectedSubfieldVal': '',
                'selectedFilterVal': '',

                'autoFocusInput': false
		    };
		
            if(options) $.extend(o, options);
        
            var $propFilterTable = $(this); //must call on a table
            
            if ( isNullOrEmpty( o.$propsXml ) && !isNullOrEmpty( o.proparbitraryid ) )
            {
                var dataXml = {
                    ViewXml: o.viewxml,
                    PropArbitraryId: o.proparbitraryid
                };

                CswAjaxXml({ 
		            'url': o.getNewPropsUrl,
		            'data': dataXml,
                    stringify: true,
                    'success': function($xml) { 
                                o.$propsXml = $xml.children('propertyfilters').children('property');
                                o.filtarbitraryid = o.$propsXml.CswAttrXml('filtarbitraryid');
                                renderPropFiltRow(o);
                    } //success
                }); //ajax
            }
            else
            {
                renderPropFiltRow(o);
            }

            function renderPropFiltRow(filtOpt)
            {
                var propertyId = filtOpt.$propsXml.CswAttrXml('viewbuilderpropid');
                var propertyName = filtOpt.$propsXml.CswAttrXml('propname');
                
                if( filtOpt.includePropertyName )
                {
                    //Row propRow, Column 3: property
                    var $propSelectCell = $propFilterTable.CswTable('cell', filtOpt.propRow, filtOpt.firstColumn) //3
                                                          .empty();
                    var propCellId = makePropFilterId(propertyName,filtOpt);
                    var $props = $propSelectCell.CswSpan('init',{ID: propCellId, value: propertyName});
                }
                
                var fieldtype = filtOpt.$propsXml.CswAttrXml('fieldtype');
                var $defaultFilter = filtOpt.$propsXml.children('defaultsubfield').CswAttrXml('filter');
                
                //Row propRow, Column 4: subfield default value (hidden) 
                var $subfieldCell = $propFilterTable.CswTable('cell', filtOpt.propRow, (filtOpt.firstColumn + 1)) //4
                                                    .empty();
                var defaultSubFieldId = makePropFilterId('default_filter', filtOpt);
                var $defaultSubField = $subfieldCell.CswSpan('init', {
                                                    ID: defaultSubFieldId,
                                                    value: $defaultFilter,
                                                    cssclass: ViewBuilder_CssClasses.default_filter.name })
                                                .CswAttrDom({align:"center"});
                if( !filtOpt.advancedIsHidden )
                {
                    $defaultSubField.hide();
                }

                //Row propRow, Column 4: subfield picklist 
                var subfieldOptionsId = makePropFilterId('subfield_select', filtOpt);
                var $subfieldsOptions = $(xmlToString(filtOpt.$propsXml.children('subfields').children('select')))
                                        .CswAttrDom('id', subfieldOptionsId)
                                        .CswAttrDom('name', subfieldOptionsId)
                                        .addClass(ViewBuilder_CssClasses.subfield_select.name)
                                        .change(function() {
                                            var $this = $(this);
                                            var r = {
                                                'selectedSubfieldVal': $this.val(),
                                                'selectedFilterVal': '',
                                                'advancedIsHidden': isTrue( $this.is(':hidden') )
                                            };
                                            $.extend(filtOpt,r);
                                            renderPropFiltRow(filtOpt) });

                if( !isNullOrEmpty( filtOpt.selectedSubfieldVal ) )
                {
                    $subfieldsOptions.val(filtOpt.selectedSubfieldVal).CswAttrDom('selected',true);
                }
                $subfieldCell.append($subfieldsOptions);
                if( filtOpt.advancedIsHidden )
                {
                    $subfieldsOptions.hide();
                }
                var subfield = $subfieldsOptions.find(':selected').val();
                var defaultValue = $subfieldsOptions.find(':selected').CswAttrDom('defaultvalue');

                //Row propRow, Column 5: filter picklist
                var $filtersCell = $propFilterTable.CswTable('cell', filtOpt.propRow, (filtOpt.firstColumn + 2)) //5
                                                   .empty();
                var filtersOptionsId = makePropFilterId('filter_select', filtOpt);
                var $filtersOptions =  $(xmlToString(filtOpt.$propsXml.children('propertyfilters').children('subfield[column=' + subfield + ']').children('select')))
                                        .CswAttrDom('id', filtersOptionsId)
                                        .CswAttrDom('name', filtersOptionsId)
                                        .addClass(ViewBuilder_CssClasses.filter_select.name)
                                        .change(function() {
                                            var $this = $(this);
                                            var r = {
                                                'selectedSubfieldVal': $subfieldsOptions.val(),
                                                'selectedFilterVal': $this.val(),
                                                'advancedIsHidden': isTrue( $this.is(':hidden') )
                                            };
                                            $.extend(filtOpt,r);
                                            renderPropFiltRow(filtOpt) });

                if( !isNullOrEmpty( filtOpt.selectedFilterVal ) )
                {
                    $filtersOptions.val(filtOpt.selectedFilterVal).CswAttrDom('selected',true);
                }
                $filtersCell.append($filtersOptions);
                if( filtOpt.advancedIsHidden )
                {
                    $filtersOptions.hide();
                }
                //Row propRow, Column 6: filter input
                var $propFilterValueCell = $propFilterTable.CswTable('cell', filtOpt.propRow, (filtOpt.firstColumn + 3)) //6
                                                           .empty();
                
                var filtValInputId = makePropFilterId('propfilter_input', filtOpt);
                if( fieldtype === 'List' )
                {
                    $propFilterValueCell.append( $(xmlToString(filtOpt.$propsXml.children('filtersoptions').children('select'))) )
                                        .CswAttrDom('id',filtValInputId)
                                        .CswAttrDom('name',filtValInputId);
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
                    if( isNullOrEmpty( inputOpt.value ) )
                    {
                        filtOpt.placeholder = propertyName;
                        if(filtOpt.placeholder !== $subfieldsOptions.find(':selected').text() )
                        {
                            filtOpt.placeholder += "'s " +  $subfieldsOptions.find(':selected').text();
                        }  
                    }
                    var $filtValInput = $propFilterValueCell.CswInput('init', {ID: filtValInputId,
                                                                                type: CswInput_Types.text,
                                                                                cssclass: ViewBuilder_CssClasses.filter_value.name,
                                                                                value: inputOpt.value,
                                                                                placeholder: inputOpt.placeholder,
                                                                                width: "200px",
                                                                                autofocus: filtOpt.autoFocusInput,
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
                nodetypeorobjectclassid: '',
                relatedidtype: '',
                fieldtype: $thisProp.CswAttrXml('fieldtype'),
                ID: '',
                $parent: '',
                proparbitraryid: '',
                filtarbitraryid: $thisProp.CswAttrXml('filtarbitraryid'),
                viewbuilderpropid: ''
            };
            if(options) $.extend(o,options);

            var filtOpt = {
                'proparbitraryid': o.proparbitraryid,
                'filtarbitraryid': o.filtarbitraryid,
                'viewbuilderpropid': o.viewbuilderpropid,
                'ID': o.ID
            };

            var filtValInputId = makePropFilterId('propfilter_input', filtOpt);
            var subFieldId = makePropFilterId('subfield_select',filtOpt);
            var filterId = makePropFilterId('filter_select',filtOpt);

            var thisNodeProp = {}; //to return
            
            var $filtInput = o.$parent.find('#' + filtValInputId);
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
                    filterValue = $filtInput.find(':selected').val();
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
                var $subField = o.$parent.find('#' + subFieldId);
				var subFieldText = $subField.find(':selected').text();

                var $filter = o.$parent.find('#' + filterId);
                var filterText = $filter.find(':selected').val();

                var propType = $thisProp.CswAttrXml('proptype');
                                
                thisNodeProp = {
                    nodetypeorobjectclassid: o.nodetypeorobjectclassid, // for NodeType filters
                    relatedidtype: o.relatedidtype, // for NodeType filters
                    proptype: propType,
                    viewbuilderpropid: o.viewbuilderpropid,
                    filtarbitraryid: o.filtarbitraryid,
                    proparbitraryid: o.proparbitraryid,
                    relatedidtype: o.relatedidtype,
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

            var dataXml = {
                ViewXml: o.viewxml,
                PropFiltJson: o.filtJson
            };

            CswAjaxXml({ 
			'url': o.url,
			'data': dataXml,
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


