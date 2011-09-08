/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../controls/CswSelect.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeMultiList';
    
    var methods = {
        init: function(o) {

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var value = (false === o.Multi) ? tryParseString(propVals.value).trim() : CswMultiEditDefaultValue;
            var gestalt = tryParseString(o.propData.gestalt).trim();
            var options = propVals.options;
            
            if (o.ReadOnly) {
                $Div.append(gestalt);
            } else {
                var values = ['Select...'];
                var selected = 'Select...';
                if(o.Multi) {
                    values.push(CswMultiEditDefaultValue);
                    selected = CswMultiEditDefaultValue;
                }
                var $SelectBox = $Div.CswSelect('init', {
                                        ID: o.ID,
                                        cssclass: 'selectinput',
                                        values: values,
                                        selected: selected,
                                        onChange: function() { _handleOnChange(); }
                                     }); 

                var $ValueTableDiv = $('<div />')
									.appendTo($Div)
									.css('padding', '2px')
									.css('max-height', '110px')
									.css('border', '1px solid #336699')
									.css('overflow', 'auto');

				var $ValueTable = $ValueTableDiv.CswTable({
										ID: o.ID + '_valtbl'
									});

				var $HiddenValue = $('<input type="hidden" name="' + o.ID + '_value" id="' + o.ID + '_value" value="'+ value +'"/>')
                                    .appendTo($Div);


				// ClosureCompiler broke if I didn't define these functions first
				function _handleOnChange() {
					var optionvalue = $SelectBox.children('option:selected').CswAttrDom('value');
					var optiontext = $SelectBox.children('option:selected').text();

					if(!isNullOrEmpty(optionvalue))
					{
						$SelectBox.val('');

						var currentvalue = $HiddenValue.val();
						if(!isNullOrEmpty(currentvalue)) currentvalue += ',';
						$HiddenValue.val(currentvalue + optionvalue);

						_addValue(optionvalue, optiontext, true);

						o.onchange();
					}
				} // _handleOnChange()

				function _addValue(optionvalue, optiontext, doAnimation) {
					$SelectBox.children('option[value="'+ optionvalue +'"]').remove();

					$ValueTableDiv.show();
					var row = $ValueTable.CswTable('maxrows') + 1;
					var $cell1 = $ValueTable.CswTable('cell', row, '1');
					var $cell2 = $ValueTable.CswTable('cell', row, '2');
					$cell1.css('padding-right', '20px');

					if(doAnimation) $cell1.parent().hide();
					var $ThisValue = $('<div id="val_'+ optionvalue +'">'+ optiontext + '</div>')
										.appendTo( $cell1 );
					if(doAnimation) $cell1.parent().fadeIn('fast');

					$cell2.CswImageButton({
						ButtonType: CswImageButton_ButtonType.Delete,
						AlternateText: 'Remove',
						ID: makeId({ 'prefix': optionvalue, 'id': 'rembtn' }),
						onClick: function ($ImageDiv) { 
							$cell1.parent().fadeOut('fast', function() { 
								$SelectBox.append('<option value="' + optionvalue + '">'+ optiontext + '</option>');

								var currentvalue = $HiddenValue.val();
								var splitvalue = currentvalue.split(',');
								var newvalue = '';
								for(var i = 0; i < splitvalue.length; i++)
								{
									if(splitvalue[i] != optionvalue)
									{
										if(!isNullOrEmpty(newvalue)) newvalue += ',';
										newvalue += splitvalue[i];
									}
								}
								$HiddenValue.val(newvalue);
								if(isNullOrEmpty(newvalue))
								{
									$ValueTableDiv.hide();
								}
								$ThisValue.remove();
							});
							o.onchange();
							return CswImageButton_ButtonType.None; 
						}
					});
				} // _addValue()

//				var SplitOptions = Options.split(',');
//                for(i = 0; i < SplitOptions.length; i++)
//                {
//                    $SelectBox.append('<option value="' + SplitOptions[i] + '">' + SplitOptions[i] + '</option>');
//                }

                crawlObject(options, function(thisOption, key) {
                        if (isTrue(thisOption.selected) && false === o.Multi ) {
                            _addValue(thisOption.value, thisOption.text, false);
                        } else {
                            $SelectBox.append('<option value="' + thisOption.value + '">' + thisOption.text + '</option>');
                        }
                }, false);

                if(isNullOrEmpty(value)) {
					$ValueTableDiv.hide();
				}

//                if(o.Required)
//                {
//                    $SelectBox.addClass("required");
//                }

            } // if-else(o.ReadOnly)
        },
        save: function(o) { //$propdiv, $xml
            var $HiddenValue = o.$propdiv.find('#' + o.ID + '_value');
            if (false === o.Multi || $HiddenValue.val() !== CswMultiEditDefaultValue) {
                o.propData.values.value = $HiddenValue.val();
            } else {
                delete o.propData;
            }
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeMultiList = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
