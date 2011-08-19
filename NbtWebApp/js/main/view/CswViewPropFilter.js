/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../controls/CswSelect.js" />

; (function ($) { /// <param name="$" type="jQuery" />
	
	var pluginName = "CswViewPropFilter";

	function makePropFilterId(id, options)
	{
		var filterId = '';
		var delimiter = '_';
		var o = {
			proparbitraryid: '',
			filtarbitraryid: '',
			viewbuilderpropid: '',
			ID: ''
		};
		if(options) $.extend(o,options);
		
		if (!isNullOrEmpty(o.filtarbitraryid)) {
			filterId = makeId({ ID: id + delimiter + 'filtarbitraryid', 
								prefix: o.ID, 
								suffix: o.filtarbitraryid });
		} 
		else if (!isNullOrEmpty( o.viewbuilderpropid)) {
			filterId = makeId({ ID: id + delimiter + 'viewbuilderpropid', 
								prefix: o.ID, 
								suffix: o.viewbuilderpropid });
		}
		else if(!isNullOrEmpty(  o.proparbitraryid ) ) {
			filterId = makeId({ ID: id + delimiter + 'proparbitraryid', 
								prefix: o.ID, 
								suffix: o.proparbitraryid });
		}
		else if( !isNullOrEmpty( o.ID ) ) {
			filterId = makeId({ ID: id, 
								prefix: o.ID });
		} else {
			filterId = id;
		}
		return filterId;
	}

	var methods = {

		'init': function(options) 
		{
			var o = { 
				//options
				viewid: '',
				viewJson: '',
				propsData: '',
				proparbitraryid: '',
				filtarbitraryid: '',
				viewbuilderpropid: '',
				ID: '',
				propRow: 1,
				firstColumn: 3,
				includePropertyName: false,
				advancedIsHidden: false,

				selectedSubfieldVal: '',
				selectedFilterVal: '',
				autoFocusInput: false
			};
		
			if(options) $.extend(o, options);
		
			var $propFilterTable = $(this); //must call on a table
			
			if ( isNullOrEmpty( o.propsData ) && !isNullOrEmpty( o.proparbitraryid ) )
			{
				var jsonData = {
					ViewJson: JSON.stringify(o.viewJson),
					PropArbitraryId: o.proparbitraryid
				};

				CswAjaxJson({ 
					url: '/NbtWebApp/wsNBT.asmx/getViewPropFilterUI',
					data: jsonData,
					success: function(data) { 
					    if (debugOn()) {
					        log('CswViewPropFilter_init:');
					        log(data);
					    }
						o.propsData = data.propertyfilters;
						//o.filtarbitraryid = o.propsData.filtarbitraryid;
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
			    var propsData = filtOpt.propsData;
			    var propertyId = propsData.viewbuilderpropid;
				var propertyName = propsData.propname;
				
				if( filtOpt.includePropertyName )
				{
					//Row propRow, Column 3: property
					var $propSelectCell = $propFilterTable.CswTable('cell', filtOpt.propRow, filtOpt.firstColumn) //3
														  .empty();
					var propCellId = makePropFilterId(propertyName,filtOpt);
					var $props = $propSelectCell.CswSpan('init',{ID: propCellId, value: propertyName});
				}
				
				var fieldtype = filtOpt.propsData.fieldtype;
				var $defaultFilter = filtOpt.propsData.defaultsubfield.filter;
				
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
				var $subfieldsOptions = $(xmlToString(filtOpt.propsData.subfields.select))
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
											renderPropFiltRow(filtOpt) 
										 });

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

			    var subfieldOpt = findObject(filtOpt.propsData.propertyfilters.subfield, 'column', subfield);
			    var subfieldAry = [];
			    for (var sub in subfieldOpt) {
			        if (subfieldOpt.hasOwnProperty(sub)) {
			            subfieldAry.push({value: sub, display: subfieldOpt[sub] });
			        }
			    }

			    var $filtersOptions = $filtersCell.CswSelect('init', {ID: filtersOptionsId,
			                                                                values: subfieldAry,
			                                                                selected: '',
			                                                                cssclass: ViewBuilder_CssClasses.filter_select.name,
			                                                                onChange: function() {
			                                                                    var $this = $(this);
			                                                                    var r = {
			                                                                        'selectedSubfieldVal': $subfieldsOptions.val(),
			                                                                        'selectedFilterVal': $this.val(),
			                                                                        'advancedIsHidden': isTrue($this.is(':hidden'))
			                                                                    };
			                                                                    $.extend(filtOpt, r);
			                                                                    renderPropFiltRow(filtOpt);
			                                                                }
			                                                            });
 
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
				var $filtValInput;
				if( fieldtype === 'List' )
				{
				    var filtValOpt = filtOpt.propsData.filtersoptions.select;
				    var filtValAry = [];
				    for (var filt in filtValOpt) {
				        if(filtValOpt.hasOwnProperty(filt)) {
				            filtValAry.push({value: filt, display: filtValOpt[filt] });
				        }
				    }
				    $filtValInput = $propFilterValueCell.CswSelect('init', {ID: filtValInputId,
				                                                            values: filtValAry
				    });
    					
				}
				else if( fieldtype === 'Logical' )
				{
					$filtValInput = $propFilterValueCell.CswTristateCheckBox('init',{'ID': filtValInputId, 'Checked': defaultValue}); 
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
					$filtValInput = $propFilterValueCell.CswInput('init', {ID: filtValInputId,
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
				viewbuilderpropid: '',
				allowNullFilterValue: false
			};
			if(options) $.extend(o,options);

			var filtOpt = {
				proparbitraryid: o.proparbitraryid,
				filtarbitraryid: o.filtarbitraryid,
				viewbuilderpropid: o.viewbuilderpropid,
				ID: o.ID
			};

			var filtValInputId = makePropFilterId('propfilter_input', filtOpt);
			var subFieldId = makePropFilterId('subfield_select',filtOpt);
			var filterId = makePropFilterId('filter_select',filtOpt);

			var thisNodeProp = {}; //to return
			
			var $filtInput = tryParseElement(filtValInputId, o.$parent); //o.$parent.find('#' + filtValInputId);
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
			if(filterValue !== '' || o.allowNullFilterValue)
			{
				var $subField = tryParseElement(subFieldId, o.$parent); //o.$parent.find('#' + subFieldId);
				var subFieldText = $subField.find(':selected').text();

				var $filter = tryParseElement(filterId,o.$parent) //o.$parent.find('#' + filterId);
				var filterText = $filter.find(':selected').val();

				var propType = $thisProp.CswAttrXml('proptype');
								
				thisNodeProp = {
					nodetypeorobjectclassid: o.nodetypeorobjectclassid, // for NodeType filters
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
				viewJson: '',
				filtJson: '',
				onSuccess: function($filterXml) {}
			};
			if(options) $.extend(o,options);

			var jsonData = {
				PropFiltJson: JSON.stringify(o.filtJson),
				ViewJson: JSON.stringify(o.viewJson)
			};

			CswAjaxJson({ 
			    url: '/NbtWebApp/wsNBT.asmx/makeViewPropFilter',
			    data: jsonData,
			    success: function(data) { 
					    o.onSuccess(data);
				    }
			});
		}, // 'makefilter': function(options)
		'bindToButton': function()
		{
			var $button = $(this);

			if( !isNullOrEmpty($button) )
			{
				$('.' + ViewBuilder_CssClasses.subfield_select.name).each(function() { 
					var $input = $(this);
					$input.clickOnEnter($button);
				});
				$('.' + ViewBuilder_CssClasses.filter_select.name).each(function() { 
					var $input = $(this);
					$input.clickOnEnter($button);
				});
				$('.' + ViewBuilder_CssClasses.default_filter.name).each(function() { 
					var $input = $(this);
					$input.clickOnEnter($button);
				});                       
				$('.' + ViewBuilder_CssClasses.filter_value.name).each(function() { 
					var $input = $(this);
					$input.clickOnEnter($button);
				});
			}
			return $button;            
		} // 'bindToButton': function(options)
	} // methods 
	 
	$.fn.CswViewPropFilter = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
		}    
  
	};
})(jQuery);


